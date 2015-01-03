using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Контрол для редактирования оптимизационного расчета
    /// </summary>
    partial class OptimizationUnitControl : BaseUnitControl
    {
        System.Windows.Forms.TreeNode expressionTreeNode;
        System.Windows.Forms.TreeNode definatrionDomainTreeNode;
        System.Windows.Forms.TreeNode argumentsTreeNode;

        public OptimizationUnitControl(UnitProvider unitProvider)
            : base(unitProvider)
        {
            InitializeComponent();

            Text = "Оптимизация";

            expressionTreeNode = new System.Windows.Forms.TreeNode();
            definatrionDomainTreeNode = new System.Windows.Forms.TreeNode();
            argumentsTreeNode = new System.Windows.Forms.TreeNode("Аргументы");
        }

        bool evented;
        public override void InitiateProcess()
        {
            if (UnitProvider is FormulaUnitProvider)
            {
                formulaEditControl1.UnitProvider = UnitProvider as FormulaUnitProvider;
                formulaEditControl1.InitiateProcess();
                expressionTreeNode.Name = "Expression";
                expressionTreeNode.Text = (unitProvider as FormulaUnitProvider).GetObjectName(OptimizationUnitProvider.ExpressionObjectID);
                definatrionDomainTreeNode.Name = "DefinationDomain";
                definatrionDomainTreeNode.Text = (unitProvider as FormulaUnitProvider).GetObjectName(OptimizationUnitProvider.DefinationDomainObjectID);
                argumentsTreeNode.Name = "Arguments";
                argumentsTreeNode.Text = "Аргументы";
                this.expressionTreeView.Nodes.Clear();
                this.expressionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
                    expressionTreeNode,
                    definatrionDomainTreeNode,
                    argumentsTreeNode});

                ////RefreshArguments();
                //if (this.unitProvider != null)
                //{
                //    (this.unitProvider as OptimizationUnitProvider).SelectedExpressionChanged -= new EventHandler(OptimizationUnitControl_SelectedExpressionChanged);
                //    (this.unitProvider as OptimizationUnitProvider).ArgumentsRetrieved -= new Action<COTES.ISTOK.Calc.CalcArgumentInfo[]>(OptimizationUnitControl_ArgumentsRetrieved);
                //}
                if (!evented)
                {
                    evented = true;
                    (unitProvider as OptimizationUnitProvider).SelectedExpressionChanged += new EventHandler(OptimizationUnitControl_SelectedExpressionChanged);
                    (unitProvider as OptimizationUnitProvider).ArgumentsRetrieved += new Action<CalcArgumentInfo[]>(OptimizationUnitControl_ArgumentsRetrieved);
                    (unitProvider as OptimizationUnitProvider).EditModeChanged += new EventHandler(OptimizationUnitControl_EditModeChanged);
                }
                OptimizationUnitControl_EditModeChanged(unitProvider, EventArgs.Empty);

                RefreshArguments((unitProvider as OptimizationUnitProvider).ArgsValues);
                //OptimizationUnitControl_SelectedExpressionChanged(unitProvider, EventArgs.Empty);
            }
        }

        void OptimizationUnitControl_EditModeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)OptimizationUnitControl_EditModeChanged, sender, e);
            else
            {
                tsbLock.Enabled = !UnitProvider.EditMode;
                tsbSave.Enabled = UnitProvider.EditMode;
                tsbCancel.Enabled = unitProvider.EditMode;
                RefreshUpDownButtons(expressionTreeView.SelectedNode);
            }
        }

        protected override void DisposeControl()
        {
            if (this.unitProvider != null)
            {
                (this.unitProvider as OptimizationUnitProvider).SelectedExpressionChanged -= new EventHandler(OptimizationUnitControl_SelectedExpressionChanged);
                (this.unitProvider as OptimizationUnitProvider).ArgumentsRetrieved -= new Action<COTES.ISTOK.Calc.CalcArgumentInfo[]>(OptimizationUnitControl_ArgumentsRetrieved);
                (unitProvider as OptimizationUnitProvider).EditModeChanged -= new EventHandler(OptimizationUnitControl_EditModeChanged);
            }
            base.DisposeControl();
        }

        void OptimizationUnitControl_ArgumentsRetrieved(CalcArgumentInfo[] obj)
        {
            RefreshArguments((unitProvider as OptimizationUnitProvider).ArgsValues);
        }

        void RefreshArguments(List<OptimizationArgument> obj)
        {
            if (InvokeRequired) Invoke((Action<List<OptimizationArgument>>)RefreshArguments, obj);
            else
            {
                TreeNode argumentTreeNode;
                argumentsTreeNode.Nodes.Clear();
                foreach (var item in obj)
                {
                    argumentTreeNode = new TreeNode();
                    argumentTreeNode.Tag = item;
                    argumentTreeNode.Text = GetArgumentText(item);// item.ToString();
                    argumentsTreeNode.Nodes.Add(argumentTreeNode);
                }
                expressionTreeView.ExpandAll();
                expressionTreeView.SelectedNode = expressionTreeNode;
            }
        }

        private string GetArgumentText(OptimizationArgument item)
        {
            String suffix;

            switch (item.Mode)
            {
                case OptimizationArgumentMode.Manual:
                    suffix = " (ручной ввод)";
                    break;
                case OptimizationArgumentMode.Interval:
                    suffix = " (интервал)";
                    break;
                case OptimizationArgumentMode.Expression:
                    suffix = " (выражение)";
                    break;
                case OptimizationArgumentMode.ColumnNum:
                    suffix = " (номер колонки)";
                    break;
                case OptimizationArgumentMode.Default:
                default:
                    suffix = String.Empty;
                    break;
            }

            return String.Format("{0}{1}", item.Name, suffix);
        }

        void OptimizationUnitControl_SelectedExpressionChanged(object sender, EventArgs e)
        {
            try
            {
                formulaEditControl1.Formula = (sender as OptimizationUnitProvider).Formula;
            }
            catch (ObjectDisposedException)
            { }
        }

        private void expressionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            formulaEditControl1.Enabled = true;

            tsbArgumentDown.Enabled = false;
            tsbArgumentUp.Enabled = false;

            if (e.Node.Equals(expressionTreeNode))
                (unitProvider as OptimizationUnitProvider).SelectedExpression = OptimizationUnitProvider.ExpressionType.Expression;
            else if (e.Node.Equals(definatrionDomainTreeNode))
                (unitProvider as OptimizationUnitProvider).SelectedExpression = OptimizationUnitProvider.ExpressionType.DefinationDomain;
            else if (argumentsTreeNode.Nodes.Contains(e.Node))
            {
                IOptimizationArgument argument = e.Node.Tag as IOptimizationArgument;

                if (argument != null)
                {
                    (unitProvider as OptimizationUnitProvider).SelectedExpression = OptimizationUnitProvider.ExpressionType.ArgumentExpression;
                    (unitProvider as OptimizationUnitProvider).SelectedArgumentName = argument.Name;
                    RefreshUpDownButtons(e.Node);
                    formulaEditControl1.Enabled = argument.Mode == OptimizationArgumentMode.Expression;
                }
                else formulaEditControl1.Enabled = false;
            }
            else
            {
                formulaEditControl1.Enabled = false;
                formulaEditControl1.Formula = String.Empty;
            }
        }

        private void RefreshUpDownButtons(TreeNode treeNode)
        {
            int argumentIndex = argumentsTreeNode.Nodes.IndexOf(treeNode);
            tsbArgumentDown.Enabled = unitProvider.EditMode && 0 <= argumentIndex && argumentIndex < argumentsTreeNode.Nodes.Count - 1;
            tsbArgumentUp.Enabled = unitProvider.EditMode && argumentIndex > 0;

        }

        private void argumentsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !UnitProvider.EditMode;
            if (UnitProvider.EditMode)
            {
                renameArgumentToolStripMenuItem.Enabled = removeArgumentToolStripMenuItem.Enabled =
                  expressionTreeView.SelectedNode != null
                  && !expressionTreeView.SelectedNode.Equals(expressionTreeNode)
                  && !expressionTreeView.SelectedNode.Equals(definatrionDomainTreeNode)
                  && !expressionTreeView.SelectedNode.Equals(argumentsTreeNode);
            }
        }

        private void addArgumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArgumentNameEditForm editor = new ArgumentNameEditForm(null);
            editor.Args = (unitProvider as OptimizationUnitProvider).Arguments;
            if (editor.ShowDialog() == DialogResult.OK)
                (unitProvider as OptimizationUnitProvider).AddArgument(editor.OptimizationArgument);
        }

        private void removeArgumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (unitProvider as OptimizationUnitProvider).RemoveArgument(expressionTreeView.SelectedNode.Tag as OptimizationArgument);
        }

        private void renameArgumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptimizationArgument oldArgs = expressionTreeView.SelectedNode.Tag as OptimizationArgument;
            String oldName = oldArgs.Name;
            ArgumentNameEditForm editor = new ArgumentNameEditForm(new OptimizationArgument(oldArgs));
            editor.Args = (unitProvider as OptimizationUnitProvider).Arguments;
            //editor.OptimizationArgument = oldName;
            if (editor.ShowDialog() == DialogResult.OK)
                (unitProvider as OptimizationUnitProvider).RenameArgument(oldName, editor.OptimizationArgument);
        }

        private void argumentUpButton_Click(object sender, EventArgs e)
        {
            if (argumentsTreeNode.Nodes.Contains(expressionTreeView.SelectedNode))
                (unitProvider as OptimizationUnitProvider).MoveArgument(expressionTreeView.SelectedNode.Text, -1);
        }

        private void argumentDownButton_Click(object sender, EventArgs e)
        {
            if (argumentsTreeNode.Nodes.Contains(expressionTreeView.SelectedNode))
                (unitProvider as OptimizationUnitProvider).MoveArgument(expressionTreeView.SelectedNode.Text, 1);
        }

        private void tsbLock_Click(object sender, EventArgs e)
        {
            unitProvider.Lock();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            unitProvider.Save();
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            unitProvider.ClearUnsavedData();
        }
    }
}
