using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public abstract class VFSFolder
    {
        public abstract Stream OpenRead(string path);
        public abstract Stream OpenWrite(string path);
        public abstract VFSItem GetItem(string path);
        public abstract VFSItem[] ListFiles(string path);
        public abstract VFSItem[] ListDirectories(string path);
        public abstract bool DeleteFile(string path);
        public abstract bool DeleteFolder(string path);
        public abstract bool Rename(string from, string to);        
        public abstract VFSItem Create(string path,bool asFolder);
    }
}
