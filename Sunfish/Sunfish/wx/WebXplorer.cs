using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using XWolf;
using System.Drawing;
using System.Drawing.Imaging;
using DolphinWebXplorer2.Properties;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace DolphinWebXplorer2.wx
{
    static class WebXplorer
    {
        private static List<WShared> shares = new List<WShared>();
        private static HttpServer server;
        public static byte[] res_logo;
        public static byte[] res_favicon;
        public static byte[] res_tdelete;
        public static byte[] res_texecute;
        public static byte[] res_rename;
        public static byte[] res_screen;
        public static byte[] res_folder;
        private static int port = 90;
        private static bool sharedScreen = false;
        private static string sharedScreenPwd;
        public static Random rnd = new Random();
        private static Dictionary<string, string> acodes = new Dictionary<string, string>();

        static WebXplorer()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Resources.sunfishWebServer.Save(ms);
                ms.Position = 0;
                res_favicon = ms.ToArray();
            }
            res_logo = GetImageData(Resources.sws, ImageFormat.Png);
            res_tdelete = GetImageData(Resources.t_delete, ImageFormat.Png);
            res_texecute = GetImageData(Resources.t_execute, ImageFormat.Png);
            res_rename = GetImageData(Resources.rename, ImageFormat.Png);
            res_screen = GetImageData(Resources.screen, ImageFormat.Png);
            res_folder = GetImageData(Resources.foldericon, ImageFormat.Png);
            Win32.DestroyIcon(Win32.GetIcon(".").hIcon);
            string letters = "abcdefghikjlmnopqrstuvwxyz1234567890";
            Random rnd=new Random();
            sharedScreenPwd = "";
            while (sharedScreenPwd.Length < 10)
                sharedScreenPwd += letters[rnd.Next(letters.Length)];
        }

        private static byte[] GetImageData(Image i, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                i.Save(ms, format);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public static void Add(WShared sh)
        {
            shares.Add(sh);
        }

        public static WShared Get(string index)
        {
            foreach (WShared w in shares)
                if (w.Name.Equals(index, StringComparison.CurrentCultureIgnoreCase))
                    return w;
            return null;
        }

        public static void Delete(string index)
        {
            shares.Remove(Get(index));
        }

        public static void Delete(WShared ws)
        {
            shares.Remove(ws);
        }

        public static void Start()
        {
            if (server != null)
                Stop();
            server = new HttpServer(port);
            server.CreateProcessor += new HttpServer.CreateProcessorHandler(server_CreateProcessor);
            server.Error += new HttpServer.ErrorEventHandler(server_Error);
            if (!server.Start())
                server = null;
        }

        static void server_Error(HttpServer server, Exception e)
        {
            if (!(e is ObjectDisposedException) && !(e is HttpListenerException))
                MessageBox.Show(e.Message + "\r\n" + e.StackTrace, e.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static HttpServerProcessor server_CreateProcessor(HttpServer server)
        {
            return new XplorerProcessor();
        }

        public static void Stop()
        {
            if (server == null)
                return;
            server.Stop();
            server = null;
        }

        public static void Save(string filename)
        {
            Dictionary<bool, string> YN = new Dictionary<bool, string>();
            YN[true] = "Y";
            YN[false] = "N";
            List<string> data = new List<string>();
            data.Add("#Sunfish WebXplorer V" + Program.VERSION + " (The new Dolphin WebXplorer) (C) XWolf 2014-2015");
            data.Add("Active: " + YN[Active]);
            data.Add("Port: " + port);
            data.Add("SharedScreen:" + YN[sharedScreen]);
            data.Add("SharedScreen.Password:" + sharedScreenPwd.ToLower());
            data.Add("");
            data.Add("#Each shared line is: Shared:<name>|<path>|<enabled>|<flags>");
            data.Add("# Where flags stands for");
            data.Add("#  ·S Allow sub folder navigation");
            data.Add("#  ·U Allow upload files");
            data.Add("#  ·D Allow file deletion");
            data.Add("#  ·R Allow rile rename");
            data.Add("#  ·X Allow remote execution (execute on server side)");
            data.Add("#  ·F Allow create folders");
            data.Add("#  ·T Send thumbnails icons");
            data.Add("");
            foreach (WShared s in shares)
                data.Add("Shared: " + s.Name + "|" + s.Path + "|" + YN[s.Enabled] + "|" + s.GetFlags());
            File.WriteAllLines(filename, data.ToArray());
        }

        public static void Load(string filename)
        {
            string[] data = File.ReadAllLines(filename);
            bool setactive = false;
            sharedScreen = false;
            shares.Clear();
            Stop();
            foreach (string l in data)
            {
                if (l.StartsWith("#") || l.Length==0)
                    continue;
                int dp = l.IndexOf(':');
                if (dp < 0)
                    continue;
                string cmd = l.Substring(0, dp);
                string par = l.Substring(dp + 1).Trim();
                switch (cmd.ToLower())
                {
                    case "active":
                        setactive = "Y".Equals(par);
                        break;
                    case "port":
                        int.TryParse(par, out port);
                        break;
                    case "sharedscreen":
                        sharedScreen = "Y".Equals(par);
                        break;
                    case "sharedscreen.password":
                        sharedScreenPwd = par;
                        break;
                    case "shared":
                        {
                            string[] s = par.Split('|');
                            string sname = s.Length > 0 ? s[0] : null;
                            string spath = s.Length > 1 ? s[1] : null;
                            if (sname != null && spath != null)
                            {
                                WShared sh = new WShared(sname, spath);
                                sh.Enabled = "Y".Equals(s.Length > 2 ? s[2] : null);
                                sh.SetFlags(s.Length > 3 ? s[3] : "");
                                Add(sh);
                            }
                        }
                        break;
                }
            }
            if (setactive)
                Start();
        }

        public static bool Contains(WShared sh)
        {
            return shares.Contains(sh);
        }

        public static string CreateACode(string key)
        {
            string result = GenACode();
            acodes[key] = result;
            return result;
        }

        public static bool CheckACode(string key,string acode)
        {
            if (acodes.ContainsKey(key))
            {
                bool result = acode == acodes[key];
                acodes.Remove(key);
                return result;
            }
            return false;
        }

        private static string GenACode()
        {
            string allowed = "_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder(16);
            while (result.Length < 16)
                result.Append(allowed[rnd.Next(allowed.Length)]);
            return result.ToString();
        }

        public static WShared[] Shares { get { return shares.ToArray(); } }
        public static bool Active { get { return server != null; } }
        public static int Port { get { return port; } set { if (Active) throw new Exception("Can't change port while running"); else port = value; } }
        public static bool SharedScreen { get { return sharedScreen; } set { sharedScreen = value; } }
        public static string SharedScreenPassword { get { return sharedScreenPwd; } set { sharedScreenPwd = value; } }
    }

    class XplorerProcessor : HttpServerProcessor
    {
        protected override void Process()
        {
            string path = Path.Substring(1);
            if (path.StartsWith("·") || path == "favicon.ico")
                Resource(path.Substring(1));
            if (path.StartsWith("$"))
                Special(path.Substring(1));
            else
            {
                string shpath = path.Contains('/') ? path.Substring(0, path.IndexOf('/')) : null;
                string rpath = shpath == null ? null : path.Substring(shpath.Length + 1);
                string action = GET.ContainsKey("Action") ? GET["Action"] : null;
                rpath = UDec(rpath);
                WShared sh = WebXplorer.Get(shpath);
                if (sh == null)
                    Menu();
                else if (action != null)
                {
                    switch (action)
                    {
                        case "ICO":
                            GetIcon(sh, rpath);
                            break;
                        case "GET":
                            Download(sh, rpath);
                            break;
                        case "REN":
                            PageRename(sh, rpath);
                            break;
                        case "RENDO":
                            PageRenameDo(sh, rpath);
                            break;
                        case "RUN":
                            PageExecute(sh, rpath);
                            break;
                        case "DEL":
                            PageDelete(sh, rpath);
                            break;
                        case "DELDO":
                            PageDeleteDo(sh, rpath);
                            break;
                        case "MKD":
                            PageCreateFolder(sh, rpath);
                            break;
                        case "MKDDO":
                            PageCreateFolderDo(sh, rpath);
                            break;
                    }
                }
                else
                    if (sh.Enabled)
                        Shared(sh, rpath);
                    else
                        Menu();
            }
        }

        private void Resource(string res)
        {
            switch (res)
            {
                case "logo":
                    BinaryOut(WebXplorer.res_logo, "image/png");
                    break;
                case "avicon.ico":
                    BinaryOut(WebXplorer.res_favicon, "image/vnd.microsoft.icon");
                    break;
                case "t_delete":
                    BinaryOut(WebXplorer.res_tdelete, "image/png");
                    break;
                case "t_execute":
                    BinaryOut(WebXplorer.res_texecute, "image/png");
                    break;
                case "rename":
                    BinaryOut(WebXplorer.res_rename, "image/png");
                    break;
                case "screen":
                    BinaryOut(WebXplorer.res_screen, "image/png");
                    break;
                case "folder":
                    BinaryOut(WebXplorer.res_folder, "image/png");
                    break;
                case "/Sunfish.exe":
                    BinaryOut(GetMyself(), "application/x-msdownload");
                    break;
            }
        }

        private void Special(string path)
        {
            if (path == "screen")
            {
                BlackHeader("Shared Screen", global::DolphinWebXplorer2.Properties.Resources.ShScreen);
                Out.Write("<div id='frm'>Password: <input id='scpwd' type='password'>&nbsp;<button onclick='shs.start()'>Open</button></div>");
                Out.Write("<div id='sh'><img id='scr'></div>");
                BlackFooter();
                return;
            }
            if (path == "screencap")
            {
                if (GET["code"] != WebXplorer.SharedScreenPassword)
                    return;
                Screen scr = Program.MAINFORM.MyScreen;
                MemoryStream ms = new MemoryStream();
                using (Bitmap bmp = new Bitmap(scr.Bounds.Width, scr.Bounds.Height, PixelFormat.Format16bppRgb565))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(scr.Bounds.X, scr.Bounds.Y, 0, 0, scr.Bounds.Size);
                    ImageCodecInfo jgpEncoder = GetEncoder();
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 35L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    bmp.Save(ms, jgpEncoder, myEncoderParameters);
                    ms.Close();
                }
                BinaryOut(ms.ToArray(), "image/jpeg");
                return;
            }
            if (path == "screencmd")
            {
                if (GET["code"] != WebXplorer.SharedScreenPassword)
                    return;
                string cmd = GET["cmd"];
                if (cmd.Length == 0)
                    return;
                string cc = cmd.Substring(0, 2);
                string par = cmd.Substring(2);
                int x;
                int y;
                string[] pars = par.Split(';');
                int.TryParse(pars[0], out x);
                int.TryParse(pars[1], out y);
                Screen scr = Program.MAINFORM.MyScreen;
                System.Windows.Forms.Cursor.Position = new Point(x + scr.Bounds.X, y + scr.Bounds.Y);
                switch (cc)
                {
                    case "LD":
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        break;
                    case "LU":
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        break;
                    case "RD":
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                        break;
                    case "RU":
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                        break;
                    case "WH":
                        Thread.Sleep(100);
                        int wheel;
                        int.TryParse(pars[2], out wheel);
                        mouse_event(MOUSEEVENTF_WHEEL, 0, 0, wheel, 0);
                        break;
                }
                Out.Write("Ok");
                return;
            }
        }

        #region WINAPI
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* left button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* left button up */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* The wheel has been moved, if the mouse has a wheel. The amount of movement is specified in dwData */
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        #endregion

        private ImageCodecInfo jpegEncoder = null;

        private ImageCodecInfo GetEncoder()
        {
            if (jpegEncoder == null)
                foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                    if (codec.FormatID == ImageFormat.Jpeg.Guid)
                    {
                        jpegEncoder = codec;
                        break;
                    }
            return jpegEncoder;
        }

        private void GetIcon(WShared sh, string path)
        {
            path = sh.GetLocalPath(path);
            try
            {
                if (sh.SendThumbnails)
                    try
                    {
                        FileInfo fi = new FileInfo(path);
                        if (fi.Length < 10485760) //10Mb
                            using (Image i = Image.FromFile(path))
                            using (Image t = i.GetThumbnailImage(32, 32, null, IntPtr.Zero))
                            {
                                BinaryOut((Bitmap)t);
                                return;
                            }
                    }
                    catch { };
                using (ShellIcon i = new ShellIcon(path))
                    BinaryOut(i.Image);
            }
            catch (Exception e)
            {
                Out.WriteLine(e.GetType().Name);
                Out.WriteLine(e.Message);
            }
        }

        private void BinaryOut(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BinaryOut(ms, "image/png");
            }
        }

        private void BinaryOut(Icon image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms);
                ms.Position = 0;
                BinaryOut(ms, "image/vnd.microsoft.icon");
            }
        }

        private void BinaryOut(MemoryStream ms, string mimetype)
        {
            Response.Headers[HttpResponseHeader.ContentType] = mimetype;
            Write(ms.ToArray());
        }

        private void BinaryOut(FileStream fs, string mimetype)
        {
            if (mimetype != null)
                Response.Headers[HttpResponseHeader.ContentType] = mimetype;
            byte[] buffer = new byte[1024 * 64];
            int readed = buffer.Length;
            while (readed == buffer.Length)
            {
                readed = fs.Read(buffer, 0, buffer.Length);
                Write(buffer, 0, readed);
            }
        }

        private void BinaryOut(byte[] data, string mimetype)
        {
            Response.Headers[HttpResponseHeader.ContentType] = mimetype;
            Write(data);
        }

        private byte[] GetMyself()
        {
            return File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
        }

        private void Menu()
        {
            Header("Home", null, null);
            foreach (WShared s in WebXplorer.Shares)
            {
                if (!s.Enabled)
                    continue;
                ItemDirectory(null, s.Name, '/' + s.Name + '/');
            }
            Footer();
        }

        private void Shared(WShared sh, string path)
        {
            if (!sh.AllowSubfolders)
                path = "";
            Header(sh.Name, sh, path.Split('/'));
            string rpath = sh.GetLocalPath(path);
            try
            {
                if (sh.AllowSubfolders)
                {
                    foreach (string dir in Directory.GetDirectories(rpath))
                    {
                        string name = System.IO.Path.GetFileName(dir);
                        ItemDirectory(sh, name, sh.GetRemotePath(path + name) + '/');
                    }
                }
                foreach (string fil in Directory.GetFiles(rpath))
                {
                    string name = System.IO.Path.GetFileName(fil);
                    ItemFile(sh, name, sh.GetRemotePath(path + name), fil);
                }
            }
            catch (Exception e)
            {
                Error(e);
            }
            Footer();
        }

        private void Download(WShared sh, string path)
        {
            string rpath = sh.GetLocalPath(path);
            string name = System.IO.Path.GetFileName(rpath);
            try
            {
                using (FileStream fs = new FileStream(rpath, FileMode.Open, FileAccess.Read))
                {
                    //Response.Headers["Content-Disposition"] = "attachment; filename=\"" + name + "\"";
                    BinaryOut(fs, Win32.GetMimeType(name));
                }
            }
            catch (Exception e)
            {
                Header(sh.Name, sh, path.Split('/'));
                Error(e);
                Footer();
            }
        }

        private void PageRename(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowRename)
                Error("Forbidden", "Renaming is not allowed on this site");
            else
            {
                string oname = System.IO.Path.GetFileName(path);
                string acode = WebXplorer.CreateACode("R" + path);
                Out.Write("<div class='activity'><form action=''>");
                Out.Write("<input type='hidden' name='Action' value='RENDO'>");
                Out.Write("<input type='hidden' name='acode' value='");
                Out.Write(acode);
                Out.Write("'>");
                Out.Write("Renaming file: ");
                Out.Write(oname);
                Out.Write("<br/>Enter new name:<br/><input name='newname' value='");
                Out.Write(oname);
                Out.Write("'><input type='button' value='Cancel' onclick='history.back();'><input type='submit' value='Ok'>");
                Out.Write("</form></div>");
            }
            Footer();
        }

        private void PageRenameDo(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowRename)
                Error("Forbidden", "Renaming is not allowed on this site");
            else if (!WebXplorer.CheckACode("R" + path,GET["acode"]))
                Error("Auth code error", "<script>history.go(-2);</script>");
            else
            {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (!dir.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) && !(dir.Length==0))
                    dir+=System.IO.Path.DirectorySeparatorChar;
                string oname = System.IO.Path.GetFileName(path);
                string nname = GET["newname"];
                if (oname != nname)
                    if (nname.Contains("/") || nname.Contains("\\") || nname.Contains(":"))
                        Error("New File error", "Path separator char not allowed as file name");
                    else
                        try
                        {
                            File.Move(path, dir + nname);
                            Out.WriteLine("<script>history.go(-2);</script>");
                        }
                        catch (Exception e)
                        {
                            Error(e);
                        }
            }
            Footer();
        }

        private void PageExecute(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowExecution)
                Error("Forbidden", "Remote execution is not allowed on this site");
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(path);
                    Out.WriteLine("<script>history.back();</script>");
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
            Footer();
        }

        private void PageDelete(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowDeletion)
                Error("Forbidden", "Deletion is not allowed on this site");
            else
            {
                string oname = System.IO.Path.GetFileName(path);
                string acode = WebXplorer.CreateACode("D" + path);
                Out.Write("<div class='activity'><form action=''>");
                Out.Write("<input type='hidden' name='Action' value='DELDO'>");
                Out.Write("<input type='hidden' name='acode' value='");
                Out.Write(acode);
                Out.Write("'>");
                Out.Write("Delete file <b>");
                Out.Write(oname);
                Out.Write("</b>?<br/><input type='button' value='No' onclick='history.back();'><input type='submit' value='Yes' name='really'>");
                Out.Write("</form></div>");
            }
            Footer();
        }

        private void PageDeleteDo(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowDeletion)
                Error("Forbidden", "Deletion is not allowed on this site");
            else if (!WebXplorer.CheckACode("D" + path, GET["acode"]))
                Error("Auth code error", "<script>history.go(-2);</script>");
            else
            {
                try
                {
                    File.Delete(sh.GetLocalPath(path));
                    Out.WriteLine("<script>history.go(-2);</script>");
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
            Footer();
        }

        private void PageCreateFolder(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowNewFolder)
                Error("Forbidden", "Folder creation is not allowed on this site");
            else
            {
                string oname = System.IO.Path.GetFileName(path);
                string acode = WebXplorer.CreateACode("C" + path);
                Out.Write("<div class='activity'><form action=''>");
                Out.Write("<input type='hidden' name='Action' value='MKDDO'>");
                Out.Write("<input type='hidden' name='acode' value='");
                Out.Write(acode);
                Out.Write("'>");
                Out.Write("Create Directory: ");
                Out.Write("<br/>Enter new name:<br/><input name='dirname' value='");
                Out.Write(oname);
                Out.Write("'><input type='button' value='Cancel' onclick='history.back();'><input type='submit' value='Ok'>");
                Out.Write("</form></div>");
            }
            Footer();
        }

        private void PageCreateFolderDo(WShared sh, string path)
        {
            Header(sh.Name, sh, path.Split('/'));
            if (!sh.AllowNewFolder)
                Error("Forbidden", "Folder creation is not allowed on this site");
            else if (!WebXplorer.CheckACode("C" + path, GET["acode"]))
                Error("Auth code error", "<script>history.go(-2);</script>");
            else
            {
                string dname = GET["dirname"];
                if (dname.Contains("/") || dname.Contains("\\") || dname.Contains(":"))
                    Error("Create directory error", "Path separator char not allowed as file name");
                string dir = path.Replace('/',System.IO.Path.DirectorySeparatorChar);
                if (!dir.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) && !(dir.Length == 0))
                    dir += System.IO.Path.DirectorySeparatorChar;
                try
                {
                    Directory.CreateDirectory(sh.GetLocalPath(dir + dname));
                    Out.WriteLine("<script>history.go(-2);</script>");
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
            Footer();
        }

        private void BlackHeader(string title, string script)
        {
            Out.Write("<html><head><title>Sunfish [");
            Out.Write(title);
            Out.Write("]</title><style>");
            Out.Write(global::DolphinWebXplorer2.Properties.Resources.siteblack);
            Out.Write("</style>");
            if (script != null)
            {
                Out.Write("<script>");
                Out.Write(script);
                Out.Write("</script>");
            }
            Out.Write("</head><body><div id='ctt'><div id='main'>");
        }

        private void BlackFooter()
        {
            Out.Write("</div></div></body></html>");
        }

        private void Header(string title, WShared sh, string[] path)
        {
            Out.Write("<html><head><title>Sunfish [");
            Out.Write(title);
            Out.Write("]</title><style>");
            Out.Write(global::DolphinWebXplorer2.Properties.Resources.site);
            Out.Write("</style></head><body><div id='head'><a href='/·/Sunfish.exe' title='Download this Sunfish server'>Sunfish V");
            Out.Write(Program.VERSION);
            Out.Write("</a><div id='breadcrum'><a href='/'>ROOT</a> / ");
            string fpath = "/";
            if (sh != null)
            {
                string shared=sh.Name;
                fpath += shared + "/";
                Out.Write("<a href='");
                Out.Write(UEnc(fpath));
                Out.Write("'>");
                Out.Write(shared);
                Out.Write("</a> / ");
            }
            if (path != null)
            {
                foreach (string part in path)
                {
                    if (part.Length == 0)
                        continue;
                    fpath += part + "/";
                    Out.Write("<a href='");
                    Out.Write(UEnc(fpath));
                    Out.Write("'>");
                    Out.Write(part);
                    Out.Write("</a> / ");
                }
            }
            Out.Write("</div>");
            Out.Write("<div id='headtoolbox'>");
            if (sh != null && sh.AllowNewFolder)
                Out.Write("<button class='htool' title='New Folder' onclick='location.href+=\"?Action=MKD\"'><img src='/·folder' width='16' height='16'></button>");
            if (WebXplorer.SharedScreen)
                Out.Write("<button class='htool' title='Simple remote control' onclick='location.href=\"/$screen\"'><img src='/·screen' width='16' height='16'></button>");
            Out.Write("</div>");
            Out.Write("</div><div id='main'>");
        }

        private void Footer()
        {
            Out.Write("</div></body></html>");
        }

        private string UEnc(string url)
        {
            return url == null ? null : url.Replace("#", "|1");
        }

        private string UDec(string url)
        {
            return url == null ? null : url.Replace("|1", "#");
        }

        private void ItemDirectory(WShared sh, string name, string path)
        {
            ItemBegin(path, null, name, "Directory");
            if (sh != null)
            {
                if (sh.AllowExecution)
                    ItemTool(path, "REN", "Rename folder", "rename");
                if (sh.AllowDeletion)
                    ItemTool(path, "DEL", "Delete folder", "t_delete");
            }
            ItemEnd();
        }

        private void ItemFile(WShared sh, string name, string path, string realpath)
        {
            FileInfo f = new FileInfo(realpath);
            ItemBegin(path, "?Action=GET", name, FBytes(f.Length));
            if (sh != null)
            {
                if (sh.AllowExecution)
                    ItemTool(path, "RUN", "Execute on server", "t_execute");
                if (sh.AllowExecution)
                    ItemTool(path, "REN", "Rename file", "rename");
                if (sh.AllowDeletion)
                    ItemTool(path, "DEL", "Delete file", "t_delete");
            }
            ItemEnd();
        }

        private void ItemBegin(string path, string param, string name, string info)
        {
            Out.Write("<div class='item' onclick='firstChild.click()' title='");
            Out.Write(name);
            Out.Write("'><a href='");
            Out.Write(UEnc(path));
            if (param != null)
                Out.Write(param);
            Out.Write("'><img src='");
            Out.Write(UEnc(path));
            Out.Write("?Action=ICO' width=32 height=32 border=0><div class='iname'>");
            Out.Write(name);
            Out.Write("</a></div><div class='iinfo'>");
            Out.Write(info);
            Out.Write("</div>");
            Out.Write("<div class='itool'>");
        }

        private void ItemTool(string path, string action, string title, string icon)
        {
            Out.Write("<a class='tbt' href='");
            Out.Write(UEnc(path));
            if (action != null)
            {
                Out.Write("?Action=");
                Out.Write(action);
            }
            Out.Write("' title='");
            Out.Write(title);
            Out.Write("'><img src='/·");
            Out.Write(icon);
            Out.Write("' width=16 height=16 border=0></a>");
        }

        private void ItemEnd()
        {
            Out.Write("</div>");
            Out.Write("</div>");
        }

        private string FBytes(double lng)
        {
            string[] tail = { " bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Yb" };
            int taili = 0;
            while (lng > 1024)
            {
                lng /= 1024;
                taili++;
            }
            return lng.ToString("#0.00") + (taili >= tail.Length ? "^b" : tail[taili]);
        }

        private void Error(Exception e)
        {
            Error(e.GetType().Name, e.Message.Replace("\n", "<br>"));
        }

        private void Error(string title, string message)
        {
            Out.Write("<div class='error'>");
            Out.Write(title);
            Out.Write("<br/>");
            Out.Write(message);
            Out.Write("</div>");
        }
    }
}
