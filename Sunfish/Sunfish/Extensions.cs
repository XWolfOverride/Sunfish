using System;
using System.IO;
using System.Windows.Forms;

namespace DolphinWebXplorer2
{
    public static class Extensions
    {
        public static void Show(this Exception ex)
        {
            MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void TransferFrom(this Stream s,Stream from)
        {
            byte[] buf = new byte[10240];// 10Kb
            int readed=buf.Length;
            while (readed == buf.Length)
            {
                readed = from.Read(buf, 0, buf.Length);
                s.Write(buf, 0, readed);
            }
        }
    }
}
