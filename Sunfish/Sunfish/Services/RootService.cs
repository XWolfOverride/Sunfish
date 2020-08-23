using DolphinWebXplorer2.Middleware;
using System;
using System.IO;
using System.Reflection;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(RootServiceConfigurator))]
    class RootService : SunfishService
    {
        public const string DIR_COMMON = "/$sunfish/";
        public const string DIR_API = "api/";
        public RootService() : base(new SunfishServiceConfiguration()
        {
            Enabled = true,
            Name = "ROOT",
            Location = "/"
        })
        {

        }

        public override void Process(string path, HttpCall call)
        {
            WebUI.InitResources();
            if (path == "/")
            {
                // Root page
                if (ShowMenu)
                {
                    WebUI.WriteHeader(call);
                    foreach (SunfishService s in Sunfish.Services)
                    {
                        if (!s.Enabled)
                            continue;
                        WebUI.WriteItem(new WebUIListItem()
                        {
                            Icon= "/$sunfish/folder.png",
                            Name = s.Configuration.Name,
                            Description = s.Configuration.Location,
                            Link = s.Configuration.Location,
                        }, call);
                    }
                    WebUI.WriteFooter(call);
                }
                else
                    call.Response.StatusCode = 404;
            }
            else if (path.StartsWith(DIR_COMMON))
            {
                path = path.Substring(DIR_COMMON.Length);
                if (path.StartsWith(DIR_API))
                {
                    // API
                    path = path.Substring(DIR_API.Length);
                }
                else if (path == "Sunfish.exe")
                {
                    // Self copy
                    call.Response.ContentType = "application/x-msdownload";
                    call.Write(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location));
                }
                else
                {
                    // Internal Resources
                    WebUI.WriteResource(path, call); //TODO: Change to internal resources ZIP
                }
            }
            else
                call.HTTPNotFound();
        }

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => "Root page service and API";
        public bool ShowMenu { get; set; } = true;
    }
}
