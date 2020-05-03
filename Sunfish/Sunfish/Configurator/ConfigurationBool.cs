namespace DolphinWebXplorer2.Configurator
{
    public class ConfigurationBool : ConfigurationElement
    {
        public ConfigurationBool(string id, string label) : base(id, label)
        {
        }

        public bool DefaultValue { get; set; }
    }
}
