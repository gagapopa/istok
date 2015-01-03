namespace COTES.ISTOK.Client
{
    partial class BaseScheduleForm
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
            this.listViewSchedules = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listViewSchedules
            // 
            this.listViewSchedules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSchedules.Location = new System.Drawing.Point(0, 0);
            this.listViewSchedules.MultiSelect = false;
            this.listViewSchedules.Name = "listViewSchedules";
            this.listViewSchedules.Size = new System.Drawing.Size(292, 244);
            this.listViewSchedules.TabIndex = 14;
            this.listViewSchedules.UseCompatibleStateImageBehavior = false;
            // 
            // BaseScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.listViewSchedules);
            this.Name = "BaseScheduleForm";
            this.Text = "BaseScheduleForm";
            this.Controls.SetChildIndex(this.listViewSchedules, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ListView listViewSchedules;

    }
}