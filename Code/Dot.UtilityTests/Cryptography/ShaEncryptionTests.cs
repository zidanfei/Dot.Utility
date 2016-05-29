using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Cryptography.Tests
{
    [TestClass()]
    public class ShaEncryptionTests
    {
        [TestMethod()]
        public void Hash_SHA_256Test()
        {
            //DateTime date = DateTime.Now;
            long t = DateTime.Now.Ticks;
            string date = (t + 360000).ToString() + "asdj()*&@";


            var value = ShaEncryption.Hash_SHA_256(date, false);
            Assert.Fail();
        }
    }
}