namespace DolphinWebXplorer2.Configurator
{
    class ConfigurationMessage : ConfigurationElement
    {
        public enum MessageType
        {
            INFO, WARNING, ERROR
        }

        public ConfigurationMessage(MessageType t, string message) : base(null, null)
        {
            Type = t;
            Message = message;
        }

        public MessageType Type { get; }
        public string Message { get; }
    }
}
