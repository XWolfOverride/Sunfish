using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DolphinWebXplorer2.wx
{
    class RemoteScreen
    {
        private static ImageCodecInfo jpegEncoder = null;
        private static long lastTime;

        static RemoteScreen()
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpegEncoder = codec;
                    break;
                }
        }

        #region WINAPI
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* left button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* left button up */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* The wheel has been moved, if the mouse has a wheel. The amount of movement is specified in dwData */
        private const int KEYEVENTF_KEYDN = 0x0000;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public static void KeyDown(System.Windows.Forms.Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDN, 0);
        }

        public static void KeyUp(System.Windows.Forms.Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP /* 0x7F*/, 0);
        }
        #endregion

        public static byte[] GetScreen()
        {
            long diff = DateTime.Now.Ticks - lastTime;
            long q;
            if (diff > 25000000L)
                q = 15;
            else
            {
                q = 80 - (diff / 100000L);
                if (q < 15)
                    q = 15;
            }
            Screen scr = Program.MAINFORM.MyScreen;
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(scr.Bounds.Width, scr.Bounds.Height, PixelFormat.Format16bppRgb565))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(scr.Bounds.X, scr.Bounds.Y, 0, 0, scr.Bounds.Size);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, q);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp.Save(ms, jpegEncoder, myEncoderParameters);
                ms.Close();
            }
            lastTime = DateTime.Now.Ticks;
            return ms.ToArray();
        }

        public static void InputCommand(string cmd)
        {
            if (cmd.Length == 0)
                return;
            string cc = cmd.Substring(0, 2);
            string par = cmd.Substring(2);
            if (cc.StartsWith("K"))
            {
                byte keyb;
                byte.TryParse(par, out keyb);
                Keys key = (Keys)Enum.ToObject(typeof(Keys), keyb);
                switch (cc)
                {
                    case "KD":
                        KeyDown(key);
                        break;
                    case "KU":
                        KeyUp(key);
                        break;
                }
                return;
            }
            int x;
            int y;
            string[] pars = par.Split(';');
            int.TryParse(pars[0], out x);
            int.TryParse(pars[1], out y);
            Screen scr = Program.MAINFORM.MyScreen;
            System.Windows.Forms.Cursor.Position = new Point(x + scr.Bounds.X, y + scr.Bounds.Y);
            switch (cc)
            {
                case "LD":
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    break;
                case "LU":
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case "RD":
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    break;
                case "RU":
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    break;
                case "WH":
                    Thread.Sleep(100);
                    int wheel;
                    int.TryParse(pars[2], out wheel);
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, wheel, 0);
                    break;
            }
        }
    }
}
