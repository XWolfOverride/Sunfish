using System;
using System.Reflection;

namespace Sunfish.Configurator
{
    public abstract class SunfishServiceConfigurator
    {
        internal static SunfishServiceConfigurator GetConfiguratorForService(Type t)
        {
            DefineConfigurator cfgr = t.GetCustomAttribute<DefineConfigurator>();
            if (cfgr == null)
                return null;
            return cfgr.Instantiate();
        }

        internal protected abstract ConfigurationScreen GetConfigurationScreen();
    }
}
