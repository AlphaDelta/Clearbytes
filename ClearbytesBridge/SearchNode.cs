using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearbytesBridge
{
    public class SearchNode
    {
        public InformationType Type;
        public string Title;

        public object Data;

        TreeNode TreeNode;

        public readonly ClearbytesModule ParentModule = null;
        public readonly SearchNode Parent = null;

        public List<SearchNode> Nodes = new List<SearchNode>();

        public SearchNode(TreeNode TreeNode, string Title, InformationType Type, object Data, ClearbytesModule ParentModule)
        {
            this.TreeNode = TreeNode;
            this.Title = Title;
            this.Type = Type;
            this.Data = Data;
            this.ParentModule = ParentModule;
        }
        public SearchNode(TreeNode TreeNode, string Title, InformationType Type, object Data, ClearbytesModule ParentModule, SearchNode Parent)
        {
            this.TreeNode = TreeNode;
            this.Title = Title;
            this.Type = Type;
            this.Data = Data;
            this.ParentModule = ParentModule;
            this.Parent = Parent;
        }

        public SearchNode AddInformation(string Title, InformationType Type, object Data, bool forcewait = false)
        {
            if (Type == InformationType.Image) Bridge.ImageCache.Add((Image)Data);

            TreeNode tnode = null;
            if (forcewait)
                this.ParentModule.Parent.TreeView.Invoke((Action)delegate
                {
                    tnode = TreeNode.Nodes.Add(Title);
                });
            else
                this.ParentModule.Parent.TreeView.BeginInvoke((Action)delegate { tnode = TreeNode.Nodes.Add(Title); });

            SearchNode node = new SearchNode(tnode, Title, Type, Data, this.ParentModule, this);
            Nodes.Add(node);

            return node;
        }
    }
}
