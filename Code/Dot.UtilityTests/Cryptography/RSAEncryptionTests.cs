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
        string _content = "liuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuchliuch";
        string _rsacontent;
        string _hashData;

        [TestMethod()]
        public void SignatureDeformatterTest()
        {
            RSACryption.SaveKey(1024, out xmlPublicKey, out xmlKeys);
            RSACryption.GetHash(_content, out _hashData);
            RSACryption.SignatureFormatter(xmlKeys, _hashData, out _rsacontent);
            string content = "";
            Assert.IsTrue(RSACryption.SignatureDeformatter(xmlPublicKey, _hashData, _rsacontent));

        }

        [TestMethod()]
        public void EncryptStringTest()
        {
            RSACryption.SaveKey(1024, out xmlPublicKey, out xmlKeys);
            _rsacontent = RSACryption.EncryptString(_content, xmlPublicKey);

            var content = RSACryption.DecryptString(_rsacontent, xmlKeys);
            Assert.AreEqual(_content, content);
            //_rsacontent = RSACryption.EncryptString(_content, xmlKeys);

            //content = RSACryption.DecryptString(_rsacontent, xmlPublicKey);
            //Assert.AreEqual(_content, content);
        }

        [TestMethod()]
        public void X509CertSignatureDeformatterTest()
        {
            var cert = RSACryption.X509CertCreatePrivateKeyRSA("RSAKey");
            var pubcert = RSACryption.X509CertCreatePublicKeyRSA("RSAKey");
            var pubcert2 = RSACryption.X509CertCreatePublicKeyRSA("chenhui");
            RSACryption.GetHash(_content, out _hashData);
            RSACryption.SignatureFormatter(cert, _hashData, out _rsacontent);
            Assert.IsTrue(RSACryption.SignatureDeformatter(pubcert, _hashData, _rsacontent));
            Assert.IsFalse(RSACryption.SignatureDeformatter(pubcert2, _hashData, _rsacontent));

        }
        [TestMethod()]
        public void X509CertEncryptStringTest()
        {
            var cert = RSACryption.X509CertCreatePrivateKeyRSA("RSAKey");
            var pubcert = RSACryption.X509CertCreatePublicKeyRSA("RSAKey");
            _rsacontent = RSACryption.EncryptString(pubcert, _content);

            var content = RSACryption.DecryptString(cert, _rsacontent);
            Assert.AreEqual(_content, content);
            //_rsacontent = RSACryption.EncryptString(_content, xmlKeys);

            //content = RSACryption.DecryptString(_rsacontent, xmlPublicKey);
            //Assert.AreEqual(_content, content);
        }

    }
}