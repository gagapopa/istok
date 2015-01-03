using System;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Data;
using System.Net;

namespace COTES.ISTOK.Block.UI
{
    partial class SettingForm : Form
    {
        private ScheduleReg scheduleReg;

        public SettingForm()
        {
            InitializeComponent();
          
            int pos = versionLabel.Left + versionLabel.Width;
            versionLabel.Text = String.Format("v.{0}", VersionInfo.BuildVersion.VersionString/*CommonData.Version*/);
            versionLabel.Left = pos - versionLabel.Width;

#if EMA
            this.Text = "Настройка ДКСМ";
            groupBox3.Text = "ДКСМ-Сервер";
            groupBox1.Text = "ДКСМ-Клиент";
#endif

            InitializeMaintenanceTabPage();
        }

        private void InitializeMaintenanceTabPage()
        {
            dayOfWeekMaintenanceComboBox.Items.Clear();
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Monday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Tuesday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Wednesday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Thursday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Friday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Saturday));
            dayOfWeekMaintenanceComboBox.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Sunday));

            periodMaintenanceComboBox.Items.Clear();
            periodMaintenanceComboBox.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Day));
            periodMaintenanceComboBox.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Week));
            periodMaintenanceComboBox.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Month));

            periodMaintenanceComboBox.Enabled = dayOfWeekMaintenanceComboBox.Enabled =
                maintenanceTimePicker.Enabled = dayMaintenanceNumericUpDown.Enabled =
                txtDeleteCountValues.Enabled = txtKeepValuesDays.Enabled = maintenanceCheckBox.Checked;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPHostEntry local = Dns.GetHostEntry(hostName);

                foreach (IPAddress ipaddress in local.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        cbxInterface.Items.Add(ipaddress.ToString());
                }

                //try
                //{
                //    BlockSettings.Instance.Load(BlockSettings.Instance.DefaultConfigFile);
                //}
                //catch { }

                cmbDbType.Text = BlockSettings.Instance.DataBase.Type;
                if (String.IsNullOrEmpty(cmbDbType.Text)) cmbDbType.Text = "MSSQL";
                cbxHosts.Text = BlockSettings.Instance.DataBase.Host;
                cbxBases.Text = BlockSettings.Instance.DataBase.Name;
                txtDbLoggin.Text = BlockSettings.Instance.DataBase.User;
                txtDbPassword.Text = CommonData.SecureStringToString(
                   CommonData.DecryptText(
                       CommonData.Base64ToString(BlockSettings.Instance.DataBase.Password)));

                maintenanceCheckBox.Checked = BlockSettings.Instance.Maintenance.Enabled;

                try
                {
                    scheduleReg = new ScheduleReg(BlockSettings.Instance.Maintenance.Schedule);
                    periodMaintenanceComboBox.Text = BackUpPeriodFormatter.Format(scheduleReg.Period);
                    dayOfWeekMaintenanceComboBox.Text = DayOfWeekFormatter.Format(scheduleReg.DayOfWeek);
                    maintenanceTimePicker.Value = DateTime.Today + scheduleReg.Time.TimeOfDay;
                    dayMaintenanceNumericUpDown.Value = scheduleReg.Day;
                }
                catch (FormatException) { }

                txtDeleteCountValues.Text = BlockSettings.Instance.Maintenance.ValueDeleteCount.ToString();
                txtKeepValuesDays.Text = BlockSettings.Instance.Maintenance.KeepValuesDays.ToString();

                txtMirroringBackupPath.Text = BlockSettings.Instance.ModulesPath;
                txtMirroringPort.Text = BlockSettings.Instance.MirroringPort.ToString();

                txtReplicationUser.Text = BlockSettings.Instance.ReplicationUser;
                txtReplicationPassword.Text = CommonData.SecureStringToString(
                    CommonData.DecryptText(
                        CommonData.Base64ToString(BlockSettings.Instance.ReplicationPassword)));
                txtServerName.Text = BlockSettings.Instance.ServerName;
                txtGlobalHost.Text = BlockSettings.Instance.GlobalServerHost;
                txtGlobalPort.Text = BlockSettings.Instance.GlobalServerPort.ToString();

                cbxInterface.Text = BlockSettings.Instance.Interface;
                txtPort.Text = BlockSettings.Instance.Port.ToString();
                txtMaxValueCount.Text = BlockSettings.Instance.MaxValuesCount.ToString();

                txtVPNServer.Text = BlockSettings.Instance.Vpn.Server;
                txtVPNName.Text = BlockSettings.Instance.Vpn.Name;
                txtVPNUser.Text = BlockSettings.Instance.Vpn.UserName;
                txtVPNPassword.Text = CommonData.SecureStringToString(
                    CommonData.DecryptText(
                        CommonData.Base64ToString(BlockSettings.Instance.Vpn.Password)));
                chbxVPNUse.Checked = BlockSettings.Instance.Vpn.Use;

                string loaderPath = BlockSettings.Instance.ModulesPath;

                txtModulePath.Text = loaderPath;

                txtRouterIp.Text = BlockSettings.Instance.RouterIp;
                txtTelnetHost.Text = BlockSettings.Instance.TelnetHost;
                txtServerPriority.Text = BlockSettings.Instance.ServerPriority.ToString();

                //logSettings.Configuration = BlockSettings.Instance.Logs;
