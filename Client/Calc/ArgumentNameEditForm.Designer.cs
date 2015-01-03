namespace COTES.ISTOK.Client
{
    partial class ArgumentNameEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.argumentNameTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.valueModegroupBox = new System.Windows.Forms.GroupBox();
            this.columnNumModeRadioButton = new System.Windows.Forms.RadioButton();
            this.intervalStepLabel = new System.Windows.Forms.Label();
            this.intervalToLabel = new System.Windows.Forms.Label();
            this.intervalFromLabel = new System.Windows.Forms.Label();
            this.intervalStepTextBox = new System.Windows.Forms.TextBox();
            this.intervalToTextBox = new System.Windows.Forms.TextBox();
            this.intervalFromTextBox = new System.Windows.Forms.TextBox();
            this.intervalModeRadioButton = new System.Windows.Forms.RadioButton();
            this.expressionModeRadioButton = new System.Windows.Forms.RadioButton();
            this.manualModeRadioButton = new System.Windows.Forms.RadioButton();
            this.valueModegroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя аргумента:";
            // 
            // argumentNameTextBox
            // 
            this.argumentNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.argumentNameTextBox.Location = new System.Drawing.Point(103, 6);
            this.argumentNameTextBox.Name = "argumentNameTextBox";
            this.argumentNameTextBox.Size = new System.Drawing.Size(309, 20);
            this.argumentNameTextBox.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(256, 152);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Сохранить";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(337, 152);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // valueModegroupBox
            // 
            this.valueModegroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.valueModegroupBox.Controls.Add(this.columnNumModeRadioButton);
            this.valueModegroupBox.Controls.Add(this.intervalStepLabel);
            this.valueModegroupBox.Controls.Add(this.intervalToLabel);
            this.valueModegroupBox.Controls.Add(this.intervalFromLabel);
            this.valueModegroupBox.Controls.Add(this.intervalStepTextBox);
            this.valueModegroupBox.Controls.Add(this.intervalToTextBox);
            this.valueModegroupBox.Controls.Add(this.intervalFromTextBox);
            this.valueModegroupBox.Controls.Add(this.intervalModeRadioButton);
            this.valueModegroupBox.Controls.Add(this.expressionModeRadioButton);
            this.valueModegroupBox.Controls.Add(this.manualModeRadioButton);
            this.valueModegroupBox.Location = new System.Drawing.Point(12, 32);
            this.valueModegroupBox.Name = "valueModegroupBox";
            this.valueModegroupBox.Size = new System.Drawing.Size(400, 114);
            this.valueModegroupBox.TabIndex = 4;
            this.valueModegroupBox.TabStop = false;
            this.valueModegroupBox.Text = "Значение";
            // 
            // columnNumModeRadioButton
            // 
            this.columnNumModeRadioButton.AutoSize = true;
            this.columnNumModeRadioButton.Location = new System.Drawing.Point(6, 88);
            this.columnNumModeRadioButton.Name = "columnNumModeRadioButton";
            this.columnNumModeRadioButton.Size = new System.Drawing.Size(104, 17);
            this.columnNumModeRadioButton.TabIndex = 9;
            this.columnNumModeRadioButton.TabStop = true;
            this.columnNumModeRadioButton.Text = "Номер колонки";
            this.columnNumModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // intervalStepLabel
            // 
            this.intervalStepLabel.AutoSize = true;
            this.intervalStepLabel.Location = new System.Drawing.Point(279, 67);
            this.intervalStepLabel.Name = "intervalStepLabel";
            this.intervalStepLabel.Size = new System.Drawing.Size(29, 13);
            this.intervalStepLabel.TabIndex = 8;
            this.intervalStepLabel.Text = "шаг:";
            // 
            // intervalToLabel
            // 
            this.intervalToLabel.AutoSize = true;
            this.intervalToLabel.Location = new System.Drawing.Point(181, 67);
            this.intervalToLabel.Name = "intervalToLabel";
            this.intervalToLabel.Size = new System.Drawing.Size(22, 13);
            this.intervalToLabel.TabIndex = 7;
            this.intervalToLabel.Text = "до:";
            // 
            // intervalFromLabel
            // 
            this.intervalFromLabel.AutoSize = true;
            this.intervalFromLabel.Location = new System.Drawing.Point(88, 67);
            this.intervalFromLabel.Name = "intervalFromLabel";
            this.intervalFromLabel.Size = new System.Drawing.Size(21, 13);
            this.intervalFromLabel.TabIndex = 6;
            this.intervalFromLabel.Text = "от:";
            // 
            // intervalStepTextBox
            // 
            this.intervalStepTextBox.Location = new System.Drawing.Point(314, 64);
            this.intervalStepTextBox.Name = "intervalStepTextBox";
            this.intervalStepTextBox.Size = new System.Drawing.Size(73, 20);
            this.intervalStepTextBox.TabIndex = 5;
            // 
            // intervalToTextBox
            // 
            this.intervalToTextBox.Location = new System.Drawing.Point(209, 64);
            this.intervalToTextBox.Name = "intervalToTextBox";
            this.intervalToTextBox.Size = new System.Drawing.Size(64, 20);
            this.intervalToTextBox.TabIndex = 4;
            // 
            // intervalFromTextBox
            // 
            this.intervalFromTextBox.Location = new System.Drawing.Point(115, 64);
            this.intervalFromTextBox.Name = "intervalFromTextBox";
            this.intervalFromTextBox.Size = new System.Drawing.Size(60, 20);
            this.intervalFromTextBox.TabIndex = 3;
            // 
            // intervalModeRadioButton
            // 
            this.intervalModeRadioButton.AutoSize = true;
            this.intervalModeRadioButton.Enabled = false;
            this.intervalModeRadioButton.Location = new System.Drawing.Point(6, 65);
            this.intervalModeRadioButton.Name = "intervalModeRadioButton";
            this.intervalModeRadioButton.Size = new System.Drawing.Size(74, 17);
            this.intervalModeRadioButton.TabIndex = 2;
            this.intervalModeRadioButton.TabStop = true;
            this.intervalModeRadioButton.Text = "Интервал";
            this.intervalModeRadioButton.UseVisualStyleBackColor = true;
            this.intervalModeRadioButton.CheckedChanged += new System.EventHandler(this.intervalModeRadioButton_CheckedChanged);
            // 
            // expressionModeRadioButton
            // 
            this.expressionModeRadioButton.AutoSize = true;
            this.expressionModeRadioButton.Location = new System.Drawing.Point(6, 42);
            this.expressionModeRadioButton.Name = "expressionModeRadioButton";
            this.expressionModeRadioButton.Size = new System.Drawing.Size(84, 17);
            this.expressionModeRadioButton.TabIndex = 1;
            this.expressionModeRadioButton.TabStop = true;
            this.expressionModeRadioButton.Text = "Выражение";
            this.expressionModeRadioButton.UseVisualStyleBackColor = true;
            this.expressionModeRadioButton.CheckedChanged += new System.EventHandler(this.intervalModeRadioButton_CheckedChanged);
            // 
            // manualModeRadioButton
            // 
            this.manualModeRadioButton.AutoSize = true;
            this.manualModeRadioButton.Enabled = false;
            this.manualModeRadioButton.Location = new System.Drawing.Point(6, 19);
            this.manualModeRadioButton.Name = "manualModeRadioButton";
            this.manualModeRadioButton.Size = new System.Drawing.Size(87, 17);
            this.manualModeRadioButton.TabIndex = 0;
            this.manualModeRadioButton.TabStop = true;
            this.manualModeRadioButton.Text = "Ручной ввод";
            this.manualModeRadioButton.UseVisualStyleBackColor = true;
            this.manualModeRadioButton.CheckedChanged += new System.EventHandler(this.intervalModeRadioButton_CheckedChanged);
            // 
            // ArgumentNameEditForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(424, 181);
            this.Controls.Add(this.valueModegroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.argumentNameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ArgumentNameEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Имя аргумента";
            this.valueModegroupBox.ResumeLayout(false);
            this.valueModegroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox argumentNameTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox valueModegroupBox;
        private System.Windows.Forms.Label intervalStepLabel;
        private System.Windows.Forms.Label intervalToLabel;
        private System.Windows.Forms.Label intervalFromLabel;
        private System.Windows.Forms.TextBox intervalStepTextBox;
        private System.Windows.Forms.TextBox intervalToTextBox;
        private System.Windows.Forms.TextBox intervalFromTextBox;
        private System.Windows.Forms.RadioButton intervalModeRadioButton;
        private System.Windows.Forms.RadioButton expressionModeRadioButton;
        private System.Windows.Forms.RadioButton manualModeRadioButton;
        private System.Windows.Forms.RadioButton columnNumModeRadioButton;
    }
}