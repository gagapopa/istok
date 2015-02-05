using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client.Extension;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class UniForm : TreeForm, IUniForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        public const String calcFormCaption = "Расчеты";

        public bool CalcForm { get { return Text.Equals(calcFormCaption); } }

        private TreeNode selectedNode = null;

        ParamSearchForm frmSearch = null;

        Type selectedTabControl = null;

        /// <summary>
        /// Указывает, должна ли первая вкладка быть свойствами
        /// </summary>
        public bool IsPropertiesFirst
        {
            get { return Filter == null || Filter.IsEmpty; }
        }
        protected bool isPropertiesFirst = true;

        /// <summary>
        /// Устанавливает видимость дерева элементов
        /// </summary>
        public bool IsTreeShown
        {
            get { return !splitContainer1.Panel1Collapsed; }
            set { splitContainer1.Panel1Collapsed = !value; }
        }

        public UniForm()
            : base()
        {
            Init(null);
        }

        public UniForm(StructureProvider strucProvider, ImageList icons)
            : base(strucProvider)
        {
            Init(icons);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                ShowMenu();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка загрузки окна.", ex);
                ShowError(ex);
            }
        }

        private void Init(ImageList icons)
        {
            InitializeComponent();
            treeViewUnitObjects.ImageList = icons;
        }

        private void ShowMenu()
        {
            UTypeNode[] types = strucProvider.Session.Types;
            var typeIds = (from elem in types
                           select (UnitTypeId)elem.Idnum).ToArray();

            tsbGraphs.Visible = typeIds != null && (typeIds.Contains(UnitTypeId.Graph) || typeIds.Contains(UnitTypeId.Histogram));
            tsbSchemas.Visible = typeIds != null && typeIds.Contains(UnitTypeId.Schema);
            tsbMonitors.Visible = typeIds != null && typeIds.Contains(UnitTypeId.MonitorTable);
            tsbManuals.Visible = typeIds != null && typeIds.Contains(UnitTypeId.ManualGate);
            tsbReports.Visible = typeIds != null && (typeIds.Contains(UnitTypeId.ExcelReport) || typeIds.Contains(UnitTypeId.Report));
            tsbCalcs.Visible = typeIds != null && typeIds.Contains(UnitTypeId.TEPTemplate);
        }

        #region Рисование боковой панели (компонент)
        /// <summary>
        /// Рисует контрол указанного нода, либо набор контролов потомков (если указанный нод - папка, например)
        /// </summary>
        /// <param name="node">Нод</param>
        private void ShowPanel(UnitNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            try
            {
                UnitProvider up = null;
                up = strucProvider.GetUnitProvider(node);
                up.NodeSaved -= up_NodeSaved;
                up.NodeSaved += up_NodeSaved;

                if (up != null)
                {
                    List<BaseUnitControl> controls = new List<BaseUnitControl>();
                    Control control;

                    var types = (strucProvider.Session.Types.Select(elem => elem.Idnum)).ToArray();
                    List<int> lstTypes = new List<int>(Filter.GetTypes());

                    if (lstTypes.Count == 0)
                        lstTypes.AddRange(types);
                    else
                        for (int i = 0; i < lstTypes.Count; i++)
                            if (!types.Contains(lstTypes[i]) && lstTypes.Contains(types[i]))
                            { lstTypes.Remove(types[i]); i--; }

                    var c = UnitProviders.CreateControls(up, this);
                    if (c != null) controls.AddRange(c);

                    //TODO: WTF???

                    //debug
                    bool clean;
                    if (true)
                    {
                        //splitContainer1.Panel2.Controls.Clear();
                        ClearPanel();
                        clean = true;
                    }
                    else
                        clean = false;

                    if (controls != null && controls.Count > 0)
                    {
                        TabControl tab = null;
                        foreach (Control item in splitContainer1.Panel2.Controls)
                        {
                            if (item is TabControl)
                            {
                                tab = (TabControl)item;
                                break;
                            }
                        }
                        if (tab == null)
                        {
                            if (!clean) splitContainer1.Panel2.Controls.Clear();
                            tab = new TabControl();

                            tab.SelectedIndexChanged += new EventHandler(tab_SelectedIndexChanged);
                        }
                        tab.ImageList = Program.MainForm.Icons;
                        while (tab.TabPages.Count > 1) tab.TabPages.RemoveAt(1);
                        if (IsPropertiesFirst)
                        {
                            AddCommonControl(tab, up, true);
                            AddCustomControls(tab, controls.ToArray(), false);
                        }
                        else
                        {
                            AddCustomControls(tab, controls.ToArray(), true);
                            AddCommonControl(tab, up, false);
                        }

                        control = tab;
                    }
                    else
                    {
                        control = null;
                        for (int i = 0; i < splitContainer1.Panel2.Controls.Count; i++)
                        {
                            if (splitContainer1.Panel2.Controls[i] is CommonUnitControl && control == null)
                                control = splitContainer1.Panel2.Controls[i];
                            else
                            {
                                splitContainer1.Panel2.Controls.RemoveAt(i);
                                i--;
                            }
                        }

                        if (control == null)
                        {
                            control = new CommonUnitControl(up)
                            {
                                UniForm = this
                            };
                            ((CommonUnitControl)control).InitiateProcess();
                        }
                        else
                            ((CommonUnitControl)control).UnitProvider = up;
                    }

                    control.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Add(control);
                }
                else
                {

                }
                AfterShowPanelHack();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка отображения элемента.", ex);
                ShowError(ex);
            }
        }

        void up_NodeSaved(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired) Invoke((EventHandler)up_NodeSaved, sender, e);
                else
                {
                    UnitTreeNode unitTreeNode = treeViewUnitObjects.SelectedNode as UnitTreeNode;

                    UpdateTreeNode(unitTreeNode);
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка отображения элемента.", ex);
                ShowError(ex);
            }
        }

        UnitNode parameterToShowInTab;

        /// <summary>
        /// Вызвать костыли после отображения правой панели
        /// </summary>
        private void AfterShowPanelHack()
        {
            if (parameterToShowInTab != null)
            {
                ShowTabControl(parameterToShowInTab);
                parameterToShowInTab = null;
            }
        }

        /// <summary>
        /// Показать параметр во вкладочке
        /// </summary>
        /// <param name="node"></param>
        private void ShowTabControl(UnitNode node)
        {
            UnitTreeNode unitTreeNode = treeViewUnitObjects.SelectedNode as UnitTreeNode;
            UnitNode selectedNode;

            if (unitTreeNode != null && unitTreeNode.Node != null)
            {
                selectedNode = unitTreeNode.Node;
                TabControl tab = null;
                foreach (var item in splitContainer1.Panel2.Controls)
                {
                    if ((tab = item as TabControl) != null)
                        break;
                }

                // ищем вкладочку, которая согласиться показать параметр
                if (tab != null)
                {
                    TabPage pageToSelect = null;
                    BaseUnitControl selectedUnitControl = null;

                    foreach (TabPage page in tab.TabPages)
                    {
                        foreach (var item in page.Controls)
                        {
                            BaseUnitControl unitControl = item as BaseUnitControl;

                            if (unitControl != null && unitControl.ForShow(node))
                            {
                                pageToSelect = page;
                                selectedUnitControl = unitControl;
                            }
                        }
                    }

                    if (pageToSelect != null)
                        tab.SelectedTab = pageToSelect;
                    if (selectedUnitControl != null)
                        selectedUnitControl.SelectNode(node);
                }
            }

        }

        void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TabPage tp = (sender as TabControl).SelectedTab;
                UnitProvider up = null;
                //if (treeViewUnitObjects.SelectedNode != null && treeViewUnitObjects.SelectedNode.Tag is int)
                //{
                //    int nodeID = (int)treeViewUnitObjects.SelectedNode.Tag;
                //    //up = strucProvider.GetUnitProvider(await Task.Factory.StartNew(() => strucProvider.GetUnitNode(nodeID)));
                //    up = strucProvider.GetUnitProvider(strucProvider.GetUnitNode(nodeID));
                //}
                up = strucProvider.GetUnitProvider(strucProvider.CurrentNode);
                if (tp.Controls.Count > 0)
                {
                    BaseUnitControl buc = tp.Controls[0] as BaseUnitControl;
                    if (buc != null && up != null)
                    {
                        selectedTabControl = tp.Controls[0].GetType();
                        buc.InitiateProcess();
                    }
                    else
                        selectedTabControl = null;
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка ошибка отображения вкладки.", ex);
                ShowError(ex);
            }
        }

        /// <summary>
        /// Добавляет вкладку со свойствами
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="up"></param>
        /// <param name="first">Указывает, первый ли контрол (остальные удаляет)</param>
        private void AddCommonControl(TabControl tab, UnitProvider up, bool first)
        {
            BaseUnitControl ctrl = null;
            TabPage pg = null;
            bool pg_found = false, ctrl_found = false;
            if (first)
            {
                if (tab.TabPages.Count > 0 && tab.TabPages[0].Controls.Count > 0)
                {
                    pg = tab.TabPages[0];
                    pg_found = true;
                    if (pg.Controls[0] is CommonUnitControl)
                    {
                        ctrl = (CommonUnitControl)pg.Controls[0];
                        ctrl_found = true;
                    }
                    else
                        pg.Controls.Clear();
                }
                else
                    tab.TabPages.Clear();
            }
            if (pg == null) pg = new TabPage();
            if (ctrl == null) ctrl = new CommonUnitControl(up) { UniForm = this };
            if (first) ctrl.InitiateProcess();

            pg.Text = ((CommonUnitControl)ctrl).Text;
            ctrl.Dock = DockStyle.Fill;
            if (!ctrl_found) pg.Controls.Add(ctrl);
            if (!pg_found) tab.TabPages.Add(pg);
            if (ctrl.GetType() == selectedTabControl)
                tab.SelectedTab = pg;
        }
        /// <summary>
        /// Добавляет вкладки с контролами узлов
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="controls"></param>
        /// <param name="first">Указывает, первый ли контрол (остальные удаляет)</param>
        private void AddCustomControls(TabControl tab, BaseUnitControl[] controls, bool first)
        {
            BaseUnitControl ctrl = null;
            //TabPage pg;
            TabPage pg = null;
            bool pg_found = false, ctrl_found = false;
            bool tab_selected = false;
            if (controls.Length > 0 && first)
            {
                ctrl = controls[0];
                if (tab.TabPages.Count > 0 && tab.TabPages[0].Controls.Count > 0)
                {
                    pg = tab.TabPages[0];
                    pg_found = true;
                    if (pg.Controls[0].GetType() == ctrl.GetType())
                    {
                        ctrl = (BaseUnitControl)pg.Controls[0];
                        ctrl_found = true;
                    }
                    else
                        pg.Controls.Clear();
                }
                else
                    tab.TabPages.Clear();
                ctrl.InitiateProcess();
            }

            foreach (var item in controls)
            {
                if (!pg_found) pg = new TabPage();
                if (!ctrl_found) ctrl = item;
                else ctrl.UnitProvider = item.UnitProvider;
                pg.Text = item.Text;
                ctrl.Dock = DockStyle.Fill;
                if (!ctrl_found) pg.Controls.Add(ctrl);
                if (!pg_found) tab.TabPages.Add(pg);
                if (!tab_selected && selectedTabControl != null && ctrl.GetType() == selectedTabControl)
                {
                    tab.SelectedTab = pg;
                    tab_selected = true;
                }
                pg.ImageKey = ((int)item.Typ).ToString();
                pg_found = false;
                ctrl_found = false;
            }
        }

        /// <summary>
        /// Опустошает или заполняет чем-то по умолчанию область отображения контрола нода.
        /// </summary>
        private void ClearPanel()
        {
            foreach (Control item in splitContainer1.Panel2.Controls)
                if (item != null) item.Dispose();
            splitContainer1.Panel2.Controls.Clear();
        }
        #endregion

        private void treeViewUnitObjects_OnDeselect(object sender, EventArgs e)
        {
            try
            {
                selectionTimer.Stop();
                selectionTimer.Enabled = false;
                selectionTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                log.WarnException("", ex);
                ShowError(ex);
            }
        }

        private void treeViewUnitObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Action == TreeViewAction.ByKeyboard)
                {
                    selectionTimer.Stop();
                    selectionTimer.Enabled = false;
                    selectionTimer.Enabled = true;
                }
                else
                    selectionTimer_Tick(selectionTimer, e);
                if (treeViewUnitObjects.SelectedNode.Tag != null)
                    strucProvider.CurrentNode = strucProvider.GetUnitNode((int)treeViewUnitObjects.SelectedNode.Tag);
                else
                    strucProvider.CurrentNode = null;
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка отображение элемента.", ex);
                //Task.Factory.StartNew(() => ShowError(ex));
                ShowError(ex);
            }
        }

        /// <summary>
        /// Выводит сообщение о необходимости сохранения изменений текущего узла,
        /// если таковые имеются.
        /// </summary>
        /// <returns>true - если изменения сохранены или отсутствуют, иначе (отмена действия) - false</returns>
        private async Task<bool> SaveCurrentNodeChanges()
        {
            if (treeViewUnitObjects.SelectedNode != null)
            {
                if (treeViewUnitObjects.SelectedNode.Tag is int)
                {
                    UnitProvider up = null;
                    //int unitNodeID = (int)treeViewUnitObjects.SelectedNode.Tag;
                    //up = strucProvider.GetUnitProvider(strucProvider.GetUnitNode(unitNodeID));
                    up = strucProvider.GetUnitProvider(strucProvider.CurrentNode);
                    if (up.HasChanges)
                    {
                        DialogResult dres = MessageBox.Show("Редактируемый узел изменился. Сохранить изменения?", "Редактирование узла", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (dres == DialogResult.Yes)
                        {
                            var args = new SaveActionArgs(up, true);
                            var action = Program.MainForm.WorkflowSelector.GetWorkflow(args);
                            return ((SaveActionArgs)await action.Do(args)).Cancel;
                        }
                        else
                            if (dres == DialogResult.No)
                                up.ClearUnsavedData();
                            else
                                if (dres == DialogResult.Cancel)
                                    return false;
                    }
                    up.ClearUnsavedData();
                }
            }
            return true;
        }
        /// <summary>
        /// Выводит сообщение о необходимости сохранения изменений текущего узла,
        /// если таковые имеются.
        /// </summary>
        /// <returns>false - если изменения сохранены или отсутствуют, иначе (отмена действия) - true</returns>
        private bool CurrentNodeChanged()
        {
            if (treeViewUnitObjects.SelectedNode != null)
            {
                if (treeViewUnitObjects.SelectedNode.Tag is int)
                {
                    UnitProvider up = null;
                    //int unitNodeID = (int)treeViewUnitObjects.SelectedNode.Tag;
                    //var node = strucProvider.GetUnitNode(unitNodeID);
                    var node = strucProvider.CurrentNode;
                    if (node != null)
                    {
                        up = strucProvider.GetUnitProvider(node);
                        if (up != null && up.HasChanges)
                        {
                            DialogResult dres = MessageBox.Show("Редактируемый узел изменился. Продолжение приведет к потере изменений. Продолжить?",
                                "Редактирование узла",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question);
                            //if (dres == DialogResult.OK)
                            //{
                            //    up.ClearUnsavedData();
                            //    return false;
                            //}
                            //else
                            //    return true;
                            if (dres == DialogResult.Cancel)
                                return true;
                        }
                        up.ClearUnsavedData();
                    }
                }
            }
            return false;
        }

        protected Task Synchronize(Task task)
        {
            var t = Task.Factory.StartNew(async () => { try { await task; } catch { } });
            t.Wait();
            return t;
        }

        private void treeViewUnitObjects_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                e.Cancel = CurrentNodeChanged();
                //e.Cancel = !(await SaveCurrentNodeChanges());
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка отображения элемента.", ex);
                ShowError(ex);
            }
        }

        private void treeViewUnitObjects_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                EditNode();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка редактирования элемента структуры.", ex);
                ShowError(ex);
            }
        }

        #region Место под штучки контекстного меню (редактирование, добавление, удаление и т.п.)
        /// <summary>
        /// Открытие окна редактирования нода
        /// </summary>
        /// <param name="node"></param>
        private async Task ShowEditForm(TreeNode tnode, UnitNode node)
        {
            UnitProvider unitProvider = strucProvider.GetUnitProvider(node);

            var actionArgs = (LockActionArgs)await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => unitProvider.Lock(), true));
            if (actionArgs.Success)
            {
                UnitEditForm frmEdit = UnitProviders.CreateEditForm(strucProvider, node);

                if (frmEdit != null)
                {
                    if (this.MdiParent != null)
                        frmEdit.MdiParent = this.MdiParent;

                    frmEdit.OnSaveUnit += (u) => { if (!unitProvider.NewUnitNode.Equals(u))unitProvider.NewUnitNode = u; unitProvider.Save(); };
                    frmEdit.FormClosed += new FormClosedEventHandler((s, e) => unitProvider.ClearUnsavedData()); //strucProvider.ReleaseNode(node));
                    frmEdit.Show();
                }
            }
        }

        /// <summary>
        /// Редактирование узла
        /// </summary>
        private async void EditNode()
        {
            TreeNode treeNode = treeViewUnitObjects.SelectedNode;
            //if (treeNode == null || treeNode.Tag == null) return;
            //UnitNode node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeNode.Tag));
            UnitNode node = strucProvider.CurrentNode;
            if (node == null)
            {
                await ShowEditForm(treeNode, node);
            }
            else
                await ShowEditForm(treeNode, node);
        }
        /// <summary>
        /// Удаление узла
        /// </summary>
        private void RemoveNode()
        {
            List<int> lstIds = new List<int>();
            var lstTreeNodes = new List<TreeNode>();

            if (treeViewUnitObjects.SelectedNodes == null) return;

            foreach (var item in treeViewUnitObjects.SelectedNodes)
            {
                lstTreeNodes.Add(item);
                if (item.Tag != null && item.Tag is int) lstIds.Add((int)item.Tag);
            }

            if (lstIds.Count == 0) return;

            if (MessageBox.Show("Будут удалены все элементы, включая дочерние. Вы уверены?", 
                                "Удаление узла",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RemoveNodes(lstIds, lstTreeNodes);
            }
        }

        private void RemoveNodes(List<int> lstIds, List<TreeNode> lstTreeNodes)
        {
            var watcher = strucProvider.BeginDeleteUnitNode(lstIds.ToArray());
            watcher.AddErrorHandler((Exception e, ref bool h) => deleterErrorHandlerMethod(lstIds, lstTreeNodes, e, ref h));
            watcher.AddSuccessFinishHandler(() =>//.AddFinishHandler(() =>
            {
                foreach (var item in lstTreeNodes)
                {
                    if (treeViewUnitObjects.InvokeRequired)
                        treeViewUnitObjects.Invoke((Action)(() => treeViewUnitObjects.Nodes.Remove(item)));
                    else
                        treeViewUnitObjects.Nodes.Remove(item);
                }
                //Тотальная перерисовка веток. закомментчено, ибо не очень красиво выглядит в UI
                //List<UnitTreeNode> parentUnitNode = new List<UnitTreeNode>();
                //foreach (var item in treeViewUnitObjects.SelectedNodes)
                //{
                //    UnitTreeNode uTreeNode = item.Parent as UnitTreeNode;
                //    if (!parentUnitNode.Contains(uTreeNode))
                //        parentUnitNode.Add(uTreeNode);
                //}
                //ReloadTreeNodes(parentUnitNode);
            });
            RunWatcher(watcher);
        }

        private void deleterErrorHandlerMethod(List<int> lstIds, List<TreeNode> lstTreeNodes, Exception exp, ref bool handlered)        
        {
            if (exp is LockException)
            {
                var actionArgs = (LockActionArgs)Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => { throw exp; }, true) { OneShoot = true }).Result;
                if (!actionArgs.Success)
                {
                    RemoveNodes(lstIds, lstTreeNodes);
                }
                handlered = true;
            }
        }
        /// <summary>
        /// Обновление выделенных узлов
        /// </summary>
        private void UpdateNode()
        {
            if (treeViewUnitObjects.SelectedNodes != null
                && treeViewUnitObjects.SelectedNodes.Length > 0)
            {
                foreach (var item in treeViewUnitObjects.SelectedNodes)
                    ReloadTreeNode(item as UnitTreeNode);
            }
            else
                ReloadTreeNode(null);
        }
        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        private void AddItem(int type, UnitNode parent, TreeNode treeParent)
        {
            try
            {
                int parId = 0;
                if (parent != null) parId = parent.Idnum;
                //int newNodeID = await Task.Factory.StartNew(() => strucProvider.AddUnitNode(type, parId));
                int newNodeID = strucProvider.AddUnitNode(type, parId);

                ReloadTreeNode(treeParent as UnitTreeNode);
                //SelectUnitNode(await Task.Factory.StartNew(() => strucProvider.GetUnitNode(newNodeID)), false);
                SelectUnitNode(strucProvider.GetUnitNode(newNodeID), false);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка добавления нового элемента.", ex);
                ShowError(ex);
            }
        }

        /// <summary>
        /// Копирование узла или ветки
        /// </summary>
        /// <param name="recursive"></param>
        private void CopyNode(bool recursive)
        {
            List<int> lstIds = new List<int>();

            if (treeViewUnitObjects.SelectedNodes == null) return;

            foreach (var item in treeViewUnitObjects.SelectedNodes)
                if (item.Tag != null && item.Tag is int) lstIds.Add((int)item.Tag);

            if (lstIds.Count == 0) return;

            strucProvider.CopyUnitNode(lstIds.ToArray(), recursive);

            List<UnitTreeNode> parentUnitNode = new List<UnitTreeNode>();
            foreach (var item in treeViewUnitObjects.SelectedNodes)
            {
                UnitTreeNode uTreeNode = item as UnitTreeNode;
                if (!parentUnitNode.Contains(uTreeNode))
                    parentUnitNode.Add(uTreeNode.Parent as UnitTreeNode);
            }
            ReloadTreeNodes(parentUnitNode);
        }
        #endregion

        public void NodeNotFoundNotify(UnitNode unitNode)
        {
            UnitTreeNode treeNode = GetTreeNode(unitNode.Idnum, treeViewUnitObjects.Nodes) as UnitTreeNode;
            if (treeNode == null)
                treeNode = treeViewUnitObjects.SelectedNode as UnitTreeNode;
            if (treeNode != null)
            {
                ReloadTreeNode(treeNode.Parent as UnitTreeNode);
            }
        }

        /// <summary>
        /// Перегрузить узлы в дереве и их дочерние узлы
        /// </summary>
        /// <param name="parentUnitNode"></param>
        private void ReloadTreeNodes(IEnumerable<UnitTreeNode> parentUnitNode)
        {
            foreach (var item in parentUnitNode)
            {
                ReloadTreeNode(item);
            }
        }

        /// <summary>
        /// Перегрузить узел в дереве и его дочерние узлы
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void ReloadTreeNode(UnitTreeNode node)
        {
            TreeNodeCollection nodes;

            if (node == null)
                nodes = treeViewUnitObjects.Nodes;
            else
            {
                nodes = node.Nodes;
                UpdateTreeNode(node);
                node.ChildLoaded = false;
            }

            LoadUnitNodes(nodes, node);
            return;
        }

        /// <summary>
        /// Обновить узел в дереве
        /// </summary>
        /// <param name="item"></param>
        private async void UpdateTreeNode(UnitTreeNode item)
        {
            if (InvokeRequired) Invoke((Action<UnitTreeNode>)UpdateTreeNode, item);
            else
            {
                UnitNode unode;
                UnitProvider up;

                unode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)item.Tag));
                if (unode != null)
                {
                    item.Text = unode.GetNodeText();//.Text;
                    item.Tag = unode.Idnum;
                    string ikey = ((int)unode.Typ).ToString();
                    item.ImageKey = ikey;
                    item.SelectedImageKey = ikey;
                    if (treeViewUnitObjects.SelectedNode == item)
                    {
                        strucProvider.CurrentNode = unode;
                        up = strucProvider.GetUnitProvider(unode);
                        if (up == null || !up.HasChanges)
                            ShowPanel(unode);
                    }
                }
                else
                {
                    ReloadTreeNode(item.Parent as UnitTreeNode);
                }
            }
        }

        private async void cmsTreeView_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                List<UTypeNode> lstTypes = new List<UTypeNode>();
                UnitNode curNode;
                List<int> lstTypeIds = null;

                // Прячем нестабильную установку свойств
