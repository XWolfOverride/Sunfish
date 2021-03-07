using System.Drawing;
using System.Windows.Forms;

namespace DolphinWebXplorer2.Configurator
{
    class ConfigurationMessage : ConfigurationElement
    {
        public enum MessageType
        {
            INFO, WARNING, ERROR
        }

        public ConfigurationMessage(MessageType t, string message) : base(null, null)
        {
            Type = t;
            Message = message;
        }

        public override bool isEmpty(Control c)
        {
            throw new System.NotImplementedException();
        }

        public override object getValue(Control c)
        {
            return null;
        }

        public MessageType Type { get; }
        public string Message { get; }

        public override Color UIMandatoryColor => throw new System.NotImplementedException();
    }
}
