using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dot.Log
{
    public class Loghelper
    {

        #region 写文件
        public static void WriteLog(string content)
        {
            try
            {

                Encoding code = Encoding.GetEncoding("gb2312");
                var dir = System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string logname = dir + DateTime.Today.Date.ToString("yyyyMMdd") + ".txt";　//保存文件的路径
                string log = string.Format("\r\n\r\n记录时间:{0}\r\n{1}\r\n---------------------------------------------------------------------------------------------",
                    DateTime.Now, content);
                File.AppendAllText(logname, log, code);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 读文件
        public static string ReadLog(DateTime filename)
        {

            Encoding code = Encoding.GetEncoding("gb2312");
            var dir = System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";
            if (Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string temp = dir + DateTime.Today.Date.ToString("yyyyMMdd") + ".txt";　//保存文件的路径
            string str = "";
            if (File.Exists(temp))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(temp, code);
                    str = sr.ReadToEnd(); // 读取文件
                }
                catch { }
                sr.Close();
                sr.Dispose();
            }
            else
            {
                str = "";
            }


            return str;
        }
        #endregion
    }
}
