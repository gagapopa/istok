using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class CommonUnitControl : BaseUnitControl
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        UnitNode node;

        public CommonUnitControl(UnitProvider unitProvider)
            : base(unitProvider)
        {
            MulticontrolUnitProvider multiProvider;
            Text = "Свойства";
            InitializeComponent();

            if ((multiProvider = unitProvider as MulticontrolUnitProvider) != null)
                this.UnitProvider = multiProvider.UnitProvider;

            backColor = propertyGrid.ViewBackColor;
        }

        public override void InitiateProcess()
        {
            if (UnitProvider != null && propertyGrid.SelectedObject == null)
            {
                tsbCalc.Visible = UnitProvider.Calculable;

                UnitProvider.CurrentRevisionChanged += new EventHandler(UnitProvider_CurrentRevisionChanged);

                UnitProvider_NewUnitNodeChanged(UnitProvider, EventArgs.Empty);
                UnitProvider.NewUnitNodeChanged += new EventHandler(UnitProvider_NewUnitNodeChanged);

                UnitProvider_EditModeChanged(UnitProvider, EventArgs.Empty);
                UnitProvider.EditModeChanged += new EventHandler(UnitProvider_EditModeChanged);
            }
        }

        void UnitProvider_CurrentRevisionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) 
                Invoke((EventHandler)UnitProvider_CurrentRevisionChanged, sender, e);
            else
            {
                propertyGrid.Refresh(); 
            }
        }

        void UnitProvider_NewUnitNodeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)UnitProvider_NewUnitNodeChanged, sender, e);
            else
            {
                node = unitProvider.NewUnitNode;
                if (propertyGrid.Site == null)
                    propertyGrid.Site = UnitProvider.StructureProvider.GetServiceContainer();

                tsbLock.Enabled = unitProvider.Editable;
                if (!revisionToolStripComboBox.Items.Contains(unitProvider.CurrentRevision))
                    revisionToolStripComboBox.Items.Add(unitProvider.CurrentRevision);
                revisionToolStripComboBox.SelectedItem = unitProvider.CurrentRevision;

                propertyGrid.SelectedObject = node;//CreateDynamicObject();
                UnitProvider_EditModeChanged(sender, e);
            }
        }

        protected override void DisposeControl()
        {
            UnitProvider.CurrentRevisionChanged -= new EventHandler(UnitProvider_CurrentRevisionChanged);
            UnitProvider.NewUnitNodeChanged -= new EventHandler(UnitProvider_NewUnitNodeChanged);
            UnitProvider.EditModeChanged -= new EventHandler(UnitProvider_EditModeChanged);
        }

        Color backColor;
        void UnitProvider_EditModeChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)UnitProvider_EditModeChanged, sender, e);
            else
            {
                //node.ReadOnly = readOnly || !unitProvider.EditMode;
                if (unitProvider.EditMode)
                    propertyGrid.ViewBackColor = SystemColors.Window;
                else
                    propertyGrid.ViewBackColor = backColor; // SystemColors.Control;
                propertyGrid.Refresh();
                tsbLock.Enabled = unitProvider.Editable && !unitProvider.EditMode;
                tsbCancel.Enabled = unitProvider.EditMode;
                tsbSave.Enabled = unitProvider.EditMode;
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //unitProvider.HasChanges = true;
            //unitProvider.NewUnitNode = node;
        }

        private async void tsbLock_Click(object sender, EventArgs e)
        {
            try
            {
                await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => unitProvider.Lock(), true));
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка при взятия узла на редактирование.", exc);
                UniForm.ShowError(exc);
            }
        }

        private async void tsbSave_Click(object sender, EventArgs e)
        {
            try
            {
                //unitProvider.Save();
                var args = new SaveActionArgs(unitProvider, false);
                var workflow = Program.MainForm.WorkflowSelector.GetWorkflow(args);
                await workflow.Do(args);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка сохранения изменений узла.", exc);
                UniForm.ShowError(exc);
            }
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            try
            {
                unitProvider.ClearUnsavedData();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка сброса редактирования узла.", exc);
                UniForm.ShowError(exc);
            }
        }

        private void tsbCalc_Click(object sender, EventArgs e)
        {
            CalcForm calcForm = new CalcForm(unitProvider.StructureProvider, Program.MainForm.Icons);

            DateTime time;
            if (UnitProvider.StructureProvider.LastDateTime > DateTime.MinValue)
                time = UnitProvider.StructureProvider.LastDateTime;
            else
                time = DateTime.Now;

            calcForm.TimeBegin = time;

            calcForm.Parameter = unitProvider.UnitNode;

            calcForm.Show(this);
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

        private void logUnitToolStripButton_Click(object sender, EventArgs e)
        {
            LogUnitNodeEditForm logEditForm = new LogUnitNodeEditForm(unitProvider.StructureProvider);

            logEditForm.ShowDialog();
        }
    }
}
