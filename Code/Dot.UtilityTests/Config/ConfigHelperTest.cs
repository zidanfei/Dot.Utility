#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Config;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
#endregion

///////////////////////////////////////////////////////////////////////////////
// Copyright 2015 (c) by Dot All Rights Reserved.
//  
// Project:      Utility
// Module:       ConfigHelperTest.cs
// Description:  Tests for the Config Helper class in the Dot. Utility assembly.
//  
// Date:       Author:           Comments:
// 2015/10/14 15:07  chliu     Module created.
///////////////////////////////////////////////////////////////////////////////
namespace Dot.Utility.ConfigTest
{

    /// <summary>
    /// Tests for the Config Helper Class
    /// Documentation: 
    /// </summary>
    [TestClass()]
    public class ConfigHelperTest
    {
        #region Class Variables
        private TestContext testContextInstance;
        private ConfigHelper _configHelper;
        #endregion

        #region Setup/Teardown

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [TestInitialize()]
        public void Initialize()
        {
            //New instance of Config Helper
            _configHelper = new ConfigHelper();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TestCleanup()]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _configHelper or delete this line
        }
        #endregion

        #region Property Tests

        #region GeneratedProperties

        // No public properties were found. No tests are generated for non-public scoped properties.

        #endregion // End of GeneratedProperties

        #endregion

        #region Method Tests

        #region GeneratedMethods

        /// <summary>
        /// Get App Setting Or Default Method Test
        /// Documentation   :  获取配置文件中的AppSettings的指定键的值
        /// Method Signature:  string GetAppSettingOrDefault(string key, string defaultValue =)
        /// </summary>
        [TestMethod()]

        public void GetAppSettingOrDefaultTest()
        {
            DateTime methodStartTime = DateTime.Now;
            string expected = "test";

            //Parameters
            string key = "test";
            string defaultValue = "test";

            string results = ConfigHelper.GetAppSettingOrDefault(key, defaultValue);
            Assert.AreEqual(expected, results, "Dot.Utility.Config.ConfigHelper.GetAppSettingOrDefault method test failed");

            TimeSpan methodDuration = DateTime.Now.Subtract(methodStartTime);
            Console.WriteLine(String.Format("Dot.Utility.Config.ConfigHelper.GetAppSettingOrDefault Time Elapsed: {0}", methodDuration));
        }

        /// <summary>
        /// Get Connection String Or Default Method Test
        /// Documentation   :  获取配置文件中的ConnectionString的指定键的值
        /// Method Signature:  string GetConnectionStringOrDefault(string key, string defaultValue =)
        /// </summary>
        [TestMethod()]

        public void GetConnectionStringOrDefaultTest()
        {
            DateTime methodStartTime = DateTime.Now;
            string expected = "test";

            //Parameters
            string key = "test";
            string defaultValue = "test";

            string results = ConfigHelper.GetConnectionStringOrDefault(key, defaultValue);
            Assert.AreEqual(expected, results, "Dot.Utility.Config.ConfigHelper.GetConnectionStringOrDefault method test failed");

            TimeSpan methodDuration = DateTime.Now.Subtract(methodStartTime);
            Console.WriteLine(String.Format("Dot.Utility.Config.ConfigHelper.GetConnectionStringOrDefault Time Elapsed: {0}", methodDuration));
        }

        /// <summary>
        /// Exists App Setting Method Test
        /// Documentation   :  
        /// Method Signature:  bool ExistsAppSetting(string key)
        /// </summary>
        [TestMethod()]

        public void ExistsAppSettingTest()
        {
            DateTime methodStartTime = DateTime.Now;
            bool expected = true;

            //Parameters
            string key = "test";

            bool results = ConfigHelper.ExistsAppSetting(key);
            Assert.AreEqual(expected, results, "Dot.Utility.Config.ConfigHelper.ExistsAppSetting method test failed");

            TimeSpan methodDuration = DateTime.Now.Subtract(methodStartTime);
            Console.WriteLine(String.Format("Dot.Utility.Config.ConfigHelper.ExistsAppSetting Time Elapsed: {0}", methodDuration));
        }

        /// <summary>
        /// Exists App Setting Method Test
        /// Documentation   :  
        /// Method Signature:  bool ExistsAppSetting(string key)
        /// </summary>
        [TestMethod()]

        public void ExistsAppSetting1Test()
        {
            DateTime methodStartTime = DateTime.Now;
            var notexists = ConfigHelper.ExistsAppSetting("notexists");
            Assert.IsFalse(notexists);
            var EmailHost = ConfigHelper.ExistsAppSetting("EmailHost");
            Assert.IsTrue(EmailHost);
            EmailHost = ConfigHelper.ExistsAppSetting("emailHost");
            Assert.IsTrue(EmailHost);
            var EmailHostvalue = ConfigHelper.GetAppSettingOrDefault("emailHost");
            Assert.IsNotNull(EmailHostvalue);

            bool expected = false;

            //Parameters
            string key = "test";

            bool results = ConfigHelper.ExistsAppSetting(key);
            Assert.AreEqual(expected, results, "Dot.Utility.Config.ConfigHelper.ExistsAppSetting1 method test failed");

            TimeSpan methodDuration = DateTime.Now.Subtract(methodStartTime);
            Console.WriteLine(String.Format("Dot.Utility.Config.ConfigHelper.ExistsAppSetting1 Time Elapsed: {0}", methodDuration));
        }

        #endregion // End of GeneratedMethods


        [TestMethod()]

        public void SetTest()
        {
            //ConfigHelper.Set("PublishEnvironment","Dev1");
            //ConfigHelper.Set("nou", "nou");
        }
        #endregion

    }
}
