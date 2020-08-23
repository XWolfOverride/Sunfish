using DolphinWebXplorer2.Middleware;
using Json.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            string meta;
            if (!call.Parameters.TryGetValue("meta", out meta))
                meta = null;
            else if (string.IsNullOrEmpty(meta))
                meta = null;
            switch (meta)
            {
                case null:
                    ProcessGET(path, call);
                    break;
                case "icon":
                    ProcessIcon(path, call);
                    break;
                default:
                    call.HTTPBadRequest();
                    break;
            }
        }

        private void ProcessGET(string path, HttpCall call)
        {
            if (path.EndsWith("/"))
            {
                VFSItem idx = vfs.GetItem(path + index);
                //Directory entry, go for index file or navigation
                if (index != null)
                {
                    if (idx != null && !idx.Directory)
                    {
                        DownloadAt(idx, call);
                        return;
                    }
                }
                if (allowNavigation)
                {
                    idx = vfs.GetItem(path);
                    if (idx != null && idx.Directory)
                        WriteIndex(idx, call);
                    else
                        call.HTTPNotFound();
                }
                else
                    call.HTTPForbidden();
            }
            else
            {
                VFSItem idx = vfs.GetItem(path);
                if (idx != null)
                {
                    if (idx.Directory)
                        call.Redirect(call.Request.Url.LocalPath + "/");
                    else
                        DownloadAt(idx, call);
                }
                else
                    call.HTTPNotFound();
            }
        }

        private void ProcessIcon(string path, HttpCall call)
        {
            if (path.EndsWith("/"))
            {
                call.HTTPForbidden();
            }
            else
            {
                VFSItem fil = vfs.GetItem(path);
                VFSFolder fold = fil.Folder;

                try
                {
                    if (fil.Length < 10485760) //10Mb
                        using (Stream fstream = fil.OpenRead())
                        using (Image i = Image.FromStream(fstream))
                        using (Image t = i.GetThumbnailImage(32, 32, null, IntPtr.Zero))
                        {
                            WritePNG((Bitmap)t, call);
                            return;
                        }
                }
                catch { };

                if (fold is VFSFolderFileSystem)
                {
                    string realpath = ((VFSFolderFileSystem)fold).GetFSPath(fil.Path);
                    using (ShellIcon i = new ShellIcon(realpath))
                        WritePNG(i.Image, call);
                }
                else
                {
                    using (ShellIcon i = new ShellIcon(path))
                        WritePNG(i.Image, call);
                }
            }
        }

        public void WriteIcon(Icon image, HttpCall call)
        {
            call.Response.ContentType = "image/vnd.microsoft.icon";
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms);
                ms.Position = 0;
                call.Write(ms);
            }
        }

        public void WritePNG(Image image, HttpCall call)
        {
            call.Response.ContentType = "image/png";
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                call.Write(ms);
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
                        Icon = "/$sunfish/folder.png",
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
                    Icon = d + "?meta=icon",
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
