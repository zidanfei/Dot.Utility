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
            //Log.LogFactory.DataLog.Info(new LogMessage() { UserId = "UserId4", Message = DateTime.Now.ToString(),AreaName="area1",ControllerName="con2",ActionName="action3" });
            var log = new LogMessage()
            {
                UserId = "UserId4",
                Message = DateTime.Now.ToString(),
                AreaName = "area1",
                ControllerName = "con2",
                ActionName = "action3",
            };
            log.ExtendPropety.Add("ou", "testou");
            Log.LogFactory.WebBusinessLog.Info(log            );
            Log.LogFactory.WebDataLog.Info(new LogMessage() { UserId = "UserId4", Message = DateTime.Now.ToString(),AreaName="area1",ControllerName="con2",ActionName="action3" });
            //Log.LogFactory.RunningLog.Info(new LogMessage() { UserId = "UserId4", Message = DateTime.Now.ToString(),AreaName="area1",ControllerName="con2",ActionName="action3" });
            Loghelper.WriteLog("today");

        }

        [TestMethod()]
        public void ReadLogTest()
        {
            var log = Loghelper.ReadLog(DateTime.Today);

        }
    }
}
