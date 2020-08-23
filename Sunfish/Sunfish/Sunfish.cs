using DolphinWebXplorer2.Middleware;
using DolphinWebXplorer2.Services;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DolphinWebXplorer2
{
    static class Sunfish
    {
        class SunfishConfiguration
        {
            public int Port = 90;
            public bool Active = false;
            public bool SunfishRoot = true;
            public List<SunfishServiceConfiguration> Services = new List<SunfishServiceConfiguration>();
        }

        public const string DEFAULT_CONF_FILE = "sunfish2";
        private static SunfishConfiguration conf = new SunfishConfiguration();
        private static List<SunfishService> srvs = new List<SunfishService>();
        private static RootService sroot = new RootService();
        private static HttpServer server;

        #region Configuration & Initialization
        static Sunfish()
        {
            Load();
        }

        static void Load()
        {
            if (!File.Exists(DEFAULT_CONF_FILE))
                return;

            string json = File.ReadAllText(DEFAULT_CONF_FILE);
            conf = JsonNet.Deserialize<SunfishConfiguration>(json);
            if (conf.Services == null)
                conf.Services = new List<SunfishServiceConfiguration>();
            sroot.ShowMenu = conf.SunfishRoot;
            foreach (SunfishServiceConfiguration ssc in conf.Services)
            {
                srvs.Add(SunfishService.Instance(ssc));
            }
            if (conf.Active)
            {
                //Bypass set active check
                conf.Active = false;
                SetActive(true);
            }
        }

        public static void Save()
        {
            string json = JsonNet.Serialize(conf);
            File.WriteAllText(DEFAULT_CONF_FILE, json);
        }
        #endregion

        private static void SetActive(bool act)
        {
            if (conf.Active == act)
                return;
            if (act)
            {
                server = new HttpServer(conf.Port, server_Process);
                server.Error += server_Error;
                try
                {
                    server.Start();
                }
                catch (Exception e)
                {
                    server = null;
                    throw e;
                }
            }
            else
            {
                if (server != null)
                {
                    server.Stop();
                    server = null;
                }
            }
            conf.Active = act;
        }

        static void server_Process(HttpServer server, HttpCall call)
        {
            var path = call.Request.Url.LocalPath;
            SunfishService s = GetServiceForPath(ref path);
            if (string.IsNullOrWhiteSpace(path))
                call.Redirect(call.Request.Url.LocalPath + "/");
            else
            {
                if (s == null || !s.Enabled)
                    ErrorService.Process404(call);
                else
                    s.Process(path, call);
            }
            call.Close();
        }

        static void server_Error(HttpServer server, Exception e)
        {
            //TODO: Log
            //if (!(e is ObjectDisposedException) && !(e is HttpListenerException))
            //    MessageBox.Show(e.Message + "\r\n" + e.StackTrace, e.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void SetPort(int port)
        {
            bool act = Active;
            Active = false;
            conf.Port = port;
            Active = act;
        }

        public static SunfishService AddService(SunfishServiceConfiguration ssc)
        {
            SunfishService s;
            srvs.Add(s = SunfishService.Instance(ssc));
            conf.Services.Add(ssc);
            return s;
        }

        public static SunfishService ReplaceService(SunfishService sold, SunfishServiceConfiguration ssc)
        {
            SunfishService s = SunfishService.Instance(ssc);
            int i = srvs.IndexOf(sold);
            if (i < 0)
                srvs.Add(s);
            else
                srvs[i] = s;
            i = conf.Services.IndexOf(ssc);
            if (i < 0)
                conf.Services.Add(ssc);
            return s;
        }

        public static void DeleteService(SunfishService srv)
        {
            conf.Services.Remove(srv.Configuration);
        }

        public static SunfishService GetServiceForPath(ref string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/" || path.StartsWith("/$sunfish"))
                return sroot;
            string candidateRelativePath = "";
            SunfishService candidate = null;
            foreach (SunfishService s in srvs)
            {
                string plb = s.Configuration.Location;
                if (!plb.EndsWith("/"))
                    plb = plb + "/";
                if ((path == s.Configuration.Location || path.StartsWith(plb)) &&
                    (candidate == null || (candidate.Configuration.Location.Length < s.Configuration.Location.Length)))
                {
                    candidate = s;
                    candidateRelativePath = path.Substring(s.Configuration.Location.Length);
                }
            }
            if (candidate != null)
                path = candidateRelativePath;
            return candidate;
        }

        public static bool Active { get => conf.Active; set => SetActive(value); }
        public static int Port { get => conf.Port; set => SetPort(value); }
        public static bool RootMenu { get => conf.SunfishRoot; set => conf.SunfishRoot = value; }
        public static SunfishService[] Services => srvs.ToArray();
    }
}
