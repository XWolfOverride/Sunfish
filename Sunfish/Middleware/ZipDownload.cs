using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunfish.Middleware
{
    public class ZipDownload : IDisposable
    {
        private HttpCall call;
        private ZipArchive z;
        private FileStream temp;
        private string temppath;
        public ZipDownload(HttpCall call)
        {
            this.call = call;
            temp = new FileStream(temppath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip", FileMode.Create, FileAccess.ReadWrite);
            z = new ZipArchive(temp, ZipArchiveMode.Create, true, Encoding.UTF8);
        }

        public void AddFile(VFSItem fil, string name)
        {
            ZipArchiveEntry zfil = z.CreateEntry(name);
            using (Stream zs = zfil.Open())
            using (Stream src = fil.OpenRead())
            {
                zs.TransferFrom(src);
            }
        }

        public void AddDirectory(VFSItem fil, string name)
        {
            foreach (VFSItem i in fil.ListFiles())
                AddFile(i, name == null ? i.Name : name + '/' + i.Name);
            foreach (VFSItem i in fil.ListDirectories())
                AddDirectory(i, name == null ? i.Name : name + '/' + i.Name);
        }

        public void Dispose()
        {
            z.Dispose();
            temp.Position = 0;
            call.Response.ContentType = "application/zip";
            call.Response.OutputStream.TransferFrom(temp);
            temp.Close();
            File.Delete(temppath);
        }
    }
}
