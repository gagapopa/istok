//namespace COTES.ISTOK.Modules.Tunnel
//{
//    partial class ModTunnelForm
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModTunnelForm));
//            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
//            this.filterToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
//            this.label2 = new System.Windows.Forms.Label();
//            this.RefreshButton = new System.Windows.Forms.Button();
//            this.monitoringDataGridView = new System.Windows.Forms.DataGridView();
//            this.dataGridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
//            this.columnsGridToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
//            this.columnContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
//            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.hideAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
//            this.unRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.columnsHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.timer1 = new System.Windows.Forms.Timer(this.components);
//            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
//            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
//            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.neutralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
//            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
//            this.autoRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            this.headerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
//            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
//            ((System.ComponentModel.ISupportInitialize)(this.monitoringDataGridView)).BeginInit();
//            this.dataGridContextMenuStrip.SuspendLayout();
//            this.columnContextMenuStrip.SuspendLayout();
//            this.mainMenuStrip.SuspendLayout();
//            this.headerContextMenuStrip.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // toolStripSeparator1
//            // 
//            this.toolStripSeparator1.Name = "toolStripSeparator1";
//            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
//            // 
//            // filterToolStripTextBox
//            // 
//            this.filterToolStripTextBox.Name = "filterToolStripTextBox";
//            resources.ApplyResources(this.filterToolStripTextBox, "filterToolStripTextBox");
//            // 
//            // label2
//            // 
//            resources.ApplyResources(this.label2, "label2");
//            this.label2.Name = "label2";
//            // 
//            // RefreshButton
//            // 
//            resources.ApplyResources(this.RefreshButton, "RefreshButton");
//            this.RefreshButton.Name = "RefreshButton";
//            this.RefreshButton.UseVisualStyleBackColor = true;
//            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
//            // 
//            // monitoringDataGridView
//            // 
//            this.monitoringDataGridView.AllowUserToAddRows = false;
//            this.monitoringDataGridView.AllowUserToDeleteRows = false;
//            this.monitoringDataGridView.AllowUserToOrderColumns = true;
//            resources.ApplyResources(this.monitoringDataGridView, "monitoringDataGridView");
//            this.monitoringDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
//            this.monitoringDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
//            this.monitoringDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
//            this.monitoringDataGridView.ContextMenuStrip = this.dataGridContextMenuStrip;
//            this.monitoringDataGridView.MultiSelect = false;
//            this.monitoringDataGridView.Name = "monitoringDataGridView";
//            this.monitoringDataGridView.ReadOnly = true;
//            this.monitoringDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
//            this.monitoringDataGridView.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.monitoringDataGridView_CellContextMenuStripNeeded);
//            this.monitoringDataGridView.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
//            this.monitoringDataGridView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView1_ColumnAdded);
//            this.monitoringDataGridView.ColumnRemoved += new System.Windows.Forms.DataGridViewColumnEventHandler(this.monitoringDataGridView_ColumnRemoved);
//            // 
//            // dataGridContextMenuStrip
//            // 
//            this.dataGridContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.columnsGridToolStripMenuItem1,
//            this.unRegisterToolStripMenuItem});
//            this.dataGridContextMenuStrip.Name = "dataGridContextMenuStrip";
//            resources.ApplyResources(this.dataGridContextMenuStrip, "dataGridContextMenuStrip");
//            // 
//            // columnsGridToolStripMenuItem1
//            // 
//            this.columnsGridToolStripMenuItem1.DropDown = this.columnContextMenuStrip;
//            this.columnsGridToolStripMenuItem1.Name = "columnsGridToolStripMenuItem1";
//            resources.ApplyResources(this.columnsGridToolStripMenuItem1, "columnsGridToolStripMenuItem1");
//            // 
//            // columnContextMenuStrip
//            // 
//            this.columnContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.showAllToolStripMenuItem,
//            this.hideAllToolStripMenuItem,
//            this.toolStripSeparator2});
//            this.columnContextMenuStrip.Name = "columnContextMenuStrip";
//            this.columnContextMenuStrip.OwnerItem = this.columnsHeaderToolStripMenuItem;
//            this.columnContextMenuStrip.ShowCheckMargin = true;
//            this.columnContextMenuStrip.ShowImageMargin = false;
//            resources.ApplyResources(this.columnContextMenuStrip, "columnContextMenuStrip");
//            // 
//            // showAllToolStripMenuItem
//            // 
//            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
//            resources.ApplyResources(this.showAllToolStripMenuItem, "showAllToolStripMenuItem");
//            this.showAllToolStripMenuItem.Click += new System.EventHandler(this.showAllToolStripMenuItem_Click);
//            // 
//            // hideAllToolStripMenuItem
//            // 
//            this.hideAllToolStripMenuItem.Name = "hideAllToolStripMenuItem";
//            resources.ApplyResources(this.hideAllToolStripMenuItem, "hideAllToolStripMenuItem");
//            this.hideAllToolStripMenuItem.Click += new System.EventHandler(this.hideAllToolStripMenuItem_Click);
//            // 
//            // toolStripSeparator2
//            // 
//            this.toolStripSeparator2.Name = "toolStripSeparator2";
//            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
//            // 
//            // unRegisterToolStripMenuItem
//            // 
//            this.unRegisterToolStripMenuItem.Name = "unRegisterToolStripMenuItem";
//            resources.ApplyResources(this.unRegisterToolStripMenuItem, "unRegisterToolStripMenuItem");
//            this.unRegisterToolStripMenuItem.Click += new System.EventHandler(this.unRegisterToolStripMenuItem_Click);
//            // 
//            // columnsToolStripMenuItem
//            // 
//            this.columnsToolStripMenuItem.DropDown = this.columnContextMenuStrip;
//            this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
//            resources.ApplyResources(this.columnsToolStripMenuItem, "columnsToolStripMenuItem");
//            // 
//            // columnsHeaderToolStripMenuItem
//            // 
//            this.columnsHeaderToolStripMenuItem.DropDown = this.columnContextMenuStrip;
//            this.columnsHeaderToolStripMenuItem.Name = "columnsHeaderToolStripMenuItem";
//            resources.ApplyResources(this.columnsHeaderToolStripMenuItem, "columnsHeaderToolStripMenuItem");
//            // 
//            // timer1
//            // 
//            this.timer1.Interval = 1000;
//            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
//            // 
//            // mainMenuStrip
//            // 
//            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.fileToolStripMenuItem,
//            this.settingsToolStripMenuItem,
//            this.helpToolStripMenuItem});
//            resources.ApplyResources(this.mainMenuStrip, "mainMenuStrip");
//            this.mainMenuStrip.Name = "mainMenuStrip";
//            // 
//            // fileToolStripMenuItem
//            // 
//            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.startToolStripMenuItem,
//            this.stopToolStripMenuItem,
//            this.toolStripMenuItem1,
//            this.quitToolStripMenuItem});
//            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
//            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
//            // 
//            // startToolStripMenuItem
//            // 
//            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
//            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
//            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
//            // 
//            // stopToolStripMenuItem
//            // 
//            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
//            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
//            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
//            // 
//            // toolStripMenuItem1
//            // 
//            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
//            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
//            // 
//            // quitToolStripMenuItem
//            // 
//            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
//            resources.ApplyResources(this.quitToolStripMenuItem, "quitToolStripMenuItem");
//            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
//            // 
//            // settingsToolStripMenuItem
//            // 
//            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.configurationToolStripMenuItem,
//            this.languageToolStripMenuItem,
//            this.columnsToolStripMenuItem,
//            this.toolStripMenuItem2,
//            this.autoRefreshToolStripMenuItem});
//            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
//            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
//            // 
//            // configurationToolStripMenuItem
//            // 
//            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
//            resources.ApplyResources(this.configurationToolStripMenuItem, "configurationToolStripMenuItem");
//            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
//            // 
//            // languageToolStripMenuItem
//            // 
//            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.neutralToolStripMenuItem,
//            this.toolStripMenuItem3});
//            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
//            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
//            this.languageToolStripMenuItem.DropDownOpening += new System.EventHandler(this.languageToolStripMenuItem_DropDownOpening);
//            // 
//            // neutralToolStripMenuItem
//            // 
//            this.neutralToolStripMenuItem.Name = "neutralToolStripMenuItem";
//            resources.ApplyResources(this.neutralToolStripMenuItem, "neutralToolStripMenuItem");
//            this.neutralToolStripMenuItem.Click += new System.EventHandler(this.menuItem_Click);
//            // 
//            // toolStripMenuItem3
//            // 
//            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
//            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
//            // 
//            // toolStripMenuItem2
//            // 
//            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
//            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
//            // 
//            // autoRefreshToolStripMenuItem
//            // 
//            this.autoRefreshToolStripMenuItem.Name = "autoRefreshToolStripMenuItem";
//            resources.ApplyResources(this.autoRefreshToolStripMenuItem, "autoRefreshToolStripMenuItem");
//            this.autoRefreshToolStripMenuItem.Click += new System.EventHandler(this.autoRefreshToolStripMenuItem_Click);
//            // 
//            // helpToolStripMenuItem
//            // 
//            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.aboutToolStripMenuItem});
//            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
//            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
//            // 
//            // aboutToolStripMenuItem
//            // 
//            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
//            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
//            // 
//            // headerContextMenuStrip
//            // 
//            this.headerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
//            this.hideToolStripMenuItem,
//            this.columnsHeaderToolStripMenuItem,
//            this.toolStripSeparator1,
//            this.filterToolStripTextBox});
//            this.headerContextMenuStrip.Name = "headerContextMenuStrip";
//            resources.ApplyResources(this.headerContextMenuStrip, "headerContextMenuStrip");
//            // 
//            // hideToolStripMenuItem
//            // 
//            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
//            resources.ApplyResources(this.hideToolStripMenuItem, "hideToolStripMenuItem");
//            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
//            // 
//            // ModTunnelForm
//            // 
//            resources.ApplyResources(this, "$this");
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.Controls.Add(this.RefreshButton);
//            this.Controls.Add(this.monitoringDataGridView);
//            this.Controls.Add(this.label2);
//            this.Controls.Add(this.mainMenuStrip);
//            this.MainMenuStrip = this.mainMenuStrip;
//            this.Name = "ModTunnelForm";
//            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
//            this.Resize += new System.EventHandler(this.Form1_Resize);
//            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModTunnelForm_FormClosing);
//            this.Load += new System.EventHandler(this.ModTunnelForm_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.monitoringDataGridView)).EndInit();
//            this.dataGridContextMenuStrip.ResumeLayout(false);
//            this.columnContextMenuStrip.ResumeLayout(false);
//            this.mainMenuStrip.ResumeLayout(false);
//            this.mainMenuStrip.PerformLayout();
//            this.headerContextMenuStrip.ResumeLayout(false);
//            this.headerContextMenuStrip.PerformLayout();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion

//        private System.Windows.Forms.Label label2;
//        private System.Windows.Forms.Button RefreshButton;
//        private System.Windows.Forms.DataGridView monitoringDataGridView;
//        private System.Windows.Forms.Timer timer1;
//        private System.Windows.Forms.MenuStrip mainMenuStrip;
//        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
//        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
//        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
//        private System.Windows.Forms.ContextMenuStrip dataGridContextMenuStrip;
//        private System.Windows.Forms.ToolStripMenuItem columnsGridToolStripMenuItem1;
//        private System.Windows.Forms.ContextMenuStrip columnContextMenuStrip;
//        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem hideAllToolStripMenuItem;
//        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
//        private System.Windows.Forms.ContextMenuStrip headerContextMenuStrip;
//        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem columnsHeaderToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem unRegisterToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem columnsToolStripMenuItem;
//        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
//        private System.Windows.Forms.ToolStripMenuItem autoRefreshToolStripMenuItem;
//        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
//        private System.Windows.Forms.ToolStripTextBox filterToolStripTextBox;
//        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
//        private System.Windows.Forms.ToolStripMenuItem neutralToolStripMenuItem;
//        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
//    }
//}

