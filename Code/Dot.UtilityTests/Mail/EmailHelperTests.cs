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

        /// <summary>
        /// 给邮箱用户添加另外发送为权限：Get-Mailbox | Add-ADPermission -User 'NT Authority\Self' -ExtendedRights 'Send-as'
        /// </summary>
        [TestMethod()]
        public void SendEmailTest2()
        {
            EmailHelper helper = new EmailHelper("test", "chliu@ronglian.com");
            var res = helper.SendEmail("test");
            Assert.IsTrue(res);
        }
    }
}
