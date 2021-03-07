using DolphinWebXplorer2.Middleware;
using System.Net;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(ErrorServiceConfigurator))]
    class ErrorService : SunfishService
    {
        public ErrorService(SunfishServiceConfiguration ssc) : base(ssc)
        {
        }

        public override void Process(string path, HttpCall call)
        {
            call.Response.Headers[HttpResponseHeader.ContentType] = "text/plain";
            call.Write("Error");
        }

        public static void Process404(HttpCall call)
        {
            //call.Response.Headers[HttpResponseHeader.ContentType] = "text/plain";
            call.Response.StatusCode = 404;
            //call.Response.StatusDescription = "Not found";
            //call.Write("Not found");
            call.Out.Close();
        }


        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => null;
    }
}
