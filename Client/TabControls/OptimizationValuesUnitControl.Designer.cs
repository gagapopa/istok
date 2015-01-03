namespace COTES.ISTOK.Client
{
    partial class OptimizationValuesUnitControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripButton tsbNextTime;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Все варианты");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.valuesDataGridView = new System.Windows.Forms.DataGridView();
            this.paramNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCodeImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.paramCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showParameterValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDependensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useCalcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parentArgumentTreeView = new System.Windows.Forms.TreeView();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbLock = new System.Windows.Forms.ToolStripButton();
            this.tsbCalc = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPrevTime = new System.Windows.Forms.ToolStripButton();
            this.tsbMimeTex = new System.Windows.Forms.ToolStripButton();
            this.tsbSort = new System.Windows.Forms.ToolStripButton();
            this.tslTop = new System.Windows.Forms.ToolStripLabel();
            tsbNextTime = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valuesDataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsbNextTime
            // 
            tsbNextTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbNextTime.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowright_blue241;
            tsbNextTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbNextTime.Name = "tsbNextTime";
            tsbNextTime.Size = new System.Drawing.Size(23, 22);
            tsbNextTime.Text = "Следующие время";
            tsbNextTime.Click += new System.EventHandler(this.nextTimeButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.valuesDataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.parentArgumentTreeView);
            this.splitContainer1.Size = new System.Drawing.Size(752, 305);
            this.splitContainer1.SplitterDistance = 559;
            this.splitContainer1.TabIndex = 5;
            // 
            // valuesDataGridView
            // 
            this.valuesDataGridView.AllowUserToAddRows = false;
            this.valuesDataGridView.AllowUserToDeleteRows = false;
            this.valuesDataGridView.AllowUserToOrderColumns = true;
            this.valuesDataGridView.AllowUserToResizeRows = false;
            this.valuesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.valuesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.valuesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramNameColumn,
            this.clmCodeImage,
            this.paramCodeColumn});
            this.valuesDataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.valuesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valuesDataGridView.Location = new System.Drawing.Point(0, 0);
            this.valuesDataGridView.MultiSelect = false;
            this.valuesDataGridView.Name = "valuesDataGridView";
            this.valuesDataGridView.Size = new System.Drawing.Size(559, 305);
            this.valuesDataGridView.TabIndex = 4;
            this.valuesDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.valuesDataGridView_CellValidated);
            this.valuesDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.valuesDataGridView_CellFormatting);
            this.valuesDataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.valuesDataGridView_CellPainting);
            // 
            // paramNameColumn
            // 
            this.paramNameColumn.DataPropertyName = "Text";
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.paramNameColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.paramNameColumn.Frozen = true;
            this.paramNameColumn.HeaderText = "Параметр";
            this.paramNameColumn.Name = "paramNameColumn";
            this.paramNameColumn.ReadOnly = true;
            this.paramNameColumn.Width = 177;
            // 
            // clmCodeImage
            // 
            this.clmCodeImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmCodeImage.DataPropertyName = "ParamCodeImage";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Wheat;
            dataGridViewCellStyle5.NullValue = "System.Drawing.Bitma";
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            this.clmCodeImage.DefaultCellStyle = dataGridViewCellStyle5;
            this.clmCodeImage.Frozen = true;
            this.clmCodeImage.HeaderText = "Код";
            this.clmCodeImage.Name = "clmCodeImage";
            this.clmCodeImage.ReadOnly = true;
            this.clmCodeImage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmCodeImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clmCodeImage.Width = 68;
            // 
            // paramCodeColumn
            // 
            this.paramCodeColumn.DataPropertyName = "paramCode";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.paramCodeColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.paramCodeColumn.Frozen = true;
            this.paramCodeColumn.HeaderText = "Код";
            this.paramCodeColumn.Name = "paramCodeColumn";
            this.paramCodeColumn.ReadOnly = true;
            this.paramCodeColumn.Width = 176;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showParameterValuesToolStripMenuItem,
            this.showReferencesToolStripMenuItem,
            this.showDependensToolStripMenuItem,
            this.toolStripMenuItem2,
            this.addColumnToolStripMenuItem,
            this.deleteColumnToolStripMenuItem,
            this.useCalcToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toCSVToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(218, 170);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // showParameterValuesToolStripMenuItem
            // 
            this.showParameterValuesToolStripMenuItem.Name = "showParameterValuesToolStripMenuItem";
            this.showParameterValuesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showParameterValuesToolStripMenuItem.Text = "Просмотреть значения";
            this.showParameterValuesToolStripMenuItem.Visible = false;
            // 
            // showReferencesToolStripMenuItem
            // 
            this.showReferencesToolStripMenuItem.Name = "showReferencesToolStripMenuItem";
            this.showReferencesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showReferencesToolStripMenuItem.Text = "Просмотреть ссылки";
            this.showReferencesToolStripMenuItem.Click += new System.EventHandler(this.showReferencesToolStripMenuItem_Click);
            // 
            // showDependensToolStripMenuItem
            // 
            this.showDependensToolStripMenuItem.Name = "showDependensToolStripMenuItem";
            this.showDependensToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showDependensToolStripMenuItem.Text = "Просмотреть зависимости";
            this.showDependensToolStripMenuItem.Click += new System.EventHandler(this.showDependensToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(214, 6);
            // 
            // addColumnToolStripMenuItem
            // 
            this.addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
            this.addColumnToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.addColumnToolStripMenuItem.Text = "Добавить колонку";
            this.addColumnToolStripMenuItem.Click += new System.EventHandler(this.addColumnToolStripMenuItem_Click);
            // 
            // deleteColumnToolStripMenuItem
            // 
            this.deleteColumnToolStripMenuItem.Name = "deleteColumnToolStripMenuItem";
            this.deleteColumnToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.deleteColumnToolStripMenuItem.Text = "Удалить колонку";
            this.deleteColumnToolStripMenuItem.Click += new System.EventHandler(this.deleteColumnToolStripMenuItem_Click);
            // 
            // useCalcToolStripMenuItem
            // 
            this.useCalcToolStripMenuItem.Name = "useCalcToolStripMenuItem";
            this.useCalcToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.useCalcToolStripMenuItem.Text = "Use Calc";
            this.useCalcToolStripMenuItem.Click += new System.EventHandler(this.useCalcToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(214, 6);
            // 
            // toCSVToolStripMenuItem
            // 
            this.toCSVToolStripMenuItem.Name = "toCSVToolStripMenuItem";
            this.toCSVToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.toCSVToolStripMenuItem.Text = "Выгрузить в CSV";
            this.toCSVToolStripMenuItem.Click += new System.EventHandler(this.toCSVToolStripMenuItem_Click);
            // 
            // parentArgumentTreeView
            // 
            this.parentArgumentTreeView.CheckBoxes = true;
            this.parentArgumentTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parentArgumentTreeView.Location = new System.Drawing.Point(0, 0);
            this.parentArgumentTreeView.Name = "parentArgumentTreeView";
            treeNode2.Checked = true;
            treeNode2.Name = "ShowAllNode";
            treeNode2.Text = "Все варианты";
            this.parentArgumentTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.parentArgumentTreeView.Size = new System.Drawing.Size(189, 305);
            this.parentArgumentTreeView.TabIndex = 0;
            this.parentArgumentTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.parentArgumentTreeView_AfterCheck);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV файлы|*.csv|Все файлы|*.*";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLock,
            this.tsbCalc,
            this.tsbSave,
            this.tsbCancel,
            this.toolStripSeparator1,
            this.tsbPrevTime,
            tsbNextTime,
            this.tsbMimeTex,
            this.tsbSort,
            this.tslTop});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(752, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbLock
            // 
            this.tsbLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLock.Image = global::COTES.ISTOK.Client.Properties.Resources.edit;
            this.tsbLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLock.Name = "tsbLock";
            this.tsbLock.Size = new System.Drawing.Size(23, 22);
            this.tsbLock.Text = "Взять на редактирование";
            this.tsbLock.Click += new System.EventHandler(this.tsbLock_Click);
            // 
            // tsbCalc
            // 
            this.tsbCalc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCalc.Image = global::COTES.ISTOK.Client.Properties.Resources.kcalc;
            this.tsbCalc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCalc.Name = "tsbCalc";
            this.tsbCalc.Size = new System.Drawing.Size(23, 22);
            this.tsbCalc.Text = "Расчитать";
            this.tsbCalc.Click += new System.EventHandler(this.tsbCalc_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::COTES.ISTOK.Client.Properties.Resources.filesave;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Сохранить";
            this.tsbSave.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCancel.Image = global::COTES.ISTOK.Client.Properties.Resources.cancel;
            this.tsbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancel.Name = "tsbCancel";
            this.tsbCancel.Size = new System.Drawing.Size(23, 22);
            this.tsbCancel.Text = "Отмена";
            this.tsbCancel.Click += new System.EventHandler(this.tsbCancel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbPrevTime
            // 
            this.tsbPrevTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPrevTime.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowleft_blue242;
            this.tsbPrevTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPrevTime.Name = "tsbPrevTime";
            this.tsbPrevTime.Size = new System.Drawing.Size(23, 22);
            this.tsbPrevTime.Text = "Предыдущие время";
            this.tsbPrevTime.Click += new System.EventHandler(this.prevTimeButton_Click);
            // 
            // tsbMimeTex
            // 
            this.tsbMimeTex.CheckOnClick = true;
            this.tsbMimeTex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMimeTex.Image = global::COTES.ISTOK.Client.Properties.Resources.mimetex;
            this.tsbMimeTex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMimeTex.Name = "tsbMimeTex";
            this.tsbMimeTex.Size = new System.Drawing.Size(23, 22);
            this.tsbMimeTex.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tsbSort
            // 
            this.tsbSort.CheckOnClick = true;
            this.tsbSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSort.Image = global::COTES.ISTOK.Client.Properties.Resources.optimization_node;
            this.tsbSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSort.Name = "tsbSort";
            this.tsbSort.Size = new System.Drawing.Size(23, 22);
            this.tsbSort.Text = "Сортировать";
            this.tsbSort.CheckedChanged += new System.EventHandler(this.sortCheckBox_CheckedChanged);
            // 
            // tslTop
            // 
            this.tslTop.Name = "tslTop";
            this.tslTop.Size = new System.Drawing.Size(46, 22);
            this.tslTop.Text = "Лучших";
            // 
            // OptimizationValuesUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "OptimizationValuesUnitControl";
            this.Size = new System.Drawing.Size(752, 330);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.valuesDataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView valuesDataGridView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showParameterValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showReferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDependensToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toCSVToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView parentArgumentTreeView;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramNameColumn;
        private System.Windows.Forms.DataGridViewImageColumn clmCodeImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramCodeColumn;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useCalcToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbLock;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private System.Windows.Forms.ToolStripButton tsbCalc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbPrevTime;
        private System.Windows.Forms.ToolStripButton tsbMimeTex;
        private System.Windows.Forms.ToolStripButton tsbSort;
        private System.Windows.Forms.ToolStripLabel tslTop;
        private System.Windows.Forms.ToolStripMenuItem deleteColumnToolStripMenuItem;
    }
}
