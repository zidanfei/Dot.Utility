using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Dot.Utility.Log.Tests
{
    [TestClass()]
    public class LoghelperTests
    {
        [TestMethod()]
        public void WriteLogTest()
        {
            Loghelper.WriteLog("today");

        }

        [TestMethod()]
        public void ReadLogTest()
        {
           var log=  Loghelper.ReadLog(DateTime.Today);

        }
    }
}
