using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility
{
    public class CommendHelper
    {
        #region 服务

        public void StartService(string cmd)
        {
            Commend(string.Format(@"net start ""{0}""", cmd));
        }

        public void StopService(string cmd)
        {
            Commend(string.Format(@"net stop ""{0}""", cmd));
        }

        #endregion


        /// <summary>
        /// cmd执行命令
        /// </summary>
        /// <param name="execon"></param>
        public void Commend(string execon)
        {
            Process p = null;
            p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string strOutput = execon;// "MsiExec.exe /X{F94EABAD-9ADA-4336-B3C8-1E3DB6693015}";
            p.StandardInput.WriteLine(strOutput);
            p.StandardInput.WriteLine("exit");
            while (p.StandardOutput.EndOfStream)
            {
                strOutput = p.StandardOutput.ReadLine();
            }
            p.Close();
        }
    }
}
