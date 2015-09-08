using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.PSWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;
using Microsoft.Hosting.DPM.ServiceImplementation;
using System.Management.Automation.Runspaces;
namespace Dot.Utility.PSWrapper.Tests
{
    [TestClass()]
    public class CmdletProcessorTests
    {

        /// <summary>
        /// 运行命令前 先修改策略 运行 set-executionpolicy remotesigned
        /// Set-ExecutionPolicy remotesigned -Scope CurrentUser
        /// 参考 http://www.cnblogs.com/zhaozhan/archive/2012/06/01/2529384.html
        /// </summary>
        [TestMethod()]
        public void RunCmdletTest()
        {
            //List<string> getlist = new List<string>();
            //getlist.Add("Set-ExecutionPolicy RemoteSigned");
            //var s= ExecuteShellScript(getlist, null);

            using (CmdletProcessor cp = new CmdletProcessor())
            {
                string pspath = System.AppDomain.CurrentDomain.BaseDirectory + "\\PSWrapper\\get-process.ps1";
                var par = new CmdletParameterSetBase(pspath).AddParameter(new CmdletParameterSwitchValuePair("processName", new CmdletStringParameter("qq")));
                var result = cp.RunCmdlet<PSObject>(par);


            }
        }

        public class ShellParameter
        {
            public string ShellKey { get; set; }
            public string ShellValue { get; set; }
        }
        public static string ExecuteShellScript(List<string> getshellstrlist, List<ShellParameter> getshellparalist)
        {
            string getresutl = null;
            try
            {
                //Create Runspace
                Runspace newspace = RunspaceFactory.CreateRunspace();
                Pipeline newline = newspace.CreatePipeline();

                //open runspace
                newspace.Open();

                if (getshellstrlist.Count > 0)
                {
                    foreach (string getshellstr in getshellstrlist)
                    {
                        //Add Command ShellString
                        newline.Commands.AddScript(getshellstr);
                    }
                }

                //Check Parameter
                if (getshellparalist != null && getshellparalist.Count > 0)
                {
                    int count = 0;
                    foreach (ShellParameter getshellpar in getshellparalist)
                    {
                        //Set parameter
                        //注入脚本一个.NEt对象 这样在powershell脚本中就可以直接通过$key访问和操作这个对象
                        //newspace.SessionStateProxy.SetVariable(getshellpar.ShellKey,getshellpar.ShellValue);
                        CommandParameter cmdpara = new CommandParameter(getshellpar.ShellKey, getshellpar.ShellValue);
                        newline.Commands[count].Parameters.Add(cmdpara);
                    }
                }

                //Exec Restult
                var getresult = newline.Invoke();
                if (getresult != null)
                {
                    StringBuilder getbuilder = new StringBuilder();
                    foreach (var getresstr in getresult)
                    {
                        getbuilder.AppendLine(getresstr.ToString());
                    }
                    getresutl = getbuilder.ToString();
                }

                //close 
                newspace.Close();
            }
            catch (Exception se)
            {
                //catch Excetption 
            }
            return getresutl;
        }
    }
}
