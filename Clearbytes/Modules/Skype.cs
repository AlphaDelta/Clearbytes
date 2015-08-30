using ClearbytesBridge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("Skype", @"Retrieves data from the Skype user folders", true)]
    public class Skype : ClearbytesModule
    {
        static string SKYPE_PATH = Program.AppData + @"\Skype\";
        public override void Search()
        {
            string[] folders = Directory.GetDirectories(SKYPE_PATH, "*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                if (!File.Exists(folder + @"\main.db")) continue; //Check if it's a user folder

                string skypename = Path.GetFileName(folder);
                SearchNode node = this.AddInformation(skypename, InformationType.Title, new TitleInfo("User folder", String.Format("Contains information about the Skype user '{0}'.", skypename)));

                //Skype avatars
                string avatarpath = folder + @"\Pictures\";
                if (Directory.Exists(avatarpath))
                {
                    SearchNode anode = node.AddInformation("Avatars", InformationType.Title, new TitleInfo("Avatars", "Contains current and previous avatar images."), true);
                    string[] files = Directory.GetFiles(avatarpath, "*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                        anode.AddInformation(Path.GetFileNameWithoutExtension(file), InformationType.ImageFile, file);
                }

                //Skype downloadable media cache
                string mediacache = folder + @"\media_messaging\media_cache\";
                if (Directory.Exists(mediacache))
                {
                    SearchNode mnode = node.AddInformation("Media cache", InformationType.Title, new TitleInfo("Media cache", "Contains downloaded image data and downloadable-image thumbnails."), true);
                    string[] files = Directory.GetFiles(mediacache, "*.jpg", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        Match m = Regex.Match(file, @"[\da-f]{32}");
                        if (!m.Success || m.Groups.Count < 1) continue;
                        mnode.AddInformation(m.Groups[0].Value, InformationType.ImageFile, file);
                    }
                }

                string maindb = folder + @"\main.db";
                if (File.Exists(maindb))
                {
                    CSQLite kvc = new CSQLite(maindb);

                    /*QueryResult res = kvc.QuickQuery("SELECT sql FROM sqlite_master WHERE type='table'");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("eas.db structure\r\n--------------------------------------------------\r\n\r\n");
                    foreach (object[] obj in res.Rows)
                        sb.Append((string)obj[0] + "\r\n");

                    SearchNode dbnode = node.AddInformation("eas.db", InformationType.Text, sb.ToString(), true);*/

                    SearchNode mainnode = node.AddInformation("main.db", InformationType.Title, new TitleInfo("main.db", "Contains user data"), true);

                    QueryResult res = kvc.QuickQuery("SELECT partner_handle,partner_dispname,starttime,finishtime,filepath,filename,filesize,convo_id FROM Transfers");
                    if (res.Returned > 0)
                    {
                        List<ListViewItem> items = new List<ListViewItem>();

                        foreach (object[] obj in res.Rows)
                        {
                            string starttime = (obj[2].GetType() == typeof(DBNull) || (long)obj[2] < 1 ? "" : Program.FormatUnixTimestamp((int)((long)obj[2]), "yyyy-MM-dd H:mm:ss"));
                            string endtime = (obj[3].GetType() == typeof(DBNull) || (long)obj[3] < 1 ? "" : Program.FormatUnixTimestamp((int)((long)obj[3]), "yyyy-MM-dd H:mm:ss"));
                            items.Add(new ListViewItem(new string[] { obj[7].ToString(), obj[0].ToString(), obj[5].ToString(), obj[6].ToString(), obj[4].ToString(), starttime, endtime }));
                        }

                        TableInfo tinfo = new TableInfo(new string[] { "Conversation ID", "Partner handle", "File name", "File size", "File location", "Start time", "Finish time" }, items.ToArray());
                        mainnode.AddInformation("Transfers", InformationType.Table, tinfo);
                    }
                }
            }
        }
    }
}
