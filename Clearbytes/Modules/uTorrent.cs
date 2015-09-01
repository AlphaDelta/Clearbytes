using ClearbytesBridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("uTorrent", @"Retrieves uTorrent history and settings", true)]
    public class uTorrent : ClearbytesModule
    {
        static string UTORRENT_PATH = Program.AppData + @"\uTorrent\";
        public override void Search()
        {
            if (!Directory.Exists(UTORRENT_PATH)) return;

            string[] history = Directory.GetFiles(UTORRENT_PATH, "*.torrent", SearchOption.TopDirectoryOnly);

            if (history.Length > 0)
            {
                SearchNode hnode = this.AddInformation("History", InformationType.Title, new TitleInfo("History", "Contains all previously downloaded torrent files and magnet links"));

                foreach (string t in history)
                {
                    try
                    {
                        hnode.AddInformation(Path.GetFileNameWithoutExtension(t), InformationType.BinaryFile, t);
                    }
                    catch { }
                }
            }
        }
    }
}
