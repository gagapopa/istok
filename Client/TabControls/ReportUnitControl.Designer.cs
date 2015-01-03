namespace COTES.ISTOK.Client
{
    partial class ReportUnitControl
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.chbxSave = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(3, 29);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(113, 23);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Сформировать";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // chbxSave
            // 
            this.chbxSave.AutoSize = true;
            this.chbxSave.Checked = true;
            this.chbxSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxSave.Location = new System.Drawing.Point(3, 6);
            this.chbxSave.Name = "chbxSave";
            this.chbxSave.Size = new System.Drawing.Size(134, 17);
            this.chbxSave.TabIndex = 9;
            this.chbxSave.Text = "Сохранить в системе";
            this.chbxSave.UseVisualStyleBackColor = true;
            this.chbxSave.CheckedChanged += new System.EventHandler(this.chbxSave_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbxSave);
            this.panel1.Controls.Add(this.btnCreate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 60);
            this.panel1.TabIndex = 5;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(531, 312);
            this.propertyGrid1.TabIndex = 6;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // ReportUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.panel1);
            this.Name = "ReportUnitControl";
            this.Size = new System.Drawing.Size(531, 372);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.CheckBox chbxSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
