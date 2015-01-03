using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования констант и пользовательских функций
    /// </summary>
    partial class ConstantEditForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        List<ConstsInfo> removingConsts = new List<ConstsInfo>();

        public ConstantEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(functionsTabPage);
        }

        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                //AsyncOperationWatcher<Object> watcher = 
                var consts = await Task.Factory.StartNew(() => strucProvider.Session.GetConsts());
                constReceived(consts);

                //watcher.AddValueRecivedHandler(v => constReceived(v as IEnumerable<ConstsInfo>));
                //RunWatcher(watcher);

                //watcher = 
                var funcs = await Task.Factory.StartNew(() => strucProvider.Session.GetCustomFunctions());
                functionReceived(funcs);
                //watcher.AddValueRecivedHandler(v => functionReceived(v as IEnumerable<CustomFunctionInfo>));
                //RunWatcher(watcher);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка при просмотре констант расчета.", exc);
                Program.MainForm.ShowError(exc);
            }
        }

        void constReceived(IEnumerable<ConstsInfo> consts)
        {
            if (InvokeRequired) BeginInvoke((Action<IEnumerable<ConstsInfo>>)constReceived, consts);
            else
            {
                DataGridViewRow gridRow;
                foreach (var item in consts)
                {
                    gridRow = constantDataGridView.Rows[constantDataGridView.Rows.Add()];
                    gridRow.Tag = item;
                    gridRow.Cells[ConstNameColumn.Index].Value = item.Name;
                    gridRow.Cells[constDescriptionColumn.Index].Value = item.Description;
                    gridRow.Cells[ConstValueColumn.Index].Value = item.Value;
                    gridRow.ReadOnly = !item.Editable;
                }
            }
        }

        void functionReceived(IEnumerable<CustomFunctionInfo> functions)
        {
            if (InvokeRequired) BeginInvoke((Action<IEnumerable<CustomFunctionInfo>>)functionReceived, functions);
            else
            {
                DataGridViewRow gridRow;
                foreach (var item in functions)
                {
                    gridRow = functionsDataGridView.Rows[functionsDataGridView.Rows.Add()];
                    gridRow.Tag = item;
                    gridRow.Cells[functionNameColumn.Index].Value = item.Name;
                    gridRow.Cells[functionGroupColumn.Index].Value = item.GroupName;
                    gridRow.Cells[functionDescriptionColumn.Index].Value = item.Comment;
                }
            }
        }

        CustomFunctionInfo currentEditedFunction;

        private void functionsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow gridRow;
            CustomFunctionInfo functionInfo;
            FormulaUnitProvider formulaProvider;

            if (currentEditedFunction != null)
            {
                formulaProvider = formulaEditControl1.UnitProvider as FormulaUnitProvider;
                currentEditedFunction.Text = formulaProvider.Formula;
            }

            if (functionsDataGridView.CurrentCell != null
                && (gridRow = functionsDataGridView.Rows[functionsDataGridView.CurrentCell.RowIndex]) != null
                && !gridRow.IsNewRow)
            {
                if ((functionInfo = gridRow.Tag as CustomFunctionInfo) == null)
                {
                    functionInfo = new CustomFunctionInfo(0, null, null, null, String.Empty);
                    gridRow.Tag = functionInfo;
                }
                formulaProvider = new FormulaUnitProvider(strucProvider);
                formulaEditControl1.UnitProvider = formulaProvider;
                formulaEditControl1.Enabled = true;
                //formulaProvider.Formula = functionInfo.Text;
                formulaEditControl1.Formula = functionInfo.Text;
                currentEditedFunction = functionInfo;
            }
            else
            {
                formulaEditControl1.Enabled = false;
                formulaEditControl1.Formula = null;
                currentEditedFunction = null;
            }
        }

        private void removeConstantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            ConstsInfo constant;

            foreach (DataGridViewCell cell in constantDataGridView.SelectedCells)
                if (!selectedRows.Contains(constantDataGridView.Rows[cell.RowIndex]))
                    selectedRows.Add(constantDataGridView.Rows[cell.RowIndex]);

            foreach (var item in selectedRows)
            {
                if ((constant = item.Tag as ConstsInfo) != null && constant.Editable)
                {
                    removingConsts.Add(constant);
                    constantDataGridView.Rows.Remove(item);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ConstsInfo constsInfo;
            CustomFunctionInfo functionInfo;
            String constName, constDescription, constValue;
            List<ConstsInfo> savingConsts = new List<ConstsInfo>();
            List<CustomFunctionInfo> savingFunctions = new List<CustomFunctionInfo>();

            try
            {
                foreach (DataGridViewRow gridRow in constantDataGridView.Rows)
                {
                    if (!gridRow.IsNewRow)
                    {
                        if ((constsInfo = gridRow.Tag as ConstsInfo) != null && !constsInfo.Editable) continue;

                        constName = gridRow.Cells[ConstNameColumn.Index].Value.ToString();
                        constDescription = gridRow.Cells[constDescriptionColumn.Index].Value.ToString();
                        constValue = gridRow.Cells[ConstValueColumn.Index].Value.ToString();

                        if (constsInfo == null)
                            savingConsts.Add(constsInfo = new ConstsInfo(constName, constDescription, constValue));
                        else if (!String.Equals(constName, constsInfo.Name)
                            || !String.Equals(constDescription, constsInfo.Description)
                            || !String.Equals(constValue, constsInfo.Value))
                        {
                            constsInfo.Name = constName;
                            constsInfo.Description = constDescription;
                            constsInfo.Value = constValue;
                            savingConsts.Add(constsInfo);
                        }
                    }
                }

                if (savingConsts.Count > 0)
                {
                    //AsyncOperationWatcher watcher = 
                    strucProvider.Session.SaveConsts(savingConsts.ToArray());
                    //RunWatcher(watcher);
                }
                if (removingConsts.Count > 0)
                {
                    //AsyncOperationWatcher removingWatcher = 
                    strucProvider.Session.RemoveConsts(removingConsts.ToArray());
                    removingConsts.Clear();
                    //RunWatcher(removingWatcher);
                }

                foreach (DataGridViewRow gridRow in functionsDataGridView.Rows)
                {
                    if (!gridRow.IsNewRow)
                    {
                        if ((functionInfo = gridRow.Tag as CustomFunctionInfo) == null)
                            functionInfo = new CustomFunctionInfo(0, null, null, null, null);
                        functionInfo.Name = gridRow.Cells[functionNameColumn.Index].Value == null ? null :
                            gridRow.Cells[functionNameColumn.Index].Value.ToString();
                        functionInfo.Comment = gridRow.Cells[functionDescriptionColumn.Index].Value == null ? null :
                            gridRow.Cells[functionDescriptionColumn.Index].Value.ToString();
                        functionInfo.GroupName = gridRow.Cells[functionGroupColumn.Index].Value == null ? null :
                            gridRow.Cells[functionGroupColumn.Index].Value.ToString();
                        savingFunctions.Add(functionInfo);
                    }
                }

                if (savingFunctions.Count > 0)
                {
                    //AsyncOperationWatcher watcher = 
                    strucProvider.Session.SaveCustomFunctions(savingFunctions.ToArray());
                    //RunWatcher(watcher);
                }


                CloseOnFinishWatcher = true;
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка при редактировании констант расчета.", exc);
                Program.MainForm.ShowError(exc);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
