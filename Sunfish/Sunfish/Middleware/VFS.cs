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

        public void AddVirtualFolder(string path, VFSFolder folder)
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

        private VFSFolder LocateFolder(ref string path)
        {
            VFSFolder candidate = null;
            string candidatePath = null;
            string candidateRelativePath = null;
            foreach (string k in vf.Keys)
            {
                if (candidate == null ||
                    (path.StartsWith(k) && k.Length > candidatePath.Length))
                {
                    candidate = vf[k];
                    candidatePath = k;
                    candidateRelativePath = path.Substring(k.Length);
                }
            }
            path = candidateRelativePath;
            return candidate;
        }

        public Stream OpenRead(string path)
        {
            VFSFolder folder = LocateFolder(ref path);
            if (folder == null)
                return null;
            return folder.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            VFSFolder folder = LocateFolder(ref path);
            if (folder == null)
                return null;
            return folder.OpenWrite(path);
        }

        public VFSItem GetItem(string path)
        {
            while (path != "/" && path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            if (path.Length == 0)
                path = "/";
            VFSFolder folder = LocateFolder(ref path);
            if (folder == null)
                return null;
            return folder.GetItem(path);
        }

        public VFSItem Create(string path)
        {
            bool asDir = path.EndsWith("/");
            if (asDir)
                path = path.Substring(0, path.Length - 1);
            int sep = path.LastIndexOf('/');
            string inner = path.Substring(sep + 1);
            path = path.Substring(0, sep);
            VFSItem dir = GetItem(path);
            while (dir == null)
            {
                sep = path.LastIndexOf('/');
                if (sep < 0)
                    return null;
                inner = path.Substring(sep + 1) + '/' + inner;
                path = path.Substring(0, sep);
                dir = GetItem(path);
            }
            if (!dir.Directory)
                return null;
            return dir.Create(inner, asDir);
        }

        public string[] ListFiles(string path)
        {
            VFSFolder folder = LocateFolder(ref path);
            if (folder == null)
                return null;
            return folder.ListFiles(path);
        }

        public string[] ListDirectories(string path)
        {
            VFSFolder folder = LocateFolder(ref path);
            if (folder == null)
                return null;
            return folder.ListDirectories(path);
        }
    }
}
