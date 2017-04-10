using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Net;
using System.IO;
using System.Net;
using System.Text;

using System.Text.RegularExpressions;

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
