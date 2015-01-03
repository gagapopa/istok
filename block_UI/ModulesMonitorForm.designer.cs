namespace COTES.ISTOK.Block.UI
{
    partial class ModulesMonitorForm
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
            this.intervalTextBox = new System.Windows.Forms.TextBox();
            this.autoRefreshCheckBox = new System.Windows.Forms.CheckBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.channelMonitoringControl1 = new COTES.ISTOK.Monitoring.ChannelMonitoringControl();
            this.gcCollecectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // intervalTextBox
            // 
            this.intervalTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.intervalTextBox.Enabled = false;
            this.intervalTextBox.Location = new System.Drawing.Point(220, 261);
            this.intervalTextBox.Name = "intervalTextBox";
            this.intervalTextBox.Size = new System.Drawing.Size(75, 20);
            this.intervalTextBox.TabIndex = 3;
            this.intervalTextBox.Text = "1";
            this.intervalTextBox.TextChanged += new System.EventHandler(this.intervalTextBox_TextChanged);
            // 
            // autoRefreshCheckBox
            // 
            this.autoRefreshCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoRefreshCheckBox.AutoSize = true;
            this.autoRefreshCheckBox.Location = new System.Drawing.Point(84, 263);
            this.autoRefreshCheckBox.Name = "autoRefreshCheckBox";
            this.autoRefreshCheckBox.Size = new System.Drawing.Size(140, 17);
            this.autoRefreshCheckBox.TabIndex = 2;
            this.autoRefreshCheckBox.Text = "Автообновление (сек):";
            this.autoRefreshCheckBox.UseVisualStyleBackColor = true;
            this.autoRefreshCheckBox.CheckedChanged += new System.EventHandler(this.autoRefreshCheckBox_CheckedChanged);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RefreshButton.Location = new System.Drawing.Point(3, 259);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshButton.TabIndex = 1;
            this.RefreshButton.Text = "Обновить";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.RefreshButton_Click);
            // 
            // channelMonitoringControl1
            // 
            this.channelMonitoringControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.channelMonitoringControl1.ChannelDiagnostics = null;
            this.channelMonitoringControl1.Location = new System.Drawing.Point(12, 12);
            this.channelMonitoringControl1.Name = "channelMonitoringControl1";
            this.channelMonitoringControl1.Size = new System.Drawing.Size(451, 241);
            this.channelMonitoringControl1.TabIndex = 4;
            // 
            // gcCollecectButton
            // 
            this.gcCollecectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gcCollecectButton.Location = new System.Drawing.Point(301, 259);
            this.gcCollecectButton.Name = "gcCollecectButton";
            this.gcCollecectButton.Size = new System.Drawing.Size(74, 23);
            this.gcCollecectButton.TabIndex = 5;
            this.gcCollecectButton.Text = "GCCollect";
            this.gcCollecectButton.UseVisualStyleBackColor = true;
            this.gcCollecectButton.Visible = false;
            this.gcCollecectButton.Click += new System.EventHandler(this.gcCollecectButton_Click);
            // 
            // ModulesMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 294);
            this.Controls.Add(this.channelMonitoringControl1);
            this.Controls.Add(this.gcCollecectButton);
            this.Controls.Add(this.intervalTextBox);
            this.Controls.Add(this.autoRefreshCheckBox);
            this.Controls.Add(this.RefreshButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(314, 200);
            this.Name = "ModulesMonitorForm";
            this.Text = "Мониторинг каналов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModulesMonitorForm_FormClosing);
            this.Resize += new System.EventHandler(this.ModulesMonitorForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.TextBox intervalTextBox;
        private System.Windows.Forms.CheckBox autoRefreshCheckBox;
        private System.Windows.Forms.Timer timer1;
        private COTES.ISTOK.Monitoring.ChannelMonitoringControl channelMonitoringControl1;
        private System.Windows.Forms.Button gcCollecectButton;
    }
}
