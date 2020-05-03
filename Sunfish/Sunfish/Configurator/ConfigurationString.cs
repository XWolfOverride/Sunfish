using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Configurator
{
    public class ConfigurationString:ConfigurationElement
    {
        public ConfigurationString(string id) : base(id)
        {
        }

        public string DefaultValue { get; set; }
        public bool IsPassword { get; set; }
        public bool IsDirectoiryPath { get; set; }
        public bool IsFilePath { get; set; }
    }
}
