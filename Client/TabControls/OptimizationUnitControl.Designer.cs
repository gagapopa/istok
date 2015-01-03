namespace COTES.ISTOK.Client
{
    partial class OptimizationUnitControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.expressionTreeView = new System.Windows.Forms.TreeView();
            this.argumentsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addArgumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeArgumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameArgumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbLock = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbArgumentUp = new System.Windows.Forms.ToolStripButton();
            this.tsbArgumentDown = new System.Windows.Forms.ToolStripButton();
            this.formulaEditControl1 = new COTES.ISTOK.Client.FormulaEditControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.argumentsContextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.expressionTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.formulaEditControl1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(671, 376);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 10;
            // 
            // expressionTreeView
            // 
            this.expressionTreeView.ContextMenuStrip = this.argumentsContextMenuStrip;
            this.expressionTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expressionTreeView.HideSelection = false;
            this.expressionTreeView.Location = new System.Drawing.Point(0, 25);
            this.expressionTreeView.Name = "expressionTreeView";
            this.expressionTreeView.ShowPlusMinus = false;
            this.expressionTreeView.Size = new System.Drawing.Size(155, 351);
            this.expressionTreeView.TabIndex = 0;
            this.expressionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.expressionTreeView_AfterSelect);
            // 
            // argumentsContextMenuStrip
            // 
            this.argumentsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addArgumentToolStripMenuItem,
            this.removeArgumentToolStripMenuItem,
            this.renameArgumentToolStripMenuItem});
            this.argumentsContextMenuStrip.Name = "argumentsContextMenuStrip";
            this.argumentsContextMenuStrip.Size = new System.Drawing.Size(186, 70);
            this.argumentsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.argumentsContextMenuStrip_Opening);
            // 
            // addArgumentToolStripMenuItem
            // 
            this.addArgumentToolStripMenuItem.Name = "addArgumentToolStripMenuItem";
            this.addArgumentToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.addArgumentToolStripMenuItem.Text = "Добавить аргумент";
            this.addArgumentToolStripMenuItem.Click += new System.EventHandler(this.addArgumentToolStripMenuItem_Click);
            // 
            // removeArgumentToolStripMenuItem
            // 
            this.removeArgumentToolStripMenuItem.Name = "removeArgumentToolStripMenuItem";
            this.removeArgumentToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.removeArgumentToolStripMenuItem.Text = "Удалить аргумент";
            this.removeArgumentToolStripMenuItem.Click += new System.EventHandler(this.removeArgumentToolStripMenuItem_Click);
            // 
            // renameArgumentToolStripMenuItem
            // 
            this.renameArgumentToolStripMenuItem.Name = "renameArgumentToolStripMenuItem";
            this.renameArgumentToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.renameArgumentToolStripMenuItem.Text = "Изменить аргумент";
            this.renameArgumentToolStripMenuItem.Click += new System.EventHandler(this.renameArgumentToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLock,
            this.tsbSave,
            this.tsbCancel,
            this.toolStripSeparator1,
            this.tsbArgumentUp,
            this.tsbArgumentDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(155, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbLock
            // 
            this.tsbLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLock.Image = global::COTES.ISTOK.Client.Properties.Resources.edit;
            this.tsbLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLock.Name = "tsbLock";
            this.tsbLock.Size = new System.Drawing.Size(23, 22);
            this.tsbLock.Text = "Изменить";
            this.tsbLock.ToolTipText = "Редактировать";
            this.tsbLock.Click += new System.EventHandler(this.tsbLock_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::COTES.ISTOK.Client.Properties.Resources.filesave;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Сохранить";
            this.tsbSave.ToolTipText = "Сохранить";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCancel.Image = global::COTES.ISTOK.Client.Properties.Resources.cancel;
            this.tsbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancel.Name = "tsbCancel";
            this.tsbCancel.Size = new System.Drawing.Size(23, 22);
            this.tsbCancel.Text = "Отмена";
            this.tsbCancel.ToolTipText = "Отмена";
            this.tsbCancel.Click += new System.EventHandler(this.tsbCancel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbArgumentUp
            // 
            this.tsbArgumentUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbArgumentUp.Image = global::COTES.ISTOK.Client.Properties.Resources._1uparrow;
            this.tsbArgumentUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbArgumentUp.Name = "tsbArgumentUp";
            this.tsbArgumentUp.Size = new System.Drawing.Size(23, 22);
            this.tsbArgumentUp.Text = "Переместить аргумент вверх";
            this.tsbArgumentUp.Click += new System.EventHandler(this.argumentUpButton_Click);
            // 
            // tsbArgumentDown
            // 
            this.tsbArgumentDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbArgumentDown.Image = global::COTES.ISTOK.Client.Properties.Resources._1downarrow;
            this.tsbArgumentDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbArgumentDown.Name = "tsbArgumentDown";
            this.tsbArgumentDown.Size = new System.Drawing.Size(23, 22);
            this.tsbArgumentDown.Text = "Переместить аргумент вниз";
            this.tsbArgumentDown.Click += new System.EventHandler(this.argumentDownButton_Click);
            // 
            // formulaEditControl1
            // 
            this.formulaEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formulaEditControl1.Formula = null;
            this.formulaEditControl1.Location = new System.Drawing.Point(0, 53);
            this.formulaEditControl1.Name = "formulaEditControl1";
            this.formulaEditControl1.ShowEditButtons = false;
            this.formulaEditControl1.Size = new System.Drawing.Size(512, 323);
            this.formulaEditControl1.TabIndex = 8;
            this.formulaEditControl1.Typ = COTES.ISTOK.ASC.UnitTypeId.Unknown;
            this.formulaEditControl1.UnitProvider = null;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 53);
            this.panel1.TabIndex = 9;
            this.panel1.Visible = false;
            // 
            // OptimizationUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "OptimizationUnitControl";
            this.Size = new System.Drawing.Size(671, 376);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.argumentsContextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView expressionTreeView;
        private FormulaEditControl formulaEditControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip argumentsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addArgumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeArgumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameArgumentToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbLock;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbArgumentUp;
        private System.Windows.Forms.ToolStripButton tsbArgumentDown;
    }
}
