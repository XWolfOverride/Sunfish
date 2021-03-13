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
            Name = System.IO.Path.GetFileName(path);
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

        public VFSItem[] ListDirectories()
        {
            return Folder.ListDirectories(Path);
        }

        public VFSItem[] ListFiles()
        {
            return Folder.ListFiles(Path);
        }

        public bool Delete()
        {
            if (Directory)
                return Folder.DeleteFolder(Path);
            else
                return Folder.DeleteFile(Path);
        }

        public bool RenameTo(string newName)
        {
            return Folder.Rename(Path, newName);
        }

        public VFSItem Create(string name, bool asFolder)
        {
            return Folder.Create(System.IO.Path.Combine(Path, name), asFolder);
        }

        public string Path { get; }
        public string Name { get; }
        public bool Directory { get; }
        public long Length { get; }
        public VFSFolder Folder { get; }
    }
}
