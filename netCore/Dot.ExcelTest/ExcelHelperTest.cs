using Dot.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Dot.ExcelTest
{
    [TestClass]
    public class ExcelHelperTest
    {
        [TestMethod]
        public void ImportExceltoDtTest()
        {
            DataTable table = ExcelHelper.ImportExceltoDt(DotEnvironment.MapDllPath(@"config\AIS_SyncConfig.xlsx"), "BD_Properties", 0);
            Assert.AreEqual("Type", table.Columns[0].ColumnName);
            Assert.AreEqual("PropertyName", table.Columns[0].ColumnName);
            Assert.AreEqual("Description", table.Columns[0].ColumnName);
            Assert.AreEqual("cnDescription", table.Columns[0].ColumnName);
            Assert.AreEqual("IsEnabled", table.Columns[0].ColumnName);
            Assert.AreEqual("NeedUpdate", table.Columns[0].ColumnName);

        }

        [TestMethod]
        public void ExportDTtoExcelTest()
        {
            string filePath = DotEnvironment.MapAbsolutePath("AIS_SyncConfig.xls");
            Compare2(filePath);
            filePath = DotEnvironment.MapAbsolutePath("AIS_SyncConfig.xlsx");
            Compare2(filePath);


        }



        void Compare2(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Type"));
            dt.Columns.Add(new DataColumn("PropertyName"));
            dt.Columns.Add(new DataColumn("Description"));
            var row = dt.NewRow();
            row["Type"] = "ADUser";
            row["PropertyName"] = "accountExpires";
            row["Description"] = "Account-Expires";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Type"] = "ADUser";
            row["PropertyName"] = "accountNameHistory";
            row["Description"] = "Account-Name-History";
            dt.Rows.Add(row);
            //无表头，无sheet名
            {
                ExcelHelper.ExportDTtoExcel(dt, "", "", filePath);

                DataTable table = ExcelHelper.ImportExceltoDt(filePath, 0, 0);
                Compare1(table);

            }
            //无表头，有sheet名
            {
                ExcelHelper.ExportDTtoExcel(dt, "", "BD_Properties", filePath);

                DataTable table = ExcelHelper.ImportExceltoDt(filePath, "BD_Properties", 0);
                Assert.AreEqual("BD_Properties", table.TableName);
                Compare1(table);

            }

            //有表头，无sheet名
            {
                ExcelHelper.ExportDTtoExcel(dt, "BD_Properties", "", filePath);

                DataTable table = ExcelHelper.ImportExceltoDt(filePath, 0, 1);
                Compare1(table);

            }
            //有表头，有sheet名
            {
                ExcelHelper.ExportDTtoExcel(dt, "BD_Properties", "BD_Properties", filePath);

                DataTable table = ExcelHelper.ImportExceltoDt(filePath, "BD_Properties", 1);
                Assert.AreEqual("BD_Properties", table.TableName);
                Compare1(table);
            }
        }

        void Compare1(DataTable table)
        {
            Assert.AreEqual("Type", table.Columns[0].ColumnName);
            Assert.AreEqual("PropertyName", table.Columns[1].ColumnName);
            Assert.AreEqual("Description", table.Columns[2].ColumnName);

            Assert.AreEqual("ADUser", table.Rows[0][0].ToString());
            Assert.AreEqual("accountExpires", table.Rows[0][1].ToString());
            Assert.AreEqual("Account-Expires", table.Rows[0][2].ToString());

            Assert.AreEqual("ADUser", table.Rows[1][0].ToString());
            Assert.AreEqual("accountNameHistory", table.Rows[1][1].ToString());
            Assert.AreEqual("Account-Name-History", table.Rows[1][2].ToString());
        }





        [TestMethod]
        public void ExportDStoExcelTest()
        {
            string filePath = DotEnvironment.MapAbsolutePath("AIS_SyncConfig.xls");
            Compare3(filePath);
            filePath = DotEnvironment.MapAbsolutePath("AIS_SyncConfig.xlsx");
            Compare3(filePath);


        }

        void Compare3(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            DataSet ds = new DataSet();

            {
                DataTable dt = new DataTable();
                dt.TableName = "BD_Properties";
                dt.Columns.Add(new DataColumn("Type"));
                dt.Columns.Add(new DataColumn("PropertyName"));
                dt.Columns.Add(new DataColumn("Description"));
                var row = dt.NewRow();
                row["Type"] = "ADUser";
                row["PropertyName"] = "accountExpires";
                row["Description"] = "Account-Expires";
                dt.Rows.Add(row);

                row = dt.NewRow();
                row["Type"] = "ADUser";
                row["PropertyName"] = "accountNameHistory";
                row["Description"] = "Account-Name-History";
                dt.Rows.Add(row);

                ds.Tables.Add(dt);
            }
            {
                DataTable dt = new DataTable();
                dt.TableName = "Test";
                dt.Columns.Add(new DataColumn("Type"));
                dt.Columns.Add(new DataColumn("PropertyName"));
                dt.Columns.Add(new DataColumn("Description"));
                var row = dt.NewRow();
                row["Type"] = "ADUser";
                row["PropertyName"] = "accountExpires";
                row["Description"] = "Account-Expires";
                dt.Rows.Add(row);

                row = dt.NewRow();
                row["Type"] = "ADUser";
                row["PropertyName"] = "accountNameHistory";
                row["Description"] = "Account-Name-History";
                dt.Rows.Add(row);

                ds.Tables.Add(dt);
            }

            {
                ExcelHelper.ExportDStoExcel(ds, filePath);

                DataTable table = ExcelHelper.ImportExceltoDt(filePath, 0, 0);
                Assert.AreEqual(table.TableName, "BD_Properties");
                Compare1(table);
                table = ExcelHelper.ImportExceltoDt(filePath, 1, 0);
                Assert.AreEqual(table.TableName, "Test");
                Compare1(table);
            }
        }
    }
}
