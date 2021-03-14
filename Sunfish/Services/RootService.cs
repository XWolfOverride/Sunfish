using Sunfish.Middleware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Sunfish.Services
{
    [DefineConfigurator(typeof(RootServiceConfigurator))]
    class RootService : SunfishService
    {
        public const string DIR_COMMON = "/$sunfish/";

        static RootService()
        {
            LinkHome = new WebUILink()
            {
                Icon = "home",
                Link = "/",
                Tooltip = "Root",
            };
        }

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
            if (path == "/")
            {
                // Root page
                if (ShowMenu)
                {
                    List<WebUILink> items = new List<WebUILink>();
                    foreach (SunfishService s in Sunfish.Services)
                    {
                        if (!s.Enabled)
                            continue;
                        items.Add(new WebUILink()
                        {
                            Icon = "/$sunfish/folder.png",
                            Name = s.Configuration.Name,
                            Description = s.Configuration.Location,
                            Link = s.Configuration.Location,
                        });
                    }
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data["Breadcrumb"] = new WebUILink[] { LinkHome };
                    //data["Actions"] = actions;
                    data["Items"] = items;
                    WebUI.WriteTemplate("directory-index", call, data);
                }
                else
                    call.Response.StatusCode = 404;
            }
            else if (path.StartsWith(DIR_COMMON))
            {
                path = path.Substring(DIR_COMMON.Length);
                if (path == "Sunfish.exe")
                {
                    // Self copy
                    call.Response.ContentType = "application/x-msdownload";
                    call.Write(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location));
                }
                else if (path == "info")
                {
                    WebUI.WriteTemplate("sunfish-header", call, null);
                    call.Write("<p>Sunfish " + Program.VERSION + " (C) XWolfOverride</p>");
#if DEBUG
                    call.Write("<p>Debug build</p>");
                    if (WebUI.EXTERNAL_RESOURCES)
                        call.Write("<p>Using external resources</p>");
                    else
                        call.Write("<p>Using internal resources</p>");
#else
                    call.Write("<p>Release build</p>");
#endif
                    if (WebUI.Ready)
                        call.Write("<p>Resources loaded succesfully</p>");
                    else
                        call.Write("<p>No resources</p>");
                    call.Write("<p><b><a href='info/seal'>Seal resources</a></b></p>");
                    WebUI.WriteTemplate("sunfish-footer", call, null);

                } else if (path == "info/seal")
                {
                    WebUI.Seal();
                    call.Response.StatusCode = 307;
                    call.Response.Headers["Location"] = "/$sunfish/info";
                }
                else

                {
                    // Internal Resources
                    WebUI.WriteResource(path, call);
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

        public static WebUILink LinkHome { get; private set; }

        public override string Description => "Root page service and API";
        public bool ShowMenu { get; set; } = true;
    }
}
