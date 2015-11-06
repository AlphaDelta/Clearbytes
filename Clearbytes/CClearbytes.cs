using System;
using System.Collections.Generic;
using System.Text;
using ClearbytesBridge;
using System.Drawing;

namespace Clearbytes
{
    internal class CClearbytes : IClearbytes
    {
        Main main;
        public CClearbytes(Main m) { main = m; }

        public void SwitchPanel(InformationType type) { main.SwitchPanel(type); }

        public void SetImage(Image img)
        {
            main.imgData.Image = img;
        }
        public void SetText(string txt) { main.txtData.Text = txt; }
    }
}
