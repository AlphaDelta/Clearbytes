using System;
using System.Collections.Generic;
using System.Text;

namespace ClearbytesBridge
{
    public class SearchNodeCollection : List<SearchNode>
    {
        ClearbytesModule Parent;
        public SearchNodeCollection(ClearbytesModule Parent)
        {
            this.Parent = Parent;
        }

        public new void Add(SearchNode item)
        {
            base.Add(item);
        }
    }
}
