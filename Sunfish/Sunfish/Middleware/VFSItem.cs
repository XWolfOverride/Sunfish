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

        public VFSItem(VFSFolder vfolder, string path, bool isFolder, long length)
        {
            Folder = vfolder;
            Path = path;
            Directory = isFolder;
            Name = System.IO.Path.GetFileName(Path);
            Length = length;
        }

        public Stream OpenRead()
        {
            return Folder.OpenRead(Path);
        }

        public Stream OpenWrite()
        {
            return Folder.OpenWrite(Path);
        }

        public string[] ListDirectories()
        {
            return Folder.ListDirectories(Path);
        }

        public string[] ListFiles()
        {
            return Folder.ListFiles(Path);
        }

        public string Path { get; }
        public string Name { get; }
        public bool Directory { get; }
        public long Length { get; }

        public VFSFolder Folder { get; }
    }
}
