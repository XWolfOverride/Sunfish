using DolphinWebXplorer2.Configurator;
using System;

namespace DolphinWebXplorer2
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefineConfigurator : Attribute
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
