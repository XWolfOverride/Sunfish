using DolphinWebXplorer2.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    [AttributeUsage(AttributeTargets.Class)]
    class DefineConfigurator : Attribute
    {
        public DefineConfigurator(Type configuratorType)
        {
            ConfiguratorType = configuratorType;
        }

        public SunfishServiceConfigurator Instantiate()
        {
            Type t = ConfiguratorType;
            return (SunfishServiceConfigurator)t.GetConstructor(new Type[0]).Invoke(new object[0]);
        }

        public Type ConfiguratorType { get; }
    }
}
