namespace COTES.ISTOK.Client
{
    partial class ViewAuditForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.auditEntryListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.auditItemListView = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.auditUserRoleLabel = new System.Windows.Forms.Label();
            this.auditUserLoginLabel = new System.Windows.Forms.Label();
            this.auditUserPositionLabel = new System.Windows.Forms.Label();
            this.auditUserFullNameLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.retrieveNext100Button = new System.Windows.Forms.Button();
            this.retrieveLast100Button = new System.Windows.Forms.Button();
            this.retrieveByTimeButton = new System.Windows.Forms.Button();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 123);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.auditEntryListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(697, 328);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 0;
            // 
            // auditEntryListView
            // 
            this.auditEntryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.auditEntryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.auditEntryListView.GridLines = true;
            this.auditEntryListView.HideSelection = false;
            this.auditEntryListView.Location = new System.Drawing.Point(0, 0);
            this.auditEntryListView.Name = "auditEntryListView";
            this.auditEntryListView.Size = new System.Drawing.Size(232, 328);
            this.auditEntryListView.TabIndex = 0;
            this.auditEntryListView.UseCompatibleStateImageBehavior = false;
            this.auditEntryListView.View = System.Windows.Forms.View.Details;
            this.auditEntryListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.auditEntryListView_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Время";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Пользователь";
            this.columnHeader2.Width = 100;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.auditItemListView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(461, 328);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // auditItemListView
            // 
            this.auditItemListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.auditItemListView.GridLines = true;
            this.auditItemListView.Location = new System.Drawing.Point(3, 81);
            this.auditItemListView.Name = "auditItemListView";
            this.auditItemListView.Size = new System.Drawing.Size(455, 244);
            this.auditItemListView.TabIndex = 0;
            this.auditItemListView.UseCompatibleStateImageBehavior = false;
            this.auditItemListView.View = System.Windows.Forms.View.Details;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.auditUserRoleLabel);
            this.groupBox1.Controls.Add(this.auditUserLoginLabel);
            this.groupBox1.Controls.Add(this.auditUserPositionLabel);
            this.groupBox1.Controls.Add(this.auditUserFullNameLabel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 72);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // auditUserRoleLabel
            // 
            this.auditUserRoleLabel.AutoSize = true;
            this.auditUserRoleLabel.Location = new System.Drawing.Point(6, 55);
            this.auditUserRoleLabel.Name = "auditUserRoleLabel";
            this.auditUserRoleLabel.Size = new System.Drawing.Size(73, 13);
            this.auditUserRoleLabel.TabIndex = 3;
            this.auditUserRoleLabel.Text = "Разрешения:";
            // 
            // auditUserLoginLabel
            // 
            this.auditUserLoginLabel.AutoSize = true;
            this.auditUserLoginLabel.Location = new System.Drawing.Point(6, 16);
            this.auditUserLoginLabel.Name = "auditUserLoginLabel";
            this.auditUserLoginLabel.Size = new System.Drawing.Size(41, 13);
            this.auditUserLoginLabel.TabIndex = 0;
            this.auditUserLoginLabel.Text = "Логин:";
            // 
            // auditUserPositionLabel
            // 
            this.auditUserPositionLabel.AutoSize = true;
            this.auditUserPositionLabel.Location = new System.Drawing.Point(6, 42);
            this.auditUserPositionLabel.Name = "auditUserPositionLabel";
            this.auditUserPositionLabel.Size = new System.Drawing.Size(68, 13);
            this.auditUserPositionLabel.TabIndex = 2;
            this.auditUserPositionLabel.Text = "Должность:";
            // 
            // auditUserFullNameLabel
            // 
            this.auditUserFullNameLabel.AutoSize = true;
            this.auditUserFullNameLabel.Location = new System.Drawing.Point(6, 29);
            this.auditUserFullNameLabel.Name = "auditUserFullNameLabel";
            this.auditUserFullNameLabel.Size = new System.Drawing.Size(71, 13);
            this.auditUserFullNameLabel.TabIndex = 1;
            this.auditUserFullNameLabel.Text = "Полное имя:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.checkedListBox1);
            this.panel1.Controls.Add(this.retrieveNext100Button);
            this.panel1.Controls.Add(this.retrieveLast100Button);
            this.panel1.Controls.Add(this.retrieveByTimeButton);
            this.panel1.Controls.Add(this.dateTimePickerTo);
            this.panel1.Controls.Add(this.dateTimePickerFrom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 123);
            this.panel1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(218, 95);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(467, 20);
            this.textBox1.TabIndex = 6;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(218, 8);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(467, 79);
            this.checkedListBox1.TabIndex = 5;
            // 
            // retrieveNext100Button
            // 
            this.retrieveNext100Button.Location = new System.Drawing.Point(105, 93);
            this.retrieveNext100Button.Name = "retrieveNext100Button";
            this.retrieveNext100Button.Size = new System.Drawing.Size(98, 23);
            this.retrieveNext100Button.TabIndex = 4;
            this.retrieveNext100Button.Text = "Следующие 100";
            this.retrieveNext100Button.UseVisualStyleBackColor = true;
            this.retrieveNext100Button.Click += new System.EventHandler(this.retrieveNext100Button_Click);
            // 
            // retrieveLast100Button
            // 
            this.retrieveLast100Button.Location = new System.Drawing.Point(105, 64);
            this.retrieveLast100Button.Name = "retrieveLast100Button";
            this.retrieveLast100Button.Size = new System.Drawing.Size(98, 23);
            this.retrieveLast100Button.TabIndex = 3;
            this.retrieveLast100Button.Text = "Последние 100";
            this.retrieveLast100Button.UseVisualStyleBackColor = true;
            this.retrieveLast100Button.Click += new System.EventHandler(this.retrieveLast100Button_Click);
            // 
            // retrieveByTimeButton
            // 
            this.retrieveByTimeButton.Location = new System.Drawing.Point(12, 64);
            this.retrieveByTimeButton.Name = "retrieveByTimeButton";
            this.retrieveByTimeButton.Size = new System.Drawing.Size(87, 23);
            this.retrieveByTimeButton.TabIndex = 2;
            this.retrieveByTimeButton.Text = "По времени";
            this.retrieveByTimeButton.UseVisualStyleBackColor = true;
            this.retrieveByTimeButton.Click += new System.EventHandler(this.retrieveByTimeButton_Click);
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(12, 38);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerTo.TabIndex = 1;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(12, 12);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerFrom.TabIndex = 0;
            // 
            // ViewAuditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 451);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "ViewAuditForm";
            this.Text = "Аудит";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView auditEntryListView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView auditItemListView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label auditUserRoleLabel;
        private System.Windows.Forms.Label auditUserPositionLabel;
        private System.Windows.Forms.Label auditUserFullNameLabel;
        private System.Windows.Forms.Label auditUserLoginLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button retrieveLast100Button;
        private System.Windows.Forms.Button retrieveByTimeButton;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Button retrieveNext100Button;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TextBox textBox1;
    }
}