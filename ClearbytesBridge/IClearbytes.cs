using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClearbytesBridge
{
    public interface IClearbytes
    {
        void SwitchPanel(InformationType type);

        void SetImage(Image img);
        void SetText(string txt);
        void SetBinary(byte[] arr);
        void SetBinary(string file);
        void SetTitle(TitleInfo info);
        void SetTable(TableInfo info);

        bool IsCanceled();
    }
}
