using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    class SunfishServiceConfiguration
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool BasePath { get; set; }
        public Dictionary<string, string> conf { get; set; } = new Dictionary<string, string>();
    }
}
