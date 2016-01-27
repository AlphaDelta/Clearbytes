using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Clearbytes
{
    static class Program
    {
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string Temp = Path.Combine(Path.GetTempPath(), @"Clearbytes\");
        public static string Archives = Path.Combine(Temp, @"archives\");

        const int TEMP_DIR_TIMEOUT = 60 * 60 * 24 * 2; //2 days

        public const string
        SUSPICIOUS_TOR = "WHERE url LIKE '%torproject.org/download%' OR url LIKE '%dist.torproject.org%' OR url LIKE '%hiddenwiki%' OR url LIKE '%hidden+wiki%' OR url LIKE '%hidden%20wiki%'",
        SUSPICIOUS_I2P = "WHERE url LIKE '%geti2p.net/%' OR url LIKE '%.i2p/%'",
        SUSPICIOUS_DARKNET = "WHERE url LIKE '%reddit.com/r/darknetmarkets' OR url LIKE '%reddit.com/r/darknetmarkets/' OR url LIKE '%reddit.com/r/darknetmarkets/new%' OR url LIKE '%reddit.com/r/darknetmarkets/top%' " +
                        "OR url LIKE '%reddit.com/r/drugs' OR url LIKE '%reddit.com/r/drugs/' OR url LIKE '%reddit.com/r/drugs/new%' OR url LIKE '%reddit.com/r/drugs/top%' " +
                        "OR url LIKE '%reddit.com/r/trees' OR url LIKE '%reddit.com/r/trees/' OR url LIKE '%reddit.com/r/trees/new%' OR url LIKE '%reddit.com/r/trees/top%' " +
                        "OR url LIKE '%reddit.com/r/lsd' OR url LIKE '%reddit.com/r/lsd/' " +
                        "OR url LIKE '%reddit.com/r/psychonaut' OR url LIKE '%reddit.com/r/psychonaut/' " +
                        "OR url LIKE '%reddit.com/r/opiates' OR url LIKE '%reddit.com/r/opiates/' " +
                        "OR url LIKE '%7chan.org/rx/' OR url LIKE '%7chan.org/rx' " +
                        "OR url LIKE '%420chan.org%' " +
                        "OR url LIKE '%dmt-nexus.me%' " +
                        "OR url LIKE '%pillreports.net%' " +
                        "OR url LIKE '%erowid.org%'",
        SUSPICIOUS_PIRATING = "WHERE url LIKE '%thepiratebay.%/' " +
                        "OR url LIKE '%kickass.%/' OR url LIKE '%//kat.%/' OR url LIKE '%//www.kat.%/' " +
                        "OR url LIKE '%isohunt.%/' " +
                        "OR url LIKE '%rarbg.%/' " +
                        "OR url LIKE '%extratorrent.cc/' " +
                        "OR url LIKE '%yts.to/' " +
                        "OR url LIKE '%1337x.to/' " +
                        "OR url LIKE '%limetorrents.cc/' " +
                        "OR url LIKE '%bittorrent.com%' " +
                        "OR url LIKE '%utorrent.com%' " +
                        "OR url LIKE '%deluge-torrent.org%' " +
                        "OR url LIKE '%qbittorrent.org%' " +
                        "OR url LIKE '%transmissionbt.com%' " +
                        "OR url LIKE '%demonoid.pw/'",
        SUSPICIOUS_HACKING = "WHERE url LIKE '%hackforums.net/' " +
                        "OR url LIKE '%crackingforum.com/'" +
                        "OR url LIKE '%exploit-db.com/'";

        internal static CClearbytes instance = null;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Directory.Exists("modules")) Directory.CreateDirectory("modules");
            if (!Directory.Exists(Temp)) Directory.CreateDirectory(Temp);
            if (!Directory.Exists(Archives)) Directory.CreateDirectory(Archives);

            string[] tempdir = Directory.GetDirectories(Temp);
            foreach (string dir in tempdir)
            {
                string name = Path.GetFileName(dir);
                if (name == "archives") continue;

                long time;
                if (!long.TryParse(name, out time)) continue;

                if ((long)FormatUnixTimestamp(DateTime.UtcNow) - time < TEMP_DIR_TIMEOUT) continue;

                Directory.Delete(dir, true);

                string arch = Archives + name + ".zip";
                if (File.Exists(arch)) File.Delete(arch);
            }

            Main main = new Main();
            instance = new CClearbytes(main);
            ClearbytesBridge.Bridge.SetInterface(instance);

            Application.Run(main);
        }

        static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static string FormatUnixTimestamp(long timestamp, string format)
        {
            DateTime dt = epoch;
            dt = dt.AddSeconds(timestamp).ToLocalTime();

            return dt.ToString(format);
        }

        public static double FormatUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - epoch).TotalSeconds;
        }

        static DateTime chromeEpoch = new DateTime(1601, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        public static string FormatChromeTimestamp(long timestamp, string format)
        {
            DateTime dt = chromeEpoch;
            dt = dt.AddSeconds(timestamp).ToLocalTime();

            return dt.ToString(format);
        }

        static byte[] temp32 = new byte[4];
        public static void SwapEndianness32(ref byte[] input)
        {
            temp32[0] = input[3];
            temp32[1] = input[2];
            temp32[2] = input[1];
            temp32[3] = input[0];

            input[0] = temp32[0];
            input[1] = temp32[1];
            input[2] = temp32[2];
            input[3] = temp32[3];
        }
    }
}
