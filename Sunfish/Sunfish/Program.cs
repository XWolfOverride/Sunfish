using DolphinWebXplorer2.wx;
using System;
using System.Windows.Forms;

namespace DolphinWebXplorer2
{
    static class Program
    {
        public static string VERSION = "1.0a";
        private static Form1 mainform;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(mainform = new Form1());
            }
            finally
            {
                WebXplorer.Stop();
            }
        }

        public static Form1 MAINFORM { get { return mainform; } }
    }
}
