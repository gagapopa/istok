namespace COTES.ISTOK.KeyGenerator
{
    partial class KeyGeneratorForm
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
            this.machineCodeTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.organizatonTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.registerDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.registerKeyTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.nudMaxBlockCount = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxBlockCount)).BeginInit();
            this.SuspendLayout();
            // 
            // machineCodeTextBox
            // 
            this.machineCodeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.machineCodeTextBox.Location = new System.Drawing.Point(103, 12);
            this.machineCodeTextBox.Name = "machineCodeTextBox";
            this.machineCodeTextBox.Size = new System.Drawing.Size(177, 20);
            this.machineCodeTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "machineID:";
            // 
            // organizatonTextBox
            // 
            this.organizatonTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.organizatonTextBox.Location = new System.Drawing.Point(103, 38);
            this.organizatonTextBox.Name = "organizatonTextBox";
            this.organizatonTextBox.Size = new System.Drawing.Size(177, 20);
            this.organizatonTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Organization:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Register Date:";
            // 
            // registerDateTimePicker
            // 
            this.registerDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.registerDateTimePicker.Location = new System.Drawing.Point(103, 90);
            this.registerDateTimePicker.Name = "registerDateTimePicker";
            this.registerDateTimePicker.Size = new System.Drawing.Size(177, 20);
            this.registerDateTimePicker.TabIndex = 5;
            // 
            // registerKeyTextBox
            // 
            this.registerKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.registerKeyTextBox.Location = new System.Drawing.Point(103, 116);
            this.registerKeyTextBox.Multiline = true;
            this.registerKeyTextBox.Name = "registerKeyTextBox";
            this.registerKeyTextBox.ReadOnly = true;
            this.registerKeyTextBox.Size = new System.Drawing.Size(177, 89);
            this.registerKeyTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Key";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenerateButton.Location = new System.Drawing.Point(109, 211);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(75, 23);
            this.GenerateButton.TabIndex = 8;
            this.GenerateButton.Text = "Generate";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // nudMaxBlockCount
            // 
            this.nudMaxBlockCount.Location = new System.Drawing.Point(103, 64);
            this.nudMaxBlockCount.Name = "nudMaxBlockCount";
            this.nudMaxBlockCount.Size = new System.Drawing.Size(177, 20);
            this.nudMaxBlockCount.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "MaxBlockCount:";
            // 
            // KeyGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 242);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudMaxBlockCount);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.registerKeyTextBox);
            this.Controls.Add(this.registerDateTimePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.organizatonTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.machineCodeTextBox);
            this.MinimumSize = new System.Drawing.Size(300, 230);
            this.Name = "KeyGeneratorForm";
            this.Text = "Key Generator";
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxBlockCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox machineCodeTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox organizatonTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker registerDateTimePicker;
        private System.Windows.Forms.TextBox registerKeyTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.NumericUpDown nudMaxBlockCount;
        private System.Windows.Forms.Label label5;
    }
}

