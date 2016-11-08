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
    public class Cyh_RSATests
    {

        string xmlKeys, xmlPublicKey;
        Cyh_RSA _rsa = new Cyh_RSA();
        string _content = "hello word";
        string _rsacontent;
        [TestMethod()]
        public void EncryptStringTest()
        {
            _rsa.SaveKey(1024, out xmlPublicKey, out xmlKeys);
            _rsacontent = _rsa.EncryptString( _content,xmlPublicKey);

            var content = _rsa.DecryptString( _rsacontent,xmlKeys);
            Assert.AreEqual(_content, content);
        }
    }
}