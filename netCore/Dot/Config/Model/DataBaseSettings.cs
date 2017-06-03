using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Dot.Config.Model
{
    /// <summary>
    /// 服务配置信息-数据库服务器配置
    /// </summary>
    [Serializable]
    public class DataBaseSetting
    {
        private string databaseType = "sqlite";
        /// <summary>
        /// 当前数据访问的数据库类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("DataBaseType")]
        public string DataBaseType
        {
            get
            {
                return this.databaseType;
            }
            set
            {
                this.databaseType = value;
            }
        }

        private List<ConnectionString> connectionStrings = new List<ConnectionString>();
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [System.Xml.Serialization.XmlArray("ConnectionStrings")]
        public List<ConnectionString> ConnectionStrings
        {
            get
            {
                return this.connectionStrings;
            }
            set
            {
                this.connectionStrings = value;
            }
        }

        private List<MappingAssembly> tempMappings = new List<MappingAssembly>();
        /// <summary>
        /// 当前数据实体和映射文件所在的程序集集合
        /// </summary>
        [System.Xml.Serialization.XmlArray("MappingAssemblys")]
        public List<MappingAssembly> MappingAssemblys
        {
            get
            {
                return this.tempMappings;
            }
            set
            {
                this.tempMappings = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public string ConfigFile
        {
            get
            {
                var str = string.Empty;

                str = string.Format("{0}-hibernate.cfg.xml", this.DataBaseType);
                str = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, str);

                return str;
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public List<Assembly> GetMappingAssemblys
        {
            get
            {
                List<Assembly> tempMappings = new List<Assembly>();
                var tempPath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (this.MappingAssemblys != null && this.MappingAssemblys.Count > 0)
                {
                    var tempAssemblyName = string.Empty;

                    foreach (var assembly in this.MappingAssemblys)
                    {
                        if (assembly == null || string.IsNullOrEmpty(assembly.AssemblyName))
                            continue;

                        if (!(assembly.AssemblyName.EndsWith(".dll") || assembly.AssemblyName.EndsWith(".DLL")))
                            tempAssemblyName = string.Format("{0}.dll", assembly.AssemblyName);
                        else
                            tempAssemblyName = assembly.AssemblyName;

                        var filePath = System.IO.Path.Combine(tempPath, tempAssemblyName);

                        if (!System.IO.File.Exists(filePath))
                            continue;

                        try
                        {
                            Assembly tempAssembly = Assembly.LoadFrom(filePath);
                            tempMappings.Add(tempAssembly);
                        }
                        catch
                        {
                        }
                    }
                }

                return tempMappings;
            }
        }

        public string GetConnectionString(string key)
        {
            string returnConnectionString = string.Empty;

            ConnectionString tempConnection = null;
            if (!string.IsNullOrEmpty(key))
            {
                tempConnection = this.ConnectionStrings.Where(pre => pre.Key.ToLower() == key.ToLower()).FirstOrDefault();
            }
            
            returnConnectionString = tempConnection == null ? this.GetDefaultConnectionString() : tempConnection.Value;

            return returnConnectionString;
        }

        private string GetDefaultConnectionString()
        {
            if (this.connectionStrings.Count == 0)
                return string.Empty;

           var tempConnection = this.ConnectionStrings.Where(pre => pre.Key.ToLower() == "DefaultConnectionString".ToLower()).FirstOrDefault();

           return tempConnection == null ? this.GetDefaultConnectionString() : tempConnection.Value;
        }
    }

    public class DataBaseType
    {
        public const string Sqlite = "sqlite";
        public const string SqlServer = "sqlserver";
        public const string Oracle = "oracle";
        public const string MySql = "mysql";
    }

    [Serializable]
    public class ConnectionString
    {
        private string key = string.Empty;
        /// <summary>
        /// 当前数据访问的数据库类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("key")]
        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        private string name = string.Empty;
        /// <summary>
        /// 当前数据访问的数据库类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("value")]
        public string Value
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private LoadBalanceDbType dbType = LoadBalanceDbType.Best;
        /// <summary>
        /// 当前数据访问的数据库类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("DbType")]
        public LoadBalanceDbType DbType
        {
            get
            {
                return this.dbType;
            }
            set
            {
                this.dbType = value;
            }
        }
    }

    [Serializable]
    public class MappingAssembly
    {
        private string name = string.Empty;
        /// <summary>
        /// 当前数据访问的数据库类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("AssemblyName")]
        public string AssemblyName
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
