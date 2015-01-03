namespace COTES.ISTOK.Client
{
    partial class UserEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserEditForm));
            this.label2 = new System.Windows.Forms.Label();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.adminCheckBox = new System.Windows.Forms.CheckBox();
            this.privEditDataGridView = new System.Windows.Forms.DataGridView();
            this.typeIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.canReadColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.canWriteColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.canExecuteColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.userPrivTabPage = new System.Windows.Forms.TabPage();
            this.groupPrivTabPage = new System.Windows.Forms.TabPage();
            this.removeAllGroupsButton = new System.Windows.Forms.Button();
            this.addAllGroupsButton = new System.Windows.Forms.Button();
            this.groupComboBox = new System.Windows.Forms.ComboBox();
            this.removeGroupButton = new System.Windows.Forms.Button();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.groupPrivDataGridView = new System.Windows.Forms.DataGridView();
            this.groupIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupNameTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.canGroupReadCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.canGroupWriteGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.canGroupExecuteGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.structureHideCheckBox = new System.Windows.Forms.CheckBox();
            this.canLockCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fullNameTextBox = new System.Windows.Forms.TextBox();
            this.positionTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupAddRemovePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.privEditDataGridView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.userPrivTabPage.SuspendLayout();
            this.groupPrivTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupPrivDataGridView)).BeginInit();
            this.groupAddRemovePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Пароль";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loginTextBox
            // 
            this.loginTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loginTextBox.Location = new System.Drawing.Point(118, 9);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(449, 20);
            this.loginTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.Location = new System.Drawing.Point(118, 35);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(449, 20);
            this.passwordTextBox.TabIndex = 3;
            this.passwordTextBox.Validated += new System.EventHandler(this.textBox2_Validated);
            this.passwordTextBox.Enter += new System.EventHandler(this.textBox2_Enter);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(12, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Разрешения";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // adminCheckBox
            // 
            this.adminCheckBox.AutoSize = true;
            this.adminCheckBox.Location = new System.Drawing.Point(118, 114);
            this.adminCheckBox.Name = "adminCheckBox";
            this.adminCheckBox.Size = new System.Drawing.Size(105, 17);
            this.adminCheckBox.TabIndex = 9;
            this.adminCheckBox.Text = "Администратор";
            this.adminCheckBox.UseVisualStyleBackColor = true;
            // 
            // privEditDataGridView
            // 
            this.privEditDataGridView.AllowUserToAddRows = false;
            this.privEditDataGridView.AllowUserToDeleteRows = false;
            this.privEditDataGridView.AllowUserToResizeColumns = false;
            this.privEditDataGridView.AllowUserToResizeRows = false;
            this.privEditDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.privEditDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.privEditDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.privEditDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.privEditDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.typeIDColumn,
            this.unitTypeColumn,
            this.canReadColumn,
            this.canWriteColumn,
            this.canExecuteColumn});
            this.privEditDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.privEditDataGridView.Location = new System.Drawing.Point(3, 3);
            this.privEditDataGridView.MultiSelect = false;
            this.privEditDataGridView.Name = "privEditDataGridView";
            this.privEditDataGridView.RowHeadersVisible = false;
            this.privEditDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.privEditDataGridView.Size = new System.Drawing.Size(541, 255);
            this.privEditDataGridView.TabIndex = 0;
            this.privEditDataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.privEditDataGridView_MouseDoubleClick);
            // 
            // typeIDColumn
            // 
            this.typeIDColumn.DataPropertyName = "typeID";
            this.typeIDColumn.HeaderText = "ID";
            this.typeIDColumn.Name = "typeIDColumn";
            this.typeIDColumn.Visible = false;
            // 
            // unitTypeColumn
            // 
            this.unitTypeColumn.DataPropertyName = "type";
            this.unitTypeColumn.HeaderText = "Тип";
            this.unitTypeColumn.Name = "unitTypeColumn";
            this.unitTypeColumn.ReadOnly = true;
            // 
            // canReadColumn
            // 
            this.canReadColumn.DataPropertyName = "canRead";
            this.canReadColumn.HeaderText = "Читать";
            this.canReadColumn.Name = "canReadColumn";
            // 
            // canWriteColumn
            // 
            this.canWriteColumn.DataPropertyName = "canWrite";
            this.canWriteColumn.HeaderText = "Изменять";
            this.canWriteColumn.Name = "canWriteColumn";
            // 
            // canExecuteColumn
            // 
            this.canExecuteColumn.DataPropertyName = "canExecute";
            this.canExecuteColumn.HeaderText = "Выполнять";
            this.canExecuteColumn.Name = "canExecuteColumn";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.userPrivTabPage);
            this.tabControl1.Controls.Add(this.groupPrivTabPage);
            this.tabControl1.Location = new System.Drawing.Point(12, 137);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(555, 287);
            this.tabControl1.TabIndex = 12;
            // 
            // userPrivTabPage
            // 
            this.userPrivTabPage.Controls.Add(this.privEditDataGridView);
            this.userPrivTabPage.Location = new System.Drawing.Point(4, 22);
            this.userPrivTabPage.Name = "userPrivTabPage";
            this.userPrivTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.userPrivTabPage.Size = new System.Drawing.Size(547, 261);
            this.userPrivTabPage.TabIndex = 0;
            this.userPrivTabPage.Text = "по оборудованию";
            this.userPrivTabPage.UseVisualStyleBackColor = true;
            // 
            // groupPrivTabPage
            // 
            this.groupPrivTabPage.Controls.Add(this.groupPrivDataGridView);
            this.groupPrivTabPage.Controls.Add(this.groupAddRemovePanel);
            this.groupPrivTabPage.Location = new System.Drawing.Point(4, 22);
            this.groupPrivTabPage.Name = "groupPrivTabPage";
            this.groupPrivTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.groupPrivTabPage.Size = new System.Drawing.Size(547, 261);
            this.groupPrivTabPage.TabIndex = 1;
            this.groupPrivTabPage.Text = "по группам";
            this.groupPrivTabPage.UseVisualStyleBackColor = true;
            // 
            // removeAllGroupsButton
            // 
            this.removeAllGroupsButton.Location = new System.Drawing.Point(434, 3);
            this.removeAllGroupsButton.Name = "removeAllGroupsButton";
            this.removeAllGroupsButton.Size = new System.Drawing.Size(80, 23);
            this.removeAllGroupsButton.TabIndex = 6;
            this.removeAllGroupsButton.Text = "Удалить Все";
            this.removeAllGroupsButton.UseVisualStyleBackColor = true;
            this.removeAllGroupsButton.Click += new System.EventHandler(this.removeAllGroupsButton_Click);
            // 
            // addAllGroupsButton
            // 
            this.addAllGroupsButton.Location = new System.Drawing.Point(340, 3);
            this.addAllGroupsButton.Name = "addAllGroupsButton";
            this.addAllGroupsButton.Size = new System.Drawing.Size(88, 23);
            this.addAllGroupsButton.TabIndex = 5;
            this.addAllGroupsButton.Text = "Добавить Все";
            this.addAllGroupsButton.UseVisualStyleBackColor = true;
            this.addAllGroupsButton.Click += new System.EventHandler(this.addAllGroupsButton_Click);
            // 
            // groupComboBox
            // 
            this.groupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupComboBox.FormattingEnabled = true;
            this.groupComboBox.Location = new System.Drawing.Point(3, 3);
            this.groupComboBox.Name = "groupComboBox";
            this.groupComboBox.Size = new System.Drawing.Size(169, 21);
            this.groupComboBox.TabIndex = 4;
            // 
            // removeGroupButton
            // 
            this.removeGroupButton.Location = new System.Drawing.Point(259, 3);
            this.removeGroupButton.Name = "removeGroupButton";
            this.removeGroupButton.Size = new System.Drawing.Size(75, 23);
            this.removeGroupButton.TabIndex = 3;
            this.removeGroupButton.Text = "Удалить";
            this.removeGroupButton.UseVisualStyleBackColor = true;
            this.removeGroupButton.Click += new System.EventHandler(this.removeGroupButton_Click);
            // 
            // addGroupButton
            // 
            this.addGroupButton.Location = new System.Drawing.Point(178, 3);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(75, 23);
            this.addGroupButton.TabIndex = 2;
            this.addGroupButton.Text = "Добавить";
            this.addGroupButton.UseVisualStyleBackColor = true;
            this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
            // 
            // groupPrivDataGridView
            // 
            this.groupPrivDataGridView.AllowUserToAddRows = false;
            this.groupPrivDataGridView.AllowUserToDeleteRows = false;
            this.groupPrivDataGridView.AllowUserToResizeColumns = false;
            this.groupPrivDataGridView.AllowUserToResizeRows = false;
            this.groupPrivDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.groupPrivDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.groupPrivDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.groupPrivDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupPrivDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.groupIDColumn,
            this.groupNameTextBoxColumn,
            this.canGroupReadCheckBoxColumn,
            this.canGroupWriteGridViewCheckBoxColumn,
            this.canGroupExecuteGridViewCheckBoxColumn});
            this.groupPrivDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupPrivDataGridView.Location = new System.Drawing.Point(3, 3);
            this.groupPrivDataGridView.MultiSelect = false;
            this.groupPrivDataGridView.Name = "groupPrivDataGridView";
            this.groupPrivDataGridView.RowHeadersVisible = false;
            this.groupPrivDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.groupPrivDataGridView.Size = new System.Drawing.Size(541, 224);
            this.groupPrivDataGridView.TabIndex = 1;
            this.groupPrivDataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.privEditDataGridView_MouseDoubleClick);
            // 
            // groupIDColumn
            // 
            this.groupIDColumn.DataPropertyName = "groupID";
            this.groupIDColumn.HeaderText = "ID";
            this.groupIDColumn.Name = "groupIDColumn";
            this.groupIDColumn.Visible = false;
            // 
            // groupNameTextBoxColumn
            // 
            this.groupNameTextBoxColumn.DataPropertyName = "group";
            this.groupNameTextBoxColumn.HeaderText = "Группа";
            this.groupNameTextBoxColumn.Name = "groupNameTextBoxColumn";
            this.groupNameTextBoxColumn.ReadOnly = true;
            // 
            // canGroupReadCheckBoxColumn
            // 
            this.canGroupReadCheckBoxColumn.DataPropertyName = "canRead";
            this.canGroupReadCheckBoxColumn.HeaderText = "Читать";
            this.canGroupReadCheckBoxColumn.Name = "canGroupReadCheckBoxColumn";
            this.canGroupReadCheckBoxColumn.ReadOnly = true;
            // 
            // canGroupWriteGridViewCheckBoxColumn
            // 
            this.canGroupWriteGridViewCheckBoxColumn.DataPropertyName = "canWrite";
            this.canGroupWriteGridViewCheckBoxColumn.HeaderText = "Изменять";
            this.canGroupWriteGridViewCheckBoxColumn.Name = "canGroupWriteGridViewCheckBoxColumn";
            // 
            // canGroupExecuteGridViewCheckBoxColumn
            // 
            this.canGroupExecuteGridViewCheckBoxColumn.DataPropertyName = "canExecute";
            this.canGroupExecuteGridViewCheckBoxColumn.HeaderText = "Выполнять";
            this.canGroupExecuteGridViewCheckBoxColumn.Name = "canGroupExecuteGridViewCheckBoxColumn";
            // 
            // structureHideCheckBox
            // 
            this.structureHideCheckBox.AutoSize = true;
            this.structureHideCheckBox.Location = new System.Drawing.Point(230, 115);
            this.structureHideCheckBox.Name = "structureHideCheckBox";
            this.structureHideCheckBox.Size = new System.Drawing.Size(119, 17);
            this.structureHideCheckBox.TabIndex = 10;
            this.structureHideCheckBox.Text = "Скрыть структуры";
            this.structureHideCheckBox.UseVisualStyleBackColor = true;
            // 
            // canLockCheckBox
            // 
            this.canLockCheckBox.AutoSize = true;
            this.canLockCheckBox.Location = new System.Drawing.Point(355, 115);
            this.canLockCheckBox.Name = "canLockCheckBox";
            this.canLockCheckBox.Size = new System.Drawing.Size(146, 17);
            this.canLockCheckBox.TabIndex = 11;
            this.canLockCheckBox.Text = "Фиксировать значения";
            this.canLockCheckBox.UseVisualStyleBackColor = true;
            this.canLockCheckBox.Visible = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(411, 430);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(492, 430);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // fullNameTextBox
            // 
            this.fullNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fullNameTextBox.Location = new System.Drawing.Point(118, 61);
            this.fullNameTextBox.Name = "fullNameTextBox";
            this.fullNameTextBox.Size = new System.Drawing.Size(449, 20);
            this.fullNameTextBox.TabIndex = 5;
            // 
            // positionTextBox
            // 
            this.positionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.positionTextBox.Location = new System.Drawing.Point(118, 88);
            this.positionTextBox.Name = "positionTextBox";
            this.positionTextBox.Size = new System.Drawing.Size(449, 20);
            this.positionTextBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Полное имя";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Должность";
            // 
            // groupAddRemovePanel
            // 
            this.groupAddRemovePanel.Controls.Add(this.groupComboBox);
            this.groupAddRemovePanel.Controls.Add(this.removeAllGroupsButton);
            this.groupAddRemovePanel.Controls.Add(this.addGroupButton);
            this.groupAddRemovePanel.Controls.Add(this.addAllGroupsButton);
            this.groupAddRemovePanel.Controls.Add(this.removeGroupButton);
            this.groupAddRemovePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupAddRemovePanel.Location = new System.Drawing.Point(3, 227);
            this.groupAddRemovePanel.Name = "groupAddRemovePanel";
            this.groupAddRemovePanel.Size = new System.Drawing.Size(541, 31);
            this.groupAddRemovePanel.TabIndex = 7;
            // 
            // UserEditForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 465);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.positionTextBox);
            this.Controls.Add(this.fullNameTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.canLockCheckBox);
            this.Controls.Add(this.structureHideCheckBox);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.adminCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(565, 350);
            this.Name = "UserEditForm";
            this.Text = "Редактирование пользователя";
            ((System.ComponentModel.ISupportInitialize)(this.privEditDataGridView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.userPrivTabPage.ResumeLayout(false);
            this.groupPrivTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupPrivDataGridView)).EndInit();
            this.groupAddRemovePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox adminCheckBox;
        private System.Windows.Forms.DataGridView privEditDataGridView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage userPrivTabPage;
        private System.Windows.Forms.TabPage groupPrivTabPage;
        private System.Windows.Forms.DataGridView groupPrivDataGridView;
        private System.Windows.Forms.ComboBox groupComboBox;
        private System.Windows.Forms.Button removeGroupButton;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.CheckBox structureHideCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitTypeColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canReadColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canWriteColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canExecuteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupNameTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canGroupReadCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canGroupWriteGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn canGroupExecuteGridViewCheckBoxColumn;
        private System.Windows.Forms.Button removeAllGroupsButton;
        private System.Windows.Forms.Button addAllGroupsButton;
        private System.Windows.Forms.CheckBox canLockCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox fullNameTextBox;
        private System.Windows.Forms.TextBox positionTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel groupAddRemovePanel;
    }
}
