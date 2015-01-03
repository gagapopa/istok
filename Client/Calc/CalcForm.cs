using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно расчета
    /// </summary>
    partial class CalcForm : BaseAsyncWorkForm
    {
        public UnitNode[] UnitNodes { get; set; }
        UnitNode uparam;
        DateTime dt;
        List<NodeBack> lstNodeBacks = new List<NodeBack>();
        Interval interval = Interval.Zero;
        string dateFormat = "";

        Dictionary<int, TreeNode> dicIdnum = new Dictionary<int, TreeNode>();
        Dictionary<int, UnitNode> dicNodes = new Dictionary<int, UnitNode>();

        AsyncOperationWatcher<UAsyncResult> curretnCalcWatcher;
        int[] requiredTypes = new int[] { (int)UnitTypeId.TEP, (int)UnitTypeId.TEPTemplate };
        bool calc_flag;
        //RemoteDataService rds;
        
        ToolStripStatusLabel lblTime;
        public CalcForm(StructureProvider strucProvider, ImageList imageList)
            : base(strucProvider)
        {
            InitializeComponent();
            tvParameters.ImageList = imageList;
            lblTime = new ToolStripStatusLabel();
            lblTime.Visible = false;
            lblTime.Alignment = ToolStripItemAlignment.Right;

            btnStop.Enabled = false;

            datTo.Value = datTo.Value.Date;
            datFrom.Value = datTo.Value;
            datFrom.Value = datFrom.Value.AddDays(-1);

            DateFormat = "dd MMMM yyyy HH:mm";
            Interval = Interval.Zero;
            Parameter = null;

            calcMessageViewControl1.strucProvider = strucProvider;

            try
            {
                //ClientSettings.Instance.Load(ClientSettings.Instance.DefaultConfigFile); // ???
                cbxColored.Checked = ClientSettings.Instance.TepIsColored; //OldClientSettings.Instance.LoadKey("Client\\TepColor"));
            }
            catch { }
        }

        public enum CalcFormMode { Calc, Lock };

        public CalcFormMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Параметр-узел, который требуется расчитать
        /// </summary>
        public UnitNode Parameter
        {
            get { return uparam; }
            set
            {
                uparam = value;
            }
        }

        /// <summary>
        /// Начало периода расчета
        /// </summary>
        public DateTime TimeBegin
        {
            get { return datFrom.Value; }
            set
            {
                datFrom.Value = value;
                if (interval != Interval.Zero)
                    datTo.Value = interval.GetNextTime(datFrom.Value);
            }
        }

        /// <summary>
        /// Конец периода расчета
        /// </summary>
        public DateTime TimeEnd
        {
            get { return datTo.Value; }
            set
            {
                datTo.Value = value;
                Interval = Interval.Zero;
            }
        }

        /// <summary>
        /// Интервал в секундах
        /// </summary>
        public Interval Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                if (interval != Interval.Zero)
                {
                    datTo.Value = interval.GetNextTime(datFrom.Value);

                    datTo.Enabled = false;
                    nextTimeButton.Enabled = true;
                    prevTimeButton.Enabled = true;
                }
                else
                {
                    datTo.Enabled = true;
                    nextTimeButton.Enabled = false;
                    prevTimeButton.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Формат отображения времени
        /// </summary>
        public string DateFormat
        {
            get { return dateFormat; }
            set
            {
                dateFormat = value;
                datTo.CustomFormat = dateFormat;
                datFrom.CustomFormat = dateFormat;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateView();

            base.OnLoad(e);
        }

        public event EventHandler CalcFinished = null;

        private async void UpdateView()
        {
            tvParameters.Nodes.Clear();
            //AsyncOperationWatcher<Object> watcher = null;
            TreeWrapp<UnitNode>[] tree = null;

            int[] ids = null;
            if (uparam != null)
            {
                ids = new int[] { uparam.Idnum };
            }
            else
            {
                if (UnitNodes != null && UnitNodes.Length > 0)
                {
                    ids = (from elem in UnitNodes
                           select elem.Idnum).ToArray();
                }
            }

            if (ids != null)
            {
                tree = await Task.Factory.StartNew(() => strucProvider.GetUnitNodeTree(ids, requiredTypes, Privileges.Execute));
            }

            if (tree != null)
            {
                tvParameters.SuspendLayout();
                btnStart.Enabled = btnCheckAll.Enabled = btnUncheckAll.Enabled == false;
                //watcher.AddValueRecivedHandler(UnitTreeReceive);
                //watcher.AddFinishHandler(OperationComplete);
                //RunWatcher(watcher);
                UnitTreeReceive(tree);
                OperationComplete();
            }
        }

        private void UnitTreeReceive(Object x)
        {
            if (InvokeRequired)
                Invoke((Action<Object>)UnitTreeReceive, x);
            else
            {
                TreeWrapp<UnitNode>[] uNodes = x as TreeWrapp<UnitNode>[];
                if (uNodes != null)
                {
                    TreeNode node = GetNode(uNodes);

                    foreach (TreeNode item in node.Nodes)
                        tvParameters.Nodes.Add(item);
                }
            }
        }

        private void OperationComplete()
        {
            if (InvokeRequired) Invoke((Action)OperationComplete);
            else
            {
                if (Interval == Interval.Zero)
                {
                    var list = ToList(tvParameters.Nodes).ToArray();

                    if (list.Length > 0)
                    {
                        var interval = (from g in list select g.Interval).Max();

                        if (interval != Interval.Zero)
                        {
                            TimeBegin = interval.NearestEarlierTime(TimeBegin);
                            TimeEnd = interval.GetNextTime(TimeBegin);
                        }
                    }
                }

                tvParameters.ResumeLayout();
                btnStart.Enabled = btnCheckAll.Enabled = btnUncheckAll.Enabled == true;
            }
        }

        private IEnumerable<ParameterGateNode> ToList(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                ParameterGateNode gateNode = node.Tag as ParameterGateNode;

                if (gateNode != null)
                {
                    yield return gateNode;
                }

                foreach (var item in ToList(node.Nodes))
                {
                    yield return item;
                }
            }
        }

        private TreeNode GetNode(IEnumerable<TreeWrapp<UnitNode>> list)
        {
            TreeNode res = new TreeNode();
            TreeNode node;
            TreeNode ch_node;

            if (list != null)
                foreach (TreeWrapp<UnitNode> unitNodeWrapp in list)
                {
                    UnitNode unitNode = unitNodeWrapp.Item;
                    if (unitNode == null) continue;
                    if (unitNode.Typ == (int)UnitTypeId.OptimizeCalc)
                    {
                        OptimizationGateNode optimizationNode = unitNode as OptimizationGateNode;

                        if (String.IsNullOrEmpty(optimizationNode.Expression))
                        {
                            bool cont = true;
                            for (int i = 0; cont && i < optimizationNode.ArgsValues.Length; i++)
                                cont = optimizationNode.ArgsValues[i].Mode == OptimizationArgumentMode.Expression
                                    && String.IsNullOrEmpty(optimizationNode.ArgsValues[i].Expression);

                            if (cont)
                                continue;
                        }
                    }
                    ch_node = new TreeNode(unitNode.GetNodeText());
                    ch_node.SelectedImageKey = ((int)unitNode.Typ).ToString();
                    ch_node.ImageKey = ((int)unitNode.Typ).ToString();
                    dicIdnum[unitNode.Idnum] = ch_node;
                    dicNodes[unitNode.Idnum] = unitNode;
                    ch_node.ToolTipText = unitNode.FullName;

                    ch_node.Checked = true;
                    ch_node.Tag = unitNode;
                    res.Nodes.Add(ch_node);
                    if (unitNode.HasChild)
                    {
                        if (ch_node == null) ch_node = res;
                        node = GetNode(unitNodeWrapp.ChildNodes);
                        foreach (TreeNode ptr in node.Nodes)
                            ch_node.Nodes.Add(ptr);
                    }
                    if (unitNode.Typ != (int)UnitTypeId.TEPTemplate) ch_node.Expand();
                }

            return res;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartCalc();
        }

        private void StartCalc()
        {
            DateTime timeStart, timeEnd;

            timeStart = datFrom.Value;
            timeEnd = datTo.Value;

            List<UnitNode> parameterList = new List<UnitNode>();
            foreach (int idnum in dicIdnum.Keys)
            {
                UnitNode node;
                if (dicIdnum[idnum].Checked
                    && (node = dicIdnum[idnum].Tag as UnitNode) != null
                    && (node.Typ == (int)UnitTypeId.TEP || node.Typ == (int)UnitTypeId.OptimizeCalc))
                {
                    parameterList.Add(node);
                }
            }
            calc_flag = true;
            btnStart.Enabled = false;
            lblTime.Visible = true;
            timer1.Enabled = true;
            btnStop.Enabled = true;
            calcMessageViewControl1.ClearMessage();
            ClearColoredTree();
            dt = DateTime.Now;

            //throw new NotImplementedException();
            curretnCalcWatcher = strucProvider.Session.BeginCalc(parameterList.ToArray(), timeStart,
               timeEnd, cbxRecalcAll.Checked);

            curretnCalcWatcher.AddValueRecivedHandler(CalcResultReceived);
            curretnCalcWatcher.AddMessageReceivedHandler(CalcMessagesReceived);
            curretnCalcWatcher.AddFinishHandler(CalcFinish);
            RunWatcher(curretnCalcWatcher, false);
        }

        public void CalcResultReceived(UAsyncResult x)
        {
            if (InvokeRequired) Invoke((Action<UAsyncResult>)CalcResultReceived, x);
            else
            {
                if (!cbxColored.Checked)
                    return;
                NodeBack[] calcResult = x.CalcNodeBack;
                if (calcResult != null)
                {
                    tvParameters.BeginUpdate();
                    foreach (NodeBack nodeCalcResult in calcResult)
                    {
                        TreeNode node;
                        if (dicIdnum.TryGetValue(nodeCalcResult.ParameterID, out node))
                        {
                            TreeNode parent = node.Parent;
                            if (nodeCalcResult.State != NodeBackState.Failed
                                && !double.IsNaN(nodeCalcResult.TimeValue.Value))
                                node.Text = String.Format("{0} ({1})", ((UnitNode)node.Tag).GetNodeText(), nodeCalcResult.TimeValue.Value);
                            switch (nodeCalcResult.State)
                            {
                                case NodeBackState.Failed:
                                    node.BackColor = Color.Red;
                                    if (parent != null)
                                    {
                                        if (parent.BackColor == Color.Empty ||
                                            parent.BackColor == Color.Red)
                                            parent.BackColor = Color.Red;
                                        else
                                            parent.BackColor = Color.LightGray;
                                    }
                                    break;
                                case NodeBackState.Middle:
                                    node.BackColor = Color.LightGray;
                                    if (parent != null)
                                    {
                                        parent.BackColor = Color.LightGray;
                                    }
                                    break;
                                case NodeBackState.Success:
                                    node.BackColor = Color.Green;
                                    if (parent != null)
                                    {
                                        if (parent.BackColor == Color.Empty ||
                                            parent.BackColor == Color.Green)
                                            parent.BackColor = Color.Green;
                                        else
                                            parent.BackColor = Color.LightGray;
                                    }
                                    break;
                                case NodeBackState.Unknown:
                                    node.BackColor = Color.Yellow;
                                    if (parent != null)
                                    {
                                        if (parent.BackColor == Color.Empty ||
                                            parent.BackColor == Color.Yellow)
                                            parent.BackColor = Color.Yellow;
                                        else
                                            parent.BackColor = Color.LightGray;
                                    }
                                    break;
                            }
                        }
                    }
                    tvParameters.EndUpdate();
                }
            }
        }

        public void CalcMessagesReceived(Message[] messages)
        {
            if (InvokeRequired) Invoke((Action)(() => CalcMessagesReceived(messages)));
            else calcMessageViewControl1.AddMessage(messages);
        }

        private void SetCalcStatusLabel()
        {
            StringBuilder builder = new StringBuilder();
            if (!calc_flag)
            {
                builder.Append("Расчет произведен за ");
                TimeSpan timeSpan = DateTime.Now.Subtract(dt);
                int elLevel = 0;
                if (timeSpan.Days > 0)
                {
                    builder.AppendFormat("{0} д ", timeSpan.Days);
                    elLevel = 4;
                }
                if (timeSpan.Hours > 0 && elLevel < 6)
                {
                    builder.AppendFormat("{0} ч ", timeSpan.Hours);
                    elLevel = 3;
                }
                if (timeSpan.Minutes > 0 && elLevel < 5)
                {
                    builder.AppendFormat("{0} м ", timeSpan.Minutes);
                    elLevel = 2;
                }
                if (timeSpan.Seconds > 0 && elLevel < 4)
                {
                    builder.AppendFormat("{0} с ", timeSpan.Seconds);
                    elLevel = 1;
                } if (timeSpan.Milliseconds > 0 && elLevel < 3)
                    builder.AppendFormat("{0} мс ", timeSpan.Milliseconds);
            }
            calcMessageViewControl1.StatusString = builder.ToString();
        }

        public void CalcFinish()
        {
            if (InvokeRequired) Invoke((Action)CalcFinish);
            else
            {
                calc_flag = false;
                timer1.Enabled = false;
                btnStop.Enabled = false;
                btnStart.Enabled = true;
                lblTime.Visible = false;
                SetCalcStatusLabel();
                try
                {
                    if (CalcFinished != null) CalcFinished(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        private void ClearColor(TreeNode node)
        {
            int i;

            if (node == null) return;

            node.BackColor = Color.Empty;

            for (i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].BackColor = Color.Empty;
                ClearColor(node.Nodes[i]);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopCalc();
        }

        private void StopCalc()
        {
            if (calc_flag && curretnCalcWatcher != null)
            {
                curretnCalcWatcher.AbortOperation();
                curretnCalcWatcher = null;
                CalcFinish();
            }
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode item in tvParameters.Nodes)
            {
                item.Checked = false;
                tvParameters_AfterCheck(this, new TreeViewEventArgs(item, TreeViewAction.ByMouse));
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode item in tvParameters.Nodes)
            {
                item.Checked = true;
                tvParameters_AfterCheck(this, new TreeViewEventArgs(item, TreeViewAction.ByMouse));
            }
        }

        protected override void BaseAsyncWorkForm_FormClosing(object sender, 
                                                              FormClosingEventArgs e)
        {
            //
            // HACK: may contain errors
            //
            DialogResult dr;

            if (!calc_flag) return;

            dr = MessageBox.Show("Запущен процесс расчета, остановить?", "Предупреждение", MessageBoxButtons.YesNoCancel);

            if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
                if (dr == DialogResult.Yes)
                {
                    StopCalc();
                }
        }

        private void tvParameters_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByKeyboard ||
                e.Action == TreeViewAction.ByMouse)
            {
                if (e.Node.Parent != null && e.Node.Checked)
                    e.Node.Parent.Checked = e.Node.Checked;
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                    tvParameters_AfterCheck(this, new TreeViewEventArgs(node, TreeViewAction.ByMouse));
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (calc_flag)
                {
                    TimeSpan ts = DateTime.Now.Subtract(dt);
                    DateTime dtime = new DateTime(ts.Ticks);
                    String format = "{0:mm:ss.fff}";
                    if (ts.Hours > 0) format = "{0:HH:mm:ss.fff}";
                    lblTime.Text = String.Format(format, dtime);
                    calcMessageViewControl1.StatusString = "(" + String.Format(format, dtime) + ") ";
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
        }
        private void cmsRelations_Opening(object sender, CancelEventArgs e)
        {
            UnitNode node;
            if (tvParameters.SelectedNode == null ||
                ((node = tvParameters.SelectedNode.Tag as UnitNode)) == null ||
                (node.Typ != (int)UnitTypeId.TEP))
                e.Cancel = true;
        }

        private void showRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnitNode node;

            if (tvParameters.SelectedNode == null ||
                (node = tvParameters.SelectedNode.Tag as UnitNode) == null)
                return;

            TepForm refForm = new TepForm(strucProvider, Program.MainForm.Icons);
            Program.MainForm.AddExtendForm(refForm);
            refForm.Node = node;
            refForm.Relation = FormulaRelation.Reference;
            refForm.CurrentTime = datFrom.Value;
            refForm.TopMost = true;
            //refForm.ShowDialog();
            refForm.Show();
        }

        private void showDepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnitNode node;

            if (tvParameters.SelectedNode == null ||
                (node = tvParameters.SelectedNode.Tag as UnitNode) == null)
                return;

            TepForm refForm = new TepForm(strucProvider, Program.MainForm.Icons);
            Program.MainForm.AddExtendForm(refForm);
            refForm.Node = node;
            refForm.Relation = FormulaRelation.Dependence;
            refForm.CurrentTime = datFrom.Value;
            refForm.TopMost = true;
            //refForm.ShowDialog();
            refForm.Show();
        }

        private void showParameterValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterNode parameterNode;

            if (tvParameters.SelectedNode == null ||
                (parameterNode = tvParameters.SelectedNode.Tag as ParameterNode) == null)
                return;

            ParameterValuesController valuesController = new ParameterValuesController(strucProvider,
                parameterNode as ParameterNode, datFrom.Value);
            ParameterValuesEditorForm parameterValuesForm = new ParameterValuesEditorForm(strucProvider, valuesController);
            Program.MainForm.AddExtendForm(parameterValuesForm);
            valuesController.AsyncForm = parameterValuesForm;

            parameterValuesForm.ShowDialog();
        }

        private void nextTimeButton_Click(object sender, EventArgs e)
        {
            if (Interval != Interval.Zero)
                TimeBegin = Interval.GetNextTime(TimeBegin);
        }

        private void prevTimeButton_Click(object sender, EventArgs e)
        {
            if (Interval != Interval.Zero)
                TimeBegin = Interval.GetPrevTime(TimeBegin);
        }

        private void cbxColored_CheckedChanged(object sender, EventArgs e)
        {
            ClientSettings.Instance.TepIsColored = cbxColored.Checked;
            ClientSettings.Instance.Save();//ClientSettings.Instance.DefaultConfigFile);
        }

        private void datFrom_ValueChanged(object sender, EventArgs e)
        {
            TimeBegin = datFrom.Value;
        }

        public override object InitializeLifetimeService()
        {
            System.Runtime.Remoting.Lifetime.ILease lease =
                (System.Runtime.Remoting.Lifetime.ILease)base.InitializeLifetimeService();
            lease.InitialLeaseTime = TimeSpan.Zero;

            return lease;
        }

        private void goToParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node;
            CalcMessage requiredMessage = calcMessageViewControl1.CurrentMessage as CalcMessage;
            if (requiredMessage != null)
            {
                UnitNode unitNode = GetNodeNameByID(requiredMessage.Position.NodeID);
                if (unitNode != null &&
                    dicIdnum.TryGetValue(unitNode.Idnum, out node) &&
                    node != null)
                {
                    tvParameters.SelectedNode = node;
                    tvParameters.Select();
                }
            }
        }

        private void ClearColoredTree()
        {
            if (tvParameters.Nodes.Count > 0)
            {
                tvParameters.BeginUpdate();
                foreach (TreeNode item in tvParameters.Nodes)
                {
                    ClearColor(item);
                }
                tvParameters.EndUpdate();
            }
        }

        protected override void ShowStatusStripAsyncView()
        {
            if (InvokeRequired) Invoke((Action)ShowStatusStripAsyncView);
            else
            {
                base.ShowStatusStripAsyncView();
                toolStripSplitButtonAbort.Visible = toolStripSplitButtonAbort.Visible && !calc_flag;
            }
        }

        private UnitNode GetNodeNameByID(int nodeID)
        {
            if (nodeID >= 0)
            {
                UnitNode unitNode;
                if (dicNodes.TryGetValue(nodeID, out unitNode))
                    return unitNode;
                else
                {
                    try
                    {
                        unitNode = strucProvider.GetUnitNode(nodeID);
                        dicNodes[nodeID] = unitNode;
                        return unitNode;
                    }
                    catch { }
                }
            }
            return null;
        }
    }
}
