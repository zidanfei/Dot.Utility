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
    public class RSAEncryptionTests
    {
        string xmlKeys, xmlPublicKey;
        RSAEncryption _rsa = new RSAEncryption();
        string _content = "hello word";
        string _rsacontent;


        [TestMethod()]
        public void RSAEncryptTest()
        {
            _rsa.RSAKey(out xmlKeys, out xmlPublicKey);
            _rsacontent = _rsa.RSAEncrypt(xmlPublicKey, Encoding.Default.GetBytes(_content));

            var content = _rsa.RSADecrypt(xmlKeys, Encoding.Default.GetBytes(_rsacontent));
            Assert.AreEqual(_content, content);

        }

         
    }
}