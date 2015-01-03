namespace COTES.ISTOK.Client
{
    partial class ExcelEditForm
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
            this.excelControl = new COTES.ISTOK.Client.WinExcelControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbLoadFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscbInsertType = new System.Windows.Forms.ToolStripComboBox();
            this.tsbAddParameter = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveParam = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.excelControl);
            this.splitContainer1.Size = new System.Drawing.Size(581, 306);
            this.splitContainer1.SplitterDistance = 345;
            // 
            // pgNode
            // 
            this.pgNode.Size = new System.Drawing.Size(232, 135);
            // 
            // pgParameter
            // 
            this.pgParameter.Size = new System.Drawing.Size(232, 141);
            // 
            // excelControl
            // 
            this.excelControl.AllowDrop = true;
            this.excelControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.excelControl.Location = new System.Drawing.Point(0, 28);
            this.excelControl.Name = "excelControl";
            this.excelControl.Size = new System.Drawing.Size(343, 278);
            this.excelControl.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLoadFile,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.tscbInsertType,
            this.tsbAddParameter,
            this.tsbRemoveParam});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(345, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbLoadFile
            // 
            this.tsbLoadFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLoadFile.Image = global::COTES.ISTOK.Client.Properties.Resources.excel;
            this.tsbLoadFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadFile.Name = "tsbLoadFile";
            this.tsbLoadFile.Size = new System.Drawing.Size(23, 22);
            this.tsbLoadFile.Text = "Загрузить шаблон";
            this.tsbLoadFile.Click += new System.EventHandler(this.tsbLoadFile_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(86, 22);
            this.toolStripLabel1.Text = "Тип параметра:";
            // 
            // tscbInsertType
            // 
            this.tscbInsertType.AutoSize = false;
            this.tscbInsertType.Name = "tscbInsertType";
            this.tscbInsertType.Size = new System.Drawing.Size(180, 21);
            this.tscbInsertType.SelectedIndexChanged += new System.EventHandler(this.tscbInsertType_SelectedIndexChanged);
            // 
            // tsbAddParameter
            // 
            this.tsbAddParameter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddParameter.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_add;
            this.tsbAddParameter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddParameter.Name = "tsbAddParameter";
            this.tsbAddParameter.Size = new System.Drawing.Size(23, 22);
            this.tsbAddParameter.Text = "Добавить параметр";
            this.tsbAddParameter.Click += new System.EventHandler(this.tsbAddParameter_Click);
            // 
            // tsbRemoveParam
            // 
            this.tsbRemoveParam.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveParam.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_remove;
            this.tsbRemoveParam.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveParam.Name = "tsbRemoveParam";
            this.tsbRemoveParam.Size = new System.Drawing.Size(23, 20);
            this.tsbRemoveParam.Text = "Удалить параметр";
            this.tsbRemoveParam.Click += new System.EventHandler(this.tsbRemoveParam_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Файлы Excel |*.xls;*.xlsx";
            // 
            // ExcelEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(581, 363);
            this.Name = "ExcelEditForm";
            this.Load += new System.EventHandler(this.ExcelEditForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExcelEditForm_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WinExcelControl excelControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbRemoveParam;
        private System.Windows.Forms.ToolStripComboBox tscbInsertType;
        private System.Windows.Forms.ToolStripButton tsbAddParameter;
        private System.Windows.Forms.ToolStripButton tsbLoadFile;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}
