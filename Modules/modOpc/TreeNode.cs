using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Modules.modOpc
{
    public class TreeNode
    {
        //List<TreeNode>

        public TreeNode(string p)
        {
            Text = p;

            Nodes = new List<TreeNode>();
        }

        public string Text { get; set; }

        public object Tag { get; set; }

        public List<TreeNode> Nodes { get; private set; }
    }
}
