namespace COTES.ISTOK.Client
{
    partial class WordImportForm
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
            this.cbxTables = new System.Windows.Forms.ComboBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addColumnInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formulaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.customInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColumnInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.btnChangeName = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chbxAllColumns = new System.Windows.Forms.CheckBox();
            this.btnDeleteTable = new System.Windows.Forms.Button();
            this.txtPostfix = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddPostfix = new System.Windows.Forms.Button();
            this.chbxEmptyPostfix = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtAddText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAddBefore = new System.Windows.Forms.Button();
            this.btnAddAfter = new System.Windows.Forms.Button();
            this.btnClearNewLine = new System.Windows.Forms.Button();
            this.btnIndex = new System.Windows.Forms.Button();
            this.txtFormula = new System.Windows.Forms.TextBox();
            this.chbxIndexChangeFormula = new System.Windows.Forms.CheckBox();
            this.chbxIndexAllTables = new System.Windows.Forms.CheckBox();
            this.btnFormula = new System.Windows.Forms.Button();
            this.btnSaveFormula = new System.Windows.Forms.Button();
            this.btnCheckDoubles = new System.Windows.Forms.Button();
            this.btnCheckAllFormulas = new System.Windows.Forms.Button();
            this.btnCheckFormula = new System.Windows.Forms.Button();
            this.lbxReport = new System.Windows.Forms.ListBox();
            this.cmsReport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ignoreErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearErrorStopList = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCommaPointReplace = new System.Windows.Forms.Button();
            this.btnReplaceMacro = new System.Windows.Forms.Button();
            this.btnAddEOL = new System.Windows.Forms.Button();
            this.btnReplaceBrackets = new System.Windows.Forms.Button();
            this.btnReplaceIf01 = new System.Windows.Forms.Button();
            this.btnReplaceCaret = new System.Windows.Forms.Button();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnAllInOne = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chbxAllRows = new System.Windows.Forms.CheckBox();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.cmsReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxTables
            // 
            this.cbxTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxTables.FormattingEnabled = true;
            this.cbxTables.Location = new System.Drawing.Point(0, 137);
            this.cbxTables.Name = "cbxTables";
            this.cbxTables.Size = new System.Drawing.Size(802, 21);
            this.cbxTables.TabIndex = 4;
            this.cbxTables.SelectedIndexChanged += new System.EventHandler(this.cbxTables_SelectedIndexChanged);
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ContextMenuStrip = this.contextMenuStrip1;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(802, 131);
            this.dgv.TabIndex = 3;
            this.dgv.SelectionChanged += new System.EventHandler(this.dgv_SelectionChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addColumnInfoToolStripMenuItem,
            this.deleteColumnInfoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addRowToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 76);
            // 
            // addColumnInfoToolStripMenuItem
            // 
            this.addColumnInfoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.indexToolStripMenuItem,
            this.nameToolStripMenuItem,
            this.codeToolStripMenuItem,
            this.unitToolStripMenuItem,
            this.typeToolStripMenuItem,
            this.formulaToolStripMenuItem,
            this.sourceToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripTextBox1,
            this.customInfoToolStripMenuItem,
            this.toolStripSeparator2,
            this.clearInfoToolStripMenuItem});
            this.addColumnInfoToolStripMenuItem.Name = "addColumnInfoToolStripMenuItem";
            this.addColumnInfoToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.addColumnInfoToolStripMenuItem.Text = "Add column info";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.indexToolStripMenuItem.Text = "Index";
            this.indexToolStripMenuItem.Click += new System.EventHandler(this.indexToolStripMenuItem_Click);
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.nameToolStripMenuItem.Text = "Name";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.nameToolStripMenuItem_Click);
            // 
            // codeToolStripMenuItem
            // 
            this.codeToolStripMenuItem.Name = "codeToolStripMenuItem";
            this.codeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.codeToolStripMenuItem.Text = "Code";
            this.codeToolStripMenuItem.Click += new System.EventHandler(this.codeToolStripMenuItem_Click);
            // 
            // unitToolStripMenuItem
            // 
            this.unitToolStripMenuItem.Name = "unitToolStripMenuItem";
            this.unitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.unitToolStripMenuItem.Text = "Unit";
            this.unitToolStripMenuItem.Click += new System.EventHandler(this.unitToolStripMenuItem_Click);
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.typeToolStripMenuItem.Text = "Type";
            this.typeToolStripMenuItem.Click += new System.EventHandler(this.typeToolStripMenuItem_Click);
            // 
            // formulaToolStripMenuItem
            // 
            this.formulaToolStripMenuItem.Name = "formulaToolStripMenuItem";
            this.formulaToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.formulaToolStripMenuItem.Text = "Formula";
            this.formulaToolStripMenuItem.Click += new System.EventHandler(this.formulaToolStripMenuItem_Click);
            // 
            // sourceToolStripMenuItem
            // 
            this.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem";
            this.sourceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.sourceToolStripMenuItem.Text = "Source";
            this.sourceToolStripMenuItem.Click += new System.EventHandler(this.sourceToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // customInfoToolStripMenuItem
            // 
            this.customInfoToolStripMenuItem.Name = "customInfoToolStripMenuItem";
            this.customInfoToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.customInfoToolStripMenuItem.Text = "Custom info";
            this.customInfoToolStripMenuItem.Click += new System.EventHandler(this.customInfoToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // clearInfoToolStripMenuItem
            // 
            this.clearInfoToolStripMenuItem.Name = "clearInfoToolStripMenuItem";
            this.clearInfoToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.clearInfoToolStripMenuItem.Text = "Clear info";
            this.clearInfoToolStripMenuItem.Click += new System.EventHandler(this.clearInfoToolStripMenuItem_Click);
            // 
            // deleteColumnInfoToolStripMenuItem
            // 
            this.deleteColumnInfoToolStripMenuItem.Name = "deleteColumnInfoToolStripMenuItem";
            this.deleteColumnInfoToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.deleteColumnInfoToolStripMenuItem.Text = "Delete column info";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 6);
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            this.addRowToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.addRowToolStripMenuItem.Text = "Add row";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(634, 370);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(715, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtTableName
            // 
            this.txtTableName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTableName.Location = new System.Drawing.Point(53, 164);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(100, 20);
            this.txtTableName.TabIndex = 7;
            // 
            // btnChangeName
            // 
            this.btnChangeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeName.Location = new System.Drawing.Point(159, 162);
            this.btnChangeName.Name = "btnChangeName";
            this.btnChangeName.Size = new System.Drawing.Size(75, 23);
            this.btnChangeName.TabIndex = 8;
            this.btnChangeName.Text = "Change";
            this.btnChangeName.UseVisualStyleBackColor = true;
            this.btnChangeName.Click += new System.EventHandler(this.btnChangeName_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Table:";
            // 
            // chbxAllColumns
            // 
            this.chbxAllColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbxAllColumns.AutoSize = true;
            this.chbxAllColumns.Checked = true;
            this.chbxAllColumns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxAllColumns.Location = new System.Drawing.Point(13, 238);
            this.chbxAllColumns.Name = "chbxAllColumns";
            this.chbxAllColumns.Size = new System.Drawing.Size(121, 17);
            this.chbxAllColumns.TabIndex = 10;
            this.chbxAllColumns.Text = "Rename all columns";
            this.chbxAllColumns.UseVisualStyleBackColor = true;
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteTable.Location = new System.Drawing.Point(240, 162);
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteTable.TabIndex = 11;
            this.btnDeleteTable.Text = "Delete";
            this.btnDeleteTable.UseVisualStyleBackColor = true;
            this.btnDeleteTable.Click += new System.EventHandler(this.btnDeleteTable_Click);
            // 
            // txtPostfix
            // 
            this.txtPostfix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPostfix.Location = new System.Drawing.Point(53, 187);
            this.txtPostfix.Name = "txtPostfix";
            this.txtPostfix.Size = new System.Drawing.Size(100, 20);
            this.txtPostfix.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Postfix:";
            // 
            // btnAddPostfix
            // 
            this.btnAddPostfix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPostfix.Location = new System.Drawing.Point(159, 185);
            this.btnAddPostfix.Name = "btnAddPostfix";
            this.btnAddPostfix.Size = new System.Drawing.Size(75, 23);
            this.btnAddPostfix.TabIndex = 14;
            this.btnAddPostfix.Text = "Add";
            this.btnAddPostfix.UseVisualStyleBackColor = true;
            this.btnAddPostfix.Click += new System.EventHandler(this.btnAddPostfix_Click);
            // 
            // chbxEmptyPostfix
            // 
            this.chbxEmptyPostfix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbxEmptyPostfix.AutoSize = true;
            this.chbxEmptyPostfix.Location = new System.Drawing.Point(240, 189);
            this.chbxEmptyPostfix.Name = "chbxEmptyPostfix";
            this.chbxEmptyPostfix.Size = new System.Drawing.Size(77, 17);
            this.chbxEmptyPostfix.TabIndex = 15;
            this.chbxEmptyPostfix.Text = "Empty only";
            this.chbxEmptyPostfix.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(321, 162);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(31, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "S";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(358, 162);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(31, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "L";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtAddText
            // 
            this.txtAddText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddText.Location = new System.Drawing.Point(53, 212);
            this.txtAddText.Name = "txtAddText";
            this.txtAddText.Size = new System.Drawing.Size(100, 20);
            this.txtAddText.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "+Text:";
            // 
            // btnAddBefore
            // 
            this.btnAddBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddBefore.Location = new System.Drawing.Point(159, 210);
            this.btnAddBefore.Name = "btnAddBefore";
            this.btnAddBefore.Size = new System.Drawing.Size(75, 23);
            this.btnAddBefore.TabIndex = 20;
            this.btnAddBefore.Text = "Before";
            this.btnAddBefore.UseVisualStyleBackColor = true;
            this.btnAddBefore.Click += new System.EventHandler(this.btnAddBefore_Click);
            // 
            // btnAddAfter
            // 
            this.btnAddAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddAfter.Location = new System.Drawing.Point(240, 210);
            this.btnAddAfter.Name = "btnAddAfter";
            this.btnAddAfter.Size = new System.Drawing.Size(75, 23);
            this.btnAddAfter.TabIndex = 21;
            this.btnAddAfter.Text = "After";
            this.btnAddAfter.UseVisualStyleBackColor = true;
            this.btnAddAfter.Click += new System.EventHandler(this.btnAddAfter_Click);
            // 
            // btnClearNewLine
            // 
            this.btnClearNewLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearNewLine.Location = new System.Drawing.Point(321, 210);
            this.btnClearNewLine.Name = "btnClearNewLine";
            this.btnClearNewLine.Size = new System.Drawing.Size(68, 23);
            this.btnClearNewLine.TabIndex = 22;
            this.btnClearNewLine.Text = "Clear \\n\\r";
            this.btnClearNewLine.UseVisualStyleBackColor = true;
            this.btnClearNewLine.Click += new System.EventHandler(this.btnClearNewLine_Click);
            // 
            // btnIndex
            // 
            this.btnIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIndex.Location = new System.Drawing.Point(406, 162);
            this.btnIndex.Name = "btnIndex";
            this.btnIndex.Size = new System.Drawing.Size(53, 23);
            this.btnIndex.TabIndex = 23;
            this.btnIndex.Text = "Index";
            this.btnIndex.UseVisualStyleBackColor = true;
            this.btnIndex.Click += new System.EventHandler(this.btnIndex_Click);
            // 
            // txtFormula
            // 
            this.txtFormula.AcceptsReturn = true;
            this.txtFormula.AcceptsTab = true;
            this.txtFormula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFormula.Location = new System.Drawing.Point(12, 261);
            this.txtFormula.Multiline = true;
            this.txtFormula.Name = "txtFormula";
            this.txtFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFormula.Size = new System.Drawing.Size(448, 103);
            this.txtFormula.TabIndex = 24;
            this.txtFormula.WordWrap = false;
            // 
            // chbxIndexChangeFormula
            // 
            this.chbxIndexChangeFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbxIndexChangeFormula.AutoSize = true;
            this.chbxIndexChangeFormula.Checked = true;
            this.chbxIndexChangeFormula.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxIndexChangeFormula.Location = new System.Drawing.Point(650, 214);
            this.chbxIndexChangeFormula.Name = "chbxIndexChangeFormula";
            this.chbxIndexChangeFormula.Size = new System.Drawing.Size(100, 17);
            this.chbxIndexChangeFormula.TabIndex = 25;
            this.chbxIndexChangeFormula.Text = "Change formula";
            this.chbxIndexChangeFormula.UseVisualStyleBackColor = true;
            // 
            // chbxIndexAllTables
            // 
            this.chbxIndexAllTables.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbxIndexAllTables.AutoSize = true;
            this.chbxIndexAllTables.Checked = true;
            this.chbxIndexAllTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxIndexAllTables.Location = new System.Drawing.Point(650, 166);
            this.chbxIndexAllTables.Name = "chbxIndexAllTables";
            this.chbxIndexAllTables.Size = new System.Drawing.Size(68, 17);
            this.chbxIndexAllTables.TabIndex = 26;
            this.chbxIndexAllTables.Text = "All tables";
            this.chbxIndexAllTables.UseVisualStyleBackColor = true;
            // 
            // btnFormula
            // 
            this.btnFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFormula.Location = new System.Drawing.Point(406, 185);
            this.btnFormula.Name = "btnFormula";
            this.btnFormula.Size = new System.Drawing.Size(53, 23);
            this.btnFormula.TabIndex = 27;
            this.btnFormula.Text = "Formula";
            this.btnFormula.UseVisualStyleBackColor = true;
            this.btnFormula.Click += new System.EventHandler(this.btnFormula_Click);
            // 
            // btnSaveFormula
            // 
            this.btnSaveFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFormula.Location = new System.Drawing.Point(738, 341);
            this.btnSaveFormula.Name = "btnSaveFormula";
            this.btnSaveFormula.Size = new System.Drawing.Size(52, 23);
            this.btnSaveFormula.TabIndex = 28;
            this.btnSaveFormula.Text = "Save";
            this.btnSaveFormula.UseVisualStyleBackColor = true;
            this.btnSaveFormula.Click += new System.EventHandler(this.btnSaveFormula_Click);
            // 
            // btnCheckDoubles
            // 
            this.btnCheckDoubles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCheckDoubles.Location = new System.Drawing.Point(406, 210);
            this.btnCheckDoubles.Name = "btnCheckDoubles";
            this.btnCheckDoubles.Size = new System.Drawing.Size(53, 23);
            this.btnCheckDoubles.TabIndex = 29;
            this.btnCheckDoubles.Text = "Ch DBL";
            this.btnCheckDoubles.UseVisualStyleBackColor = true;
            this.btnCheckDoubles.Click += new System.EventHandler(this.btnCheckDoubles_Click);
            // 
            // btnCheckAllFormulas
            // 
            this.btnCheckAllFormulas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckAllFormulas.Location = new System.Drawing.Point(738, 312);
            this.btnCheckAllFormulas.Name = "btnCheckAllFormulas";
            this.btnCheckAllFormulas.Size = new System.Drawing.Size(52, 23);
            this.btnCheckAllFormulas.TabIndex = 30;
            this.btnCheckAllFormulas.Text = "ChkA";
            this.btnCheckAllFormulas.UseVisualStyleBackColor = true;
            this.btnCheckAllFormulas.Click += new System.EventHandler(this.btnCheckAllFormulas_Click);
            // 
            // btnCheckFormula
            // 
            this.btnCheckFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckFormula.Location = new System.Drawing.Point(738, 283);
            this.btnCheckFormula.Name = "btnCheckFormula";
            this.btnCheckFormula.Size = new System.Drawing.Size(52, 23);
            this.btnCheckFormula.TabIndex = 32;
            this.btnCheckFormula.Text = "Chk";
            this.btnCheckFormula.UseVisualStyleBackColor = true;
            this.btnCheckFormula.Click += new System.EventHandler(this.btnCheckFormula_Click);
            // 
            // lbxReport
            // 
            this.lbxReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxReport.ContextMenuStrip = this.cmsReport;
            this.lbxReport.FormattingEnabled = true;
            this.lbxReport.HorizontalScrollbar = true;
            this.lbxReport.Location = new System.Drawing.Point(466, 261);
            this.lbxReport.Name = "lbxReport";
            this.lbxReport.Size = new System.Drawing.Size(266, 95);
            this.lbxReport.TabIndex = 33;
            this.lbxReport.DoubleClick += new System.EventHandler(this.lbxReport_DoubleClick);
            // 
            // cmsReport
            // 
            this.cmsReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreErrorToolStripMenuItem,
            this.ignoreIndexToolStripMenuItem,
            this.toolStripMenuItem2,
            this.copyToolStripMenuItem});
            this.cmsReport.Name = "cmsReport";
            this.cmsReport.Size = new System.Drawing.Size(153, 98);
            // 
            // ignoreErrorToolStripMenuItem
            // 
            this.ignoreErrorToolStripMenuItem.Name = "ignoreErrorToolStripMenuItem";
            this.ignoreErrorToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.ignoreErrorToolStripMenuItem.Text = "Ignore Error";
            this.ignoreErrorToolStripMenuItem.Click += new System.EventHandler(this.ignoreErrorToolStripMenuItem_Click);
            // 
            // ignoreIndexToolStripMenuItem
            // 
            this.ignoreIndexToolStripMenuItem.Name = "ignoreIndexToolStripMenuItem";
            this.ignoreIndexToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.ignoreIndexToolStripMenuItem.Text = "Ignore Index";
            this.ignoreIndexToolStripMenuItem.Click += new System.EventHandler(this.ignoreIndexToolStripMenuItem_Click);
            // 
            // btnClearErrorStopList
            // 
            this.btnClearErrorStopList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearErrorStopList.Location = new System.Drawing.Point(738, 254);
            this.btnClearErrorStopList.Name = "btnClearErrorStopList";
            this.btnClearErrorStopList.Size = new System.Drawing.Size(52, 23);
            this.btnClearErrorStopList.TabIndex = 34;
            this.btnClearErrorStopList.Text = "ClrEr";
            this.btnClearErrorStopList.UseVisualStyleBackColor = true;
            this.btnClearErrorStopList.Click += new System.EventHandler(this.btnClearErrorStopList_Click);
            // 
            // btnCommaPointReplace
            // 
            this.btnCommaPointReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCommaPointReplace.Location = new System.Drawing.Point(465, 162);
            this.btnCommaPointReplace.Name = "btnCommaPointReplace";
            this.btnCommaPointReplace.Size = new System.Drawing.Size(53, 23);
            this.btnCommaPointReplace.TabIndex = 36;
            this.btnCommaPointReplace.Text = "ComPnt";
            this.btnCommaPointReplace.UseVisualStyleBackColor = true;
            this.btnCommaPointReplace.Click += new System.EventHandler(this.btnCommaPointReplace_Click);
            // 
            // btnReplaceMacro
            // 
            this.btnReplaceMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplaceMacro.Location = new System.Drawing.Point(465, 185);
            this.btnReplaceMacro.Name = "btnReplaceMacro";
            this.btnReplaceMacro.Size = new System.Drawing.Size(53, 23);
            this.btnReplaceMacro.TabIndex = 37;
            this.btnReplaceMacro.Text = "Macro";
            this.btnReplaceMacro.UseVisualStyleBackColor = true;
            this.btnReplaceMacro.Click += new System.EventHandler(this.btnReplaceMacro_Click);
            // 
            // btnAddEOL
            // 
            this.btnAddEOL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddEOL.Location = new System.Drawing.Point(465, 210);
            this.btnAddEOL.Name = "btnAddEOL";
            this.btnAddEOL.Size = new System.Drawing.Size(53, 23);
            this.btnAddEOL.TabIndex = 38;
            this.btnAddEOL.Text = "Add ;";
            this.btnAddEOL.UseVisualStyleBackColor = true;
            this.btnAddEOL.Click += new System.EventHandler(this.btnAddEOL_Click);
            // 
            // btnReplaceBrackets
            // 
            this.btnReplaceBrackets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplaceBrackets.Location = new System.Drawing.Point(524, 162);
            this.btnReplaceBrackets.Name = "btnReplaceBrackets";
            this.btnReplaceBrackets.Size = new System.Drawing.Size(53, 23);
            this.btnReplaceBrackets.TabIndex = 39;
            this.btnReplaceBrackets.Text = "Ch [ ]";
            this.btnReplaceBrackets.UseVisualStyleBackColor = true;
            this.btnReplaceBrackets.Click += new System.EventHandler(this.btnReplaceBrackets_Click);
            // 
            // btnReplaceIf01
            // 
            this.btnReplaceIf01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplaceIf01.Location = new System.Drawing.Point(525, 185);
            this.btnReplaceIf01.Name = "btnReplaceIf01";
            this.btnReplaceIf01.Size = new System.Drawing.Size(53, 23);
            this.btnReplaceIf01.TabIndex = 40;
            this.btnReplaceIf01.Text = "if()0;1;";
            this.btnReplaceIf01.UseVisualStyleBackColor = true;
            this.btnReplaceIf01.Click += new System.EventHandler(this.btnReplaceIf01_Click);
            // 
            // btnReplaceCaret
            // 
            this.btnReplaceCaret.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplaceCaret.Location = new System.Drawing.Point(525, 210);
            this.btnReplaceCaret.Name = "btnReplaceCaret";
            this.btnReplaceCaret.Size = new System.Drawing.Size(53, 23);
            this.btnReplaceCaret.TabIndex = 41;
            this.btnReplaceCaret.Text = "\\^";
            this.btnReplaceCaret.UseVisualStyleBackColor = true;
            this.btnReplaceCaret.Click += new System.EventHandler(this.btnReplaceCaret_Click);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddColumn.Location = new System.Drawing.Point(321, 185);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(68, 23);
            this.btnAddColumn.TabIndex = 42;
            this.btnAddColumn.Text = "+Column";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // btnAllInOne
            // 
            this.btnAllInOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAllInOne.Location = new System.Drawing.Point(584, 210);
            this.btnAllInOne.Name = "btnAllInOne";
            this.btnAllInOne.Size = new System.Drawing.Size(53, 23);
            this.btnAllInOne.TabIndex = 43;
            this.btnAllInOne.Text = "All in";
            this.btnAllInOne.UseVisualStyleBackColor = true;
            this.btnAllInOne.Click += new System.EventHandler(this.btnAllInOne_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // chbxAllRows
            // 
            this.chbxAllRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbxAllRows.AutoSize = true;
            this.chbxAllRows.Checked = true;
            this.chbxAllRows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxAllRows.Location = new System.Drawing.Point(650, 189);
            this.chbxAllRows.Name = "chbxAllRows";
            this.chbxAllRows.Size = new System.Drawing.Size(62, 17);
            this.chbxAllRows.TabIndex = 44;
            this.chbxAllRows.Text = "All rows";
            this.chbxAllRows.UseVisualStyleBackColor = true;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // WordImportForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 405);
            this.Controls.Add(this.chbxAllRows);
            this.Controls.Add(this.btnAllInOne);
            this.Controls.Add(this.btnAddColumn);
            this.Controls.Add(this.btnReplaceCaret);
            this.Controls.Add(this.btnReplaceIf01);
            this.Controls.Add(this.btnReplaceBrackets);
            this.Controls.Add(this.btnAddEOL);
            this.Controls.Add(this.btnReplaceMacro);
            this.Controls.Add(this.btnCommaPointReplace);
            this.Controls.Add(this.btnClearErrorStopList);
            this.Controls.Add(this.lbxReport);
            this.Controls.Add(this.btnCheckFormula);
            this.Controls.Add(this.btnCheckAllFormulas);
            this.Controls.Add(this.btnCheckDoubles);
            this.Controls.Add(this.btnSaveFormula);
            this.Controls.Add(this.btnFormula);
            this.Controls.Add(this.chbxIndexAllTables);
            this.Controls.Add(this.chbxIndexChangeFormula);
            this.Controls.Add(this.txtFormula);
            this.Controls.Add(this.btnIndex);
            this.Controls.Add(this.btnClearNewLine);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAddAfter);
            this.Controls.Add(this.btnAddBefore);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtAddText);
            this.Controls.Add(this.chbxEmptyPostfix);
            this.Controls.Add(this.btnAddPostfix);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPostfix);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.cbxTables);
            this.Controls.Add(this.btnDeleteTable);
            this.Controls.Add(this.btnChangeName);
            this.Controls.Add(this.chbxAllColumns);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTableName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WordImportForm";
            this.Text = "Настройка импортирования";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.cmsReport.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTables;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addColumnInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem customInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem clearInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteColumnInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Button btnChangeName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chbxAllColumns;
        private System.Windows.Forms.Button btnDeleteTable;
        private System.Windows.Forms.TextBox txtPostfix;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddPostfix;
        private System.Windows.Forms.CheckBox chbxEmptyPostfix;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtAddText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAddBefore;
        private System.Windows.Forms.Button btnAddAfter;
        private System.Windows.Forms.Button btnClearNewLine;
        private System.Windows.Forms.Button btnIndex;
        private System.Windows.Forms.ToolStripMenuItem formulaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceToolStripMenuItem;
        private System.Windows.Forms.TextBox txtFormula;
        private System.Windows.Forms.CheckBox chbxIndexChangeFormula;
        private System.Windows.Forms.CheckBox chbxIndexAllTables;
        private System.Windows.Forms.Button btnFormula;
        private System.Windows.Forms.Button btnSaveFormula;
        private System.Windows.Forms.Button btnCheckDoubles;
        private System.Windows.Forms.Button btnCheckAllFormulas;
        private System.Windows.Forms.Button btnCheckFormula;
        private System.Windows.Forms.ListBox lbxReport;
        private System.Windows.Forms.Button btnClearErrorStopList;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip cmsReport;
        private System.Windows.Forms.ToolStripMenuItem ignoreErrorToolStripMenuItem;
        private System.Windows.Forms.Button btnCommaPointReplace;
        private System.Windows.Forms.Button btnReplaceMacro;
        private System.Windows.Forms.Button btnAddEOL;
        private System.Windows.Forms.Button btnReplaceBrackets;
        private System.Windows.Forms.ToolStripMenuItem ignoreIndexToolStripMenuItem;
        private System.Windows.Forms.Button btnReplaceIf01;
        private System.Windows.Forms.Button btnReplaceCaret;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.Button btnAllInOne;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox chbxAllRows;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}