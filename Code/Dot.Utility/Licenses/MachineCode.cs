/**
 * 机器码绑定机器信息(硬盘，CPU，内存，网卡，主板，mac）
 * 不允许在虚拟机中运行
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;

namespace Dot.Utility.Licenses
{

    /// <summary>   
    /// 机器码   
    /// </summary>   
    public class MachineCode
    {
        public static string GetMachineCode()
        {
            return (GetHostName()+"_"+ GetCpuInfo() + "_" + GetHDid() + "_" + GetMoAddress()).Replace(" ","");
        }

        static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        ///   <summary>    
        ///   获取cpu序列号        
        ///   </summary>    
        ///   <returns> string </returns>    
        static string GetCpuInfo()
        {
            string cpuInfo = " ";
            using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
            {
                ManagementObjectCollection moc = cimobject.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    mo.Dispose();
                    break;
                }
            }
            return cpuInfo.ToString();
        }

        ///   <summary>    
        ///   获取硬盘ID        
        ///   </summary>    
        ///   <returns> string </returns>    
        static string GetHDid()
        {
            string HDid = " ";
            using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
            {
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (ManagementObject mo in moc1)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                    mo.Dispose();
                    break;
                }
            }
            return HDid.ToString();
        }

        ///   <summary>    
        ///   获取网卡硬件地址    
        ///   </summary>    
        ///   <returns> string </returns>    
        static string GetMoAddress()
        {
            string MoAddress = " ";
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection moc2 = mc.GetInstances();
                foreach (ManagementObject mo in moc2)
                {
                    if ((bool)mo["IPEnabled"] == true)
                        MoAddress = mo["MacAddress"].ToString();
                    mo.Dispose();
                    break;
                }
            }
            return MoAddress.ToString();
        }
 
    }
}
