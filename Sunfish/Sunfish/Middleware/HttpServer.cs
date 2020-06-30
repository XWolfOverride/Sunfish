using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace DolphinWebXplorer2.Middleware
{
    class HttpServer
    {
        private int port;
        private HttpListener lis = null;
        private Thread loop = null;
        private List<Thread> processors = new List<Thread>();
        private List<LogError> errors = new List<LogError>();
        private Processor prc;
        //private WindowsFirewallRule fwRule;
        public delegate void ErrorEventHandler(HttpServer server, Exception e);
        public delegate void Processor(HttpServer server, HttpCall call);
        public event ErrorEventHandler Error;

        public HttpServer(int port, Processor prc)
        {
            this.port = port;
            this.prc = prc;
        }

        public void Start()
        {
            if (lis != null)
                return;
            lis = new HttpListener();

            // WARNING: This require the server to be launched as administrative process
            //  This is very problematic due the actions that the server do are width administrator
            //  rights, even the (future) embedded execution of C#, the file upload and deletion
            //  and also the execution of server applications.

            lis.Prefixes.Add("http://+:" + port + "/");

            try
            {
                lis.Start();
            }
            catch (HttpListenerException e)
            {
                lis = null;
                throw e;
            }
            loop = new Thread(ServerLoop);
            loop.Start();
            //fwRule = WindowsFirewall.Allow(port);
        }

        private void ServerLoop()
        {
            for (; ; )
            {
                HttpListenerContext ctx = lis.GetContext();
                new Thread(new ParameterizedThreadStart(MultithreadCall)).Start(ctx);
            }
        }

        private void MultithreadCall(object octx)
        {
            lock (processors)
            {
                processors.Add(Thread.CurrentThread);
            }
            try
            {
                CallNewProcessor((HttpListenerContext)octx);
            }
            catch (Exception e)
            {
                LogError(e);
            }
            finally
            {
                lock (processors)
                {
                    processors.Remove(Thread.CurrentThread);
                }
            }
        }

        private void CallNewProcessor(HttpListenerContext ctx)
        {
            HttpCall call = new HttpCall(ctx);
            try
            {
                prc(this, call);
            }
            finally
            {
                call.Close();
            }
        }

        public bool Stop()
        {
            if (lis == null)
                return false;
            if (loop != null)
            {
                try { loop.Abort(); }
                catch { };
                loop = null;
            }
            try { lis.Stop(); }
            catch { };
            try { lis.Close(); }
            catch { };
            try
            {
                lis = null;
                foreach (Thread son in processors.ToArray())
                    try { son.Abort(); }
                    catch { };
            }
            finally
            {
                //WindowsFirewall.Remove(fwRule);
            }
            return true;
        }

        internal void LogError(Exception e)
        {
            errors.Add(new LogError(e));
            Error?.Invoke(this, e);
        }

        public bool Up { get { return lis != null; } }
        public int Port { get { return port; } set { port = value; } }
    }

    public class HttpCall
    {
        private static char[] CRLF = { '\r', '\n' };
        private BufferedStream bs;
        private StreamWriter swout;
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        private Dictionary<string, HttpPostFile> files = new Dictionary<string, HttpPostFile>();

        internal HttpCall(HttpListenerContext ctx)
        {
            Request = ctx.Request;
            Response = ctx.Response;
            User = ctx.User;
            if ("POST" == Request.HttpMethod)
                ProcessPost(Request.InputStream, Request.ContentType, Request.ContentEncoding);
            GetHeaders();
        }

        private void GetHeaders()
        {
            string qs = Request.Url.Query;
            if (qs.StartsWith("?"))
                ReadEncodedParameters(qs.Substring(1));
        }

        private void ReadEncodedParameters(string qs)
        {
            string[] args = qs.Split('&');
            foreach (string arg in args)
            {
                int ppos = arg.IndexOf('=');
                if (ppos == -1)
                    parameters[arg] = "true";
                else
                {
                    string aname = arg.Substring(0, ppos);
                    string aval = arg.Substring(ppos + 1);
                    parameters[aname] = HttpUtility.UrlDecode(aval);
                }
            }
        }

        public void OpenOutput(string contentType = null, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = new UTF8Encoding(false);
            if (string.IsNullOrEmpty(contentType) || string.IsNullOrWhiteSpace(contentType))
                contentType = "text/html";
            bs = new BufferedStream(Response.OutputStream);
            swout = new StreamWriter(bs, encoding);
            Response.Headers[HttpResponseHeader.ContentType] = contentType;
            Response.Headers[HttpResponseHeader.ContentEncoding] = encoding.WebName;
        }

        #region POST
        private void ProcessPost(Stream input, string contentType, Encoding enc)
        {
            string[] cttp = contentType.Split(';');
            string ctype = cttp[0];
            Dictionary<string, string> cttParams = null;
            BufferedStream bfs = new BufferedStream(input, 1024 * 1024);
            if (cttp.Length > 1)
            {
                cttParams = new Dictionary<string, string>();
                for (int i = 1; i < cttp.Length; i++)
                {
                    string[] s = cttp[i].Trim().Split('=');
                    cttParams[s[0]] = s.Length > 1 ? s[1] : "X";
                }
            }
            if ("multipart/form-data".Equals(ctype, StringComparison.InvariantCultureIgnoreCase))
                ReadPostMultipart(bfs, cttParams, enc);
            else if ("application/x-www-form-urlencoded".Equals(ctype, StringComparison.InvariantCultureIgnoreCase))
                ReadPostForm(bfs, enc);
            bfs.Close();
            input.Close();
        }

        private void ReadPostForm(Stream input, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buf = new byte[102400];
            int readed = 0;
            while ((readed = input.Read(buf, 0, buf.Length)) > 0)
            {
                ms.Write(buf, 0, readed);
                if (ms.Length > 1024 * 1024)
                    return;
            }
            ms.Close();
            string args = encoding.GetString(ms.ToArray());
            ReadEncodedParameters(args);
        }

        private void ReadPostMultipart(Stream input, Dictionary<string, string> cttParams, Encoding encoding)
        {
            string mimeBoundary = "--" + cttParams["boundary"];
            BinaryReader br = new BinaryReader(input, encoding);
            string bnd = ReadLine(br);
            if (bnd == mimeBoundary + "--")
                return; //empty
            if (bnd != mimeBoundary)
                throw new Exception("form-data boundary error.");
            mimeBoundary = "\r\n" + mimeBoundary;
            char[] mimeBoundaryChr = mimeBoundary.ToCharArray();
            byte[] mimeBoundaryBytes = Encoding.ASCII.GetBytes(mimeBoundary);
            while (ReadFormData(br, mimeBoundaryChr, mimeBoundaryBytes)) ;
        }

        private bool ReadFormData(BinaryReader br, char[] mimeBoundaryChr, byte[] mimeBoundaryBytes)
        {
            Dictionary<string, string> hdrs = new Dictionary<string, string>();
            while (true)
            {
                string line = ReadLine(br);
                if (line.Length == 0)
                    break;
                int dp = line.IndexOf(':');
                if (dp < 0)
                    throw new Exception("multipart form data error");
                hdrs[line.Substring(0, dp)] = line.Substring(dp + 2); //jump ':' and space
            }
            Dictionary<string, string> cdisp = GetContentDisposition(hdrs);
            if (hdrs.ContainsKey("Content-Type") || cdisp.ContainsKey("filename"))
            {
                byte[] data = ReadUntil(br, mimeBoundaryBytes); //TODO: Slow
                string fname = cdisp["filename"];
                if (fname.Length > 0)
                    files[cdisp["name"]] = new HttpPostFile(fname, data, hdrs["Content-Type"]);
            }
            else
            {
                string data = ReadStringToBoundary(br, mimeBoundaryChr);
                parameters[cdisp["name"]] = data;
            }
            char ch1 = br.ReadChar();
            char ch2 = br.ReadChar();
            if (ch1 == '\r' && ch2 == '\n')
                return true;
            if (ch1 == '-' && ch2 == '-')
                return false;
            throw new Exception("Error reading multipart (" + ch1 + ch2 + ")");
        }

        private static string ReadLine(BinaryReader br)
        {
            return ReadUntil(br, CRLF);
        }

        private static string ReadStringToBoundary(BinaryReader br, char[] mimeBoundaryChr)
        {
            return ReadUntil(br, mimeBoundaryChr);
        }

        private static string ReadUntil(BinaryReader br, char[] signal)
        {
            List<char> chars = new List<char>();
            while (true)
            {
                char ch = br.ReadChar();
                chars.Add(ch);
                if (chars.Count >= signal.Length)
                {
                    bool eq = true;
                    for (int i = 0; i < signal.Length; i++)
                    {
                        if (signal[i] != chars[(chars.Count - signal.Length) + i])
                        {
                            eq = false;
                            break;
                        }
                    }
                    if (eq)
                    {
                        char[] chs = chars.ToArray();
                        return new string(chs, 0, chs.Length - signal.Length);
                    }
                }
            }
        }

        private static byte[] ReadUntil(BinaryReader br, byte[] signal)
        {
            MemoryStream ms = new MemoryStream(10240);
            int signalIdx = 0;
            long end = 0;
            byte[] onebyte = new byte[1];
            byte b;

            while (true)
            {
                b = onebyte[0] = br.ReadByte();
                ms.Write(onebyte, 0, 1);
                if (b == signal[signalIdx])
                {
                    signalIdx++;
                    if (signalIdx == signal.Length)
                    {
                        ms.SetLength(end);
                        return ms.ToArray();
                    }
                }
                else
                {
                    end += signalIdx + 1;
                    signalIdx = 0;
                }
            }
        }

        private static Dictionary<string, string> GetContentDisposition(Dictionary<string, string> hdrs)
        {
            if (!hdrs.ContainsKey("Content-Disposition"))
                return null;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string cd in hdrs["Content-Disposition"].Split(';'))
            {
                int pos = cd.IndexOf("=");
                if (pos < 0)
                    continue;
                string name = cd.Substring(1, pos - 1);
                string var = cd.Substring(pos + 1);
                if (var.Length > 0 && var[0] == '"')
                    var = var.Substring(1, var.Length - 2);
                result[name] = var;
            }
            return result;
        }
        #endregion

        #region Output
        public void NotFound()
        {
            Response.StatusCode = 404;
            Response.StatusDescription = "Not found";
        }

        public void Redirect(string to)
        {
            Response.StatusCode = 307; //Temporary Redirect
            Response.Headers["Location"] = to;
        }

        public void Write(string s)
        {
            if (s != null)
                GetOut().Write(s);
        }

        public void Write(object o)
        {
            if (o != null)
                GetOut().Write(o);
        }

        public void Write(byte[] data)
        {
            GetOut().Flush();
            GetOut().BaseStream.Write(data, 0, data.Length);
        }

        public void Write(byte[] data, int offset, int count)
        {
            GetOut().Flush();
            GetOut().BaseStream.Write(data, offset, count);
        }

        public void Close()
        {
            if (swout == null)
                GetOut();
            swout.Close();
        }

        #endregion

        private StreamWriter GetOut()
        {
            if (swout == null)
                OpenOutput();
            return swout;
        }

        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; }
        public IPrincipal User { get; }
        public StreamWriter Out => GetOut();
    }

    public class LogError
    {
        public LogError(Exception e)
        {
            Exception = e;
        }

        public DateTime Time { get; } = DateTime.Now;
        public Exception Exception { get; }
    }

    public class HttpPostFile
    {
        internal HttpPostFile(string filename, byte[] data, string mimeType)
        {
            Filename = filename;
            MimeType = mimeType;
            Data = data;
        }

        public string Filename { get; }
        public string MimeType { get; }
        public byte[] Data { get; }
    }

    public static class HttpTools
    {
        public static string nl2br(string s)
        {
            return s.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
        }

        public static string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }
    }
}
