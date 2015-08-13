using System;
using System.Configuration;

namespace Dot.Utility.Memcached
{
	/// <summary>
	/// Summary description for MemcachedConfigSection.
	/// </summary>
	public class MemcachedConfigSection : IConfigurationSectionHandler
	{
		public MemcachedConfigSection()
		{

		}

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return null;
		}
	}
}
