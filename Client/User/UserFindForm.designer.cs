namespace COTES.ISTOK.Client
{
    partial class UserFindForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserFindForm));
            this.userDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.usersTabPage = new System.Windows.Forms.TabPage();
            this.groupsTabPage = new System.Windows.Forms.TabPage();
            this.groupDataGridView = new System.Windows.Forms.DataGridView();
            this.groupIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.userDataGridView)).BeginInit();
            this.userContextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.usersTabPage.SuspendLayout();
            this.groupsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGridView)).BeginInit();
            this.groupContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // userDataGridView
            // 
            this.userDataGridView.AllowUserToAddRows = false;
            this.userDataGridView.AllowUserToDeleteRows = false;
            this.userDataGridView.AllowUserToResizeRows = false;
            this.userDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.userDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.userDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.userDataGridView.ContextMenuStrip = this.userContextMenuStrip;
            this.userDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userDataGridView.Location = new System.Drawing.Point(3, 3);
            this.userDataGridView.Name = "userDataGridView";
            this.userDataGridView.ReadOnly = true;
            this.userDataGridView.RowHeadersVisible = false;
            this.userDataGridView.Size = new System.Drawing.Size(279, 221);
            this.userDataGridView.TabIndex = 0;
            this.userDataGridView.DoubleClick += new System.EventHandler(this.editUserToolStripMenuItem_Click);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "idnum";
            this.Column1.HeaderText = "Идентификатор";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Visible = false;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.DataPropertyName = "name";
            this.Column2.HeaderText = "Наименование";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // userContextMenuStrip
            // 
            this.userContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUserToolStripMenuItem,
            this.addUserToolStripMenuItem,
            this.removeUserToolStripMenuItem});
            this.userContextMenuStrip.Name = "userContextMenuStrip";
            this.userContextMenuStrip.Size = new System.Drawing.Size(164, 92);
            this.userContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.userContextMenuStrip_Opening);
            // 
            // editUserToolStripMenuItem
            // 
            this.editUserToolStripMenuItem.Name = "editUserToolStripMenuItem";
            this.editUserToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editUserToolStripMenuItem.Text = "Редактировать...";
            this.editUserToolStripMenuItem.Click += new System.EventHandler(this.editUserToolStripMenuItem_Click);
            // 
            // addUserToolStripMenuItem
            // 
            this.addUserToolStripMenuItem.Name = "addUserToolStripMenuItem";
            this.addUserToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addUserToolStripMenuItem.Text = "Добавить...";
            this.addUserToolStripMenuItem.Click += new System.EventHandler(this.addUserToolStripMenuItem_Click);
            // 
            // removeUserToolStripMenuItem
            // 
            this.removeUserToolStripMenuItem.Name = "removeUserToolStripMenuItem";
            this.removeUserToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.removeUserToolStripMenuItem.Text = "Удалить";
            this.removeUserToolStripMenuItem.Click += new System.EventHandler(this.removeUserToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.usersTabPage);
            this.tabControl1.Controls.Add(this.groupsTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(293, 253);
            this.tabControl1.TabIndex = 1;
            // 
            // usersTabPage
            // 
            this.usersTabPage.Controls.Add(this.userDataGridView);
            this.usersTabPage.Location = new System.Drawing.Point(4, 22);
            this.usersTabPage.Name = "usersTabPage";
            this.usersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.usersTabPage.Size = new System.Drawing.Size(285, 227);
            this.usersTabPage.TabIndex = 0;
            this.usersTabPage.Text = "Пользователи";
            this.usersTabPage.UseVisualStyleBackColor = true;
            // 
            // groupsTabPage
            // 
            this.groupsTabPage.Controls.Add(this.groupDataGridView);
            this.groupsTabPage.Location = new System.Drawing.Point(4, 22);
            this.groupsTabPage.Name = "groupsTabPage";
            this.groupsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.groupsTabPage.Size = new System.Drawing.Size(285, 227);
            this.groupsTabPage.TabIndex = 1;
            this.groupsTabPage.Text = "Группы";
            this.groupsTabPage.UseVisualStyleBackColor = true;
            // 
            // groupDataGridView
            // 
            this.groupDataGridView.AllowUserToAddRows = false;
            this.groupDataGridView.AllowUserToDeleteRows = false;
            this.groupDataGridView.AllowUserToResizeRows = false;
            this.groupDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.groupDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.groupDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.groupIdColumn,
            this.groupNameColumn,
            this.groupDescriptionColumn});
            this.groupDataGridView.ContextMenuStrip = this.groupContextMenuStrip;
            this.groupDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDataGridView.Location = new System.Drawing.Point(3, 3);
            this.groupDataGridView.Name = "groupDataGridView";
            this.groupDataGridView.ReadOnly = true;
            this.groupDataGridView.RowHeadersVisible = false;
            this.groupDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.groupDataGridView.Size = new System.Drawing.Size(279, 221);
            this.groupDataGridView.TabIndex = 0;
            this.groupDataGridView.DoubleClick += new System.EventHandler(this.editGroupToolStripMenuItem_Click);
            // 
            // groupIdColumn
            // 
            this.groupIdColumn.DataPropertyName = "idnum";
            this.groupIdColumn.HeaderText = "id";
            this.groupIdColumn.Name = "groupIdColumn";
            this.groupIdColumn.ReadOnly = true;
            this.groupIdColumn.Visible = false;
            // 
            // groupNameColumn
            // 
            this.groupNameColumn.DataPropertyName = "name";
            this.groupNameColumn.HeaderText = "Наименование";
            this.groupNameColumn.Name = "groupNameColumn";
            this.groupNameColumn.ReadOnly = true;
            // 
            // groupDescriptionColumn
            // 
            this.groupDescriptionColumn.DataPropertyName = "description";
            this.groupDescriptionColumn.HeaderText = "Описание";
            this.groupDescriptionColumn.Name = "groupDescriptionColumn";
            this.groupDescriptionColumn.ReadOnly = true;
            // 
            // groupContextMenuStrip
            // 
            this.groupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editGroupToolStripMenuItem,
            this.addGroupToolStripMenuItem,
            this.removeGroupToolStripMenuItem});
            this.groupContextMenuStrip.Name = "groupContextMenuStrip";
            this.groupContextMenuStrip.Size = new System.Drawing.Size(164, 70);
            this.groupContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.groupContextMenuStrip_Opening);
            // 
            // editGroupToolStripMenuItem
            // 
            this.editGroupToolStripMenuItem.Name = "editGroupToolStripMenuItem";
            this.editGroupToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editGroupToolStripMenuItem.Text = "Редактировать...";
            this.editGroupToolStripMenuItem.Click += new System.EventHandler(this.editGroupToolStripMenuItem_Click);
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addGroupToolStripMenuItem.Text = "Добавить...";
            this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.addGroupToolStripMenuItem_Click);
            // 
            // removeGroupToolStripMenuItem
            // 
            this.removeGroupToolStripMenuItem.Name = "removeGroupToolStripMenuItem";
            this.removeGroupToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.removeGroupToolStripMenuItem.Text = "Удалить";
            this.removeGroupToolStripMenuItem.Click += new System.EventHandler(this.removeGroupToolStripMenuItem_Click);
            // 
            // UserFindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 275);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserFindForm";
            this.Text = "Список пользователей";
            this.Controls.SetChildIndex(this.tabControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.userDataGridView)).EndInit();
            this.userContextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.usersTabPage.ResumeLayout(false);
            this.groupsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGridView)).EndInit();
            this.groupContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView userDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage usersTabPage;
        private System.Windows.Forms.TabPage groupsTabPage;
        private System.Windows.Forms.DataGridView groupDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupDescriptionColumn;
        private System.Windows.Forms.ContextMenuStrip userContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUserToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip groupContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeGroupToolStripMenuItem;
    }
}