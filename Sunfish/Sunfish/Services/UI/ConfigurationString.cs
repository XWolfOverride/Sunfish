using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services.UI
{
    public class ConfigurationString:ConfigurationElement
    {
        public ConfigurationString(string id,string defvalue) : base(id)
        {
            DefaultValue = defvalue;
        }

        public string DefaultValue { get; }
    }
}
