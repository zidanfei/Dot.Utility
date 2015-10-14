using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Net;

namespace Dot.UtilityTests.Net
{
    [TestClass]
    public class HttpHelperTest
    {
        [TestMethod]
        public void GetTest()
        {
            HttpHelper helper = new HttpHelper();
            var d = helper.Get("", "", "", "");
        }
    }
}
