using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class MultiSelectTreeViewControl : TreeView
    {
        List<TreeNode> lstSelectedNodes = new List<TreeNode>();
        //TreeNode lastNode = null;
        TreeNode firstNode = null;

        const int WM_PRINTCLIENT = 0x318;
        const int PRF_CLIENT = 0x04;

        public MultiSelectTreeViewControl()
        {
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            //if (Environment.OSVersion.Version.Major < 6)
            //    SetStyle(ControlStyles.UserPaint, true);
            
            //SelectedNodes = new TreeNode[] { };
            //InitializeComponent();
        }

        public event EventHandler OnDeselect;


        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool MultiSelect { get; set; }

        [Browsable(false)]
        public TreeNode[] SelectedNodes
        {
            get { return lstSelectedNodes.ToArray(); }
            set
            {
                lstSelectedNodes.Clear();
                WashNodes();
                if (value != null)
                {
                    lstSelectedNodes.AddRange(value);
                    PaintNodes();
                }
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl && lstSelectedNodes.Contains(e.Node))
            {
                e.Cancel = true;

                WashNode(e.Node);
                lstSelectedNodes.Remove(e.Node);
                //DrawNodes();
                return;
            }

            //lastNode = e.Node;
            if (!bShift) firstNode = e.Node;

            base.OnBeforeSelect(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (!MultiSelect)
            {
                WashNodes();
                lstSelectedNodes.Clear();
                firstNode = e.Node;
            }

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!lstSelectedNodes.Contains(e.Node))
                {
                    lstSelectedNodes.Add(e.Node);
                }
                else
                {
                    WashNodes();
                    lstSelectedNodes.Remove(e.Node);
                }
                PaintNode(e.Node);
            }
            else
            {
                if (bShift)
                {
                    var lst = new List<TreeNode>();

                    WashNodes();
                    lstSelectedNodes.Clear();

                    TreeNode uppernode = firstNode;
                    TreeNode bottomnode = e.Node;

                    if (uppernode == null) uppernode = bottomnode;
                    //if (lstSelectedNodes.Count > 0)
                    //    uppernode = lstSelectedNodes[lstSelectedNodes.Count - 1];
                    //else
                    //    uppernode = bottomnode;

                    bool bParent = IsParent(uppernode, e.Node);
                    if (!bParent)
                    {
                        bParent = IsParent(bottomnode, uppernode);
                        if (bParent)
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!lstSelectedNodes.Contains(n))
                                lst.Add(n);

                            n = n.Parent;
                        }
                    }
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper)
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!lstSelectedNodes.Contains(n))
                                    lst.Add(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            }

                        }
                        else
                        {
                            if (!lstSelectedNodes.Contains(uppernode)) lst.Add(uppernode);
                            if (!lstSelectedNodes.Contains(bottomnode)) lst.Add(bottomnode);
                        }

                    }

                    lstSelectedNodes.AddRange(lst);

                    PaintNodes();
                    //firstNode = e.Node;

                }
                else
                {
                    if (lstSelectedNodes != null && lstSelectedNodes.Count > 0)
                    {
                        WashNodes();
                        lstSelectedNodes.Clear();
                    }
                    lstSelectedNodes.Add(e.Node);
                    PaintNode(e.Node);
                }
            }

            base.OnAfterSelect(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = this.GetNodeAt(e.X, e.Y);

                if (node != null)
                {
                    if (!lstSelectedNodes.Contains(node))
                    {
                        WashNodes();
                        lstSelectedNodes.Clear();
                        this.SelectedNode = node;
                    }
                }
                //else
                //{
                //    WashNodes();
                //    lstSelectedNodes.Clear();
                //    this.SelectedNode = null;
                //}
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            TreeNode node = this.GetNodeAt(e.X, e.Y);

            if (node == null)
            {
                WashNodes();
                lstSelectedNodes.Clear();
                this.SelectedNode = null;
                if (OnDeselect != null)
                    OnDeselect(this, new EventArgs());
            }
            base.OnMouseDown(e);
        }

        private bool IsParent(TreeNode parent, TreeNode child)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");

            foreach (TreeNode item in parent.Nodes)
            {
                if (item == child) return true;
                if (IsParent(item, child)) return true;
            }

            return false;
        }

        protected void WashNodes()
        {
            if (lstSelectedNodes.Count == 0) return;

            foreach (TreeNode n in lstSelectedNodes)
            {
                n.BackColor = Color.Empty;
                n.ForeColor = Color.Empty;
            }
        }
        protected void WashNode(TreeNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            node.BackColor = Color.Empty;
            node.ForeColor = Color.Empty;
        }

        protected void PaintNodes()
        {
            if (!MultiSelect) return;

            foreach (var item in lstSelectedNodes)
            {
                item.BackColor = SystemColors.Highlight;
                item.ForeColor = SystemColors.HighlightText;
            }
        }
        protected void PaintNode(TreeNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (!MultiSelect) return;

            node.BackColor = SystemColors.Highlight;
            node.ForeColor = SystemColors.HighlightText;
        }
    }
}
