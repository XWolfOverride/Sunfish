using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Services.UI
{
    class ConfigurationMessage:ConfigurationElement
    {
        public enum MessageType
        {
            INFO,WARNING,ERROR
        }

        public ConfigurationMessage(MessageType t,string message) : base(null)
        {
            Type = t;
            Message = message;
        }

        public MessageType Type { get; }
        public string Message { get; }
    }
}
