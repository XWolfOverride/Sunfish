using System.Collections.Generic;

namespace DolphinWebXplorer2
{
    public class SunfishServiceConfiguration
    {
        public T GetConf<T>(string key, T def = default)
        {
            object value;
            if (Settings.TryGetValue(key, out value))
                return (T) value;
            return def;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Location { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    }
}
