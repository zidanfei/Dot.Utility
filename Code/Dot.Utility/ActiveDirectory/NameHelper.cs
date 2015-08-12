
using Dot.Utility;
using Dot.Utility.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace Dot.Utility.ActiveDirectory
{
    /// <summary>
    /// 姓名操作类
    /// </summary>
    public class NameHelper
    {
        public NameHelper(string domain, string user, string pwd, string rootOU)
        {
            Domain = domain;
            DomainUser = user;
            DomainPass = pwd;
            RootName = rootOU;
        }

        /// <summary>
        /// 通过姓名生成登录名
        /// </summary>
        /// <param name="CNName"></param>
        /// <param name="ENName"></param>
        /// <returns></returns>
        public string GetLoginName(string CNName, string ENName)
        {
            string tempLoginName = string.Empty;
            string familyName = string.Empty;
            if (string.IsNullOrWhiteSpace(CNName))
            {
                return CNName;
            }
            if (IsCompoundSurname(CNName, out familyName))//复姓
            {
                #region 复姓
                /*
                 * 复姓有4个优先级
                 */

                //第一优先级：姓的全拼+名第一个字母简拼 +第二个字字母简拼
                tempLoginName = ChineseCharacterToQuanPinYin(familyName.Substring(0, 1)) + ChineseCharacterToQuanPinYin(familyName.Substring(1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2));
                if (!IsLoginNameExists(tempLoginName))
                {
                    return tempLoginName;
                }
                else //存在
                {
                    //第二优先级：姓的全拼+名第一个字字母全拼+名第二个字字母简拼
                    if (CNName.Length < 3)
                    {
                        tempLoginName = ChineseCharacterToQuanPinYin(familyName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(familyName.Substring(1)).ToLower();
                    }
                    else if (CNName.Length == 3)
                    {
                        tempLoginName = ChineseCharacterToQuanPinYin(familyName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(familyName.Substring(1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(2)).ToLower();
                    }
                    else if (CNName.Length > 3)
                    {
                        tempLoginName = ChineseCharacterToQuanPinYin(familyName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(familyName.Substring(1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(2, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(3)).ToLower();
                    }
                    if (!IsLoginNameExists(tempLoginName))
                    {
                        return tempLoginName;
                    }
                    else//不存在
                    {
                        //第三优先级：姓缩写+名缩写
                        tempLoginName = ChineseCharacterToPinYinAbbreviation(familyName.Substring(0, 1)).ToUpper() + ChineseCharacterToPinYinAbbreviation(familyName.Substring(1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower();
                        if (!IsLoginNameExists(tempLoginName))
                        {
                            return tempLoginName;
                        }
                        else
                        {
                            //第四优先级：英文名
                            if (!IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToPinYinAbbreviation(familyName).ToLower()))
                            {
                                return tempLoginName;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                    }
                }
                #endregion 复姓
            }
            else if (CNName.Length < 5)//单姓
            {
                #region 单姓
                /*
                 * 单姓 分为17个优先级
                 */

                //第一优先级：姓的全拼+名第一个字母简拼 +第二个字字母简拼
                //string tempLoginName = string.Empty;
                if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第二优先级：姓的全拼+名第一个字字母全拼+名第二个字字母简拼     
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(1, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower()))
                {
                    return tempLoginName;
                }
                //第三优先级：姓的全拼+名全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第四优先级：姓全拼.名第一个字字母简拼+名第二个字字母简拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "." + ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第五优先级：姓全拼.名第一个字字母全拼+名第二个字字母简拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "." + ChineseCharacterToQuanPinYin(CNName.Substring(1, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower()))
                {
                    return tempLoginName;
                }
                //第六优先级：姓全拼.名全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "." + ChineseCharacterToQuanPinYin(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第七优先级：姓全拼_名第一个字字母简拼+名第二个字字母简拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "_" + ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第八优先级：姓全拼_名第一个字字母全拼+名第二个字字母简拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "_" + ChineseCharacterToQuanPinYin(CNName.Substring(1, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower()))
                {
                    return tempLoginName;
                }
                //第九优先级：姓全拼_名全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)) + "_" + ChineseCharacterToQuanPinYin(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十优先级：名缩写+.姓全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower() + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十一优先级：第一个全，以后简+.姓全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(1, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower() + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十二优先级：名全称+.姓全拼
                else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToQuanPinYin(CNName.Substring(1)).ToLower() + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十三优先级：英文名+.姓全拼
                else if (!string.IsNullOrEmpty(ENName) && !IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十四优先级：英文名+.姓第一个字母+名第一个字字母简拼+名第二个字字母简拼
                else if (!string.IsNullOrEmpty(ENName) && !IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToPinYinAbbreviation(CNName.Substring(0, 1)) + ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十五优先级：英文名+.姓全拼+名第一个字字母简拼+名第二个字字母简拼
                else if (!string.IsNullOrEmpty(ENName) && !IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十六优先级：英文名+.姓全拼+名第一个字字母全拼+名第二个字字母简拼
                else if (!string.IsNullOrEmpty(ENName) && !IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(1, 1)).ToLower() + ChineseCharacterToPinYinAbbreviation(CNName.Substring(2)).ToLower()))
                {
                    return tempLoginName;
                }
                //第十七优先级：英文名.姓全拼+名全拼
                else if (!string.IsNullOrEmpty(ENName) && !IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToQuanPinYin(CNName.Substring(0, 1)).ToLower() + ChineseCharacterToQuanPinYin(CNName.Substring(1)).ToLower()))
                {
                    return tempLoginName;
                }
                else
                {
                    return string.Empty;
                }
                #endregion 单姓
            }
            else//归属于少数民族
            {
                #region 少数民族姓名
                if (CNName.IndexOf('·') > 0)
                {
                    if (!IsLoginNameExists(tempLoginName = ChineseCharacterToPinYinAbbreviation(CNName.Replace("·", ""))))
                    {
                        return tempLoginName;
                    }
                    else if (!IsLoginNameExists(tempLoginName = ChineseCharacterToPinYinAbbreviation(CNName.Substring(0, CNName.IndexOf('·'))) + "." + ChineseCharacterToPinYinAbbreviation(CNName.Substring(CNName.IndexOf('·') + 1))))
                    {
                        return tempLoginName;
                    }
                    else if (!IsLoginNameExists(tempLoginName = ENName + "." + ChineseCharacterToPinYinAbbreviation(CNName.Substring(0, CNName.IndexOf('·')))))
                    {
                        return tempLoginName;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!IsLoginNameExists(tempLoginName = ChineseCharacterToPinYinAbbreviation(CNName)))
                    {
                        return tempLoginName;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                #endregion 少数民族姓名
            }
        }

        #region Convert

        /// <summary>
        /// 汉字转换成全拼的拼音
        /// </summary>
        /// <param name="Chstr">汉字字符串</param>
        /// <returns>转换后的拼音字符串</returns>
        public static string ChineseCharacterToQuanPinYin(string Chstr)
        {
            string name = string.Empty;
            for (int i = 0; i < Chstr.Length; i++)
            {
                var chinese = SQLiteHelper.ExecuteDataset("Data Source=" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\NameDB.DB"), CommandType.Text,
                    @"select * from ChinesePinyin
                where Chinese='" + Chstr.Substring(i, 1) + "'");
                if (chinese.Tables[0].Rows.Count > 0)
                    name += chinese.Tables[0].Rows[0]["PinYin1"].ToString();
            }
            return name;
        }

        /// <summary>
        /// 汉字转拼音缩写
        /// 字母和符号不转换
        /// </summary>
        /// <param name="str">要转换的汉字字符串</param>
        /// <returns>拼音缩写</returns>
        public static string ChineseCharacterToPinYinAbbreviation(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            string name = string.Empty;
            StringBuilder sbname = new StringBuilder(50);
            for (int i = 0; i < str.Length; i++)
            {
                name = ChineseCharacterToQuanPinYin(str.Substring(i, 1)).ToLower();
                if (name.Length > 1)//每执行一次加1
                {
                    sbname.Append(name.Substring(0, 1));
                }
                else if (name.Length > 0)
                {
                    sbname.Append(name);
                }
            }
            return sbname.ToString();

        }

        /// <summary>
        /// 取单个字符的拼音声母
        /// </summary>
        /// <param name="c">要转换的单个汉字</param>
        /// <returns>拼音声母</returns>
        public static string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "g";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return "*";
        }

        #endregion Convert

        /// <summary>
        /// 中文名拆分为数组
        /// 姓为数组第一个数据
        /// </summary>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public static string[] SplitName(string cnName)
        {
            if (string.IsNullOrWhiteSpace(cnName))
            {
                return new string[2];
            }
            if (cnName.Length == 1)
            {
                return new string[] { cnName, "" };
            }
            else if (cnName.Length == 2)
            {
                return new string[] { cnName.Substring(0, 1), cnName.Substring(1, 1) };
            }
            else
            {
                string familyName = string.Empty;
                List<string> NameList = new List<string>();
                if (IsCompoundSurname(cnName, out familyName))
                {
                    return new string[] { cnName.Substring(0, 2), cnName.Substring(2) };
                }
                else
                {
                    return new string[] { cnName.Substring(0, 1), cnName.Substring(1) };
                }
            }
        }

        string Domain { get; set; }
        string DomainUser { get; set; }
        string DomainPass { get; set; }
        string RootName { get; set; }

        public bool IsLoginNameExists(string loginName)
        {
            ADHelper adHelper = new ADHelper(Domain, DomainUser, DomainPass);
            var root = adHelper.GetRootEntry();
            //var rootOrg = ADHelper.GetOrganizeEntry(root, RootName);
            var u = ADHelper.GetUserEntryByAccount(root, loginName);
            if (u != null)
                return true;
            return false;
        }

        /// <summary>
        /// 判断是否复姓
        /// </summary>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public static bool IsCompoundSurname(string cnName, out string familyName)
        {
            familyName = string.Empty;
            List<string> CompoundSurname = (typeof(CompoundSurname)).GetEnumNames().ToList();
            foreach (string name in CompoundSurname)
            {
                if (cnName.StartsWith(name))
                {
                    familyName = name;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 复姓
    /// </summary>
    public enum CompoundSurname
    {
        欧阳, 太史, 端木, 上官, 司马, 东方, 独孤, 南宫, 万俟, 闻人, 夏侯, 诸葛, 尉迟, 公羊, 赫连, 澹台, 皇甫, 宗政, 濮阳, 公冶, 太叔, 申屠, 公孙, 慕容, 仲孙, 钟离, 长孙, 宇文, 司徒, 鲜于, 司空, 闾丘, 子车, 亓官, 司寇, 巫马, 公西, 颛孙, 壤驷, 公良, 漆雕, 乐正, 宰父, 谷梁, 拓跋, 夹谷, 轩辕, 令狐, 段干, 百里, 呼延, 东郭, 南门, 羊舌, 微生, 公户, 公玉, 公仪, 梁丘, 公仲, 公上, 公门, 公山, 公坚, 左丘, 公伯, 西门, 公祖, 第五, 公乘, 贯丘, 公皙, 南荣, 东里, 东宫, 仲长, 子书, 子桑, 即墨, 达奚, 褚师, 吴铭
    }

  
    
}
