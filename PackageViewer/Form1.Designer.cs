namespace COTES.ISTOK
{
    partial class Form1
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.clbParameters = new System.Windows.Forms.CheckedListBox();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.btnLoadPackages = new System.Windows.Forms.Button();
            this.lbxPackages = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(340, 167);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.Size = new System.Drawing.Size(409, 159);
            this.dgv.TabIndex = 0;
            // 
            // clbParameters
            // 
            this.clbParameters.CheckOnClick = true;
            this.clbParameters.FormattingEnabled = true;
            this.clbParameters.HorizontalScrollbar = true;
            this.clbParameters.Location = new System.Drawing.Point(12, 39);
            this.clbParameters.Name = "clbParameters";
            this.clbParameters.Size = new System.Drawing.Size(322, 109);
            this.clbParameters.TabIndex = 1;
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDateFrom.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateFrom.Location = new System.Drawing.Point(600, 12);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.Size = new System.Drawing.Size(149, 20);
            this.dtpDateFrom.TabIndex = 2;
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDateTo.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateTo.Location = new System.Drawing.Point(600, 38);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.Size = new System.Drawing.Size(149, 20);
            this.dtpDateTo.TabIndex = 3;
            // 
            // btnLoadPackages
            // 
            this.btnLoadPackages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadPackages.Location = new System.Drawing.Point(674, 138);
            this.btnLoadPackages.Name = "btnLoadPackages";
            this.btnLoadPackages.Size = new System.Drawing.Size(75, 23);
            this.btnLoadPackages.TabIndex = 4;
            this.btnLoadPackages.Text = "Load";
            this.btnLoadPackages.UseVisualStyleBackColor = true;
            this.btnLoadPackages.Click += new System.EventHandler(this.btnLoadPackages_Click);
            // 
            // lbxPackages
            // 
            this.lbxPackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxPackages.FormattingEnabled = true;
            this.lbxPackages.HorizontalScrollbar = true;
            this.lbxPackages.Location = new System.Drawing.Point(12, 167);
            this.lbxPackages.Name = "lbxPackages";
            this.lbxPackages.Size = new System.Drawing.Size(322, 147);
            this.lbxPackages.TabIndex = 5;
            this.lbxPackages.SelectedIndexChanged += new System.EventHandler(this.lbxPackages_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 338);
            this.Controls.Add(this.lbxPackages);
            this.Controls.Add(this.btnLoadPackages);
            this.Controls.Add(this.dtpDateTo);
            this.Controls.Add(this.dtpDateFrom);
            this.Controls.Add(this.clbParameters);
            this.Controls.Add(this.dgv);
            this.Name = "Form1";
            this.Text = "PackageViewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.CheckedListBox clbParameters;
        private System.Windows.Forms.DateTimePicker dtpDateFrom;
        private System.Windows.Forms.DateTimePicker dtpDateTo;
        private System.Windows.Forms.Button btnLoadPackages;
        private System.Windows.Forms.ListBox lbxPackages;
    }
}

