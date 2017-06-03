using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot.Adapter
{
    public class DotMapperConfiguration : MapperConfiguration
    {
        public DotMapperConfiguration(MapperConfigurationExpression configurationExpression):base(configurationExpression)
        {
        }
        public DotMapperConfiguration(Action<IMapperConfigurationExpression> configure) : base(configure)
        {
        }

        
    }
}
