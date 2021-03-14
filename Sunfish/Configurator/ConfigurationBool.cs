using System.Drawing;
using System.Windows.Forms;

namespace Sunfish.Configurator
{
    public class ConfigurationBool : ConfigurationElement
    {
        public ConfigurationBool(string id, string label) : base(id, label)
        {
        }

        public override bool isEmpty(Control c)
        {
            throw new System.NotImplementedException();
        }

        public override object getValue(Control c)
        {
            CheckBox cb = c as CheckBox;
            if (cb == null)
                return null;
            return cb.Checked;
        }

        public bool DefaultValue { get; set; }

        public override Color UIMandatoryColor => throw new System.NotImplementedException();
    }
}
