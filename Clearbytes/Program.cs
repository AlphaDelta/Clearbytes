using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Clearbytes
{
    static class Program
    {
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

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

        public static string FormatChromeTimestamp(long timestamp, string format)
        {
            DateTime dt = new DateTime(1601, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt.AddSeconds(timestamp).ToLocalTime();

            return dt.ToString(format);
        }
    }
}
