namespace COTES.ISTOK.Client
{
    partial class ParameterValuesEditorForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.valuesViewDataGridView = new System.Windows.Forms.DataGridView();
            this.TimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeginValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramQualityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valuesPrevContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.correctValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decorrectValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevTimeButton = new System.Windows.Forms.Button();
            this.nextTimeButton = new System.Windows.Forms.Button();
            this.intervalComboBox = new System.Windows.Forms.ComboBox();
            this.startIntervalDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.okButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.lockButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.valuesViewDataGridView)).BeginInit();
            this.valuesPrevContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // valuesViewDataGridView
            // 
            this.valuesViewDataGridView.AllowUserToAddRows = false;
            this.valuesViewDataGridView.AllowUserToDeleteRows = false;
            this.valuesViewDataGridView.AllowUserToResizeRows = false;
            this.valuesViewDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valuesViewDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.valuesViewDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.valuesViewDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.valuesViewDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TimeColumn,
            this.BeginValueColumn,
            this.ValueColumn,
            this.ChangeTimeColumn,
            this.Column1,
            this.paramQualityColumn});
            this.valuesViewDataGridView.ContextMenuStrip = this.valuesPrevContextMenuStrip;
            this.valuesViewDataGridView.Location = new System.Drawing.Point(12, 66);
            this.valuesViewDataGridView.Name = "valuesViewDataGridView";
            this.valuesViewDataGridView.RowHeadersVisible = false;
            this.valuesViewDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.valuesViewDataGridView.Size = new System.Drawing.Size(340, 153);
            this.valuesViewDataGridView.TabIndex = 3;
            this.valuesViewDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.valuesViewDataGridView_CellFormatting);
            this.valuesViewDataGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.valuesViewDataGridView_RowPrePaint);
            this.valuesViewDataGridView.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.valuesViewDataGridView_RowValidated);
            this.valuesViewDataGridView.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.valuesViewDataGridView_RowValidating);
            // 
            // TimeColumn
            // 
            this.TimeColumn.DataPropertyName = "Time";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Format = "G";
            dataGridViewCellStyle1.NullValue = null;
            this.TimeColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.TimeColumn.HeaderText = "Время";
            this.TimeColumn.Name = "TimeColumn";
            this.TimeColumn.ReadOnly = true;
            // 
            // BeginValueColumn
            // 
            this.BeginValueColumn.DataPropertyName = "BegValue";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BeginValueColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.BeginValueColumn.HeaderText = "Значение б/п";
            this.BeginValueColumn.Name = "BeginValueColumn";
            this.BeginValueColumn.ReadOnly = true;
            // 
            // ValueColumn
            // 
            this.ValueColumn.DataPropertyName = "Value";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ValueColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.ValueColumn.HeaderText = "Значение";
            this.ValueColumn.Name = "ValueColumn";
            // 
            // ChangeTimeColumn
            // 
            this.ChangeTimeColumn.DataPropertyName = "ChangeTime";
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ChangeTimeColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.ChangeTimeColumn.HeaderText = "Время редактирования";
            this.ChangeTimeColumn.Name = "ChangeTimeColumn";
            this.ChangeTimeColumn.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "par";
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.Visible = false;
            // 
            // paramQualityColumn
            // 
            this.paramQualityColumn.DataPropertyName = "qvl";
            this.paramQualityColumn.HeaderText = "Качество";
            this.paramQualityColumn.Name = "paramQualityColumn";
            this.paramQualityColumn.Visible = false;
            // 
            // valuesPrevContextMenuStrip
            // 
            this.valuesPrevContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.correctToolStripMenuItem,
            this.correctValueToolStripMenuItem,
            this.decorrectValueToolStripMenuItem});
            this.valuesPrevContextMenuStrip.Name = "valuesPrevContextMenuStrip";
            this.valuesPrevContextMenuStrip.Size = new System.Drawing.Size(199, 98);
            this.valuesPrevContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.valuesPrevContextMenuStrip_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addToolStripMenuItem.Text = "Добавить";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // correctToolStripMenuItem
            // 
            this.correctToolStripMenuItem.Name = "correctToolStripMenuItem";
            this.correctToolStripMenuItem.Size = new System.Drawing.Size(195, 6);
            // 
            // correctValueToolStripMenuItem
            // 
            this.correctValueToolStripMenuItem.Name = "correctValueToolStripMenuItem";
            this.correctValueToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.correctValueToolStripMenuItem.Text = "Скорректировать";
            this.correctValueToolStripMenuItem.Click += new System.EventHandler(this.correctValueToolStripMenuItem_Click);
            // 
            // decorrectValueToolStripMenuItem
            // 
            this.decorrectValueToolStripMenuItem.Name = "decorrectValueToolStripMenuItem";
            this.decorrectValueToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.decorrectValueToolStripMenuItem.Text = "Убрать корректировку";
            this.decorrectValueToolStripMenuItem.Click += new System.EventHandler(this.decorrectValueToolStripMenuItem_Click);
            // 
            // prevTimeButton
            // 
            this.prevTimeButton.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowleft_blue242;
            this.prevTimeButton.Location = new System.Drawing.Point(12, 12);
            this.prevTimeButton.Name = "prevTimeButton";
            this.prevTimeButton.Size = new System.Drawing.Size(24, 23);
            this.prevTimeButton.TabIndex = 4;
            this.prevTimeButton.UseVisualStyleBackColor = true;
            this.prevTimeButton.Click += new System.EventHandler(this.prevTimeButton_Click);
            // 
            // nextTimeButton
            // 
            this.nextTimeButton.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowright_blue241;
            this.nextTimeButton.Location = new System.Drawing.Point(226, 12);
            this.nextTimeButton.Name = "nextTimeButton";
            this.nextTimeButton.Size = new System.Drawing.Size(27, 23);
            this.nextTimeButton.TabIndex = 5;
            this.nextTimeButton.UseVisualStyleBackColor = true;
            this.nextTimeButton.Click += new System.EventHandler(this.nextTimeButton_Click);
            // 
            // intervalComboBox
            // 
            this.intervalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.intervalComboBox.FormattingEnabled = true;
            this.intervalComboBox.Location = new System.Drawing.Point(71, 39);
            this.intervalComboBox.Name = "intervalComboBox";
            this.intervalComboBox.Size = new System.Drawing.Size(121, 21);
            this.intervalComboBox.TabIndex = 6;
            this.intervalComboBox.DropDownClosed += new System.EventHandler(this.intervalComboBox_DropDownClosed);
            // 
            // startIntervalDateTimePicker
            // 
            this.startIntervalDateTimePicker.CustomFormat = "dd MMMM yyyy г. HH:mm:ss ";
            this.startIntervalDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startIntervalDateTimePicker.Location = new System.Drawing.Point(42, 13);
            this.startIntervalDateTimePicker.Name = "startIntervalDateTimePicker";
            this.startIntervalDateTimePicker.Size = new System.Drawing.Size(178, 20);
            this.startIntervalDateTimePicker.TabIndex = 8;
            this.startIntervalDateTimePicker.CloseUp += new System.EventHandler(this.startIntervalDateTimePicker_CloseUp);
            this.startIntervalDateTimePicker.KeyUp += new System.Windows.Forms.KeyEventHandler(this.startIntervalDateTimePicker_KeyUp);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(117, 225);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 15;
            this.okButton.Text = "Сохранить";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(196, 225);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 16;
            this.clearButton.Text = "Сбросить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // lockButton
            // 
            this.lockButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lockButton.Location = new System.Drawing.Point(36, 225);
            this.lockButton.Name = "lockButton";
            this.lockButton.Size = new System.Drawing.Size(75, 23);
            this.lockButton.TabIndex = 17;
            this.lockButton.Text = "Изменить";
            this.lockButton.UseVisualStyleBackColor = true;
            this.lockButton.Click += new System.EventHandler(this.lockButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(277, 225);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 18;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ParameterValuesEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(364, 273);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.lockButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.startIntervalDateTimePicker);
            this.Controls.Add(this.intervalComboBox);
            this.Controls.Add(this.nextTimeButton);
            this.Controls.Add(this.prevTimeButton);
            this.Controls.Add(this.valuesViewDataGridView);
            this.MinimumSize = new System.Drawing.Size(370, 300);
            this.Name = "ParameterValuesEditorForm";
            this.Text = "ManualGatePrevValuesEditrorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ParameterValuesEditorForm_FormClosing);
            this.Controls.SetChildIndex(this.valuesViewDataGridView, 0);
            this.Controls.SetChildIndex(this.prevTimeButton, 0);
            this.Controls.SetChildIndex(this.nextTimeButton, 0);
            this.Controls.SetChildIndex(this.intervalComboBox, 0);
            this.Controls.SetChildIndex(this.startIntervalDateTimePicker, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.clearButton, 0);
            this.Controls.SetChildIndex(this.lockButton, 0);
            this.Controls.SetChildIndex(this.closeButton, 0);
            ((System.ComponentModel.ISupportInitialize)(this.valuesViewDataGridView)).EndInit();
            this.valuesPrevContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView valuesViewDataGridView;
        private System.Windows.Forms.ContextMenuStrip valuesPrevContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button prevTimeButton;
        private System.Windows.Forms.Button nextTimeButton;
        private System.Windows.Forms.ComboBox intervalComboBox;
        private System.Windows.Forms.DateTimePicker startIntervalDateTimePicker;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BeginValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChangeTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramQualityColumn;
        private System.Windows.Forms.ToolStripSeparator correctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decorrectValueToolStripMenuItem;
        private System.Windows.Forms.Button lockButton;
        private System.Windows.Forms.Button closeButton;
    }
}