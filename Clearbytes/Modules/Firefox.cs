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
        static string FIREFOX_PATH_LOCAL = Program.LocalAppData + @"\Mozilla\Firefox\Profiles\";
        const int INT_256KB = 256 * 1024;

        static byte[]
            PNG_HEADER = { 0x89, 0x50, 0x4E },
            JPG_HEADER = { 0xFF, 0xD8, 0xFF },
            GIF_HEADER = { 0x47, 0x49, 0x46 };

        public override void Search()
        {
            if (!Directory.Exists(FIREFOX_PATH)) return;

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

                /* Cache */
                string localfolder = FIREFOX_PATH_LOCAL + profile;
                string cachefolder = FIREFOX_PATH_LOCAL + profile + @"\cache2\entries";
                if (Directory.Exists(localfolder) && Directory.Exists(cachefolder))
                {
                    string[] cachefiles = Directory.GetFiles(cachefolder);
                    if (cachefiles.Length < 1) continue;

                    SearchNode cachenode = node.AddInformation("Cache", InformationType.Title, new TitleInfo("Cache", String.Format("Contains data that Firefox has cached for the respective profile, such as images, videos, and style sheets.", profile)), true);

                    byte[] intbuff = new byte[4];
                    byte[] headerbuff = new byte[3];
                    foreach (string cachefile in cachefiles)
                    {
                        if (Main.SearchCanceled) return;

                        FileStream fcache = null;
                        try
                        {
                            fcache = File.Open(cachefile, FileMode.Open, FileAccess.Read, FileShare.Read);

                            if (fcache.Length < 16) continue; //Arbitrary minimum
                            if (fcache.Read(headerbuff, 0, 3) != 3) continue;
                            if (WinAPI.memcmp(headerbuff, PNG_HEADER, 3) != 0 &&
                                WinAPI.memcmp(headerbuff, JPG_HEADER, 3) != 0 &&
                                WinAPI.memcmp(headerbuff, GIF_HEADER, 3) != 0)
                                continue;

                            fcache.Seek(-4, SeekOrigin.End);
                            if (fcache.Read(intbuff, 0, 4) != 4) continue;
                            Program.SwapEndianness32(ref intbuff);
                            int metaoffset = BitConverter.ToInt32(intbuff, 0);
                            if (metaoffset < 16) continue;

                            int hash = 4 + (int)Math.Ceiling((float)fcache.Length / (float)INT_256KB) * 2;

                            fcache.Seek(metaoffset + hash, SeekOrigin.Begin);

                            if (fcache.Read(intbuff, 0, 4) != 4) continue;
                            Program.SwapEndianness32(ref intbuff);
                            int metaversion = BitConverter.ToInt32(intbuff, 0);
                            if (metaversion != 1) continue;

                            fcache.Seek(0x14, SeekOrigin.Current);
                            if (fcache.Read(intbuff, 0, 4) != 4) continue;
                            Program.SwapEndianness32(ref intbuff);
                            int metaurllen = BitConverter.ToInt32(intbuff, 0);
                            if (metaurllen < 1) continue;

                            byte[] asciibuff = new byte[metaurllen];
                            if (fcache.Read(asciibuff, 0, metaurllen) != metaurllen) continue;
                            string originurl = Encoding.ASCII.GetString(asciibuff);

                            fcache.Seek(0, SeekOrigin.Begin);

                            cachenode.AddInformation(originurl, InformationType.Delegate,
                                (Action)delegate {
                                    Bridge.Interface.SwitchPanel(InformationType.Image);

                                    try
                                    {
                                        fcache = File.Open(cachefile, FileMode.Open, FileAccess.Read, FileShare.Read);

                                        byte[] bitmapbuff = new byte[metaoffset];
                                        if (fcache.Read(bitmapbuff, 0, metaoffset) != metaoffset) return;
                                        Bitmap bt;
                                        using (MemoryStream ms = new MemoryStream(bitmapbuff))
                                            bt = new Bitmap(ms);

                                        Bridge.Interface.SetImage(bt);
                                    }
                                    catch (IOException) { } //Generic
                                    catch (UnauthorizedAccessException) { } //Lock error
                                    finally
                                    {
                                        if(fcache != null)
                                            fcache.Close();
                                    }
                                });
                        }
                        catch (IOException) { } //Generic
                        catch (UnauthorizedAccessException) { } //Lock error
                        finally
                        {
                            if(fcache != null)
                                fcache.Close();
                        }
                    }
                }
            }
        }

        static void HistorySearch(string param, CSQLite kvc, List<ListViewItem> items, Color back)
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
