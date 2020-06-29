using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class VFS
    {
        private Dictionary<string, VFSFolder> vf = new Dictionary<string, VFSFolder>();

        public void AddVirtualFolder(string path,VFSFolder folder)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = "/";
            else
            {
                path = path.Trim().ToLower();
                if (path[0] != '/')
                    path = '/' + path;
            }
            vf[path] = folder;
        }

        public Stream OpenRead(string path)
        {
            return null;
        }

        public Stream OpenWrite(string path)
        {
            return null;
        }

        public string[] ListFiles(string path)
        {
            return null;
        }

        public string[] ListDirectories(string path)
        {
            return null;
        }
    }
}
