
using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Dot.Utility.Database
{
    /// <summary> 
    /// SqlServer数据访问帮助类 
    /// </summary> 
    public sealed class OleDbHelper
    {
        #region 私有构造函数和方法

        private OleDbHelper()
        {
        }

        /// <summary> 
        /// 将OleDbParameter参数数组(参数值)分配给OleDbCommand命令. 
        /// 这个方法将给任何一个参数分配DBNull.Value; 
        /// 该操作将阻止默认值的使用. 
        /// </summary> 
        /// <param name="command">命令名</param> 
        /// <param name="commandParameters">OleDbParameters数组</param> 
        private static void AttachParameters(OleDbCommand command, OleDbParameter[] commandParameters)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (OleDbParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value. 
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary> 
        /// 将DataRow类型的列值分配到OleDbParameter参数数组. 
        /// </summary> 
        /// <param name="commandParameters">要分配值的OleDbParameter参数数组</param> 
        /// <param name="dataRow">将要分配给存储过程参数的DataRow</param> 
        private static void AssignParameterValues(OleDbParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                return;
            }

            int i = 0;
            // 设置参数值 
            foreach (OleDbParameter commandParameter in commandParameters)
            {
                // 创建参数名称,如果不存在,只抛出一个异常. 
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format("请提供参数{0}一个有效的名称{1}.", i, commandParameter.ParameterName));
                // 从dataRow的表中获取为参数数组中数组名称的列的索引. 
                // 如果存在和参数名称相同的列,则将列值赋给当前名称的参数. 
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary> 
        /// 将一个对象数组分配给OleDbParameter参数数组. 
        /// </summary> 
        /// <param name="commandParameters">要分配值的OleDbParameter参数数组</param> 
        /// <param name="parameterValues">将要分配给存储过程参数的对象数组</param> 
        private static void AssignParameterValues(OleDbParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }

            // 确保对象数组个数与参数个数匹配,如果不匹配,抛出一个异常. 
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("参数值个数与参数不匹配.");
            }

            // 给参数赋值 
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property 
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary> 
        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数 
        /// </summary> 
        /// <param name="command">要处理的OleDbCommand</param> 
        /// <param name="connection">数据库连接</param> 
        /// <param name="transaction">一个有效的事务或者是null值</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名或都T-SQL命令文本</param> 
        /// <param name="commandParameters">和命令相关联的OleDbParameter参数数组,如果没有参数为'null'</param> 
        /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param> 
        private static void PrepareCommand(OleDbCommand command, OleDbConnection connection, OleDbTransaction transaction, CommandType commandType, string commandText, OleDbParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it 
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // 给命令分配一个数据库连接. 
            command.Connection = connection;

            // 设置命令文本(存储过程名或SQL语句) 
            command.CommandText = commandText;

            // 分配事务 
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // 设置命令类型. 
            command.CommandType = commandType;

            // 分配命令参数 
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion 私有构造函数和方法结束


        #region ExecuteNonQuery命令

        /// <summary> 
        /// 执行指定连接字符串,类型的OleDbCommand. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders"); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名称或SQL语句</param> 
        /// <returns>返回命令影响的行数</returns> 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定连接字符串,类型的OleDbCommand.如果没有提供参数,不返回结果. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名称或SQL语句</param> 
        /// <param name="commandParameters">OleDbParameter参数数组</param> 
        /// <returns>返回命令影响的行数</returns> 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary> 
        /// 执行指定连接字符串的存储过程,将对象数组的值赋给存储过程参数, 
        /// 此方法需要在参数缓存方法中探索参数并生成参数. 
        /// </summary> 
        /// <remarks> 
        /// 这个方法没有提供访问输出参数和返回值. 
        /// 示例:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串/param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="parameterValues">分配到存储过程输入参数的对象数组</param> 
        /// <returns>返回受影响的行数</returns> 
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果存在参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从探索存储过程参数(加载到缓存)并分配给存储过程参数数组. 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数情况下 
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
        /// <param name="commandText">T存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // 创建OleDbCommand命令,并进行预处理 
            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command 
            int retval = cmd.ExecuteNonQuery();

            // 清除参数,以便再次使用. 
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值 
        /// 示例:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQuery(OleDbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 给存储过程分配参数值 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行带事务的OleDbCommand. 
        /// </summary> 
        /// <remarks> 
        /// 示例.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders"); 
        /// </remarks> 
        /// <param name="transaction">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回影响的行数/returns> 
        public static int ExecuteNonQuery(OleDbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行带事务的OleDbCommand(指定参数). 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQuery(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理 
            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行 
            int retval = cmd.ExecuteNonQuery();

            // 清除参数集,以便再次使用. 
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary> 
        /// 执行带事务的OleDbCommand(指定参数值). 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值 
        /// 示例:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36); 
        /// </remarks> 
        /// <param name="transaction">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回受影响的行数</returns> 
        public static int ExecuteNonQuery(OleDbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值 
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery方法结束

        #region ExecuteDataset方法

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例: 
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamters参数数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");

            // 创建并打开数据库连接对象,操作完成释放对象. 
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 调用指定数据库连接字符串重载方法. 
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值. 
        /// 示例: 
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中检索存储过程参数 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 给存储过程参数分配值 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // 预处理 
            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建OleDbDataAdapter和DataSet. 
            using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // 填充DataSet. 
                da.Fill(ds);

                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                return ds;
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定参数值,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输入参数和返回值. 
        /// 示例.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 比缓存中加载存储过程参数 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 给存储过程参数分配值 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定事务的命令,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定事务的命令,指定参数,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理 
            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建 DataAdapter & DataSet 
            using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        /// <summary> 
        /// 执行指定事务的命令,指定参数值,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输入参数和返回值. 
        /// 示例.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(OleDbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数分配值 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataset数据集命令结束

        #region ExecuteReader 数据阅读器

        /// <summary> 
        /// 枚举,标识数据库连接是由SqlHelper提供还是由调用者提供 
        /// </summary> 
        private enum OleDbConnectionOwnership
        {
            /// <summary>由SqlHelper提供连接</summary> 
            Internal,
            /// <summary>由调用者提供连接</summary> 
            External
        }

        /// <summary> 
        /// 执行指定数据库连接对象的数据阅读器. 
        /// </summary> 
        /// <remarks> 
        /// 如果是SqlHelper打开连接,当连接关闭DataReader也将关闭. 
        /// 如果是调用都打开连接,DataReader由调用都管理. 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="transaction">一个有效的事务,或者为 'null'</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">OleDbParameters参数数组,如果没有参数则为'null'</param> 
        /// <param name="connectionOwnership">标识数据库连接对象是由调用者提供还是由SqlHelper提供</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        private static OleDbDataReader ExecuteReader(OleDbConnection connection, OleDbTransaction transaction, CommandType commandType, string commandText, OleDbParameter[] commandParameters, OleDbConnectionOwnership connectionOwnership)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // 创建命令 
            OleDbCommand cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // 创建数据阅读器 
                OleDbDataReader dataReader;

                if (connectionOwnership == OleDbConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // 清除参数,以便再次使用.. 
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command 
                // then the SqlReader can磘 set its values. 
                // When this happen, the parameters can磘 be used again in other command. 
                bool canClear = true;
                foreach (OleDbParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的数据阅读器. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的数据阅读器,指定参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组(new OleDbParameter("@prodid", 24))</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            OleDbConnection connection = null;
            try
            {
                connection = new OleDbConnection(connectionString);
                connection.Open();

                return ExecuteReader(connection, null, commandType, commandText, commandParameters, OleDbConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves 
                if (connection != null)
                    connection.Close();
                throw;
            }

        }

        /// <summary> 
        /// 执行指定数据库连接字符串的数据阅读器,指定参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的数据阅读器. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteReader(connection, (OleDbTransaction)null, commandType, commandText, commandParameters, OleDbConnectionOwnership.External);
        }

        /// <summary> 
        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">T存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///   OleDbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, OleDbConnectionOwnership.External);
        }

        /// <summary> 
        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  OleDbDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值 
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader数据阅读器

        #region ExecuteScalar 返回结果集中的第一行第一列

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法 
            return ExecuteScalar(connectionString, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,指定参数,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            // 创建并打开数据库连接对象,操作完成释放对象. 
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 调用指定数据库连接字符串重载方法. 
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,指定参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值 
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法 
            return ExecuteScalar(connection, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // 创建OleDbCommand命令,并进行预处理 
            OleDbCommand cmd = new OleDbCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行OleDbCommand命令,并返回结果. 
            object retval = cmd.ExecuteScalar();

            // 清除参数,以便再次使用. 
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值 
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount"); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbTransaction transaction, CommandType commandType, string commandText)
        {
            // 执行参数为空的方法 
            return ExecuteScalar(transaction, commandType, commandText, (OleDbParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 创建OleDbCommand命令,并进行预处理 
            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行OleDbCommand命令,并返回结果. 
            object retval = cmd.ExecuteScalar();

            // 清除参数,以便再次使用. 
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,指定参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalar(OleDbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // PPull the parameters for this stored procedure from the parameter cache () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // 没有参数值 
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar
        
        #region FillDataset 填充数据集
        /// <summary> 
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.)</param> 
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            // 创建并打开数据库连接对象,操作完成释放对象. 
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 调用指定数据库连接字符串重载方法. 
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.指定命令参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        public static void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象. 
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 调用指定数据库连接字符串重载方法. 
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集,指定存储过程参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param>    
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        public static void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            // 创建并打开数据库连接对象,操作完成释放对象. 
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 调用指定数据库连接字符串重载方法. 
                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param>    
        public static void FillDataset(OleDbConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        public static void FillDataset(OleDbConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params OleDbParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定存储过程参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        public static void FillDataset(OleDbConnection connection, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // 没有参数值 
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,映射数据表并填充数据集. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        public static void FillDataset(OleDbTransaction transaction, CommandType commandType,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        public static void FillDataset(OleDbTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params OleDbParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary> 
        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定存储过程参数值. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值参数. 
        /// 
        /// 示例:  
        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36); 
        /// </remarks> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        public static void FillDataset(OleDbTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果有参数值 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 给存储过程参数赋值 
                AssignParameterValues(commandParameters, parameterValues);

                // 调用重载方法 
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // 没有参数值 
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary> 
        /// [私有方法][内部调用]执行指定数据库连接对象/事务的命令,映射数据表并填充数据集,DataSet/TableNames/OleDbParameters. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OleDbParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="transaction">一个有效的连接事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
        /// <param name="tableNames">表映射的数据表数组 
        /// 用户定义的表名 (可有是实际的表名.) 
        /// </param> 
        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
        private static void FillDataset(OleDbConnection connection, OleDbTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params OleDbParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            // 创建OleDbCommand命令,并进行预处理 
            OleDbCommand command = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行命令 
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(command))
            {

                // 追加表映射 
                if (tableNames != null && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0)
                            throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                // 填充数据集使用默认表名称 
                dataAdapter.Fill(dataSet);

                // 清除参数,以便再次使用. 
                command.Parameters.Clear();
            }

            if (mustCloseConnection)
                connection.Close();
        }
        #endregion

        #region UpdateDataset 更新数据集
        /// <summary> 
        /// 执行数据集更新到数据库,指定inserted, updated, or deleted命令. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order"); 
        /// </remarks> 
        /// <param name="insertCommand">[追加记录]一个有效的T-SQL语句或存储过程</param> 
        /// <param name="deleteCommand">[删除记录]一个有效的T-SQL语句或存储过程</param> 
        /// <param name="updateCommand">[更新记录]一个有效的T-SQL语句或存储过程</param> 
        /// <param name="dataSet">要更新到数据库的DataSet</param> 
        /// <param name="tableName">要更新到数据库的DataTable</param> 
        public static void UpdateDataset(OleDbCommand insertCommand, OleDbCommand deleteCommand, OleDbCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null)
                throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null)
                throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null)
                throw new ArgumentNullException("updateCommand");
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");

            // 创建OleDbDataAdapter,当操作完成后释放. 
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter())
            {
                // 设置数据适配器命令 
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // 更新数据集改变到数据库 
                dataAdapter.Update(dataSet, tableName);

                // 提交所有改变到数据集. 
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand 创建一条OleDbCommand命令
        /// <summary> 
        /// 创建OleDbCommand命令,指定数据库连接对象,存储过程名和参数. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  OleDbCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="sourceColumns">源表的列名称数组</param> 
        /// <returns>返回OleDbCommand命令</returns> 
        public static OleDbCommand CreateCommand(OleDbConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 创建命令 
            OleDbCommand cmd = new OleDbCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // 如果有参数值 
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 将源表的列到映射到DataSet命令中. 
                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the OleDbCommand object 
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }
        #endregion

        #region ExecuteNonQueryTypedParams 类型化参数(DataRow)
        /// <summary> 
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回受影响的行数. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回受影响的行数. 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQueryTypedParams(OleDbConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回受影响的行数. 
        /// </summary> 
        /// <param name="transaction">一个有效的连接事务 object</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回影响的行数</returns> 
        public static int ExecuteNonQueryTypedParams(OleDbTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // Sf the row has values, the store procedure parameters must be initialized 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteDatasetTypedParams 类型化参数(DataRow)
        /// <summary> 
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回一个包含结果集的DataSet.</returns> 
        public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            //如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet. 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回一个包含结果集的DataSet.</returns> 
        /// 
        public static DataSet ExecuteDatasetTypedParams(OleDbConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet. 
        /// </summary> 
        /// <param name="transaction">一个有效的连接事务 object</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回一个包含结果集的DataSet.</returns> 
        public static DataSet ExecuteDatasetTypedParams(OleDbTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region ExecuteReaderTypedParams 类型化参数(DataRow)
        /// <summary> 
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataReader. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary> 
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataReader. 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReaderTypedParams(OleDbConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回DataReader. 
        /// </summary> 
        /// <param name="transaction">一个有效的连接事务 object</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回包含结果集的OleDbDataReader</returns> 
        public static OleDbDataReader ExecuteReaderTypedParams(OleDbTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteScalarTypedParams 类型化参数(DataRow)
        /// <summary> 
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalarTypedParams(OleDbConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary> 
        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
        /// </summary> 
        /// <param name="transaction">一个有效的连接事务 object</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="dataRow">使用DataRow作为参数值</param> 
        /// <returns>返回结果集中的第一行第一列</returns> 
        public static object ExecuteScalarTypedParams(OleDbTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            // 如果row有值,存储过程必须初始化. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
                OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // 分配参数值 
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion
        
        /// <summary>  
        /// 利用反射和泛型  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static IList<T> ConvertToList<T>(DataTable dt) where T : class,new()
        {

            // 定义集合  
            IList<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite)
                            continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }

            return ts;
        }
    }

    /// <summary> 
    /// OleDbHelperParameterCache提供缓存存储过程参数,并能够在运行时从存储过程中探索参数. 
    /// </summary> 
    public sealed class OleDbHelperParameterCache
    {
        #region 私有方法,字段,构造函数
        // 私有构造函数,妨止类被实例化. 
        private OleDbHelperParameterCache()
        {
        }

        // 这个方法要注意 
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary> 
        /// 探索运行时的存储过程,返回OleDbParameter参数数组. 
        /// 初始化参数值为 DBNull.Value. 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接</param> 
        /// <param name="spName">存储过程名称</param> 
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        private static OleDbParameter[] DiscoverSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            OleDbCommand cmd = new OleDbCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            // 检索cmd指定的存储过程的参数信息,并填充到cmd的Parameters参数集中. 
            OleDbCommandBuilder.DeriveParameters(cmd);
            connection.Close();
            // 如果不包含返回值参数,将参数集中的每一个参数删除. 
            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            // 创建参数数组 
            OleDbParameter[] discoveredParameters = new OleDbParameter[cmd.Parameters.Count];
            // 将cmd的Parameters参数集复制到discoveredParameters数组. 
            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // 初始化参数值为 DBNull.Value. 
            foreach (OleDbParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary> 
        /// OleDbParameter参数数组的深层拷贝. 
        /// </summary> 
        /// <param name="originalParameters">原始参数数组</param> 
        /// <returns>返回一个同样的参数数组</returns> 
        private static OleDbParameter[] CloneParameters(OleDbParameter[] originalParameters)
        {
            OleDbParameter[] clonedParameters = new OleDbParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (OleDbParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion 私有方法,字段,构造函数结束

        #region 缓存方法

        /// <summary> 
        /// 追加参数数组到缓存. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandText">存储过程名或SQL语句</param> 
        /// <param name="commandParameters">要缓存的参数数组</param> 
        public static void CacheParameterSet(string connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary> 
        /// 从缓存中获取参数数组. 
        /// </summary> 
        /// <param name="connectionString">一个有效的数据库连接字符</param> 
        /// <param name="commandText">存储过程名或SQL语句</param> 
        /// <returns>参数数组</returns> 
        public static OleDbParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            OleDbParameter[] cachedParameters = paramCache[hashKey] as OleDbParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion 缓存方法结束

        #region 检索指定的存储过程的参数集

        /// <summary> 
        /// 返回指定的存储过程的参数集 
        /// </summary> 
        /// <remarks> 
        /// 这个方法将查询数据库,并将信息存储到缓存. 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符</param> 
        /// <param name="spName">存储过程名</param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary> 
        /// 返回指定的存储过程的参数集 
        /// </summary> 
        /// <remarks> 
        /// 这个方法将查询数据库,并将信息存储到缓存. 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符.</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0)
                throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary> 
        /// [内部]返回指定的存储过程的参数集(使用连接对象). 
        /// </summary> 
        /// <remarks> 
        /// 这个方法将查询数据库,并将信息存储到缓存. 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接字符</param> 
        /// <param name="spName">存储过程名</param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary> 
        /// [内部]返回指定的存储过程的参数集(使用连接对象) 
        /// </summary> 
        /// <remarks> 
        /// 这个方法将查询数据库,并将信息存储到缓存. 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="includeReturnValueParameter"> 
        /// 是否包含返回值参数 
        /// </param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            using (OleDbConnection clonedConnection = (OleDbConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary> 
        /// [私有]返回指定的存储过程的参数集(使用连接对象) 
        /// </summary> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
        /// <returns>返回OleDbParameter参数数组</returns> 
        private static OleDbParameter[] GetSpParameterSetInternal(OleDbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0)
                throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            OleDbParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as OleDbParameter[];
            if (cachedParameters == null)
            {
                OleDbParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion 参数集检索结束
    }

    /// <summary>
    /// 数据库的通用访问代码
    /// 此类为抽象类，不允许实例化，在应用时直接调用即可
    /// </summary>
    public class OleDbHelper1
    {
        //获取数据库连接字符串，其属于静态变量且只读，项目中所有文档可以直接使用，但不能修改
        public static readonly string ConnectionStringLocalTransaction = "Provider=Microsoft.Jet.OleDb.4.0;Data Source= " + System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Yu_ZhiChuang.mdb ") + "; Persist Security Info=True;Jet OLEDB:Database Password=12241226";
        // 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数。
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        ///执行一个不需要返回值的OleDbCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <remarks>
        /// 使用示例：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OleDbCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到OleDbCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                //清空OleDbCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        ///执行一条不返回结果的OleDbCommand，通过一个已经存在的数据库连接 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个现有的数据库连接</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OleDbCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 执行一条不返回结果的OleDbCommand，通过一个已经存在的数据库事物处理 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的 OleDb 事物处理</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OleDbCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 执行一条返回结果集的OleDbCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  OleDbDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的OleDbDataReader</returns>
        public static OleDbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);
            // 在这里使用try/catch处理是因为如果方法出现异常，则OleDbDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 执行一条返回结果集的OleDbCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  OleDbDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的OleDbDataReader</returns>
        public static DataSet ExecuteDateSet(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);
            // 在这里使用try/catch处理是因为如果方法出现异常，则DataSet就不存在，
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                da.Dispose();
                da = null;
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
                //OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //cmd.Parameters.Clear();
                //return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列的OleDbCommand命令，通过专用的连接字符串。 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列的OleDbCommand命令，通过已经存在的数据库连接。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个已经存在的数据库连接</param>
        /// <param name="commandType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-OleDb 语句</param>
        /// <param name="commandParameters">以数组形式提供OleDbCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public static object ExecuteScalar(OleDbConnection connection, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 缓存参数数组
        /// </summary>
        /// <param name="cacheKey">参数缓存的键值</param>
        /// <param name="cmdParms">被缓存的参数列表</param>
        public static void CacheParameters(string cacheKey, params OleDbParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }
        /// <summary>
        /// 获取被缓存的参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的KEY值</param>
        /// <returns>返回缓存的参数数组</returns>
        public static OleDbParameter[] GetCachedParameters(string cacheKey)
        {
            OleDbParameter[] cachedParms = (OleDbParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            //新建一个参数的克隆列表
            OleDbParameter[] clonedParms = new OleDbParameter[cachedParms.Length];
            //通过循环为克隆参数列表赋值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法复制参数列表中的参数
                clonedParms[i] = (OleDbParameter)((ICloneable)cachedParms[i]).Clone();
            return clonedParms;
        }
        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">OleDbCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">OleDbCommand命令类型 (存储过程， T-OleDb语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-OleDb语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}