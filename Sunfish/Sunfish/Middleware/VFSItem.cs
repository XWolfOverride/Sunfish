﻿using System;
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
        private string name;

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

        public string Path { get; }
        public string Name { get; }
        public bool Folder { get; }
    }
}
