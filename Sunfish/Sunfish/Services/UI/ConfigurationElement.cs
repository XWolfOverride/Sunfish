using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services.UI
{
    public abstract class ConfigurationElement
    {
        protected ConfigurationElement(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public string Label { get; set; }
    }
}
