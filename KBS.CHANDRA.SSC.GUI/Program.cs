using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KBS.CHANDRA.SSC.GUI;
using KBS.CHANDRA.SSC.FUNCTION;

namespace KBS.CHANDRA.SSC.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestMenu());
        }
    }
}
