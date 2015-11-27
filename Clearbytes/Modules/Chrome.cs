using ClearbytesBridge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("Google Chrome", @"Retrieves data from the Chrome profile folders", true)]
    public class Chrome : ClearbytesModule
    {
        static string CHROME_PATH = Program.LocalAppData + @"\Google\Chrome\User Data\";
        public override void Search()
        {
            if (!Directory.Exists(CHROME_PATH)) return;

            string[] folders = Directory.GetDirectories(CHROME_PATH, "*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                if (!File.Exists(folder + @"\History")) continue;

                string profile = Path.GetFileName(folder);
                SearchNode node = this.AddInformation(profile, InformationType.Title, new TitleInfo("Profile folder", String.Format("Contains information about the Chrome profile '{0}'.", profile)));

                string placesdb = folder + @"\History";
                CSQLite kvc = new CSQLite(placesdb);

                /* History */
                List<ListViewItem> items = new List<ListViewItem>();

                //Yellow - TOR/TOR related
                HistorySearch(Program.SUSPICIOUS_TOR, kvc, items, Color.Yellow);

                //Red - I2P/I2P related
                HistorySearch(Program.SUSPICIOUS_I2P, kvc, items, Color.Red);

                //Purple - Drugs/darknet
                HistorySearch(Program.SUSPICIOUS_DARKNET, kvc, items, Color.MediumPurple);

                //Cyan - Torrenting/Pirating
                HistorySearch(Program.SUSPICIOUS_PIRATING, kvc, items, Color.Cyan);

                //Gray - Hacking/Cracking
                HistorySearch(Program.SUSPICIOUS_HACKING, kvc, items, Color.Gray);

                if (items.Count > 0)
                {
                    TableInfo tinfo = new TableInfo(new string[] { "ID", "Title", "URL", "Visit count", "Last visit" }, items.ToArray());
                    node.AddInformation("Suspicious history", InformationType.Table, tinfo);
                }

                /* Downloads */
                QueryResult res = kvc.QuickQuery("SELECT id,target_path,start_time,total_bytes,end_time,mime_type,original_mime_type FROM downloads");
                if (res.Returned > 0)
                {
                    items = new List<ListViewItem>();

                    foreach (object[] obj in res.Rows)
                    {
                        items.Add(new ListViewItem(
                            new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    obj[3].ToString(),
                                    obj[4].ToString(),
                                    obj[5].ToString(),
                                    obj[6].ToString(),
                                }));
                    }

                    TableInfo tinfo = new TableInfo(new string[] { "ID", "target_path", "start_time count", "total_bytes", "end_time", "mime_type", "original_mime_type" }, items.ToArray());
                    node.AddInformation("Downloads", InformationType.Table, tinfo);
                }

                res = kvc.QuickQuery("SELECT id,chain_index,url FROM downloads_url_chains");
                if (res.Returned > 0)
                {
                    items = new List<ListViewItem>();

                    foreach (object[] obj in res.Rows)
                    {
                        items.Add(new ListViewItem(
                            new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                }));
                    }

                    TableInfo tinfo = new TableInfo(new string[] { "ID", "chain_index", "url" }, items.ToArray());
                    node.AddInformation("Download url chains", InformationType.Table, tinfo);
                }
                
                /* Web data */
                //TODO: Use autofill_profiles to build a table of information rather than having it fragmented as it currenty is
                string webdb = folder + @"\Web Data";
                if (File.Exists(webdb))
                {
                    kvc = new CSQLite(webdb);

                    SearchNode webnode = node.AddInformation("Web data", InformationType.Title, new TitleInfo("Web data", "Contains autofill data such as names, emails, and credit card information."), true);

                    /* Names */
                    items = new List<ListViewItem>();
                    res = kvc.QuickQuery("SELECT guid,full_name,first_name,middle_name,last_name FROM autofill_profile_names");
                    if (res.Returned > 0)
                    {
                        foreach (object[] obj in res.Rows)
                        {
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    obj[3].ToString(),
                                    obj[4].ToString()
                                }));
                        }
                        TableInfo tinfo = new TableInfo(new string[] { "Group ID", "Full name", "First name", "Middle name", "Last name" }, items.ToArray());
                        webnode.AddInformation("Names", InformationType.Table, tinfo);
                    }

                    /* Emails */
                    items = new List<ListViewItem>();
                    res = kvc.QuickQuery("SELECT guid,email FROM autofill_profile_emails");
                    if (res.Returned > 0)
                    {
                        foreach (object[] obj in res.Rows)
                        {
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString()
                                }));
                        }
                        TableInfo tinfo = new TableInfo(new string[] { "Group ID", "Email" }, items.ToArray());
                        webnode.AddInformation("Emails", InformationType.Table, tinfo);
                    }

                    /* Credit card information */
                    List<ListViewItem> ccs = new List<ListViewItem>();
                    res = kvc.QuickQuery("SELECT name_on_card,expiration_month,expiration_year,origin FROM credit_cards");
                    if (res.Returned > 0)
                    {
                        foreach (object[] obj in res.Rows)
                        {
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    "XXXX XXXX XXXX XXXX",
                                    String.Format("{0}/{1}", obj[1].ToString(), obj[2].ToString()),
                                    obj[3].ToString()
                                }));
                        }
                    }
                    res = kvc.QuickQuery("SELECT name_on_card,last_four,exp_month,exp_year FROM masked_credit_cards");
                    if (res.Returned > 0)
                    {
                        foreach (object[] obj in res.Rows)
                        {
                            items.Add(new ListViewItem(
                                new string[] {
                                    obj[0].ToString(),
                                    "XXXX XXXX XXXX " + obj[1].ToString(),
                                    String.Format("{0}/{1}", obj[2].ToString(), obj[3].ToString()),
                                    ""
                                }));
                        }
                    }

                    if (ccs.Count > 0)
                    {
                        TableInfo tinfo = new TableInfo(new string[] { "Full name", "Number", "Exp date", "Origin" }, ccs.ToArray());
                        webnode.AddInformation("Credit card information", InformationType.Table, tinfo);
                    }
                }
            }
        }

        static void HistorySearch(string param, CSQLite kvc, List<ListViewItem> items, Color back)
        {
            QueryResult res = kvc.QuickQuery("SELECT id,title,url,visit_count,last_visit_time FROM urls " + param);
            if (res.Returned > 0)
            {
                foreach (object[] obj in res.Rows)
                {
                    if (Main.SearchCanceled) return;

                    string lastvisit = (obj[4].GetType() == typeof(DBNull) || (long)obj[4] < 1 ? "" : Program.FormatChromeTimestamp((long)obj[4] / 1000000, "yyyy-MM-dd H:mm:ss"));

                    items.Add(new ListViewItem(
                        new string[] {
                                    obj[0].ToString(),
                                    obj[1].ToString(),
                                    obj[2].ToString(),
                                    obj[3].ToString(),
                                    lastvisit,
                                }) { BackColor = back });
                }
            }
        }
    }
}
