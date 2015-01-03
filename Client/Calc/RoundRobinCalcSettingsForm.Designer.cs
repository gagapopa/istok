namespace COTES.ISTOK.Client
{
    partial class RoundRobinCalcSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoundRobinCalcSettingsForm));
            this.allowAutoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.lastCalcStartLabel = new System.Windows.Forms.Label();
            this.nextCalcStartLabel = new System.Windows.Forms.Label();
            this.checkCalcServerTimer = new System.Windows.Forms.Timer(this.components);
            this.calcTimerLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.calcMessageViewControl1 = new COTES.ISTOK.Client.Calc.CalcMessageViewControl();
            this.SuspendLayout();
            // 
            // allowAutoStartCheckBox
            // 
            this.allowAutoStartCheckBox.AutoSize = true;
            this.allowAutoStartCheckBox.Location = new System.Drawing.Point(12, 12);
            this.allowAutoStartCheckBox.Name = "allowAutoStartCheckBox";
            this.allowAutoStartCheckBox.Size = new System.Drawing.Size(322, 17);
            this.allowAutoStartCheckBox.TabIndex = 0;
            this.allowAutoStartCheckBox.Text = "Разрешить автоматический запуск циклического расчета";
            this.allowAutoStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // lastCalcStartLabel
            // 
            this.lastCalcStartLabel.AutoSize = true;
            this.lastCalcStartLabel.Location = new System.Drawing.Point(12, 32);
            this.lastCalcStartLabel.Name = "lastCalcStartLabel";
            this.lastCalcStartLabel.Size = new System.Drawing.Size(73, 13);
            this.lastCalcStartLabel.TabIndex = 14;
            this.lastCalcStartLabel.Text = "LastCalcStart:";
            // 
            // nextCalcStartLabel
            // 
            this.nextCalcStartLabel.AutoSize = true;
            this.nextCalcStartLabel.Location = new System.Drawing.Point(12, 46);
            this.nextCalcStartLabel.Name = "nextCalcStartLabel";
            this.nextCalcStartLabel.Size = new System.Drawing.Size(75, 13);
            this.nextCalcStartLabel.TabIndex = 15;
            this.nextCalcStartLabel.Text = "NextCalcStart:";
            // 
            // checkCalcServerTimer
            // 
            this.checkCalcServerTimer.Enabled = true;
            this.checkCalcServerTimer.Tick += new System.EventHandler(this.checkCalcServerTimer_Tick);
            // 
            // calcTimerLabel
            // 
            this.calcTimerLabel.AutoSize = true;
            this.calcTimerLabel.Location = new System.Drawing.Point(265, 32);
            this.calcTimerLabel.Name = "calcTimerLabel";
            this.calcTimerLabel.Size = new System.Drawing.Size(53, 13);
            this.calcTimerLabel.TabIndex = 19;
            this.calcTimerLabel.Text = "calcTimer";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(225, 188);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 20;
            this.okButton.Text = "ОК";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(306, 188);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 21;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // calcMessageViewControl1
            // 
            this.calcMessageViewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.calcMessageViewControl1.Location = new System.Drawing.Point(3, 62);
            this.calcMessageViewControl1.Name = "calcMessageViewControl1";
            this.calcMessageViewControl1.PageCount = -1;
            this.calcMessageViewControl1.Size = new System.Drawing.Size(378, 120);
            this.calcMessageViewControl1.TabIndex = 22;
            // 
            // RoundRobinCalcSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 236);
            this.Controls.Add(this.calcMessageViewControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.calcTimerLabel);
            this.Controls.Add(this.nextCalcStartLabel);
            this.Controls.Add(this.lastCalcStartLabel);
            this.Controls.Add(this.allowAutoStartCheckBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(360, 200);
            this.Name = "RoundRobinCalcSettingsForm";
            this.Text = "Настройка циклического расчета";
            this.Controls.SetChildIndex(this.allowAutoStartCheckBox, 0);
            this.Controls.SetChildIndex(this.lastCalcStartLabel, 0);
            this.Controls.SetChildIndex(this.nextCalcStartLabel, 0);
            this.Controls.SetChildIndex(this.calcTimerLabel, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.calcMessageViewControl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox allowAutoStartCheckBox;
        private System.Windows.Forms.Label lastCalcStartLabel;
        private System.Windows.Forms.Label nextCalcStartLabel;
        private System.Windows.Forms.Timer checkCalcServerTimer;
        private System.Windows.Forms.Label calcTimerLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private COTES.ISTOK.Client.Calc.CalcMessageViewControl calcMessageViewControl1;
    }
}
