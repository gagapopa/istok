using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class SettingsForm : Form
    {
        private object obj = null;

        public SettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Объект для настроек
        /// </summary>
        public object Object
        {
            get { return obj; }
            set { obj = value; pgSettings.SelectedObject = obj; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}