using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class ButtonEditForm : COTES.ISTOK.Client.UnitEditForm
    {
        public ButtonEditForm()
            : base()
        {
            InitializeComponent();
        }
        public ButtonEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            ShowStatusStripAsyncView();
            //StatusVisible = false;
            //asyncStatusToolStrip.Visible = false;
        }
        public ButtonEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
            ShowStatusStripAsyncView();
            //StatusVisible = false;
            //asyncStatusToolStrip.Visible = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                SaveUnit();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void ShowStatus(string status, double progress)
        {
            if (InvokeRequired) Invoke((Action<String, double>)ShowStatus, status, progress);
            else
            {
                label1.Text = status;
                progressBar1.Value = (int)progress;
                base.ShowStatus(status, progress);
            }
        }

        protected override void ShowStatusStripAsyncView()
        {
            if (InvokeRequired) Invoke((Action)ShowStatusStripAsyncView);
            else
            {
                //lock (watchers)
                //{
                bool visible = !AllWatchersComplete();
                //if (asyncStatusToolStrip != null)
                //    asyncStatusToolStrip.Visible = visible;
                if (button1 != null)
                {
                    button1.Visible = visible;
                    progressBar1.Visible = visible;
                    label1.Visible = visible;
                }
                //}

                base.ShowStatusStripAsyncView();
            }
        }

        private void asyncAbortToolStripButton_Click(object sender, EventArgs e)
        {
            AbortAllAsyncOperations();
        }
    }
}
