using DolphinWebXplorer2.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services
{
    class AdminServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_ADMIN_PWD = "adminPwd";
      
        internal protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                Elements = new ConfigurationElement[]
                {
                    new ConfigurationString(CFG_ADMIN_PWD)
                    {
                        Tooltip="Password for the administration panel.\n Leave it blank for no password access (not recommended)",
                        IsPassword=true,
                        Label="Admin password: (blank for none)"
                    }
                }
            };
        }
    }
}
