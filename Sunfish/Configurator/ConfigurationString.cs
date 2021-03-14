using System.Drawing;
using System.Windows.Forms;

namespace Sunfish.Configurator
{
    public class ConfigurationString : ConfigurationElement
    {
        public ConfigurationString(string id, string label) : base(id, label)
        {
        }

        public override bool isEmpty(Control c)
        {
            TextBox tb = c as TextBox;
            if (tb == null)
                return false;
            return string.IsNullOrWhiteSpace(tb.Text);
        }

        public override object getValue(Control c)
        {
            TextBox tb = c as TextBox;
            if (tb == null)
                return null;
            return tb.Text;
        }

        public string DefaultValue { get; set; }
        public bool IsPassword { get; set; }
        public bool IsDirectoiryPath { get; set; }
        public bool IsFilePath { get; set; }

        public override Color UIMandatoryColor => Color.LightGoldenrodYellow;
    }
}
