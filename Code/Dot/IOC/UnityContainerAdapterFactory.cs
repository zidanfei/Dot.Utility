
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Dot.IOC
{
    class UnityContainerAdapterFactory : IObjectContainerFactory
    {
        public IObjectContainer CreateContainer()
        {
            var container = new UnityContainer();

            UnityAdapterHelper.OnUnityContainerCreated(new UnityContainerCreatedEventArgs(container));

            return new UnityContainerAdapter(container);
        }
    }
}