using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.ActiveDirectory.Tests
{
    [TestClass()]
    public class ADHelperTests
    {
        [TestMethod()]
        public void SetPasswordTest()
        {
            ADHelper adhelper = new ADHelper("192.168.200.170", "administrator", "yxp.123");
            var u = ADHelper.GetUserEntryByAccount(adhelper.RootEntry, "anhainan1");
            ADHelper.SetPassword(u, "!QAZ2wsx");
            u.CommitChanges(); 
        }
    }
}