namespace COTES.ISTOK.Modules.Tunnel
{
    partial class ConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.timeOutTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.autoRefreshPeriodTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.abortButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.messageLevelComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // portTextBox
            // 
            resources.ApplyResources(this.portTextBox, "portTextBox");
            this.portTextBox.Name = "portTextBox";
            // 
            // timeOutTextBox
            // 
            resources.ApplyResources(this.timeOutTextBox, "timeOutTextBox");
            this.timeOutTextBox.Name = "timeOutTextBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // autoRefreshPeriodTextBox
            // 
            resources.ApplyResources(this.autoRefreshPeriodTextBox, "autoRefreshPeriodTextBox");
            this.autoRefreshPeriodTextBox.Name = "autoRefreshPeriodTextBox";
            // 
            // saveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // abortButton
            // 
            this.abortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.abortButton, "abortButton");
            this.abortButton.Name = "abortButton";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // messageLevelComboBox
            // 
            resources.ApplyResources(this.messageLevelComboBox, "messageLevelComboBox");
            this.messageLevelComboBox.FormattingEnabled = true;
            this.messageLevelComboBox.Name = "messageLevelComboBox";
            // 
            // ConfigurationForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.abortButton);
            this.Controls.Add(this.autoRefreshPeriodTextBox);
            this.Controls.Add(this.messageLevelComboBox);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.timeOutTextBox);
            this.Name = "ConfigurationForm";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox timeOutTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox autoRefreshPeriodTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox messageLevelComboBox;
    }
}