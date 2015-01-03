using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate
{
    public partial class DataUpdateForm : Form
    {
        public DataUpdateForm()
        {
            InitializeComponent();
            RefreshLayout();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable servTable = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();

                cbxHosts.Items.Clear();
                foreach (DataRow item in servTable.Rows)
                {
                    cbxHosts.Items.Add(item[0] + "\\" + item[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void btnRefreshBase_Click(object sender, EventArgs e)
        {
            try
            {
                MyDBdata dbwork = new MyDBdata(ServerType.MSSQL);

                cbxBases.Items.Clear();

                dbwork.DB_User = txtLogin.Text;
                dbwork.DB_Password = txtPassword.Text;
                dbwork.DB_Host = cbxHosts.Text;

                DataTable table = dbwork.GetSchema("Databases");

                foreach (DataRow row in table.Rows)
                {
                    cbxBases.Items.Add(row["database_name"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MyDBdata dbData = new MyDBdata(ServerType.MSSQL);
            dbData.DB_Host = cbxHosts.Text;
            dbData.DB_Name = cbxBases.Text;
            dbData.DB_User = txtLogin.Text;
            dbData.DB_Password = txtPassword.Text;

            try
            {
                dbData.Test();
                MessageBox.Show("Успешно", "Успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            selectConfigPathButton.Enabled = selectConfigPathRadioButton.Checked;

            cbxHosts.Enabled =
                btnRefresh.Enabled =
                cbxBases.Enabled =
                btnRefreshBase.Enabled =
                txtLogin.Enabled =
                txtPassword.Enabled =
                btnTest.Enabled =
                selectDBManualyRadioButton.Checked;
        }

        private void updateDBButton_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentService service = new CurrentService();

                service.RegisterMessageWriter(WriteMessage);

                service.CurrentServiceType = CurrentService.ServiceType.Global;

                service.Settings = GlobalSettings.Instance;

                if (useStandardConfigRadioButton.Checked)
                {
                    service.Settings.Load(GlobalSettings.DefaultConfigPath);
                }
                else if (selectConfigPathRadioButton.Checked)
                {
                    service.Settings.Load(selectConfigPathTextBox.Text);
                }
                else if (selectDBManualyRadioButton.Checked)
                {
                    service.Settings.DataBase.Type = "MSSQL";
                    service.Settings.DataBase.Host = cbxHosts.Text;
                    service.Settings.DataBase.Name = cbxBases.Text;
                    service.Settings.DataBase.User = txtLogin.Text;
                    service.Settings.DataBase.Password = 
                        CommonData.StringToBase64(
                            CommonData.EncryptText(
                                CommonData.StringToSecureString(txtPassword.Text))); 
                }

                UpdateProccess updater = new UpdateProccess(service);

                updater.Proccess();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error have ocuried while update", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteMessage(MessageCategory category, String text)
        {
            textBox1.Text += String.Format("{0}\t{1}\r\n", category, text);
        }
    }
}
