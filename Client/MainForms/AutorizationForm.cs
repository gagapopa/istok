using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;

namespace COTES.ISTOK.Client
{
    partial class AutorizationForm : Form
    {
        public string ServerName { get { return tbServerName.Text; } }
        public string UserName { get { return tbUserName.Text; } }
        public string Password { get { return tbPassword.Text; } }

        private readonly int showServerHeight;
        private readonly int hideServerHeight;

        public AutorizationForm(AutorizationForm prevAuthForm)
        {
            InitializeComponent();

            showServerHeight = this.Height;
            hideServerHeight = this.Height - tbServerName.Top + tbPassword.Top;

#if EMA
            label1.Text = "ДКСМ-Клиент";
            showAddressCheckBox.Text = "ДКСМ-Клиент";
#endif
            if (prevAuthForm == null)
            {
                try
                {
                    tbServerName.Text = ClientSettings.Instance.Host + ":" + ClientSettings.Instance.Port.ToString();
                    tbUserName.Text = ClientSettings.Instance.User;
                }
                catch { }

                if (String.IsNullOrEmpty(ServerName) || String.IsNullOrEmpty(UserName))//settings.User))
                    ShowServer();
                else HideServer();
            }
            else
            {
                tbServerName.Text = prevAuthForm.ServerName;
                tbUserName.Text = prevAuthForm.UserName;
                if (prevAuthForm.showAddressCheckBox.Checked)
                    ShowServer();
                else
                    HideServer();
            }
        }

        public void HideServer()
        {
            showAddressCheckBox.Checked = false;
        }
        public void ShowServer()
        {
            showAddressCheckBox.Checked = true;
        }

        public void SaveParams()
        {
            string[] objs = tbServerName.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (objs.Length > 0) ClientSettings.Instance.Host = objs[0];
                if (objs.Length > 1) ClientSettings.Instance.Port = UInt32.Parse(objs[1]);
                ClientSettings.Instance.User = tbUserName.Text;
                ClientSettings.Instance.Save();//ClientSettings.Instance.DefaultConfigFile);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения настроек", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitForm()
        {
            tbPassword.Text = "";
            if (String.IsNullOrEmpty(tbUserName.Text)) this.ActiveControl = tbUserName;
            else this.ActiveControl = tbPassword;
        }
        protected override void OnShown(EventArgs e)
        {
            InitForm();
            base.OnShown(e);
        }        

        private void showAddressCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool show = showAddressCheckBox.Checked;

            tbServerName.Visible = show;
            tbServerName.Enabled = show;
            label1.Visible = show;

            Height = show ? showServerHeight : hideServerHeight;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
