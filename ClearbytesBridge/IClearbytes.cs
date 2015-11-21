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
    }
}
