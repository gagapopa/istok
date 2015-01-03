namespace XMLTest
{
    partial class Form1
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.txtColumnName = new System.Windows.Forms.TextBox();
            this.lbxStructs = new System.Windows.Forms.ListBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.popupStructs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addStructToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteStructToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtStructName = new System.Windows.Forms.TextBox();
            this.btnChangeName = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.popupGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.getValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFormulaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFormulaToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxNulls = new System.Windows.Forms.CheckBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.btnDelColumn = new System.Windows.Forms.Button();
            this.btnAddFormulaCodToImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.popupStructs.SuspendLayout();
            this.popupGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(343, 67);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(343, 38);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(169, 12);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(249, 20);
            this.txtFilename.TabIndex = 6;
            this.txtFilename.Text = "test.xml";
            // 
            // dgv
            // 
            this.dgv.AllowUserToOrderColumns = true;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 158);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(406, 175);
            this.dgv.TabIndex = 7;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);
            this.dgv.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_EditingControlShowing);
            this.dgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgView_KeyDown);
            this.dgv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgv_MouseDown);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddColumn.Location = new System.Drawing.Point(257, 129);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(75, 23);
            this.btnAddColumn.TabIndex = 9;
            this.btnAddColumn.Text = "Add column";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // txtColumnName
            // 
            this.txtColumnName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumnName.Location = new System.Drawing.Point(169, 131);
            this.txtColumnName.Name = "txtColumnName";
            this.txtColumnName.Size = new System.Drawing.Size(82, 20);
            this.txtColumnName.TabIndex = 10;
            // 
            // lbxStructs
            // 
            this.lbxStructs.FormattingEnabled = true;
            this.lbxStructs.Location = new System.Drawing.Point(12, 12);
            this.lbxStructs.Name = "lbxStructs";
            this.lbxStructs.Size = new System.Drawing.Size(151, 82);
            this.lbxStructs.TabIndex = 12;
            this.lbxStructs.SelectedIndexChanged += new System.EventHandler(this.lbxStructs_SelectedIndexChanged);
            this.lbxStructs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbxStructs_MouseDoubleClick);
            this.lbxStructs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbxStructs_MouseDown);
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(12, 129);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(48, 23);
            this.btnCommit.TabIndex = 13;
            this.btnCommit.Text = "comit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // popupStructs
            // 
            this.popupStructs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addStructToolStripMenuItem,
            this.deleteStructToolStripMenuItem,
            this.toolStripMenuItem1,
            this.refreshToolStripMenuItem});
            this.popupStructs.Name = "popupStructs";
            this.popupStructs.Size = new System.Drawing.Size(141, 76);
            // 
            // addStructToolStripMenuItem
            // 
            this.addStructToolStripMenuItem.Name = "addStructToolStripMenuItem";
            this.addStructToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.addStructToolStripMenuItem.Text = "Add struct";
            this.addStructToolStripMenuItem.Click += new System.EventHandler(this.addStructToolStripMenuItem_Click);
            // 
            // deleteStructToolStripMenuItem
            // 
            this.deleteStructToolStripMenuItem.Name = "deleteStructToolStripMenuItem";
            this.deleteStructToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.deleteStructToolStripMenuItem.Text = "Delete struct";
            this.deleteStructToolStripMenuItem.Click += new System.EventHandler(this.deleteStructToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // txtStructName
            // 
            this.txtStructName.Location = new System.Drawing.Point(12, 100);
            this.txtStructName.Name = "txtStructName";
            this.txtStructName.Size = new System.Drawing.Size(111, 20);
            this.txtStructName.TabIndex = 16;
            // 
            // btnChangeName
            // 
            this.btnChangeName.Location = new System.Drawing.Point(129, 98);
            this.btnChangeName.Name = "btnChangeName";
            this.btnChangeName.Size = new System.Drawing.Size(34, 23);
            this.btnChangeName.TabIndex = 17;
            this.btnChangeName.Text = "/\\";
            this.btnChangeName.UseVisualStyleBackColor = true;
            this.btnChangeName.Click += new System.EventHandler(this.btnChangeName_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(66, 129);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Rename column";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // popupGrid
            // 
            this.popupGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getValueToolStripMenuItem,
            this.getValuesToolStripMenuItem,
            this.copyToAllToolStripMenuItem,
            this.setFormulaToolStripMenuItem,
            this.setFormulaToAllToolStripMenuItem});
            this.popupGrid.Name = "popupGrid";
            this.popupGrid.Size = new System.Drawing.Size(167, 114);
            // 
            // getValueToolStripMenuItem
            // 
            this.getValueToolStripMenuItem.Name = "getValueToolStripMenuItem";
            this.getValueToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.getValueToolStripMenuItem.Text = "Get Value";
            this.getValueToolStripMenuItem.Click += new System.EventHandler(this.getValueToolStripMenuItem_Click);
            // 
            // getValuesToolStripMenuItem
            // 
            this.getValuesToolStripMenuItem.Name = "getValuesToolStripMenuItem";
            this.getValuesToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.getValuesToolStripMenuItem.Text = "Get Values";
            this.getValuesToolStripMenuItem.Click += new System.EventHandler(this.getValuesToolStripMenuItem_Click);
            // 
            // copyToAllToolStripMenuItem
            // 
            this.copyToAllToolStripMenuItem.Name = "copyToAllToolStripMenuItem";
            this.copyToAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.copyToAllToolStripMenuItem.Text = "Copy to all";
            this.copyToAllToolStripMenuItem.Click += new System.EventHandler(this.copyToAllToolStripMenuItem_Click);
            // 
            // setFormulaToolStripMenuItem
            // 
            this.setFormulaToolStripMenuItem.Name = "setFormulaToolStripMenuItem";
            this.setFormulaToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.setFormulaToolStripMenuItem.Text = "Set Formula";
            this.setFormulaToolStripMenuItem.Click += new System.EventHandler(this.setFormulaToolStripMenuItem_Click);
            // 
            // setFormulaToAllToolStripMenuItem
            // 
            this.setFormulaToAllToolStripMenuItem.Name = "setFormulaToAllToolStripMenuItem";
            this.setFormulaToAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.setFormulaToAllToolStripMenuItem.Text = "Set Formula to all";
            this.setFormulaToAllToolStripMenuItem.Click += new System.EventHandler(this.setFormulaToAllToolStripMenuItem_Click);
            // 
            // cbxNulls
            // 
            this.cbxNulls.AutoSize = true;
            this.cbxNulls.Location = new System.Drawing.Point(169, 102);
            this.cbxNulls.Name = "cbxNulls";
            this.cbxNulls.Size = new System.Drawing.Size(66, 17);
            this.cbxNulls.TabIndex = 20;
            this.cbxNulls.Text = "Null only";
            this.cbxNulls.UseVisualStyleBackColor = true;
            // 
            // btnColor
            // 
            this.btnColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColor.Location = new System.Drawing.Point(343, 96);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(75, 23);
            this.btnColor.TabIndex = 21;
            this.btnColor.Text = "Color";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnDelColumn
            // 
            this.btnDelColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelColumn.Location = new System.Drawing.Point(338, 129);
            this.btnDelColumn.Name = "btnDelColumn";
            this.btnDelColumn.Size = new System.Drawing.Size(80, 23);
            this.btnDelColumn.TabIndex = 22;
            this.btnDelColumn.Text = "Del column";
            this.btnDelColumn.UseVisualStyleBackColor = true;
            this.btnDelColumn.Click += new System.EventHandler(this.btnDelColumn_Click);
            // 
            // btnAddFormulaCodToImport
            // 
            this.btnAddFormulaCodToImport.Location = new System.Drawing.Point(169, 38);
            this.btnAddFormulaCodToImport.Name = "btnAddFormulaCodToImport";
            this.btnAddFormulaCodToImport.Size = new System.Drawing.Size(139, 23);
            this.btnAddFormulaCodToImport.TabIndex = 23;
            this.btnAddFormulaCodToImport.Text = "+formulacod to import";
            this.btnAddFormulaCodToImport.UseVisualStyleBackColor = true;
            this.btnAddFormulaCodToImport.Click += new System.EventHandler(this.btnAddFormulaCodToImport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 345);
            this.Controls.Add(this.btnAddFormulaCodToImport);
            this.Controls.Add(this.btnDelColumn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnChangeName);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.cbxNulls);
            this.Controls.Add(this.txtStructName);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.lbxStructs);
            this.Controls.Add(this.txtColumnName);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.btnAddColumn);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.MinimumSize = new System.Drawing.Size(368, 270);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.popupStructs.ResumeLayout(false);
            this.popupGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.TextBox txtColumnName;
        private System.Windows.Forms.ListBox lbxStructs;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ContextMenuStrip popupStructs;
        private System.Windows.Forms.ToolStripMenuItem addStructToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteStructToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.TextBox txtStructName;
        private System.Windows.Forms.Button btnChangeName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip popupGrid;
        private System.Windows.Forms.ToolStripMenuItem getValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getValuesToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbxNulls;
        private System.Windows.Forms.ToolStripMenuItem copyToAllToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.ToolStripMenuItem setFormulaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setFormulaToAllToolStripMenuItem;
        private System.Windows.Forms.Button btnDelColumn;
        private System.Windows.Forms.Button btnAddFormulaCodToImport;
    }
}

