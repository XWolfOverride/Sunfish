using Sunfish.Configurator;

namespace Sunfish.Services
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
