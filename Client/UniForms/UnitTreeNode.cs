using System.Windows.Forms;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Расширяет функционал для отслеживая 
    /// были ли дочерние узлы подгружены.
    /// </summary>
    class UnitTreeNode : TreeNode
    {
        public bool ChildLoaded { set; get;}

        public UnitNode Node { set; get; }
    }
}
