using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Dot.Utility.ActiveDirectory.Tests
{
    [TestClass()]
    public class ChineseToPinYinTests
    {
        [TestMethod()]
        public void ConvertTest()
        {
            var res = ChineseToPinYin.Convert("刘晨辉", "_", true);
            Assert.AreEqual(res, "Liu_Chen_Hui");
            res = ChineseToPinYin.Convert("刘晨辉", "", true);
            Assert.AreEqual(res, "LiuChenHui");
            res = ChineseToPinYin.Convert("刘晨辉", "_", false);
            Assert.AreEqual(res, "liu_chen_hui");
            res = ChineseToPinYin.Convert("刘晨辉", "", false);
            Assert.AreEqual(res, "liuchenhui");

            ChineseToPinYin.Phrase.Add("刘晨辉", "Liuch");
            res = ChineseToPinYin.Convert("刘晨辉", "", false);
            Assert.AreEqual(res, "Liuch");

        }


    }
}
