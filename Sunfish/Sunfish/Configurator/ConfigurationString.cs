namespace DolphinWebXplorer2.Configurator
{
    public class ConfigurationString : ConfigurationElement
    {
        public ConfigurationString(string id, string label) : base(id, label)
        {
        }

        public string DefaultValue { get; set; }
        public bool IsPassword { get; set; }
        public bool IsDirectoiryPath { get; set; }
        public bool IsFilePath { get; set; }
    }
}
