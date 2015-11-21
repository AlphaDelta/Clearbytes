using System;
using System.Collections.Generic;
using System.Text;

namespace ClearbytesBridge
{
    public struct TitleInfo
    {
        public string Title;
        public string Description;

        public TitleInfo(string Title, string Description)
        {
            this.Title = Title;
            this.Description = Description;
        }
    }
}
