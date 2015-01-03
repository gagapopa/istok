namespace COTES.ISTOKDataUpdate
{
    partial class DataUpdateForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.selectConfigPathButton = new System.Windows.Forms.Button();
            this.selectConfigPathTextBox = new System.Windows.Forms.TextBox();
            this.selectConfigPathRadioButton = new System.Windows.Forms.RadioButton();
            this.btnRefreshBase = new System.Windows.Forms.Button();
            this.cbxBases = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbxHosts = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.selectDBManualyRadioButton = new System.Windows.Forms.RadioButton();
            this.useStandardConfigRadioButton = new System.Windows.Forms.RadioButton();
            this.updateDBButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 272);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(502, 134);
            this.textBox1.TabIndex = 0;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.selectConfigPathButton);
            this.panel1.Controls.Add(this.selectConfigPathTextBox);
            this.panel1.Controls.Add(this.selectConfigPathRadioButton);
            this.panel1.Controls.Add(this.btnRefreshBase);
            this.panel1.Controls.Add(this.cbxBases);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.cbxHosts);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.btnTest);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtLogin);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.selectDBManualyRadioButton);
            this.panel1.Controls.Add(this.useStandardConfigRadioButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(502, 234);
            this.panel1.TabIndex = 3;
            // 
            // selectConfigPathButton
            // 
            this.selectConfigPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectConfigPathButton.AutoSize = true;
            this.selectConfigPathButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectConfigPathButton.Location = new System.Drawing.Point(466, 47);
            this.selectConfigPathButton.Name = "selectConfigPathButton";
            this.selectConfigPathButton.Size = new System.Drawing.Size(26, 23);
            this.selectConfigPathButton.TabIndex = 36;
            this.selectConfigPathButton.Text = "...";
            this.selectConfigPathButton.UseVisualStyleBackColor = true;
            // 
            // selectConfigPathTextBox
            // 
            this.selectConfigPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectConfigPathTextBox.Location = new System.Drawing.Point(35, 49);
            this.selectConfigPathTextBox.Name = "selectConfigPathTextBox";
            this.selectConfigPathTextBox.ReadOnly = true;
            this.selectConfigPathTextBox.Size = new System.Drawing.Size(425, 20);
            this.selectConfigPathTextBox.TabIndex = 35;
            // 
            // selectConfigPathRadioButton
            // 
            this.selectConfigPathRadioButton.AutoSize = true;
            this.selectConfigPathRadioButton.Enabled = false;
            this.selectConfigPathRadioButton.Location = new System.Drawing.Point(13, 26);
            this.selectConfigPathRadioButton.Name = "selectConfigPathRadioButton";
            this.selectConfigPathRadioButton.Size = new System.Drawing.Size(107, 17);
            this.selectConfigPathRadioButton.TabIndex = 34;
            this.selectConfigPathRadioButton.TabStop = true;
            this.selectConfigPathRadioButton.Text = "SelectConfigPath";
            this.selectConfigPathRadioButton.UseVisualStyleBackColor = true;
            this.selectConfigPathRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // btnRefreshBase
            // 
            this.btnRefreshBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBase.Location = new System.Drawing.Point(417, 123);
            this.btnRefreshBase.Name = "btnRefreshBase";
            this.btnRefreshBase.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshBase.TabIndex = 33;
            this.btnRefreshBase.Text = "Обновить";
            this.btnRefreshBase.UseVisualStyleBackColor = true;
            this.btnRefreshBase.Click += new System.EventHandler(this.btnRefreshBase_Click);
            // 
            // cbxBases
            // 
            this.cbxBases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBases.FormattingEnabled = true;
            this.cbxBases.Location = new System.Drawing.Point(103, 125);
            this.cbxBases.Name = "cbxBases";
            this.cbxBases.Size = new System.Drawing.Size(308, 21);
            this.cbxBases.TabIndex = 32;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(417, 96);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 31;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbxHosts
            // 
            this.cbxHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxHosts.FormattingEnabled = true;
            this.cbxHosts.Location = new System.Drawing.Point(103, 98);
            this.cbxHosts.Name = "cbxHosts";
            this.cbxHosts.Size = new System.Drawing.Size(308, 21);
            this.cbxHosts.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(32, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Компьютер";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(418, 204);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(74, 23);
            this.btnTest.TabIndex = 27;
            this.btnTest.Text = "Проверить";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(103, 178);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(389, 20);
            this.txtPassword.TabIndex = 26;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(32, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Пароль";
            // 
            // txtLogin
            // 
            this.txtLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogin.Location = new System.Drawing.Point(103, 152);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(389, 20);
            this.txtLogin.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(32, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Логин";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(32, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "База";
            // 
            // selectDBManualyRadioButton
            // 
            this.selectDBManualyRadioButton.AutoSize = true;
            this.selectDBManualyRadioButton.Location = new System.Drawing.Point(13, 75);
            this.selectDBManualyRadioButton.Name = "selectDBManualyRadioButton";
            this.selectDBManualyRadioButton.Size = new System.Drawing.Size(116, 17);
            this.selectDBManualyRadioButton.TabIndex = 1;
            this.selectDBManualyRadioButton.Text = "Select DB Manualy";
            this.selectDBManualyRadioButton.UseVisualStyleBackColor = true;
            this.selectDBManualyRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // useStandardConfigRadioButton
            // 
            this.useStandardConfigRadioButton.AutoSize = true;
            this.useStandardConfigRadioButton.Checked = true;
            this.useStandardConfigRadioButton.Location = new System.Drawing.Point(13, 3);
            this.useStandardConfigRadioButton.Name = "useStandardConfigRadioButton";
            this.useStandardConfigRadioButton.Size = new System.Drawing.Size(139, 17);
            this.useStandardConfigRadioButton.TabIndex = 0;
            this.useStandardConfigRadioButton.TabStop = true;
            this.useStandardConfigRadioButton.Text = "Use Default Config Path";
            this.useStandardConfigRadioButton.UseVisualStyleBackColor = true;
            this.useStandardConfigRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // updateDBButton
            // 
            this.updateDBButton.Location = new System.Drawing.Point(3, 243);
            this.updateDBButton.Name = "updateDBButton";
            this.updateDBButton.Size = new System.Drawing.Size(102, 23);
            this.updateDBButton.TabIndex = 4;
            this.updateDBButton.Text = "Update";
            this.updateDBButton.UseVisualStyleBackColor = true;
            this.updateDBButton.Click += new System.EventHandler(this.updateDBButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.updateDBButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(508, 409);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // DataUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 409);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DataUpdateForm";
            this.Text = "ISTOK DB Update";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton selectDBManualyRadioButton;
        private System.Windows.Forms.RadioButton useStandardConfigRadioButton;
        private System.Windows.Forms.Button btnRefreshBase;
        private System.Windows.Forms.ComboBox cbxBases;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbxHosts;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnTest;
        public System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button updateDBButton;
        private System.Windows.Forms.Button selectConfigPathButton;
        private System.Windows.Forms.TextBox selectConfigPathTextBox;
        private System.Windows.Forms.RadioButton selectConfigPathRadioButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

