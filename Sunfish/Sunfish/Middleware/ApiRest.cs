using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class ApiRest
    {
        //JsonNet.Serialize
        public static void WriteError(string text, HttpCall call)
        {
            call.Write("{\"error\":\"" + text.Replace("\"", "\\\"") + "\"}");
        }
    }
}
