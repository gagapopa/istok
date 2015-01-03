namespace COTES.ISTOK.Client
{
    partial class NormFuncUnitControl
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
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tvDimensions = new System.Windows.Forms.TreeView();
            this.mnuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.addBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pgProperties = new System.Windows.Forms.PropertyGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.graph = new ZedGraph.ZedGraphControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.revisionToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.pgCalc = new System.Windows.Forms.PropertyGrid();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.mnuRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.showReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.mnuTree.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.mnuRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(484, 341);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tvDimensions);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pgProperties);
            this.splitContainer3.Size = new System.Drawing.Size(260, 341);
            this.splitContainer3.SplitterDistance = 207;
            this.splitContainer3.TabIndex = 0;
            // 
            // tvDimensions
            // 
            this.tvDimensions.ContextMenuStrip = this.mnuTree;
            this.tvDimensions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDimensions.HideSelection = false;
            this.tvDimensions.Location = new System.Drawing.Point(0, 0);
            this.tvDimensions.Name = "tvDimensions";
            this.tvDimensions.Size = new System.Drawing.Size(260, 207);
            this.tvDimensions.TabIndex = 0;
            this.tvDimensions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDimensions_AfterSelect);
            this.tvDimensions.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvDimensions_BeforeSelect);
            // 
            // mnuTree
            // 
            this.mnuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDimensionToolStripMenuItem,
            this.deleteDimensionToolStripMenuItem,
            this.toolStripMenuItem5,
            this.addBranchToolStripMenuItem,
            this.removeBranchToolStripMenuItem,
            this.toolStripMenuItem6,
            this.updateToolStripMenuItem});
            this.mnuTree.Name = "mnuTree";
            this.mnuTree.Size = new System.Drawing.Size(192, 126);
            this.mnuTree.Opening += new System.ComponentModel.CancelEventHandler(this.mnuTree_Opening);
            // 
            // addDimensionToolStripMenuItem
            // 
            this.addDimensionToolStripMenuItem.Name = "addDimensionToolStripMenuItem";
            this.addDimensionToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addDimensionToolStripMenuItem.Text = "Добавить измерение";
            this.addDimensionToolStripMenuItem.Click += new System.EventHandler(this.addDimensionToolStripMenuItem_Click);
            // 
            // deleteDimensionToolStripMenuItem
            // 
            this.deleteDimensionToolStripMenuItem.Name = "deleteDimensionToolStripMenuItem";
            this.deleteDimensionToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.deleteDimensionToolStripMenuItem.Text = "Удалить измерение";
            this.deleteDimensionToolStripMenuItem.Click += new System.EventHandler(this.deleteDimensionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(188, 6);
            // 
            // addBranchToolStripMenuItem
            // 
            this.addBranchToolStripMenuItem.Name = "addBranchToolStripMenuItem";
            this.addBranchToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addBranchToolStripMenuItem.Text = "Добавить ветку";
            this.addBranchToolStripMenuItem.Click += new System.EventHandler(this.addBranchToolStripMenuItem_Click);
            // 
            // removeBranchToolStripMenuItem
            // 
            this.removeBranchToolStripMenuItem.Name = "removeBranchToolStripMenuItem";
            this.removeBranchToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.removeBranchToolStripMenuItem.Text = "Удалить ветку";
            this.removeBranchToolStripMenuItem.Click += new System.EventHandler(this.removeBranchToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(188, 6);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.updateToolStripMenuItem.Text = "Обновить";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // pgProperties
            // 
            this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProperties.HelpVisible = false;
            this.pgProperties.Location = new System.Drawing.Point(0, 0);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(260, 130);
            this.pgProperties.TabIndex = 0;
            this.pgProperties.ToolbarVisible = false;
            this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.graph);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pgCalc);
            this.splitContainer2.Panel2.Controls.Add(this.dgv);
            this.splitContainer2.Size = new System.Drawing.Size(220, 341);
            this.splitContainer2.SplitterDistance = 207;
            this.splitContainer2.TabIndex = 0;
            // 
            // graph
            // 
            this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph.Location = new System.Drawing.Point(0, 25);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0;
            this.graph.ScrollMaxX = 0;
            this.graph.ScrollMaxY = 0;
            this.graph.ScrollMaxY2 = 0;
            this.graph.ScrollMinX = 0;
            this.graph.ScrollMinY = 0;
            this.graph.ScrollMinY2 = 0;
            this.graph.Size = new System.Drawing.Size(220, 182);
            this.graph.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.revisionToolStripComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(220, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // revisionToolStripComboBox
            // 
            this.revisionToolStripComboBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.revisionToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.revisionToolStripComboBox.Name = "revisionToolStripComboBox";
            this.revisionToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            this.revisionToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.revisionToolStripComboBox_SelectedIndexChanged);
            this.revisionToolStripComboBox.DropDown += new System.EventHandler(this.revisionToolStripComboBox_DropDown);
            // 
            // pgCalc
            // 
            this.pgCalc.HelpVisible = false;
            this.pgCalc.Location = new System.Drawing.Point(55, 0);
            this.pgCalc.Name = "pgCalc";
            this.pgCalc.Size = new System.Drawing.Size(45, 130);
            this.pgCalc.TabIndex = 1;
            this.pgCalc.ToolbarVisible = false;
            this.pgCalc.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.pgCalc.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgCalc_PropertyValueChanged);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.ContextMenuStrip = this.mnuRight;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.Size = new System.Drawing.Size(49, 130);
            this.dgv.TabIndex = 0;
            this.dgv.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellValueChanged);
            this.dgv.Leave += new System.EventHandler(this.dgv_Leave);
            this.dgv.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dgv_CellParsing);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            // 
            // mnuRight
            // 
            this.mnuRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createTableToolStripMenuItem,
            this.deleteTableToolStripMenuItem,
            this.toolStripMenuItem3,
            this.addRowToolStripMenuItem,
            this.addColumnToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteRowToolStripMenuItem,
            this.deleteColumnToolStripMenuItem,
            this.toolStripMenuItem2,
            this.graphToolStripMenuItem,
            this.toolStripMenuItem4,
            this.showReferenceToolStripMenuItem});
            this.mnuRight.Name = "mnuRight";
            this.mnuRight.Size = new System.Drawing.Size(180, 204);
            this.mnuRight.Opening += new System.ComponentModel.CancelEventHandler(this.mnuRight_Opening);
            // 
            // createTableToolStripMenuItem
            // 
            this.createTableToolStripMenuItem.Name = "createTableToolStripMenuItem";
            this.createTableToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.createTableToolStripMenuItem.Text = "Создать таблицу";
            this.createTableToolStripMenuItem.Visible = false;
            // 
            // deleteTableToolStripMenuItem
            // 
            this.deleteTableToolStripMenuItem.Name = "deleteTableToolStripMenuItem";
            this.deleteTableToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.deleteTableToolStripMenuItem.Text = "Очистить таблицу";
            this.deleteTableToolStripMenuItem.Click += new System.EventHandler(this.deleteTableToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(176, 6);
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            this.addRowToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.addRowToolStripMenuItem.Text = "Добавить строку";
            this.addRowToolStripMenuItem.Visible = false;
            this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
            // 
            // addColumnToolStripMenuItem
            // 
            this.addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
            this.addColumnToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.addColumnToolStripMenuItem.Text = "Добавить столбец";
            this.addColumnToolStripMenuItem.Click += new System.EventHandler(this.addColumnToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(176, 6);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.deleteRowToolStripMenuItem.Text = "Удалить строку";
            this.deleteRowToolStripMenuItem.Visible = false;
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // deleteColumnToolStripMenuItem
            // 
            this.deleteColumnToolStripMenuItem.Name = "deleteColumnToolStripMenuItem";
            this.deleteColumnToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.deleteColumnToolStripMenuItem.Text = "Удалить столбец";
            this.deleteColumnToolStripMenuItem.Click += new System.EventHandler(this.deleteColumnToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(176, 6);
            this.toolStripMenuItem2.Visible = false;
            // 
            // graphToolStripMenuItem
            // 
            this.graphToolStripMenuItem.Name = "graphToolStripMenuItem";
            this.graphToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.graphToolStripMenuItem.Text = "Обновить график";
            this.graphToolStripMenuItem.Visible = false;
            this.graphToolStripMenuItem.Click += new System.EventHandler(this.graphToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(176, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // showReferenceToolStripMenuItem
            // 
            this.showReferenceToolStripMenuItem.Name = "showReferenceToolStripMenuItem";
            this.showReferenceToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.showReferenceToolStripMenuItem.Text = "Показать ссылки";
            this.showReferenceToolStripMenuItem.Visible = false;
            this.showReferenceToolStripMenuItem.Click += new System.EventHandler(this.showReferenceToolStripMenuItem_Click);
            // 
            // NormFuncUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NormFuncUnitControl";
            this.Size = new System.Drawing.Size(484, 341);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.mnuTree.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.mnuRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TreeView tvDimensions;
        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.ContextMenuStrip mnuRight;
        private System.Windows.Forms.ToolStripMenuItem createTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem addRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem graphToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem showReferenceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuTree;
        private System.Windows.Forms.ToolStripMenuItem addDimensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDimensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem addBranchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBranchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PropertyGrid pgProperties;
        private System.Windows.Forms.PropertyGrid pgCalc;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox revisionToolStripComboBox;
    }
}
