using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearbytesBridge
{
    public delegate void Action();

    public enum InformationType
    {
        Title,
        Text,
        Image,
        ImageFile,
        Binary,
        BinaryFile,
        Table,
        Delegate,
        None
    }

    public class Bridge
    {
        static IClearbytes _Interface = null;
        public static IClearbytes Interface { get { return _Interface; } }

        public static List<Image> ImageCache = new List<Image>();

        public static void SetInterface(IClearbytes Interface)
        {
            if (_Interface != null) throw new Exception("Interface cannot be set twice.");

            _Interface = Interface;
        }
    }
}
