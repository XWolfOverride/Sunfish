﻿using DolphinWebXplorer2.Middleware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(RootServiceConfigurator))]
    class RootService : SunfishService
    {
        public const string DIR_COMMON = "/$sunfish/";
        public const string DIR_API = "api/";

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
            WebUI.InitResources();
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
                if (path.StartsWith(DIR_API))
                {
                    // API
                    path = "/" + path.Substring(DIR_API.Length);
                    SunfishService servc = Sunfish.GetServiceForPath(ref path);
                    if (servc == null || servc is RootService)
                    {
                        ApiRest.WriteError("Service does not exists", call);
                    }
                    else
                    {
                        if (path.StartsWith("/"))
                            path = path.Substring(1);
                        if (path == "")
                        {
                            string meta;
                            if (!call.Parameters.TryGetValue("format", out meta))
                                meta = "json";
                            switch (meta)
                            {
                                case "js":
                                    ApiRest.WriteError("Not implemented", call);
                                    break;
                                case "json":
                                    ApiRest.WriteError("Not implemented", call);
                                    break;
                                default:
                                    ApiRest.WriteError("Format only supports 'js' and 'json'.", call);
                                    break;
                            }
                        }
                        else
                        {
                            //call method (path is method name)
                        }
                    }

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

        public static WebUILink LinkHome { get; private set; }

        public override string Description => "Root page service and API";
        public bool ShowMenu { get; set; } = true;
    }
}
