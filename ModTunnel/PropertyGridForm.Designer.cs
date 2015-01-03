namespace COTES.ISTOK.Modules.Tunnel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyGridForm));
            this.propertyDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.propertyDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyDataGridView
            // 
            this.propertyDataGridView.AccessibleDescription = null;
            this.propertyDataGridView.AccessibleName = null;
            this.propertyDataGridView.AllowUserToAddRows = false;
            this.propertyDataGridView.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.propertyDataGridView, "propertyDataGridView");
            this.propertyDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.propertyDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.propertyDataGridView.BackgroundImage = null;
            this.propertyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.propertyDataGridView.Font = null;
            this.propertyDataGridView.MultiSelect = false;
            this.propertyDataGridView.Name = "propertyDataGridView";
            this.propertyDataGridView.ReadOnly = true;
            // 
            // PropertyGridForm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.propertyDataGridView);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "PropertyGridForm";
            this.Shown += new System.EventHandler(this.PropertyGridForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.propertyDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView propertyDataGridView;
    }
}