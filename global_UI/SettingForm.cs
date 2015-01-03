using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.Assignment;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Data.Sql;
using System.IO;
using System.Net;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.Assignment.UI
{
    public partial class SettingForm : Form
    {
        private MessageCategoryTypeConverter messConverter = new MessageCategoryTypeConverter();
        private ServiceController globalsvc = null;
        private string servicePath = "";

        private BackUpSettings buDiff = null;
        private BackUpSettings buFull = null;
        private BackUpSettings buCur = null;

#if EMA
        private const string regFormat = @"Зарегистрировано для: {0} (на {1} ДКСМ-Серверов)"; 
#else
        private const string regFormat = @"Зарегистрировано для: {0} (на {1} серверов сбора данных)";
#endif

        public SettingForm()
        {
            InitializeComponent();
            int pos = versionLabel.Left + versionLabel.Width;
            versionLabel.Text = String.Format("v.{0}", VersionInfo.BuildVersion.VersionString /*CommonData.Version*/);
            versionLabel.Left = pos - versionLabel.Width;

#if EMA
            this.Text = "Настройка ДКСМ-Клиента";
            startToolStripMenuItem.Text = "Запустить клиент";
            stopToolStripMenuItem.Text = "Остановить клиент";
#endif

            LoadServerTypes();
            LoadBackUpTypes();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            try
            {
                //try
                //{
                //    GlobalSettings.Instance.Load(GlobalSettings.Instance.DefaultConfigFile);
                //}
                //catch
                //{ }

                cmbDbType.Text = GlobalSettings.Instance.DataBase.Type;
                cbxBases.Text = GlobalSettings.Instance.DataBase.Name;
                cbxHosts.Text = GlobalSettings.Instance.DataBase.Host;
                txtLogin.Text = GlobalSettings.Instance.DataBase.User;
                txtPassword.Text =
                    CommonData.SecureStringToString(
                        CommonData.DecryptText(
                            CommonData.Base64ToString(GlobalSettings.Instance.DataBase.Password)));

                txtRemotingPort.Text = GlobalSettings.Instance.Port.ToString();
                txtInterface.Text = GlobalSettings.Instance.Host.ToString();
                txtMaxValueCountPerQuery.Text = GlobalSettings.Instance.ValuesMaxCount.ToString();

                txtCalculationPeriod.Text = GlobalSettings.Instance.CalculationInterval.ToString();
                txtParameterConstraction.Text = GlobalSettings.Instance.ParametersConstraction.ToString();
                txtMaxLoopCount.Text = GlobalSettings.Instance.LoopConstraction.ToString();

                buDiff = GlobalSettings.Instance.BackUpDiff;
                buFull = GlobalSettings.Instance.BackUpFull;

                // decrypt data base recvisits in profiles

                //var log_conf = GlobalSettings.Instance.Logs;
                //foreach (var it in log_conf.Journals)
                //    if (it is DataBaseProfile)
                //    {
                //        var profile = it as DataBaseProfile;
                //        profile.ConnectionString =
                //            CommonData.SecureStringToString(
                //                CommonData.DecryptText(
                //                    CommonData.Base64ToString(profile.ConnectionString)));
                //    }

                //logSettingsPanel.Configuration = log_conf;
            }
            catch
            {
                buFull = new BackUpSettings();
                buDiff = new BackUpSettings();
            }
            finally
            {
                cbxBackupType.SelectedIndex = 0;
            }
        }

        private void LoadServerTypes()
        {
            cmbDbType.Items.Clear();

            cmbDbType.Items.Add(ServerTypeFormatter.Format(ServerType.MSSQL));
        }
        private void LoadBackUpTypes()
        {
            cbxBackupType.Items.Clear();
            cbxBackupType.Items.Add("Дифференциальный");
            cbxBackupType.Items.Add("Полный");

            cbxBackupDayOfWeek.Items.Clear();
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Monday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Tuesday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Wednesday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Thursday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Friday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Saturday));
            cbxBackupDayOfWeek.Items.Add(DayOfWeekFormatter.Format(DayOfWeek.Sunday));

            cbxBackupPeriod.Items.Clear();
            cbxBackupPeriod.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Day));
            cbxBackupPeriod.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Week));
            cbxBackupPeriod.Items.Add(BackUpPeriodFormatter.Format(BackUpPeriod.Month));
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            GlobalSettings.Instance.DataBase.Type = cmbDbType.Text;
            GlobalSettings.Instance.DataBase.Name = cbxBases.Text;
            GlobalSettings.Instance.DataBase.Host = cbxHosts.Text;
            GlobalSettings.Instance.DataBase.User = txtLogin.Text;
            GlobalSettings.Instance.DataBase.Password =
                CommonData.StringToBase64(
                   CommonData.EncryptText(
                       CommonData.StringToSecureString(txtPassword.Text)));

            try { GlobalSettings.Instance.Port = UInt32.Parse(txtRemotingPort.Text); }
            catch
            {
                ShowError("Номер порта должен быть не отрицательным числом...");
                return;
            }
            GlobalSettings.Instance.Host = txtInterface.Text;
            try { GlobalSettings.Instance.ValuesMaxCount = UInt32.Parse(txtMaxValueCountPerQuery.Text); }
            catch
            {
                ShowError("Количество значений должно быть не отрицательным числом...");
                return;
            }

            SaveBackupSettings();
            buDiff.Type = BackUpType.Differential;
            buFull.Type = BackUpType.Full;
            buDiff.DataBase = buFull.DataBase = cbxBases.Text;

            GlobalSettings.Instance.BackUpDiff = buDiff;
            GlobalSettings.Instance.BackUpFull = buFull;
            try { GlobalSettings.Instance.LoopConstraction = UInt32.Parse(txtMaxLoopCount.Text); }
            catch 
            {
                ShowError("Ограничение циклов должно быть не отрицательным числом...");
                return;
            }

            try { GlobalSettings.Instance.ParametersConstraction = UInt32.Parse(txtParameterConstraction.Text); }
            catch
            {
                ShowError("Ограниечение параметров должно быть не отрицательным числом...");
                return;
            }
            try { GlobalSettings.Instance.CalculationInterval = UInt32.Parse(txtCalculationPeriod.Text); }
            catch
            {
                ShowError("Интервал расчета должен быть не отрицательным числом...");
                return;
            }


            // crypt data base recvisits in profiles

            //GlobalSettings.Instance.Logs = this.logSettingsPanel.Configuration;

            GlobalSettings.Instance.Save();//GlobalSettings.Instance.DefaultConfigFile);


            //
            // DOTO: this is need refactoring
            //
            if (CheckService() && globalsvc != null)
            {
                DialogResult dr;

                if (globalsvc.Status == ServiceControllerStatus.Running)
                {
                    dr = MessageBox.Show("Перезапустить службу?", "Подтверждение", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            globalsvc.Stop();
                            globalsvc.WaitForStatus(ServiceControllerStatus.Stopped);
                            globalsvc.Start();
                            globalsvc.WaitForStatus(ServiceControllerStatus.Running);
                            MessageBox.Show("Служба успешно перезапущена", globalsvc.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show("Ошибка перезапуска службы", "Ошибка");
                        }
                    }
                }
                else
                    if (globalsvc.Status == ServiceControllerStatus.Stopped)
                    {
                        dr = MessageBox.Show("Запустить службу?", "Подтверждение", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            try
                            {
                                globalsvc.Start();
                                globalsvc.WaitForStatus(ServiceControllerStatus.Running);
                                MessageBox.Show("Служба успешно запущена", globalsvc.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (InvalidOperationException)
                            {
                                MessageBox.Show("Ошибка перезапуска службы", "Ошибка");
                            }
                        }
                    }
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MyDBdata dbData = new MyDBdata(ServerTypeFormatter.Format(cmbDbType.Text));
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

        private void ShowForm()
        {
            if (this.Visible)
                this.Hide();
            else
                this.Show();

            RegestrationInfo();

            //if (registeredByLabel.Visible = registerDateLabel.Visible =
            //    !(btnRegister.Visible = !CommonData.CheckRegister(GNSI.LicenseFile)))//servicePath)))
            //{
            //    registeredByLabel.Text = string.Format(regFormat, CommonData.RegisterOrganization, CommonData.MaxBlockCount);
            //    registerDateLabel.Text = CommonData.RegisterDate.ToString("dd-MMMM-yyyy");
            //}
        }

        private void RegestrationInfo()
        {
            try
            {
                if (CommonData.CheckRegister(GNSI.LicenseFile))
                {
                    registeredByLabel.Visible = true;
                    registerDateLabel.Visible = true;
                    registeredByLabel.Text = string.Format(regFormat, CommonData.RegisterOrganization, CommonData.MaxBlockCount);
                    registerDateLabel.Text = CommonData.RegisterDate.ToString("dd-MMMM-yyyy");
                    btnRegister.Text = "Перерегистрация";
                }
                else
                {
                    registeredByLabel.Visible = false;
                    registerDateLabel.Visible = false;
                    btnRegister.Text = "Регистрация";
                }
            }
            catch (Exception)
            {
                //registeredByLabel.Visible = false;
                //registerDateLabel.Visible = false;
                //btnRegister.Visible = false;
                registeredByLabel.Visible = false;
                registerDateLabel.Visible = false;
                btnRegister.Text = "Регистрация";
            }
        }

        private void serviceIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void SettingForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.Opacity = 1;
        }

        private bool CheckService()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            bool res = true;

            globalsvc = null;
            foreach (ServiceController item in scServices)
            {
                if (item.ServiceName == CommonData.GlobalServiceName)
                {
                    globalsvc = item;
                    servicePath = GetImagePath(globalsvc.ServiceName);
                    break;
                }
            }

            if (globalsvc == null)
            {
                res = false;
                //MessageBox.Show("Служба не установлена", "Ошибка");
            }

            return res;
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void SettingForm_VisibleChanged(object sender, EventArgs e)
        {
        }

        private string GetImagePath(string serviceName)
        {
            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            RegistryKey keyHKLM = Registry.LocalMachine;
            RegistryKey key;
            string value;
            int i;

            key = keyHKLM.OpenSubKey(registryPath);

            value = key.GetValue("ImagePath").ToString();
            key.Close();
            value = Environment.ExpandEnvironmentVariables(value);

            i = value.LastIndexOf('\\');

            if (i != -1)
                value = value.Substring(1, i - 1);

            return value;
        }

        private void cbxBackup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveBackupSettings();
            switch (cbxBackupType.SelectedIndex)
            {
                case 0:
                    buCur = buDiff;
                    break;
                case 1:
                    buCur = buFull;
                    break;
            }
            UpdateBackupSettings(buCur);
        }

        private void SaveBackupSettings()
        {
            if (buCur == null) return;

            buCur.Name = txtBackupName.Text;
            buCur.Description = txtBackupDescription.Text;
            buCur.FileName = txtBackupFileName.Text;
            buCur.InUse = chkDiff.Checked;
            buCur.DayOfWeek = DayOfWeekFormatter.Format(cbxBackupDayOfWeek.Text);
            buCur.Period = BackUpPeriodFormatter.Format(cbxBackupPeriod.Text);
            try
            {
                buCur.Time = DateTime.Parse(mtbBackupTime.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка ввода времени.", "Ошибка");
                buCur.Time = DateTime.MinValue;
            }
            buCur.TTL = (int)udDiffTTL.Value;
            buCur.Day = (int)udBackupDay.Value;
        }
        private void UpdateBackupSettings(BackUpSettings bu)
        {
            if (bu == null) bu = new BackUpSettings(BackUpType.Full);

            txtBackupName.Text = bu.Name;
            txtBackupDescription.Text = bu.Description;
            txtBackupFileName.Text = bu.FileName;
            chkDiff.Checked = bu.InUse;
            cbxBackupDayOfWeek.Text = DayOfWeekFormatter.Format(bu.DayOfWeek);
            cbxBackupPeriod.Text = BackUpPeriodFormatter.Format(bu.Period);
            mtbBackupTime.Text = bu.Time.ToString("HH:mm");
            udDiffTTL.Value = (decimal)bu.TTL;
            udBackupDay.Value = (decimal)bu.Day;

            UpdateView();
        }

        private void UpdateView()
        {
            bool use = chkDiff.Checked;

            txtBackupName.Enabled = use;
            txtBackupDescription.Enabled = use;
            txtBackupFileName.Enabled = use;
            cbxBackupPeriod.Enabled = use;
            mtbBackupTime.Enabled = use;
            udDiffTTL.Enabled = use;

            udBackupDay.Enabled = (use && BackUpPeriodFormatter.Format(cbxBackupPeriod.Text) == BackUpPeriod.Month);
            cbxBackupDayOfWeek.Enabled = (use && BackUpPeriodFormatter.Format(cbxBackupPeriod.Text) == BackUpPeriod.Week);
        }

        private void cbxDiffPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void chkDiff_CheckedChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            byte[] regKey;
            EnterRegisterKeyForm enterForm = new EnterRegisterKeyForm();
            enterForm.MachineCode = CommonData.GetMachineCode();
            if (enterForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    regKey = enterForm.EnteredKey;
                    if (CommonData.CheckRegisterKey(regKey))
                    {
                        FileInfo finfo = new FileInfo(GNSI.LicenseFile);
                        if (!finfo.Directory.Exists) finfo.Directory.Create();
                        FileStream file = new FileStream(GNSI.LicenseFile, FileMode.Create);
                        file.Write(regKey, 0, regKey.Length);
                        file.Close();
                        //btnRegister.Visible = false;
                        //registeredByLabel.Visible = registerDateLabel.Visible = true;

                        //registeredByLabel.Text = string.Format(regFormat, CommonData.RegisterOrganization, CommonData.MaxBlockCount);
                        //registerDateLabel.Text = CommonData.RegisterDate.ToString("dd-MMMM-yyyy");
                        RegestrationInfo();
                    }
                    else MessageBox.Show("Введенный код неверен");
                }
                catch (System.Security.SecurityException exc)
                {
                    MessageBox.Show(exc.Message, "Недостаточно прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Сбой при проверке лицензии", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var principal = new System.Security.Principal.WindowsPrincipal(
                    System.Security.Principal.WindowsIdentity.GetCurrent());

                if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    if (CheckService() && globalsvc != null)
                        if (globalsvc.Status == ServiceControllerStatus.Stopped)
                            globalsvc.Start(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckService() && globalsvc != null)
                    if (globalsvc.Status == ServiceControllerStatus.Running)
                        globalsvc.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable servTable = SqlDataSourceEnumerator.Instance.GetDataSources();

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
                MyDBdata dbwork = new MyDBdata(ServerTypeFormatter.Format(cmbDbType.Text));

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

        private void filtrateOnlyDigigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar);
        }

        private void contextMenuStripMain_Opening(object sender, CancelEventArgs e)
        {
            //Program.UpdateService();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;

            if (CheckService() && globalsvc != null)
            {
                if (globalsvc.Status == ServiceControllerStatus.Running)
                    stopToolStripMenuItem.Enabled = true;
                else
                    if (globalsvc.Status == ServiceControllerStatus.Stopped)
                        startToolStripMenuItem.Enabled = true;
            }

        }

        private void btnCreateDateBase_Click(object sender, EventArgs e)
        {
            CreateDBForm createForm = new CreateDBForm();
            createForm.Show(this);
        }
    }
}
