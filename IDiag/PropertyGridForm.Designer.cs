namespace COTES.ISTOK.Monitoring
{
    partial class PropertyGridForm
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
            this.propertyDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.propertyDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyDataGridView
            // 
            this.propertyDataGridView.AllowUserToAddRows = false;
            this.propertyDataGridView.AllowUserToDeleteRows = false;
            this.propertyDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.propertyDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.propertyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.propertyDataGridView.Location = new System.Drawing.Point(12, 12);
            this.propertyDataGridView.MultiSelect = false;
            this.propertyDataGridView.Name = "propertyDataGridView";
            this.propertyDataGridView.ReadOnly = true;
            this.propertyDataGridView.Size = new System.Drawing.Size(268, 249);
            this.propertyDataGridView.TabIndex = 0;
            // 
            // PropertyGridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.propertyDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PropertyGridForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Окно свойств";
            this.Shown += new System.EventHandler(this.PropertyGridForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.propertyDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView propertyDataGridView;
    }
}