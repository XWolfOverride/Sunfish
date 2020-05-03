using DolphinWebXplorer2.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    class SunfishServerProcessor : HttpServerProcessor
    {
        protected override void Process()
        {
            Error404();
        }
    }
}
