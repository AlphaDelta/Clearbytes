using ClearbytesBridge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("Firefox", @"Retrieves data from the Firefox profile folders", true)]
    public class Firefox : ClearbytesModule
    {
        static string FIREFOX_PATH = Program.AppData + @"\Mozilla\Firefox\Profiles\";
        public override void Search()
        {
            string[] folders = Directory.GetDirectories(FIREFOX_PATH, "*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                if (!File.Exists(folder + @"\key3.db")) continue;

                string profile = Path.GetFileName(folder);
                SearchNode node = this.AddInformation(profile, InformationType.Title, new TitleInfo("Profile folder", String.Format("Contains information about the Firefox profile '{0}'.", profile)));

                string cookiedb = folder + @"\cookies.sqlite";
                if (File.Exists(cookiedb))
                {
                    CSQLite kvc = new CSQLite(cookiedb);

                    QueryResult res = kvc.QuickQuery("SELECT id,baseDomain,name,value,host,path,expiry,lastAccessed,creationTime,isSecure,isHttpOnly FROM moz_cookies");
                    if (res.Returned > 0)
                    {
                        List<ListViewItem> items = new List<ListViewItem>();

                        foreach (object[] obj in res.Rows)
                        {
                            if (Main.SearchCanceled) return;

                            string expiry = (obj[6].GetType() == typeof(DBNull) || (long)obj[6] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[6], "yyyy-MM-dd H:mm:ss"));
                            string lastaccessed = (obj[7].GetType() == typeof(DBNull) || (long)obj[7] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[7] / 1000000, "yyyy-MM-dd H:mm:ss"));
                            string creationtime = (obj[8].GetType() == typeof(DBNull) || (long)obj[8] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[8] / 1000000, "yyyy-MM-dd H:mm:ss"));
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    obj[3].ToString(),
                                    obj[4].ToString(),
                                    obj[5].ToString(),
                                    expiry,
                                    lastaccessed,
                                    creationtime,
                                    ((long)obj[9] == 1 ? "True" : "False"),
                                    ((long)obj[10] == 1 ? "True" : "False"),
                                }));
                        }

                        TableInfo tinfo = new TableInfo(new string[] { "ID", "Base domain", "Name", "Value", "Host", "Path", "Expires", "Last Accessed", "Created", "Is secure", "Is HTTP Only" }, items.ToArray());
                        node.AddInformation("Cookies", InformationType.Table, tinfo);
                    }
                }


                string placesdb = folder + @"\places.sqlite";
                if (File.Exists(placesdb))
                {
                    CSQLite kvc = new CSQLite(placesdb);

                    List<ListViewItem> items = new List<ListViewItem>();

                    //Yellow - TOR/TOR related
                    HistorySearch("WHERE url LIKE '%torproject.org/download%' OR url LIKE '%dist.torproject.org%' OR url LIKE '%hiddenwiki%' OR url LIKE '%hidden+wiki%' OR url LIKE '%hidden%20wiki%'", kvc, items, Color.Yellow);
                    
                    //Purple - Drugs/darknet
                    HistorySearch("WHERE url LIKE '%reddit.com/r/darknetmarkets' OR url LIKE '%reddit.com/r/darknetmarkets/' OR url LIKE '%reddit.com/r/darknetmarkets/new%' OR url LIKE '%reddit.com/r/darknetmarkets/top%' " +
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
                        kvc, items, Color.MediumPurple);

                    //Cyan - Torrenting/Pirating
                    HistorySearch("WHERE url LIKE '%thepiratebay.%/' " +
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
                        kvc, items, Color.Cyan);

                    //Gray - Hacking/Cracking
                    HistorySearch("WHERE url LIKE '%hackforums.net/' " +
                        "OR url LIKE '%crackingforum.com/'" +
                        "OR url LIKE '%exploit-db.com/'",
                        kvc, items, Color.Gray);

                    if (items.Count > 0)
                    {
                        TableInfo tinfo = new TableInfo(new string[] { "ID", "Title", "URL", "Last visit" }, items.ToArray());
                        node.AddInformation("Suspicious history", InformationType.Table, tinfo);
                    }
                }

                string frmdb = folder + @"\formhistory.sqlite";
                if (File.Exists(frmdb))
                {
                    CSQLite kvc = new CSQLite(frmdb);

                    QueryResult res = kvc.QuickQuery("SELECT id,fieldname,value,firstUsed,lastUsed FROM moz_formhistory ORDER BY fieldname");
                    if (res.Returned > 0)
                    {
                        List<ListViewItem> items = new List<ListViewItem>();

                        foreach (object[] obj in res.Rows)
                        {
                            if (Main.SearchCanceled) return;

                            string created = (obj[3].GetType() == typeof(DBNull) || (long)obj[3] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[3] / 1000000, "yyyy-MM-dd H:mm:ss"));
                            string lastused = (obj[4].GetType() == typeof(DBNull) || (long)obj[4] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[4] / 1000000, "yyyy-MM-dd H:mm:ss"));

                            Color c = Color.White;
                            string fname = obj[1].ToString();
                            if (fname.Contains("name")) c = Color.Orange;
                            else if (fname.Contains("email")) c = Color.CornflowerBlue;
                            else if (fname.Contains("address") || fname.Contains("country") || fname.Contains("postcode") || fname.Contains("areacode")) c = Color.Red;

                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    created,
                                    lastused,
                                }) { BackColor = c });
                        }

                        TableInfo tinfo = new TableInfo(new string[] { "ID", "Name", "Value", "Created", "Last used" }, items.ToArray());
                        node.AddInformation("Form history", InformationType.Table, tinfo);
                    }
                }


                //downloads.sqlite empty???
                /*string dldb = folder + @"\downloads.sqlite";
                if (File.Exists(dldb))
                {
                    CSQLite kvc = new CSQLite(dldb);
                    
                    QueryResult res = kvc.QuickQuery("SELECT id,name,source,target,startTime,endTime,referrer,maxBytes,mimeType FROM moz_downloads");
                    if (res.Returned > 0)
                    {
                        List<ListViewItem> items = new List<ListViewItem>();

                        foreach (object[] obj in res.Rows)
                        {
                            //string starttime = (obj[4].GetType() == typeof(DBNull) || (long)obj[4] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[4], "yyyy-MM-dd H:mm:ss"));
                            //string endtime = (obj[5].GetType() == typeof(DBNull) || (long)obj[5] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[5], "yyyy-MM-dd H:mm:ss"));
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    obj[3].ToString(),
                                    obj[7].ToString(),
                                    obj[8].ToString(),
                                    obj[6].ToString(),
                                    obj[4].ToString(),
                                    obj[5].ToString(),
                                }));
                        }

                        TableInfo tinfo = new TableInfo(new string[] { "ID", "Name", "Source", "Target", "Size", "Mime Type", "Referrer", "Start Time", "End Time" }, items.ToArray());
                        node.AddInformation("Downloads (Pre Firefox 21)", InformationType.Table, tinfo);
                    }
                }*/
            }
        }

        public static void HistorySearch(string param, CSQLite kvc, List<ListViewItem> items, Color back)
        {
            QueryResult res = kvc.QuickQuery("SELECT id,title,url,last_visit_date FROM moz_places " + param);
            if (res.Returned > 0)
            {
                foreach (object[] obj in res.Rows)
                {
                    if (Main.SearchCanceled) return;

                    string lastvisit = (obj[3].GetType() == typeof(DBNull) || (long)obj[3] < 1 ? "" : Program.FormatUnixTimestamp((long)obj[3] / 1000000, "yyyy-MM-dd H:mm:ss"));

                    items.Add(new ListViewItem(
                        new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    lastvisit,
                                }) { BackColor = back });
                }
            }
        }
    }
}
