namespace COTES.ISTOK.Client
{
    partial class BaseAsyncWorkForm
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
            this.statusStripAsyncView = new System.Windows.Forms.StatusStrip();
            this.toolStripSplitButtonAbort = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripAsyncView.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripAsyncView
            // 
            this.statusStripAsyncView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonAbort,
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
            this.statusStripAsyncView.Location = new System.Drawing.Point(0, 244);
            this.statusStripAsyncView.Name = "statusStripAsyncView";
            this.statusStripAsyncView.Size = new System.Drawing.Size(292, 22);
            this.statusStripAsyncView.TabIndex = 13;
            this.statusStripAsyncView.Text = "statusStrip1";
            // 
            // toolStripSplitButtonAbort
            // 
            this.toolStripSplitButtonAbort.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.toolStripSplitButtonAbort.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSplitButtonAbort.AutoToolTip = false;
            this.toolStripSplitButtonAbort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButtonAbort.DoubleClickEnabled = true;
            this.toolStripSplitButtonAbort.DropDownButtonWidth = 0;
            this.toolStripSplitButtonAbort.Image = global::COTES.ISTOK.Client.Properties.Resources.cancel;
            this.toolStripSplitButtonAbort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButtonAbort.Name = "toolStripSplitButtonAbort";
            this.toolStripSplitButtonAbort.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.toolStripSplitButtonAbort.Size = new System.Drawing.Size(21, 20);
            this.toolStripSplitButtonAbort.Text = "Отменить";
            this.toolStripSplitButtonAbort.ButtonClick += new System.EventHandler(this.toolStripSplitButtonAbort_ButtonClick);
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(150, 16);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel.Text = "...";
            // 
            // BaseAsyncWorkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.statusStripAsyncView);
            this.DoubleBuffered = true;
            this.Name = "BaseAsyncWorkForm";
            this.Text = "BaseAsyncWorkForm";
            this.Load += new System.EventHandler(this.BaseAsyncWorkForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BaseAsyncWorkForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BaseAsyncWorkForm_FormClosing);
            this.statusStripAsyncView.ResumeLayout(false);
            this.statusStripAsyncView.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        protected System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonAbort;
        private System.Windows.Forms.StatusStrip statusStripAsyncView;
    }
}