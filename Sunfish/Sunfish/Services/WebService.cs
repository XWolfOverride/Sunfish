using DolphinWebXplorer2.Middleware;
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
        private VFS vfs = new VFS();
        private string index;
        private bool allowNavigation;
        private bool allowDelete;
        private bool allowExec;
        private bool readOnly;
        public WebService(SunfishServiceConfiguration ssc) : base(ssc)
        {
            vfs.AddVirtualFolder(null, new VFSFolderFileSystem(ssc.GetConf<string>(WebServiceConfigurator.CFG_PATH)));
            index = ssc.GetConf<string>(WebServiceConfigurator.CFG_INDEX);
            if (string.IsNullOrWhiteSpace(index))
                index = null;
            allowNavigation = ssc.GetConf<bool>(WebServiceConfigurator.CFG_SHARE);
            allowDelete = ssc.GetConf<bool>(WebServiceConfigurator.CFG_DELETE);
            allowExec = ssc.GetConf<bool>(WebServiceConfigurator.CFG_EXECUTE);
            readOnly = ssc.GetConf<bool>(WebServiceConfigurator.CFG_RONLY);
        }

        #region WebServer

        private void ErrorPage(int code, HttpCall call, string text)
        {
            call.Response.StatusCode = code;
            call.Write(text);
        }

        private void Error500(HttpCall call, string text)
        {
            ErrorPage(500, call, text);
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
                call.Response.ContentType = MimeTypes.GetMimeType(Path.GetExtension(item.Name));
                call.Out.BaseStream.TransferFrom(s);
            }
        }

        public override void Process(string path, HttpCall call)
        {
            string action;
            if (!call.Parameters.TryGetValue("action", out action))
                action = null;
            else if (string.IsNullOrEmpty(action))
                action = null;
            switch (action)
            {
                case null:
                    ProcessGET(path, call);
                    break;
                case "icon":
                    ProcessIcon(path, call);
                    break;
                case "delete":
                    ProcessDelete(path, call);
                    break;
                case "rename":
                    ProcessRename(path, call);
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
                    //TODO: Block subfolder navigation if not allowed checking path
                    idx = vfs.GetItem(path);
                    if (idx != null && idx.Directory)
                        WriteIndex(path, idx, call);
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

        private void ProcessDelete(string path, HttpCall call)
        {
            VFSItem fil = vfs.GetItem(path);
            if (fil != null)
                call.Write(fil.Delete() ? "OK" : "KO");
            else
                call.Write("KO");
        }

        private void ProcessRename(string path, HttpCall call)
        {
            char[] forbiddenTo = { '/', '\\' };
            string to;
            if (!call.Parameters.TryGetValue("to", out to))
            {
                call.Write("KO (missing to)");
                return;
            }
            if (to.IndexOfAny(forbiddenTo) >= 0 || to == "." || to == "..")
            {
                call.Write("KO (forbidden)");
                return;
            }

            VFSItem fil = vfs.GetItem(path);
            if (fil != null)
                call.Write(fil.RenameTo(to) ? "OK" : "KO");
            else
                call.Write("KO");
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

        private void WriteIndex(string path, VFSItem dir, HttpCall call)
        {
            List<WebUILink> items = new List<WebUILink>();
            List<string> fileList = new List<string>();
            foreach (string d in dir.ListDirectories())
                fileList.Add(d);
            fileList.Sort();
            foreach (string d in fileList)
            {
                items.Add(new WebUILink()
                {
                    Icon = "/$sunfish/folder.png",
                    Name = d,
                    Description = "Directory",
                    Link = d + "/",
                    Style = "directory",
                    Actions = new WebUILink[]{
                        readOnly?null:new WebUILink()
                        {
                            Icon="drive_file_rename_outline",
                            Tooltip="Rename",
                            Click="sunfish.renameFile(this)"
                        },
                        allowDelete?new WebUILink()
                        {
                            Icon="delete",
                            Tooltip="Delete folder",
                            Click="sunfish.deleteFile(this);"
                        }:null
                    }
                });
            }
            fileList.Clear();
            foreach (string d in dir.ListFiles())
                fileList.Add(d);
            fileList.Sort();
            foreach (string d in fileList)
            {
                items.Add(new WebUILink()
                {
                    Icon = d + "?action=icon",
                    Name = d,
                    Description = "File",
                    Link = d,
                    Actions = new WebUILink[]{
                        allowExec?new WebUILink()
                        {
                            Icon="api",
                            Tooltip="Open in server",
                            Click="sunfish.openFile(this)"
                        }:null,
                        readOnly?null:new WebUILink()
                        {
                            Icon="drive_file_rename_outline",
                            Tooltip="Rename",
                            Click="sunfish.renameFile(this)"
                        },
                        allowDelete?new WebUILink()
                        {
                            Icon="delete",
                            Tooltip="Delete file",
                            Click="sunfish.deleteFile(this)"
                        }:null
                    }
                }); ;
            }
            WebUI.InitResources();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Breadcrumb"] = GetBreadcrumb(path);
            //data["Actions"] = actions;
            data["Items"] = items;
            data["Include"] = "<script src=\"/$sunfish/sunfish-directory.js\"></script>";
            WebUI.WriteTemplate("directory-index", call, data);
        }

        private WebUILink[] GetBreadcrumb(string path)
        {
            string link = "";
            path = Configuration.Location + path;
            List<WebUILink> lst = new List<WebUILink>();
            lst.Add(RootService.LinkHome);
            foreach (string p in path.Split('/'))
            {
                if (p.Length == 0)
                    continue;
                link += "/" + p;
                lst.Add(new WebUILink()
                {
                    Name = p,
                    Link = link,
                });
            }
            return lst.ToArray();
        }

        #endregion

        protected override void Start()
        {
        }

        protected override void Stop()
        {
        }

        public override string Description => "For Webpages or file sharing";
    }
}
