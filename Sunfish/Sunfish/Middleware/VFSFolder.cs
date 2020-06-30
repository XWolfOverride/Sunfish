﻿using System;
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
        public abstract string[] ListFiles(string path);
        public abstract string[] ListDirectories(string path);
    }
}
