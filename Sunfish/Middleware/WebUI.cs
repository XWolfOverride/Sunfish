using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static string rpath = @"C:\Users\XWolf\Source\Repos\Sunfish\Sunfish\ShareWeb\$sunfish";
        private const string SECBEGIN = "<!--SEC:";
        private const string SECTAGEND = "-->";
        public static Dictionary<string, Templater> Templs = new Dictionary<string, Templater>();

        public static void InitResources() //TODO: Now calling from several sites, in future make private and call in static ctor
        {
            Templs.Clear();
            string template = File.ReadAllText(Path.Combine(rpath, "$index.html"));
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
            call.Write(File.ReadAllBytes(Path.Combine(rpath, path)));
        }

        public static void WriteTemplate(string template, HttpCall call, params object[] para)
        {
            call.Write(Templs[template].Process(para));
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
