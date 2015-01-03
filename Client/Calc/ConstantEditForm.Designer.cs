namespace COTES.ISTOK.Client
{
    partial class ConstantEditForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.constantDataGridView = new System.Windows.Forms.DataGridView();
            this.ConstNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.constDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConstValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.constantContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeConstantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.constsTabPage = new System.Windows.Forms.TabPage();
            this.functionsTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.functionsDataGridView = new System.Windows.Forms.DataGridView();
            this.functionNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.functionGroupColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.functionDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.formulaEditControl1 = new COTES.ISTOK.Client.FormulaEditControl();
            this.argumentsPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.constantDataGridView)).BeginInit();
            this.constantContextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.constsTabPage.SuspendLayout();
            this.functionsTabPage.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.functionsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 305);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(438, 30);
            this.panel1.TabIndex = 14;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(360, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(279, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Сохранить";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // constantDataGridView
            // 
            this.constantDataGridView.AllowUserToResizeRows = false;
            this.constantDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.constantDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.constantDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.constantDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ConstNameColumn,
            this.constDescriptionColumn,
            this.ConstValueColumn});
            this.constantDataGridView.ContextMenuStrip = this.constantContextMenuStrip;
            this.constantDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.constantDataGridView.Location = new System.Drawing.Point(3, 3);
            this.constantDataGridView.Name = "constantDataGridView";
            this.constantDataGridView.RowHeadersVisible = false;
            this.constantDataGridView.Size = new System.Drawing.Size(424, 273);
            this.constantDataGridView.TabIndex = 15;
            // 
            // ConstNameColumn
            // 
            this.ConstNameColumn.DataPropertyName = "Name";
            this.ConstNameColumn.HeaderText = "Наименование";
            this.ConstNameColumn.Name = "ConstNameColumn";
            // 
            // constDescriptionColumn
            // 
            this.constDescriptionColumn.HeaderText = "Описание";
            this.constDescriptionColumn.Name = "constDescriptionColumn";
            // 
            // ConstValueColumn
            // 
            this.ConstValueColumn.DataPropertyName = "Value";
            this.ConstValueColumn.HeaderText = "Значение";
            this.ConstValueColumn.Name = "ConstValueColumn";
            // 
            // constantContextMenuStrip
            // 
            this.constantContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeConstantToolStripMenuItem});
            this.constantContextMenuStrip.Name = "constantContextMenuStrip";
            this.constantContextMenuStrip.Size = new System.Drawing.Size(147, 26);
            // 
            // removeConstantToolStripMenuItem
            // 
            this.removeConstantToolStripMenuItem.Name = "removeConstantToolStripMenuItem";
            this.removeConstantToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeConstantToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.removeConstantToolStripMenuItem.Text = "Remove";
            this.removeConstantToolStripMenuItem.Click += new System.EventHandler(this.removeConstantToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.constsTabPage);
            this.tabControl1.Controls.Add(this.functionsTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(438, 305);
            this.tabControl1.TabIndex = 16;
            // 
            // constsTabPage
            // 
            this.constsTabPage.Controls.Add(this.constantDataGridView);
            this.constsTabPage.Location = new System.Drawing.Point(4, 22);
            this.constsTabPage.Name = "constsTabPage";
            this.constsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.constsTabPage.Size = new System.Drawing.Size(430, 279);
            this.constsTabPage.TabIndex = 0;
            this.constsTabPage.Text = "Константы";
            this.constsTabPage.UseVisualStyleBackColor = true;
            // 
            // functionsTabPage
            // 
            this.functionsTabPage.Controls.Add(this.splitContainer1);
            this.functionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.functionsTabPage.Name = "functionsTabPage";
            this.functionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.functionsTabPage.Size = new System.Drawing.Size(430, 279);
            this.functionsTabPage.TabIndex = 1;
            this.functionsTabPage.Text = "Функции";
            this.functionsTabPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.functionsDataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.formulaEditControl1);
            this.splitContainer1.Panel2.Controls.Add(this.argumentsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(424, 273);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.TabIndex = 0;
            // 
            // functionsDataGridView
            // 
            this.functionsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.functionsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.functionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.functionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.functionNameColumn,
            this.functionGroupColumn,
            this.functionDescriptionColumn});
            this.functionsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.functionsDataGridView.MultiSelect = false;
            this.functionsDataGridView.Name = "functionsDataGridView";
            this.functionsDataGridView.RowHeadersVisible = false;
            this.functionsDataGridView.Size = new System.Drawing.Size(141, 273);
            this.functionsDataGridView.TabIndex = 0;
            this.functionsDataGridView.SelectionChanged += new System.EventHandler(this.functionsDataGridView_SelectionChanged);
            // 
            // functionNameColumn
            // 
            this.functionNameColumn.HeaderText = "Имя";
            this.functionNameColumn.Name = "functionNameColumn";
            // 
            // functionGroupColumn
            // 
            this.functionGroupColumn.HeaderText = "Группа";
            this.functionGroupColumn.Name = "functionGroupColumn";
            // 
            // functionDescriptionColumn
            // 
            this.functionDescriptionColumn.HeaderText = "Описание";
            this.functionDescriptionColumn.Name = "functionDescriptionColumn";
            // 
            // formulaEditControl1
            // 
            this.formulaEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formulaEditControl1.Formula = null;
            this.formulaEditControl1.Location = new System.Drawing.Point(0, 65);
            this.formulaEditControl1.Name = "formulaEditControl1";
            this.formulaEditControl1.Size = new System.Drawing.Size(279, 208);
            this.formulaEditControl1.TabIndex = 1;
            this.formulaEditControl1.UnitProvider = null;
            // 
            // argumentsPanel
            // 
            this.argumentsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.argumentsPanel.Location = new System.Drawing.Point(0, 0);
            this.argumentsPanel.Name = "argumentsPanel";
            this.argumentsPanel.Size = new System.Drawing.Size(279, 65);
            this.argumentsPanel.TabIndex = 0;
            this.argumentsPanel.Visible = false;
            // 
            // ConstantEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 357);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "ConstantEditForm";
            this.Text = "Редактирование констант";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.constantDataGridView)).EndInit();
            this.constantContextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.constsTabPage.ResumeLayout(false);
            this.functionsTabPage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.functionsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.DataGridView constantDataGridView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage constsTabPage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConstNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn constDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConstValueColumn;
        private System.Windows.Forms.ContextMenuStrip constantContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeConstantToolStripMenuItem;
        private System.Windows.Forms.TabPage functionsTabPage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView functionsDataGridView;
        private FormulaEditControl formulaEditControl1;
        private System.Windows.Forms.Panel argumentsPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn functionNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn functionGroupColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn functionDescriptionColumn;
    }
}