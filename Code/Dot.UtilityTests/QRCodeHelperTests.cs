using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using ThoughtWorks.QRCode.Codec.Data;

namespace Dot.Utility.Tests
{
    [TestClass()]
    public class QRCodeHelperTests
    {
        [TestMethod()]
        public void CreateCode_SimpleTest()
        {

            QRCodeHelper.CreateCode_Simple("http://www.baidu.com", AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        [TestMethod()]
        public void CreateCode_ChooseTest()
        {
            QRCodeHelper.CreateCode_Choose( AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "http://www.baidu.com", ENCODE_MODE.BYTE, ERROR_CORRECTION.M,4,2);

        }
    }


}