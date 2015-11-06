using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearbytesBridge
{
    //TODO: Sort all this shit into their own files
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

    public interface IClearbytes
    {
        void SwitchPanel(InformationType type);

        void SetImage(Image img);
        void SetText(string txt);
    }

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

    public delegate void Action();
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
                this.ParentModule.Parent.TreeView.Invoke((Action)delegate {
                    tnode = TreeNode.Nodes.Add(Title);
                });
            else
                this.ParentModule.Parent.TreeView.BeginInvoke((Action)delegate { tnode = TreeNode.Nodes.Add(Title); });

            SearchNode node = new SearchNode(tnode, Title, Type, Data, this.ParentModule, this);
            Nodes.Add(node);

            return node;
        }
    }

    public class TableInfo
    {
        public readonly string[] Columns;
        public readonly ListViewItem[] Rows;

        public TableInfo(string[] Columns, ListViewItem[] Rows)
        {
            this.Columns = Columns;
            this.Rows = Rows;
        }
    }
}