#if !DEBUG
            //setPropertyToolStripMenuItem.Visible = false;
            //toolStripMenuItem5.Visible = false;
            statisticToolStripMenuItem.Visible = false;
#endif
                // Прячем все пункты меню, которые могут быть не видны
                getParamsToolStripMenuItem.Visible = false;
                updateSpravToolStripMenuItem.Visible = false;
                deleteValuesToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Visible = false;
                editToolStripMenuItem.Enabled = false;
                removeItemToolStripMenuItem.Enabled = false;
                referencesToolStripMenuItem.Visible = false;
                addItemToolStripMenuItem.Enabled = false;
                toolStripMenuItem5.Visible = false;
                setPropertyToolStripMenuItem.Visible = false;

                addItemToolStripMenuItem.DropDownItems.Clear();

                //if (Program.MainForm.Types != null) lstTypes.AddRange(Program.MainForm.Types);
                if (strucProvider.Session.Types != null)
                    lstTypes.AddRange(strucProvider.Session.Types);

                removeItemToolStripMenuItem.Enabled = treeViewUnitObjects.SelectedNode != null;

                if (treeViewUnitObjects.SelectedNode != null && treeViewUnitObjects.SelectedNode.Tag != null
                    && treeViewUnitObjects.SelectedNode.Tag is int)
                {
                    int nodeID = (int)treeViewUnitObjects.SelectedNode.Tag;
                    curNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode(nodeID));
                }
                else
                {
                    curNode = null;
                }

                if (curNode != null)
                {
                    foreach (var item in lstTypes)
                    {
                        if (item.Idnum == curNode.Typ)
                        {
                            if (!item.ChildFilterAll)
                                lstTypeIds = item.ChildFilter;
                            else
                                lstTypeIds = null;
                            break;
                        }
                    }
                    bool canEdit = strucProvider.CheckAccess(curNode, Privileges.Write);

                    // обращение к блочному
                    getParamsToolStripMenuItem.Visible = canEdit && curNode.Typ == (int)UnitTypeId.Channel;
                    updateSpravToolStripMenuItem.Visible = canEdit && curNode.Typ == (int)UnitTypeId.Block || curNode.Typ == (int)UnitTypeId.Channel;
                    deleteValuesToolStripMenuItem.Visible = canEdit && curNode.Typ == (int)UnitTypeId.Block || curNode.Typ == (int)UnitTypeId.Channel;
                    toolStripMenuItem2.Visible = canEdit && curNode.Typ == (int)UnitTypeId.Channel || curNode.Typ == (int)UnitTypeId.Block;
                    // просто редактирование
                    editToolStripMenuItem.Enabled = canEdit && UnitProviders.CanCreateEditForm(curNode);
                    removeItemToolStripMenuItem.Enabled = canEdit;
                    // прочие
                    referencesToolStripMenuItem.Visible = curNode is NormFuncNode;

                    toolStripMenuItem5.Visible = strucProvider.FilterParams == FilterParams.All;
                    setPropertyToolStripMenuItem.Visible = strucProvider.FilterParams == FilterParams.All;

                }

                UserNode unode = strucProvider.Session.User;

                ToolStripItem mnuItem;

                if (unode != null)
                {
                    foreach (var item in lstTypes)
                    {
                        if (item.Idnum != (int)UnitTypeId.Unknown &&
                            (lstTypeIds == null || lstTypeIds.Contains(item.Idnum))
                            && (Filter.IsEmpty || Filter.Contains(item.Idnum)))
                        {
                            int typ = item.Idnum;
                            mnuItem = new ToolStripMenuItem(item.Text);
                            mnuItem.Image = Program.MainForm.Icons.Images[item.Idnum.ToString()];
                            mnuItem.Visible = true;
                            mnuItem.Click += new EventHandler((EventHandler)((s, ev) => AddItem(typ, curNode, treeViewUnitObjects.SelectedNode)));
                            addItemToolStripMenuItem.DropDownItems.Add(mnuItem);
                        }
                    }
                }
                addItemToolStripMenuItem.Enabled = addItemToolStripMenuItem.DropDownItems.Count > 0;
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка открытия окна контекстного меню.", ex);
                ShowError(ex);
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveNode();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка удаления элемента.", ex);
                ShowError(ex);
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateNode();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка обновления элементов структуры.", ex);
                ShowError(ex);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                EditNode();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка редактирования элемента.", ex);
                ShowError(ex);
            }
        }

        private void copyItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CopyNode(false);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка копирования элемента.", ex);
                ShowError(ex);
            }
        }

        private void copyTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CopyNode(true);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка копирования элемента.", ex);
                ShowError(ex);
            }
        }

        private void treeViewUnitObjects_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
                {
                    UnitTreeNode treeNode = (UnitTreeNode)e.Data.GetData(typeof(UnitTreeNode));

                    if (treeNode.TreeView == treeViewUnitObjects) e.Effect = DragDropEffects.Move;
                    else e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка перетаскивания элемента.", ex);
                ShowError(ex);
            }
        }

        private UnitTreeNode prevOverTreeNode;
        private enum AddNodeInfo { AddAfter, AddBefore, AddInto };
        private AddNodeInfo addInfo;
        private void treeViewUnitObjects_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
                {
                    UnitTreeNode treeNode = (UnitTreeNode)e.Data.GetData(typeof(UnitTreeNode));
                    if (treeNode.TreeView == treeViewUnitObjects)
                    {
                        Point clientPoint = treeViewUnitObjects.PointToClient(new Point(e.X, e.Y));
                        UnitTreeNode unitTreeNode = treeViewUnitObjects.GetNodeAt(clientPoint.X, clientPoint.Y) as UnitTreeNode;

                        if (unitTreeNode != null)
                        {
                            if ((clientPoint.Y - unitTreeNode.Bounds.Y) < unitTreeNode.Bounds.Height * 0.15) addInfo = AddNodeInfo.AddBefore;
                            else if ((unitTreeNode.Bounds.Y + unitTreeNode.Bounds.Height - clientPoint.Y) < unitTreeNode.Bounds.Height * 0.15) addInfo = AddNodeInfo.AddAfter;
                            else
                            {
                                if (prevOverTreeNode != unitTreeNode || addInfo != AddNodeInfo.AddInto)
                                    unitTreeNode.BackColor = Color.Cyan;
                                addInfo = AddNodeInfo.AddInto;
                            }

                            UnitNode oldParent = strucProvider.GetUnitNode(treeNode.Node.ParentId);
                            UnitNode newParent = addInfo == AddNodeInfo.AddInto ? unitTreeNode.Node : strucProvider.GetUnitNode(unitTreeNode.Node.ParentId);

                            if (!strucProvider.CheckAccess(treeNode.Node, Privileges.Write)
                                || (oldParent != null && !strucProvider.CheckAccess(oldParent, Privileges.Write))
                                || (newParent != null && !strucProvider.CheckAccess(newParent, Privileges.Write)))
                            {
                                e.Effect = DragDropEffects.None;
                            }
                        }
                        if (prevOverTreeNode != null && (prevOverTreeNode != unitTreeNode || addInfo != AddNodeInfo.AddInto))
                            prevOverTreeNode.BackColor = treeViewUnitObjects.BackColor;
                        prevOverTreeNode = unitTreeNode;
                    }
                    else
                        e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка перетаскивания элемента.", ex);
                ShowError(ex);
            }
        }

        private void treeViewUnitObjects_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
                {
                    UnitTreeNode droppedTreeNode = (UnitTreeNode)e.Data.GetData(typeof(UnitTreeNode));
                    if (droppedTreeNode.Node != null)
                    {
                        if (prevOverTreeNode != null)
                        {
                            prevOverTreeNode.BackColor = treeViewUnitObjects.BackColor;
                            int id = (int)prevOverTreeNode.Tag;
                            UnitNode node = strucProvider.GetUnitNode(id);
                            if (node != null)
                            {
                                UnitNode parent = node;
                                while (parent != null)
                                {
                                    if (parent.Idnum == droppedTreeNode.Node.Idnum)
                                    {
                                        e.Effect = DragDropEffects.None;
                                        return;
                                    }
                                    if (parent.ParentId == 0)
                                        parent = null;
                                    else
                                        parent = strucProvider.GetUnitNode(parent.ParentId);
                                }

                                UnitNode newParent = addInfo == AddNodeInfo.AddInto ? node : strucProvider.GetUnitNode(node.ParentId);
                                TreeNode newParentTreeNode = addInfo == AddNodeInfo.AddInto ? prevOverTreeNode : prevOverTreeNode.Parent;

                                strucProvider.MoveUnitNode(newParent, droppedTreeNode.Node, addInfo == AddNodeInfo.AddInto ? null : node, addInfo == AddNodeInfo.AddAfter);

                                UpdateAfterMove(droppedTreeNode, newParentTreeNode);

                                if (droppedTreeNode.TreeView == treeViewUnitObjects) e.Effect = DragDropEffects.Move;
                                else e.Effect = DragDropEffects.Copy;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка перетаскивания элемента.", exc);
                ShowError(exc);
            }
        }

        private void treeViewUnitObjects_DragLeave(object sender, EventArgs e)
        {
            try
            {
                if (prevOverTreeNode != null)
                {
                    prevOverTreeNode.BackColor = treeViewUnitObjects.BackColor;
                    prevOverTreeNode = null;
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка перетаскивания элемента.", ex);
                ShowError(ex);
            }
        }

        /// <summary>
        /// Обновить структуру после перетааскивания
        /// </summary>
        /// <param name="droppedTreeNode">Перетащенный элемент</param>
        /// <param name="newParentTreeNode">Новый родитель</param>
        private void UpdateAfterMove(UnitTreeNode droppedTreeNode, TreeNode newParentTreeNode)
        {
            if (InvokeRequired) Invoke((Action<UnitTreeNode, TreeNode>)UpdateAfterMove, droppedTreeNode, newParentTreeNode);
            else
            {
                TreeNode selectedNode = treeViewUnitObjects.SelectedNode;
                if (selectedNode == null) return;
                bool oldParentExpand = droppedTreeNode.Parent != null && droppedTreeNode.Parent.IsExpanded,
                   newParentExpand = newParentTreeNode != null && newParentTreeNode.IsExpanded;
                int oldParentId = droppedTreeNode.Parent == null ? 0 : (int)droppedTreeNode.Parent.Tag,
                    newParentId = newParentTreeNode == null ? 0 : (int)newParentTreeNode.Tag;

                var oldParentNode = droppedTreeNode.Parent as UnitTreeNode;
                ReloadTreeNode(droppedTreeNode.Parent as UnitTreeNode);

                if (newParentTreeNode != oldParentNode)
                    ReloadTreeNode(newParentTreeNode as UnitTreeNode);

                TreeNode oldParent = GetTreeNode(oldParentId, treeViewUnitObjects.Nodes),
                    newParent = GetTreeNode(newParentId, treeViewUnitObjects.Nodes);
                TreeNode tempNode = newParent;

                treeViewUnitObjects.SelectedNode = GetTreeNode((int)selectedNode.Tag, treeViewUnitObjects.Nodes);

                while (tempNode != null && tempNode != oldParent) tempNode = tempNode.Parent;
                if (tempNode == oldParent && newParentExpand && newParent != null && !newParent.IsExpanded) newParent.Expand();

                if (oldParentExpand && oldParent != null && !oldParent.IsExpanded) oldParent.Expand();
                if (tempNode != oldParent && newParentExpand && newParent != null && !newParent.IsExpanded) newParent.Expand();
            }
        }

        /// <summary>
        /// Получить узел дерева по Ид узла
        /// </summary>
        /// <param name="id">ИД узла</param>
        /// <param name="nodes">Список узлов дерева для поиска</param>
        /// <returns></returns>
        private TreeNode GetTreeNode(int id, TreeNodeCollection nodes)
        {
            TreeNode ret;
            foreach (TreeNode treeNode in nodes)
                if (treeNode.Tag is int && (int)treeNode.Tag == id)
                    return treeNode;

            foreach (TreeNode treeNode in nodes)
                if ((ret = GetTreeNode(id, treeNode.Nodes)) != null) return ret;
            return null;
        }

        private void treeViewUnitObjects_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                e.UseDefaultCursors = false;
            }
        }

        private async void getParamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                UnitTreeNode unitTreeNode = treeViewUnitObjects.SelectedNode as UnitTreeNode;
                ChannelNode chNode;

                chNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)unitTreeNode.Tag) as ChannelNode);

                if (chNode != null)
                {
                    GetParamsForm getForm = new GetParamsForm(strucProvider, chNode, strucProvider.GetUnitProvider(chNode));

                    getForm.OnUpdated += new EventHandler((s, ev) => { /*UpdateTreeNode(unitTreeNode);*/ ReloadTreeNode(unitTreeNode); });

                    getForm.Show(this);
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка запроса параметров для системы сбора.", ex);
                ShowError(ex);
            }
        }

        private void updateSpravToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "Обновление справочников", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK) return;
                UnitNode node;

                //if (treeViewUnitObjects.SelectedNode == null
                //    || !(treeViewUnitObjects.SelectedNode.Tag is int))
                //    return;

                //block = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeViewUnitObjects.SelectedNode.Tag) as BlockNode);
                node = strucProvider.CurrentNode;

                if (node != null && (node.Typ == (int)UnitTypeId.Block || node.Typ == (int)UnitTypeId.Channel))
                {
                    AsyncOperationWatcher watcher =
                        strucProvider.SendSprav(node.Idnum);
                    RunWatcher(watcher);
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка обновления справочников для системы сбора.", ex);
                ShowError(ex);
            }
        }
        private async void deleteValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                UnitNode unitNode;

                //if (treeViewUnitObjects.SelectedNode == null || !(treeViewUnitObjects.SelectedNode.Tag is int))
                //    return;
                //unitNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeViewUnitObjects.SelectedNode.Tag) as UnitNode);
                unitNode = strucProvider.CurrentNode;

                if (unitNode != null)
                {
                    SelectTimeForm selectTimeForm = new SelectTimeForm();

                    if (selectTimeForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            await Task.Factory.StartNew(() => strucProvider.DeleteLoadValues(unitNode.Idnum, selectTimeForm.Time));
                        }
                        catch (Exception exc)
                        {
                            log.WarnException("Ошибка удаления собранных значений.", exc);
                            ShowError(exc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка удаления собранных значений.", ex);
                ShowError(ex);
            }
        }

        private async Task UpdatePanel()
        //private void UpdatePanel()
        {
            if (InvokeRequired)
            {
                Invoke((Action)(async () => await UpdatePanel()));
                //Invoke((Action)(() => UpdatePanel()));
            }
            else
            {
                if (treeViewUnitObjects.SelectedNode == null)
                    ClearPanel();
                else
                {
                    TreeNode treeNode = treeViewUnitObjects.SelectedNode;
                    UnitNode node;

                    if (treeNode.Tag is int)
                    {
                        UnitProvider up = null;
                        int unitNodeID = (int)treeViewUnitObjects.SelectedNode.Tag;
                        node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode(unitNodeID));
                        //node = strucProvider.GetUnitNode(unitNodeID);

                        up = strucProvider.GetUnitProvider(node);
                        //if (node != null && dicUnitProviders.TryGetValue(node.Idnum, out up))
                        //{
                        //    up.Dispose();
                        //    up = null;
                        //}

                        if (node == null)
                        {
                            ReloadTreeNode(treeNode.Parent as UnitTreeNode);
                        }
                        else
                            this.Invoke((Action<UnitNode>)(ShowPanel), node);
                    }
                    selectedNode = treeNode;
                }
            }
        }

        private void selectionTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Synchronize(UpdatePanel());
                //UpdatePanel();
                selectionTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка отображения элемента структуры.", ex);
                ShowError(ex);
            }
        }

        private void UniForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                strucProvider.Dispose();
                strucProvider = null;

                if (frmSearch != null && !frmSearch.IsDisposed)
                    frmSearch.Dispose();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка закрытия окна.", ex);
                ShowError(ex);
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int parentId;

                if (frmSearch != null && !frmSearch.IsDisposed)
                    frmSearch.Dispose();

                if (treeViewUnitObjects.SelectedNode == null && !(treeViewUnitObjects.SelectedNode.Tag is int)) return;
                parentId = (int)treeViewUnitObjects.SelectedNode.Tag;

                frmSearch = new ParamSearchForm(strucProvider, parentId);
                Program.MainForm.AddExtendForm(frmSearch);
                frmSearch.Show(this);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка поиска элементов.", ex);
                ShowError(ex);
            }
        }

        private void FilterChange_Click(object sender, EventArgs e)
        {
            try
            {
                bool some = false;
                int type;
                foreach (ToolStripButton item in tsFilter.Items)
                {
                    if (item.Checked)
                    {
                        some = true;
                        break;
                    }
                }
                type = (int)UnitTypeId.Folder;
                if (tsbFolders.Checked) Filter.Add(type);
                else Filter.Remove(type);
                type = (int)UnitTypeId.Graph;
                if (tsbGraphs.Checked) Filter.Add(type);
                else Filter.Remove(type);
                type = (int)UnitTypeId.TEPTemplate;
                if (tsbCalcs.Checked) { Filter.Add(type); Filter.Add((int)UnitTypeId.TEP); }
                else { Filter.Remove(type); Filter.Remove((int)UnitTypeId.TEP); }
                type = (int)UnitTypeId.ManualGate;
                if (tsbManuals.Checked) { Filter.Add(type); Filter.Add((int)UnitTypeId.ManualParameter); }
                else { Filter.Remove(type); Filter.Remove((int)UnitTypeId.ManualParameter); }
                type = (int)UnitTypeId.MonitorTable;
                if (tsbMonitors.Checked) Filter.Add(type);
                else Filter.Remove(type);
                type = (int)UnitTypeId.Schema;
                if (tsbSchemas.Checked) Filter.Add(type);
                else Filter.Remove(type);
                type = (int)UnitTypeId.ExcelReport;
                if (tsbReports.Checked) { Filter.Add(type); Filter.Add((int)UnitTypeId.Report); }
                else { Filter.Remove(type); Filter.Remove((int)UnitTypeId.Report); }

                if (some)
                    AddDefaultTypes();
                else
                    Filter.Clear();
                UpdateTree();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка фильтрации элементов структуры.", ex);
                ShowError(ex);
            }
        }

        private void UpdateTree()
        {
            try
            {
                treeViewUnitObjects.BeginUpdate();
                //treeViewUnitObjects.Nodes.Clear();
                LoadUnitNodes(treeViewUnitObjects.Nodes, null);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка загрузки структуры элементов.", ex);
                ShowError(ex);
            }
            finally
            {
                treeViewUnitObjects.EndUpdate();
            }
        }

        private void AddDefaultTypes()
        {
            // ???
        }

        private void UniForm_Load(object sender, EventArgs e)
        {
            try
            {
                tsFilter.Visible = Filter == null || Filter.IsEmpty;
                SetMenuImages();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка загрузки структуры элементов.", ex);
                ShowError(ex);
            }
        }

        private void SetMenuImages()
        {
            ImageList icons = Program.MainForm.Icons;
            if (icons != null)
            {
                string key;
                key = ((int)UnitTypeId.Folder).ToString();
                if (icons.Images.ContainsKey(key)) tsbFolders.Image = icons.Images[key];
                key = ((int)UnitTypeId.Graph).ToString();
                if (icons.Images.ContainsKey(key)) tsbGraphs.Image = icons.Images[key];
                key = ((int)UnitTypeId.Schema).ToString();
                if (icons.Images.ContainsKey(key)) tsbSchemas.Image = icons.Images[key];
#if EMA
                key = ((int)UnitTypeId.Report).ToString();
#else
                key = ((int)UnitTypeId.ExcelReport).ToString();
#endif
                if (icons.Images.ContainsKey(key)) tsbReports.Image = icons.Images[key];
                key = ((int)UnitTypeId.MonitorTable).ToString();
                if (icons.Images.ContainsKey(key)) tsbMonitors.Image = icons.Images[key];
                key = ((int)UnitTypeId.ManualGate).ToString();
                if (icons.Images.ContainsKey(key)) tsbManuals.Image = icons.Images[key];
                key = ((int)UnitTypeId.TEPTemplate).ToString();
                if (icons.Images.ContainsKey(key)) tsbCalcs.Image = icons.Images[key];
            }
        }

        private void tsbShowTree_Click(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.Panel1Collapsed = !tsbShowTree.Checked;
            }
            catch (Exception ex)
            {
                log.WarnException("", ex);
                ShowError(ex);
            }
        }

        private void UniForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = CurrentNodeChanged();
                SetFormClosingFlag(!e.Cancel);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка закрытия окна.", ex);
                ShowError(ex);
            }
        }

        internal UnitProvider[] GetTabbedProviders()
        {
            TabControl tab = null;
            List<UnitProvider> providerList = new List<UnitProvider>();
            BaseUnitControl unitControl;

            foreach (Control control in splitContainer1.Panel2.Controls)
            {
                if (control is TabControl)
                    tab = control as TabControl;
                if ((unitControl = control as BaseUnitControl) != null)
                    providerList.Add(unitControl.UnitProvider);
            }

            if (tab != null)
                foreach (TabPage tabPage in tab.TabPages)
                    foreach (Control control in tabPage.Controls)
                        if ((unitControl = control as BaseUnitControl) != null)
                            providerList.Add(unitControl.UnitProvider);
            return providerList.ToArray();
        }

        #region IUniForm Members

        UnitNode IUniForm.GetUnitNode(TreeNode treeNode)
        {
            if (treeNode == null || treeNode.Tag == null)
                return null;

            int id = (int)treeNode.Tag;

            return strucProvider.GetUnitNode(id);
        }

        TreeView IUniForm.StructureTree
        {
            get { return treeViewUnitObjects; }
        }

        Panel IUniForm.StructurePanel
        {
            get { return structurePanel; }
        }

        #endregion

        private async void setPropertyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode treeNode = treeViewUnitObjects.SelectedNode;
                if (treeNode == null || treeNode.Tag == null) return;
                UnitNode node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeNode.Tag));

                SetPropertyForm setForm = new SetPropertyForm(strucProvider, node, Filter.GetTypes());
                //setForm.OnSave += UniForm_Save;
                setForm.FormClosed += new FormClosedEventHandler((s, args) => ReloadTreeNode(treeNode as UnitTreeNode));
                //setForm.MdiParent = MdiParent;
                setForm.ShowDialog();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка установки свойств.", ex);
                ShowError(ex);
            }
        }

        /// <summary>
        /// Найти и выделить указанный узел
        /// </summary>
        /// <param name="node">Искомый узел</param>
        /// <param name="showInTab">Отобразить информацию во вкладочке</param>
        public async void SelectUnitNode(UnitNode node, bool showInTab)
        {
            Stack<UnitNode> ShowNode = new Stack<UnitNode>();
            UnitTreeNode nearestNode = null;
            UnitNode unitNode = node;
            Suspended = true;

            // Ищем ли сам узел или ближайший загруженный родительский узел
            while (unitNode != null
                && (nearestNode = FindNode(unitNode)) == null)
            {
                // в стеке формируем список узлов, которые требуется подгрузить
                ShowNode.Push(unitNode);
                unitNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode(unitNode.ParentId));
            }

            if (ShowNode.Count == 0 && nearestNode != null)
            {
                treeViewUnitObjects.SelectedNode = nearestNode;
                Suspended = false;
                return;
            }

            TreeNodeCollection collection;
            if (nearestNode != null)

                collection = nearestNode.Nodes;
            else
                collection = treeViewUnitObjects.Nodes;

            LoadUnitNodes(collection, nearestNode);
            //if (watcher == null)
            //{
            RecursiveLoadUnitNode(nearestNode, ShowNode, showInTab);
            //}
            //else
            //    watcher.AddFinishHandler(() => RecursiveLoadUnitNode(nearestNode, ShowNode, showInTab));
        }

        /// <summary>
        /// Рекурсивно подгружать структуру, что бы выделить нужный узел.
        /// </summary>
        /// <param name="parentTreeNode">Родительский узел для подгружаемых узлов</param>
        /// <param name="ShowNode">Стэк с подгружаемыми параметрами</param>
        /// <param name="showInTab">Показывать параметр во вкладочки</param>
        public void RecursiveLoadUnitNode(UnitTreeNode parentTreeNode, Stack<UnitNode> ShowNode, bool showInTab)
        {
            if (InvokeRequired)
                Invoke((Action<UnitTreeNode, Stack<UnitNode>, bool>)RecursiveLoadUnitNode,
                    parentTreeNode,
                    ShowNode,
                    showInTab);
            else
            {
                bool finded = false;
                UnitTreeNode nearestNode;
                TreeNodeCollection collection;
                UnitNode unitNode;

                if (ShowNode.Count == 0)
                    return;

                unitNode = ShowNode.Pop();

                if (parentTreeNode == null)
                    collection = treeViewUnitObjects.Nodes;
                else
                    collection = parentTreeNode.Nodes;

                foreach (var item in collection)
                {
                    if ((nearestNode = item as UnitTreeNode) != null
                        && nearestNode.Node != null
                        && nearestNode.Node.Idnum == unitNode.Idnum)
                    {
                        finded = true;
                        if (ShowNode.Count == 0)
                        {
                            if (showInTab)
                            {
                                parameterToShowInTab = unitNode;
                            }
                            treeViewUnitObjects.SelectedNode = nearestNode;
                        }
                        else
                        {
                            //var watcher = 
                            LoadUnitNodes(nearestNode.Nodes, nearestNode);
                            //watcher.AddFinishHandler(() => RecursiveLoadUnitNode(nearestNode, ShowNode, showInTab));
                            RecursiveLoadUnitNode(nearestNode, ShowNode, showInTab);
                            return;
                        }
                        break;
                    }
                }
                if (!finded)
                {
                    if (showInTab)
                    {
                        parameterToShowInTab = unitNode;
                    }
                    treeViewUnitObjects.SelectedNode = parentTreeNode;
                }
                Suspended = false;
            }
        }

        /// <summary>
        /// Поиск в загруженном на форме дереве нужный узел
        /// </summary>
        /// <param name="unitNode">Искомый узел</param>
        /// <returns></returns>
        private UnitTreeNode FindNode(UnitNode unitNode)
        {
            // обходим дерево сверху
            TreeNodeCollection collection = treeViewUnitObjects.Nodes;
            UnitTreeNode unitTreeNode;

            Queue<UnitTreeNode> findQueue = new Queue<UnitTreeNode>();

            // ложим корневые элементы в очередь
            foreach (var item in collection)
            {
                if ((unitTreeNode = item as UnitTreeNode) != null)
                {
                    findQueue.Enqueue(unitTreeNode);
                }
            }

            while (findQueue.Count > 0)
            {
                unitTreeNode = findQueue.Dequeue();
                // если нашли элемент выходим
                if (unitTreeNode.Node != null
                    && unitTreeNode.Node.Idnum == unitNode.Idnum)
                    return unitTreeNode;
                // ложим дочерние элементы в очередь
                foreach (var item in unitTreeNode.Nodes)
                {
                    if ((unitTreeNode = item as UnitTreeNode) != null)
                    {
                        findQueue.Enqueue(unitTreeNode);
                    }
                }
            }

            // ничего не нашли
            return null;
        }

        private async void referencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                NormFuncNode curNode;

                if (treeViewUnitObjects.SelectedNode != null && treeViewUnitObjects.SelectedNode.Tag != null
                    && treeViewUnitObjects.SelectedNode.Tag is int)
                    curNode = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeViewUnitObjects.SelectedNode.Tag) as NormFuncNode);
                else
                    curNode = null;

                TepForm form = new TepForm(strucProvider, Program.MainForm.Icons);
                Program.MainForm.AddExtendForm(form);
                //form.Node = currentParam;
                form.Func = new COTES.ISTOK.Calc.FunctionInfo(curNode.Code, "", "");
                form.Relation = FormulaRelation.Reference;
                form.TopMost = true;
                //form.ShowDialog();
                form.Show();
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка запроса ссылок на параметр/функцию.", ex);
                ShowError(ex);
            }
        }

        private async void statisticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode treeNode = treeViewUnitObjects.SelectedNode;
                if (treeNode == null || treeNode.Tag == null) return;
                UnitNode node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeNode.Tag));

                Dictionary<int, int> stat = await Task.Factory.StartNew(() => strucProvider.GetStatistic(node.Idnum));

                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                foreach (var type in stat.Keys)
                {
                    builder.AppendFormat("\n{0}: {1}", type, stat[type]);
                }

                MessageBox.Show(builder.ToString());
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка запроса статистики.", ex);
                ShowError(ex);
            }
        }

        private async void viewAuditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode treeNode = treeViewUnitObjects.SelectedNode;
                if (treeNode == null || treeNode.Tag == null) return;
                //UnitNode node = current_data_service.GetUnitNode((int)treeNode.Tag);
                UnitNode node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)treeNode.Tag));
                if (node != null)
                {
                    var auditForm = new ViewAuditForm(strucProvider.Session);
                    //auditForm.RDS = current_data_service;
                    auditForm.UnitNode = node;

                    auditForm.MdiParent = MdiParent;
                    auditForm.Show();
                }
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка просмотра аудита.", ex);
                ShowError(ex);
            }
        }
    }
}
