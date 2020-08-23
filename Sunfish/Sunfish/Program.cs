using DolphinWebXplorer2.Middleware;
using DolphinWebXplorer2.wx;
using System;
using System.Windows.Forms;

namespace DolphinWebXplorer2
{
    static class Program
    {
        public static string VERSION = "2.0(alpha5)";
        private static Form1 mainform;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Win32.SetProcessDPIAware();
            try
            {
                Application.Run(mainform = new Form1());
            }
            finally
            {
                Sunfish.Active = false;
            }
        }

        public static Form1 MAINFORM { get { return mainform; } }
    }
}
