using DolphinWebXplorer2.Middleware;
using Json.Net;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace DolphinWebXplorer2.Services
{
    [DefineConfigurator(typeof(WebServiceConfigurator))]
    class WebService : SunfishService
    {
        private string index;
        private bool allowNavigation;
        private bool allowSubfolderNavigation;
        public WebService(SunfishServiceConfiguration ssc) : base(ssc)
        {
            vfs.AddVirtualFolder(null, new VFSFolderFileSystem(ssc.GetConf<string>(WebServiceConfigurator.CFG_PATH)));
            index = ssc.GetConf<string>(WebServiceConfigurator.CFG_INDEX);
            if (string.IsNullOrWhiteSpace(index))
                index = null;
            allowNavigation = ssc.GetConf<bool>(WebServiceConfigurator.CFG_SHARE);
            allowSubfolderNavigation = ssc.GetConf<bool>(WebServiceConfigurator.CFG_NAVIGATION);
        }

        private void ErrorPage(int code, HttpCall call, string text)
        {
            call.Response.StatusCode = code;
            call.Write(text);
        }

        private void Error500(HttpCall call, string text)
        {
            ErrorPage(500, call, text);
        }

        private void DownloadAt(string path, HttpCall call)
        {
            using (Stream s = vfs.OpenRead(path))
            {
                if (s == null)
                {
                    Error500(call, "Problem transfering file");
                    return;
                }
                call.Out.BaseStream.TransferFrom(s);
            }
        }

        private void DownloadAt(VFSItem item, HttpCall call)
        {
            using (Stream s = item.OpenRead())
            {
                if (s == null)
                {
                    Error500(call, "Problem transfering file");
                    return;
                }
                call.Out.BaseStream.TransferFrom(s);
            }
        }

        public override void Process(string path, HttpCall call)
        {
            if (path.EndsWith("/"))
            {
                VFSItem idx = vfs.GetItem(path + index);
                //Directory entry, go for index file or navigation
                if (index != null)
                {
                    if (idx != null && !idx.Folder)
                    {
                        DownloadAt(idx, call);
                        return;
                    }
                }
                if (allowNavigation)
                {
                    idx = vfs.GetItem(path);
                    if (idx != null && idx.Folder)
                        WriteIndex(idx, call);
                    else
                        call.NotFound();
                }
                else
                    call.Forbidden();
            }
            else
            {
                VFSItem idx = vfs.GetItem(path);
                if (idx != null)
                {
                    if (idx.Folder)
                        call.Redirect(call.Request.Url.LocalPath + "/");
                    else
                        DownloadAt(idx, call);
                }
                else
                    call.NotFound();
            }
        }

        private void WriteIndex(VFSItem dir, HttpCall call)
        {
            WebUI.InitResources();
            WebUI.WriteHeader(call);
            List<string> fileList = new List<string>();
            if (allowSubfolderNavigation)
            {
                foreach (string d in dir.ListDirectories())
                    fileList.Add(d);
                fileList.Sort();
                foreach (string d in fileList)
                {
                    WebUI.WriteItem(new WebUIListItem()
                    {
                        Name = d,
                        Description = "Directory",
                        Link = d + "/"
                    }, call);
                }
                fileList.Clear();
            }
            foreach (string d in dir.ListFiles())
                fileList.Add(d);
            fileList.Sort();
            foreach (string d in fileList)
            {
                WebUI.WriteItem(new WebUIListItem()
                {
                    Name = d,
                    Description = "File",
                    Link = d
                }, call);
            }
            WebUI.WriteFooter(call);
        }

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => "For Webpages or file sharing";
    }
}
