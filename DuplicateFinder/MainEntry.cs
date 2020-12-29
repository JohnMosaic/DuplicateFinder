using System;
using System.Windows.Forms;

namespace DuplicateFinder
{
    static class MainEntry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WinUI.DupUI());
        }
    }
}
