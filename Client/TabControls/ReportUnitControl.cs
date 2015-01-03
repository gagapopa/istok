using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using FastReport;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Контрол формирования отчётов
    /// </summary>
    internal partial class ReportUnitControl : COTES.ISTOK.Client.BaseUnitControl
    {
        ReportUnitProvider reportUnitProvider = null;

        byte[] reportBuffer = null;

        public ReportUnitControl(ReportUnitProvider unitProvider, bool editMode)
            : base(unitProvider)
        {
            InitializeComponent();
            propertyGrid1.Site = unitProvider.StructureProvider.Session.GetServiceContainer();
        }
        public ReportUnitControl(ReportUnitProvider unitProvider)
            : this(unitProvider, false)
        {
            //
        }

        public override void InitiateProcess()
        {
            if (UnitProvider is ReportUnitProvider)
            {
                reportUnitProvider = (ReportUnitProvider)UnitProvider;

                propertyGrid1.SelectedObject = reportUnitProvider.PropertiesContainer;
                propertyGrid1.ExpandAllGridItems();

                chbxSave.Checked = reportUnitProvider.SaveInSystem;

                reportUnitProvider.ReportGenerated += new EventHandler<ReportEventArgs>(reportUnitProvider_ReportGenerated);
            }
        }

        protected override void DisposeControl()
        {
            if (reportUnitProvider != null)
            {
                reportUnitProvider.ReportGenerated -= new EventHandler<ReportEventArgs>(reportUnitProvider_ReportGenerated);
            }
        }

        /// <summary>
        /// Отобразить сформированный отчёт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Аргумент с готовым отчётом</param>
        void reportUnitProvider_ReportGenerated(object sender, ReportEventArgs e)
        {
            FastReport.Report frx = e.Report;

            if (frx != null)
                this.Invoke((Action)(() => frx.ShowPrepared()));
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (reportUnitProvider!=null)
                reportUnitProvider.GenerateReport(); 
        }

        protected override void LockControls()
        {
            if (this.InvokeRequired)
                this.Invoke((Action)LockControls);
            else
            {
                btnCreate.Enabled = false;
                base.LockControls();
            }
        }
        protected override void UnlockControls()
        {
            if (this.InvokeRequired)
                this.Invoke((Action)UnlockControls);
            else
            {
                btnCreate.Enabled = true;
                base.UnlockControls();
            }
        }

        private void chbxSave_CheckedChanged(object sender, EventArgs e)
        {
            reportUnitProvider.SaveInSystem = (sender as CheckBox).Checked;
        }
    }
}
