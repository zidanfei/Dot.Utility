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
            throw new NotImplementedException();
        }

        internal void Fill(DataSet ds)
        {
            throw new NotImplementedException();
        }
    }
}