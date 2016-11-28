using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Office.Tests
{
    [TestClass()]
    public class ExcelHelperTests
    {
        [TestMethod()]
        public void ImportExceltoDtTest()
        {
            var dt= ExcelHelper.ImportExceltoDt("F:\\nc部门.xls");
        }
    }
}