using Dot.Adapter;
//using Dot.AOP;
using Dot.Config.Model;
using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.ComponentModel
{
 
    public class DotAppStart : AppImplementationBase
    {
        protected override void InitEnvironment()
        {
            PluginTable.Plugins.AddPlugin<DotPlugin>();
            base.InitEnvironment();
        }
        protected override void RaiseComposeOperations()
        {
            //InterceptionHelper.LoadByIOCSettings();

            base.RaiseComposeOperations();

            //ObjectContainerFactory.CreateContainer().RegisterType<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
           
        }

       
    }
}