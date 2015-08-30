using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Clearbytes
{
    static class Program
    {
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
