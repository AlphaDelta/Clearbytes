using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearbytesBridge
{
    public class ClearbytesModule
    {
        bool ParentAdded = false;
        TreeNode _Parent = null;
        public TreeNode Parent { get { return _Parent; } }

        public void SetParent(TreeNode Parent)
        {
            if (_Parent != null) throw new Exception("Parent cannot be set twice.");

            _Parent = Parent;
        }

        TreeView _ParentTreeView = null;
        public TreeView ParentTreeView { get { return _ParentTreeView; } }

        public void SetParentTreeView(TreeView Parent)
        {
            if (_ParentTreeView != null) throw new Exception("Parent tree view cannot be set twice.");

            _ParentTreeView = Parent;
        }

        public List<SearchNode> Nodes = new List<SearchNode>();

        public virtual void Search() { }

        public SearchNode AddInformation(string Title, InformationType Type, object Data)
        {
            if (Type == InformationType.Image) Bridge.ImageCache.Add((Image)Data);

            TreeNode tnode = null;
            _ParentTreeView.Invoke((Action)delegate { tnode = _Parent.Nodes.Add(Title); });

            SearchNode node = new SearchNode(tnode, Title, Type, Data, this);
            Nodes.Add(node);
            //Pointlessly complicated, but I'm already balls deep so what the hell am I supposed to do?
            if (!ParentAdded)
            {
                _ParentTreeView.Invoke((Action)delegate { _ParentTreeView.Nodes.Add(_Parent); });
                ParentAdded = true;
            }

            return node;
        }
    }
}
