using DolphinWebXplorer2.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    class SunfishServerProcessor
    {
        private HttpCall call;

        public SunfishServerProcessor(HttpCall call)
        {
            this.call = call;
            Process();
        }
        private void Process()
        {
            SunfishService s = Sunfish.GetServiceForPath(call.Request.Url.LocalPath);
            //Response.Headers[HttpResponseHeader.ContentType] = "text/plain";
            //call.OpenOutput();
            return;
            //if (s == null)
            //    if (Sunfish.RootMenu)
            //    {
            //        Error404();
            //        return;
            //    }
            //    else
            //    {
            //        Error404();
            //        return;
            //    }
        }
    }
}
