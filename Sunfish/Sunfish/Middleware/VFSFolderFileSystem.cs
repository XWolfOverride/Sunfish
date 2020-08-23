using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class VFSFolderFileSystem : VFSFolder
    {
        private string basePath;

        public VFSFolderFileSystem(string path)
        {
            basePath = path;
        }

        public bool AllowSubfolder { get; set; } = true;
        public bool ReadOnly { get; set; } = true;

        public override Stream OpenRead(string path)
        {
            path = Path.Combine(basePath, path);
            try
            {
                return File.OpenRead(path);
            }
            catch { };
            return null;
        }

        public override Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        public override VFSItem GetItem(string path)
        {
            if (Path.DirectorySeparatorChar != '/')
                path = path.Replace('/', Path.DirectorySeparatorChar);
            string fpath = Path.Combine(basePath, path);
            FileInfo fi = new FileInfo(fpath);
            DirectoryInfo di = new DirectoryInfo(fpath);
            if (!fi.Exists && !di.Exists)
                return null;
            return new VFSItem(this, path, di.Exists, fi.Exists ? fi.Length : 0);
        }

        public override string[] ListDirectories(string path)
        {
            string fpath = Path.Combine(basePath, path);
            List<string> lst = new List<string>();
            foreach (string d in Directory.GetDirectories(fpath))
                lst.Add(d.Substring(fpath.Length + 1));
            lst.Sort();
            return lst.ToArray();
        }

        public override string[] ListFiles(string path)
        {
            string fpath = Path.Combine(basePath, path);
            List<string> lst = new List<string>();
            foreach (string d in Directory.GetFiles(fpath))
                lst.Add(d.Substring(fpath.Length + 1));
            lst.Sort();
            return lst.ToArray();
        }

        public string GetFSPath(string path)
        {
            return Path.Combine(basePath, path);
        }
    }
}
