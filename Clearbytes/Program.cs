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

        public static string FormatUnixTimestamp(long timestamp, string format)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt.AddSeconds(timestamp).ToLocalTime();

            return dt.ToString(format);
        }
    }
}
