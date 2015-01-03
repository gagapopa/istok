namespace COTES.ISTOK.Block.UI
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabMisc = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtServerPriority = new System.Windows.Forms.TextBox();
            this.labelServerPriority = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.btnOpenPathToModuls = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.txtModulePath = new System.Windows.Forms.TextBox();
            this.txtMaxValueCount = new System.Windows.Forms.TextBox();
            this.cbxInterface = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtGlobalPort = new System.Windows.Forms.TextBox();
            this.txtGlobalHost = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tabDb = new System.Windows.Forms.TabPage();
            this.chkWA = new System.Windows.Forms.CheckBox();
            this.btnRefreshBase = new System.Windows.Forms.Button();
            this.cmbDbType = new System.Windows.Forms.ComboBox();
            this.cbxBases = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxHosts = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDbLoggin = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtDbPassword = new System.Windows.Forms.TextBox();
            this.tabMaintanceDb = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.maintenanceCheckBox = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDeleteCountValues = new System.Windows.Forms.TextBox();
            this.maintenanceTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.dayMaintenanceNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.txtKeepValuesDays = new System.Windows.Forms.TextBox();
            this.dayOfWeekMaintenanceComboBox = new System.Windows.Forms.ComboBox();
            this.periodMaintenanceComboBox = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvServers = new System.Windows.Forms.DataGridView();
            this.clmHost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label25 = new System.Windows.Forms.Label();
            this.txtMirroringPort = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtReplicationPassword = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtMirroringBackupPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtReplicationUser = new System.Windows.Forms.TextBox();
            this.tpVPN = new System.Windows.Forms.TabPage();
            this.label27 = new System.Windows.Forms.Label();
            this.txtVPNName = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtVPNPassword = new System.Windows.Forms.TextBox();
            this.txtVPNUser = new System.Windows.Forms.TextBox();
            this.txtVPNServer = new System.Windows.Forms.TextBox();
            this.chbxVPNUse = new System.Windows.Forms.CheckBox();
            this.tabPageNet = new System.Windows.Forms.TabPage();
            this.txtTelnetHost = new System.Windows.Forms.TextBox();
            this.txtRouterIp = new System.Windows.Forms.TextBox();
            this.labelRouteIp = new System.Windows.Forms.Label();
            this.labelTelnet = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.folderBrowserDialogModulePathSelector = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTipForm = new System.Windows.Forms.ToolTip(this.components);
            this.serviceIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.serviceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.startServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabSettings.SuspendLayout();
            this.tabMisc.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabDb.SuspendLayout();
            this.tabMaintanceDb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dayMaintenanceNumericUpDown)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).BeginInit();
            this.panel2.SuspendLayout();
            this.tpVPN.SuspendLayout();
            this.tabPageNet.SuspendLayout();
            this.panel1.SuspendLayout();
            this.serviceContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabMisc);
            this.tabSettings.Controls.Add(this.tabDb);
            this.tabSettings.Controls.Add(this.tabMaintanceDb);
            this.tabSettings.Controls.Add(this.tabPage1);
            this.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSettings.Location = new System.Drawing.Point(0, 0);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(355, 304);
            this.tabSettings.TabIndex = 0;
            // 
            // tabMisc
            // 
            this.tabMisc.BackColor = System.Drawing.Color.Transparent;
            this.tabMisc.Controls.Add(this.groupBox3);
            this.tabMisc.Controls.Add(this.groupBox1);
            this.tabMisc.Location = new System.Drawing.Point(4, 22);
            this.tabMisc.Name = "tabMisc";
            this.tabMisc.Padding = new System.Windows.Forms.Padding(3);
            this.tabMisc.Size = new System.Drawing.Size(347, 278);
            this.tabMisc.TabIndex = 1;
            this.tabMisc.Text = "Основные";
            this.tabMisc.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.txtServerPriority);
            this.groupBox3.Controls.Add(this.labelServerPriority);
            this.groupBox3.Controls.Add(this.txtServerName);
            this.groupBox3.Controls.Add(this.btnOpenPathToModuls);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Controls.Add(this.txtModulePath);
            this.groupBox3.Controls.Add(this.txtMaxValueCount);
            this.groupBox3.Controls.Add(this.cbxInterface);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.txtPort);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(8, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(312, 179);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Сервер сбора данных";
            // 
            // txtServerPriority
            // 
            this.txtServerPriority.Location = new System.Drawing.Point(116, 150);
            this.txtServerPriority.Name = "txtServerPriority";
            this.txtServerPriority.Size = new System.Drawing.Size(190, 20);
            this.txtServerPriority.TabIndex = 11;
            this.toolTipForm.SetToolTip(this.txtServerPriority, "Максимальное количество значений одного параметра,\r\nкоторое может вернуть сервер");
            this.txtServerPriority.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // labelServerPriority
            // 
            this.labelServerPriority.AutoSize = true;
            this.labelServerPriority.Location = new System.Drawing.Point(4, 153);
            this.labelServerPriority.Name = "labelServerPriority";
            this.labelServerPriority.Size = new System.Drawing.Size(106, 13);
            this.labelServerPriority.TabIndex = 10;
            this.labelServerPriority.Text = "Приоритет сервера";
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.Location = new System.Drawing.Point(116, 19);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(190, 20);
            this.txtServerName.TabIndex = 9;
            // 
            // btnOpenPathToModuls
            // 
            this.btnOpenPathToModuls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenPathToModuls.Location = new System.Drawing.Point(282, 70);
            this.btnOpenPathToModuls.Name = "btnOpenPathToModuls";
            this.btnOpenPathToModuls.Size = new System.Drawing.Size(24, 23);
            this.btnOpenPathToModuls.TabIndex = 8;
            this.btnOpenPathToModuls.Text = "...";
            this.btnOpenPathToModuls.UseVisualStyleBackColor = true;
            this.btnOpenPathToModuls.Click += new System.EventHandler(this.button4_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label24.Location = new System.Drawing.Point(4, 48);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(64, 13);
            this.label24.TabIndex = 5;
            this.label24.Text = "Интерфейс";
            // 
            // txtModulePath
            // 
            this.txtModulePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModulePath.Location = new System.Drawing.Point(116, 72);
            this.txtModulePath.Name = "txtModulePath";
            this.txtModulePath.ReadOnly = true;
            this.txtModulePath.Size = new System.Drawing.Size(160, 20);
            this.txtModulePath.TabIndex = 7;
            // 
            // txtMaxValueCount
            // 
            this.txtMaxValueCount.Location = new System.Drawing.Point(116, 124);
            this.txtMaxValueCount.Name = "txtMaxValueCount";
            this.txtMaxValueCount.Size = new System.Drawing.Size(190, 20);
            this.txtMaxValueCount.TabIndex = 3;
            this.toolTipForm.SetToolTip(this.txtMaxValueCount, "Максимальное количество значений одного параметра,\r\nкоторое может вернуть сервер");
            this.txtMaxValueCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // cbxInterface
            // 
            this.cbxInterface.FormattingEnabled = true;
            this.cbxInterface.Location = new System.Drawing.Point(116, 45);
            this.cbxInterface.Name = "cbxInterface";
            this.cbxInterface.Size = new System.Drawing.Size(190, 21);
            this.cbxInterface.TabIndex = 4;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label21.Location = new System.Drawing.Point(4, 75);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 13);
            this.label21.TabIndex = 6;
            this.label21.Text = "Путь к модулям";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new System.Drawing.Point(4, 127);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "Кол. значений";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(116, 98);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(190, 20);
            this.txtPort.TabIndex = 1;
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(4, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Порт";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(4, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя сервера";
            this.toolTipForm.SetToolTip(this.label1, "Совпадает с именем блока в структуре оборудования");
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtGlobalPort);
            this.groupBox1.Controls.Add(this.txtGlobalHost);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Location = new System.Drawing.Point(6, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 81);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Станционный сервер";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 54);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "Порт";
            // 
            // txtGlobalPort
            // 
            this.txtGlobalPort.Location = new System.Drawing.Point(116, 51);
            this.txtGlobalPort.Name = "txtGlobalPort";
            this.txtGlobalPort.Size = new System.Drawing.Size(92, 20);
            this.txtGlobalPort.TabIndex = 16;
            this.txtGlobalPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // txtGlobalHost
            // 
            this.txtGlobalHost.Location = new System.Drawing.Point(116, 25);
            this.txtGlobalHost.Name = "txtGlobalHost";
            this.txtGlobalHost.Size = new System.Drawing.Size(92, 20);
            this.txtGlobalHost.TabIndex = 15;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 13);
            this.label16.TabIndex = 14;
            this.label16.Text = "Сервер";
            // 
            // tabDb
            // 
            this.tabDb.BackColor = System.Drawing.Color.Transparent;
            this.tabDb.Controls.Add(this.chkWA);
            this.tabDb.Controls.Add(this.btnRefreshBase);
            this.tabDb.Controls.Add(this.cmbDbType);
            this.tabDb.Controls.Add(this.cbxBases);
            this.tabDb.Controls.Add(this.label3);
            this.tabDb.Controls.Add(this.btnRefresh);
            this.tabDb.Controls.Add(this.label5);
            this.tabDb.Controls.Add(this.cbxHosts);
            this.tabDb.Controls.Add(this.label6);
            this.tabDb.Controls.Add(this.txtDbLoggin);
            this.tabDb.Controls.Add(this.label7);
            this.tabDb.Controls.Add(this.label8);
            this.tabDb.Controls.Add(this.button1);
            this.tabDb.Controls.Add(this.txtDbPassword);
            this.tabDb.Location = new System.Drawing.Point(4, 22);
            this.tabDb.Name = "tabDb";
            this.tabDb.Padding = new System.Windows.Forms.Padding(3);
            this.tabDb.Size = new System.Drawing.Size(347, 278);
            this.tabDb.TabIndex = 2;
            this.tabDb.Text = "СУБД";
            this.tabDb.UseVisualStyleBackColor = true;
            // 
            // chkWA
            // 
            this.chkWA.AutoSize = true;
            this.chkWA.Enabled = false;
            this.chkWA.Location = new System.Drawing.Point(94, 89);
            this.chkWA.Name = "chkWA";
            this.chkWA.Size = new System.Drawing.Size(139, 17);
            this.chkWA.TabIndex = 29;
            this.chkWA.Text = "Авторизация Windows";
            this.chkWA.UseVisualStyleBackColor = true;
            // 
            // btnRefreshBase
            // 
            this.btnRefreshBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBase.Location = new System.Drawing.Point(264, 62);
            this.btnRefreshBase.Name = "btnRefreshBase";
            this.btnRefreshBase.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshBase.TabIndex = 27;
            this.btnRefreshBase.Text = "Обновить";
            this.btnRefreshBase.UseVisualStyleBackColor = true;
            this.btnRefreshBase.Click += new System.EventHandler(this.btnRefreshBase_Click);
            // 
            // cmbDbType
            // 
            this.cmbDbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDbType.FormattingEnabled = true;
            this.cmbDbType.Items.AddRange(new object[] {
            "MSSQL"});
            this.cmbDbType.Location = new System.Drawing.Point(94, 10);
            this.cmbDbType.Name = "cmbDbType";
            this.cmbDbType.Size = new System.Drawing.Size(245, 21);
            this.cmbDbType.TabIndex = 15;
            // 
            // cbxBases
            // 
            this.cbxBases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBases.FormattingEnabled = true;
            this.cbxBases.Location = new System.Drawing.Point(94, 64);
            this.cbxBases.Name = "cbxBases";
            this.cbxBases.Size = new System.Drawing.Size(160, 21);
            this.cbxBases.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(8, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Тип сервера";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(264, 35);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 25;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(8, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "База данных";
            // 
            // cbxHosts
            // 
            this.cbxHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxHosts.FormattingEnabled = true;
            this.cbxHosts.Location = new System.Drawing.Point(94, 37);
            this.cbxHosts.Name = "cbxHosts";
            this.cbxHosts.Size = new System.Drawing.Size(160, 21);
            this.cbxHosts.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(8, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Пользователь";
            // 
            // txtDbLoggin
            // 
            this.txtDbLoggin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDbLoggin.Location = new System.Drawing.Point(94, 112);
            this.txtDbLoggin.Name = "txtDbLoggin";
            this.txtDbLoggin.Size = new System.Drawing.Size(245, 20);
            this.txtDbLoggin.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(8, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Сервер";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(8, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Пароль";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(264, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Проверить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtDbPassword
            // 
            this.txtDbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDbPassword.Location = new System.Drawing.Point(94, 138);
            this.txtDbPassword.Name = "txtDbPassword";
            this.txtDbPassword.PasswordChar = '*';
            this.txtDbPassword.Size = new System.Drawing.Size(245, 20);
            this.txtDbPassword.TabIndex = 11;
            // 
            // tabMaintanceDb
            // 
            this.tabMaintanceDb.BackColor = System.Drawing.Color.Transparent;
            this.tabMaintanceDb.Controls.Add(this.label23);
            this.tabMaintanceDb.Controls.Add(this.label22);
            this.tabMaintanceDb.Controls.Add(this.maintenanceCheckBox);
            this.tabMaintanceDb.Controls.Add(this.label18);
            this.tabMaintanceDb.Controls.Add(this.label12);
            this.tabMaintanceDb.Controls.Add(this.label11);
            this.tabMaintanceDb.Controls.Add(this.txtDeleteCountValues);
            this.tabMaintanceDb.Controls.Add(this.maintenanceTimePicker);
            this.tabMaintanceDb.Controls.Add(this.label13);
            this.tabMaintanceDb.Controls.Add(this.dayMaintenanceNumericUpDown);
            this.tabMaintanceDb.Controls.Add(this.txtKeepValuesDays);
            this.tabMaintanceDb.Controls.Add(this.dayOfWeekMaintenanceComboBox);
            this.tabMaintanceDb.Controls.Add(this.periodMaintenanceComboBox);
            this.tabMaintanceDb.Location = new System.Drawing.Point(4, 22);
            this.tabMaintanceDb.Name = "tabMaintanceDb";
            this.tabMaintanceDb.Size = new System.Drawing.Size(347, 278);
            this.tabMaintanceDb.TabIndex = 4;
            this.tabMaintanceDb.Text = "Обслуживание БД";
            this.tabMaintanceDb.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(220, 65);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(39, 13);
            this.label23.TabIndex = 24;
            this.label23.Text = "Число";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(220, 38);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(40, 13);
            this.label22.TabIndex = 23;
            this.label22.Text = "Время";
            // 
            // maintenanceCheckBox
            // 
            this.maintenanceCheckBox.AutoSize = true;
            this.maintenanceCheckBox.Location = new System.Drawing.Point(8, 12);
            this.maintenanceCheckBox.Name = "maintenanceCheckBox";
            this.maintenanceCheckBox.Size = new System.Drawing.Size(169, 17);
            this.maintenanceCheckBox.TabIndex = 16;
            this.maintenanceCheckBox.Text = "Производить обслуживание";
            this.maintenanceCheckBox.UseVisualStyleBackColor = true;
            this.maintenanceCheckBox.CheckedChanged += new System.EventHandler(this.maintenanceCheckBox_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(8, 65);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(34, 13);
            this.label18.TabIndex = 22;
            this.label18.Text = "День";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(8, 92);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Удалять по (записей)";
            this.toolTipForm.SetToolTip(this.label12, "По сколько записей удалять за одну транзакцию");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 38);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Период";
            // 
            // txtDeleteCountValues
            // 
            this.txtDeleteCountValues.Location = new System.Drawing.Point(131, 89);
            this.txtDeleteCountValues.Name = "txtDeleteCountValues";
            this.txtDeleteCountValues.Size = new System.Drawing.Size(83, 20);
            this.txtDeleteCountValues.TabIndex = 10;
            this.toolTipForm.SetToolTip(this.txtDeleteCountValues, "По сколько записей удалять за одну транзакцию");
            this.txtDeleteCountValues.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // maintenanceTimePicker
            // 
            this.maintenanceTimePicker.CustomFormat = "HH:mm";
            this.maintenanceTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.maintenanceTimePicker.Location = new System.Drawing.Point(266, 36);
            this.maintenanceTimePicker.Name = "maintenanceTimePicker";
            this.maintenanceTimePicker.ShowUpDown = true;
            this.maintenanceTimePicker.Size = new System.Drawing.Size(51, 20);
            this.maintenanceTimePicker.TabIndex = 20;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(8, 118);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Хранить (дней)";
            this.toolTipForm.SetToolTip(this.label13, "Сколько дней хранить данные");
            // 
            // dayMaintenanceNumericUpDown
            // 
            this.dayMaintenanceNumericUpDown.Location = new System.Drawing.Point(266, 63);
            this.dayMaintenanceNumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.dayMaintenanceNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.dayMaintenanceNumericUpDown.Name = "dayMaintenanceNumericUpDown";
            this.dayMaintenanceNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this.dayMaintenanceNumericUpDown.TabIndex = 19;
            this.dayMaintenanceNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtKeepValuesDays
            // 
            this.txtKeepValuesDays.Location = new System.Drawing.Point(131, 115);
            this.txtKeepValuesDays.Name = "txtKeepValuesDays";
            this.txtKeepValuesDays.Size = new System.Drawing.Size(83, 20);
            this.txtKeepValuesDays.TabIndex = 12;
            this.toolTipForm.SetToolTip(this.txtKeepValuesDays, "Сколько дней хранить данные");
            this.txtKeepValuesDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filltrateOnlyDigit_KeyPress);
            // 
            // dayOfWeekMaintenanceComboBox
            // 
            this.dayOfWeekMaintenanceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dayOfWeekMaintenanceComboBox.FormattingEnabled = true;
            this.dayOfWeekMaintenanceComboBox.Location = new System.Drawing.Point(131, 62);
            this.dayOfWeekMaintenanceComboBox.Name = "dayOfWeekMaintenanceComboBox";
            this.dayOfWeekMaintenanceComboBox.Size = new System.Drawing.Size(83, 21);
            this.dayOfWeekMaintenanceComboBox.TabIndex = 18;
            // 
            // periodMaintenanceComboBox
            // 
            this.periodMaintenanceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.periodMaintenanceComboBox.FormattingEnabled = true;
            this.periodMaintenanceComboBox.Location = new System.Drawing.Point(131, 35);
            this.periodMaintenanceComboBox.Name = "periodMaintenanceComboBox";
            this.periodMaintenanceComboBox.Size = new System.Drawing.Size(83, 21);
            this.periodMaintenanceComboBox.TabIndex = 17;
            this.periodMaintenanceComboBox.SelectedIndexChanged += new System.EventHandler(this.periodMaintenanceComboBox_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPage1.Controls.Add(this.dgvServers);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(347, 278);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "Дублирование";
            // 
            // dgvServers
            // 
            this.dgvServers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmHost,
            this.clmPort,
            this.clmPriority});
            this.dgvServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvServers.Location = new System.Drawing.Point(3, 113);
            this.dgvServers.Name = "dgvServers";
            this.dgvServers.Size = new System.Drawing.Size(341, 162);
            this.dgvServers.TabIndex = 18;
            this.dgvServers.Visible = false;
            // 
            // clmHost
            // 
            this.clmHost.HeaderText = "Сервер";
            this.clmHost.Name = "clmHost";
            // 
            // clmPort
            // 
            this.clmPort.HeaderText = "Порт";
            this.clmPort.Name = "clmPort";
            // 
            // clmPriority
            // 
            this.clmPriority.HeaderText = "Приоритет";
            this.clmPriority.Name = "clmPriority";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.txtMirroringPort);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.txtReplicationPassword);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.txtMirroringBackupPath);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtReplicationUser);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(341, 110);
            this.panel2.TabIndex = 19;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(5, 6);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(122, 13);
            this.label25.TabIndex = 15;
            this.label25.Text = "Порт зеркалирования:";
            // 
            // txtMirroringPort
            // 
            this.txtMirroringPort.Location = new System.Drawing.Point(161, 3);
            this.txtMirroringPort.Name = "txtMirroringPort";
            this.txtMirroringPort.Size = new System.Drawing.Size(175, 20);
            this.txtMirroringPort.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Пароль:";
            // 
            // txtReplicationPassword
            // 
            this.txtReplicationPassword.Location = new System.Drawing.Point(161, 81);
            this.txtReplicationPassword.Name = "txtReplicationPassword";
            this.txtReplicationPassword.PasswordChar = '*';
            this.txtReplicationPassword.Size = new System.Drawing.Size(175, 20);
            this.txtReplicationPassword.TabIndex = 7;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(5, 32);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(133, 13);
            this.label26.TabIndex = 17;
            this.label26.Text = "Путь к резервной копии:";
            // 
            // txtMirroringBackupPath
            // 
            this.txtMirroringBackupPath.Location = new System.Drawing.Point(161, 29);
            this.txtMirroringBackupPath.Name = "txtMirroringBackupPath";
            this.txtMirroringBackupPath.Size = new System.Drawing.Size(175, 20);
            this.txtMirroringBackupPath.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Пользователь:";
            // 
            // txtReplicationUser
            // 
            this.txtReplicationUser.Location = new System.Drawing.Point(161, 55);
            this.txtReplicationUser.Name = "txtReplicationUser";
            this.txtReplicationUser.Size = new System.Drawing.Size(175, 20);
            this.txtReplicationUser.TabIndex = 5;
            // 
            // tpVPN
            // 
            this.tpVPN.BackColor = System.Drawing.Color.Transparent;
            this.tpVPN.Controls.Add(this.label27);
            this.tpVPN.Controls.Add(this.txtVPNName);
            this.tpVPN.Controls.Add(this.label20);
            this.tpVPN.Controls.Add(this.label19);
            this.tpVPN.Controls.Add(this.label14);
            this.tpVPN.Controls.Add(this.txtVPNPassword);
            this.tpVPN.Controls.Add(this.txtVPNUser);
            this.tpVPN.Controls.Add(this.txtVPNServer);
            this.tpVPN.Controls.Add(this.chbxVPNUse);
            this.tpVPN.Location = new System.Drawing.Point(4, 22);
            this.tpVPN.Name = "tpVPN";
            this.tpVPN.Size = new System.Drawing.Size(347, 278);
            this.tpVPN.TabIndex = 5;
            this.tpVPN.Text = "VPN";
            this.tpVPN.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(8, 41);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(130, 13);
            this.label27.TabIndex = 8;
            this.label27.Text = "Название подключения:";
            // 
            // txtVPNName
            // 
            this.txtVPNName.Enabled = false;
            this.txtVPNName.Location = new System.Drawing.Point(144, 38);
            this.txtVPNName.Name = "txtVPNName";
            this.txtVPNName.Size = new System.Drawing.Size(100, 20);
            this.txtVPNName.TabIndex = 7;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 119);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(48, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Пароль:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(8, 93);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(106, 13);
            this.label19.TabIndex = 5;
            this.label19.Text = "Имя пользователя:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(86, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Адрес сервера:";
            // 
            // txtVPNPassword
            // 
            this.txtVPNPassword.Enabled = false;
            this.txtVPNPassword.Location = new System.Drawing.Point(144, 116);
            this.txtVPNPassword.Name = "txtVPNPassword";
            this.txtVPNPassword.Size = new System.Drawing.Size(100, 20);
            this.txtVPNPassword.TabIndex = 2;
            this.txtVPNPassword.UseSystemPasswordChar = true;
            // 
            // txtVPNUser
            // 
            this.txtVPNUser.Enabled = false;
            this.txtVPNUser.Location = new System.Drawing.Point(144, 90);
            this.txtVPNUser.Name = "txtVPNUser";
            this.txtVPNUser.Size = new System.Drawing.Size(100, 20);
            this.txtVPNUser.TabIndex = 3;
            // 
            // txtVPNServer
            // 
            this.txtVPNServer.Enabled = false;
            this.txtVPNServer.Location = new System.Drawing.Point(144, 64);
            this.txtVPNServer.Name = "txtVPNServer";
            this.txtVPNServer.Size = new System.Drawing.Size(100, 20);
            this.txtVPNServer.TabIndex = 2;
            // 
            // chbxVPNUse
            // 
            this.chbxVPNUse.AutoSize = true;
            this.chbxVPNUse.Location = new System.Drawing.Point(8, 12);
            this.chbxVPNUse.Name = "chbxVPNUse";
            this.chbxVPNUse.Size = new System.Drawing.Size(194, 17);
            this.chbxVPNUse.TabIndex = 1;
            this.chbxVPNUse.Text = "Использовать VPN подключение";
            this.chbxVPNUse.UseVisualStyleBackColor = true;
            this.chbxVPNUse.CheckedChanged += new System.EventHandler(this.chbxVPNUse_CheckedChanged);
            // 
            // tabPageNet
            // 
            this.tabPageNet.Controls.Add(this.txtTelnetHost);
            this.tabPageNet.Controls.Add(this.txtRouterIp);
            this.tabPageNet.Controls.Add(this.labelRouteIp);
            this.tabPageNet.Controls.Add(this.labelTelnet);
            this.tabPageNet.Location = new System.Drawing.Point(4, 22);
            this.tabPageNet.Name = "tabPageNet";
            this.tabPageNet.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNet.Size = new System.Drawing.Size(347, 278);
            this.tabPageNet.TabIndex = 7;
            this.tabPageNet.Text = "Сетевая инфраструктура";
            this.tabPageNet.UseVisualStyleBackColor = true;
            // 
            // txtTelnetHost
            // 
            this.txtTelnetHost.Location = new System.Drawing.Point(143, 6);
            this.txtTelnetHost.Name = "txtTelnetHost";
            this.txtTelnetHost.Size = new System.Drawing.Size(196, 20);
            this.txtTelnetHost.TabIndex = 3;
            // 
            // txtRouterIp
            // 
            this.txtRouterIp.Location = new System.Drawing.Point(143, 32);
            this.txtRouterIp.Name = "txtRouterIp";
            this.txtRouterIp.Size = new System.Drawing.Size(196, 20);
            this.txtRouterIp.TabIndex = 2;
            // 
            // labelRouteIp
            // 
            this.labelRouteIp.AutoSize = true;
            this.labelRouteIp.Location = new System.Drawing.Point(10, 35);
            this.labelRouteIp.Name = "labelRouteIp";
            this.labelRouteIp.Size = new System.Drawing.Size(126, 13);
            this.labelRouteIp.TabIndex = 1;
            this.labelRouteIp.Text = "Адрес маршрутизатора";
            // 
            // labelTelnet
            // 
            this.labelTelnet.AutoSize = true;
            this.labelTelnet.Location = new System.Drawing.Point(10, 9);
            this.labelTelnet.Name = "labelTelnet";
            this.labelTelnet.Size = new System.Drawing.Size(74, 13);
            this.labelTelnet.TabIndex = 0;
            this.labelTelnet.Text = "Хост телнета";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.versionLabel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 304);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(355, 39);
            this.panel1.TabIndex = 1;
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(295, 11);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(57, 13);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "ver. x.x.x.x";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(12, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // folderBrowserDialogModulePathSelector
            // 
            this.folderBrowserDialogModulePathSelector.Description = "Выбор директории с модулями загрузки";
            this.folderBrowserDialogModulePathSelector.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // serviceIcon
            // 
            this.serviceIcon.ContextMenuStrip = this.serviceContextMenuStrip;
            this.serviceIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("serviceIcon.Icon")));
            this.serviceIcon.Text = "Сервер сбора данных";
            this.serviceIcon.Visible = true;
            this.serviceIcon.DoubleClick += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // serviceContextMenuStrip
            // 
            this.serviceContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.monitoringToolStripMenuItem,
            this.toolStripMenuItem1,
            this.startServiceToolStripMenuItem,
            this.stopServiceToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.serviceContextMenuStrip.Name = "serviceContextMenuStrip";
            this.serviceContextMenuStrip.Size = new System.Drawing.Size(193, 148);
            this.serviceContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.serviceContextMenuStrip_Opening);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.settingsToolStripMenuItem.Text = "Настройки";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.aboutToolStripMenuItem.Text = "О программе";
            this.aboutToolStripMenuItem.Visible = false;
            // 
            // monitoringToolStripMenuItem
            // 
            this.monitoringToolStripMenuItem.Name = "monitoringToolStripMenuItem";
            this.monitoringToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.monitoringToolStripMenuItem.Text = "Мониторинг каналов";
            this.monitoringToolStripMenuItem.Click += new System.EventHandler(this.monitoringToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(189, 6);
            // 
            // startServiceToolStripMenuItem
            // 
            this.startServiceToolStripMenuItem.Name = "startServiceToolStripMenuItem";
            this.startServiceToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.startServiceToolStripMenuItem.Text = "Запустить сервер";
            this.startServiceToolStripMenuItem.Click += new System.EventHandler(this.startServiceToolStripMenuItem_Click);
            // 
            // stopServiceToolStripMenuItem
            // 
            this.stopServiceToolStripMenuItem.Name = "stopServiceToolStripMenuItem";
            this.stopServiceToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.stopServiceToolStripMenuItem.Text = "Остановить сервер";
            this.stopServiceToolStripMenuItem.Click += new System.EventHandler(this.stopServiceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(189, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 343);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка сервера сбора данных";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.Shown += new System.EventHandler(this.SettingForm_Shown);
            this.tabSettings.ResumeLayout(false);
            this.tabMisc.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabDb.ResumeLayout(false);
            this.tabDb.PerformLayout();
            this.tabMaintanceDb.ResumeLayout(false);
            this.tabMaintanceDb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dayMaintenanceNumericUpDown)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tpVPN.ResumeLayout(false);
            this.tpVPN.PerformLayout();
            this.tabPageNet.ResumeLayout(false);
            this.tabPageNet.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.serviceContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabMisc;
        private System.Windows.Forms.TabPage tabDb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox txtDbPassword;
        public System.Windows.Forms.TextBox txtDbLoggin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbDbType;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogModulePathSelector;
        private System.Windows.Forms.ToolTip toolTipForm;
        private System.Windows.Forms.TabPage tabMaintanceDb;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMaxValueCount;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NotifyIcon serviceIcon;
        private System.Windows.Forms.ContextMenuStrip serviceContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitoringToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox maintenanceCheckBox;
        public System.Windows.Forms.TextBox txtKeepValuesDays;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox txtDeleteCountValues;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox dayOfWeekMaintenanceComboBox;
        private System.Windows.Forms.ComboBox periodMaintenanceComboBox;
        private System.Windows.Forms.NumericUpDown dayMaintenanceNumericUpDown;
        private System.Windows.Forms.DateTimePicker maintenanceTimePicker;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnRefreshBase;
        private System.Windows.Forms.ComboBox cbxBases;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbxHosts;
        private System.Windows.Forms.CheckBox chkWA;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox cbxInterface;
        private System.Windows.Forms.Button btnOpenPathToModuls;
        private System.Windows.Forms.TextBox txtModulePath;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtReplicationPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtReplicationUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtMirroringBackupPath;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtMirroringPort;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtGlobalPort;
        private System.Windows.Forms.TextBox txtGlobalHost;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.DataGridView dgvServers;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmHost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPriority;
        private System.Windows.Forms.ToolStripMenuItem startServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TabPage tpVPN;
        private System.Windows.Forms.TextBox txtVPNUser;
        private System.Windows.Forms.TextBox txtVPNServer;
        private System.Windows.Forms.CheckBox chbxVPNUse;
        private System.Windows.Forms.TextBox txtVPNPassword;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox txtVPNName;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label14;
        //private LogsConfigurationsUI.LogSettings logSettings;
        private System.Windows.Forms.TabPage tabPageNet;
        private System.Windows.Forms.TextBox txtTelnetHost;
        private System.Windows.Forms.TextBox txtRouterIp;
        private System.Windows.Forms.Label labelRouteIp;
        private System.Windows.Forms.Label labelTelnet;
        private System.Windows.Forms.TextBox txtServerPriority;
        private System.Windows.Forms.Label labelServerPriority;
        private System.Windows.Forms.Panel panel2;
    }
}

