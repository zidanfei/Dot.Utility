using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.RegularExpressions
{
    public class CommonRegex
    {
        static object lockObject = new object();
        static CommonRegex _init;
        CommonRegex()
        {

        }

        public static CommonRegex Initialize()
        {
            if (_init == null)
            {
                lock (lockObject)
                {
                    if (_init == null)
                        _init = new CommonRegex();
                }
            }
            return _init;
        }

        public static CommonRegex Init
        {
            get
            {
                if (_init == null)
                {
                    lock (lockObject)
                    {
                        if (_init == null)
                            _init = new CommonRegex();
                    }
                }
                return _init;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        System.Text.RegularExpressions.Regex _path_SpecialCharacters;

        /// <summary>
        /// 匹配windows路径不能包含的特别字符
        /// </summary>
        public System.Text.RegularExpressions.Regex WindowsPath_SpecialCharacters
        {
            get
            {
                if (_path_SpecialCharacters == null)
                {
                    lock (this)
                    {
                        if (_path_SpecialCharacters == null)
                            _path_SpecialCharacters = new System.Text.RegularExpressions.Regex("[:\\/*?\"<>|]");
                    }
                }
                return _path_SpecialCharacters;
            }
        }

        ~CommonRegex()
        {
            _init = null;
        }
    }
}
