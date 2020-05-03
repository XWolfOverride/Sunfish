using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Configurator
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
