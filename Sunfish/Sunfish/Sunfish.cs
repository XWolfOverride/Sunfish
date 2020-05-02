using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DolphinWebXplorer2
{
    class SunfishServiceConfiguration
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public Dictionary<string, string> conf { get; set; } = new Dictionary<string, string>();
    }

    class Sunfish
    {
        public const string DEFAULT_CONF_FILE = "sunfish2";
        private List<SunfishServiceConfiguration> conf = new List<SunfishServiceConfiguration>();

        static Sunfish()
        {
            Load();
        }
        
        static void Load()
        {
            if (!File.Exists(DEFAULT_CONF_FILE))
                return;
            //JsonSerializer
        }

        static void Save()
        {
            //JsonSerializer
        }

    }
}
