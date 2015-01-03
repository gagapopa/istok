namespace COTES.ISTOK.Client
{
    partial class TepForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TepForm));
            this.tvParameters = new System.Windows.Forms.TreeView();
            this.parametersMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnGroup = new System.Windows.Forms.Button();
            this.dateTimeLabel = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.rtbFormula = new COTES.ISTOK.Client.FormulaTextBox(this.components);
            this.rtbSourceFormula = new COTES.ISTOK.Client.FormulaTextBox(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.parametersMenuStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvParameters
            // 
            this.tvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvParameters.CheckBoxes = true;
            this.tvParameters.ContextMenuStrip = this.parametersMenuStrip;
            this.tvParameters.HideSelection = false;
            this.tvParameters.Location = new System.Drawing.Point(0, 33);
            this.tvParameters.Name = "tvParameters";
            this.tvParameters.ShowLines = false;
            this.tvParameters.ShowNodeToolTips = true;
            this.tvParameters.ShowPlusMinus = false;
            this.tvParameters.ShowRootLines = false;
            this.tvParameters.Size = new System.Drawing.Size(359, 351);
            this.tvParameters.TabIndex = 0;
            this.tvParameters.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvParameters_BeforeExpand);
            this.tvParameters.DoubleClick += new System.EventHandler(this.tvParameters_DoubleClick_1);
            this.tvParameters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvParameters_AfterSelect);
            this.tvParameters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvParameters_MouseUp);
            this.tvParameters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tep_change_KeyDown);
            // 
            // parametersMenuStrip
            // 
            this.parametersMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewValuesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.showToolStripMenuItem});
            this.parametersMenuStrip.Name = "parametersMenuStrip";
            this.parametersMenuStrip.Size = new System.Drawing.Size(185, 76);
            this.parametersMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.parametersMenuStrip_Opening);
            // 
            // viewValuesToolStripMenuItem
            // 
            this.viewValuesToolStripMenuItem.Name = "viewValuesToolStripMenuItem";
            this.viewValuesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.viewValuesToolStripMenuItem.Text = "Просмотр значений";
            this.viewValuesToolStripMenuItem.Click += new System.EventHandler(this.viewValuesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.editToolStripMenuItem.Text = "Изменить параметр";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.showToolStripMenuItem.Text = "Показать параметр";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Параметры:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Формула:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Интервал, с:";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(3, 25);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.ReadOnly = true;
            this.txtInterval.Size = new System.Drawing.Size(95, 20);
            this.txtInterval.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnGroup);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.tvParameters);
            this.splitContainer1.Panel1MinSize = 170;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dateTimeLabel);
            this.splitContainer1.Panel2.Controls.Add(this.txtValue);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.txtCode);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.txtInterval);
            this.splitContainer1.Panel2.Resize += new System.EventHandler(this.splitContainer1_Panel2_Resize);
            this.splitContainer1.Size = new System.Drawing.Size(566, 382);
            this.splitContainer1.SplitterDistance = 360;
            this.splitContainer1.TabIndex = 8;
            // 
            // btnGroup
            // 
            this.btnGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroup.Location = new System.Drawing.Point(261, 4);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(98, 23);
            this.btnGroup.TabIndex = 4;
            this.btnGroup.Text = "Сгруппировать";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            // 
            // dateTimeLabel
            // 
            this.dateTimeLabel.AutoSize = true;
            this.dateTimeLabel.Location = new System.Drawing.Point(102, 9);
            this.dateTimeLabel.Name = "dateTimeLabel";
            this.dateTimeLabel.Size = new System.Drawing.Size(0, 13);
            this.dateTimeLabel.TabIndex = 12;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(105, 25);
            this.txtValue.Name = "txtValue";
            this.txtValue.ReadOnly = true;
            this.txtValue.Size = new System.Drawing.Size(95, 20);
            this.txtValue.TabIndex = 11;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 90);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.rtbFormula);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.rtbSourceFormula);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Size = new System.Drawing.Size(187, 292);
            this.splitContainer2.SplitterDistance = 138;
            this.splitContainer2.TabIndex = 10;
            // 
            // rtbFormula
            // 
            this.rtbFormula.AcceptsTab = true;
            this.rtbFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbFormula.Font = new System.Drawing.Font("Courier New", 10F);
            this.rtbFormula.HideSelection = false;
            this.rtbFormula.Location = new System.Drawing.Point(3, 16);
            this.rtbFormula.Name = "rtbFormula";
            this.rtbFormula.ReadOnly = true;
            this.rtbFormula.ShowCodeImages = false;
            this.rtbFormula.Size = new System.Drawing.Size(181, 119);
            this.rtbFormula.TabIndex = 6;
            this.rtbFormula.Text = "";
            this.rtbFormula.WordWrap = false;
            // 
            // rtbSourceFormula
            // 
            this.rtbSourceFormula.AcceptsTab = true;
            this.rtbSourceFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSourceFormula.Font = new System.Drawing.Font("Courier New", 10F);
            this.rtbSourceFormula.Location = new System.Drawing.Point(6, 16);
            this.rtbSourceFormula.Name = "rtbSourceFormula";
            this.rtbSourceFormula.ReadOnly = true;
            this.rtbSourceFormula.ShowCodeImages = false;
            this.rtbSourceFormula.Size = new System.Drawing.Size(178, 131);
            this.rtbSourceFormula.TabIndex = 8;
            this.rtbSourceFormula.Text = "";
            this.rtbSourceFormula.WordWrap = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(171, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Формула исходного параметра:";
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.HideSelection = false;
            this.txtCode.Location = new System.Drawing.Point(3, 64);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(196, 20);
            this.txtCode.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Код:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 382);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 35);
            this.panel1.TabIndex = 15;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(479, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(398, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // TepForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 439);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TepForm";
            this.Text = "tep_change";
            this.Load += new System.EventHandler(this.tep_change_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tep_change_KeyDown);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.parametersMenuStrip.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.ContextMenuStrip parametersMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewValuesToolStripMenuItem;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label dateTimeLabel;
        private FormulaTextBox rtbFormula;
        private FormulaTextBox rtbSourceFormula;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}
