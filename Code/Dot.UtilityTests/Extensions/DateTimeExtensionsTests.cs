using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Tests
{
    [TestClass()]
    public class DateTimeExtensionsTests
    {
        [TestMethod()]
        public void AddWeekdaysTest()
        {
            var date = DateTime.Today.AddWeekdays(2);
            Assert.IsTrue(date > DateTime.Today.AddDays(2));
        }

        [TestMethod()]
        public void FirstDayOfMonthTest()
        {
            var date = DateTime.Today.FirstDayOfMonth();
            Assert.AreEqual(date, DateTime.Today.AddDays(-DateTime.Today.Day + 1));
        }

        [TestMethod()]
        public void LastDayOfMonthTest()
        {
            var date = DateTime.Today.LastDayOfMonth();
            Assert.AreEqual(date, DateTime.Today.AddDays(-DateTime.Today.Day).AddMonths(1));
        }

        [TestMethod()]
        public void CompareToTest()
        {
            Assert.IsTrue(DateTime.Now.CompareTo(DateTime.Today) > 0);

        }

        

        [TestMethod()]
        public void ConvertDateTimeIntTest()
        {
            var d = DateTime.Now.ConvertDateTimeInt();
        }
    }
}
