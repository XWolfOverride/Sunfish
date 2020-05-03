namespace DolphinWebXplorer2.Configurator
{
    public abstract class ConfigurationElement
    {
        protected ConfigurationElement(string id, string label)
        {
            Id = id;
            Label = label;
        }

        public string Id { get; }
        public string Label { get; }
        public string Tooltip { get; set; }
    }
}
