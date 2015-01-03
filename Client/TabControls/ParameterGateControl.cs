using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore.Utils;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Компонент, для отображения ручного ввода и расчета
    /// </summary>
    partial class ParameterGateControl : BaseUnitControl
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        DateTimePicker dateTimePicker;

        //public bool CanLockValues { get; private set; }

        public bool LockAlways { get; set; }

        public bool LockRequired
        {
            get
            {
                return LockAlways && UnitNode.Typ == (int)UnitTypeId.ManualGate;
            }
        }

        public ParameterGateControl(ParameterGateUnitProvider parameterProvider)
            : base(parameterProvider)
        {
            //LockAlways = true;
            //CanLockValues = parameterProvider.StructureProvider.CheckAccess(parameterProvider.UnitNode, Privileges.Execute);
            LockAlways = parameterProvider.StructureProvider.CheckAccess(parameterProvider.UnitNode, Privileges.Execute);

            InitializeComponent();
            this.SuspendLayout();
            toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(toolStripSeparator1) + 1, new ToolStripControlHost(dateTimePicker = new DateTimePicker()));
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "dd MMMM yyyy HH:mm:ss";
            this.dateTimePicker.Enabled = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(84, 29);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 2;
            this.dateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            this.dateTimePicker.DropDown += new System.EventHandler(this.dateTimePicker_DropDown);
            this.dateTimePicker.CloseUp += new System.EventHandler(this.dateTimePicker_CloseUp);

            paramNameColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            paramCodeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            changeTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;

            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, manualGateDataGridView, new object[] { true });
            this.ResumeLayout();
            this.PerformLayout();
            CreateHandle();
        }

        bool evented = false;
        public override void InitiateProcess()
        {
            ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;

            if (parameterProvider != null && !initiated)
            {
                String timeFormat = "dd MMMM yyyy ";
                Interval interval = parameterProvider.Interval;

                if (interval > Interval.Month)
                {
                    timeFormat = "MMMM yyyy";
                }
                else if (interval < Interval.Day)
                {
                    timeFormat += "HH";
                    if (interval < Interval.Hour)
                    {
                        timeFormat += ":mm";
                        if (interval < Interval.Minute)
                        {
                            timeFormat += ":ss";
                        }
                    }
                }

                dateTimePicker.CustomFormat = timeFormat;
                tsbPrevTime.Enabled = tsbNextTime.Enabled =
                    dateTimePicker.Enabled = interval != Interval.Zero;
                paramTimeColumn.Visible = interval == Interval.Zero;

                if (interval != Interval.Zero)
                {
                    prevValueCountToolStripMenuItem.Visible =
                        prevValueCountToolStripTextBox.Visible = true;
                }

                if (!evented)
                {
                    evented = true;

                    parameterProvider.QueryTimeChanged += new EventHandler(parameterProvider_QueryTimeChanged);
                    parameterProvider.ParameterListChanged += new EventHandler(parameterProvider_ParameterListChanged);
                    parameterProvider.ParameterCodeImageChanged += new EventHandler(parameterProvider_ParameterCodeImageChanged);
                    parameterProvider.ParameterValuesChanged += new EventHandler(parameterProvider_ParameterValuesChanged);
                    parameterProvider.PrevValueColumnCountChanged += new EventHandler(parameterProvider_PrevValueColumnCountChanged);
                    parameterProvider.UseMimeTexChanged += new EventHandler(parameterProvider_UseMimeTexChanged);
                    parameterProvider.ValuesEditModeChanged += new EventHandler(parameterProvider_ValuesEditModeChanged);
                }

                tsbLock.Visible = unitProvider.StructureProvider.CheckAccess(UnitProvider.UnitNode, Privileges.Execute) && !LockAlways;

                tsbMimeTex.Checked = parameterProvider.UseMimeTex;
                parameterProvider_QueryTimeChanged(parameterProvider, EventArgs.Empty);

                parameterProvider_UseMimeTexChanged(parameterProvider, EventArgs.Empty);

                //parameterProvider_PrevValueColumnCountChanged(parameterProvider, EventArgs.Empty);

                //parameterProvider_ParameterListChanged(parameterProvider, EventArgs.Empty);

                //parameterProvider_ParameterValuesChanged(parameterProvider, EventArgs.Empty);

                //parameterProvider_ValuesEditModeChanged(parameterProvider, EventArgs.Empty);

                // скорректировать колонки
                ChangePrevColumns();

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //await Task.Factory.StartNew(() => parameterProvider.StartProvider());
                        parameterProvider.StartProvider();
                    }
                    catch (Exception ex)
                    {
                        log.WarnException("Ошибка инициализации окна просмотра значений.", ex);
                        UniForm.ShowError(ex);
                    }
                });
                initiated = true;
            }
        }

        void parameterProvider_ValuesEditModeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) BeginInvoke((EventHandler)parameterProvider_ValuesEditModeChanged, sender, e);
            else
            {
                ParameterGateUnitProvider parameterProvider = UnitProvider as ParameterGateUnitProvider;
                if (parameterProvider != null)
                {
                    this.tsbLock.Enabled = !parameterProvider.ValuesEditMode;
                    this.tsbSave.Enabled = parameterProvider.ValuesEditMode;
                    this.tsbCancel.Enabled = parameterProvider.ValuesEditMode;
                }
                UpdateValuesGrid();
            }
        }

        void parameterProvider_QueryTimeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)parameterProvider_QueryTimeChanged, sender, e);
            else
            {
                ParameterGateUnitProvider parameterProvider;
                if ((parameterProvider = sender as ParameterGateUnitProvider) != null)
                {
                    if (parameterProvider.QueryTime >= dateTimePicker.MinDate
                        && parameterProvider.QueryTime <= dateTimePicker.MaxDate)
                        dateTimePicker.Value = parameterProvider.QueryTime;
                } 
            }
        }

        public override bool ForShow(UnitNode node)
        {
            // проверка по типу
            ParameterGateUnitProvider parameterProvider;

            if ((parameterProvider = unitProvider as ParameterGateUnitProvider) != null)
            {
                if ((parameterProvider.UnitNode.Typ == (int)UnitTypeId.TEPTemplate && node.Typ == (int)UnitTypeId.TEP)
                    || (parameterProvider.UnitNode.Typ == (int)UnitTypeId.ManualGate && node.Typ == (int)UnitTypeId.ManualParameter))
                    return true;
            }
            return base.ForShow(node);
        }

        UnitNode nodeToSelect;

        public override void SelectNode(UnitNode unitNode)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((Action<UnitNode>)SelectNode, unitNode);
                else
                {
                    ParameterNode parameter;
                    lock (manualGateDataGridView)
                    {
                        // костыль что бы выделить узел после загрузки узлов
                        if (manualGateDataGridView.Rows.Count == 0)
                            nodeToSelect = unitNode;
                        foreach (DataGridViewRow gridRow in manualGateDataGridView.Rows)
                        {
                            if ((parameter = gridRow.Tag as ParameterNode) != null && parameter.Idnum == unitNode.Idnum)
                            {
                                manualGateDataGridView.CurrentCell = gridRow.Cells[paramValueColumn.Index];
                                break;
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        protected override void DisposeControl()
        {
            ParameterGateUnitProvider parameterProvider;
            if ((parameterProvider = unitProvider as ParameterGateUnitProvider) != null)
            {
                parameterProvider.QueryTimeChanged -= new EventHandler(parameterProvider_QueryTimeChanged);
                parameterProvider.ParameterListChanged -= new EventHandler(parameterProvider_ParameterListChanged);
                parameterProvider.ParameterCodeImageChanged -= new EventHandler(parameterProvider_ParameterCodeImageChanged);
                parameterProvider.ParameterValuesChanged -= new EventHandler(parameterProvider_ParameterValuesChanged);
                parameterProvider.PrevValueColumnCountChanged -= new EventHandler(parameterProvider_PrevValueColumnCountChanged);
                parameterProvider.UseMimeTexChanged -= new EventHandler(parameterProvider_UseMimeTexChanged);
                parameterProvider.ValuesEditModeChanged -= new EventHandler(parameterProvider_ValuesEditModeChanged);
            }
            base.DisposeControl();
        }
        bool valueChangeIgnore;
        void parameterProvider_ParameterListChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)parameterProvider_ParameterListChanged, sender, e);
                else
                {
                    try
                    {
                        ParameterNode selectedParameter = null;
                        int selectedColumn = 0;

                        valueChangeIgnore = true;
                        ParameterNode[] nodes = ((ParameterGateUnitProvider)unitProvider).GetParameterNodes();
                        DataGridViewRow gridRow;

                        // сохранение текущей выделенной ячейки
                        if (manualGateDataGridView.CurrentCell != null)
                        {
                            selectedParameter = manualGateDataGridView.Rows[manualGateDataGridView.CurrentCell.RowIndex].Tag as ParameterNode;
                            selectedColumn = manualGateDataGridView.CurrentCell.ColumnIndex;
                        }

                        lock (manualGateDataGridView)
                        {
                            manualGateDataGridView.Rows.Clear();
                            foreach (ParameterNode parameter in nodes)
                            {
                                gridRow = manualGateDataGridView.Rows[manualGateDataGridView.Rows.Add()];
                                gridRow.Tag = parameter;
                                gridRow.Cells[paramNameColumn.Index].Value = parameter.GetNodeText();
                                gridRow.Cells[paramCodeColumn.Index].Value = parameter.Code;
                                gridRow.Cells[clmCodeImage.Index].Value = Properties.Resources.tep;
                                gridRow.Cells[clmCodeImage.Index].ToolTipText = parameter.Code;
                                if (selectedParameter != null && selectedParameter.Idnum == parameter.Idnum)
                                {
                                    manualGateDataGridView.CurrentCell = gridRow.Cells[selectedColumn];
                                }
                            }
                        }

                        // костыль что бы выделить узел после загрузки узлов
                        if (nodeToSelect != null)
                        {
                            SelectNode(nodeToSelect);
                            nodeToSelect = null;
                        }
                        parameterProvider_ParameterCodeImageChanged(sender, e);
                    }
                    finally
                    {
                        valueChangeIgnore = false;
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        void parameterProvider_ParameterCodeImageChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)parameterProvider_ParameterCodeImageChanged, sender, e);
                else
                {
                    ParameterNode parameter;
                    lock (manualGateDataGridView)
                    {
                        foreach (DataGridViewRow gridRow in manualGateDataGridView.Rows)
                        {
                            if ((parameter = gridRow.Tag as ParameterNode) != null)
                            {
                                gridRow.Cells[clmCodeImage.Index].Value =
                                    ((ParameterGateUnitProvider)unitProvider).GetParameterCodeImage(parameter);
                            }
                        }
                    }
                    SetCodeColumnWidth();
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        async void parameterProvider_ParameterValuesChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke((Action)(async () => { await ShowParameterValues(); }));
            else
            {
                await ShowParameterValues();
            }
        }

        private async Task ShowParameterValues()
        {
            UpdateValuesGrid();
            if (LockRequired)
            {
                ParameterGateUnitProvider parameterProvider = UnitProvider as ParameterGateUnitProvider;
                LockActionArgs args = (LockActionArgs)await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(parameterProvider.LockValues, true));
                //if (args.Success)
                //{
                //    await ShowParameterValues();
                //}
            }
        }

        private void UpdateValuesGrid()
        {
            String columnName;
            ParameterNode parameter;
            ParamValueItem receiveItem;
            ParameterGateUnitProvider parameterProvider;

            if ((parameterProvider = unitProvider as ParameterGateUnitProvider) != null)
            {
                bool readOnly = !parameterProvider.ValuesEditMode //|| unitProvider.ReadOnly
                    || !unitProvider.StructureProvider.CheckAccess(unitProvider.UnitNode, Privileges.Write | Privileges.Execute);
                // can calc parameters
                bool calc = unitProvider.UnitNode.Typ == (int)UnitTypeId.TEPTemplate;
                bool canCalc = calc && unitProvider.StructureProvider.CheckAccess(UnitProvider.UnitNode, Privileges.Execute);

                // Обновить панель
                UpdateSaveButton(parameterProvider);
                paramValueColumn.ReadOnly = calc || readOnly;
                showDependensToolStripMenuItem.Visible = calc;
                beginValueColumn.Visible = calc;
                tsbCalc.Visible = canCalc;

                // обновить настройки по умолчанию
                if (calc || readOnly)
                {
                    paramValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;
                    paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
                }
                else
                {
                    paramValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledValueCellStyle;
                    ParameterGateNode gateNode = unitProvider.UnitNode as ParameterGateNode;
                    if (gateNode != null && gateNode.Typ == (int)UnitTypeId.ManualGate && gateNode.Interval == Interval.Zero)
                        paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledCellStyle;
                    else
                        paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
                }

                // обновить для каждой строчке в таблице
                foreach (DataGridViewRow gridRow in manualGateDataGridView.Rows)
                {
                    if ((parameter = gridRow.Tag as ParameterNode) != null)
                    {
                        receiveItem = parameterProvider.GetParameterValue(parameter, 0);

                        gridRow.Cells[paramValueColumn.Index].ReadOnly = paramValueColumn.ReadOnly;
                        if (parameterProvider.IsLocked(parameter))
                        {
                            gridRow.Cells[paramValueColumn.Index].ReadOnly = true;
                            gridRow.Cells[paramValueColumn.Index].Style = ValuesControlSettings.Instance.DisabledValueCellStyle;
                            gridRow.Cells[paramTimeColumn.Index].ReadOnly = true;
                            gridRow.Cells[paramTimeColumn.Index].Style = ValuesControlSettings.Instance.DefaultCellStyle;
                        }
                        else if (parameterProvider.IsCorrecting(parameter))
                        {
                            gridRow.Cells[paramValueColumn.Index].Style = ValuesControlSettings.Instance.EnabledValueCellStyle;
                            gridRow.Cells[paramTimeColumn.Index].Style = ValuesControlSettings.Instance.DisabledValueCellStyle;
                            gridRow.Cells[paramValueColumn.Index].ReadOnly = false;
                        }
                        else if (parameterProvider.IsCorrected(parameter, 0))
                            gridRow.Cells[paramValueColumn.Index].Style = ValuesControlSettings.Instance.ValueCorrectedCellStyle;
                        else
                        {
                            gridRow.Cells[paramValueColumn.Index].Style = paramValueColumn.DefaultCellStyle;
                            gridRow.Cells[paramTimeColumn.Index].ReadOnly = paramTimeColumn.ReadOnly;
                            gridRow.Cells[paramTimeColumn.Index].Style = paramTimeColumn.DefaultCellStyle;
                        }

                        if (receiveItem != null)
                        {
                            if (receiveItem is CorrectedParamValueItem)
                            {
                                gridRow.Cells[beginValueColumn.Index].Value = (receiveItem as CorrectedParamValueItem).OriginalValueItem.Value;
                            }
                            else
                                gridRow.Cells[beginValueColumn.Index].Value = double.NaN;// receiveItem.Value;

                            gridRow.Cells[paramValueColumn.Index].Value = receiveItem.Value;
                            gridRow.Cells[paramValueColumn.Index].ToolTipText = GetParamValueItemTip(receiveItem);

                            gridRow.Cells[paramTimeColumn.Index].Value = receiveItem.Time;
                            gridRow.Cells[changeTimeColumn.Index].Value = receiveItem.ChangeTime;
                        }
                        else
                        {
                            gridRow.Cells[beginValueColumn.Index].Value = double.NaN;// receiveItem.Value;
                            gridRow.Cells[paramValueColumn.Index].Value = double.NaN;

                            gridRow.Cells[paramTimeColumn.Index].Value = null;
                            gridRow.Cells[changeTimeColumn.Index].Value = null;
                        }

                        // обновить значения для колонок с предыдущими значениями
                        for (uint i = 0; i < parameterProvider.PrevValueColumnCount; i++)
                        {
                            receiveItem = parameterProvider.GetParameterValue(parameter, i + 1);
                            columnName = String.Format(PrevValueColumnNameFomat, i);

                            if (manualGateDataGridView.Columns.Contains(columnName))
                            {
                                DataGridViewCell gridCell = gridRow.Cells[columnName];

                                // выставляем стиль для скорректированных колонок
                                if (parameterProvider.IsCorrected(parameter, i + 1))
                                    gridCell.Style = ValuesControlSettings.Instance.ValueCorrectedCellStyle;
                                else
                                    gridCell.Style = ValuesControlSettings.Instance.DisabledValueCellStyle;

                                // выставляем значение
                                if (receiveItem != null && manualGateDataGridView.Columns.Contains(columnName))
                                {
                                    gridCell.Value = receiveItem.Value;
                                    gridCell.ToolTipText = GetParamValueItemTip(receiveItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSaveButton(ParameterGateUnitProvider parameterProvider)
        {
            bool readOnly = !parameterProvider.ValuesEditMode 
                      || !unitProvider.StructureProvider.CheckAccess(unitProvider.UnitNode, Privileges.Write | Privileges.Execute);
            bool calc = unitProvider.UnitNode.Typ == (int)UnitTypeId.TEPTemplate;

            // Обновить панель
            tsbSave.Enabled = !readOnly  || (calc && parameterProvider.HasChanges);
            tsbCancel.Enabled = !readOnly || (calc && parameterProvider.HasChanges);
        }

        void parameterProvider_UseMimeTexChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)parameterProvider_UseMimeTexChanged, sender, e);
                else
                {
                    ParameterGateUnitProvider parameterProvider = sender as ParameterGateUnitProvider;

                    if (parameterProvider != null)
                    {
                        manualGateDataGridView.Columns[paramCodeColumn.Name].Visible = !parameterProvider.UseMimeTex;
                        manualGateDataGridView.Columns[clmCodeImage.Name].Visible = parameterProvider.UseMimeTex;
                        SetCodeColumnWidth();
                    }
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        const String PrevValueColumnNameFomat = "PrevParamValue{0}";

        List<DataGridViewColumn> prevColumnList = new List<DataGridViewColumn>();

        void parameterProvider_PrevValueColumnCountChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    Invoke((EventHandler)parameterProvider_PrevValueColumnCountChanged, sender, e);
                else
                {
                    ChangePrevColumns();
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        private void ChangePrevColumns()
        {
            ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;

            if (prevColumnList.Count != parameterProvider.PrevValueColumnCount)
            {
                prevValueCountToolStripTextBox.Text = parameterProvider.PrevValueColumnCount.ToString();

                foreach (DataGridViewColumn item in prevColumnList)
                {
                    manualGateDataGridView.Columns.Remove(item);
                }
                prevColumnList.Clear();

                if (parameterProvider.Interval != Interval.Zero)
                {
                    for (int i = 0; i < parameterProvider.PrevValueColumnCount; i++)
                    {
                        String columnName = String.Format(PrevValueColumnNameFomat, i);
                        if (!manualGateDataGridView.Columns.Contains(columnName))
                        {
                            DataGridViewColumn column = new DataGridViewTextBoxColumn();
                            column.Name = columnName;
                            column.DataPropertyName = columnName;
                            column.ReadOnly = true;
                            column.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;
                            manualGateDataGridView.Columns.Insert(paramCodeColumn.Index + 1, column);
                            prevColumnList.Add(column);
                        }
                    }
                }
                foreach (DataGridViewColumn gridColumn in manualGateDataGridView.Columns)
                {
                    gridColumn.DisplayIndex = gridColumn.Index;
                }
                SetPrevValuesColumnHeaders();
            }
        }

        private void manualGateDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            ValuesControlSettings.Instance.FormatParameterValue(e, manualGateDataGridView.Rows[e.RowIndex].Tag as ParameterNode);
        }

        private void manualGateDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewRow gridRow = manualGateDataGridView.Rows[e.RowIndex];
            ParameterNode param = gridRow.Tag as ParameterNode;
            if (param != null)
            {
                if (((ParameterGateUnitProvider)unitProvider).Modified(param))
                {
                    manualGateDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(manualGateDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                }
                else
                    manualGateDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(manualGateDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ParameterGateUnitProvider parameterGateProvider;

            if (manualGateDataGridView.CurrentCell != null)
                manualGateDataGridView_RowValidated(sender, new DataGridViewCellEventArgs(manualGateDataGridView.CurrentCell.ColumnIndex, manualGateDataGridView.CurrentCell.RowIndex));
            if ((parameterGateProvider = unitProvider as ParameterGateUnitProvider) != null)
            {
                parameterGateProvider.SaveValues();
            }
            UpdateValuesGrid();
        }

        private void tsbCalc_Click(object sender, EventArgs e)
        {
            ParameterGateUnitProvider parameterGateProvider;

            if ((parameterGateProvider = unitProvider as ParameterGateUnitProvider) != null)
            {
                CalcForm calcForm = new CalcForm(unitProvider.StructureProvider, Program.MainForm.Icons);
                Program.MainForm.AddExtendForm(calcForm);

                calcForm.TimeBegin = parameterGateProvider.QueryTime;
                if (parameterGateProvider.Interval != Interval.Zero)
                    calcForm.TimeEnd = parameterGateProvider.Interval.GetNextTime(calcForm.TimeBegin);
                calcForm.Interval = parameterGateProvider.Interval;
                calcForm.Parameter = unitProvider.UnitNode;
                calcForm.CalcFinished += new EventHandler(((ParameterGateUnitProvider)unitProvider).CalcForm_CalcFinished);
                calcForm.Show(this);
            }
        }

        private void prevTimeButton_Click(object sender, EventArgs e)
        {
            TimeChange(((ParameterGateUnitProvider)unitProvider).GetPrevTime());
        }

        /// <summary>
        /// Обновить заголовки колонок с предыдущим значениями в зависимости с текущим временем
        /// </summary>
        private void SetPrevValuesColumnHeaders()
        {
            ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;

            if (parameterProvider.QueryTime > DateTime.MinValue)
            {
                dateTimePicker.Value = parameterProvider.QueryTime;

                for (int i = 0; i < prevColumnList.Count; i++)
                {
                    manualGateDataGridView.Columns[prevColumnList[i].Name].HeaderText =
                        parameterProvider.Interval.GetTime(parameterProvider.QueryTime, -i - 1).ToString(dateTimePicker.CustomFormat);
                }
                manualGateDataGridView.Refresh();
            }
        }

        private void nextTimeButton_Click(object sender, EventArgs e)
        {
            TimeChange(((ParameterGateUnitProvider)unitProvider).GetNextTime());
        }

        private void manualGateDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (manualGateDataGridView.Columns[e.ColumnIndex].Name.Equals(paramTimeColumn.Name))
            {
                MessageBox.Show("Неверный формат времени", "Неверный формат времени", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show(e.Exception.Message, "Неверный формат", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }

        private void manualGateDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (manualGateDataGridView.Columns[e.ColumnIndex].Name.Equals(paramTimeColumn.Name))
            {
                try
                {
                    Convert.ToDateTime(manualGateDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                }
                catch
                {
                    MessageBox.Show("Неверный формат времени", "Неверный формат времени", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void manualGateDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            ParameterGateUnitProvider parameterProvider;

            if (valueChangeIgnore) return;
            if ((parameterProvider = UnitProvider as ParameterGateUnitProvider) != null 
                && parameterProvider.Received)
            {
                DataGridViewRow gridRow = manualGateDataGridView.Rows[e.RowIndex];
                ParameterNode param = gridRow.Tag as ParameterNode;
                if (param != null)
                {
                    double val;
                    DateTime time;

                    if (!parameterProvider.ValuesEditMode
                        || (parameterProvider.UnitNode.Typ != (int)UnitTypeId.ManualGate
                        && !parameterProvider.IsCorrecting(param)))
                        return;

                    if (gridRow.Cells[paramValueColumn.Index].Value == null
                        || (!double.TryParse(gridRow.Cells[paramValueColumn.Index].Value.ToString(),
                                            System.Globalization.NumberStyles.Float,
                                            System.Globalization.NumberFormatInfo.CurrentInfo,
                                            out val) // распарсить введенное значение при текущей культуре
                        && !double.TryParse(gridRow.Cells[paramValueColumn.Index].Value.ToString(),
                                            System.Globalization.NumberStyles.Float,
                                            System.Globalization.NumberFormatInfo.InvariantInfo,
                                            out val))) // если не получилось то по забугорной
                        val = double.NaN;

                    if (parameterProvider.Interval == Interval.Zero)
                    {
                        if (gridRow.Cells[paramTimeColumn.Index].Value != null)
                            time = DateTime.Parse(gridRow.Cells[paramTimeColumn.Index].Value.ToString());
                        else time = DateTime.Now;
                    }
                    else time = parameterProvider.QueryTime;

                    ParamValueItem receiveItem = parameterProvider.GetParameterValue(param, 0);
                    if (receiveItem != null || !double.IsNaN(val))
                    {
                        if (receiveItem == null)
                            receiveItem = new ParamValueItem();
                        if (param.Typ == (int)UnitTypeId.ManualParameter || receiveItem is CorrectedParamValueItem)
                        {
                            receiveItem.Value = val;

                            receiveItem.Quality = double.IsNaN(val) ? Quality.Bad : Quality.Good;
                            receiveItem.Time = time;
                        }
                        parameterProvider.SetParameterValue(param, receiveItem);
                        UpdateSaveButton(parameterProvider);
                    }

                    if (parameterProvider.Modified(param))
                    {
                        if (parameterProvider.Interval
                            == Interval.Zero && gridRow.Cells[paramTimeColumn.Index].Value ==null)//DateTime.MinValue.Equals(receiveItem.Time))
                            gridRow.Cells[paramTimeColumn.Index].Value = DateTime.Now;
                    }
                }
            }
        }

        private void prevValueCountToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int prevColumnCount = int.Parse(prevValueCountToolStripTextBox.Text);
                if (prevColumnCount != ((ParameterGateUnitProvider)unitProvider).PrevValueColumnCount)
                {
                    manualGateDataGridView.DataSource = null;
                    ((ParameterGateUnitProvider)unitProvider).PrevValueColumnCount = prevColumnCount;
                }
            }
            catch (FormatException)
            {
                prevValueCountToolStripTextBox.Text = ((ParameterGateUnitProvider)unitProvider).PrevValueColumnCount.ToString();
            }
        }

        bool dateTimePickerDroppedDown;
        private void dateTimePicker_DropDown(object sender, EventArgs e)
        {
            dateTimePickerDroppedDown = true;
        }

        private void dateTimePicker_CloseUp(object sender, EventArgs e)
        {
            dateTimePickerDroppedDown = false;
            dateTimePicker_ValueChanged(sender, e);
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (!dateTimePickerDroppedDown &&
                !((ParameterGateUnitProvider)unitProvider).QueryTime.Equals(dateTimePicker.Value))
            {
                TimeChange(dateTimePicker.Value);
            }
        }

        private void TimeChange(DateTime time)
        {
            try
            {
                UniForm uniForm = UniForm as UniForm;
                UnitProvider[] unitProviders;
                List<ParameterGateUnitProvider> parmeterProviderList = new List<ParameterGateUnitProvider>();

                if (uniForm != null)
                    unitProviders = uniForm.GetTabbedProviders();
                else unitProviders = new UnitProvider[] { UnitProvider };

                bool modified = false, save = false;
                foreach (UnitProvider provider in unitProviders)
                {
                    ParameterGateUnitProvider parameterProvider;
                    if ((parameterProvider = provider as ParameterGateUnitProvider) != null)
                    {
                        parmeterProviderList.Add(parameterProvider);
                        modified |= parameterProvider.HasChanges;
                    }
                }

                if (modified)
                {
                    DialogResult res = MessageBox.Show("Значения изменились. Сохранить изменения", "Сохранений значений", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                        save = true;
                    else if (res == DialogResult.Cancel)
                    {
                        dateTimePicker.Value = ((ParameterGateUnitProvider)unitProvider).QueryTime;
                        return;
                    }
                }
                foreach (ParameterGateUnitProvider parameterProvider in parmeterProviderList)
                {
                    if (save)
                        parameterProvider.SaveValues();
                    parameterProvider.RetrieveValues(time);
                }
                SetPrevValuesColumnHeaders();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса значений параметров.", exc);
                UniForm.ShowError(exc);
            }
        }

        private void showParameterValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualGateDataGridView.CurrentRow != null)
            {
                try
                {
                    ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;
                    ParameterNode node;
                    if (parameterProvider != null
                        && (node = manualGateDataGridView.CurrentRow.Tag as ParameterNode) != null)
                    {
                        ParameterValuesController valuesController = new ParameterValuesController(parameterProvider.StructureProvider, node, parameterProvider.QueryTime);
                        ParameterValuesEditorForm parameterValuesForm = new ParameterValuesEditorForm(parameterProvider.StructureProvider, valuesController);
                        Program.MainForm.AddExtendForm(parameterValuesForm);

                        parameterValuesForm.ValueSaved += new EventHandler(((ParameterGateUnitProvider)unitProvider).CalcForm_CalcFinished);
                        valuesController.AsyncForm = parameterValuesForm;
                        parameterValuesForm.ShowDialog();
                    }
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка запроса значений параметров.", exc);
                    UniForm.ShowError(exc);
                }
            }
        }

        private void showReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualGateDataGridView.CurrentRow != null)
            {
                try
                {
                    ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;
                    ParameterNode node;
                    if (parameterProvider != null
                        && (node = manualGateDataGridView.CurrentRow.Tag as ParameterNode) != null)
                    {
                        TepForm referenceForm = new TepForm(parameterProvider.StructureProvider, Program.MainForm.Icons);
                        Program.MainForm.AddExtendForm(referenceForm);
                        referenceForm.Node = node;
                        referenceForm.Relation = FormulaRelation.Reference;
                        referenceForm.CurrentTime = parameterProvider.QueryTime;
                        referenceForm.ReadOnly = unitProvider.ReadOnly;
                        referenceForm.TopMost = true;
                        referenceForm.Show();
                    }
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка запроса ссылок на параметр.", exc);
                    UniForm.ShowError(exc);
                }
            }
        }

        private void showDependensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualGateDataGridView.CurrentRow != null)
            {
                try
                {
                    ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;
                    ParameterNode node;
                    if (parameterProvider != null
                        && (node = manualGateDataGridView.CurrentRow.Tag as ParameterNode) != null)
                    {
                        TepForm referenceForm = new TepForm(parameterProvider.StructureProvider, Program.MainForm.Icons);
                        Program.MainForm.AddExtendForm(referenceForm);
                        referenceForm.Node = node;
                        referenceForm.Relation = FormulaRelation.Dependence;
                        referenceForm.CurrentTime = parameterProvider.QueryTime;
                        referenceForm.ReadOnly = unitProvider.ReadOnly;
                        referenceForm.TopMost = true;
                        referenceForm.Show();
                    }
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка запроса зависимостей параметра.", exc);
                    UniForm.ShowError(exc);
                }
            }
        }

        private void SetCodeColumnWidth()
        {
            if (((ParameterGateUnitProvider)unitProvider).UseMimeTex)
            {
                int codeColumnWidth = ((ParameterGateUnitProvider)unitProvider).CodeColumnWidth;
                if (manualGateDataGridView.Rows.Count > 0)
                {
                    int height = ((ParameterGateUnitProvider)unitProvider).RowHeight;
                    foreach (DataGridViewRow gridRow in manualGateDataGridView.Rows)
                    {
                        if (height > gridRow.MinimumHeight)
                            gridRow.MinimumHeight = height;
                    }
                }
                if (manualGateDataGridView.Columns.Contains(clmCodeImage.Name))
                {
                    if (0 < codeColumnWidth)
                        manualGateDataGridView.Columns[clmCodeImage.Name].Width = codeColumnWidth;
                    if (clmCodeImage.MinimumWidth < codeColumnWidth)
                        manualGateDataGridView.Columns[clmCodeImage.Name].MinimumWidth = codeColumnWidth;
                }
                clmCodeImage.Resizable = DataGridViewTriState.False;
            }
        }

        private void toCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = unitProvider.UnitNode.Text;

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (Stream s=saveFileDialog1.OpenFile())
                {
                    GridToCSV(s, manualGateDataGridView, new String[] { paramCodeColumn.Name }, new String[] { clmCodeImage.Name }); 
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ((ParameterGateUnitProvider)unitProvider).UseMimeTex = tsbMimeTex.Checked;
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;
            if (manualGateDataGridView.CurrentRow == null)
            {
                e.Cancel = true;
                return;
            }
            ParameterNode parameterNode = manualGateDataGridView.CurrentRow.Tag as ParameterNode;
            ParamValueItem receiveItem;
            bool correctVisible;

            correctVisible = parameterProvider.StructureProvider.CheckAccess(parameterProvider.UnitNode, Privileges.Execute)
                            && (parameterProvider = unitProvider as ParameterGateUnitProvider) != null
                            && (parameterNode = manualGateDataGridView.CurrentRow.Tag as ParameterNode) != null
                            && parameterNode.Typ == (int)UnitTypeId.TEP
                           ; //&& parameterProvider.ValuesEditMode; //!parameterProvider.ReadOnly;

            correctValueToolStripMenuItem.Visible = correctVisible;
            decorrectValueToolStripMenuItem.Visible = correctVisible
                && (receiveItem = parameterProvider.GetParameterValue(parameterNode, 0)) != null
                && receiveItem is CorrectedParamValueItem;
            correctToolStripMenuItem.Visible = correctVisible;
        }

        private async void correctValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow gridRow = manualGateDataGridView.CurrentRow;
            ParameterNode parameter;

            if ((parameter = gridRow.Tag as ParameterNode) != null)
            {
                await CorrectValue(gridRow, parameter);
            }
        }

        private async Task CorrectValue(DataGridViewRow gridRow, ParameterNode parameter)
        {
            try
            {
                ParameterGateUnitProvider parameterProvider;
                if ((parameterProvider = unitProvider as ParameterGateUnitProvider) != null)
                {
                    await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => parameterProvider.Correct(parameter), true));

                    UpdateValuesGrid();
                    manualGateDataGridView.CurrentCell = gridRow.Cells[paramValueColumn.Index];
                    manualGateDataGridView.BeginEdit(true);
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка корректирования значения параметра.", exc);
                UniForm.ShowError(exc);
            }
        }

        private void decorrectValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow gridRow = manualGateDataGridView.CurrentRow;
                ParameterGateUnitProvider parameterProvider;
                ParameterNode parameter;

                if ((parameterProvider = unitProvider as ParameterGateUnitProvider) != null
                    && (parameter = gridRow.Tag as ParameterNode) != null)
                {
                    parameterProvider.Deccorect(parameter);
                    UpdateValuesGrid();
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка корректирования значения параметра.", exc);
                UniForm.ShowError(exc);
            }
        }

        private async void tsbLock_Click(object sender, EventArgs e)
        {
            try
            {
                ParameterGateUnitProvider parameterProvider = UnitProvider as ParameterGateUnitProvider;
                LockActionArgs args = (LockActionArgs)await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(parameterProvider.LockValues, true));
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка взятия шаблона на редактирование значений.", exc);
                UniForm.ShowError(exc);
            }
        }

        private async void tsbCancel_Click(object sender, EventArgs e)
        {
            await ClearValues();
        }

        private async Task ClearValues()
        {
            try
            {
                ParameterGateUnitProvider parameterProvider = unitProvider as ParameterGateUnitProvider;

                await Task.Factory.StartNew(() => parameterProvider.ClearUnsavedValues());
                if (LockRequired)
                {
                    LockActionArgs args = (LockActionArgs)await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(parameterProvider.LockValues, true));
                    //if (args.Success)
                    //{
                    //    await ClearValues();
                    //}
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка сброса редактирования значений.", exc);
                UniForm.ShowError(exc);
            }
        }

        private string GetParamValueItemTip(ParamValueItem valueItem)
        {
            CorrectedParamValueItem correctedItem = valueItem as CorrectedParamValueItem;
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("Значение: {0}", LocalizeNaN(valueItem.Value));
            if (correctedItem != null)
            {
                if (!double.Equals(correctedItem.OriginalValueItem.Value, correctedItem.CorrectedValueItem.Value))
                    builder.AppendFormat("\nНачальное значение: {0}", LocalizeNaN(correctedItem.OriginalValueItem.Value));
                if (correctedItem.CorrectedValueItem.Time != DateTime.MinValue
                    && !DateTime.Equals(correctedItem.OriginalValueItem.Time, correctedItem.CorrectedValueItem.Time))
                    builder.AppendFormat("\nИзначальное время: {0}", correctedItem.OriginalValueItem.Time);
                if (!Object.Equals(correctedItem.OriginalValueItem.Arguments, correctedItem.CorrectedValueItem.Arguments))
                    builder.AppendFormat("\nИзначальные аргументы: {0}", correctedItem.OriginalValueItem.Arguments);
            }
            builder.AppendFormat("\nВремя изменения: {0}", valueItem.ChangeTime);
            return builder.ToString();
        }

        private string LocalizeNaN(double value)
        {
            if (double.IsNaN(value)) return "Нет";
            return value.ToString();
        }
    }

    class ValuesControlSettings : DoubleControlSettings
    {
        static ValuesControlSettings instance = null;

        public static ValuesControlSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new ValuesControlSettings();
                return instance;
            }
        }

        protected ValuesControlSettings()
        {
            DefaultCellStyle = new DataGridViewCellStyle();
            DefaultCellStyle.BackColor = Color.FromArgb(224, 224, 224);

            EnabledCellStyle = new DataGridViewCellStyle();
            EnabledCellStyle.BackColor = Color.White;

            EnabledValueCellStyle = new DataGridViewCellStyle();
            EnabledValueCellStyle.BackColor = Color.White;
            EnabledValueCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DisabledValueCellStyle = new DataGridViewCellStyle(EnabledValueCellStyle);
            DisabledValueCellStyle.BackColor = Color.FromArgb(224, 224, 224);

            ValueCorrectedCellStyle = new DataGridViewCellStyle(EnabledValueCellStyle);
            ValueCorrectedCellStyle.BackColor = Color.Olive;

            LockedCellStyle = new DataGridViewCellStyle(EnabledValueCellStyle);
            LockedCellStyle.BackColor = Color.FromArgb(220, 221, 200);

            NonOptimalCellStyle = DisabledValueCellStyle;

            OptimalCellStyle = new DataGridViewCellStyle(EnabledValueCellStyle);
            OptimalCellStyle.BackColor = Color.Blue;
            OptimalCellStyle.ForeColor = Color.White;

            FailedCellStyle = new DataGridViewCellStyle(EnabledValueCellStyle);
            FailedCellStyle.BackColor = Color.Red;

            RoundDigit = 3;
        }

        public DataGridViewCellStyle DefaultCellStyle { get; set; }
        public DataGridViewCellStyle EnabledCellStyle { get; set; }
        public DataGridViewCellStyle EnabledValueCellStyle { get; set; }
        public DataGridViewCellStyle DisabledValueCellStyle { get; set; }
        public DataGridViewCellStyle ValueCorrectedCellStyle { get; set; }
        public DataGridViewCellStyle LockedCellStyle { get; set; }

        public DataGridViewCellStyle NonOptimalCellStyle { get; set; }
        public DataGridViewCellStyle OptimalCellStyle { get; set; }
        public DataGridViewCellStyle FailedCellStyle { get; set; }

        public void FormatParameterValue(DataGridViewCellFormattingEventArgs e, ParameterNode parameterNode)
        {
            if (double.NaN.Equals(e.Value))
            {
                e.Value = "——";
                e.FormattingApplied = true;
            }
            else if (DateTime.MinValue.Equals(e.Value))
            {
                e.Value = String.Empty;
                e.FormattingApplied = true;
            }
            else if (e.Value is double)
            {
                if (parameterNode != null
                    && parameterNode.Typ != (int)UnitTypeId.ManualParameter)
                {
                    double val = (double)e.Value;
                    e.Value = val.ToString(DoubleFormat(parameterNode));
                }
            }
        }
    }
}
