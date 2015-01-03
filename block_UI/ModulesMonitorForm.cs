using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.Block;
using System.Collections;
using COTES.ISTOK.DiagnosticsInfo;
using System.ServiceModel;

namespace COTES.ISTOK.Block.UI
{
    public sealed partial class ModulesMonitorForm : Form
    {
        private static ModulesMonitorForm monitorForm = null;
        //private Diagnostics blockDiagnostics = null;

        private ChannelFactory<IDiagnostics> Block
        {
            get;
            set;
            //get
            //{
            //    try { if (blockDiagnostics != null)blockDiagnostics.Test(null); }
            //    catch (RemotingException) { blockDiagnostics = null; }
            //    catch (System.Net.Sockets.SocketException) { blockDiagnostics = null; }
            //    return blockDiagnostics;
            //}
            //set { blockDiagnostics = value; }
        }

        public static ModulesMonitorForm MonitorForm
        {
            get
            {
                if (monitorForm == null) monitorForm = new ModulesMonitorForm();
                return monitorForm;
            }
        }


        public ModulesMonitorForm()
        {
            InitializeComponent();

            //if (Block == null) Block = CreateBlockDiagnostics();
            //channelMonitoringControl1.ChannelDiagnostics = Block;
            Exec();
        }

        private void ModulesMonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            { this.WindowState = FormWindowState.Minimized; e.Cancel = true; }
        }

        private void ModulesMonitorForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) MonitorForm.Hide();
        }

        public void Exec()
        {
            try
            {
                if (Block != null)
                    Block.Close();
            }
            catch (CommunicationException) { }

            //if (Block == null)
            //{
            Block = CreateBlockDiagnostics();
            channelMonitoringControl1.ChannelDiagnostics = Block.CreateChannel();
            //}
            channelMonitoringControl1.RefreshChannelsInfo();
        }

        private ChannelFactory<IDiagnostics> CreateBlockDiagnostics()
        {
            const String urlFormat = "net.tcp://{0}:{1}/{2}";

            ////ClientSettings.FileName = NSI.ConfigFile;
            String url = String.Format(urlFormat,
                                       "localhost",
                                       BlockSettings.Instance.Port,//ClientSettings.Instance.LoadKey("Settings/Remoting_port"),
                                       "BlockDiagnostics");//CommonData.BlockDiagnosticsURI);
            //Diagnostics remoteObject = (Diagnostics)Activator.GetObject(typeof(Diagnostics), url);
            //return remoteObject;

            ChannelFactory<IDiagnostics> factory;
            //String url;

            //if (!dicBlockUIDs.TryGetValue(uid_block, out factory))
            //if (!dicBlockUIDs.TryGetValue(uid_block, out url))
            //{
            //    throw new ArgumentException("uid_block");
            //}

            EndpointAddress address = new EndpointAddress(url);
            factory = new ChannelFactory<IDiagnostics>("NetTcpBinding_BlockDiagnostics", address);
            factory.Open();
            //return factory.CreateChannel();
            return factory;

            //return gb.GetDiagnosticsObject();


            //if (factory.State != CommunicationState.Opened)
            //{
            //    factory.Open();
            //}

            //return factory.CreateChannel();

        }

        private void autoRefreshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            intervalTextBox.Enabled = autoRefreshCheckBox.Checked;
            timer1.Enabled = autoRefreshCheckBox.Checked;
        }

        private void intervalTextBox_TextChanged(object sender, EventArgs e)
        {
            if (autoRefreshCheckBox.Checked)
            {
                double tick;
                try
                {
                    tick = double.Parse(intervalTextBox.Text);
                    if (tick <= 0) tick = 1;
                }
                catch { tick = 1; }
                timer1.Interval = (int)(tick * 1000);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            Exec();
        }

        private void gcCollecectButton_Click(object sender, EventArgs e)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
    }
}
