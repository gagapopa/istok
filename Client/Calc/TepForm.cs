using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно запроса ссылок и зависимостей
    /// </summary>
    partial class TepForm : BaseAsyncWorkForm
    {
        private FormulaRelation relation = FormulaRelation.Dependence;
        private UnitNode unitNode = null;
        private DateTime curentTime = DateTime.MinValue;

        /// <summary>
        /// Указывает какую зависимость отобразить (Ссылка или Зависимость)
        /// </summary>
        public FormulaRelation Relation
        {
            get { return relation; }
            set { relation = value; }
        }

        /// <summary>
        /// Узел, зависимости которого будут отображаться
        /// </summary>
        public UnitNode Node
        {
            get { return unitNode; }
            set { unitNode = value; }
        }

        private FunctionInfo function = null;
        public FunctionInfo Func
        {
            get { return function; }
            set { function = value; }
        }

        public DateTime CurrentTime
        {
            get { return curentTime; }
            set { curentTime = value; }
        }

        public RevisionInfo Revision { get; set; }

        public ArgumentsValues Arguments { get; set; }

        public bool ReadOnly
        {
            get;
            set;
        }

        public TepForm(StructureProvider strucProvider, ImageList icons)
            : base(strucProvider)
        {
            InitializeComponent();
            tvParameters.ImageList = icons;
            Revision = strucProvider.CurrentRevision;
        }

        public new void ShowDialog()
        {
            MdiParent = null;
            base.ShowDialog();
        }

        private void UpdateView()
        {
            tvParameters.Nodes.Clear();

            if (unitNode != null && unitNode is CalcParameterNode)
                rtbSourceFormula.Text = ((CalcParameterNode)unitNode).Formula;
            //AsyncOperationWatcher<Object> watcher = null;
            object res = null;

            switch (relation)
            {
                case FormulaRelation.Dependence:
                    if (!(unitNode is ParameterNode))
                        throw new Exception("Некорретный тип узла");
                    Text = "Поиск зависимостей" + (unitNode != null ? "  - " + unitNode.Text + " [" + unitNode.Code + "]" : "");
                    //res = strucProvider.GetDependence(Revision, unitNode.Idnum);
                    ShowDependences(strucProvider.GetDependence(Revision, unitNode.Idnum));
                    break;
                case FormulaRelation.Reference:
                    if (function == null)
                    {
                        Text = "Поиск ссылок" + (unitNode != null ? "  - " + unitNode.Text + " [" + unitNode.Code + "]" : "");
                        //res = strucProvider.GetReference(unitNode.Code);
                        ShowReferences(strucProvider.GetReference(unitNode.Code));
                    }
                    else
                    {
                        Text = "Поиск ссылок" + (function != null ? "  - " + function.Name/* + " [" + unitNode.Code + "]"*/ : "");
                        //res = strucProvider.GetReference(function.Name);
                        ShowReferences(strucProvider.GetReference(function.Name));
                    }
                    break;
            }

            if (res != null)
            {
                NodesReceived(res);
            }
        }

        private void ShowDependences(ParameterNodeDependence[] parameterNodeDependence)
        {
            ParameterNode[] nodes;

            tvParameters.ShowPlusMinus = true;
            tvParameters.ShowRootLines = true;
            nodesDictionary = new Dictionary<int, ParameterNode[]>();

            foreach (var tuple in parameterNodeDependence)
            {
                nodesDictionary[tuple.ParameterId] = tuple.Dependences;
            }

            nodesDictionary.TryGetValue(Node.Idnum, out nodes);

            if (nodes == null) return;

            DisplayTreeNodes(nodes, tvParameters.Nodes);
        }

        private void ShowReferences(ParameterNodeReference[] parameterNodeReference)
        {
            var revisions = (from p in parameterNodeReference select p.Revision).Distinct();

            foreach (var revision in revisions)
            {
                var treeNode = tvParameters.Nodes.Add(revision.ToString());
                treeNode.Tag = revision;

                DisplayTreeNodes((from p in parameterNodeReference where p.Revision.Equals(revision) select p.ParameterNode).ToArray(), treeNode.Nodes);
                treeNode.Expand();
            }
        }

        Dictionary<int, ParameterNode[]> nodesDictionary;

        private void NodesReceived(Object x)
        {
            if (InvokeRequired)
                Invoke((Action<Object>)NodesReceived, x);
            else
            {
                ParameterNode[] nodes = x as ParameterNode[];
                ParameterNodeDependence[] dependences;

                if ((dependences = x as ParameterNodeDependence[]) != null)
                {
                    tvParameters.ShowPlusMinus = true;
                    tvParameters.ShowRootLines = true;
                    nodesDictionary = new Dictionary<int, ParameterNode[]>();

                    foreach (var tuple in dependences)
                    {
                        nodesDictionary[tuple.ParameterId] = tuple.Dependences;
                    }

                    nodesDictionary.TryGetValue(Node.Idnum, out nodes);
                }

                if (nodes == null) return;

                DisplayTreeNodes(nodes, tvParameters.Nodes);
            }
        }

        private void tvParameters_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ParameterNode parameterNode;
            ParameterNode[] nodes;

            if ((parameterNode = e.Node.Tag as ParameterNode) != null
                && nodesDictionary != null
                && nodesDictionary.TryGetValue(parameterNode.Idnum, out nodes))
            {
                e.Node.Nodes.Clear();

                DisplayTreeNodes(nodes, e.Node.Nodes);
            }
        }

        private void DisplayTreeNodes(ParameterNode[] nodes, TreeNodeCollection treeNodes)
        {
            ParameterNode parameterNode;
            TreeNode node;

            tvParameters.BeginUpdate();
            foreach (ParameterNode item in nodes)
            {
                parameterNode = item;

                node = new TreeNode(parameterNode.GetNodeText());

                if (item.Idnum == -1)
                    node.Text = String.Format("Параметр не найден: {0}", parameterNode.GetNodeText());

                node.Tag = parameterNode;
                node.ImageKey = ((int)parameterNode.Typ).ToString();
                node.SelectedImageKey = ((int)parameterNode.Typ).ToString();
                node.ToolTipText = parameterNode.FullName;
                node.Checked = true;
                if (nodesDictionary != null && nodesDictionary.ContainsKey(item.Idnum))
                    node.Nodes.Add(String.Empty);

                treeNodes.Add(node);
            }
            tvParameters.EndUpdate();
        }

        private void tep_change_Load(object sender, EventArgs e)
        {
            if (OnSave == null) tvParameters.CheckBoxes = false;
            UpdateView();
            if (relation == FormulaRelation.CopyBranch || relation == FormulaRelation.UpdateBranch)
            {
                //findForm = new FindReplaceForm();
                //findForm.Find += new EventHandler<FindReplaceEventArgs>(findForm_Find);
                ////FindComplete += new EventHandler<FindReplaceCompleteEventArgs>(findForm.sourceForm_FindComplete);
                //findForm.Show(this);
            }
        }

        public IEnumerable<ParameterNodeReference> NodesToSave
        {
            get
            {
                //if (relation == FormulaRelation.CopyBranch || relation == FormulaRelation.UpdateBranch)
                //{
                //    TreeNode currentNode = tvParameters.Nodes[0];
                //    UnitNode retNode = null;
                //    try
                //    {
                //        retNode = getNodesToSave(currentNode);
                //    }
                //    catch (Exception exc)
                //    {
                //        MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        return null;
                //    }
                //    return new UnitNode[] { retNode };
                //}

                List<ParameterNodeReference> lstNodes = new List<ParameterNodeReference>();

                foreach (TreeNode item in tvParameters.Nodes)
                {
                    RevisionInfo revision = (RevisionInfo)item.Tag;
                    if (revision != null)
                    {
                        lstNodes.AddRange(getReferencesToSave(revision, item.Nodes));
                    }
                    else
                    {
                        lstNodes.AddRange(getReferencesToSave(RevisionInfo.Default, tvParameters.Nodes));
                        break;
                    }
                }

                return lstNodes.AsReadOnly();
            }
        }

        private IEnumerable<ParameterNodeReference> getReferencesToSave(RevisionInfo revision, TreeNodeCollection treeNodeCollection)
        {
            foreach (TreeNode item in treeNodeCollection)
            {
                if (item.Checked)
                {
                    ParameterNode parameter = item.Tag as ParameterNode;
                    if (parameter != null)
                    {
                        yield return new ParameterNodeReference()
                        {
                            ParameterNode = parameter,
                            Revision = revision
                        };
                    }
                }
            }
        }

        public void Save()
        {
            if (OnSave != null)
                OnSave(this, EventArgs.Empty);
        }

        private UnitNode getNodesToSave(TreeNode currentNode)
        {
            UnitNode retNode = null;
            if (currentNode != null && currentNode.Tag != null && currentNode.Tag is UnitNode && currentNode.Checked)
            {
                ParameterNode param = null;
                UnitNode node = (UnitNode)currentNode.Tag;

                if (param != null)
                {
                    if (relation != FormulaRelation.UpdateBranch || param.Idnum != node.Idnum)
                        throw new Exception(String.Format("Параметр с кодом '{0}' уже существует", param.Code));
                }
                retNode = (UnitNode)node.Clone();
                foreach (TreeNode treeNode in currentNode.Nodes)
                {
                    UnitNode unitNode = getNodesToSave(treeNode);
                }
            }
            return retNode;
        }

        private void tvParameters_DoubleClick(object sender, EventArgs e)
        {

        }

        COTES.ISTOK.ASC.TypeConverters.IntervalTypeConverter intervalConverter =
            new COTES.ISTOK.ASC.TypeConverters.IntervalTypeConverter();

        String doubleFormat = "0.000";

        public Dictionary<int, double> DummyValues { get; set; }

        private async void tvParameters_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ParameterNode node;
            UnitNode unitNode;

            ClearInfo();
            if (tvParameters.SelectedNode == null) return;

            unitNode = tvParameters.SelectedNode.Tag as UnitNode;
            if (unitNode == null)
                return;
            txtCode.Text = unitNode.Code;

            node = unitNode as ParameterNode;
            if (node == null) return;
            if (!strucProvider.CheckAccess(node, Privileges.Read))
                return;

            RevisionInfo revision;
            TreeNode treeNode = e.Node;
            while ((revision = treeNode.Tag as RevisionInfo) == null 
                && treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            if (revision == null)
                revision = Revision;


            txtInterval.Text = intervalConverter.ConvertToString(strucProvider.GetParameterInterval(node.Idnum));

            if (node is CalcParameterNode)
                rtbFormula.Text = ((CalcParameterNode)node).GetFormulaStorage().Get(revision);
            else rtbFormula.Text = String.Empty;
            try
            {
                txtValue.Text = "n/a";
                if (curentTime != DateTime.MinValue)
                {
                    dateTimeLabel.Text = curentTime.ToString("yyyy-MM-dd");// HH:mm:ss");
                    double val;
                    Interval interval = Interval.Zero;
                    ParameterNode parameter = Node as ParameterNode;

                    if (DummyValues != null
                        && DummyValues.TryGetValue(node.Idnum, out val))
                    {
                        if (!double.IsNaN(val))
                            txtValue.Text = val.ToString(doubleFormat);
                        else txtValue.Text = "n/a";
                    }
                    else
                    {
                        if ((interval = await Task.Factory.StartNew(() => strucProvider.GetParameterInterval(node.Idnum))) == Interval.Zero && parameter != null)
                            interval = await Task.Factory.StartNew(() => strucProvider.GetParameterInterval(parameter.Idnum));
                        ArgumentsValues args = null;
                        if (Arguments != null)
                        {
                            OptimizationGateNode optimizationNode = await GetOpimizationNode(node, Arguments);
                            //if (optimizationNode != null)
                            //    args = Arguments.Degrade(optimizationNode.Idnum);
                        }
                        //throw new NotImplementedException();
                        AsyncOperationWatcher<UAsyncResult> watcher = strucProvider.BeginGetValues(node.Idnum, args, curentTime, interval.GetNextTime(curentTime));
                        watcher.AddValueRecivedHandler(valueReceived);
                        RunWatcher(watcher);
                    }
                }
                else dateTimeLabel.Text = String.Empty;
            }
            catch { }
        }

        private async Task<OptimizationGateNode> GetOpimizationNode(UnitNode node, ArgumentsValues args)
        {
            UnitNode unitNode = node;
            OptimizationGateNode optimizationNode = null;
            //List<int> optimizationNodeList = new List<int>();

            //for (int i = 0; i < args.Length; i++)
            //    optimizationNodeList.Add(args[i].OptimizationNodeID);

            while (unitNode != null
                && ((optimizationNode = unitNode as OptimizationGateNode) == null
                //|| !optimizationNodeList.Contains(optimizationNode.Idnum)
                ))
                unitNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode(unitNode.ParentId));

            return optimizationNode;
        }

        //private void valueReceived(Package pack)
        private void valueReceived(UAsyncResult result)
        {
            if (InvokeRequired) Invoke((Action<UAsyncResult>)valueReceived, result);
            else
            {
                if (result != null && result.Packages != null)
                {
                    Package pack = result.Packages.First();
                    if (pack != null && pack.Values.Count > 0)
                    {
                        double val = pack.Values[0].Value;
                        if (!double.IsNaN(val))
                            txtValue.Text = val.ToString(doubleFormat);
                        else txtValue.Text = "n/a";
                    }
                }
            }
        }


        private void ClearInfo()
        {
            txtCode.Text = "";
            txtInterval.Text = "";
            rtbFormula.Text = "";
        }

        private void tep_change_KeyDown(object sender, KeyEventArgs e)
        {
            if (relation == FormulaRelation.CopyBranch || relation == FormulaRelation.UpdateBranch)
            {
                //if (e.Control && e.KeyCode == Keys.F)
                //{
                //    if (findForm == null)
                //    {
                //        findForm = new FindReplaceForm();
                //        findForm.Find += new EventHandler<FindReplaceEventArgs>(findForm_Find);
                //        //FindComplete += new EventHandler<FindReplaceCompleteEventArgs>(findForm.sourceForm_FindComplete);
                //        findForm.Show(this);
                //    }
                //    else findForm.Show(this);
                //}
                //else if (e.KeyCode == Keys.F3 && findForm != null)
                //{
                //    findForm.FindNext();
                //}
            }
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            tvParameters.TreeViewNodeSorter = new NodeSorter();
            tvParameters.Sort();
        }

        private void parametersMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (tvParameters.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }
            ParameterNode node = tvParameters.SelectedNode.Tag as ParameterNode;

            if (node.Idnum < 0 
                || !strucProvider.CheckAccess(node, Privileges.Read))
            {
                e.Cancel = true;
                return;
            }

            if (node != null)
            {
                editToolStripMenuItem.Visible = !strucProvider.Session.User.StructureHide
                    && strucProvider.CheckAccess(node, Privileges.Write);
            }
        }

        private async void viewValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ParameterNode node = tvParameters.SelectedNode.Tag as ParameterNode;

            ParameterValuesController valuesController = new ParameterValuesController(strucProvider, node, Arguments, curentTime);
            ParameterValuesEditorForm valuesForm = new ParameterValuesEditorForm(strucProvider, valuesController);
            Program.MainForm.AddExtendForm(valuesForm);
            valuesController.AsyncForm = valuesForm;
            //valuesForm.ReadOnly = ReadOnly;
            valuesForm.FormClosing += new FormClosingEventHandler(valuesForm_FormClosing);
            //if (curentTime != DateTime.MinValue) valuesForm.CurrentTime = curentTime;
            if ((await Task.Factory.StartNew(() => strucProvider.GetParameterInterval(node.Idnum))) == Interval.Zero)
            {
                //if (node.Typ == UnitTypeId.Parameter)
                //    valuesForm.CurrentInterval = rds.GetParameterInterval(Node);
                ParameterNode parent = Node as ParameterNode;
                if (parent != null)
                {
                    valuesForm.CurrentInterval = await Task.Factory.StartNew(() => strucProvider.GetParameterInterval(parent.Idnum));
                }
            }

            valuesForm.Show(this);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterNode node = tvParameters.SelectedNode.Tag as ParameterNode;

            Program.MainForm.EditParam(node);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterNode node = tvParameters.SelectedNode.Tag as ParameterNode;
            
            Program.MainForm.ShowParam(node);
        }

        void valuesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ParameterValuesEditorForm valuesForm = sender as ParameterValuesEditorForm;
            if (valuesForm != null)
            {
                if (valuesForm.ValuesController.CurrentStartTime != DateTimePicker.MinimumDateTime) curentTime = valuesForm.ValuesController.CurrentStartTime;
            }
        }

        private void tvParameters_DoubleClick_1(object sender, EventArgs e)
        {
            ParameterNode node;

            if (tvParameters.SelectedNode == null ||
                tvParameters.SelectedNode.Tag == null ||
                !(tvParameters.SelectedNode.Tag is ParameterNode))
                return;

            node = tvParameters.SelectedNode.Tag as ParameterNode;
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            Control control = sender as Control;

            txtInterval.Width = txtValue.Width = (control.Size.Width - 10) / 2;
            txtValue.Left = txtInterval.Right + 5;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Save();
            this.Close();
        }

        private void tvParameters_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                tvParameters.SelectedNode = tvParameters.GetNodeAt(e.X, e.Y);

                //if (tvParameters.SelectedNode != null)
                //{
                //    parametersMenuStrip.Show(tvParameters, e.Location);
                //}
            }
        }

        public event EventHandler OnSave;
    }

    public class NodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            return string.Compare(tx.ImageKey, ty.ImageKey);
        }
    }


    //public class TepChangeEventArgs : EventArgs
    //{
    //    UnitNode[] p_nodes = null;

    //    public TepChangeEventArgs(UnitNode[] nodes)
    //    {
    //        p_nodes = nodes;
    //    }

    //    public UnitNode[] Nodes
    //    {
    //        get { return p_nodes; }
    //    }
    //}

    public enum FormulaRelation
    {
        /// <summary>
        /// Ссылка на данный параметр
        /// </summary>
        Reference,
        /// <summary>
        /// Зависимость параметра от других
        /// </summary>
        Dependence,
        /// <summary>
        /// Копирование ветки, с возможностью замены
        /// </summary>
        CopyBranch,
        UpdateBranch
    }
}