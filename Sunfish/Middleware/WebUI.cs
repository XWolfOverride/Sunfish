using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    class WebUI
    {
        static WebUI()
        {
            InitResources();
        }

        #region Frontend Resources
#if DEBUG
        private static bool EXTERNAL_RESOURCES = true;
        private static string sfpath;
#endif

        private const string SECBEGIN = "<!--SEC:";
        private const string SECTAGEND = "-->";
        public static Dictionary<string, Templater> Templs = new Dictionary<string, Templater>();

        private static void InitResources()
        {
#if DEBUG
            if (EXTERNAL_RESOURCES)
            {
                Templs.Clear();
                sfpath=Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "$sunfish");
                EXTERNAL_RESOURCES = Directory.Exists(sfpath);
                if (EXTERNAL_RESOURCES)
                {
                    ProcessTemplate(File.ReadAllText(Path.Combine(sfpath, "$index.html")));
                    return;
                }
            }
#endif

        }

        private static void ProcessTemplate(string template)
        {
            string tname = "";
            string tval;
            int i = template.IndexOf(SECBEGIN);
            while (i >= 0)
            {
                tval = template.Substring(0, i);
                if (!string.IsNullOrEmpty(tval))
                    Templs[tname] = new Templater(tval);
                template = template.Substring(i + SECBEGIN.Length);
                i = template.IndexOf(SECTAGEND);
                tname = template.Substring(0, i);
                template = template.Substring(i + SECTAGEND.Length);
                i = template.IndexOf(SECBEGIN);
            }
            if (!string.IsNullOrEmpty(template))
                Templs[tname] = new Templater(template);
        }

        #endregion

        public static void WriteResource(string path, HttpCall call)
        {
#if DEBUG
            if (EXTERNAL_RESOURCES)
            {
                call.Write(File.ReadAllBytes(Path.Combine(sfpath, path)));
                return;
            }
#endif

        }

        public static void WriteTemplate(string template, HttpCall call, params object[] para)
        {
#if DEBUG
            if (EXTERNAL_RESOURCES)
                InitResources();
#endif
            Templater t;
            if (Templs.TryGetValue(template, out t))
                call.Write(t.Process(para));
            else
                throw new Exception("No template found for: " + template);
        }

        public static string FBytes(double lng)
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
    }

    public class WebUILink
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public string Tooltip { get; set; }
        public string Style { get; set; }
        public string Click { get; set; }
        public WebUILink[] Actions { get; set; }
    }
}
