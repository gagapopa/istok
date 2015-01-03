namespace COTES.ISTOK.Assignment.UI
{
    partial class CreateDBForm
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
            this.dataBaseSuffixTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataBaseDataPathTextBox = new System.Windows.Forms.TextBox();
            this.dataBaseLogFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.password = new System.Windows.Forms.Label();
            this.login = new System.Windows.Forms.Label();
            this.userPasswordTextBox = new System.Windows.Forms.TextBox();
            this.userLoginTextBox = new System.Windows.Forms.TextBox();
            this.useInternalSecurityCheckBox = new System.Windows.Forms.CheckBox();
            this.createDBButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cbxHosts = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataBaseSuffixTextBox
            // 
            this.dataBaseSuffixTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataBaseSuffixTextBox.Location = new System.Drawing.Point(133, 38);
            this.dataBaseSuffixTextBox.Name = "dataBaseSuffixTextBox";
            this.dataBaseSuffixTextBox.Size = new System.Drawing.Size(189, 20);
            this.dataBaseSuffixTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Суфикс базы:";
            // 
            // dataBaseDataPathTextBox
            // 
            this.dataBaseDataPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataBaseDataPathTextBox.Location = new System.Drawing.Point(133, 64);
            this.dataBaseDataPathTextBox.Name = "dataBaseDataPathTextBox";
            this.dataBaseDataPathTextBox.Size = new System.Drawing.Size(189, 20);
            this.dataBaseDataPathTextBox.TabIndex = 2;
            // 
            // dataBaseLogFilePathTextBox
            // 
            this.dataBaseLogFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataBaseLogFilePathTextBox.Location = new System.Drawing.Point(133, 90);
            this.dataBaseLogFilePathTextBox.Name = "dataBaseLogFilePathTextBox";
            this.dataBaseLogFilePathTextBox.Size = new System.Drawing.Size(189, 20);
            this.dataBaseLogFilePathTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Путь файла данных:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Путь файла журнала:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.password);
            this.groupBox1.Controls.Add(this.login);
            this.groupBox1.Controls.Add(this.userPasswordTextBox);
            this.groupBox1.Controls.Add(this.userLoginTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 80);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Авторизация SQL";
            // 
            // password
            // 
            this.password.AutoSize = true;
            this.password.Location = new System.Drawing.Point(6, 48);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(48, 13);
            this.password.TabIndex = 3;
            this.password.Text = "Пароль:";
            // 
            // login
            // 
            this.login.AutoSize = true;
            this.login.Location = new System.Drawing.Point(6, 22);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(106, 13);
            this.login.TabIndex = 2;
            this.login.Text = "Имя пользователя:";
            // 
            // userPasswordTextBox
            // 
            this.userPasswordTextBox.Location = new System.Drawing.Point(121, 45);
            this.userPasswordTextBox.Name = "userPasswordTextBox";
            this.userPasswordTextBox.Size = new System.Drawing.Size(189, 20);
            this.userPasswordTextBox.TabIndex = 1;
            this.userPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // userLoginTextBox
            // 
            this.userLoginTextBox.Location = new System.Drawing.Point(121, 19);
            this.userLoginTextBox.Name = "userLoginTextBox";
            this.userLoginTextBox.Size = new System.Drawing.Size(189, 20);
            this.userLoginTextBox.TabIndex = 0;
            // 
            // useInternalSecurityCheckBox
            // 
            this.useInternalSecurityCheckBox.AutoSize = true;
            this.useInternalSecurityCheckBox.Location = new System.Drawing.Point(12, 116);
            this.useInternalSecurityCheckBox.Name = "useInternalSecurityCheckBox";
            this.useInternalSecurityCheckBox.Size = new System.Drawing.Size(110, 17);
            this.useInternalSecurityCheckBox.TabIndex = 7;
            this.useInternalSecurityCheckBox.Text = "Авторизация ОС";
            this.useInternalSecurityCheckBox.UseVisualStyleBackColor = true;
            // 
            // createDBButton
            // 
            this.createDBButton.Location = new System.Drawing.Point(12, 225);
            this.createDBButton.Name = "createDBButton";
            this.createDBButton.Size = new System.Drawing.Size(75, 23);
            this.createDBButton.TabIndex = 8;
            this.createDBButton.Text = "Создать";
            this.createDBButton.UseVisualStyleBackColor = true;
            this.createDBButton.Click += new System.EventHandler(this.createDBButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(93, 225);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Закрыть";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Имя сервера:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(328, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Обновить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbxHosts
            // 
            this.cbxHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxHosts.FormattingEnabled = true;
            this.cbxHosts.Location = new System.Drawing.Point(133, 12);
            this.cbxHosts.Name = "cbxHosts";
            this.cbxHosts.Size = new System.Drawing.Size(189, 21);
            this.cbxHosts.TabIndex = 18;
            // 
            // CreateDBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 255);
            this.Controls.Add(this.cbxHosts);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.createDBButton);
            this.Controls.Add(this.useInternalSecurityCheckBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataBaseLogFilePathTextBox);
            this.Controls.Add(this.dataBaseDataPathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataBaseSuffixTextBox);
            this.Name = "CreateDBForm";
            this.Text = "Создать базу проекта";
            this.Load += new System.EventHandler(this.CreateDBForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dataBaseSuffixTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dataBaseDataPathTextBox;
        private System.Windows.Forms.TextBox dataBaseLogFilePathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox userPasswordTextBox;
        private System.Windows.Forms.TextBox userLoginTextBox;
        private System.Windows.Forms.CheckBox useInternalSecurityCheckBox;
        private System.Windows.Forms.Label password;
        private System.Windows.Forms.Label login;
        private System.Windows.Forms.Button createDBButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cbxHosts;
    }
}