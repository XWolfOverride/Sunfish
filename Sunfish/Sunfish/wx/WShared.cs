﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DolphinWebXplorer2.wx
{
    public class WShared
    {
        private string name;
        private string path;
        private bool enabled;
        private bool allowSubfolders;
        private bool allowUpload;
        private bool allowDeletion;
        private bool allowRename;
        private bool allowExecution;
        private bool allowNewFolder;

        public WShared()
        {
        }

        public WShared(string name, string path)
        {
            this.name = name;
            Path = path;
        }

        public string GetLocalPath(string path)
        {
            return this.path + path.Replace('/',System.IO.Path.DirectorySeparatorChar);
        }

        public string GetRemotePath(string path)
        {
            return '/' + name +'/'+ path;
        }

        public string Name { get { return name; } set { name = value; } }
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                if (!path.EndsWith("\\"))
                    path += '\\';
            }
        }
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        public bool AllowSubfolders { get { return allowSubfolders; } set { allowSubfolders = value; } }
        public bool AllowUpload { get { return allowUpload; } set { allowUpload = value; } }
        public bool AllowDeletion { get { return allowDeletion; } set { allowDeletion = value; } }
        public bool AllowRename { get { return allowRename; } set { allowRename = value; } }
        public bool AllowExecution { get { return allowExecution; } set { allowExecution = value; } }
        public bool AllowNewFolder { get { return allowNewFolder; } set { allowNewFolder = value; } }
    }
}
