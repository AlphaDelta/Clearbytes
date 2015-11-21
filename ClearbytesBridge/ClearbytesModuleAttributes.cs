using System;
using System.Collections.Generic;
using System.Text;

namespace ClearbytesBridge
{
    public class ClearbytesModuleAttributes : Attribute
    {
        public string Name, Description;
        public bool Active;

        public ClearbytesModuleAttributes(string Name, string Description = "", bool Active = true)
        {
            this.Name = Name;
            this.Description = Description;
            this.Active = Active;
        }
    }
}
