using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.Config;

namespace Dot.Demo.Definition
{
    public class AppSettings
    {
        public static string UserName { get { return ConfigHelper.GetAppSettingOrDefault("UserName", null); } }
    }
}
