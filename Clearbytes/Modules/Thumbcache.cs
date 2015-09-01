using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearbytesBridge;
using System.Drawing;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("Thumbcache",
        @"Reads thumbnail data inside of the %localappdata%\Microsoft\Windows\Explorer\thumbcache_*.db files",
        true)] //Huge memory-sink, enable only in releases and release-candidates
    public class Thumbcache : ClearbytesModule
    {
        const int
            CMMM_MNUM_SIZE = 0x18,
            CMMM_HEADER_SIZE = 0x58,
            BITMAP_NAME_LENGTH_OFFSET = 0x08,
            BITMAP_NAME_OFFSET = 0x1C,
            AVERAGE_HEADER_LENGTH = 4 + 4 + BITMAP_NAME_LENGTH_OFFSET + 4 + 4 + 4 + BITMAP_NAME_OFFSET;

        static string loc = String.Format(@"{0}\Microsoft\Windows\Explorer\", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        string[] sizes = new string[]
        {
            "16",
            "32",
            "48",
            "96",
            "256",
            "1024",
            "1600",
        };

        public override void Search()
        {
            foreach (string s in sizes)
                ReadThumbcache(s);
            //foreach (string s in sizes)
            //    ReadThumbcache(s, "iconcache_");
        }

        static byte[] CMMM_HEADER = { 0x43, 0x4D, 0x4D, 0x4D }; //CMMM
        void ReadThumbcache(string name, string prefix = "thumbcache_")
        {
            string rname = prefix + name;
            FileStream fs = null;
            try
            {
                fs = File.Open(String.Format("{0}{1}.db", loc, rname), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (fs.Length <= CMMM_MNUM_SIZE) return; //If the cache isn't larger than a header is obviously doesn't contain any content, so it's safe to skip

                SearchNode tcnode = this.AddInformation(rname, InformationType.Title, new TitleInfo(rname, String.Format("Contains bitmap data for {0}x{0} thumbnails", name)));

                fs.Seek(CMMM_MNUM_SIZE, SeekOrigin.Current);

                byte[]
                hbuffer = new byte[4], //Magic number
                hlbuffer = new byte[4], //Header + bitmap len
                nlbuffer = new byte[4], //Name length
                padbuffer = new byte[4], //Padding length
                lbuffer = new byte[4]; //Bitmap len

                while (fs.Read(hbuffer, 0, 4) == 4 && WinAPI.memcmp(hbuffer, CMMM_HEADER, 4) == 0)
                {
                    if (fs.Read(hlbuffer, 0, 4) != 4) return; //Get the length of the header + bitmap length
                    int headerbitmaplen = BitConverter.ToInt32(hlbuffer, 0);

                    fs.Seek(BITMAP_NAME_LENGTH_OFFSET, SeekOrigin.Current);

                    if (fs.Read(nlbuffer, 0, 4) != 4) return; //Get the length of the bitmap name
                    int namelen = BitConverter.ToInt32(nlbuffer, 0);

                    if (fs.Read(padbuffer, 0, 4) != 4) return; //Get the length of the padding between the name and the bitmap data
                    int padding = BitConverter.ToInt32(padbuffer, 0);

                    //fs.Seek(BITMAP_LENGTH_OFFSET, SeekOrigin.Current);

                    if (fs.Read(lbuffer, 0, 4) != 4) return; //Get the length of the bitmap data
                    int bitmaplen = BitConverter.ToInt32(lbuffer, 0);

                    //int headerlen = headerbitmaplen - bitmaplen;

                    fs.Seek(BITMAP_NAME_OFFSET, SeekOrigin.Current);

                    byte[] nbuffer = new byte[namelen]; //Name, it's random, but it's better than nothing
                    if (fs.Read(nbuffer, 0, namelen) != namelen) return; //Get bitmap name
                    string bitmapname = Encoding.Unicode.GetString(nbuffer);

                    if (padding > 0)
                        fs.Seek(padding, SeekOrigin.Current);
                    //if (headerlen > CMMM_HEADER_SIZE) //Check if there's padding on the bitmap and skip it if there is
                    //    fs.Seek(headerlen - CMMM_HEADER_SIZE, SeekOrigin.Current);

                    if (bitmaplen > 0)
                    {
                        byte[] bitmapbuffer = new byte[bitmaplen];
                        if (fs.Read(bitmapbuffer, 0, bitmaplen) != bitmaplen) return; //Get bitmap data
                        Bitmap bt;
                        using (MemoryStream ms = new MemoryStream(bitmapbuffer))
                            bt = new Bitmap(ms);

                        tcnode.AddInformation(bitmapname, InformationType.Image, (Image)bt);
                    }

                    int endpadding = headerbitmaplen - (AVERAGE_HEADER_LENGTH + namelen + padding + bitmaplen);
                    if (endpadding > 0)
                        fs.Seek(endpadding, SeekOrigin.Current);
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
    }
}
