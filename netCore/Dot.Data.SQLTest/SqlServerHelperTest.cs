using Dot.Data.MSSqlService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Data.SQLTest
{
    [TestClass]
    public class SqlServerHelperTest
    {
        [TestMethod]
        public void SqlBulkCopyByDatatableTest()
        {
            string connecCon = "Data Source=.;Initial Catalog=CoreAIS_SourceDB;User ID=sa;Password=pass@word1;MultipleActiveResultSets=true;";
            string targetCon = "Data Source=.;Initial Catalog=CoreAIS_DBImportDB;User ID=sa;Password=pass@word1;MultipleActiveResultSets=true;";
            string sql = @"SELECT [OrgCode]
                            ,[OrgName],[ParentId],str([OrgState]) OrgState,getdate() ImportedDate
                            FROM TestData_Init_Organizations o  ";
            SqlServerHelper.SqlBulkCopyByDatatable(connecCon, targetCon, "Import_Organizations", sql);
        }
    }
}
