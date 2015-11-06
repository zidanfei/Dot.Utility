using Dot.Utility.Office;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Dot.UtilityTests.Office
{
   

    [TestClass()]
    public class ExcelHelperTest
    {
     

        [TestMethod()]
        public void ExportDTtoExcel()
        {
            try
            {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "test" });
            dt.Columns.Add(new DataColumn() { ColumnName = "hello" });
                var row = dt.NewRow();
                dt.Rows.Add(row);
            row["test"] = @"ExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExpor
tDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoE
xcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoE
xcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcel";
                row["hello"] = @"ExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExpor
tDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoE
xcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoE
xcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcelExportDTtoExcel";

                ExcelHelper.ExportDTtoExcel(dt,"test",AppDomain.CurrentDomain.BaseDirectory+"\\test.xls");

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
