using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    public class SunfishServiceConfiguration
    {
        public string GetConf(string key, string def = null)
        {
            string value;
            if (Settings.TryGetValue(key, out value))
                return value;
            return def;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Location { get; set; }
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}
