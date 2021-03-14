using Sunfish.Configurator;

namespace Sunfish.Services
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
                    new ConfigurationString(CFG_ADMIN_PWD,"Admin password: (blank for none)")
                    {
                        Tooltip="Password for the administration panel.\n Leave it blank for no password access (not recommended)",
                        IsPassword=true,
                    }
                }
            };
        }
    }
}
