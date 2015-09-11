
using System;
using System.Linq;
using AutoMapper;
using System.Reflection;
using Dot.IOC;
using AutoMapper.Mappers;

namespace Dot.Adapter
{
    [Export(typeof(ITypeAdapterFactory))]
    public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
    {
        private bool _composed = false;

        public ITypeAdapter Create()
        {
            if (!_composed)
            {
                this.Compose();
                _composed = true;
            }

            return new AutomapperTypeAdapter();
        }

        private void Compose()
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    //RemoveDataReaderMapper();

                    this.ConfigureAutoMapper(cfg);
                });
            }
            catch (Exception ex)
            {
                Dot.Utility.Log.LogFactory.ExceptionLog.Error("初始化 Automapper的过程中出现错误:" + ex.Message);
            }
        }

        private static void RemoveDataReaderMapper()
        {
           
            //var item = MapperRegistry.Mappers.FirstOrDefault(m => m is DataReaderMapper);
            //if (item != null)
            //{
            //    MapperRegistry.Mappers.Remove(item);
            //}
           
        }

        private void ConfigureAutoMapper(IConfiguration cfg)
        {
            //AutoMapper Help
            //https://github.com/AutoMapper/AutoMapper/wiki
            //http://www.rqna.net/qna/yxzrz-automapper-bidirectional-mapping-with-reversemap-and-formember.html

            //基类执行程序集搜索、Mappter 初始化。
            //scan all assemblies finding Automapper Profile
            var profiles = DotEnvironment.GetAppPlugins()
                .SelectMany(p => p.Assembly.GetTypes())
                .Where(t => t.BaseType == typeof(AutoMapper.Profile))
                .Distinct().ToList();
            foreach (var item in profiles)
            {
                cfg.AddProfile(Activator.CreateInstance(item) as AutoMapper.Profile);
            }

            var maps = Mapper.GetAllTypeMaps();
            foreach (var map in maps)
            {
                
            }

            Dot.Utility.Log.LogFactory.PlatformLog.Debug("Domain Dto 映射完成");
        }
    }
}
