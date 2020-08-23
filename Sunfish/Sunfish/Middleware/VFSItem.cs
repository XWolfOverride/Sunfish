using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class VFSItem
    {
        private VFSFolder vfolder;
        
        public VFSItem(VFSFolder vfolder, string path, bool isFolder)
        {
            this.vfolder = vfolder;
            this.Path = path;
            this.Folder = isFolder;
            Name = System.IO.Path.GetFileName(Path);
        }

        public Stream OpenRead()
        {
            return vfolder.OpenRead(Path);
        }

        public Stream OpenWrite()
        {
            return vfolder.OpenWrite(Path);
        }

        public string[] ListDirectories()
        {
            return vfolder.ListDirectories(Path);
        }

        public string[] ListFiles()
        {
            return vfolder.ListFiles(Path);
        }

        public string Path { get; }
        public string Name { get; }
        public bool Folder { get; }
    }
}
