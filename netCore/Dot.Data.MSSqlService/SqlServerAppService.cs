using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Dot.Data.MSSqlService
{

    [Export(typeof(ISqlHelper), Key = "System.Data.SqlClient")]
    public class SqlServerAppService : ISqlHelper
    {

        public int ExecuteNonQuery(string connectionString, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {

            return SqlServerHelper.ExecuteNonQuery(connectionString, commandType, commandText, Convert(commandParameters));
        }

        public int ExecuteNonQuery(DbConnection connection, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteNonQuery(connection as SqlConnection, commandType, commandText, Convert(commandParameters));
        }

        public int ExecuteNonQuery(DbTransaction transaction, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteNonQuery(transaction as SqlTransaction, commandType, commandText, Convert(commandParameters));
        }


        public System.Data.DataSet ExecuteDataset(string connectionString, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteDataset(connectionString, commandType, commandText, Convert(commandParameters));
        }

        public System.Data.DataSet ExecuteDataset(DbConnection connection, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteDataset(connection as SqlConnection, commandType, commandText, Convert(commandParameters));
        }

        public DataSet GetPaging(string connectionString, string commandText, int pageIndex, int pageSize, string orderby, out long total, out long pageCount, params DbParameter[] commandParameters)
        {
            int start = 1;
            int end = 10;
            if (pageIndex > 0 && pageSize > 0)
            {
                start = pageSize * (pageIndex - 1) + 1;
                end = pageSize * ((pageIndex - 1) + 1);
            }
            Regex order = new Regex("-(asc|desc)$", RegexOptions.IgnoreCase);
            if (order.IsMatch(orderby))
            {
                orderby = order.Replace(orderby, " $1");
            }
            string sql = string.Format(@"
                     SELECT  *  FROM 
                     ( SELECT  ROW_NUMBER()  OVER  ( ORDER BY {1} )  AS RowNumber,  *  
                     FROM 
                     ({0} ) A_
                     )
                     AS A1_
                     WHERE RowNumber BETWEEN  {2} AND {3} ORDER BY {1}  ;"
                     , commandText, orderby, start, end
                );
            string sqlTotal = string.Format(@"SELECT count(*)  FROM ({0} ) A_;", commandText, orderby, start, end);
            long.TryParse(SqlServerHelper.ExecuteScalar(connectionString, CommandType.Text, sqlTotal).ToString(), out total);
            pageCount = long.Parse((total / pageSize).ToString());
            pageCount = (total % pageSize != 0) ? pageCount + 1 : pageCount;
            return SqlServerHelper.ExecuteDataset(connectionString, CommandType.Text, sql, Convert(commandParameters));
        }
        public DataSet GetPaging(DbConnection connection, string commandText, int pageIndex, int pageSize, string orderby, out long total, out long pageCount, params DbParameter[] commandParameters)
        {
            return GetPaging(connection.ConnectionString, commandText, pageIndex, pageSize, orderby, out total, out pageCount, Convert(commandParameters));
        }


        public System.Data.DataSet ExecuteDataset(DbTransaction transaction, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteDataset(transaction as SqlTransaction, commandType, commandText, Convert(commandParameters));
        }

        public DbDataReader ExecuteReader(string connectionString, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteReader(connectionString, commandType, commandText, Convert(commandParameters));
        }

        public DbDataReader ExecuteReader(DbConnection connection, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteReader(connection as SqlConnection, commandType, commandText, Convert(commandParameters));
        }

        public DbDataReader ExecuteReader(DbTransaction transaction, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteReader(transaction as SqlTransaction, commandType, commandText, Convert(commandParameters));
        }

        public object ExecuteScalar(string connectionString, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteReader(connectionString, commandType, commandText, Convert(commandParameters));
        }

        public object ExecuteScalar(DbConnection connection, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteReader(connection as SqlConnection, commandType, commandText, Convert(commandParameters));
        }

        public object ExecuteScalar(DbTransaction transaction, System.Data.CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return SqlServerHelper.ExecuteScalar(transaction as SqlTransaction, commandType, commandText, Convert(commandParameters));
        }

        public void FillDataset(string connectionString, System.Data.CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            SqlServerHelper.FillDataset(connectionString, commandType, commandText, dataSet, tableNames, Convert(commandParameters));
        }

        public void FillDataset(DbConnection connection, System.Data.CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            SqlServerHelper.FillDataset(connection as SqlConnection, commandType, commandText, dataSet, tableNames, Convert(commandParameters));
        }

     

        SqlParameter[] Convert(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
                return null;
            List<SqlParameter> p = new List<SqlParameter>();
            foreach (SqlParameter item in commandParameters)
            {
                p.Add(item);
            }
            return p.ToArray();
        }

        public DbConnection GetDbConnection(string con)
        {
            DbConnection dbCon = new SqlConnection(con);
            return dbCon;
        }

        public DbParameter GetParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public string GetPrefixPar()
        {
            return "@";
        }


    }
}
