using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using COTES.ISTOK.ASC;

namespace WebClient
{
    public partial class TreeTemplate : System.Web.UI.MasterPage
    {
        protected void TreeViewStructure_Load(object sender, EventArgs e)
        {
            try
            {
                ParametersPageManager parameters =
                    ParametersPageManager.FromQueryStrings(Request.QueryString);

                int object_id = Int32.Parse(parameters[Configuration.Get(Setting.IdObjectMarker)]);
                UnitPropertiesView.Show(this.GetDataService().GetObjectDescriptor(object_id));
                ObjectInformLabel.Text = String.Empty;
            }
            catch
            {
                ObjectInformLabel.Text = Configuration.Get(Setting.ObjectNotSelectedMessage);
            }

            try
            {
                if (TreeViewStructure.Nodes.Count != 0)
                    return;

                WebRemoteDataService data_service = this.GetDataService();
                var root_nodes = 
                    data_service.GetRootStructureLayer();

                FillTreeStructureLayer(TreeViewStructure.Nodes, root_nodes);

                if (Session[Configuration.Get(Setting.TreeContentMarker)] != null)
                {
                    var nodes = (Session[Configuration.Get(Setting.TreeContentMarker)] as TreeNodeCollection);
                    ExpandTreeLevelFromSaveState(TreeViewStructure.Nodes, nodes);
                }
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
            var tabPanel = new AjaxControlToolkit.TabPanel();
            tabPanel.HeaderText = "Совсем левая вкладка";
            //ContentTab.Controls.Add(tabPanel);
            Tabs.Tabs.Add(tabPanel);
        }

        protected void ExpandTreeLevelFromSaveState(TreeNodeCollection current, 
                                                    TreeNodeCollection cache)
        {
            TreeNode temp = null;
            for (int i = 0; i < current.Count; i++)
            {
                temp = GetNodeFromValue(cache, current[i].Value);
                if (temp == null) continue;

                if (temp.Expanded.HasValue && temp.Expanded.Value)
                {
                    current[i].Expand();
                    FillTreeStructureLayer(current[i].ChildNodes,
                                           this.GetDataService().GetStructureLayer(Int32.Parse(current[i].Value)));
                    ExpandTreeLevelFromSaveState(current[i].ChildNodes, temp.ChildNodes);
                }
            }
        }

        protected TreeNode GetNodeFromValue(TreeNodeCollection nodes, 
                                            object value)
        {
            foreach (TreeNode it in nodes)
                if (it.Value.Equals(value))
                    return it;

            return null;
        }

        protected void TreeViewStructure_Unload(object sender, EventArgs e)
        {
            try
            {
                if (TreeViewStructure.Nodes.Count != 0)
                    Session[Configuration.Get(Setting.TreeContentMarker)] = TreeViewStructure.Nodes;
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        private void FillTreeStructureLayer(TreeNodeCollection nodes, IEnumerable<UnitNode> layer)
        {
            var data_service = this.GetDataService();

            nodes.Clear();
            TreeNode temp = null;
            foreach (var it in layer)
            {
                temp = new TreeNode()
                {
                    Text = it.Text,
                    Value = it.Idnum.ToString(),
                    Expanded = false,
                    ImageUrl = data_service.GetTypeIconUrl(it.Typ),
                    NavigateUrl = data_service.GetTypeProcessUrl(it.Typ, it.Idnum)
                };

                if (it.HasChild)
                    temp.ChildNodes.Add(new TreeNode("")
                    {
                        Expanded = false
                    });

                nodes.Add(temp);
            }
        }

        protected void TreeViewStructure_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
        {
            try
            {
                WebRemoteDataService data_service = this.GetDataService();

                var nodes_layer =
                    data_service.GetStructureLayer(int.Parse(e.Node.Value));

                FillTreeStructureLayer(e.Node.ChildNodes, nodes_layer);
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        protected void TreeViewStructure_TreeNodeCollapsed(object sender, TreeNodeEventArgs e)
        {
            try
            {
                e.Node.ChildNodes.Clear();
                e.Node.ChildNodes.Add(new TreeNode() { Expanded = false });
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        public bool SpecificTabVisible
        {
            set
            {
                ContentTab.Visible = value;
                Tabs.ActiveTab = value ? ContentTab : PropertyViewTab;
            }
        }
    }
}
