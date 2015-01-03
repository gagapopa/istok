namespace COTES.ISTOK.Client
{
    partial class ParameterGateControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbLock = new System.Windows.Forms.ToolStripButton();
            this.tsbCalc = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPrevTime = new System.Windows.Forms.ToolStripButton();
            this.tsbNextTime = new System.Windows.Forms.ToolStripButton();
            this.tsbMimeTex = new System.Windows.Forms.ToolStripButton();
            this.manualGateDataGridView = new System.Windows.Forms.DataGridView();
            this.paramNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCodeImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.paramCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.beginValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.changeTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showParameterValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDependensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.correctValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decorrectValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevValueCountToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.prevValueCountToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDateTimeColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.manualGateDataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
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
            this.tsbNextTime,
            this.tsbMimeTex});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(518, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbLock
            // 
            this.tsbLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLock.Enabled = false;
            this.tsbLock.Image = global::COTES.ISTOK.Client.Properties.Resources.edit;
            this.tsbLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLock.Name = "tsbLock";
            this.tsbLock.Size = new System.Drawing.Size(23, 22);
            this.tsbLock.Text = "Изменить";
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
            this.tsbSave.Enabled = false;
            this.tsbSave.Image = global::COTES.ISTOK.Client.Properties.Resources.filesave;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Сохранить";
            this.tsbSave.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCancel.Enabled = false;
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
            this.tsbPrevTime.Text = "Предыдущий интервал";
            this.tsbPrevTime.Click += new System.EventHandler(this.prevTimeButton_Click);
            // 
            // tsbNextTime
            // 
            this.tsbNextTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNextTime.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowright_blue241;
            this.tsbNextTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNextTime.Name = "tsbNextTime";
            this.tsbNextTime.Size = new System.Drawing.Size(23, 22);
            this.tsbNextTime.Text = "Следуюший интервал";
            this.tsbNextTime.Click += new System.EventHandler(this.nextTimeButton_Click);
            // 
            // tsbMimeTex
            // 
            this.tsbMimeTex.CheckOnClick = true;
            this.tsbMimeTex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMimeTex.Image = global::COTES.ISTOK.Client.Properties.Resources.mimetex;
            this.tsbMimeTex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMimeTex.Name = "tsbMimeTex";
            this.tsbMimeTex.Size = new System.Drawing.Size(23, 22);
            this.tsbMimeTex.Text = "Код параметра";
            this.tsbMimeTex.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // manualGateDataGridView
            // 
            this.manualGateDataGridView.AllowUserToAddRows = false;
            this.manualGateDataGridView.AllowUserToDeleteRows = false;
            this.manualGateDataGridView.AllowUserToOrderColumns = true;
            this.manualGateDataGridView.AllowUserToResizeRows = false;
            this.manualGateDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.manualGateDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.manualGateDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.manualGateDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramNameColumn,
            this.clmCodeImage,
            this.paramCodeColumn,
            this.paramValueColumn,
            this.beginValueColumn,
            this.paramTimeColumn,
            this.changeTimeColumn});
            this.manualGateDataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.manualGateDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manualGateDataGridView.Location = new System.Drawing.Point(0, 25);
            this.manualGateDataGridView.MultiSelect = false;
            this.manualGateDataGridView.Name = "manualGateDataGridView";
            this.manualGateDataGridView.Size = new System.Drawing.Size(518, 436);
            this.manualGateDataGridView.TabIndex = 3;
            this.manualGateDataGridView.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.manualGateDataGridView_RowValidating);
            this.manualGateDataGridView.DoubleClick += new System.EventHandler(this.showParameterValuesToolStripMenuItem_Click);
            this.manualGateDataGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.manualGateDataGridView_RowPrePaint);
            this.manualGateDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.manualGateDataGridView_CellFormatting);
            this.manualGateDataGridView.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.manualGateDataGridView_RowValidated);
            this.manualGateDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.manualGateDataGridView_DataError);
            // 
            // paramNameColumn
            // 
            this.paramNameColumn.DataPropertyName = "Text";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.paramNameColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.paramNameColumn.HeaderText = "Параметр";
            this.paramNameColumn.Name = "paramNameColumn";
            this.paramNameColumn.ReadOnly = true;
            // 
            // clmCodeImage
            // 
            this.clmCodeImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmCodeImage.DataPropertyName = "ParamCodeImage";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Wheat;
            dataGridViewCellStyle2.NullValue = "System.Drawing.Bitma";
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            this.clmCodeImage.DefaultCellStyle = dataGridViewCellStyle2;
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
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.paramCodeColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.paramCodeColumn.HeaderText = "Код";
            this.paramCodeColumn.Name = "paramCodeColumn";
            this.paramCodeColumn.ReadOnly = true;
            // 
            // paramValueColumn
            // 
            this.paramValueColumn.DataPropertyName = "ParamValue";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.NullValue = "---";
            this.paramValueColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.paramValueColumn.HeaderText = "Значение";
            this.paramValueColumn.Name = "paramValueColumn";
            // 
            // beginValueColumn
            // 
            this.beginValueColumn.DataPropertyName = "ParamBeginValue";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.beginValueColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.beginValueColumn.HeaderText = "Начальное значение";
            this.beginValueColumn.Name = "beginValueColumn";
            this.beginValueColumn.ReadOnly = true;
            // 
            // paramTimeColumn
            // 
            this.paramTimeColumn.DataPropertyName = "ParamTime";
            this.paramTimeColumn.HeaderText = "Время";
            this.paramTimeColumn.Name = "paramTimeColumn";
            this.paramTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // changeTimeColumn
            // 
            this.changeTimeColumn.DataPropertyName = "ParamChangeTime";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.changeTimeColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.changeTimeColumn.HeaderText = "Время изменения";
            this.changeTimeColumn.Name = "changeTimeColumn";
            this.changeTimeColumn.ReadOnly = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showParameterValuesToolStripMenuItem,
            this.showReferencesToolStripMenuItem,
            this.showDependensToolStripMenuItem,
            this.correctToolStripMenuItem,
            this.correctValueToolStripMenuItem,
            this.decorrectValueToolStripMenuItem,
            this.prevValueCountToolStripMenuItem,
            this.prevValueCountToolStripTextBox,
            this.toolStripMenuItem1,
            this.toCSVToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(218, 177);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // showParameterValuesToolStripMenuItem
            // 
            this.showParameterValuesToolStripMenuItem.Name = "showParameterValuesToolStripMenuItem";
            this.showParameterValuesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showParameterValuesToolStripMenuItem.Text = "Просмотреть значения";
            this.showParameterValuesToolStripMenuItem.Click += new System.EventHandler(this.showParameterValuesToolStripMenuItem_Click);
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
            // correctToolStripMenuItem
            // 
            this.correctToolStripMenuItem.Name = "correctToolStripMenuItem";
            this.correctToolStripMenuItem.Size = new System.Drawing.Size(214, 6);
            // 
            // correctValueToolStripMenuItem
            // 
            this.correctValueToolStripMenuItem.Name = "correctValueToolStripMenuItem";
            this.correctValueToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.correctValueToolStripMenuItem.Text = "Скорректировать";
            this.correctValueToolStripMenuItem.Click += new System.EventHandler(this.correctValueToolStripMenuItem_Click);
            // 
            // decorrectValueToolStripMenuItem
            // 
            this.decorrectValueToolStripMenuItem.Name = "decorrectValueToolStripMenuItem";
            this.decorrectValueToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.decorrectValueToolStripMenuItem.Text = "Убрать корректировку";
            this.decorrectValueToolStripMenuItem.Click += new System.EventHandler(this.decorrectValueToolStripMenuItem_Click);
            // 
            // prevValueCountToolStripMenuItem
            // 
            this.prevValueCountToolStripMenuItem.Name = "prevValueCountToolStripMenuItem";
            this.prevValueCountToolStripMenuItem.Size = new System.Drawing.Size(214, 6);
            // 
            // prevValueCountToolStripTextBox
            // 
            this.prevValueCountToolStripTextBox.Name = "prevValueCountToolStripTextBox";
            this.prevValueCountToolStripTextBox.Size = new System.Drawing.Size(100, 21);
            this.prevValueCountToolStripTextBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.prevValueCountToolStripTextBox.TextChanged += new System.EventHandler(this.prevValueCountToolStripTextBox_TextChanged);
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
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Text";
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn1.HeaderText = "Параметр";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 79;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "ParamBeginValue";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn2.HeaderText = "Начальное значение";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 79;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "ParamValue";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.NullValue = "---";
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn3.HeaderText = "Значение";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewDateTimeColumn1
            // 
            this.dataGridViewDateTimeColumn1.DataPropertyName = "ParamTime";
            this.dataGridViewDateTimeColumn1.HeaderText = "Время";
            this.dataGridViewDateTimeColumn1.Name = "dataGridViewDateTimeColumn1";
            this.dataGridViewDateTimeColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDateTimeColumn1.Width = 79;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ParamChangeTime";
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn4.HeaderText = "Время изменения";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 79;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV файлы|*.csv|Все файлы|*.*";
            // 
            // ParameterGateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.manualGateDataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ParameterGateControl";
            this.Size = new System.Drawing.Size(518, 461);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.manualGateDataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView manualGateDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewDateTimeColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showParameterValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showReferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDependensToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator prevValueCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox prevValueCountToolStripTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toCSVToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripSeparator correctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decorrectValueToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbLock;
        private System.Windows.Forms.ToolStripButton tsbCalc;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbPrevTime;
        private System.Windows.Forms.ToolStripButton tsbNextTime;
        private System.Windows.Forms.ToolStripButton tsbMimeTex;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramNameColumn;
        private System.Windows.Forms.DataGridViewImageColumn clmCodeImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramCodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn beginValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn changeTimeColumn;
    }
}
