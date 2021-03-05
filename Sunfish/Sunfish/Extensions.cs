using System;
using System.Collections.Generic;
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

        public static void TransferFrom(this Stream s, Stream from)
        {
            byte[] buf = new byte[524288];// 512Kb
            int readed = buf.Length;
            while (readed == buf.Length)
            {
                readed = from.Read(buf, 0, buf.Length);
                s.Write(buf, 0, readed);
            }
        }

        public static void TransferFrom(this Stream s, Stream from, long length)
        {
            byte[] buf = new byte[Math.Min(524288, length)];// 512Kb
            while (length > 0)
            {
                int toRead = (int)Math.Min(buf.Length, length);
                int readed = from.Read(buf, 0, toRead);
                if (readed != toRead)
                    throw new IOException("Unexpected EOF");
                s.Write(buf, 0, readed);
                length -= readed;
            }
        }

        public static T GetValue<K, T>(this Dictionary<K, T> dict, K key, T def)
        {
            T value;
            if (dict.TryGetValue(key, out value))
                return value;
            return def;
        }
    }
}
