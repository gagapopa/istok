namespace COTES.ISTOK.Assignment.UI
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.label16 = new System.Windows.Forms.Label();
            this.txtCalculationPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtParameterConstraction = new System.Windows.Forms.TextBox();
            this.txtMaxLoopCount = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtMaxValueCountPerQuery = new System.Windows.Forms.TextBox();
            this.txtInterface = new System.Windows.Forms.TextBox();
            this.serviceIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageReservation = new System.Windows.Forms.TabPage();
            this.groupBoxReservation = new System.Windows.Forms.GroupBox();
            this.txtBackupFileName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cbxBackupType = new System.Windows.Forms.ComboBox();
            this.udDiffTTL = new System.Windows.Forms.NumericUpDown();
            this.udBackupDay = new System.Windows.Forms.NumericUpDown();
            this.cbxBackupDayOfWeek = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.mtbBackupTime = new System.Windows.Forms.MaskedTextBox();
            this.cbxBackupPeriod = new System.Windows.Forms.ComboBox();
            this.txtBackupDescription = new System.Windows.Forms.TextBox();
            this.txtBackupName = new System.Windows.Forms.TextBox();
            this.chkDiff = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPageDb = new System.Windows.Forms.TabPage();
            this.groupBoxBd = new System.Windows.Forms.GroupBox();
            this.btnCreateDateBase = new System.Windows.Forms.Button();
            this.btnRefreshBase = new System.Windows.Forms.Button();
            this.cbxBases = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbxHosts = new System.Windows.Forms.ComboBox();
            this.cmbDbType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageCommon = new System.Windows.Forms.TabPage();
            this.groupBoxCommon = new System.Windows.Forms.GroupBox();
            this.lblInterface = new System.Windows.Forms.Label();
            this.registerDateLabel = new System.Windows.Forms.Label();
            this.registeredByLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtRemotingPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.panel1.SuspendLayout();
            this.contextMenuStripMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPageReservation.SuspendLayout();
            this.groupBoxReservation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDiffTTL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBackupDay)).BeginInit();
            this.tabPageDb.SuspendLayout();
            this.groupBoxBd.SuspendLayout();
            this.tabPageCommon.SuspendLayout();
            this.groupBoxCommon.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.versionLabel);
            this.panel1.Controls.Add(this.btnRegister);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 318);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(373, 39);
            this.panel1.TabIndex = 1;
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(312, 11);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(57, 13);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "ver. x.x.x.x";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(93, 6);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(108, 23);
            this.btnRegister.TabIndex = 1;
            this.btnRegister.Text = "Регистрация";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.registerButton_Click);
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
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Выбор директории с модулями загрузки";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(4, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Период расчета";
            this.toolTipMain.SetToolTip(this.label16, "период в миллисекундах повторения расчета ТЭП");
            // 
            // txtCalculationPeriod
            // 
            this.txtCalculationPeriod.Location = new System.Drawing.Point(162, 19);
            this.txtCalculationPeriod.Name = "txtCalculationPeriod";
            this.txtCalculationPeriod.Size = new System.Drawing.Size(104, 20);
            this.txtCalculationPeriod.TabIndex = 17;
            this.txtCalculationPeriod.Text = "3600";
            this.toolTipMain.SetToolTip(this.txtCalculationPeriod, "Периодичность запуска циклического расчета (с)");
            this.txtCalculationPeriod.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filtrateOnlyDigigit_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(4, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Ограничение параметра";
            this.toolTipMain.SetToolTip(this.label1, "Ограничение по количесту расчитанных значений одного параметра за цикл");
            // 
            // txtParameterConstraction
            // 
            this.txtParameterConstraction.Location = new System.Drawing.Point(162, 45);
            this.txtParameterConstraction.Name = "txtParameterConstraction";
            this.txtParameterConstraction.Size = new System.Drawing.Size(104, 20);
            this.txtParameterConstraction.TabIndex = 20;
            this.txtParameterConstraction.Text = "500";
            this.toolTipMain.SetToolTip(this.txtParameterConstraction, "Ограничение по количеству рассчитанных значений одного параметра за цикл");
            this.txtParameterConstraction.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filtrateOnlyDigigit_KeyPress);
            // 
            // txtMaxLoopCount
            // 
            this.txtMaxLoopCount.Location = new System.Drawing.Point(162, 71);
            this.txtMaxLoopCount.Name = "txtMaxLoopCount";
            this.txtMaxLoopCount.Size = new System.Drawing.Size(104, 20);
            this.txtMaxLoopCount.TabIndex = 26;
            this.txtMaxLoopCount.Text = "100";
            this.toolTipMain.SetToolTip(this.txtMaxLoopCount, "Максимальное количество итераций расчета циклов");
            this.txtMaxLoopCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filtrateOnlyDigigit_KeyPress);
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new System.Drawing.Point(261, 149);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(40, 13);
            this.label18.TabIndex = 34;
            this.label18.Text = "Время";
            this.toolTipMain.SetToolTip(this.label18, "Время запуска резервирования");
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label19.Location = new System.Drawing.Point(3, 176);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 13);
            this.label19.TabIndex = 35;
            this.label19.Text = "День";
            this.toolTipMain.SetToolTip(this.label19, "День недели");
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label20.Location = new System.Drawing.Point(262, 176);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(39, 13);
            this.label20.TabIndex = 37;
            this.label20.Text = "Число";
            this.toolTipMain.SetToolTip(this.label20, "Число месяца");
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label22.Location = new System.Drawing.Point(224, 202);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(77, 13);
            this.label22.TabIndex = 39;
            this.label22.Text = "Актуальность";
            this.toolTipMain.SetToolTip(this.label22, "Количество дней актуальности резерва");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(3, 123);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 13);
            this.label13.TabIndex = 45;
            this.label13.Text = "Файл";
            this.toolTipMain.SetToolTip(this.label13, "Путь к файлу резерва");
            // 
            // txtMaxValueCountPerQuery
            // 
            this.txtMaxValueCountPerQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxValueCountPerQuery.Location = new System.Drawing.Point(116, 42);
            this.txtMaxValueCountPerQuery.Name = "txtMaxValueCountPerQuery";
            this.txtMaxValueCountPerQuery.Size = new System.Drawing.Size(177, 20);
            this.txtMaxValueCountPerQuery.TabIndex = 6;
            this.toolTipMain.SetToolTip(this.txtMaxValueCountPerQuery, "Максимальное количество значений одного параметра,\r\nкоторое может вернуть сервер");
            this.txtMaxValueCountPerQuery.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filtrateOnlyDigigit_KeyPress);
            // 
            // txtInterface
            // 
            this.txtInterface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInterface.Location = new System.Drawing.Point(116, 68);
            this.txtInterface.Name = "txtInterface";
            this.txtInterface.Size = new System.Drawing.Size(177, 20);
            this.txtInterface.TabIndex = 16;
            this.toolTipMain.SetToolTip(this.txtInterface, "Максимальное количество значений одного параметра,\r\nкоторое может вернуть сервер");
            // 
            // serviceIcon
            // 
            this.serviceIcon.ContextMenuStrip = this.contextMenuStripMain;
            this.serviceIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("serviceIcon.Icon")));
            this.serviceIcon.Text = "Общестанционная служба";
            this.serviceIcon.Visible = true;
            this.serviceIcon.DoubleClick += new System.EventHandler(this.serviceIcon_DoubleClick);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripMenuItem1,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStrip1";
            this.contextMenuStripMain.Size = new System.Drawing.Size(173, 126);
            this.contextMenuStripMain.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMain_Opening);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showToolStripMenuItem.Text = "Настройки";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(172, 22);
            this.toolStripMenuItem2.Text = "О программе";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.startToolStripMenuItem.Text = "Запустить сервер";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.stopToolStripMenuItem.Text = "Остановить север";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(169, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(365, 292);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Расчет";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.txtMaxLoopCount);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtParameterConstraction);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.txtCalculationPeriod);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(365, 292);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Настройки расчетов";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(3, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Ограничение циклов";
            // 
            // tabPageReservation
            // 
            this.tabPageReservation.Controls.Add(this.groupBoxReservation);
            this.tabPageReservation.Location = new System.Drawing.Point(4, 22);
            this.tabPageReservation.Name = "tabPageReservation";
            this.tabPageReservation.Size = new System.Drawing.Size(365, 292);
            this.tabPageReservation.TabIndex = 4;
            this.tabPageReservation.Text = "Резервирование";
            this.tabPageReservation.UseVisualStyleBackColor = true;
            // 
            // groupBoxReservation
            // 
            this.groupBoxReservation.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxReservation.Controls.Add(this.label13);
            this.groupBoxReservation.Controls.Add(this.txtBackupFileName);
            this.groupBoxReservation.Controls.Add(this.label12);
            this.groupBoxReservation.Controls.Add(this.cbxBackupType);
            this.groupBoxReservation.Controls.Add(this.udDiffTTL);
            this.groupBoxReservation.Controls.Add(this.udBackupDay);
            this.groupBoxReservation.Controls.Add(this.label22);
            this.groupBoxReservation.Controls.Add(this.label20);
            this.groupBoxReservation.Controls.Add(this.label19);
            this.groupBoxReservation.Controls.Add(this.label18);
            this.groupBoxReservation.Controls.Add(this.cbxBackupDayOfWeek);
            this.groupBoxReservation.Controls.Add(this.label14);
            this.groupBoxReservation.Controls.Add(this.mtbBackupTime);
            this.groupBoxReservation.Controls.Add(this.cbxBackupPeriod);
            this.groupBoxReservation.Controls.Add(this.txtBackupDescription);
            this.groupBoxReservation.Controls.Add(this.txtBackupName);
            this.groupBoxReservation.Controls.Add(this.chkDiff);
            this.groupBoxReservation.Controls.Add(this.label11);
            this.groupBoxReservation.Controls.Add(this.label15);
            this.groupBoxReservation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxReservation.Location = new System.Drawing.Point(0, 0);
            this.groupBoxReservation.Name = "groupBoxReservation";
            this.groupBoxReservation.Size = new System.Drawing.Size(365, 292);
            this.groupBoxReservation.TabIndex = 3;
            this.groupBoxReservation.TabStop = false;
            this.groupBoxReservation.Text = "Параметры резервирования";
            // 
            // txtBackupFileName
            // 
            this.txtBackupFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupFileName.Location = new System.Drawing.Point(74, 120);
            this.txtBackupFileName.Name = "txtBackupFileName";
            this.txtBackupFileName.Size = new System.Drawing.Size(285, 20);
            this.txtBackupFileName.TabIndex = 44;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(5, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 13);
            this.label12.TabIndex = 43;
            this.label12.Text = "Тип";
            // 
            // cbxBackupType
            // 
            this.cbxBackupType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBackupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBackupType.FormattingEnabled = true;
            this.cbxBackupType.Location = new System.Drawing.Point(74, 19);
            this.cbxBackupType.Name = "cbxBackupType";
            this.cbxBackupType.Size = new System.Drawing.Size(285, 21);
            this.cbxBackupType.TabIndex = 42;
            this.cbxBackupType.SelectedIndexChanged += new System.EventHandler(this.cbxBackup_SelectedIndexChanged);
            // 
            // udDiffTTL
            // 
            this.udDiffTTL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udDiffTTL.Location = new System.Drawing.Point(316, 200);
            this.udDiffTTL.Name = "udDiffTTL";
            this.udDiffTTL.Size = new System.Drawing.Size(43, 20);
            this.udDiffTTL.TabIndex = 41;
            // 
            // udBackupDay
            // 
            this.udBackupDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udBackupDay.Location = new System.Drawing.Point(316, 172);
            this.udBackupDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.udBackupDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBackupDay.Name = "udBackupDay";
            this.udBackupDay.Size = new System.Drawing.Size(43, 20);
            this.udBackupDay.TabIndex = 40;
            this.udBackupDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbxBackupDayOfWeek
            // 
            this.cbxBackupDayOfWeek.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBackupDayOfWeek.FormattingEnabled = true;
            this.cbxBackupDayOfWeek.Location = new System.Drawing.Point(74, 173);
            this.cbxBackupDayOfWeek.Name = "cbxBackupDayOfWeek";
            this.cbxBackupDayOfWeek.Size = new System.Drawing.Size(181, 21);
            this.cbxBackupDayOfWeek.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(3, 149);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 32;
            this.label14.Text = "Период";
            // 
            // mtbBackupTime
            // 
            this.mtbBackupTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mtbBackupTime.Location = new System.Drawing.Point(316, 146);
            this.mtbBackupTime.Mask = "00:00";
            this.mtbBackupTime.Name = "mtbBackupTime";
            this.mtbBackupTime.Size = new System.Drawing.Size(43, 20);
            this.mtbBackupTime.TabIndex = 31;
            this.mtbBackupTime.Text = "0000";
            this.mtbBackupTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mtbBackupTime.ValidatingType = typeof(System.DateTime);
            // 
            // cbxBackupPeriod
            // 
            this.cbxBackupPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBackupPeriod.FormattingEnabled = true;
            this.cbxBackupPeriod.Location = new System.Drawing.Point(74, 146);
            this.cbxBackupPeriod.Name = "cbxBackupPeriod";
            this.cbxBackupPeriod.Size = new System.Drawing.Size(181, 21);
            this.cbxBackupPeriod.TabIndex = 29;
            this.cbxBackupPeriod.SelectedIndexChanged += new System.EventHandler(this.cbxDiffPeriod_SelectedIndexChanged);
            // 
            // txtBackupDescription
            // 
            this.txtBackupDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupDescription.Location = new System.Drawing.Point(74, 94);
            this.txtBackupDescription.Name = "txtBackupDescription";
            this.txtBackupDescription.Size = new System.Drawing.Size(285, 20);
            this.txtBackupDescription.TabIndex = 24;
            // 
            // txtBackupName
            // 
            this.txtBackupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupName.Location = new System.Drawing.Point(74, 68);
            this.txtBackupName.Name = "txtBackupName";
            this.txtBackupName.Size = new System.Drawing.Size(285, 20);
            this.txtBackupName.TabIndex = 23;
            // 
            // chkDiff
            // 
            this.chkDiff.AutoSize = true;
            this.chkDiff.Location = new System.Drawing.Point(6, 46);
            this.chkDiff.Name = "chkDiff";
            this.chkDiff.Size = new System.Drawing.Size(99, 17);
            this.chkDiff.TabIndex = 21;
            this.chkDiff.Text = "Использовать";
            this.chkDiff.UseVisualStyleBackColor = true;
            this.chkDiff.CheckedChanged += new System.EventHandler(this.chkDiff_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(3, 97);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Описание";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new System.Drawing.Point(3, 71);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Название";
            // 
            // tabPageDb
            // 
            this.tabPageDb.Controls.Add(this.groupBoxBd);
            this.tabPageDb.Location = new System.Drawing.Point(4, 22);
            this.tabPageDb.Name = "tabPageDb";
            this.tabPageDb.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDb.Size = new System.Drawing.Size(365, 292);
            this.tabPageDb.TabIndex = 2;
            this.tabPageDb.Text = "СУБД";
            this.tabPageDb.UseVisualStyleBackColor = true;
            // 
            // groupBoxBd
            // 
            this.groupBoxBd.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxBd.Controls.Add(this.btnCreateDateBase);
            this.groupBoxBd.Controls.Add(this.btnRefreshBase);
            this.groupBoxBd.Controls.Add(this.cbxBases);
            this.groupBoxBd.Controls.Add(this.btnRefresh);
            this.groupBoxBd.Controls.Add(this.cbxHosts);
            this.groupBoxBd.Controls.Add(this.cmbDbType);
            this.groupBoxBd.Controls.Add(this.label7);
            this.groupBoxBd.Controls.Add(this.btnTest);
            this.groupBoxBd.Controls.Add(this.txtPassword);
            this.groupBoxBd.Controls.Add(this.label8);
            this.groupBoxBd.Controls.Add(this.txtLogin);
            this.groupBoxBd.Controls.Add(this.label6);
            this.groupBoxBd.Controls.Add(this.label5);
            this.groupBoxBd.Controls.Add(this.label3);
            this.groupBoxBd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxBd.Location = new System.Drawing.Point(3, 3);
            this.groupBoxBd.Name = "groupBoxBd";
            this.groupBoxBd.Size = new System.Drawing.Size(359, 286);
            this.groupBoxBd.TabIndex = 2;
            this.groupBoxBd.TabStop = false;
            this.groupBoxBd.Text = "Параметры доступа к СУБД ";
            // 
            // btnCreateDateBase
            // 
            this.btnCreateDateBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateDateBase.Location = new System.Drawing.Point(278, 100);
            this.btnCreateDateBase.Name = "btnCreateDateBase";
            this.btnCreateDateBase.Size = new System.Drawing.Size(75, 23);
            this.btnCreateDateBase.TabIndex = 21;
            this.btnCreateDateBase.Text = "Создать";
            this.btnCreateDateBase.UseVisualStyleBackColor = true;
            this.btnCreateDateBase.Click += new System.EventHandler(this.btnCreateDateBase_Click);
            // 
            // btnRefreshBase
            // 
            this.btnRefreshBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBase.Location = new System.Drawing.Point(279, 71);
            this.btnRefreshBase.Name = "btnRefreshBase";
            this.btnRefreshBase.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshBase.TabIndex = 20;
            this.btnRefreshBase.Text = "Обновить";
            this.btnRefreshBase.UseVisualStyleBackColor = true;
            this.btnRefreshBase.Click += new System.EventHandler(this.btnRefreshBase_Click);
            // 
            // cbxBases
            // 
            this.cbxBases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBases.FormattingEnabled = true;
            this.cbxBases.Location = new System.Drawing.Point(79, 73);
            this.cbxBases.Name = "cbxBases";
            this.cbxBases.Size = new System.Drawing.Size(194, 21);
            this.cbxBases.TabIndex = 19;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(279, 44);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 18;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbxHosts
            // 
            this.cbxHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxHosts.FormattingEnabled = true;
            this.cbxHosts.Location = new System.Drawing.Point(79, 46);
            this.cbxHosts.Name = "cbxHosts";
            this.cbxHosts.Size = new System.Drawing.Size(194, 21);
            this.cbxHosts.TabIndex = 17;
            // 
            // cmbDbType
            // 
            this.cmbDbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDbType.FormattingEnabled = true;
            this.cmbDbType.Location = new System.Drawing.Point(79, 19);
            this.cmbDbType.Name = "cmbDbType";
            this.cmbDbType.Size = new System.Drawing.Size(274, 21);
            this.cmbDbType.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(8, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Компьютер";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(279, 204);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(74, 23);
            this.btnTest.TabIndex = 12;
            this.btnTest.Text = "Проверить";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(79, 178);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(274, 20);
            this.txtPassword.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(8, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Пароль";
            // 
            // txtLogin
            // 
            this.txtLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogin.Location = new System.Drawing.Point(79, 152);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(274, 20);
            this.txtLogin.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(8, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Логин";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(8, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "База";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(8, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Тип";
            // 
            // tabPageCommon
            // 
            this.tabPageCommon.Controls.Add(this.groupBoxCommon);
            this.tabPageCommon.Location = new System.Drawing.Point(4, 22);
            this.tabPageCommon.Name = "tabPageCommon";
            this.tabPageCommon.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCommon.Size = new System.Drawing.Size(365, 292);
            this.tabPageCommon.TabIndex = 1;
            this.tabPageCommon.Text = "Основные";
            this.tabPageCommon.UseVisualStyleBackColor = true;
            // 
            // groupBoxCommon
            // 
            this.groupBoxCommon.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxCommon.Controls.Add(this.txtInterface);
            this.groupBoxCommon.Controls.Add(this.lblInterface);
            this.groupBoxCommon.Controls.Add(this.registerDateLabel);
            this.groupBoxCommon.Controls.Add(this.registeredByLabel);
            this.groupBoxCommon.Controls.Add(this.txtMaxValueCountPerQuery);
            this.groupBoxCommon.Controls.Add(this.label17);
            this.groupBoxCommon.Controls.Add(this.txtRemotingPort);
            this.groupBoxCommon.Controls.Add(this.label2);
            this.groupBoxCommon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCommon.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCommon.Name = "groupBoxCommon";
            this.groupBoxCommon.Size = new System.Drawing.Size(359, 286);
            this.groupBoxCommon.TabIndex = 8;
            this.groupBoxCommon.TabStop = false;
            this.groupBoxCommon.Text = "Настройки службы";
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Location = new System.Drawing.Point(3, 68);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(64, 13);
            this.lblInterface.TabIndex = 15;
            this.lblInterface.Text = "Интерфейс";
            // 
            // registerDateLabel
            // 
            this.registerDateLabel.AutoSize = true;
            this.registerDateLabel.Location = new System.Drawing.Point(5, 256);
            this.registerDateLabel.Name = "registerDateLabel";
            this.registerDateLabel.Size = new System.Drawing.Size(13, 13);
            this.registerDateLabel.TabIndex = 14;
            this.registerDateLabel.Text = "  ";
            // 
            // registeredByLabel
            // 
            this.registeredByLabel.AutoSize = true;
            this.registeredByLabel.Location = new System.Drawing.Point(3, 228);
            this.registeredByLabel.Name = "registeredByLabel";
            this.registeredByLabel.Size = new System.Drawing.Size(13, 13);
            this.registeredByLabel.TabIndex = 13;
            this.registeredByLabel.Text = "  ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new System.Drawing.Point(2, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Кол. значений";
            // 
            // txtRemotingPort
            // 
            this.txtRemotingPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemotingPort.Location = new System.Drawing.Point(116, 16);
            this.txtRemotingPort.Name = "txtRemotingPort";
            this.txtRemotingPort.Size = new System.Drawing.Size(177, 20);
            this.txtRemotingPort.TabIndex = 4;
            this.txtRemotingPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filtrateOnlyDigigit_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Порт";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageCommon);
            this.tabControlMain.Controls.Add(this.tabPageDb);
            this.tabControlMain.Controls.Add(this.tabPageReservation);
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(373, 318);
            this.tabControlMain.TabIndex = 0;
            // 
            // SettingForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 357);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(290, 328);
            this.Name = "SettingForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка станционного сервера";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.Shown += new System.EventHandler(this.SettingForm_Shown);
            this.VisibleChanged += new System.EventHandler(this.SettingForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPageReservation.ResumeLayout(false);
            this.groupBoxReservation.ResumeLayout(false);
            this.groupBoxReservation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDiffTTL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBackupDay)).EndInit();
            this.tabPageDb.ResumeLayout(false);
            this.groupBoxBd.ResumeLayout(false);
            this.groupBoxBd.PerformLayout();
            this.tabPageCommon.ResumeLayout(false);
            this.groupBoxCommon.ResumeLayout(false);
            this.groupBoxCommon.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.NotifyIcon serviceIcon;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtMaxLoopCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtParameterConstraction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCalculationPeriod;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tabPageReservation;
        private System.Windows.Forms.GroupBox groupBoxReservation;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtBackupFileName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbxBackupType;
        private System.Windows.Forms.NumericUpDown udDiffTTL;
        private System.Windows.Forms.NumericUpDown udBackupDay;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cbxBackupDayOfWeek;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.MaskedTextBox mtbBackupTime;
        private System.Windows.Forms.ComboBox cbxBackupPeriod;
        private System.Windows.Forms.TextBox txtBackupDescription;
        private System.Windows.Forms.TextBox txtBackupName;
        private System.Windows.Forms.CheckBox chkDiff;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabPageDb;
        private System.Windows.Forms.GroupBox groupBoxBd;
        private System.Windows.Forms.Button btnRefreshBase;
        private System.Windows.Forms.ComboBox cbxBases;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbxHosts;
        private System.Windows.Forms.ComboBox cmbDbType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnTest;
        public System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPageCommon;
        private System.Windows.Forms.GroupBox groupBoxCommon;
        private System.Windows.Forms.Label registerDateLabel;
        private System.Windows.Forms.Label registeredByLabel;
        private System.Windows.Forms.TextBox txtMaxValueCountPerQuery;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtRemotingPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TextBox txtInterface;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.Button btnCreateDateBase;
    }
}

