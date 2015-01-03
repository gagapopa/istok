using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования формул
    /// </summary>
    partial class FormulaEditControl : BaseUnitControl, IDisposable
    {
        private string formula;

        private Dictionary<string, FunctionInfo> funcs = new Dictionary<string, FunctionInfo>();

        private Point mouseLocation = new Point();

        private ParameterNode currentParam = null;
        private FunctionInfo currentFunction = null;
        private string strToolTip = "";

        private bool showEditButtons;
        public bool ShowEditButtons
        {
            get { return showEditButtons; }
            set
            {
                showEditButtons = value;
                OnShowEditButtonsChanged();
            }
        }

        private void OnShowEditButtonsChanged()
        {
            if (InvokeRequired) Invoke((Action)OnShowEditButtonsChanged);
            else
            {
                btnLock.Visible = ShowEditButtons;
                saveToolStripButton.Visible = ShowEditButtons;
                btnCancel.Visible = ShowEditButtons; 
            }
        }
        //public FormulaUnitProvider UnitProvider
        //{
        //    get { return unitProvider as FormulaUnitProvider; }
        //    set { unitProvider = value; /*UpdateSnappList();*/ }
        //}

        public FormulaEditControl()
            : base()
        {
            InitializeComponent();
            ShowEditButtons = true;
            InitializeFormula();
        }
        public FormulaEditControl(UnitProvider unitProvider)
            : base(unitProvider)
        {
            InitializeComponent();
            ShowEditButtons = true;
            InitializeFormula();
            CreateControl();
        }

        bool evented;
        public async override void InitiateProcess()
        {
            FormulaUnitProvider formulaProvider = UnitProvider as FormulaUnitProvider;
            if (formulaProvider != null && !initiated)
            {
                UnitProvider_EditModeChanged(formulaProvider, EventArgs.Empty);
                UnitProvider_FormulaChanged(formulaProvider, EventArgs.Empty);
                SetArgumentList(formulaProvider.Arguments);

                await Task.Factory.StartNew(() => formulaProvider.StartProvider());

                if (!evented)
                {
                    formulaProvider.ConstsRetrieved += new Action<ConstsInfo[]>(SetConstsList);
                    formulaProvider.ArgumentsRetrieved += new Action<CalcArgumentInfo[]>(SetArgumentList);
                    formulaProvider.EditModeChanged += new EventHandler(UnitProvider_EditModeChanged);
                    formulaProvider.FormulaChanged += new EventHandler(UnitProvider_FormulaChanged);
                    evented = true;
                }
                revisionToolStripComboBox.Items.Add(formulaProvider.CurrentRevision);
                revisionToolStripComboBox.SelectedItem = formulaProvider.CurrentRevision;

                SetFunctionList(formulaProvider.Functions);
                SetConstsList(formulaProvider.Consts);
            }
            base.InitiateProcess();
        }

        void UnitProvider_FormulaChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)UnitProvider_FormulaChanged, sender, e);
            else
            {
                FormulaUnitProvider formulaProvider = UnitProvider as FormulaUnitProvider;
                if (formulaProvider != null)
                {
                    formula = formulaProvider.Formula;
                    UpdateFormula();
                }
            }
        }

        protected override void DisposeControl()
        {
            FormulaUnitProvider formulaProvider = UnitProvider as FormulaUnitProvider;
            if (formulaProvider!=null)
            {
                formulaProvider.ConstsRetrieved -= new Action<ConstsInfo[]>(SetConstsList);
                formulaProvider.ArgumentsRetrieved -= new Action<CalcArgumentInfo[]>(SetArgumentList);
                formulaProvider.FormulaChanged -= new EventHandler(UnitProvider_FormulaChanged);
                formulaProvider.EditModeChanged -= new EventHandler(UnitProvider_EditModeChanged); 
            }
            base.DisposeControl();
        }

        void UnitProvider_EditModeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)UnitProvider_EditModeChanged, sender, e);
            else
            {
                rtbFormula.ReadOnly = !UnitProvider.EditMode;
                rtbFormula.BackColor = SystemColors.Window;
                rtbFormula.BackColor = UnitProvider.EditMode ? SystemColors.Window : SystemColors.Control;
                this.btnLock.Enabled = !UnitProvider.EditMode;
                this.saveToolStripButton.Enabled = UnitProvider.EditMode;
                this.btnCancel.Enabled = UnitProvider.EditMode;
                this.btnCheck.Enabled = UnitProvider.EditMode;
                this.toolStripSeparator1.Enabled = UnitProvider.EditMode;
                this.snippetToolStripDropDownButton.Enabled = UnitProvider.EditMode;
                this.functionToolStripDropDownButton.Enabled = UnitProvider.EditMode;
                this.constsToolStripDropDownButton.Enabled = UnitProvider.EditMode;
                this.argsToolStripDropDownButton.Enabled = UnitProvider.EditMode;
                this.toolStripLabel1.Enabled = UnitProvider.EditMode;
                this.cbxAggregation.Enabled = UnitProvider.EditMode;
                this.btnCut.Enabled = UnitProvider.EditMode;
                this.btnCopy.Enabled = UnitProvider.EditMode;
                this.btnPaste.Enabled = UnitProvider.EditMode;
                this.showImageToolStripButton.Enabled = UnitProvider.EditMode; 
            }
        }

        //private void UpdateSnappList()
        //{
        //    if (UnitProvider != null)
        //    {
        //        UnitProvider.FunctionsRetrieved += new Action<FunctionInfo[]>(SetFunctionList);
        //        UnitProvider.ConstsRetrieved += new Action<ConstsInfo[]>(SetConstsList);
        //        UnitProvider.ArgumentsRetrieved += new Action<CalcArgumentInfo[]>(SetArgumentList);

        //        SetFunctionList(UnitProvider.Functions);
        //        SetConstsList(UnitProvider.Consts);
        //        SetArgumentList(UnitProvider.Arguments);
        //    }
        //}

        FunctionCatalog topFunctions = new FunctionCatalog(null);

        public void SetFunctionList(IEnumerable<FunctionInfo> functions)
        {
            string node_type;

            funcs.Clear();
            topFunctions.Clear();
            if (functions != null)
                foreach (FunctionInfo func in functions)
                {
                    node_type = func.GroupName;
                    funcs[func.Name] = func;
                    topFunctions.AddItem(node_type, func);
                }
        }

        private void functionToolStripDropDownButton_DropDownOpening(object sender, EventArgs e)
        {
            SetFunctionList((UnitProvider as FormulaUnitProvider).Functions);
            FillFunctionMennu(functionToolStripDropDownButton, topFunctions);
        }

        private void FillFunctionMennu(ToolStripDropDownItem toolStripMenuItem, FunctionCatalog catalog)
        {
            toolStripMenuItem.DropDownItems.Clear();
            foreach (var item in catalog.Catalogs)
            {
                ToolStripMenuItem nodeTypeStripItem = new ToolStripMenuItem(item.CatalogName);
                nodeTypeStripItem.Tag = item;
                nodeTypeStripItem.DropDownOpening += new EventHandler(nodeTypeStripItem_DropDownOpening);
                nodeTypeStripItem.DropDownItems.Add("Пусто");
                toolStripMenuItem.DropDownItems.Add(nodeTypeStripItem);
            }
            StringBuilder typesBuilder = new StringBuilder();
            foreach (var item in catalog.Functions)
            {
                ToolStripItem funcMenu = new ToolStripMenuItem(item.Name);

                typesBuilder.Length = 0;
                foreach (var argument in item.Arguments)
                {
                    if (typesBuilder.Length > 0) typesBuilder.Append(", ");
                    typesBuilder.Append(argument.Name);
                    if (!String.IsNullOrEmpty(argument.DefaultValue))
                        typesBuilder.AppendFormat(" = {0}", argument.DefaultValue);
                }

                funcMenu.Text = item.Name + "(" + typesBuilder.ToString() + ")";
                funcMenu.ToolTipText = item.Comment;
                funcMenu.Tag = item;
                funcMenu.Click += new EventHandler(funcMenu_Click);
                toolStripMenuItem.DropDownItems.Add(funcMenu);
            }
        }

        void nodeTypeStripItem_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem nodeTypeStripItem;
            FunctionCatalog category;

            if ((nodeTypeStripItem = sender as ToolStripMenuItem) != null
                && (category = nodeTypeStripItem.Tag as FunctionCatalog) != null)
                FillFunctionMennu(nodeTypeStripItem, category);
        }

        class FunctionCatalog
        {
            public const String SubCategorySeparator = ";";
            public IEnumerable<FunctionCatalog> Catalogs { get { return catalogs.Values; } }
            public List<FunctionInfo> Functions { get; protected set; }
            Dictionary<String, FunctionCatalog> catalogs;

            public String CatalogName { get; set; }

            public FunctionCatalog(String name)
            {
                CatalogName = name;
                catalogs = new Dictionary<String, FunctionCatalog>();
                Functions = new List<FunctionInfo>();
            }

            public void AddItem(String subCatalogName, FunctionInfo function)
            {
                if (String.IsNullOrEmpty(subCatalogName))
                    Functions.Add(function);
                else
                {
                    int separatorIndex = subCatalogName.IndexOf(SubCategorySeparator);
                    String topCategoryName = separatorIndex > 0 ? subCatalogName.Substring(0, separatorIndex) : subCatalogName;
                    FunctionCatalog subCatalog;
                    if (!catalogs.TryGetValue(topCategoryName, out subCatalog))
                        catalogs.Add(topCategoryName, subCatalog = new FunctionCatalog(topCategoryName));
                    subCatalog.AddItem(separatorIndex > 0 ? subCatalogName.Substring(separatorIndex + 1) : null, function);
                }
            }

            public void Clear()
            {
                Functions.Clear();
                catalogs.Clear();
            }
        }

        public void SetConstsList(IEnumerable<ConstsInfo> consts)
        {
            if (InvokeRequired) Invoke((Action<IEnumerable<ConstsInfo>>)SetConstsList, consts);
            else
            {
                ToolStripItem constItem;
                constsToolStripDropDownButton.DropDownItems.Clear();
                if (consts != null)
                    foreach (var item in consts)
                    {
                        constItem = new ToolStripMenuItem();
                        constItem.Text = item.Name;
                        constItem.ToolTipText = item.Description;
                        constItem.Click += new EventHandler(constItem_Click);
                        constsToolStripDropDownButton.DropDownItems.Add(constItem);
                    }
            }
        }

        public void SetArgumentList(IEnumerable<CalcArgumentInfo> arguments)
        {
            argsToolStripDropDownButton.DropDownItems.Clear();
            if (arguments != null)
            {
                argsToolStripDropDownButton.DropDownItems.Clear();
                ToolStripItem argumentToolStrip;
                foreach (var item in arguments)
                {
                    argumentToolStrip = new ToolStripMenuItem();
                    argumentToolStrip.Text = String.Format("[{1}] {0}", item.Name, item.ParameterAccessor);
                    argumentToolStrip.Tag = item;
                    argumentToolStrip.Click += new EventHandler(argumentToolStrip_Click);
                    argsToolStripDropDownButton.DropDownItems.Add(argumentToolStrip);
                }
            }
            argsToolStripDropDownButton.Visible = argsToolStripDropDownButton.DropDownItems.Count > 0;
        }

        private void InitializeFormula()
        {
            snippetToolStripDropDownButton.Visible = false;
            showImageToolStripButton.Visible = false;

            splitContainer1.Panel2Collapsed = true;

            rtbFormula.ShowCodeImages = showImageToolStripButton.Checked;
            rtbFormula.AllowDrop = true;
            rtbFormula.DragEnter += new DragEventHandler(rtbFormula_DragEnter);
            rtbFormula.DragDrop += new DragEventHandler(rtbFormula_DragDrop);

            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Nothing));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.First));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Last));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Average));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Sum));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Maximum));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Minimum));
            cbxAggregation.Items.Add(AggregFormat(CalcAggregation.Count));
            cbxAggregation.SelectedIndex = 0;
        }

        void argumentToolStrip_Click(object sender, EventArgs e)
        {
            CalcArgumentInfo parameter;
            ToolStripItem argumentItem = sender as ToolStripItem;
            if (argumentItem != null && (parameter = argumentItem.Tag as CalcArgumentInfo) != null)
            {
                rtbFormula.SelectedText = parameter.Name;
            }
        }

        void constItem_Click(object sender, EventArgs e)
        {
            rtbFormula.SelectedText = (sender as ToolStripItem).Text;
        }


        void rtbFormula_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
            {
                UnitTreeNode cont = ((UnitTreeNode)e.Data.GetData(typeof(UnitTreeNode)));
                if (cont.Node is ParameterNode || cont.Node is NormFuncNode)
                {
                    e.Effect = DragDropEffects.Copy;
                    AddParameter(cont.Node);
                }
                else e.Effect = DragDropEffects.None;
                e.Data.SetData("");
            }
        }

        void rtbFormula_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Текущая формула
        /// </summary>
        public string Formula
        {
            get { return formula; }
            set { formula = value; UpdateFormula(); }
        }

        private string AggregFormatC(CalcAggregation aggreg)
        {
            switch (aggreg)
            {
                case CalcAggregation.Average:
                    return "avg";
                case CalcAggregation.First:
                    return "first";
                case CalcAggregation.Last:
                    return "last";
                case CalcAggregation.Maximum:
                    return "maxp";
                case CalcAggregation.Minimum:
                    return "minp";
                case CalcAggregation.Nothing:
                    return "";
                case CalcAggregation.Sum:
                    return "sum";
                case CalcAggregation.Count:
                    return "count";
                default:
                    return "";
            }
        }
        private string AggregFormat(CalcAggregation aggreg)
        {
            switch (aggreg)
            {
                case CalcAggregation.Average:
                    return "Среднее";
                case CalcAggregation.First:
                    return "Первое";
                case CalcAggregation.Last:
                    return "Последнее";
                case CalcAggregation.Maximum:
                    return "Максимум";
                case CalcAggregation.Minimum:
                    return "Минимум";
                case CalcAggregation.Nothing:
                    return "---";
                case CalcAggregation.Sum:
                    return "Сумма";
                case CalcAggregation.Count:
                    return "Количество значений";
                default:
                    return "---";
            }
        }
        private CalcAggregation AggregFormat(string aggreg)
        {
            switch (aggreg)
            {
                case "Среднее":
                    return CalcAggregation.Average;
                case "Первое":
                    return CalcAggregation.First;
                case "Последнее":
                    return CalcAggregation.Last;
                case "Максимум":
                    return CalcAggregation.Maximum;
                case "Минимум":
                    return CalcAggregation.Minimum;
                case "---":
                    return CalcAggregation.Nothing;
                case "Сумма":
                    return CalcAggregation.Sum;
                case "Количество значений":
                    return CalcAggregation.Count;
                default:
                    return CalcAggregation.Nothing;
            }
        }

        private void UpdateFormula()
        {
            rtbFormula.Text = formula;
            rtbFormula.ClearUndo();
            rtbFormula.TextChanged -= new EventHandler(rtbFormula_TextChanged);
            rtbFormula.TextChanged += new EventHandler(rtbFormula_TextChanged);
        }

        void funcMenu_Click(object sender, EventArgs e)
        {
            ToolStripItem menu = sender as ToolStripItem;
            FunctionInfo func;

            if (menu != null && (func = menu.Tag as FunctionInfo) != null)
            {
                String str = String.Empty;
                try
                {
                    //for (int i = 0; i < func.Types.Length; i++)
                    //{
                    //    if (i > 0) str += ", ";
                    //    switch (func.Types[i].Name)
                    //    {
                    //        case "Double":
                    //            str += "0.0";
                    //            break;
                    //        case "String":
                    //            str += "\"\"";
                    //            break;
                    //        case "Int32":
                    //            str += "0";
                    //            break;
                    //    }
                    //}
                }
                catch { str = ""; }
                rtbFormula.SelectedText = func.Name + "(" + str + ")";
            }
        }

        private string FormatType(Type type)
        {
            switch (type.Name)
            {
                case "Double":
                    return "Вещественное";
                case "String":
                    return "Строка";
                case "Int32":
                    return "Целое";
                default:
                    return "N/A";
            }
        }

        /// <summary>
        /// Добавление в тексте всем \n по \r
        /// </summary>
        /// <returns></returns>
        private string CorrectText(string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in text)
            {
                switch (item)
                {
                    case '\r':
                        break;
                    case '\n':
                        sb.AppendLine();
                        break;
                    default:
                        sb.Append(item);
                        break;
                }
            }

            return sb.ToString();
        }

        private void rtbFormula_TextChanged(object sender, EventArgs e)
        {
            FormulaUnitProvider formulaProvider;
            if (Enabled && (formulaProvider=UnitProvider as FormulaUnitProvider) != null)
                formulaProvider.Formula = rtbFormula.Text;
        }

        private async void CheckFormula()
        {
            FormulaUnitProvider formulaProvider;
            if ((formulaProvider=UnitProvider as FormulaUnitProvider) != null)
            {
                calcMessageViewControl1.ClearMessage();
                //formulaProvider.CheckFormula(MessageReceived, CheckFinish);
                var messages = await Task.Factory.StartNew<Message[]>(formulaProvider.CheckFormula);
                MessageReceived(messages);
                CheckFinish();
            }
        }

        private void MessageReceived(Message[] res)
        {
            if (InvokeRequired) Invoke((Action)(() => MessageReceived(res)));
            else
            {
                if (res != null && res.Length > 0)
                    calcMessageViewControl1.AddMessage(res);

                splitContainer1.Panel2Collapsed = calcMessageViewControl1.Count == 0;
                rtbFormula.Invalidate();
                //if (calcMessageViewControl1.Count == 0) MessageBox.Show("Формула верна", "Проверка формулы");
            }
        }

        private void CheckFinish()
        {
            if (InvokeRequired) Invoke((Action)CheckFinish);
            else if (calcMessageViewControl1.Count == 0) MessageBox.Show("Формула верна", "Проверка формулы");
        }

        private void AddParameter(UnitNode node)
        {
            CalcAggregation aggreg;
            string txt;

            if (node == null) return;

            if (string.IsNullOrEmpty(node.Code))
            {
                MessageBox.Show("У параметра отсутствует код");
                return;
            }

            if (node is ParameterNode)
            {
                aggreg = AggregFormat(cbxAggregation.Text);
                txt = "$" + node.Code + "$";
                if (aggreg != CalcAggregation.Nothing)
                {
                    txt = AggregFormatC(aggreg) + "(" + txt + ")";
                }
                rtbFormula.SelectedText = txt;
            }
            else
                if (node is NormFuncNode)
                {
                    NormFuncNode func = (NormFuncNode)node;
                    StringBuilder sb = new StringBuilder();
                    MultiDimensionalTable mdt = func.GetMDTable(UnitProvider.CurrentRevision);
                    foreach (var item in mdt.DimensionInfo)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append("[");
                        sb.Append(item.Name);
                        sb.Append("]");
                    }
                    txt = func.Code + "(" + sb.ToString() + ")";
                    rtbFormula.SelectedText = txt;
                    //MessageBox.Show("Извините, пока не реализовано.");
                    //return;
                }
        }

        private void rtbFormula_KeyPress(object sender, KeyPressEventArgs e)
        {
            int line;

            if (e.KeyChar == 13)
            {
                line = rtbFormula.GetLineFromCharIndex(rtbFormula.SelectionStart);
            }
        }

        private void rtbFormula_KeyUp(object sender, KeyEventArgs e)
        {
            int line;

            if (e.KeyCode == Keys.OemSemicolon)
            {
                line = rtbFormula.GetLineFromCharIndex(rtbFormula.SelectionStart);
            }
        }

        private async void rtbFormula_MouseMove(object sender, MouseEventArgs e)
        {
            string str;

            mouseLocation = e.Location;

            str = await GetToolTip();
            if (string.IsNullOrEmpty(str))
            {
                ttText.SetToolTip(rtbFormula, "");
            }
            else
            {
                if (str != strToolTip)
                    ttText.SetToolTip(rtbFormula, str);
            }
            strToolTip = str;
        }

        private async Task<string> GetToolTip()
        {
            ParameterNode node;
            int pos;
            string text = "";
            FunctionInfo function;

            currentParam = null;
            currentFunction = null;

            if (rtbFormula.Text == "") return "";

            pos = rtbFormula.GetCharIndexFromPosition(mouseLocation);

            FormulaTextBox.TextRepresentation repr = rtbFormula.GetTextRepresentationAtPoint(mouseLocation);
            if (repr != null)
            {
                switch (repr.RepresentationType)
                {
                    case FormulaTextBox.ElementType.Code:
                        node = await Task.Factory.StartNew(() => unitProvider.StructureProvider.GetUnitNode(repr.Value.Replace("$", ""))) as ParameterNode;
                        currentParam = node;

                        if (node != null)
                            text = node.FullName;
                        break;
                    case FormulaTextBox.ElementType.Function:
                        if (funcs.TryGetValue(repr.Value, out function))
                        {
                            currentFunction = function;
                            text = function.Comment;
                            if (currentFunction.Arguments != null && currentFunction.Arguments.Length > 0)
                            {
                                text += "\nАргументы: ";
                                for (int i = 0; i < currentFunction.Arguments.Length; i++)
                                {
                                    if (i > 0) text += ", ";
                                    text += currentFunction.Arguments[i].Name;
                                    if (!String.IsNullOrEmpty(currentFunction.Arguments[i].DefaultValue))
                                        text += String.Format("={0}", currentFunction.Arguments[i].DefaultValue);
                                }
                            }
                        }
                        break;
                }
            }
            return text;
        }

        private void showRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRelations(this, currentParam, FormulaRelation.Reference);
        }

        private void showDepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRelations(this, currentParam, FormulaRelation.Dependence);
        }

        private void ShowRelations(FormulaEditControl formulaEditControl, ParameterNode currentParam, FormulaRelation formulaRelation)
        {
            TepForm form = new TepForm(unitProvider.StructureProvider, Program.MainForm.Icons);
            Program.MainForm.AddExtendForm(form);
            form.Node = currentParam;
            form.Relation = formulaRelation;
            form.TopMost = true;
            //form.ShowDialog();
            form.Show();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (currentFunction != null
                || (currentParam != null && currentParam.Typ == (int)UnitTypeId.ManualParameter))
                showDepToolStripMenuItem.Visible = false;
            else showDepToolStripMenuItem.Visible = true;

            if (currentParam == null && currentFunction == null)
                e.Cancel = true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            CheckFormula();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            rtbFormula.Cut();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            rtbFormula.Copy();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            rtbFormula.Paste();
        }
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5:
                    CheckFormula();
                    return true;
            }
            return false;
        }

        private void showImageToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            rtbFormula.ShowCodeImages = showImageToolStripButton.Checked;
        }

        private int GetPosition(string text, int line, int col)
        {
            int pos = 0, work_line = line;
            while (--work_line > 0 && pos >= 0) { pos = text.IndexOf("\r\n", pos) + "\r\n".Length; }
            if (pos >= 0)
            {
                pos += col;
                pos -= line - 1;
            }
            return pos;
        }

        private async void saveToolStripButton_Click(object sender, EventArgs e)
        {
            FormulaUnitProvider formulaProvider;
            if ((formulaProvider = UnitProvider as FormulaUnitProvider) != null)
            {
                calcMessageViewControl1.ClearMessage();
                //formulaProvider.CheckFormula(MessageReceived, PreSave);
                MessageReceived(await Task.Factory.StartNew<Message[]>(formulaProvider.CheckFormula));

                if (calcMessageViewControl1.Count == 0
                    || MessageBox.Show("В формуле присутсвуют ошибки, все равно сохранить?", "Сохранение формулы", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => unitProvider.Save(), true));
                }
            }
        }

        //private void PreSave()
        //{
        //    DialogResult save = DialogResult.Yes;
        //    if (calcMessageViewControl1.Count > 0) 
        //        save = MessageBox.Show("В формуле присутсвуют ошибки, все равно сохранить?", "Сохранение формулы", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //    if (save == DialogResult.Yes) unitProvider.Save();
        //}

        #region IDisposable Members

        //void IDisposable.Dispose()
        //{
        //    if (UnitProvider != null)
        //    {
        //        UnitProvider.FunctionsRetrieved -= new Action<FunctionInfo[]>(SetFunctionList);
        //        UnitProvider.ConstsRetrieved -= new Action<ConstsInfo[]>(SetConstsList);
        //        UnitProvider.ArgumentsRetrieved -= new Action<CalcArgumentInfo[]>(SetArgumentList);
        //    }
        //}

        #endregion

        private async void btnLock_Click(object sender, EventArgs e)
        {
            await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(unitProvider.Lock, true));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UnitProvider.ClearUnsavedData();
        }

        private void revisionToolStripComboBox_DropDown(object sender, EventArgs e)
        {
            Object selectedObject = revisionToolStripComboBox.SelectedItem;

            revisionToolStripComboBox.Items.Clear();

            foreach (var revision in unitProvider.StructureProvider.Session.Revisions)
            {
                revisionToolStripComboBox.Items.Add(revision);
            }

            revisionToolStripComboBox.SelectedItem = selectedObject;
        }

        private void revisionToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RevisionInfo revision = revisionToolStripComboBox.SelectedItem as RevisionInfo;

            //if (revision != null)
            //{
            //    unitProvider.StructureProvider.CurrentRevision = revision;
            //}
        }
    }
}
