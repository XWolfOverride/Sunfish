using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sunfish.Middleware
{
    public class WebUI
    {
        static WebUI()
        {
            DeletePresealed();
            InitResources();
        }

        #region Frontend Resources
#if DEBUG
        public static bool EXTERNAL_RESOURCES = true;
        private static string sfpath;
#endif

        private const string SECBEGIN = "<!--SEC:";
        private const string SECTAGEND = "-->";
        public static Dictionary<string, Templater> Templs = new Dictionary<string, Templater>();
        private static bool ready;
        private static Dictionary<string, byte[]> Res = new Dictionary<string, byte[]>();

        private static void InitResources()
        {
            ready = false;
            Templs.Clear();
            Res.Clear();
            using (FileStream fs = new FileStream(Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
            {
                LocatePak(fs);
                if (fs.Position < fs.Length)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.TransferFrom(fs, fs.Length - fs.Position - 5);
                    ms.Position = 0;
                    using (ZipArchive z = new ZipArchive(ms, ZipArchiveMode.Read, true))
                    {
                        foreach (ZipArchiveEntry e in z.Entries)
                        {
                            byte[] ctt = new byte[e.Length];
                            using (Stream zf = e.Open())
                                zf.Read(ctt, 0, ctt.Length);
                            if (e.Name[0] == '$')
                                ProcessTemplate(Encoding.UTF8.GetString(ctt));
                            else
                                Res[e.Name] = ctt;
                        }
                    }
#if DEBUG
                    EXTERNAL_RESOURCES = false;
#endif
                    ready = true;
                }
            }
#if DEBUG
            if (!ready)
                if (EXTERNAL_RESOURCES)
                {
                    sfpath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "$sunfish");
                    EXTERNAL_RESOURCES = Directory.Exists(sfpath);
                    if (EXTERNAL_RESOURCES)
                    {
                        ProcessTemplate(File.ReadAllText(Path.Combine(sfpath, "$templates.html")));
                        ready = true;
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

        private static bool LocatePak(Stream s)
        {
            byte[] buf = new byte[5];
            s.Position = s.Length - buf.Length;
            if (s.Read(buf, 0, buf.Length) != buf.Length)
                throw new Exception("Probleams reading mark");
            if (Encoding.ASCII.GetString(buf) == "SFRes")
            {
                buf = new byte[4];
                s.Position = s.Length - (5 + buf.Length);
                if (s.Read(buf, 0, buf.Length) != buf.Length)
                    throw new Exception("Probleams reading mark");
                s.Position = BitConverter.ToInt32(buf, 0);
                return true;
            }
            s.Position = s.Length;
            return false;
        }

        public static void WriteResource(string path, HttpCall call)
        {
#if DEBUG
            if (EXTERNAL_RESOURCES)
            {
                call.Write(File.ReadAllBytes(Path.Combine(sfpath, path)));
                return;
            }
#endif
            byte[] data;
            if (Res.TryGetValue(path, out data))
                call.Write(data, 0, data.Length);
            else
                throw new Exception("Resource not found");
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
#if DEBUG
            else
                throw new Exception("No template found for: " + template);
#endif
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

        public static void Seal()
        {
            string mepath = Assembly.GetExecutingAssembly().Location;
            string medir = Path.GetDirectoryName(mepath);
            string oldpath = Path.Combine(medir, "Sunfish-preseal.exe");
            string spath = Path.Combine(medir, "$sunfish");
            if (!Directory.Exists(spath))
                throw new Exception("$sunfish directory with resources not found.");

            File.Move(mepath, oldpath);

            // Create resources ZIP
            MemoryStream zms = new MemoryStream();
            using (ZipArchive z = new ZipArchive(zms, ZipArchiveMode.Create))
            {
                foreach (string fname in Directory.GetFiles(spath))
                {
                    ZipArchiveEntry zae = z.CreateEntry(fname.Substring(spath.Length + 1));
                    using (Stream zs = zae.Open())
                    {
                        byte[] filall = File.ReadAllBytes(fname);
                        zs.Write(filall, 0, filall.Length);
                    }
                }
            }

            // Install package
            using (FileStream ofs = new FileStream(oldpath, FileMode.Open, FileAccess.Read))
            {
                LocatePak(ofs);
                if (ofs.Position != ofs.Length)
                    ofs.SetLength(ofs.Position);
                ofs.Position = 0;

                using (FileStream nfs = new FileStream(mepath, FileMode.Create, FileAccess.Write))
                {
                    nfs.TransferFrom(ofs, ofs.Length);
                    byte[] zz = zms.ToArray();
                    nfs.Write(zz, 0, zz.Length);
                    zz = BitConverter.GetBytes((int)ofs.Length);
                    nfs.Write(zz, 0, zz.Length);
                    zz = Encoding.ASCII.GetBytes("SFRes");
                    nfs.Write(zz, 0, zz.Length);
                }
            }
        }

        public static void DeletePresealed()
        {
            string pspath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sunfish-preseal.exe");
            if (File.Exists(pspath))
                File.Delete(pspath);
        }

        public static bool Ready => ready;
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
