using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Utility.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Database.Tests
{
    [TestClass()]
    public class MySqlHelperTests
    {
        [TestMethod()]
        public void ExecuteDatasetTest()
        {
            string con = "Driver={MySQL};Server=192.168.42.128;Option=131072;charset=UTF8;Database=testliu; User=root;Password=pass@word1;";
            con = "Server = 192.168.42.128; Database = testliu; Uid = sa; Pwd = pass@word1;";
          var ds=  MySqlHelper.ExecuteDataset(con, System.Data.CommandType.Text,
              "SELECT * FROM hahah");

            
        }
    }
}