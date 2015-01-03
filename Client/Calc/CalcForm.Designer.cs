namespace COTES.ISTOK.Client
{
    partial class CalcForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalcForm));
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.datTo = new System.Windows.Forms.DateTimePicker();
            this.datFrom = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nextTimeButton = new System.Windows.Forms.Button();
            this.prevTimeButton = new System.Windows.Forms.Button();
            this.cbxRecalcAll = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.messagesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goToValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tvParameters = new System.Windows.Forms.TreeView();
            this.cmsRelations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showParameterValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStop = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbxColored = new System.Windows.Forms.CheckBox();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.calcMessageViewControl1 = new COTES.ISTOK.Client.Calc.CalcMessageViewControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.messagesContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.cmsRelations.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(194, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "До";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "C";
            // 
            // datTo
            // 
            this.datTo.CustomFormat = "dd MMMM yyyy HH:mm";
            this.datTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datTo.Location = new System.Drawing.Point(221, 19);
            this.datTo.Name = "datTo";
            this.datTo.Size = new System.Drawing.Size(157, 20);
            this.datTo.TabIndex = 7;
            // 
            // datFrom
            // 
            this.datFrom.CustomFormat = "dd MMMM yyyy HH:mm";
            this.datFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datFrom.Location = new System.Drawing.Point(31, 19);
            this.datFrom.Name = "datFrom";
            this.datFrom.Size = new System.Drawing.Size(157, 20);
            this.datFrom.TabIndex = 5;
            this.datFrom.ValueChanged += new System.EventHandler(this.datFrom_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.nextTimeButton);
            this.groupBox1.Controls.Add(this.prevTimeButton);
            this.groupBox1.Controls.Add(this.datFrom);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.datTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(461, 46);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Период перерасчета";
            // 
            // nextTimeButton
            // 
            this.nextTimeButton.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowright_blue241;
            this.nextTimeButton.Location = new System.Drawing.Point(419, 18);
            this.nextTimeButton.Name = "nextTimeButton";
            this.nextTimeButton.Size = new System.Drawing.Size(27, 23);
            this.nextTimeButton.TabIndex = 9;
            this.nextTimeButton.UseVisualStyleBackColor = true;
            this.nextTimeButton.Click += new System.EventHandler(this.nextTimeButton_Click);
            // 
            // prevTimeButton
            // 
            this.prevTimeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.prevTimeButton.Image = global::COTES.ISTOK.Client.Properties.Resources.arrowleft_blue242;
            this.prevTimeButton.Location = new System.Drawing.Point(384, 18);
            this.prevTimeButton.Name = "prevTimeButton";
            this.prevTimeButton.Size = new System.Drawing.Size(29, 23);
            this.prevTimeButton.TabIndex = 8;
            this.prevTimeButton.UseVisualStyleBackColor = true;
            this.prevTimeButton.Click += new System.EventHandler(this.prevTimeButton_Click);
            // 
            // cbxRecalcAll
            // 
            this.cbxRecalcAll.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbxRecalcAll.Location = new System.Drawing.Point(3, 172);
            this.cbxRecalcAll.Name = "cbxRecalcAll";
            this.cbxRecalcAll.Size = new System.Drawing.Size(160, 24);
            this.cbxRecalcAll.TabIndex = 10;
            this.cbxRecalcAll.Text = "Пересчитать все";
            this.cbxRecalcAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbxRecalcAll.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 55);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 23);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "Запустить расчет";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // messagesContextMenuStrip
            // 
            this.messagesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToValuesToolStripMenuItem,
            this.goToParameterToolStripMenuItem});
            this.messagesContextMenuStrip.Name = "messagesContextMenuStrip";
            this.messagesContextMenuStrip.Size = new System.Drawing.Size(195, 48);
            // 
            // goToValuesToolStripMenuItem
            // 
            this.goToValuesToolStripMenuItem.Name = "goToValuesToolStripMenuItem";
            this.goToValuesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.goToValuesToolStripMenuItem.Text = "Go to values";
            this.goToValuesToolStripMenuItem.Visible = false;
            // 
            // goToParameterToolStripMenuItem
            // 
            this.goToParameterToolStripMenuItem.Name = "goToParameterToolStripMenuItem";
            this.goToParameterToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.goToParameterToolStripMenuItem.Text = "Перейти к параметру";
            this.goToParameterToolStripMenuItem.Click += new System.EventHandler(this.goToParameterToolStripMenuItem_Click);
            // 
            // tvParameters
            // 
            this.tvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvParameters.CheckBoxes = true;
            this.tvParameters.ContextMenuStrip = this.cmsRelations;
            this.tvParameters.Location = new System.Drawing.Point(169, 55);
            this.tvParameters.Name = "tvParameters";
            this.tvParameters.ShowNodeToolTips = true;
            this.tvParameters.Size = new System.Drawing.Size(298, 147);
            this.tvParameters.TabIndex = 14;
            this.tvParameters.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvParameters_AfterCheck);
            // 
            // cmsRelations
            // 
            this.cmsRelations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRefToolStripMenuItem,
            this.showDepToolStripMenuItem,
            this.showParameterValuesToolStripMenuItem});
            this.cmsRelations.Name = "contextMenuStrip1";
            this.cmsRelations.Size = new System.Drawing.Size(200, 70);
            this.cmsRelations.Opening += new System.ComponentModel.CancelEventHandler(this.cmsRelations_Opening);
            // 
            // showRefToolStripMenuItem
            // 
            this.showRefToolStripMenuItem.Name = "showRefToolStripMenuItem";
            this.showRefToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showRefToolStripMenuItem.Text = "Показать ссылки";
            this.showRefToolStripMenuItem.Click += new System.EventHandler(this.showRefToolStripMenuItem_Click);
            // 
            // showDepToolStripMenuItem
            // 
            this.showDepToolStripMenuItem.Name = "showDepToolStripMenuItem";
            this.showDepToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showDepToolStripMenuItem.Text = "Показать зависимости";
            this.showDepToolStripMenuItem.Click += new System.EventHandler(this.showDepToolStripMenuItem_Click);
            // 
            // showParameterValuesToolStripMenuItem
            // 
            this.showParameterValuesToolStripMenuItem.Name = "showParameterValuesToolStripMenuItem";
            this.showParameterValuesToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showParameterValuesToolStripMenuItem.Text = "Показать значения";
            this.showParameterValuesToolStripMenuItem.Click += new System.EventHandler(this.showParameterValuesToolStripMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(3, 84);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(160, 23);
            this.btnStop.TabIndex = 15;
            this.btnStop.Text = "Остановить расчет";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.cbxRecalcAll);
            this.splitContainer1.Panel1.Controls.Add(this.cbxColored);
            this.splitContainer1.Panel1.Controls.Add(this.btnCheckAll);
            this.splitContainer1.Panel1.Controls.Add(this.btnUncheckAll);
            this.splitContainer1.Panel1.Controls.Add(this.btnStart);
            this.splitContainer1.Panel1.Controls.Add(this.tvParameters);
            this.splitContainer1.Panel1.Controls.Add(this.btnStop);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 198;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.calcMessageViewControl1);
            this.splitContainer1.Size = new System.Drawing.Size(467, 381);
            this.splitContainer1.SplitterDistance = 205;
            this.splitContainer1.TabIndex = 16;
            // 
            // cbxColored
            // 
            this.cbxColored.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbxColored.Checked = true;
            this.cbxColored.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxColored.Location = new System.Drawing.Point(3, 142);
            this.cbxColored.Name = "cbxColored";
            this.cbxColored.Size = new System.Drawing.Size(160, 24);
            this.cbxColored.TabIndex = 18;
            this.cbxColored.Text = "Отображать ход расчета";
            this.cbxColored.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbxColored.UseVisualStyleBackColor = true;
            this.cbxColored.CheckedChanged += new System.EventHandler(this.cbxColored_CheckedChanged);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(3, 113);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(86, 23);
            this.btnCheckAll.TabIndex = 17;
            this.btnCheckAll.Text = "Выделить все";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Location = new System.Drawing.Point(95, 113);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(68, 23);
            this.btnUncheckAll.TabIndex = 16;
            this.btnUncheckAll.Text = "Снять все";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // calcMessageViewControl1
            // 
            this.calcMessageViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calcMessageViewControl1.GridContextMenu = this.messagesContextMenuStrip;
            this.calcMessageViewControl1.Location = new System.Drawing.Point(0, 0);
            this.calcMessageViewControl1.Name = "calcMessageViewControl1";
            this.calcMessageViewControl1.PageCount = -1;
            this.calcMessageViewControl1.Size = new System.Drawing.Size(467, 172);
            this.calcMessageViewControl1.StatusString = null;
            this.calcMessageViewControl1.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 403);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(475, 430);
            this.Name = "CalcForm";
            this.ShowInTaskbar = false;
            this.Text = "Расчет ТЭП";
            
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.messagesContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.cmsRelations.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker datTo;
        private System.Windows.Forms.DateTimePicker datFrom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TreeView tvParameters;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip cmsRelations;
        private System.Windows.Forms.ToolStripMenuItem showRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDepToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbxColored;
        private System.Windows.Forms.CheckBox cbxRecalcAll;
        private System.Windows.Forms.Button nextTimeButton;
        private System.Windows.Forms.Button prevTimeButton;
        private System.Windows.Forms.ContextMenuStrip messagesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem goToValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showParameterValuesToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSource1;
        private COTES.ISTOK.Client.Calc.CalcMessageViewControl calcMessageViewControl1;
    }
}
