using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DolphinWebXplorer2
{
    class Sunfish
    {
        class SunfishConfiguration
        {
            public int Port = 90;
            public bool Active = false;
            public List<SunfishServiceConfiguration> Services = new List<SunfishServiceConfiguration>();
        }

        public const string DEFAULT_CONF_FILE = "sunfish2";
        private static SunfishConfiguration conf = new SunfishConfiguration();
        private static List<SunfishService> srvs = new List<SunfishService>();

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
            foreach(SunfishServiceConfiguration ssc in conf.Services)
            {
                srvs.Add(SunfishService.Instance(ssc));
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
            conf.Active = act;
            // INIT SERVER HERE
        }

        private static void SetPort(int port)
        {
            bool act = Active;
            Active = false;
            conf.Port = port;
            Active = act;
        }

        public static bool Active { get => conf.Active; set => SetActive(value); }
        public static int Port { get => conf.Port; set => SetPort(value); }
        public static SunfishService[] Services => srvs.ToArray();
    }
}
