using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Compile
{
    public class DynamicCompileHelper
    {
        #region 生成代码块

        /// <summary>
        /// 生成代码块
        /// </summary>
        /// <returns></returns>
        public static string BuildCSharpCode(string fileName)
        {
            string tempFileName = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory.Replace("Debug", string.Empty).Replace("Release", string.Empty), fileName);

            string strCodeDom = string.Empty;

            using (System.IO.StreamReader sr = System.IO.File.OpenText(tempFileName))
            {
                strCodeDom = sr.ReadToEnd();
            }

            return strCodeDom;
        }

        #endregion

        #region 动态编译代码

        /// <summary>
        /// 将代码编译为程序集
        /// </summary>
        /// <param name="codeDom"></param>
        /// <returns></returns>
        public static Assembly Compile(string codeDom)
        {
            //1>实例化C#代码服务提供对象
            CSharpCodeProvider provider = new CSharpCodeProvider();

            //2>声明编译器参数
            CompilerParameters parameters = new CompilerParameters();

            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            try
            {
                //3>动态编译
                CompilerResults result = provider.CompileAssemblyFromSource(parameters, codeDom);

                StringBuilder sb = new StringBuilder();
                if (result.Errors.Count > 0)
                {
                    foreach (CompilerError error in result.Errors)
                    {
                        sb.AppendLine(string.Format("文件名:{0};错误原因:{1}", error.ErrorText, error.FileName));
                    }

                    throw new Exception(sb.ToString());
                }

                //4>如果编译没有出错，此刻已经生成动态程序集LCQ.LCQClass

                //5>开始玩C#映射

                Assembly assembly = result.CompiledAssembly;
                return assembly;
            }
            catch (System.NotImplementedException ex)
            {
                Console.Write(ex.Message);
            }
            catch (System.ArgumentException ex)
            {
                Console.Write(ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        #endregion
    }
}
