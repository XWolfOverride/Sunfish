using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Threading;
using DolphinWebXplorer2;

namespace XWolf
{
    public class HttpServer
    {
        private int port;
        private HttpListener lis = null;
        private Thread loop = null;
        private bool multiThread = true;
        private List<Thread> processors = new List<Thread>();
        private List<LogError> errors = new List<LogError>();
        private WindowsFirewallRule fwRule;
        public delegate void ErrorEventHandler(HttpServer server, Exception e);
        public delegate HttpServerProcessor CreateProcessorHandler(HttpServer server);
        public event ErrorEventHandler Error;
        public event CreateProcessorHandler CreateProcessor;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public bool Start()
        {
            if (lis != null)
                return false;
            lis = new HttpListener();
            lis.Prefixes.Add("http://+:" + port + "/");
            try
            {
                lis.Start();
            }
            catch (HttpListenerException e)
            {
                lis = null;
                return false;
            }
            loop = new Thread(ServerLoop);
            loop.Start();
            fwRule = WindowsFirewall.Allow(port);
            return true;
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
            CallNewProcessor((HttpListenerContext)octx);
            lock (processors)
            {
                processors.Remove(Thread.CurrentThread);
            }
        }

        private void CallNewProcessor(HttpListenerContext ctx)
        {
            HttpServerProcessor proc = CreateProcessor == null ? new HttpServerProcessor() : CreateProcessor(this);
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
                WindowsFirewall.Remove(fwRule);
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

    public class HttpServerProcessor
    {
        protected HttpListenerRequest Request;
        protected HttpListenerResponse Response;
        protected System.Security.Principal.IPrincipal User;
        private StreamWriter swout;
        private string path;
        private Dictionary<String, String> getArgs = new Dictionary<string, string>();
        internal HttpServer server;

        internal void Process(HttpListenerContext ctx)
        {
            Exception error = null;
            try
            {
                Request = ctx.Request;
                Response = ctx.Response;
                User = ctx.User;
                System.Text.Encoding utf8EncoderNoBOM = new System.Text.UTF8Encoding(false);
                swout = new StreamWriter(Response.OutputStream, utf8EncoderNoBOM);
                Response.Headers[HttpResponseHeader.ContentType] = "text/html";
                Response.Headers[HttpResponseHeader.ContentEncoding] = "UTF-8";
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
            if (error!=null && server != null)
            {
                server.LogError(error);
            }
        }

        private void GetHeaders()
        {
            getArgs.Clear();
            path = Request.Url.LocalPath;
            string qs = Request.Url.Query;
            if (qs.StartsWith("?"))
            {
                string[] args = qs.Substring(1).Split('&');
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

        public string nl2br(string s)
        {
            return s.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
        }

        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        virtual protected void Process()
        {
            Error404();
        }

        protected Stream OutStream { get { return Response.OutputStream; } }
        protected StreamWriter Out { get { return swout; } }
        public string Path { get { return path; } }
        public Dictionary<String, String> GET { get { return getArgs; } }
    }

    public class LogError
    {
        private DateTime time = DateTime.Now;
        private Exception e;

        public LogError(Exception e)
        {
            this.e = e;
        }

        public DateTime Time { get { return time; } }
        public Exception Exception { get { return e; } }
    }
}
