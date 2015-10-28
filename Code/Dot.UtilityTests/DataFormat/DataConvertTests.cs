using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Xml;

namespace Dot.Utility.Tests
{
    [TestClass()]
    public class DataConvertTests
    {
        [TestMethod()]
        public void HexStringToLongTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDefaultValueTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToDataTableTest()
        {
            IList<MyClass> list = new List<MyClass>();
            list.Add(new MyClass { a = 1, b = 2, c = true });
            list.Add(new MyClass { a = 3, b = 4, c = false });
            list.Add(new MyClass { a = 5, b = 6 });

            DataTable dt = DataConvert.ToDataTable<MyClass>(list);
            try
            {
                list = DataConvert.ToList<MyClass>(dt);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [TestMethod()]
        public void ToShortGuidTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToBase32StringTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToBase32StringTest1()
        {
            Assert.Fail();
        }
    }

    class MyClass
    {
        public int a { get; set; }

        public int b { get; set; }
        public bool c { get; set; }
    }
}
