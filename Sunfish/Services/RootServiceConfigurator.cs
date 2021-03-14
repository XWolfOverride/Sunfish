using Sunfish.Configurator;

namespace Sunfish.Services
{
    class RootServiceConfigurator : SunfishServiceConfigurator
    {
        internal const string CFG_MENU = "rootMenu";

        internal protected override ConfigurationScreen GetConfigurationScreen()
        {
            return new ConfigurationScreen()
            {
                Elements = new ConfigurationElement[]
                {
                    new ConfigurationBool(CFG_MENU,"Show service menu")
                    {
                        Tooltip="Show a list of available services on the root page",
                    }
                }
            };
        }
    }
}
