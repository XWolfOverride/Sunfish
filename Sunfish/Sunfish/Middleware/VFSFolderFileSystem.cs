using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class VFSFolderFileSystem:VFSFolder
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
            }catch { };
            return null;
        }

        public override Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        public override VFSItem GetItem(string path)
        {
            string fpath = Path.Combine(basePath, path);
            FileInfo fi = new FileInfo(fpath);
            DirectoryInfo di = new DirectoryInfo(fpath);
            if (!fi.Exists && !di.Exists)
                return null;
            return new VFSItem(this,fpath,di.Exists);
        }

        public override string[] ListDirectories(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ListFiles(string path)
        {
            throw new NotImplementedException();
        }

    }
}