//#if DEBUG
//                txtReplicationUser.Text = "admin";//@"debug\admin";
//                txtReplicationPassword.Text = "odmin";
//#endif
            }
            catch
            {
                // 
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BlockSettings.Instance.DataBase.Type = cmbDbType.Text;
            BlockSettings.Instance.DataBase.Host = cbxHosts.Text;
            BlockSettings.Instance.DataBase.Name = cbxBases.Text;
            BlockSettings.Instance.DataBase.User = txtDbLoggin.Text;
            BlockSettings.Instance.DataBase.Password =
                CommonData.StringToBase64(
                    CommonData.EncryptText(
                        CommonData.StringToSecureString(txtDbPassword.Text)));


            BlockSettings.Instance.Maintenance.Enabled = maintenanceCheckBox.Checked;
            try
            {
                BackUpPeriod period = BackUpPeriodFormatter.Format(periodMaintenanceComboBox.SelectedItem.ToString());
                DayOfWeek dof = DayOfWeekFormatter.Format(dayOfWeekMaintenanceComboBox.SelectedItem.ToString());
                int day = (int)dayMaintenanceNumericUpDown.Value;
                DateTime time = maintenanceTimePicker.Value;
                scheduleReg = new ScheduleReg(period, dof, day, time);
                BlockSettings.Instance.Maintenance.Schedule = scheduleReg.ToString();
            }
            catch { }

            try { BlockSettings.Instance.Maintenance.ValueDeleteCount = UInt32.Parse(txtDeleteCountValues.Text); }
            catch
            {
                ShowError(String.Format("Число удаляемых параметров должно быть не отрицательным числом меньше {0}...",
                                        UInt32.MaxValue));
                return;
            }
            try { BlockSettings.Instance.Maintenance.KeepValuesDays = UInt32.Parse(txtKeepValuesDays.Text); }
            catch
            {
                ShowError(String.Format("Число дней хранения параметров должно быть не отрицательным числом меньше {0}...",
                                        UInt32.MaxValue));
                return;
            }
            BlockSettings.Instance.MirroringBackupPath = txtMirroringBackupPath.Text;

            try { BlockSettings.Instance.MirroringPort = UInt32.Parse(txtMirroringPort.Text); }
            catch
            {
                ShowError(String.Format("Порт зеркалирования должен быть не отрицательным числом меньше {0}...",
                                        UInt32.MaxValue));
                return;
            }
            BlockSettings.Instance.ReplicationUser = txtReplicationUser.Text;
            BlockSettings.Instance.ReplicationPassword =
                CommonData.StringToBase64(
                    CommonData.EncryptText(
                        CommonData.StringToSecureString(txtReplicationPassword.Text)));

            BlockSettings.Instance.ServerName = txtServerName.Text;
            BlockSettings.Instance.GlobalServerHost = txtGlobalHost.Text;
            try { BlockSettings.Instance.GlobalServerPort = UInt32.Parse(txtGlobalPort.Text); }
            catch
            {
                ShowError(String.Format("Порт ДКСМ клиента должен быть не отрицательным числом меньше {0}...",
                                         UInt32.MaxValue));
                return;
            }

            BlockSettings.Instance.Interface = cbxInterface.Text;

            try { BlockSettings.Instance.Port = UInt32.Parse(txtPort.Text); }
            catch
            {
                ShowError(String.Format("Номер порта должен быть не отрицательным числом меньше {0}...",
                                        UInt32.MaxValue));
                return;
            }
            try { BlockSettings.Instance.MaxValuesCount = UInt32.Parse(txtMaxValueCount.Text); }
            catch
            {
                ShowError(String.Format("Максимальное число параметров должно быть не отрицательным числом меньше {0}...",
                                        UInt32.MaxValue));
                return;
            }

            try { BlockSettings.Instance.ServerPriority = Byte.Parse(txtServerPriority.Text); }
            catch
            {
                ShowError(String.Format("Приоритет сервера должне быть числом между {0} и {1}", 
                                        Byte.MinValue, 
                                        Byte.MaxValue));
                return;
            }

            BlockSettings.Instance.ModulesPath = txtModulePath.Text;

            BlockSettings.Instance.TelnetHost = txtTelnetHost.Text;
            BlockSettings.Instance.RouterIp = txtRouterIp.Text;

            VPNSettings vpns = new VPNSettings();
            vpns.Use = chbxVPNUse.Checked;
            vpns.Name = txtVPNName.Text;
            vpns.Server = txtVPNServer.Text;
            vpns.UserName = txtVPNUser.Text;
            vpns.Password = CommonData.StringToBase64(
                                CommonData.EncryptText(
                                    CommonData.StringToSecureString(txtVPNPassword.Text)));

            BlockSettings.Instance.Vpn = vpns;

            //BlockSettings.Instance.Logs = logSettings.Configuration;

            BlockSettings.Instance.Save();//BlockSettings.Instance.DefaultConfigFile);

            Program.UpdateService();
            if (Program.BlockSvc != null)
            {
                DialogResult dr;

                if (Program.BlockSvc.Status == ServiceControllerStatus.Running)
                {
                    dr = MessageBox.Show("Перезапустить службу?", "Подтверждение", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            Program.BlockSvc.Stop();
                            Program.BlockSvc.WaitForStatus(ServiceControllerStatus.Stopped);
                            Program.BlockSvc.Start();
                            Program.BlockSvc.WaitForStatus(ServiceControllerStatus.Running);
                            MessageBox.Show("Служба успешно перезапущена", Program.BlockSvc.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show("Ошибка перезапуска службы", "Ошибка");
                        }
                    }
                }
                else if (Program.BlockSvc.Status == ServiceControllerStatus.Stopped)
                {
                    dr = MessageBox.Show("Запустить службу?", "Подтверждение", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            Program.BlockSvc.Start();
                            Program.BlockSvc.WaitForStatus(ServiceControllerStatus.Running);
                            MessageBox.Show("Служба успешно запущена", Program.BlockSvc.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show("Ошибка перезапуска службы", "Ошибка");
                        }
                    }
                }
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, 
                            "Ошибка", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ServerType serverType = ServerTypeFormatter.Format(cmbDbType.Text);
                Creator.TestConnection(serverType, cbxHosts.Text, cbxBases.Text, txtDbLoggin.Text, txtDbPassword.Text);
                MessageBox.Show("Успешно", "Успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            folderBrowserDialogModulePathSelector.SelectedPath = txtModulePath.Text;
            if (folderBrowserDialogModulePathSelector.ShowDialog(this) == DialogResult.OK)
            {
                txtModulePath.Text = folderBrowserDialogModulePathSelector.SelectedPath;
            }
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void monitoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModulesMonitorForm.MonitorForm.Show();
            if (ModulesMonitorForm.MonitorForm.WindowState == FormWindowState.Minimized)
                ModulesMonitorForm.MonitorForm.WindowState = FormWindowState.Normal;
            ModulesMonitorForm.MonitorForm.Exec();
            ModulesMonitorForm.MonitorForm.BringToFront();

        }

        private void maintenanceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            periodMaintenanceComboBox.Enabled = 
                maintenanceTimePicker.Enabled =
                txtDeleteCountValues.Enabled = 
                txtKeepValuesDays.Enabled = 
                dayMaintenanceNumericUpDown.Enabled = maintenanceCheckBox.Checked;
            
            if (maintenanceCheckBox.Checked)
                periodMaintenanceComboBox_SelectedIndexChanged(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ServerType serverType = ServerTypeFormatter.Format(cmbDbType.Text);

                DataTable servTable = Creator.GetDataSource(serverType);

                cbxHosts.Items.Clear();
                if (servTable != null)
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
                ServerType serverType = ServerTypeFormatter.Format(cmbDbType.Text);

                cbxBases.Items.Clear();
                cbxBases.Items.AddRange(Creator.GetBases(serverType, cbxHosts.Text, txtDbLoggin.Text, txtDbPassword.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении баз данных: " + ex.Message, "Ошибка");
            }
        }

        private void periodMaintenanceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BackUpPeriod period = BackUpPeriodFormatter.Format(periodMaintenanceComboBox.Text);

            dayOfWeekMaintenanceComboBox.Enabled = period == BackUpPeriod.Week;

            dayMaintenanceNumericUpDown.Enabled = period == BackUpPeriod.Month;
        }

        private void SettingForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.Opacity = 1;
        }

        private void serviceContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.UpdateService();
            startServiceToolStripMenuItem.Enabled = false;
            stopServiceToolStripMenuItem.Enabled = false;
            if (Program.BlockSvc != null)
            {
                if (Program.BlockSvc.Status == ServiceControllerStatus.Running)
                    stopServiceToolStripMenuItem.Enabled = true;
                else
                    if (Program.BlockSvc.Status == ServiceControllerStatus.Stopped)
                        startServiceToolStripMenuItem.Enabled = true;
            }
        }

        private void startServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.UpdateService();
                if (Program.BlockSvc != null)
                    if (Program.BlockSvc.Status == ServiceControllerStatus.Stopped)
                        Program.BlockSvc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void stopServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.UpdateService();
                if (Program.BlockSvc != null)
                    if (Program.BlockSvc.Status == ServiceControllerStatus.Running)
                        Program.BlockSvc.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void filltrateOnlyDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar);
        }

        private void chbxVPNUse_CheckedChanged(object sender, EventArgs e)
        {
            txtVPNName.Enabled = 
                txtVPNPassword.Enabled = 
                txtVPNServer.Enabled = 
                txtVPNUser.Enabled = chbxVPNUse.Checked;
        }
    }
}
