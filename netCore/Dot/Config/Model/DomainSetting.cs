using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Dot.Config.Model
{
    /// <summary>
    /// 服务配置信息-领域配置信息
    /// </summary>
    [Serializable]
    public class DomainSetting
    {
        private List<AggregateIdGeneratorItem> keyValueItems = new List<AggregateIdGeneratorItem>();

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("AggregateIdGeneratorItems")]
        public List<AggregateIdGeneratorItem> AggregateIdGeneratorItems
        {
            get
            {
                return this.keyValueItems;
            }
            set
            {
                this.keyValueItems = value;
            }
        }

        private List<AggregateIdGeneratorItemStrategy> aggregateIdGeneratorStrategys = new List<AggregateIdGeneratorItemStrategy>();

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("AggregateIdGeneratorStrategys")]
        public List<AggregateIdGeneratorItemStrategy> AggregateIdGeneratorItemStrategys
        {
            get
            {
                return this.aggregateIdGeneratorStrategys;
            }
            set
            {
                this.aggregateIdGeneratorStrategys = value;
            }
        }
    }

    public class AggregateIdGeneratorItem
    {
        private string key = "";
        /// <summary>
        /// 生成器key称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("GeneratorKey")]
        public string GeneratorKey
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

        private string value = "";
        /// <summary>
        /// id生成器实现
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("GeneratorValue")]
        public string GeneratorValue
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        private List<AggregateIdGeneratorAssembly> aggregateIdGeneratorAssemblys= new List<AggregateIdGeneratorAssembly>();

        /// <summary>
        /// 当前配置项应用的程序集
        /// </summary>
        [System.Xml.Serialization.XmlArray("AggregateIdGeneratorAssemblys")]
        public List<AggregateIdGeneratorAssembly> AggregateIdGeneratorAssemblys
        {
            get
            {
                return this.aggregateIdGeneratorAssemblys;
            }
            set
            {
                this.aggregateIdGeneratorAssemblys = value;
            }
        }
    }

    public class AggregateIdGeneratorAssembly
    {
        private string key = "";
        /// <summary>
        /// id生成器应用的程序集
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("AssemblyName")]
        public string AssemblyName
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
    }

    public class AggregateIdGeneratorItemStrategy
    {
        private string key = "";
        /// <summary>
        /// 聚合根类型名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("AggregateTypeName")]
        public string AggregateTypeName
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

        private string value = "";
        /// <summary>
        /// 当前聚合根类型中具体的
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Generator")]
        public string Generator
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}
