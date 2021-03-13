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
            while (basePath[basePath.Length - 1] == (Path.DirectorySeparatorChar))
            {
                if (basePath.Length < 2)
                    throw new Exception("Invalid path");
                basePath = basePath.Substring(0, basePath.Length - 1);
            }
            if (basePath.Length == 2)
                basePath += Path.DirectorySeparatorChar;
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
            path = Path.Combine(basePath, path);
            try
            {
                return File.OpenWrite(path);
            }
            catch { };
            return null;
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

        public override VFSItem[] ListDirectories(string path)
        {
            string fpath = Path.Combine(basePath, path);
            int basecut = basePath.Length;
            if (basePath[basePath.Length - 1] != Path.DirectorySeparatorChar)
                basecut++;
            List<VFSItem> lst = new List<VFSItem>();
            foreach (string d in Directory.GetDirectories(fpath))
                lst.Add(new VFSItem(this, d.Substring(basecut), true, 0));
            lst.Sort((a, b) => a.Name.CompareTo(b.Name));
            return lst.ToArray();
        }

        public override VFSItem[] ListFiles(string path)
        {
            string fpath = Path.Combine(basePath, path);
            int basecut = basePath.Length;
            if (basePath[basePath.Length - 1] != Path.DirectorySeparatorChar)
                basecut++;
            List<VFSItem> lst = new List<VFSItem>();
            foreach (string d in Directory.GetFiles(fpath))
            {
                FileInfo fi = new FileInfo(d);
                lst.Add(new VFSItem(this, d.Substring(basecut), false, fi.Length));
            }
            lst.Sort((a, b) => a.Name.CompareTo(b.Name));
            return lst.ToArray();
        }

        public override bool DeleteFile(string path)
        {
            string fpath = Path.Combine(basePath, path);
            try
            {
                File.Delete(fpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool DeleteFolder(string path)
        {
            string fpath = Path.Combine(basePath, path);
            try
            {
                FSDeleteFolder(fpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Rename(string from, string to)
        {
            string ffrom = Path.Combine(basePath, from);
            string realbase = Path.GetDirectoryName(ffrom);
            string tto = Path.Combine(realbase, to);
            try
            {
                Directory.Move(ffrom, tto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override VFSItem Create(string path, bool asFolder)
        {
            string fspath = Path.Combine(basePath, path);
            if (asFolder)
            {
                Directory.CreateDirectory(fspath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fspath));
                File.WriteAllBytes(fspath, new byte[0]);
            }
            return GetItem(path);
        }

        private void FSDeleteFolder(string path)
        {
            foreach (string d in Directory.GetDirectories(path))
                FSDeleteFolder(d);
            foreach (string d in Directory.GetFiles(path))
                File.Delete(d);
            Directory.Delete(path);
        }

        public string GetFSPath(string path)
        {
            return Path.Combine(basePath, path);
        }

    }
}
