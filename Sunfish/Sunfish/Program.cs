﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DolphinWebXplorer2.wx;

namespace DolphinWebXplorer2
{
    static class Program
    {
        public static string VERSION = "0.7c";
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
                Application.Run(new Form1());
            }
            finally
            {
                WebXplorer.Stop();
            }
        }
    }
}