using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
//using System.Management;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
using System.Globalization;
using Dot.Utility.Web;

namespace Dot.Utility.Net
{
    public class IPHelper
    {
        static readonly string _pattern = "^((?:(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d)))\\.){3}(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d))))$";
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);


        ///// <summary>
        ///// 获取用户登录IP
        ///// </summary>
        ///// <returns></returns>
        //public static string GetClientIp()
        //{
        //    try
        //    {
        //        if (null != HttpContext.Current && null != HttpContext.Current.Request)
        //        {
        //            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
        //                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
        //            else
        //                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return string.Empty;
        //}


        /// <summary>
        /// 获取本机的IP        
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {

            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            foreach (var item in ipEntry.AddressList)
            {
                Match match = Regex.Match(item.ToString(), _pattern);
                if (match.Success)
                    return item.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取本机的MAC       
        /// </summary>
        /// <returns></returns>
        //public static string GetLocalMac()
        //{
        //    string mac = null;
        //    ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
        //    ManagementObjectCollection queryCollection = query.Get();
        //    foreach (ManagementObject mo in queryCollection)
        //    {
        //        if (mo["IPEnabled"].ToString() == "True")
        //            mac = mo["MacAddress"].ToString();
        //    }
        //    return (mac);
        //}

        /// <summary>
        /// 获取远程主机IP      
        /// </summary>
        /// <param name="RemoteHostName">Name of the remote host.</param>
        /// <returns></returns>
        public static string[] GetRemoteIP(string RemoteHostName)
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(RemoteHostName);
            IPAddress[] IpAddr = ipEntry.AddressList;
            string[] strAddr = new string[IpAddr.Length];
            for (int i = 0; i < IpAddr.Length; i++)
            {
                strAddr[i] = IpAddr[i].ToString();
            }
            return (strAddr);
        }
        //
        /// <summary>
        /// 获取远程主机MAC        
        /// </summary>
        /// <param name="localIP">The local ip.</param>
        /// <param name="remoteIP">The remote ip.</param>
        /// <returns></returns>
        public static string GetRemoteMac(string localIP, string remoteIP)
        {
            Int32 ldest = inet_addr(remoteIP); //目的ip 
            Int32 lhost = inet_addr(localIP); //本地ip 

            try
            {
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                return Convert.ToString(macinfo, 16);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error:{0}", err.Message);
            }
            return 0.ToString();
        }


        //public static string GetWCFClientIP()
        //{
        //    object remoteObject;
        //    OperationContext.Current.IncomingMessageProperties.TryGetValue(RemoteEndpointMessageProperty.Name,
        //                                                                   out remoteObject);
        //    string ipAddress = (remoteObject as RemoteEndpointMessageProperty).Address;
        //    return ipAddress;
        //}

        #region IP地址验证器

        /// <summary>
        /// 验证IP地址格式，包括IPv4/IPv6
        /// </summary>
        /// <param name="ip">输入的IP地址字符串</param>
        /// <returns>判断结果，如果为真，则为格式正确的IP地址。</returns>
        public static bool IsIPAddress(string ip)
        {
            bool b = IsIPv4(ip);

            if (!b) b = IsIPv6(ip);

            return b;
        }

        /// <summary>
        /// 验证IPv4地址格式
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns>
        /// 	<c>true</c> if is IPV4 the specified ip; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPv4(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            string[] ips = ip.Split('.');

            if (ips.Length != 4)
            {
                return false;
            }

            Regex regex = new Regex(@"^\d+$");

            for (int i = 0; i < ips.Length; i++)
            {
                if (!regex.IsMatch(ips[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(ips[i], CultureInfo.InvariantCulture) > 255)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 验证IPv6地址格式
        /// </summary>
        /// <param name="ip">输入的IP地址</param>
        /// <returns>验证IPv6地址格式，如果为真，则格式正确。</returns>
        public static bool IsIPv6(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            //IPv4的文本表示法：十进制点号表示法
            //IPv6的文本表示法：带有前导零和零压缩法的冒号十六进制格式
            //解释一下就是:
            //Format is x:x:x:x:x:x:x:x
            //x is a 16 bit hexadecimal field
            //FEDC:BA98:7654:3210:FEDC:BA98:7654:3210
            //Leading zeros in a field are optional
            //:: can be used to represent multiple groups of 16 bits of zero
            //:: can only be used once in an address
            //FF01:0:0:0:0:0:0:101 = FF01::101
            //0:0:0:0:0:0:0:1 = ::1
            //0:0:0:0:0:0:0:0 = ::

            //(("FE06::1::2"),false); //只能用一次双冒号
            //(("10.2.3.5"),false);  //没有冒号
            //(("00000:000000:0000::1"),false); //每段的长度不超过4
            //(("0000:0000:0000:0000:0000:0000:0000:0000:0000:1"),false); //总长度不超过39
            //(("JKLN:ssej::1"),false); //必须是十六进制的数字
            //(("12::45::1"),false);    //双冒号必须只能出现一次
            //(("12::44:f:45::1"),false); //双冒号必须只能出现一次(不连续出现)

            //(("ABEF:452::FE10"),true); //正确的合法的IPv6
            //(("12::1"),true); //合理压缩
            //(("::1"),true); //双冒号压缩发
            //(("0000:0000:0000:0000:0000:0000:0000:0000"),true); //正确无压缩写法
            //(("::1:123:23"),true); //双冒号压缩发
            //(("123:45::ADC:6"),true); //双冒号位置不确定

            //* 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8
            //* 2、判断输入的IPV6字符串中是否有“::”。
            //* 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断
            //* 4、如果有“::” ，判断"::"是否止出现一次
            //* 5、如果出现一次以上 返回false
            //* 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$

            if (ip.Length > 39)
            {
                return false;
            }

            string pattern = string.Empty;
            string temp = ip;
            string[] ips = temp.Split(':');

            if (ips.Length > 8)
            {
                return false;
            }

            int colon = GetStringCount(ip, @":");
            if (colon > 7 || colon <= 0)
            {
                return false;
            }

            int count = GetStringCount(ip, @"::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                bool ismatch = false;

                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                ismatch = regex.IsMatch(ip);

                return ismatch;
            }
            else
            {
                bool ismatch = false;
                Regex regexDouble;

                // fe80:230::48ff:fed3:5a76
                // 3ffe:ffff:0:f101::101
                pattern = @"^([\da-f]{1,4}:){0,5}([\da-f]{1,4})::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                regexDouble = new Regex(pattern, RegexOptions.IgnoreCase);
                if (!ismatch)
                {
                    ismatch = regexDouble.IsMatch(ip);
                }

                // fe80::230:48ff:fed3:5a76
                // ::230:48ff:fed3:5a76
                pattern = @"^([\da-f]{1,4}){0,1}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                regexDouble = new Regex(pattern, RegexOptions.IgnoreCase);
                if (!ismatch)
                {
                    ismatch = regexDouble.IsMatch(ip);
                }

                return ismatch;
            }
        }

        /// <summary>
        /// 判断字符串compare 在 input字符串中出现的次数
        /// </summary>
        /// <param name="input">源字符串.</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        internal static int GetStringCount(string input, string compare)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            if (string.IsNullOrEmpty(compare))
                throw new ArgumentNullException("compare");

            int index = input.IndexOf(compare, StringComparison.CurrentCulture);
            if (index != -1)
            {
                return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 验证IP端口
        /// </summary>
        /// <param name="portId">IP端口</param>
        /// <returns></returns>
        public static bool IsIPPort(string portId)
        {
            if (string.IsNullOrWhiteSpace(portId))
            {
                return false;
            }

            Regex regex = new Regex("[0-9]+");
            return (regex.Match(portId).Success && (portId.Length < 9));
        }
        #endregion

    }
}