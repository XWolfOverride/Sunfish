using System.Drawing;
using System.Windows.Forms;

namespace Sunfish.Configurator
{
    public abstract class ConfigurationElement
    {
        protected ConfigurationElement(string id, string label)
        {
            Id = id;
            Label = label;
        }

        public abstract bool isEmpty(Control c);

        public abstract object getValue(Control c);

        public string Id { get; }
        public string Label { get; }
        public string Tooltip { get; set; }
        public bool Mandatory { get; set; }

        public abstract Color UIMandatoryColor { get; }
    }
}
