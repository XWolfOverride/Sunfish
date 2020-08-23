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
           // InitResources();
        }

        #region Frontend Resources

        private static string rpath = @"C:\Users\XWolf\Source\Repos\Sunfish\Sunfish\ShareWeb\$sunfish";
        private const string SECBEGIN = "<!--SEC:";
        private const string SECTAGEND = "-->";
        private static Dictionary<string, object> BaseData = new Dictionary<string, object>()
        {
            ["AppName"] = "Sunfish",
            ["AppVersion"] = Program.VERSION,
        };

        public static void InitResources()
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

        private static Dictionary<string, Templater> Templs = new Dictionary<string, Templater>();

        #endregion

        public static void WriteHeader(HttpCall call)
        {
            call.Write(Templs["head-a"].Process(BaseData));
            //call.Write(Templs["head-location-item"].Process(BaseData));
            call.Write(Templs["head-b"].Process(BaseData));
            //call.Write(Templs["head-toolbar-item"].Process(BaseData));
            call.Write(Templs["head-c"].Process(BaseData));
        }

        public static void WriteFooter(HttpCall call)
        {
            call.Write(Templs["footer"].Process(BaseData));
        }

        public static void WriteItem(WebUIListItem item, HttpCall call)
        {
            call.Write(Templs["item"].Process(item, BaseData));
        }

        public static void WriteResource(string path, HttpCall call)
        {
            call.Write(File.ReadAllBytes(Path.Combine(rpath, path)));
        }


    }

    public struct WebUIListItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public string Styles { get; set; }
    }
}
