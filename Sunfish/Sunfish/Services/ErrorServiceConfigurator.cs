using DolphinWebXplorer2.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    class ErrorServiceConfigurator : SunfishServiceConfigurator
    {
        internal protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                Elements = new ConfigurationElement[]
                {
                    new ConfigurationMessage(ConfigurationMessage.MessageType.ERROR,"Error loading or initializing this service")
                }
            };
        }
    }
}
