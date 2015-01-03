using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class OptimizationValuesUnitControl : BaseUnitControl
    {
        DateTimePicker dateTimePicker1;
        NumericUpDown topNumericUpDown;
        
        public OptimizationValuesUnitControl(CParameterGateProvider paramGateProvider)
            : base(paramGateProvider)
        {
            InitializeComponent();
            toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(tsbPrevTime), new ToolStripControlHost(dateTimePicker1 = new DateTimePicker()));
            // 
            // dateTimePicker
            // 
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy HH:mm:ss";
            this.dateTimePicker1.Enabled = true;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            //this.dateTimePicker1.Location = new System.Drawing.Point(84, 29);
            this.dateTimePicker1.Name = "dateTimePicker";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 2;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            this.dateTimePicker1.DropDown += new System.EventHandler(this.dateTimePicker1_DropDown);
            this.dateTimePicker1.CloseUp += new System.EventHandler(this.dateTimePicker1_CloseUp);
            toolStrip1.Items.Insert(toolStrip1.Items.IndexOf(tslTop), new ToolStripControlHost(topNumericUpDown = new NumericUpDown()));
            // 
            // topNumericUpDown
            // 
            //this.topNumericUpDown.Location = new System.Drawing.Point(525, 9);
            this.topNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.topNumericUpDown.Name = "topNumericUpDown";
            this.topNumericUpDown.Size = new System.Drawing.Size(80, 20);
            this.topNumericUpDown.TabIndex = 14;
            this.topNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.topNumericUpDown.ValueChanged += new System.EventHandler(this.topNumericUpDown_ValueChanged);

            paramNameColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            paramCodeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;

            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, valuesDataGridView, new object[] { true });

            splitContainer1.Panel2Collapsed = true;
        }

        bool evented = false;
        public override void InitiateProcess()
        {
            CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;

            base.InitiateProcess();
            
            if (paramGateProvider != null)
            {
                bool manual = paramGateProvider.UnitNode.Typ == (int)UnitTypeId.ManualGate;
                topNumericUpDown.Value = DefaultTopCount;

                String timeFormat = "dd MMMM yyyy ";
                Interval interval = paramGateProvider.Interval;

                //if (interval.ToDouble() < 0)
                //{
                //    if (interval.ToDouble() <= -1)
                //        if (interval.ToDouble() % 12 == 0) timeFormat = "yyyy";
                //        else timeFormat = "MMMM yyyy";
                //}
                //else
                //    if (interval.ToDouble() % TimeSpan.FromDays(1).TotalSeconds != 0)
                //    {
                //        timeFormat += "HH";
                //        if (interval.ToDouble() % TimeSpan.FromHours(1).TotalSeconds != 0)
                //        {
                //            timeFormat += ":mm";
                //            if (interval.ToDouble() % TimeSpan.FromMinutes(1).TotalSeconds != 0)
                //                timeFormat += ":ss";
                //        }
                //    }
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
                dateTimePicker1.CustomFormat = timeFormat;

                tsbLock.Visible = manual && !paramGateProvider.LockValueAlways;
                tsbSave.Visible = manual;
                tsbCancel.Visible = manual;
                //UpdateSaveButton();

                if (!evented)
                {
                    evented = true;

                    paramGateProvider.ArgumentsChanged += new EventHandler(paramGateProvider_ArgumentsChanged);
                    paramGateProvider.FilterChanged += new EventHandler(paramGateProvider_FilterChanged);
                    paramGateProvider.ValuesChanged += new EventHandler(paramGateProvider_ValuesChanged);
                    paramGateProvider.ParameterListChanged += new EventHandler(paramGateProvider_ParameterListChanged);
                    paramGateProvider.UseMimeTexChanged += new EventHandler(paramGateProvider_UseMimeTexChanged);
                    paramGateProvider.ParameterCodeImageChanged += new EventHandler(paramGateProvider_ParameterCodeImageChanged);
                    paramGateProvider.CurrentTimeChanged += new EventHandler(paramGateProvider_CurrentTimeChanged);
                    paramGateProvider.ValuesEditModeChanged += new EventHandler(paramGateProvider_ValuesEditModeChanged);
                }
                paramGateProvider.StartProvider();
            }
        }

        //private void UpdateSaveButton()
        //{
        //    //if (unitProvider.ReadOnly
        //    //    || !unitProvider.RemoteDataService.CheckAccess(unitProvider.UnitNode, Privileges.Write | Privileges.Execute))
        //    //{
        //    //    saveButton.Enabled = false;
        //    //    paramValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;
        //    //    paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
        //    //    paramValueColumn.ReadOnly = true;
        //    //}
        //    //else
        //    //{
        //    //    saveButton.Enabled = true;
        //    //    paramValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledValueCellStyle;
        //    //    ParameterGateNode gateNode = unitProvider.UnitNode as ParameterGateNode;
        //    //    if (gateNode != null && gateNode.Typ == UnitTypeId.ManualGate && gateNode.Interval == Interval.Zero)
        //    //        paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledCellStyle;
        //    //    else
        //    //        paramTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
        //    //    paramValueColumn.ReadOnly = false;
        //    //}
        //    if (unitProvider.UnitNode.Typ == UnitTypeId.TEPTemplate)
        //    {
        //        showDependensToolStripMenuItem.Visible = true;
        //        tsbSave.Text = "Рассчитать";
        //        //paramValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle; ;
        //        //paramValueColumn.ReadOnly = true;
        //        //beginValueColumn.Visible = true;
        //    }
        //    else
        //    {
        //        showDependensToolStripMenuItem.Visible = false;
        //        //beginValueColumn.Visible = false;
        //        tsbSave.Text = "Сохранить";
        //    }
        //}

        protected override void DisposeControl()
        {
            CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;

            if (paramGateProvider != null)
            {
                paramGateProvider.ArgumentsChanged -= new EventHandler(paramGateProvider_ArgumentsChanged);
                paramGateProvider.FilterChanged -= new EventHandler(paramGateProvider_FilterChanged);
                paramGateProvider.ValuesChanged -= new EventHandler(paramGateProvider_ValuesChanged);
                paramGateProvider.ParameterListChanged -= new EventHandler(paramGateProvider_ParameterListChanged);
                paramGateProvider.UseMimeTexChanged -= new EventHandler(paramGateProvider_UseMimeTexChanged);
                paramGateProvider.ParameterCodeImageChanged -= new EventHandler(paramGateProvider_ParameterCodeImageChanged);
                paramGateProvider.CurrentTimeChanged -= new EventHandler(paramGateProvider_CurrentTimeChanged);
                paramGateProvider.ValuesEditModeChanged -= new EventHandler(paramGateProvider_ValuesEditModeChanged);
            }

            base.DisposeControl();
        }

        void paramGateProvider_ParameterCodeImageChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_ParameterCodeImageChanged, sender, e);
                else
                {
                    CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;
                    if (paramGateProvider != null)
                    {
                        Image image = null;
                        ParameterNode parameter;
                        IOptimizationArgument optimizationArgument;
                        foreach (DataGridViewRow gridRow in valuesDataGridView.Rows)
                        {
                            if ((parameter = gridRow.Tag as ParameterNode) != null)
                                image = paramGateProvider.GetParameterCodeImage(parameter);
                            else if ((optimizationArgument = gridRow.Tag as IOptimizationArgument) != null)
                                image = paramGateProvider.GetParameterCodeImage(optimizationArgument);

                            if (image != null)
                            gridRow.Cells[clmCodeImage.Index].Value = image;
                        }
                        SetCodeColumnWidth(paramGateProvider);
                    }
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        void paramGateProvider_UseMimeTexChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_UseMimeTexChanged, sender, e);
                else
                {
                    CParameterGateProvider parameterProvider = sender as CParameterGateProvider;

                    if (parameterProvider != null)
                    {
                        valuesDataGridView.Columns[paramCodeColumn.Name].Visible = !parameterProvider.UseMimeTex;
                        valuesDataGridView.Columns[clmCodeImage.Name].Visible = parameterProvider.UseMimeTex;
                        SetCodeColumnWidth(parameterProvider);
                        tsbMimeTex.Checked = parameterProvider.UseMimeTex;
                    }
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        void paramGateProvider_ParameterListChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_ParameterListChanged, sender, e);
                else
                {
                    CParameterGateProvider paramGateProvider = sender as CParameterGateProvider;

                    if (paramGateProvider != null)
                    {
                        ParameterNode[] nodes = paramGateProvider.ParameterList;
                        DataGridViewRow gridRow;

                        valuesDataGridView.Rows.Clear();
                        if (paramGateProvider.UnitNode.Typ == (int)UnitTypeId.ManualGate)
                        {
                            foreach (var argument in paramGateProvider.OptimizationNode.ArgsValues)
                            {
                                if (argument.Mode == OptimizationArgumentMode.Manual)
                                {
                                    gridRow = valuesDataGridView.Rows[valuesDataGridView.Rows.Add()];
                                    gridRow.Tag = argument;
                                    gridRow.Cells[paramNameColumn.Index].Value = argument.Name;
                                    gridRow.Cells[paramCodeColumn.Index].Value = argument.Name;
                                }
                            }
                        }
                        if (nodes != null)
                            foreach (ParameterNode parameter in nodes)
                            {
                                gridRow = valuesDataGridView.Rows[valuesDataGridView.Rows.Add()];
                                System.Threading.Thread.Sleep(1);
                                gridRow.Tag = parameter;
                                gridRow.Cells[paramNameColumn.Index].Value = parameter.GetNodeText();
                                gridRow.Cells[paramCodeColumn.Index].Value = parameter.Code;
                                gridRow.Cells[clmCodeImage.Index].Value = Properties.Resources.tep;
                                gridRow.Cells[clmCodeImage.Index].ToolTipText = parameter.Code;
                            }
                        paramGateProvider_ParameterCodeImageChanged(sender, e);
                        paramGateProvider_ValuesChanged(paramGateProvider, EventArgs.Empty);
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        void paramGateProvider_ValuesChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_ValuesChanged, sender, e);
                else
                {
                    CParameterGateProvider parameterGateProvider = sender as CParameterGateProvider;
                    ArgumentsValues item;
                    ParameterNode parameterNode;
                    IOptimizationArgument optimizationArgument = null;
                    ParamValueItem paramValueItem;

                    foreach (DataGridViewRow gridRow in valuesDataGridView.Rows)
                        if ((parameterNode = gridRow.Tag as ParameterNode) != null
                            || (optimizationArgument = gridRow.Tag as IOptimizationArgument) != null)
                            foreach (DataGridViewColumn gridColumn in valuesDataGridView.Columns)
                            {
                                if ((item = gridColumn.Tag as ArgumentsValues) != null)
                                {
                                    if (parameterNode != null)
                                    {
                                        paramValueItem = parameterGateProvider.GetValue(parameterNode, item);
                                        if (paramValueItem != null)
                                        {
                                            gridRow.Cells[gridColumn.Index].Value = paramValueItem.Value;
                                            gridRow.Cells[gridColumn.Index].ToolTipText = GetParamValueItemTip(paramValueItem);
                                        }
                                        else
                                            gridRow.Cells[gridColumn.Index].Value = double.NaN;
                                    }
                                    else if (optimizationArgument != null)
                                    {
                                        Object val = item[optimizationArgument.Name];
                                        if (val != null)
                                            gridRow.Cells[gridColumn.Index].Value = val;
                                        else
                                            gridRow.Cells[gridColumn.Index].Value = double.NaN;
                                    }
                                }
                            }
                    //foreach (var item in parameterGateProvider.FilteredArguments)
                    //{
                    //    String columnName = GetColumnName(item);
                    //    if (valuesDataGridView.Columns.Contains(columnName))
                    //    {
                    //        if (parameterNode != null)
                    //        {
                    //            paramValueItem = parameterGateProvider.GetValue(parameterNode, item);
                    //            if (paramValueItem != null)
                    //                gridRow.Cells[columnName].Value = paramValueItem.Value;
                    //            else
                    //                gridRow.Cells[columnName].Value = double.NaN;
                    //        }
                    //        else if (optimizationArgument != null)
                    //        {
                    //            Object val = item[optimizationArgument.Name];
                    //            if (val != null)
                    //                gridRow.Cells[columnName].Value = val;
                    //            else
                    //                gridRow.Cells[columnName].Value = double.NaN;
                    //        }
                    //    }
                    //}
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        private string GetParamValueItemTip(ParamValueItem valueItem)
        {
            CorrectedParamValueItem correctedItem = valueItem as CorrectedParamValueItem;
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("Значение: {0}", valueItem.Value);
            if (valueItem.Arguments != null)
                builder.AppendFormat("\nАргумент: {0}", valueItem.Arguments);
            if (correctedItem != null)
            {
                if (!double.Equals(correctedItem.OriginalValueItem.Value, correctedItem.CorrectedValueItem.Value))
                    builder.AppendFormat("\nНачальное значение: {0}", correctedItem.OriginalValueItem.Value);
                if (correctedItem.CorrectedValueItem.Time != DateTime.MinValue
                    && !DateTime.Equals(correctedItem.OriginalValueItem.Time, correctedItem.CorrectedValueItem.Time))
                    builder.AppendFormat("\nИзначальное время: {0}", correctedItem.OriginalValueItem.Time);
                if (!Object.Equals(correctedItem.OriginalValueItem.Arguments, correctedItem.CorrectedValueItem.Arguments))
                    builder.AppendFormat("\nИзначальные аргументы: {0}", correctedItem.OriginalValueItem.Arguments);
            }
            builder.AppendFormat("\nВремя изменения: {0}", valueItem.ChangeTime);
            return builder.ToString();
        }

        async void paramGateProvider_ArgumentsChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_ArgumentsChanged, sender, e);
                else
                {
                    CParameterGateProvider parameterGateProvider = sender as CParameterGateProvider;

                    AddArgumentColumns(parameterGateProvider);
                    Dictionary<ArgumentsValues, TreeNode> treeNodeDictionary =
                        new Dictionary<ArgumentsValues, TreeNode>();
                    TreeNode node, childNode;

                    List<OptimizationGateNode> baseNodes = new List<OptimizationGateNode>();
                    OptimizationGateNode optimizationNode = parameterGateProvider.OptimizationNode;

                    while (optimizationNode != null)
                    {
                        var parentNode = await Task.Factory.StartNew(() => unitProvider.StructureProvider.GetUnitNode(optimizationNode.ParentId));
                        optimizationNode = await Task.Factory.StartNew(() => unitProvider.StructureProvider.GetParent(parentNode, (int)UnitTypeId.OptimizeCalc) as OptimizationGateNode);

                        if (optimizationNode != null)
                            baseNodes.Add(optimizationNode);
                    }

                    parentArgumentTreeView.Nodes.Clear();
                    foreach (var levelArgs in parameterGateProvider.Arguments)
                    {
                        ArgumentsValues baseArgs = levelArgs;
                        node = null;

                        baseArgs = baseArgs.Exclude(from a in parameterGateProvider.OptimizationNode.ArgsValues select a.Name);
                        //while ((baseArgs = parameterGateProvider.GetBaseArguments(baseArgs)) != null)
                        foreach (var optimNode in baseNodes)
                        {
                            childNode = node;
                            if (!treeNodeDictionary.TryGetValue(baseArgs, out node))
                            {
                                node = new TreeNode(baseArgs.ToString());
                                node.Tag = baseArgs;
                                treeNodeDictionary[baseArgs] = node;
                                node.Checked = parameterGateProvider.FilterLevelArgument(baseArgs);//.FilterArgument(baseArgs);
                            }
                            if (childNode != null && !node.Nodes.Contains(childNode))
                                node.Nodes.Add(childNode);

                            baseArgs = baseArgs.Exclude(from a in optimNode.ArgsValues select a.Name);
                        }
                        if (node != null && !parentArgumentTreeView.Nodes.Contains(node))
                            parentArgumentTreeView.Nodes.Add(node);
                    }
                    tsbSort.Enabled = parameterGateProvider.SortedArguments != null
                        && parameterGateProvider.SortedArguments.Length > 0;
                    splitContainer1.Panel2Collapsed = parentArgumentTreeView.Nodes.Count == 0;
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        void paramGateProvider_FilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((EventHandler)paramGateProvider_ArgumentsChanged, sender, e);
                else
                {
                    CParameterGateProvider parameterGateProvider = sender as CParameterGateProvider;

                    AddArgumentColumns(parameterGateProvider);

                    ShowFilterState(parameterGateProvider, parentArgumentTreeView.Nodes);

                }
            }
            catch (ObjectDisposedException)
            { }
        }

        void paramGateProvider_CurrentTimeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke((EventHandler)paramGateProvider_CurrentTimeChanged, sender, e);
            else
            {
                CParameterGateProvider parametergGateProvider = unitProvider as CParameterGateProvider;
                if (parametergGateProvider != null)
                    dateTimePicker1.Value = parametergGateProvider.CurrentTime;
            }
        }

        void paramGateProvider_ValuesEditModeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)paramGateProvider_ValuesEditModeChanged, sender, e);
            else
            {
                CParameterGateProvider parameterProvider = unitProvider as CParameterGateProvider;
                if (parameterProvider!=null)
                {
                    tsbLock.Enabled = !parameterProvider.ValuesEditMode;
                    tsbSave.Enabled = parameterProvider.ValuesEditMode;
                    tsbCancel.Enabled = parameterProvider.ValuesEditMode;
                    addColumnToolStripMenuItem.Enabled = parameterProvider.ValuesEditMode;
                    deleteColumnToolStripMenuItem.Enabled = parameterProvider.ValuesEditMode;
                    ColorArgumentColumns(parameterProvider);
                    //throw new NotImplementedException();  
                }
            }
        }

        private static void ShowFilterState(CParameterGateProvider parameterGateProvider, TreeNodeCollection nodeCollection)
        {
            ArgumentsValues args;
            foreach (TreeNode treeNode in nodeCollection)
            {
                if ((args = treeNode.Tag as ArgumentsValues) != null)
                    treeNode.Checked = parameterGateProvider.FilterLevelArgument(args);//.FilterArgument(args);
                ShowFilterState(parameterGateProvider, treeNode.Nodes);
            }
        }

        private bool addArgumentEntered;
        private void AddArgumentColumns(CParameterGateProvider parameterGateProvider)
        {
            if (addArgumentEntered)
                return;
            addArgumentEntered = true;

            try
            {
                if (parameterGateProvider != null)
                {
                    DataGridViewColumn gridColumn = null;
                    List<ArgumentsValues> argumentsList;

                    // удаление старых колонок со значениями
                    foreach (String item in argumentsColumns)
                    {
                        if (valuesDataGridView.Columns.Contains(item))
                            valuesDataGridView.Columns.Remove(item);
                    }
                    argumentsColumns.Clear();
                    columnNameList.Clear();

                    if (topNumericUpDown.Visible = tslTop.Visible = parameterGateProvider.FilteredArguments != null
                                && parameterGateProvider.FilteredArguments.Length > DefaultTopCount)
                    {
                        topNumericUpDown.Maximum = parameterGateProvider.FilteredArguments.Length;
                    }
                    if (parameterGateProvider.FilteredArguments != null)
                    {
                        if (parameterGateProvider.FilteredArguments.Length > DefaultTopCount)
                        {
                            int count = (int)topNumericUpDown.Value;
                            List<ArgumentsValues> fullList =
                                new List<ArgumentsValues>(parameterGateProvider.FilteredArguments);

                            argumentsList = new List<ArgumentsValues>();

                            if (parameterGateProvider.SortedArguments != null)
                                foreach (var args in parameterGateProvider.SortedArguments)
                                    if (fullList.Contains(args))
                                    {
                                        fullList.Remove(args);
                                        argumentsList.Add(args);
                                        if (argumentsList.Count >= count)
                                            break;
                                    }
                            for (int i = 0; i < fullList.Count && argumentsList.Count < count; i++)
                                argumentsList.Add(fullList[i]);
                        }
                        else argumentsList = new List<ArgumentsValues>(parameterGateProvider.FilteredArguments);

                        // добавление новых колонок со значениями
                        //if (parameterGateProvider.Arguments != null && parameterGateProvider.Arguments.Length > DefaultTopCount)
                        foreach (var item in argumentsList)
                        {
                            gridColumn = valuesDataGridView.Columns[valuesDataGridView.Columns.Add(GetColumnName(item), item.ToString())];
                            gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.NonOptimalCellStyle;
                            gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            gridColumn.Tag = item;
                            gridColumn.ReadOnly = true;
                            argumentsColumns.Add(gridColumn.Name);
                        }
                        sortCheckBox_CheckedChanged(tsbSort, EventArgs.Empty);
                        ColorArgumentColumns(parameterGateProvider);
                        paramGateProvider_ValuesChanged(parameterGateProvider, EventArgs.Empty);
                    }
                }
            }
            finally
            {
                addArgumentEntered = false;
            }
        }

        private void ColorArgumentColumns(CParameterGateProvider parameterGateProvider)
        {
            List<ArgumentsValues> optimalArguments = null;
            if (parameterGateProvider.OptimalArguments != null)
                optimalArguments = new List<ArgumentsValues>(parameterGateProvider.OptimalArguments.Values);
            bool readOnly = !parameterGateProvider.ValuesEditMode //|| parameterGateProvider.ReadOnly
                || !parameterGateProvider.StructureProvider.CheckAccess(parameterGateProvider.UnitNode, Privileges.Write | Privileges.Execute);
            bool calc = unitProvider.UnitNode.Typ == (int)UnitTypeId.TEPTemplate;

            foreach (DataGridViewColumn gridColumn in valuesDataGridView.Columns)
            {
                ArgumentsValues args = gridColumn.Tag as ArgumentsValues;

                if (args != null)
                {
                    gridColumn.ReadOnly = !calc && readOnly;

                    if (calc)
                    {
                        //gridColumn.ReadOnly = true;
                        if (optimalArguments != null && optimalArguments.Contains(args))
                            gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.OptimalCellStyle;
                        else if (parameterGateProvider.SortedArguments == null || parameterGateProvider.SortedArguments.Contains(args))
                            gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.NonOptimalCellStyle;
                        else gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.FailedCellStyle;
                    }
                    else
                    {
                       if(readOnly)
                           gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;
                       else 
                           gridColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledValueCellStyle;
                        //gridColumn.ReadOnly = false;

                        foreach (DataGridViewRow gridRow in valuesDataGridView.Rows)
                       {
                           ParameterNode parmeter;
                           if ((parmeter = gridRow.Tag as ParameterNode) != null)
                           {
                               if (parameterGateProvider.IsLocked(parmeter))
                               {
                                   gridRow.Cells[gridColumn.Index].ReadOnly = false;
                                   gridRow.Cells[gridColumn.Index].Style = ValuesControlSettings.Instance.DisabledValueCellStyle;
                               }
                               else
                               {
                                   gridRow.Cells[gridColumn.Index].ReadOnly = gridColumn.ReadOnly;
                                   gridRow.Cells[gridColumn.Index].Style = gridColumn.DefaultCellStyle;
                               }
                           }
                       }
                    }
                }
            }
            //foreach (DataGridViewRow gridRow in valuesDataGridView.Rows)
            //{
            //    ParameterNode parmeter;
            //    if ((parmeter=gridRow.Tag as ParameterNode)!=null)
            //    {
            //        if (parameterGateProvider.IsLocked(parmeter))
            //        {
            //            gridRow.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;
            //        }
            //    }
            //}
        }

        List<String> argumentsColumns = new List<String>();

        public const int DefaultTopCount = 10;

        private List<ArgumentsValues> columnNameList = new List<ArgumentsValues>();

        private String GetColumnName(ArgumentsValues argument)
        {
            int index = columnNameList.IndexOf(argument);
            if (index < 0)
            {
                index = columnNameList.Count;
                columnNameList.Add(argument);
            }
            return String.Format("argument_{0}", index);
        }

        private void SetCodeColumnWidth(CParameterGateProvider paramGateProvider)
        {
            if (paramGateProvider.UseMimeTex)
            {
                int codeColumnWidth = paramGateProvider.CodeColumnWidth;
                if (valuesDataGridView.Rows.Count > 0)
                {
                    int height = paramGateProvider.RowHeight;
                    foreach (DataGridViewRow gridRow in valuesDataGridView.Rows)
                    {
                        if (height > gridRow.MinimumHeight)
                            gridRow.MinimumHeight = height;
                    }
                }
                if (valuesDataGridView.Columns.Contains(clmCodeImage.Name))
                {
                    if (0 < codeColumnWidth)
                        valuesDataGridView.Columns[clmCodeImage.Name].Width = codeColumnWidth;
                    if (clmCodeImage.MinimumWidth < codeColumnWidth)
                        valuesDataGridView.Columns[clmCodeImage.Name].MinimumWidth = codeColumnWidth;
                }
                clmCodeImage.Resizable = DataGridViewTriState.False;
            }
        }

        private void nextTimeButton_Click(object sender, EventArgs e)
        {
            CParameterGateProvider optimizationProvider = unitProvider as CParameterGateProvider;

            if (optimizationProvider != null)
                optimizationProvider.NextTime();
        }

        private void prevTimeButton_Click(object sender, EventArgs e)
        {
            CParameterGateProvider optimizationProvider = unitProvider as CParameterGateProvider;

            if (optimizationProvider != null)
                optimizationProvider.PrevTime();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            CParameterGateProvider parameterGateProvider = unitProvider as CParameterGateProvider;
            if (parameterGateProvider != null)
            {
                //if (parameterGateProvider.UnitNode.Typ == UnitTypeId.ManualGate ||
                //    parameterGateProvider.HasChanges)
                //{
                    parameterGateProvider.SaveValues();
                //}
                //else
                //{
                //    CalcForm calcForm = new CalcForm(unitProvider.RemoteDataService, Program.MainForm.Icons);
                //    calcForm.TimeBegin = parameterGateProvider.CurrentTime;
                //    if (parameterGateProvider.Interval != Interval.Zero)
                //        calcForm.TimeEnd = parameterGateProvider.Interval.GetNextTime(calcForm.TimeBegin);
                //    calcForm.Interval = parameterGateProvider.Interval;
                //    calcForm.Parameter = parameterGateProvider.TopOptimizationNode;
                //    calcForm.CalcFinished += new EventHandler(parameterGateProvider.CalcForm_CalcFinished);
                //    calcForm.Show(this);
                //}
            }
        }

        private void topNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AddArgumentColumns(UnitProvider as CParameterGateProvider);
        }

        private void sortCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CParameterGateProvider parameterGateProcider = unitProvider as CParameterGateProvider;
            if (parameterGateProcider != null)
            {
                bool sorted = tsbSort.Checked;
                String columnName;
                int index = paramCodeColumn.DisplayIndex;
                IEnumerable<ArgumentsValues> arguments =
                    sorted ? parameterGateProcider.SortedArguments : parameterGateProcider.FilteredArguments;

                foreach (var item in arguments)
                {
                    columnName = GetColumnName(item);
                    if (valuesDataGridView.Columns.Contains(columnName))
                        valuesDataGridView.Columns[columnName].DisplayIndex = ++index;
                }
            }
        }

        private void valuesDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            ValuesControlSettings.Instance.FormatParameterValue(e, valuesDataGridView.Rows[e.RowIndex].Tag as ParameterNode);
        }

        private void showReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesDataGridView.CurrentRow != null)
            {
                CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;
                ParameterNode node;
                if (paramGateProvider != null
                    && (node = valuesDataGridView.CurrentRow.Tag as ParameterNode) != null)
                {
                    TepForm referenceForm = new TepForm(paramGateProvider.StructureProvider, Program.MainForm.Icons);
                    Program.MainForm.AddExtendForm(referenceForm);
                    referenceForm.Node = node;
                    referenceForm.Relation = FormulaRelation.Reference;
                    referenceForm.CurrentTime = paramGateProvider.CurrentTime;
                    ArgumentsValues args = valuesDataGridView.Columns[valuesDataGridView.CurrentCell.ColumnIndex].Tag as ArgumentsValues;
                    if (args != null)
                        referenceForm.Arguments = args;//.HidenValueArguments;
                    referenceForm.ReadOnly = unitProvider.ReadOnly;
                    referenceForm.TopMost = true;
                    referenceForm.Show();
                }
            }
        }

        private void showDependensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesDataGridView.CurrentRow != null)
            {
                CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;
                ParameterNode node;
                if (paramGateProvider != null
                    && (node = valuesDataGridView.CurrentRow.Tag as ParameterNode) != null)
                {
                    TepForm referenceForm = new TepForm(paramGateProvider.StructureProvider, Program.MainForm.Icons);
                    Program.MainForm.AddExtendForm(referenceForm);
                    referenceForm.Node = node;
                    referenceForm.Relation = FormulaRelation.Dependence;
                    referenceForm.CurrentTime = paramGateProvider.CurrentTime;
                    ArgumentsValues args = valuesDataGridView.Columns[valuesDataGridView.CurrentCell.ColumnIndex].Tag as ArgumentsValues;
                    if (args != null)
                        referenceForm.Arguments = args;//.HidenValueArguments;
                    referenceForm.ReadOnly = unitProvider.ReadOnly;
                    referenceForm.TopMost = true;
                    //referenceForm.ShowDialog();
                    referenceForm.Show();
                }
            }
        }

        private void toCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = unitProvider.UnitNode.Text;

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (Stream s = saveFileDialog1.OpenFile())
                {
                    GridToCSV(s, valuesDataGridView, new String[] { paramCodeColumn.Name }, new String[] { clmCodeImage.Name });
                }
            }
        }

        bool dateTimePicker1Dropped;
        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            dateTimePicker1Dropped = false;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void dateTimePicker1_DropDown(object sender, EventArgs e)
        {
            dateTimePicker1Dropped = true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CParameterGateProvider parameterProvider = unitProvider as CParameterGateProvider;
            if (!dateTimePicker1Dropped && parameterProvider != null)
            {
                TimeChange(dateTimePicker1.Value);
            }
        }

        private void TimeChange(DateTime dateTime)
        {
            UniForm uniForm = null;//UnitProvider.UniForm as UniForm;
            throw new NotImplementedException();
            UnitProvider[] unitProviders;// = UnitProvider.UniForm.GetTabbedProviders();
            List<CParameterGateProvider> parmeterProviderList = new List<CParameterGateProvider>();

            if (uniForm != null)
                unitProviders = uniForm.GetTabbedProviders();
            else unitProviders = new UnitProvider[] { UnitProvider };

            bool modified = false, save = false;
            foreach (UnitProvider provider in unitProviders)
            {
                CParameterGateProvider parameterProvider;
                if ((parameterProvider = provider as CParameterGateProvider) != null)
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
                    dateTimePicker1.Value = ((CParameterGateProvider)unitProvider).CurrentTime;
                    return;
                }
            }
            foreach (CParameterGateProvider parameterProvider in parmeterProviderList)
            {
                if (save)
                    parameterProvider.SaveValues();
                parameterProvider.CurrentTime = dateTime;
            }
        }

        private void parentArgumentTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                CParameterGateProvider parameterGateProvider = unitProvider as CParameterGateProvider;
                parameterGateProvider.FilterArgument(e.Node.Tag as ArgumentsValues, e.Node.Checked);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ((CParameterGateProvider)unitProvider).UseMimeTex = tsbMimeTex.Checked;

        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            CParameterGateProvider parameterGateProvider = unitProvider as CParameterGateProvider;

            if (parameterGateProvider != null && unitProvider.UnitNode.Typ == (int)UnitTypeId.ManualGate)
            {
                addColumnToolStripMenuItem.Visible = true;
                deleteColumnToolStripMenuItem.Visible = true;
                addColumnToolStripMenuItem.DropDownItems.Clear();
                //ToolStripItem addColumnItem;

                //List<ArgumentsValues> addedList = new List<ArgumentsValues>();

                //foreach (var levelArgs in parameterGateProvider.FilteredArguments)
                //{
                //    ArgumentsValues baseArgs = levelArgs.GetBaseArgument();
                //    if (baseArgs != null && !addedList.Contains(baseArgs))
                //    {
                //        addedList.Add(baseArgs);
                //        addColumnItem = new ToolStripDropDownButton(baseArgs.ToString());
                //        addColumnItem.Tag = baseArgs;
                //        addColumnItem.Click += addColumnToolStripMenuItem_Click;
                //        addColumnToolStripMenuItem.DropDownItems.Add(addColumnItem);
                //    }
                //}
            }
            else
            {
                addColumnToolStripMenuItem.Visible = false;
                deleteColumnToolStripMenuItem.Visible = false;
            }
            useCalcToolStripMenuItem.Visible = false;
            showDependensToolStripMenuItem.Visible = unitProvider.UnitNode.Typ != (int)UnitTypeId.ManualGate;
            toolStripMenuItem2.Visible = addColumnToolStripMenuItem.Visible || useCalcToolStripMenuItem.Visible;
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CParameterGateProvider parameterGateProvider;
            ToolStripDropDownItem addColumnItem;

            if ((parameterGateProvider = unitProvider as CParameterGateProvider) != null
                && (addColumnItem = sender as ToolStripDropDownItem) != null
                && addColumnItem.DropDownItems.Count == 0)
            {
                ArgumentsValues valueArguments = addColumnItem.Tag as ArgumentsValues;
                parameterGateProvider.AddColumn(valueArguments);
            }
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesDataGridView.CurrentRow != null)
            {
                CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;
                ArgumentsValues args = valuesDataGridView.Columns[valuesDataGridView.CurrentCell.ColumnIndex].Tag as ArgumentsValues;

                if (paramGateProvider != null
                    && args != null)
                {
                    paramGateProvider.DeleteColumn(args);
                }
            }
        }

        private void valuesDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //if (valueChangeIgnore) return;
            CParameterGateProvider parameterProvider = unitProvider as CParameterGateProvider;

            if (parameterProvider != null)
            {
                DataGridViewRow gridRow = valuesDataGridView.Rows[e.RowIndex];
                DataGridViewColumn gridColumn = valuesDataGridView.Columns[e.ColumnIndex];
                DataGridViewCell gridCell = gridRow.Cells[e.ColumnIndex];

                ParameterNode param;// = gridRow.Tag as ParameterNode;
                IOptimizationArgument optimizationArgument;
                ArgumentsValues valueArgument = gridColumn.Tag as ArgumentsValues;

                if (valueArgument == null)
                    return;

                double val;
                if (gridCell.Value == null
                    || (!double.TryParse(gridCell.Value.ToString(),
                                        System.Globalization.NumberStyles.Float,
                                        System.Globalization.NumberFormatInfo.CurrentInfo,
                                        out val) // распарсить введенное значение при текущей культуре
                    && !double.TryParse(gridCell.Value.ToString(),
                                        System.Globalization.NumberStyles.Float,
                                        System.Globalization.NumberFormatInfo.InvariantInfo,
                                        out val))) // если не получилось то по забугорной
                    val = double.NaN;


                if ((param = gridRow.Tag as ParameterNode) != null)
                {
                    ParamValueItem receiveItem = parameterProvider.GetValue(param, valueArgument);
                    if (receiveItem != null || !double.IsNaN(val))
                    {
                        if (receiveItem == null)
                            receiveItem = new ParamValueItem();
                        //if (param.Typ == UnitTypeId.TEP && !val.Equals(receiveItem.Value))
                        //    receiveItem.corrval = val;
                        if (param.Typ == (int)UnitTypeId.ManualParameter || receiveItem is CorrectedParamValueItem)
                        {
                            receiveItem.Value = val;
                            receiveItem.Arguments = valueArgument;
                            receiveItem.Quality = double.IsNaN(val) ? Quality.Bad : Quality.Good;
                            receiveItem.Time = parameterProvider.CurrentTime; 
                        }
                        parameterProvider.SetValue(param, valueArgument, receiveItem);
                    }
                    //if (parameterProvider.Modified(param, valueArgument))
                    //    gridCell.Style.Font = new Font(valuesDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                    //else gridCell.Style.Font = valuesDataGridView.DefaultCellStyle.Font;
                }
                else if ((optimizationArgument = gridRow.Tag as IOptimizationArgument) != null)
                {
                    parameterProvider.SetArgumentValue(optimizationArgument, valueArgument, val);
                    //if (parameterProvider.Modified(optimizationArgument, valueArgument))
                    //    gridCell.Style.Font = new Font(valuesDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                    //else gridCell.Style.Font = valuesDataGridView.DefaultCellStyle.Font;
                }
            }
        }

        private void valuesDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            ParameterNode param;
            IOptimizationArgument optimizationArgument;
            CParameterGateProvider parameterProvider;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 
                && (parameterProvider = unitProvider as CParameterGateProvider) != null)
            {
                DataGridViewCell gridCell = valuesDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                ArgumentsValues valueArgument = valuesDataGridView.Columns[e.ColumnIndex].Tag as ArgumentsValues;

                if (valueArgument == null)
                    return;
                bool modified;
                //e.CellStyle
                if ((param = valuesDataGridView.Rows[e.RowIndex].Tag as ParameterNode) != null)
                    modified = parameterProvider.Modified(param, valueArgument);
                else if ((optimizationArgument = valuesDataGridView.Rows[e.RowIndex].Tag as IOptimizationArgument) != null)
                    modified = parameterProvider.Modified(optimizationArgument, valueArgument);
                else modified = false;

                if (modified)
                    e.CellStyle.Font = new Font(valuesDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                else e.CellStyle.Font = valuesDataGridView.DefaultCellStyle.Font;
            }
        }

        private void useCalcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (valuesDataGridView.CurrentRow != null)
            //{
            //    CParameterGateProvider paramGateProvider = unitProvider as CParameterGateProvider;
            //    IValueArguments args;
            //    if (paramGateProvider != null
            //        && (args = valuesDataGridView.Columns[valuesDataGridView.CurrentCell.ColumnIndex].Tag as IValueArguments) != null)
            //    {
            //        paramGateProvider.UseCalc(args);
            //    }
            //}
        }

        private void tsbLock_Click(object sender, EventArgs e)
        {
            CParameterGateProvider parameterProvider = unitProvider as CParameterGateProvider;
            if (parameterProvider!=null)
                parameterProvider.LockValues();
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            CParameterGateProvider parameterProvider = unitProvider as CParameterGateProvider;
            if (parameterProvider != null)
                parameterProvider.ClearUnsavedValues();
        }

        private void tsbCalc_Click(object sender, EventArgs e)
        {
            CParameterGateProvider parameterGateProvider = unitProvider as CParameterGateProvider;
            if (parameterGateProvider != null)
            {
                //if (parameterGateProvider.UnitNode.Typ == UnitTypeId.ManualGate ||
                //    parameterGateProvider.HasChanges)
                //{
                //    parameterGateProvider.SaveValues();
                //}
                //else
                //{
                    CalcForm calcForm = new CalcForm(unitProvider.StructureProvider, Program.MainForm.Icons);
                    Program.MainForm.AddExtendForm(calcForm);
                    calcForm.TimeBegin = parameterGateProvider.CurrentTime;
                    if (parameterGateProvider.Interval != Interval.Zero)
                        calcForm.TimeEnd = parameterGateProvider.Interval.GetNextTime(calcForm.TimeBegin);
                    calcForm.Interval = parameterGateProvider.Interval;
                    calcForm.Parameter = parameterGateProvider.TopOptimizationNode;
                    calcForm.CalcFinished += new EventHandler(parameterGateProvider.CalcForm_CalcFinished);
                    calcForm.Show(this);
                //}
            }
        }
    }
}
