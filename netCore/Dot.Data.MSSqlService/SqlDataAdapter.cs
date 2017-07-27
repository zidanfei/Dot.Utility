using Dot.Log;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dot.Data.MSSqlService
{
    internal class SqlDataAdapter : IDisposable
    {
        private SqlCommand cmd;

        public SqlDataAdapter(SqlCommand cmd)
        {
            this.cmd = cmd;
        }

        public DataTableMappingCollection TableMappings { get; internal set; }

        public void Dispose()
        {
            this.cmd.Connection.Dispose();
            this.cmd.Dispose();
        }

        internal void Fill(DataSet ds)
        {
            if (this.cmd != null)
            {
                try
                {
                    string spid = String.Empty;
                    string program_name = String.Empty;
                    if (cmd.Connection.State == ConnectionState.Broken || cmd.Connection.State == ConnectionState.Closed)
                        cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    var cols = reader.GetColumnSchema();
                    if (ds == null)
                        ds = new DataSet();
                    DataTable dt = new DataTable();
                    foreach (var item in cols)
                    {
                        //var col = new DataColumn(item.ColumnName, item.DataType);
                        //col.AllowDBNull = true;
                        dt.Columns.Add(item.ColumnName, item.DataType);
                    }
                    while (reader.Read())
                    {
                        var newrow = dt.NewRow();
                        foreach (DataColumn item in dt.Columns)
                        {
                            if (reader[item.ColumnName] != DBNull.Value)
                                newrow[item.ColumnName] = reader[item.ColumnName].ToString();

                        }
                        dt.Rows.Add(newrow);
                    }
                    ds.Tables.Add(dt);
                }
                catch (SqlException ex)
                {
                    LogFactory.ExceptionLog.Error("填充数据失败：", ex);

                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }
    }
}