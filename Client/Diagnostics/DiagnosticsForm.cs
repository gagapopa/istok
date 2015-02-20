using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using COTES.ISTOK.ASC;
using COTES.ISTOK.DiagnosticsInfo;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    public partial class DiagnosticsForm : COTES.ISTOK.Client.BaseAsyncWorkForm
    {
        //ToolTip toolTip = new ToolTip();
        Thread threadUpdater = null;
        LogForm frmLogger;
        AdminUISettings settings = null;
        volatile List<TreeNode> lstMainNodes = new List<TreeNode>();

        const string strStateDataError = "blue";
        const string strStateOnline = "green";
        const string strStateOffline = "red";
        const string strStateDisabled = "gray";

        Icon icoBad;
        Icon icoGood;
        Icon icoRed;
        Icon icoGray;
        Icon icoBlank;

        MessageManager messageManager = null;

        public DiagnosticsForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();

            icoBad = Properties.Resources.ledred;
            icoGood = Properties.Resources.ledgreen;
            icoBlank = Properties.Resources.blank;
            icoRed = Properties.Resources.ledblue;
            icoGray = Properties.Resources.ledgray;

            messageManager = new MessageManager();
            settings = new AdminUISettings();

            frmLogger = new LogForm();
            frmLogger.MessageManager = messageManager;

            //CommonData.InitFileLog("admin_UI.log");

            threadUpdater = new Thread(new ThreadStart(UpdateMethod));
            threadUpdater.Name = "Updater";
        }

        private void JijoperWe(String text)
        {
            this.Text = text;
        }

        private void UpdateChildNode(TreeNode node)
        {
            try
            {
                SetNodeState(node);
                if (node.Nodes.Count > 0)
                {
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        try
                        {
                            UpdateChildNode(node.Nodes[i]);
                        }
                        catch (ThreadAbortException) { throw; }
                        catch { }
                    }
                }
            }
            catch (ThreadAbortException) { throw; }
            catch { }
        }

        private void UpdateMethod()
        {
            try
            {
                while (true)
                {
                    Invoke((Action<String>)JijoperWe, "Диагностика +");
                    for (int i = 0; i < tvNodes.Nodes.Count; i++)
                        UpdateChildNode(tvNodes.Nodes[i]);
                    if (InvokeRequired) Invoke((Action<String>)JijoperWe, "Диагностика");
                    Thread.Sleep(settings.UpdateInterval);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex) {MessageBox.Show("admin_UI: Update thread aborted abnormally (" + ex.Message + ")"); }

            if (InvokeRequired) BeginInvoke((Action<String>)JijoperWe, "Диагностика [X]");
        }

        /// <summary>
        /// Установить состояния узла
        /// </summary>
        /// <param name="node"></param>
        private void SetNodeState(TreeNode node)
        {
            IDiagnostics diag = null;
            NamedNodeList nodeList = null;
            bool dataError = false;

            try
            {
                if ((nodeList = node.Tag as NamedNodeList) != null) diag = nodeList.Diagnostics;

                if (nodeList.State == DiagState.Bad || nodeList.State == DiagState.BadNotComitted)
                {
                    if (node.Parent != null && node.Parent.Tag is NamedNodeList)
                    {
                        DiagState state = nodeList.State;
                        IDiagnostics pdiag = ((NamedNodeList)node.Parent.Tag).Diagnostics;
                        if (pdiag != null)
                        {
                            nodeList = new NamedNodeList(pdiag.GetBlockDiagnostics(nodeList.ID), nodeList.ID);
                            nodeList.State = state;
                            node.Tag = nodeList;
                            if (node.TreeView != null && node.TreeView.InvokeRequired)
                                node.TreeView.Invoke((Action)(() => node.Text = nodeList.Text));
                            else
                                node.Text = nodeList.Text;
                            diag = nodeList.Diagnostics;
                        }
                    }
                }
                //if (diag != null)
                {
                    TestConnection<Object>.Test(diag, null, settings.TimeoutNorm, true);
                    if (diag.GetText() != node.Text)
                    {
                        if (node.TreeView != null && node.TreeView.InvokeRequired)
                            node.TreeView.Invoke((Action)(() => node.Text = diag.GetText()));
                        else
                            node.Text = diag.GetText();
                    }

                    if (diag.CanManageChannels())
                    {
                        var channel_id = diag.GetChannels();
                        for (int i = 0; i < channel_id.Length; i++)
                            if ((diag.GetChannelState(channel_id[i]) & ChannelStatus.HasErrors) != 0)
                            {
                                dataError = true;
                                //if (tvNodes.InvokeRequired) tvNodes.Invoke((Action<TreeNode>)SetNodeDataError, node);
                                //else 
                                    SetNodeDataError(node);
                                break;
                            }
                    }
                }
                if (!dataError)
                {
                    //if (tvNodes.InvokeRequired) tvNodes.Invoke((Action<TreeNode>)SetNodeOnline, node);
                    //else 
                        SetNodeOnline(node);
                }
            }
            catch (Exception)
            {
                //if (tvNodes.InvokeRequired) tvNodes.Invoke((Action<TreeNode>)SetNodeOffline, node);
                //else
                    SetNodeOffline(node);
            }
        }
        private void SetNodeOffline(TreeNode node)
        {
            NamedNodeList nodeList = null;
            if (node == null) return;

            if (IsSelected(node))
                UpdateData(null, true);

            if ((nodeList = node.Tag as NamedNodeList) != null &&
                (nodeList.State != DiagState.Bad))
            {
                messageManager.ChangeNodeState(node, InfoMessageState.NodeOffline);
                if (node.TreeView != null && node.TreeView.InvokeRequired)
                    node.TreeView.Invoke((Action<TreeNode, string>)SetNodeState, node, strStateOffline);
                else
                    SetNodeState(node, strStateOffline);
                nodeList.State = DiagState.Bad;
                nodeList.DiagData = null;
                //if (node.IsSelected) UpdateData(node);
            }
            foreach (TreeNode item in node.Nodes)
                SetNodeState(item);
        }
        private void SetNodeDataError(TreeNode node)
        {
            NamedNodeList nodeList = null;
            if (node == null) return;

            if ((nodeList = node.Tag as NamedNodeList) != null)
            {
                messageManager.ChangeNodeState(node, InfoMessageState.NodeDataWarning);
                nodeList.State = DiagState.Bad;
                if (node.TreeView != null && node.TreeView.InvokeRequired)
                    node.TreeView.Invoke((Action<TreeNode, string>)SetNodeState, node, strStateDataError);
                else
                    SetNodeState(node, strStateDataError);
                UpdateData(node, false);
            }
        }
        private void SetNodeDisabled(TreeNode node)
        {
            NamedNodeList nodeList = null;
            if (node == null) return;

            if ((nodeList = node.Tag as NamedNodeList) != null)
            {
                nodeList.Disabled = true;
                node.Nodes.Clear();
                messageManager.ChangeNodeState(node, InfoMessageState.NodeDisabled);
                if (node.TreeView != null && node.TreeView.InvokeRequired)
                    node.TreeView.Invoke((Action<TreeNode, string>)SetNodeState, node, strStateDisabled);
                else
                    SetNodeState(node, strStateDisabled);
            }
        }
        private void SetNodeOnline(TreeNode node)
        {
            NamedNodeList namedList = null;

            if (node == null) return;

            if ((namedList = node.Tag as NamedNodeList) != null && namedList.Diagnostics != null)
            {
                try
                {
                    if (namedList.State != DiagState.Good)
                        messageManager.ChangeNodeState(node, InfoMessageState.NodeOnline);
                    if (node.TreeView != null && node.TreeView.InvokeRequired)
                        node.TreeView.Invoke((Action<TreeNode, string>)SetNodeState, node, strStateOnline);
                    else
                        SetNodeState(node, strStateOnline);
                    namedList.State = DiagState.Good;
                    if (settings.UpdateData)
                        //if (IsSelected(node))
                        UpdateData(node, true);
                }
                catch (ThreadAbortException) { throw; }
                catch
                {
                    SetNodeOffline(node);
                }
            }
        }
        private void SetNodeState(TreeNode node, string key)
        {
            if (node.ImageKey != key)
                node.ImageKey = key;
            if (node.SelectedImageKey != key)
                node.SelectedImageKey = key;
        }

        private bool IsSelected(TreeNode node)
        {
            bool res = false;
            if (node.TreeView != null && node.TreeView.InvokeRequired)
                node.TreeView.Invoke((Action)(() => res = node.IsSelected));
            else
                res = node.IsSelected;
            return res;
        }

        private  void UpdateData()
        {
            try
            {
                TreeNode node;
                IDiagnostics diag = strucProvider.Session.GetDiagnosticsObject();
                //Diagnostics diag = await Task.Factory.StartNew(() => strucProvider.Session.GetDiagnosticsObject());

                node = CreateDiagNode(0, diag);

                tvNodes.BeginUpdate();
                tvNodes.Nodes.Clear();
                tvNodes.Nodes.Add(node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                tvNodes.EndUpdate();
            }
        }
        private void UpdateData(TreeNode node, bool show)
        {
            if (node != null && node.Tag is NamedNodeList)
            {
                try
                {
                    NamedNodeList nodeList = (NamedNodeList)node.Tag;
                    IDiagnostics diag;
                    try
                    {
                        if (nodeList.Diagnostics == null)
                        {
                            if (node.Parent != null && node.Parent.Tag is NamedNodeList)
                            {
                                NamedNodeList pnodeList = (NamedNodeList)node.Parent.Tag;
                                if (pnodeList.Diagnostics != null)
                                    nodeList.Diagnostics = pnodeList.Diagnostics.GetBlockDiagnostics(nodeList.ID);
                            }
                        }
                        diag = nodeList.Diagnostics;
                        string tmp = diag.GetText();
                        if (nodeList.Text != tmp)
                        {
                            nodeList.Text = tmp;
                            if (node.TreeView != null && node.TreeView.InvokeRequired)
                                node.TreeView.Invoke((Action)(() => node.Text = tmp));
                            else
                                node.Text = nodeList.Text;
                        }
                        nodeList.DiagData = diag.GetAllInfo();
                        if (show) UpdateNode(node);
                    }
                    catch (SocketException) { }
                }
                catch (ThreadInterruptedException) { }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    String message = ex.Message;
                    message += String.Format("\nTargetSite: {0}", ex.TargetSite);
                    message += String.Format("\nStackTrace: {0}", ex.StackTrace);
                    MessageBox.Show(message);
                }
            }
            else
            {
                UpdateTabs(new TabPage[0]);
                //tabControl.Controls.Clear();
            }
        }

        private void UpdateNode(TreeNode node)
        {
            if (node != null && node.Tag is NamedNodeList && settings.UpdateTabPage && IsSelected(node))
            {
                NamedNodeList nodeList = (NamedNodeList)node.Tag;
                List<TabPage> pageList = new List<TabPage>();
                DataGridView dgv;
                TabPage tp;

                if (nodeList.DiagData != null)
                {
                    foreach (DataTable item in nodeList.DiagData.Tables)
                    {
                        tp = new TabPage();
                        dgv = new DataGridView();
                        dgv.Dock = DockStyle.Fill;
                        dgv.ReadOnly = true;
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgv.AllowUserToAddRows = false;
                        dgv.MultiSelect = false;
                        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        dgv.DataSource = item;
                        tp.Controls.Add(dgv);
                        tp.Text = item.TableName;
                        pageList.Add(tp);
                    }

                    //if (node.TreeView != null && node.TreeView.InvokeRequired)
                    //    node.TreeView.Invoke((Action<TabPage[]>)UpdateTabs, new object[] { pageList.ToArray() });
                    //else
                    UpdateTabs(pageList.ToArray());
                }
                else
                    UpdateTabs(new TabPage[0]);
            }
        }

        private void UpdateTabs(TabPage[] tabPages)
        {
            if (tabControl.InvokeRequired)
                tabControl.Invoke((Action<TabPage[]>)UpdateTabs, new object[] { tabPages });
            else
            {
                List<TabPage> lstPages = new List<TabPage>();
                bool found;

                foreach (TabPage item in tabControl.TabPages)
                {
                    found = false;
                    foreach (var tab in tabPages)
                    {
                        if (tab.Text == item.Text)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) lstPages.Add(item);
                }
                foreach (var item in lstPages)
                    tabControl.TabPages.Remove(item);

                foreach (var tab in tabPages)
                {
                    found = false;
                    foreach (TabPage item in tabControl.TabPages)
                    {
                        if (tab.Text == item.Text)
                        {
                            found = true;
                            if (item.Controls.Count == 1 && item.Controls[0] is DataGridView &&
                                tab.Controls.Count == 1 && tab.Controls[0] is DataGridView)
                            {
                                DataGridView dgv1 = (DataGridView)item.Controls[0];
                                DataGridView dgv2 = (DataGridView)tab.Controls[0];

                                int cc = 0, cr = 0;
                                if (dgv1.CurrentCell != null)
                                {
                                    cc = dgv1.CurrentCell.ColumnIndex;
                                    cr = dgv1.CurrentCell.RowIndex;
                                }
                                dgv1.DataSource = dgv2.DataSource;
                                if (dgv1.Rows.Count > cr && dgv1.Columns.Count > cc)
                                    dgv1.CurrentCell = dgv1.Rows[cr].Cells[cc];
                            }
                            else
                            {
                                List<Control> lstControls = new List<Control>();
                                while (tab.Controls.Count > 0)
                                {
                                    lstControls.Add(tab.Controls[0]);
                                    tab.Controls.RemoveAt(0);
                                }
                                item.Controls.Clear();
                                foreach (var ctrl in lstControls)
                                    item.Controls.Add(ctrl);
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        tabControl.TabPages.Add(tab);
                    }
                }
                //tabControl.TabPages.Clear();
                //tabControl.TabPages.AddRange(tabPages);
                tabControl.Update();
            }
        }

        private TreeNode CreateDiagNode(int idnum, IDiagnostics diag)
        {
            TreeNode node = new TreeNode();
            NamedNodeList nodeList = new NamedNodeList(diag, idnum);
            
            node.Tag = nodeList;
            node.Text = nodeList.Text;
            if (diag != null)
            {
                if (TestConnection<Object>.Test(diag, null) && diag.CanManageBlocks())
                {
                    int[] ids = diag.GetBlocks();
                    foreach (var item in ids)
                    {
                        //node.Nodes.Add(CreateDiagNode(item, null));
                        IDiagnostics blockDiag = diag.GetBlockDiagnostics(item);
                        node.Nodes.Add(CreateDiagNode(item, blockDiag));
                    }
                }
            }

            return node;
        }

        private void tvNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //UpdateData(tvNodes.SelectedNode);
            UpdateNode(tvNodes.SelectedNode);
        }

        private void DiagnosticsForm_Load(object sender, EventArgs e)
        {
            tabControl.Controls.Clear();
            UpdateData();
            if (threadUpdater != null) threadUpdater.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DiagState state;

            messageManager.ClearNodes(tvNodes.Nodes);
            state = messageManager.GetCurrentState();

            if (threadUpdater == null || !threadUpdater.IsAlive)
            {
                nIcon.Icon = icoRed;
                return;
            }

            switch (state)
            {
                case DiagState.Bad:
                    if (nIcon.Icon != icoBad)
                        nIcon.Icon = icoBad;
                    break;
                case DiagState.BadNotComitted:
                    if (nIcon.Icon == icoBad)
                        nIcon.Icon = icoBlank;
                    else
                        nIcon.Icon = icoBad;
                    break;
                case DiagState.Good:
                    if (nIcon.Icon != icoGood)
                        nIcon.Icon = icoGood;
                    break;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm frmProp = new SettingsForm();

            frmProp.Object = settings.Clone();
            frmProp.ShowDialog();

            if (frmProp.DialogResult == DialogResult.OK)
            {
                settings = frmProp.Object as AdminUISettings;
#if DEBUG
                MessageBox.Show("Нужно сохранение настроек!");
#endif
                //ClientSettings.Instance.SaveObject("admin_UI/Settings", settings);
                //ClientSettings.Instance.Save();
            }
        }

        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMessages();
        }

        private void ShowMessages()
        {
            if (frmLogger != null && !frmLogger.IsDisposed)
            {
                frmLogger.Show();
                if (!frmLogger.Focused) frmLogger.Activate();
            }
        }

        private void nIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowMessages();
        }

        private void DiagnosticsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (threadUpdater != null)
                threadUpdater.Abort();
        }
    }

    /// <summary>
    /// Перечисление состояний иконки трейа
    /// </summary>
    public enum DiagState
    {
        /// <summary>
        /// Все хорошо
        /// </summary>
        Good,
        /// <summary>
        /// Что-то плохо
        /// </summary>
        Bad,
        /// <summary>
        /// Что-то плохо, а никто не видел
        /// </summary>
        BadNotComitted,
        Disabled
    }

    //переименовать класс (название кривое)
    public class NamedNodeList
    {
        private string m_name;
        //private string m_url;
        private IDiagnostics diagNode = null;
        //private ServiceController svcNode = null;
        //private DiagServerInfo serverInfo = null;
        private Object properties = null;
        
        private DiagState state;
        //private WindowsImpersonationContext context = null;

        public DataSet DiagData { get; set; }

        public NamedNodeList(/*string url,*/ IDiagnostics diag/*, ServiceController service*/)
        {
            ID = 0;
            m_name = diag.GetText();
            try
            {
                //if (diag != null) m_name = diag.Text;
            }
            catch (SocketException) { }
            //m_url = url;
            diagNode = diag;
            //svcNode = service;
            state = diagNode == null ? DiagState.Bad : DiagState.Good;
        }

        public NamedNodeList(IDiagnostics diag, /*ServiceController service,*/ int idnum)
            : this(diag/*, service*/)
        {
            ID = idnum;
        }

        public int ID { get; protected set; }

        public string Text
        {
            get { return m_name; }
            set { m_name = value; }
        }

        //public WindowsImpersonationContext Context
        //{
        //    get { return context; }
        //    set { context = value; }
        //}

        //public string URL
        //{
        //    get { return m_url; }
        //}

        public bool Disabled { get; set; }

        //public Object Properties
        //{
        //    get { return properties; }
        //    set
        //    {
        //        properties = value;
        //        if (properties is ServerNode) m_name = ((ServerNode)properties).Text;
        //    }
        //}

        public IDiagnostics Diagnostics
        {
            get { return diagNode; }
            set { diagNode = value; }
        }

        //public ServiceController Service
        //{
        //    get { return svcNode; }
        //    set { svcNode = value; }
        //}

        public void ClearDiagnostics()
        {
            diagNode = null;
        }
        public DiagState State
        {
            get { return state; }
            set { state = value; }
        }
        //public DiagServerInfo ServerInfo
        //{
        //    get { return serverInfo; }
        //    set { serverInfo = value; }
        //}

        //public void Reconnect()
        //{
        //    diagNode = (Diagnostics)Activator.GetObject(typeof(Diagnostics), m_url);
        //}
    }
}
