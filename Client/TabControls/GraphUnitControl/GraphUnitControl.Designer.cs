namespace COTES.ISTOK.Client
{
    partial class GraphUnitControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphUnitControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cbxPeriod = new System.Windows.Forms.ComboBox();
            this.datTo = new System.Windows.Forms.DateTimePicker();
            this.datFrom = new System.Windows.Forms.DateTimePicker();
            this.btnNextDate = new System.Windows.Forms.Button();
            this.btnPrevDate = new System.Windows.Forms.Button();
            this.queryDataBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.chbxRemoteServer = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.gbInterval = new System.Windows.Forms.GroupBox();
            this.chbxRequestData = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbSorting = new System.Windows.Forms.GroupBox();
            this.cbxSorting = new System.Windows.Forms.ComboBox();
            this.gbPages = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udPageParams = new System.Windows.Forms.NumericUpDown();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.graph = new ZedGraph.ZedGraphControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpValues = new System.Windows.Forms.TabPage();
            this.dgvValues = new System.Windows.Forms.DataGridView();
            this.used = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.param = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpTable = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmsValueGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsGraph = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbData.SuspendLayout();
            this.gbInterval.SuspendLayout();
            this.gbSorting.SuspendLayout();
            this.gbPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPageParams)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValues)).BeginInit();
            this.tpTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.cmsValueGrid.SuspendLayout();
            this.cmsGraph.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxPeriod
            // 
            this.cbxPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPeriod.FormattingEnabled = true;
            this.cbxPeriod.Location = new System.Drawing.Point(219, 23);
            this.cbxPeriod.Name = "cbxPeriod";
            this.cbxPeriod.Size = new System.Drawing.Size(73, 21);
            this.cbxPeriod.TabIndex = 22;
            this.cbxPeriod.SelectedIndexChanged += new System.EventHandler(this.cbxPeriod_SelectedIndexChanged);
            // 
            // datTo
            // 
            this.datTo.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.datTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datTo.Location = new System.Drawing.Point(38, 45);
            this.datTo.Name = "datTo";
            this.datTo.Size = new System.Drawing.Size(136, 20);
            this.datTo.TabIndex = 19;
            // 
            // datFrom
            // 
            this.datFrom.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.datFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datFrom.Location = new System.Drawing.Point(38, 19);
            this.datFrom.Name = "datFrom";
            this.datFrom.Size = new System.Drawing.Size(137, 20);
            this.datFrom.TabIndex = 18;
            this.datFrom.ValueChanged += new System.EventHandler(this.datFrom_ValueChanged);
            // 
            // btnNextDate
            // 
            this.btnNextDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNextDate.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDate.Image")));
            this.btnNextDate.Location = new System.Drawing.Point(298, 16);
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
            this.btnPrevDate.Location = new System.Drawing.Point(181, 16);
            this.btnPrevDate.Name = "btnPrevDate";
            this.btnPrevDate.Size = new System.Drawing.Size(32, 32);
            this.btnPrevDate.TabIndex = 21;
            this.btnPrevDate.UseVisualStyleBackColor = true;
            this.btnPrevDate.Click += new System.EventHandler(this.btnPrevDate_Click);
            // 
            // queryDataBtn
            // 
            this.queryDataBtn.Image = ((System.Drawing.Image)(resources.GetObject("queryDataBtn.Image")));
            this.queryDataBtn.Location = new System.Drawing.Point(36, 16);
            this.queryDataBtn.Name = "queryDataBtn";
            this.queryDataBtn.Size = new System.Drawing.Size(32, 32);
            this.queryDataBtn.TabIndex = 20;
            this.queryDataBtn.UseVisualStyleBackColor = true;
            this.queryDataBtn.Click += new System.EventHandler(this.queryDataBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbData);
            this.splitContainer1.Panel1.Controls.Add(this.gbInterval);
            this.splitContainer1.Panel1.Controls.Add(this.gbSorting);
            this.splitContainer1.Panel1.Controls.Add(this.gbPages);
            this.splitContainer1.Panel1.Controls.Add(this.graph);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(816, 527);
            this.splitContainer1.SplitterDistance = 392;
            this.splitContainer1.TabIndex = 25;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // gbData
            // 
            this.gbData.Controls.Add(this.chbxRemoteServer);
            this.gbData.Controls.Add(this.button3);
            this.gbData.Controls.Add(this.queryDataBtn);
            this.gbData.Location = new System.Drawing.Point(587, 3);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(134, 76);
            this.gbData.TabIndex = 33;
            this.gbData.TabStop = false;
            this.gbData.Text = "Данные";
            // 
            // chbxRemoteServer
            // 
            this.chbxRemoteServer.AutoSize = true;
            this.chbxRemoteServer.Location = new System.Drawing.Point(6, 54);
            this.chbxRemoteServer.Name = "chbxRemoteServer";
            this.chbxRemoteServer.Size = new System.Drawing.Size(123, 17);
            this.chbxRemoteServer.TabIndex = 34;
            this.chbxRemoteServer.Text = "Удаленный сервер";
            this.chbxRemoteServer.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(74, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(32, 32);
            this.button3.TabIndex = 34;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // gbInterval
            // 
            this.gbInterval.Controls.Add(this.chbxRequestData);
            this.gbInterval.Controls.Add(this.datTo);
            this.gbInterval.Controls.Add(this.datFrom);
            this.gbInterval.Controls.Add(this.label1);
            this.gbInterval.Controls.Add(this.label2);
            this.gbInterval.Controls.Add(this.btnNextDate);
            this.gbInterval.Controls.Add(this.btnPrevDate);
            this.gbInterval.Controls.Add(this.cbxPeriod);
            this.gbInterval.Location = new System.Drawing.Point(245, 3);
            this.gbInterval.Name = "gbInterval";
            this.gbInterval.Size = new System.Drawing.Size(336, 76);
            this.gbInterval.TabIndex = 32;
            this.gbInterval.TabStop = false;
            this.gbInterval.Text = "Интервал";
            // 
            // chbxRequestData
            // 
            this.chbxRequestData.AutoSize = true;
            this.chbxRequestData.Location = new System.Drawing.Point(204, 54);
            this.chbxRequestData.Name = "chbxRequestData";
            this.chbxRequestData.Size = new System.Drawing.Size(103, 17);
            this.chbxRequestData.TabIndex = 27;
            this.chbxRequestData.Text = "Запрос данных";
            this.chbxRequestData.UseVisualStyleBackColor = true;
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
            this.label2.Location = new System.Drawing.Point(9, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "До:";
            // 
            // gbSorting
            // 
            this.gbSorting.Controls.Add(this.cbxSorting);
            this.gbSorting.Location = new System.Drawing.Point(137, 3);
            this.gbSorting.Name = "gbSorting";
            this.gbSorting.Size = new System.Drawing.Size(102, 76);
            this.gbSorting.TabIndex = 31;
            this.gbSorting.TabStop = false;
            this.gbSorting.Text = "Сортировка";
            // 
            // cbxSorting
            // 
            this.cbxSorting.FormattingEnabled = true;
            this.cbxSorting.Items.AddRange(new object[] {
            "Не сортировать",
            "Возрастание",
            "Убывание"});
            this.cbxSorting.Location = new System.Drawing.Point(6, 31);
            this.cbxSorting.Name = "cbxSorting";
            this.cbxSorting.Size = new System.Drawing.Size(89, 21);
            this.cbxSorting.TabIndex = 28;
            this.cbxSorting.Text = "Возрастание";
            this.cbxSorting.SelectedIndexChanged += new System.EventHandler(this.cbxSorting_SelectedIndexChanged);
            // 
            // gbPages
            // 
            this.gbPages.Controls.Add(this.label5);
            this.gbPages.Controls.Add(this.udPageParams);
            this.gbPages.Controls.Add(this.btnPrevPage);
            this.gbPages.Controls.Add(this.btnNextPage);
            this.gbPages.Controls.Add(this.lblPageInfo);
            this.gbPages.Location = new System.Drawing.Point(4, 3);
            this.gbPages.Name = "gbPages";
            this.gbPages.Size = new System.Drawing.Size(127, 76);
            this.gbPages.TabIndex = 30;
            this.gbPages.TabStop = false;
            this.gbPages.Text = "Страницы";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "На странице:";
            // 
            // udPageParams
            // 
            this.udPageParams.Location = new System.Drawing.Point(83, 51);
            this.udPageParams.Maximum = new decimal(new int[] {
            10,
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
            1,
            0,
            0,
            0});
            this.udPageParams.ValueChanged += new System.EventHandler(this.udPageParams_ValueChanged);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevPage.Image")));
            this.btnPrevPage.Location = new System.Drawing.Point(6, 19);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(22, 26);
            this.btnPrevPage.TabIndex = 2;
            this.btnPrevPage.UseVisualStyleBackColor = true;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.Location = new System.Drawing.Point(99, 19);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(22, 26);
            this.btnNextPage.TabIndex = 1;
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Location = new System.Drawing.Point(34, 19);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(59, 26);
            this.lblPageInfo.TabIndex = 0;
            this.lblPageInfo.Text = "1/1";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.Location = new System.Drawing.Point(4, 85);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0D;
            this.graph.ScrollMaxX = 0D;
            this.graph.ScrollMaxY = 0D;
            this.graph.ScrollMaxY2 = 0D;
            this.graph.ScrollMinX = 0D;
            this.graph.ScrollMinY = 0D;
            this.graph.ScrollMinY2 = 0D;
            this.graph.Size = new System.Drawing.Size(809, 304);
            this.graph.TabIndex = 0;
            this.graph.Click += new System.EventHandler(this.graph_Click);
            this.graph.Resize += new System.EventHandler(this.graph_Resize);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpValues);
            this.tabControl1.Controls.Add(this.tpTable);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(816, 131);
            this.tabControl1.TabIndex = 3;
            // 
            // tpValues
            // 
            this.tpValues.Controls.Add(this.dgvValues);
            this.tpValues.Location = new System.Drawing.Point(4, 22);
            this.tpValues.Name = "tpValues";
            this.tpValues.Padding = new System.Windows.Forms.Padding(3);
            this.tpValues.Size = new System.Drawing.Size(808, 105);
            this.tpValues.TabIndex = 0;
            this.tpValues.Text = "Значения";
            this.tpValues.UseVisualStyleBackColor = true;
            // 
            // dgvValues
            // 
            this.dgvValues.AllowUserToAddRows = false;
            this.dgvValues.AllowUserToDeleteRows = false;
            this.dgvValues.AllowUserToResizeRows = false;
            this.dgvValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvValues.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.used,
            this.param,
            this.value});
            this.dgvValues.Location = new System.Drawing.Point(0, 0);
            this.dgvValues.Name = "dgvValues";
            this.dgvValues.RowHeadersVisible = false;
            this.dgvValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvValues.Size = new System.Drawing.Size(808, 105);
            this.dgvValues.TabIndex = 0;
            this.dgvValues.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvValues_CellValueChanged);
            this.dgvValues.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvValues_CurrentCellDirtyStateChanged);
            // 
            // used
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "False";
            this.used.DefaultCellStyle = dataGridViewCellStyle1;
            this.used.FalseValue = "false";
            this.used.FillWeight = 20F;
            this.used.HeaderText = "Отображать";
            this.used.Name = "used";
            this.used.TrueValue = "true";
            // 
            // param
            // 
            this.param.HeaderText = "Параметр";
            this.param.Name = "param";
            this.param.ReadOnly = true;
            // 
            // value
            // 
            this.value.HeaderText = "Значение";
            this.value.Name = "value";
            this.value.ReadOnly = true;
            // 
            // tpTable
            // 
            this.tpTable.Controls.Add(this.dataGridView1);
            this.tpTable.Location = new System.Drawing.Point(4, 22);
            this.tpTable.Name = "tpTable";
            this.tpTable.Padding = new System.Windows.Forms.Padding(3);
            this.tpTable.Size = new System.Drawing.Size(808, 105);
            this.tpTable.TabIndex = 1;
            this.tpTable.Text = "Таблица";
            this.tpTable.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.cmsValueGrid;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(802, 99);
            this.dataGridView1.TabIndex = 0;
            // 
            // cmsValueGrid
            // 
            this.cmsValueGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToCSVToolStripMenuItem});
            this.cmsValueGrid.Name = "cmsValueGrid";
            this.cmsValueGrid.Size = new System.Drawing.Size(165, 26);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.exportToCSVToolStripMenuItem.Text = "Выгрузить в CSV";
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // cmsGraph
            // 
            this.cmsGraph.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newItemToolStripMenuItem});
            this.cmsGraph.Name = "cmsGraph";
            this.cmsGraph.Size = new System.Drawing.Size(123, 26);
            // 
            // newItemToolStripMenuItem
            // 
            this.newItemToolStripMenuItem.Name = "newItemToolStripMenuItem";
            this.newItemToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.newItemToolStripMenuItem.Text = "NewItem";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV файлы|*.csv|Все файлы|*.*";
            // 
            // GraphUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.splitContainer1);
            this.Name = "GraphUnitControl";
            this.Size = new System.Drawing.Size(816, 527);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.gbInterval.ResumeLayout(false);
            this.gbInterval.PerformLayout();
            this.gbSorting.ResumeLayout(false);
            this.gbPages.ResumeLayout(false);
            this.gbPages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPageParams)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpValues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvValues)).EndInit();
            this.tpTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.cmsValueGrid.ResumeLayout(false);
            this.cmsGraph.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.Button btnNextDate;
        private System.Windows.Forms.ComboBox cbxPeriod;
        private System.Windows.Forms.Button btnPrevDate;
        private System.Windows.Forms.Button queryDataBtn;
        private System.Windows.Forms.DateTimePicker datTo;
        private System.Windows.Forms.DateTimePicker datFrom;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpValues;
        private System.Windows.Forms.TabPage tpTable;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dgvValues;
        private System.Windows.Forms.DataGridViewCheckBoxColumn used;
        private System.Windows.Forms.DataGridViewTextBoxColumn param;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxSorting;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.GroupBox gbPages;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown udPageParams;
        private System.Windows.Forms.GroupBox gbInterval;
        private System.Windows.Forms.GroupBox gbSorting;
        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ContextMenuStrip cmsGraph;
        private System.Windows.Forms.ToolStripMenuItem newItemToolStripMenuItem;
        private System.Windows.Forms.CheckBox chbxRequestData;
        private System.Windows.Forms.CheckBox chbxRemoteServer;
        private System.Windows.Forms.ContextMenuStrip cmsValueGrid;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;

    }
}
