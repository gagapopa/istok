using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.Modules.Tunnel.Properties;
using COTES.ISTOK.Modules;
using COTES.ISTOK;

namespace COTES.ISTOK.Modules.Tunnel
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            String[] levels ={ MessageCategory.Message.ToString(), 
                MessageCategory.Warning.ToString(), MessageCategory.Error.ToString() };
            
            portTextBox.Text = Settings.Default.Port;
            timeOutTextBox.Text = Settings.Default.TimeOut.ToString();
            autoRefreshPeriodTextBox.Text = Settings.Default.AutoRefreshPeriod.ToString();
            messageLevelComboBox.DataSource = levels;
            messageLevelComboBox.SelectedIndex = Settings.Default.MessageLevel;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Port = portTextBox.Text;
            try
            {
                Settings.Default.TimeOut = int.Parse(timeOutTextBox.Text);
            }
            catch (FormatException) { }
            try
            {
                Settings.Default.AutoRefreshPeriod = double.Parse(autoRefreshPeriodTextBox.Text);
            }
            catch (FormatException) { }
            Settings.Default.MessageLevel = messageLevelComboBox.SelectedIndex;

            Settings.Default.Save();
            //Consts.CurrentMessageLevel = (MessageCategory)Settings.Default.MessageLevel;
            //Program.MessageLog.CurrentMessageLevel = (MessageCategory)Settings.Default.MessageLevel;
        }

        private void abortButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Reload();
        }
    }
}