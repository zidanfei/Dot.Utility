using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DotTests;
using Dot.Utility.Config;

namespace Dot.Adapter.Tests
{
    [TestClass()]
    public class ConfigHelperTests : TestBase
    {

        [TestMethod()]
        public void GetAppSettingOrDefaultTest()
        {
           var EmailFrom= ConfigHelper.GetValueOrDefault("EmailFrom"); 
            Assert.AreEqual("uxin_ad@sina.com", EmailFrom); 
        }
         
    }


     
}