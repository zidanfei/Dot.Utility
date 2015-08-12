using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Dot.Utility.Mail.Tests
{
    [TestClass()]
    public class EmailHelperTests
    {

        [TestMethod()]
        public void SendEmailTest()
        {
            EmailHelper helper = new EmailHelper("test", "hellotigger@sina.com");
            var res = helper.SendEmail("test");
            Assert.IsTrue(res);
        }

    }
}
