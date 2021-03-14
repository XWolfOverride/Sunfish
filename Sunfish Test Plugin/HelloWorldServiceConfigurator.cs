using Sunfish.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunfish_Test_Plugin
{
    class HelloWorldServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_THING = "Thing";
        internal const string CFG_PWD = "Pass";
        internal const string CFG_RONLY = "ReadOnly";

        protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                Elements = new ConfigurationElement[]
                {
                    new ConfigurationString(CFG_THING,"A Thing")
                    {
                        Tooltip = "A thing to write here (mandatory)",
                        Mandatory = true
                    },
                    new ConfigurationString(CFG_PWD,"Pass")
                    {
                        Tooltip = "A Password, (saved with a little security but not plain)",
                        IsPassword=true
                    },
                    new ConfigurationMessage(ConfigurationMessage.MessageType.INFO,"The below option do exactly nothing."),
                    new ConfigurationBool(CFG_RONLY,"Read only")
                    {
                        Tooltip = "Really nothing to modify",
                        DefaultValue = true,
                    },
                }
            };
        }
    }
}
