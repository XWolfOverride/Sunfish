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
        private bool multiThread = true;
        private List<Thread> processors = new List<Thread>();
        private List<LogError> errors = new List<LogError>();
        //private WindowsFirewallRule fwRule;
        public delegate void ErrorEventHandler(HttpServer server, Exception e);
        public delegate HttpServerProcessor CreateProcessorHandler(HttpServer server);
        public event ErrorEventHandler Error;
        public event CreateProcessorHandler CreateProcessor;

        public HttpServer(int port)
        {
            this.port = port;
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
                if (multiThread)
                {
                    new Thread(new ParameterizedThreadStart(MultithreadCall)).Start(ctx);
                }
                else
                    CallNewProcessor(ctx);
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
            HttpServerProcessor proc = CreateProcessor(this);
            proc.server = this;
            proc.Process(ctx);
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
            if (Error != null)
                Error(this, e);
        }

        public bool Up { get { return lis != null; } }
        public int Port { get { return port; } set { port = value; } }
        public bool MultiThread { get { return multiThread; } set { multiThread = value; } }
    }

    abstract class HttpServerProcessor
    {
        protected HttpListenerRequest Request;
        protected HttpListenerResponse Response;
        protected System.Security.Principal.IPrincipal User;
        protected HttpPost Post;
        private StreamWriter swout;
        private Dictionary<string, string> getArgs = new Dictionary<string, string>();
        internal HttpServer server;

        internal void Process(HttpListenerContext ctx)
        {
            Exception error = null;
            try
            {
                Request = ctx.Request;
                Response = ctx.Response;
                User = ctx.User;
                Encoding utf8EncoderNoBOM = new UTF8Encoding(false);
                swout = new StreamWriter(Response.OutputStream, utf8EncoderNoBOM);
                Response.Headers[HttpResponseHeader.ContentType] = "text/html";
                Response.Headers[HttpResponseHeader.ContentEncoding] = "UTF-8";
                getArgs.Clear();
                if ("POST" == Request.HttpMethod)
                {
                    Post = new HttpPost(this, Request.InputStream, Request.ContentType, Request.ContentEncoding);
                }
                GetHeaders();
                Process();
                Out.Close();
            }
            catch (Exception e)
            {
                error = e;
                try
                {
                    Out.Close();
                }
                catch { };
            }
            try
            {
                ctx.Response.Close();
            }
            catch { };
            if (error != null && server != null)
            {
                server.LogError(error);
            }
        }

        private void GetHeaders()
        {
            string qs = Request.Url.Query;
            if (qs.StartsWith("?"))
                ReadEncodedArguments(qs.Substring(1));
        }

        internal void ReadEncodedArguments(string qs)
        {
            string[] args = qs.Split('&');
            foreach (string arg in args)
            {
                int ppos = arg.IndexOf('=');
                if (ppos == -1)
                    getArgs[arg] = "true";
                else
                {
                    string aname = arg.Substring(0, ppos);
                    string aval = arg.Substring(ppos + 1);
                    getArgs[aname] = HttpUtility.UrlDecode(aval);
                }
            }
        }

        internal void SetArgument(string name, string value)
        {
            getArgs[name] = value;
        }

        public HttpListenerResponse GetResponse() { return Response; }

        protected void Error404()
        {
            Response.StatusCode = 404;
            Response.StatusDescription = "Not found";
        }

        public void Write(String s)
        {
            if (s != null)
                swout.Write(s);
        }

        public void Write(object o)
        {
            if (o != null)
                swout.Write(o);
        }

        public void Write(byte[] data)
        {
            swout.Flush();
            OutStream.Write(data, 0, data.Length);
        }

        public void Write(byte[] data, int offset, int count)
        {
            swout.Flush();
            OutStream.Write(data, offset, count);
        }

        //public string nl2br(string s)
        //{
        //    return s.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
        //}

        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        protected abstract void Process();

        protected Stream OutStream { get { return Response.OutputStream; } }
        protected StreamWriter Out { get { return swout; } }
        public Dictionary<String, String> GET { get { return getArgs; } }
        public Dictionary<String, String> POST { get { return getArgs; } }
    }

    public class HttpCall
    {
        private StreamWriter swout;
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        internal HttpCall(HttpListenerContext ctx)
        {
            Request = ctx.Request;
            Response = ctx.Response;
            User = ctx.User;
            //if ("POST" == Request.HttpMethod)
            //Post = new HttpPost(this, Request.InputStream, Request.ContentType, Request.ContentEncoding);
            //Parameters = parameters;
        }

        private void GetHeaders()
        {
            string qs = Request.Url.Query;
            if (qs.StartsWith("?"))
                ReadEncodedParameters(qs.Substring(1), parameters);
        }

        static void ReadEncodedParameters(string qs, Dictionary<string, string> parameters)
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

        public void OpenOutput()
        {
            Encoding utf8EncoderNoBOM = new UTF8Encoding(false);
            swout = new StreamWriter(Response.OutputStream, utf8EncoderNoBOM);
            Response.Headers[HttpResponseHeader.ContentType] = "text/html";
            Response.Headers[HttpResponseHeader.ContentEncoding] = "UTF-8";
        }

        private StreamWriter GetOut()
        {
            //if (swout == null)
            //{
            //    swout=
            //}
            return null;
        }

        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; }
        public IPrincipal User { get; }
        public HttpPost Post { get; }

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

    public class HttpPost
    {
        private string contentType;
        private Dictionary<string, string> parms;
        private Dictionary<string, HttpPostFile> files = new Dictionary<string, HttpPostFile>();
        private HttpServerProcessor owner;
        private Encoding encoding;
        private static char[] CRLF = { '\r', '\n' };
        private string mimeBoundary;
        private char[] mimeBoundaryChr;
        private byte[] mimeBoundaryBytes;

        internal HttpPost(HttpServerProcessor owner, Stream input, string contentType, Encoding enc)
        {
            this.owner = owner;
            encoding = enc;
            string[] cttp = contentType.Split(';');
            this.contentType = cttp[0];
            if (cttp.Length > 1)
            {
                parms = new Dictionary<string, string>();
                for (int i = 1; i < cttp.Length; i++)
                {
                    string[] s = cttp[i].Trim().Split('=');
                    parms[s[0]] = s.Length > 1 ? s[1] : "X";
                }
            }
            if ("multipart/form-data".Equals(cttp[0], StringComparison.InvariantCultureIgnoreCase))
                ReadMultipart(input);
            else
                if ("application/x-www-form-urlencoded".Equals(cttp[0], StringComparison.InvariantCultureIgnoreCase))
                ReadPost(input);
        }

        private void ReadPost(Stream input)
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
            owner.ReadEncodedArguments(args);
        }

        private void ReadMultipart(Stream input)
        {
            mimeBoundary = "--" + parms["boundary"];
            BinaryReader br = new BinaryReader(input, encoding);
            string bnd = ReadLine(br);
            if (bnd == mimeBoundary + "--")
                return; //empty
            if (bnd != mimeBoundary)
                throw new Exception("form-data boundary error.");
            mimeBoundary = "\r\n" + mimeBoundary;
            mimeBoundaryChr = mimeBoundary.ToCharArray();
            mimeBoundaryBytes = Encoding.ASCII.GetBytes(mimeBoundary);
            while (ReadFormData(br)) ;
        }

        private string ReadLine(BinaryReader br)
        {
            return ReadUntil(br, CRLF);
        }

        private string ReadStringToBoundary(BinaryReader br)
        {
            return ReadUntil(br, mimeBoundaryChr);
        }

        private string ReadUntil(BinaryReader br, char[] signal)
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

        private byte[] ReadUntil(BinaryReader br, byte[] signal)
        {
            List<byte> bytes = new List<byte>();
            while (true)
            {
                byte by = br.ReadByte();
                bytes.Add(by);
                if (bytes.Count >= signal.Length)
                {
                    bool eq = true;
                    for (int i = 0; i < signal.Length; i++)
                    {
                        if (signal[i] != bytes[(bytes.Count - signal.Length) + i])
                        {
                            eq = false;
                            break;
                        }
                    }
                    if (eq)
                    {
                        return bytes.ToArray();
                    }
                }
            }
        }

        private bool ReadFormData(BinaryReader br)
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
                hdrs[line.Substring(0, dp)] = line.Substring(dp + 1);
            }
            Dictionary<string, string> cdisp = GetContentDisposition(hdrs);
            if (hdrs.ContainsKey("Content-Type") || cdisp.ContainsKey("filename"))
            {
                byte[] data = ReadUntil(br, mimeBoundaryBytes);
                string fname = cdisp["filename"];
                if (fname.Length > 0)
                    files[cdisp["name"]] = new HttpPostFile(fname, data);
            }
            else
            {
                string data = ReadStringToBoundary(br);
                owner.SetArgument(cdisp["name"], data);
            }
            char ch1 = br.ReadChar();
            char ch2 = br.ReadChar();
            if (ch1 == '\r' && ch2 == '\n')
                return true;
            if (ch1 == '-' && ch2 == '-')
                return false;
            throw new Exception("Error reading multipart (" + ch1 + ch2 + ")");
        }

        private Dictionary<string, string> GetContentDisposition(Dictionary<string, string> hdrs)
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

        public Dictionary<string, HttpPostFile> File { get { return files; } }
    }

    public class HttpPostFile
    {
        private string filename;
        private byte[] data;

        internal HttpPostFile(string filename, byte[] data)
        {
            this.filename = filename;
            this.data = data;
        }

        public string Filename { get { return filename; } }
        public byte[] Data { get { return data; } }
    }
}
