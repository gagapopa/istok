namespace COTES.ISTOK.Client
{
    partial class CreatedReportsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatedReportsForm));
            this.dgv = new System.Windows.Forms.DataGridView();
            this.clmReportName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmsRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.Button();
            this.chbxPages = new System.Windows.Forms.CheckBox();
            this.gbPages = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udPageParams = new System.Windows.Forms.NumericUpDown();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.gbInterval = new System.Windows.Forms.GroupBox();
            this.datTo = new System.Windows.Forms.DateTimePicker();
            this.datFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnNextDate = new System.Windows.Forms.Button();
            this.btnPrevDate = new System.Windows.Forms.Button();
            this.cbxPeriod = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.reportPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.cmsRight.SuspendLayout();
            this.gbPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPageParams)).BeginInit();
            this.gbInterval.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmReportName,
            this.clmDate,
            this.clmUserName});
            this.dgv.ContextMenuStrip = this.cmsRight;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(280, 176);
            this.dgv.TabIndex = 14;
            this.dgv.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgv_MouseDoubleClick);
            this.dgv.CurrentCellChanged += new System.EventHandler(this.dgv_CurrentCellChanged);
            // 
            // clmReportName
            // 
            this.clmReportName.HeaderText = "Отчет";
            this.clmReportName.Name = "clmReportName";
            this.clmReportName.ReadOnly = true;
            // 
            // clmDate
            // 
            this.clmDate.HeaderText = "Дата создания";
            this.clmDate.Name = "clmDate";
            this.clmDate.ReadOnly = true;
            // 
            // clmUserName
            // 
            this.clmUserName.HeaderText = "Пользователь";
            this.clmUserName.Name = "clmUserName";
            this.clmUserName.ReadOnly = true;
            // 
            // cmsRight
            // 
            this.cmsRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.cmsRight.Name = "cmsRight";
            this.cmsRight.Size = new System.Drawing.Size(130, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.removeToolStripMenuItem.Text = "Удалить";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(418, 282);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chbxPages
            // 
            this.chbxPages.AutoSize = true;
            this.chbxPages.Checked = true;
            this.chbxPages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxPages.Location = new System.Drawing.Point(130, 18);
            this.chbxPages.Name = "chbxPages";
            this.chbxPages.Size = new System.Drawing.Size(80, 17);
            this.chbxPages.TabIndex = 18;
            this.chbxPages.Text = "Разбивать";
            this.chbxPages.UseVisualStyleBackColor = true;
            this.chbxPages.CheckedChanged += new System.EventHandler(this.chbxPages_CheckedChanged);
            // 
            // gbPages
            // 
            this.gbPages.Controls.Add(this.label5);
            this.gbPages.Controls.Add(this.udPageParams);
            this.gbPages.Controls.Add(this.chbxPages);
            this.gbPages.Controls.Add(this.btnPrevPage);
            this.gbPages.Controls.Add(this.btnNextPage);
            this.gbPages.Controls.Add(this.lblPageInfo);
            this.gbPages.Location = new System.Drawing.Point(3, 3);
            this.gbPages.Name = "gbPages";
            this.gbPages.Size = new System.Drawing.Size(215, 85);
            this.gbPages.TabIndex = 31;
            this.gbPages.TabStop = false;
            this.gbPages.Text = "Страницы";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "На странице:";
            // 
            // udPageParams
            // 
            this.udPageParams.Location = new System.Drawing.Point(86, 17);
            this.udPageParams.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.udPageParams.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udPageParams.Name = "udPageParams";
            this.udPageParams.Size = new System.Drawing.Size(38, 20);
            this.udPageParams.TabIndex = 31;
            this.udPageParams.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udPageParams.ValueChanged += new System.EventHandler(this.udPageParams_ValueChanged);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevPage.Image")));
            this.btnPrevPage.Location = new System.Drawing.Point(50, 48);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(22, 26);
            this.btnPrevPage.TabIndex = 2;
            this.btnPrevPage.UseVisualStyleBackColor = true;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.Location = new System.Drawing.Point(143, 48);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(22, 26);
            this.btnNextPage.TabIndex = 1;
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Location = new System.Drawing.Point(78, 48);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(59, 26);
            this.lblPageInfo.TabIndex = 0;
            this.lblPageInfo.Text = "1/1";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbInterval
            // 
            this.gbInterval.Controls.Add(this.datTo);
            this.gbInterval.Controls.Add(this.datFrom);
            this.gbInterval.Controls.Add(this.label1);
            this.gbInterval.Controls.Add(this.label2);
            this.gbInterval.Controls.Add(this.btnNextDate);
            this.gbInterval.Controls.Add(this.btnPrevDate);
            this.gbInterval.Controls.Add(this.cbxPeriod);
            this.gbInterval.Location = new System.Drawing.Point(224, 3);
            this.gbInterval.Name = "gbInterval";
            this.gbInterval.Size = new System.Drawing.Size(263, 85);
            this.gbInterval.TabIndex = 33;
            this.gbInterval.TabStop = false;
            this.gbInterval.Text = "Интервал";
            // 
            // datTo
            // 
            this.datTo.CustomFormat = "dd.MM.yyyy";
            this.datTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datTo.Location = new System.Drawing.Point(164, 19);
            this.datTo.Name = "datTo";
            this.datTo.Size = new System.Drawing.Size(94, 20);
            this.datTo.TabIndex = 19;
            this.datTo.ValueChanged += new System.EventHandler(this.datTo_ValueChanged);
            // 
            // datFrom
            // 
            this.datFrom.CustomFormat = "dd.MM.yyyy";
            this.datFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datFrom.Location = new System.Drawing.Point(38, 19);
            this.datFrom.Name = "datFrom";
            this.datFrom.Size = new System.Drawing.Size(94, 20);
            this.datFrom.TabIndex = 18;
            this.datFrom.ValueChanged += new System.EventHandler(this.datFrom_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "От:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "До:";
            // 
            // btnNextDate
            // 
            this.btnNextDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNextDate.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDate.Image")));
            this.btnNextDate.Location = new System.Drawing.Point(199, 45);
            this.btnNextDate.Name = "btnNextDate";
            this.btnNextDate.Size = new System.Drawing.Size(32, 32);
            this.btnNextDate.TabIndex = 23;
            this.btnNextDate.UseVisualStyleBackColor = true;
            this.btnNextDate.Click += new System.EventHandler(this.btnNextDate_Click);
            // 
            // btnPrevDate
            // 
            this.btnPrevDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrevDate.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevDate.Image")));
            this.btnPrevDate.Location = new System.Drawing.Point(38, 45);
            this.btnPrevDate.Name = "btnPrevDate";
            this.btnPrevDate.Size = new System.Drawing.Size(32, 32);
            this.btnPrevDate.TabIndex = 21;
            this.btnPrevDate.UseVisualStyleBackColor = true;
            this.btnPrevDate.Click += new System.EventHandler(this.btnPrevDate_Click);
            // 
            // cbxPeriod
            // 
            this.cbxPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPeriod.FormattingEnabled = true;
            this.cbxPeriod.Location = new System.Drawing.Point(76, 52);
            this.cbxPeriod.Name = "cbxPeriod";
            this.cbxPeriod.Size = new System.Drawing.Size(117, 21);
            this.cbxPeriod.TabIndex = 22;
            this.cbxPeriod.SelectedIndexChanged += new System.EventHandler(this.cbxPeriod_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(3, 100);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgv);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.reportPropertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(490, 176);
            this.splitContainer1.SplitterDistance = 280;
            this.splitContainer1.TabIndex = 34;
            // 
            // reportPropertyGrid
            // 
            this.reportPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.reportPropertyGrid.Name = "reportPropertyGrid";
            this.reportPropertyGrid.Size = new System.Drawing.Size(206, 176);
            this.reportPropertyGrid.TabIndex = 0;
            this.reportPropertyGrid.ToolbarVisible = false;
            this.reportPropertyGrid.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(496, 309);
            this.tableLayoutPanel1.TabIndex = 35;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbPages);
            this.panel1.Controls.Add(this.gbInterval);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(490, 91);
            this.panel1.TabIndex = 35;
            // 
            // CreatedReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(496, 331);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(504, 300);
            this.Name = "CreatedReportsForm";
            this.Text = "Готовые отчеты";
            this.Load += new System.EventHandler(this.CreatedReportsForm_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.cmsRight.ResumeLayout(false);
            this.gbPages.ResumeLayout(false);
            this.gbPages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPageParams)).EndInit();
            this.gbInterval.ResumeLayout(false);
            this.gbInterval.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chbxPages;
        private System.Windows.Forms.GroupBox gbPages;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown udPageParams;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.GroupBox gbInterval;
        private System.Windows.Forms.DateTimePicker datTo;
        private System.Windows.Forms.DateTimePicker datFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnNextDate;
        private System.Windows.Forms.Button btnPrevDate;
        private System.Windows.Forms.ComboBox cbxPeriod;
        private System.Windows.Forms.ContextMenuStrip cmsRight;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmReportName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmUserName;
        private System.Windows.Forms.PropertyGrid reportPropertyGrid;
    }
}
