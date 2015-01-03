namespace COTES.ISTOK.Client
{
    partial class ExcelImportForm
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.cmsMeta = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMetaName = new System.Windows.Forms.ToolStripTextBox();
            this.addMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxPages = new System.Windows.Forms.ComboBox();
            this.cbxParameters = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chbxEditor = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.revisionComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.cmsMeta.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ContextMenuStrip = this.cmsMeta;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(570, 189);
            this.dgv.TabIndex = 0;
            // 
            // cmsMeta
            // 
            this.cmsMeta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMetaName,
            this.addMetaToolStripMenuItem,
            this.deleteMetaToolStripMenuItem});
            this.cmsMeta.Name = "cmsMeta";
            this.cmsMeta.Size = new System.Drawing.Size(161, 71);
            // 
            // tsMetaName
            // 
            this.tsMetaName.Name = "tsMetaName";
            this.tsMetaName.Size = new System.Drawing.Size(100, 21);
            // 
            // addMetaToolStripMenuItem
            // 
            this.addMetaToolStripMenuItem.Name = "addMetaToolStripMenuItem";
            this.addMetaToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addMetaToolStripMenuItem.Text = "Add meta";
            this.addMetaToolStripMenuItem.Click += new System.EventHandler(this.addMetaToolStripMenuItem_Click);
            // 
            // deleteMetaToolStripMenuItem
            // 
            this.deleteMetaToolStripMenuItem.Name = "deleteMetaToolStripMenuItem";
            this.deleteMetaToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.deleteMetaToolStripMenuItem.Text = "Delete meta";
            this.deleteMetaToolStripMenuItem.Click += new System.EventHandler(this.deleteMetaToolStripMenuItem_Click);
            // 
            // cbxPages
            // 
            this.cbxPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxPages.FormattingEnabled = true;
            this.cbxPages.Location = new System.Drawing.Point(76, 12);
            this.cbxPages.Name = "cbxPages";
            this.cbxPages.Size = new System.Drawing.Size(506, 21);
            this.cbxPages.TabIndex = 1;
            this.cbxPages.SelectedIndexChanged += new System.EventHandler(this.cbxPages_SelectedIndexChanged);
            // 
            // cbxParameters
            // 
            this.cbxParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxParameters.FormattingEnabled = true;
            this.cbxParameters.Location = new System.Drawing.Point(76, 39);
            this.cbxParameters.Name = "cbxParameters";
            this.cbxParameters.Size = new System.Drawing.Size(506, 21);
            this.cbxParameters.TabIndex = 2;
            this.cbxParameters.SelectedIndexChanged += new System.EventHandler(this.cbxParameters_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Page:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Parameter:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 95);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgv);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.splitContainer1_Panel2_ControlAdded);
            this.splitContainer1.Panel2.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.splitContainer1_Panel2_ControlRemoved);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(570, 189);
            this.splitContainer1.SplitterDistance = 285;
            this.splitContainer1.TabIndex = 6;
            // 
            // chbxEditor
            // 
            this.chbxEditor.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbxEditor.AutoSize = true;
            this.chbxEditor.Location = new System.Drawing.Point(76, 66);
            this.chbxEditor.Name = "chbxEditor";
            this.chbxEditor.Size = new System.Drawing.Size(44, 23);
            this.chbxEditor.TabIndex = 8;
            this.chbxEditor.Text = "Editor";
            this.chbxEditor.UseVisualStyleBackColor = true;
            this.chbxEditor.CheckedChanged += new System.EventHandler(this.chbxEditor_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 316);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(594, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(59, 17);
            this.lblStatus.Text = "Status: OK";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(426, 290);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(507, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // revisionComboBox
            // 
            this.revisionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.revisionComboBox.FormattingEnabled = true;
            this.revisionComboBox.Location = new System.Drawing.Point(126, 68);
            this.revisionComboBox.Name = "revisionComboBox";
            this.revisionComboBox.Size = new System.Drawing.Size(216, 21);
            this.revisionComboBox.TabIndex = 12;
            // 
            // ExcelImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 338);
            this.Controls.Add(this.revisionComboBox);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbxParameters);
            this.Controls.Add(this.chbxEditor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxPages);
            this.Name = "ExcelImportForm";
            this.Text = "ExcelImport";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.cmsMeta.ResumeLayout(false);
            this.cmsMeta.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.ComboBox cbxPages;
        private System.Windows.Forms.ComboBox cbxParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip cmsMeta;
        private System.Windows.Forms.ToolStripTextBox tsMetaName;
        private System.Windows.Forms.ToolStripMenuItem addMetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMetaToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chbxEditor;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox revisionComboBox;
    }
}

