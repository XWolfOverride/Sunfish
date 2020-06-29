using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2.Middleware
{
    public class VFSFolderFileSystem:VFSFolder
    {
        public VFSFolderFileSystem(string path)
        {

        }

        public bool AllowSubfolder { get; set; } = true;
        public bool ReadOnly { get; set; } = true;
    }
}
