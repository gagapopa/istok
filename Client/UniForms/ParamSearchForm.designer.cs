namespace COTES.ISTOK.Client
{
    partial class ParamSearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParamSearchForm));
            this.tvParameters = new System.Windows.Forms.TreeView();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cbxWhole = new System.Windows.Forms.CheckBox();
            this.cbxReg = new System.Windows.Forms.CheckBox();
            this.txtPropertyName = new System.Windows.Forms.TextBox();
            this.txtPropertyValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.cbxMustExist = new System.Windows.Forms.CheckBox();
            this.chbxShaitan = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCopyMask = new System.Windows.Forms.TextBox();
            this.btnCopyFormula = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.txtFormula = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.chbxParamsOnly = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnEditor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvParameters
            // 
            this.tvParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvParameters.HideSelection = false;
            this.tvParameters.Location = new System.Drawing.Point(0, 0);
            this.tvParameters.Name = "tvParameters";
            this.tvParameters.ShowLines = false;
            this.tvParameters.ShowPlusMinus = false;
            this.tvParameters.ShowRootLines = false;
            this.tvParameters.Size = new System.Drawing.Size(498, 172);
            this.tvParameters.TabIndex = 0;
            this.tvParameters.DoubleClick += new System.EventHandler(this.tvParameters_DoubleClick);
            this.tvParameters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvParameters_AfterSelect);
            this.tvParameters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvParameters_MouseMove);
            this.tvParameters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvParameters_MouseDown);
            this.tvParameters.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvParameters_BeforeSelect);
            this.tvParameters.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvParameters_ItemDrag);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(85, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(85, 32);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(100, 20);
            this.txtCode.TabIndex = 2;
            this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Имя:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Код:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(301, 66);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(71, 46);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Искать";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cbxWhole
            // 
            this.cbxWhole.AutoSize = true;
            this.cbxWhole.Location = new System.Drawing.Point(9, 59);
            this.cbxWhole.Name = "cbxWhole";
            this.cbxWhole.Size = new System.Drawing.Size(72, 17);
            this.cbxWhole.TabIndex = 6;
            this.cbxWhole.Text = "Целиком";
            this.cbxWhole.UseVisualStyleBackColor = true;
            this.cbxWhole.CheckedChanged += new System.EventHandler(this.cbxWhole_CheckedChanged);
            // 
            // cbxReg
            // 
            this.cbxReg.AutoSize = true;
            this.cbxReg.Location = new System.Drawing.Point(87, 59);
            this.cbxReg.Name = "cbxReg";
            this.cbxReg.Size = new System.Drawing.Size(124, 17);
            this.cbxReg.TabIndex = 7;
            this.cbxReg.Text = "Учитывать регистр";
            this.cbxReg.UseVisualStyleBackColor = true;
            this.cbxReg.CheckedChanged += new System.EventHandler(this.cbxReg_CheckedChanged);
            // 
            // txtPropertyName
            // 
            this.txtPropertyName.Location = new System.Drawing.Point(85, 6);
            this.txtPropertyName.Name = "txtPropertyName";
            this.txtPropertyName.Size = new System.Drawing.Size(100, 20);
            this.txtPropertyName.TabIndex = 14;
            this.txtPropertyName.TextChanged += new System.EventHandler(this.txtPropertyName_TextChanged);
            // 
            // txtPropertyValue
            // 
            this.txtPropertyValue.Enabled = false;
            this.txtPropertyValue.Location = new System.Drawing.Point(85, 32);
            this.txtPropertyValue.Name = "txtPropertyValue";
            this.txtPropertyValue.Size = new System.Drawing.Size(100, 20);
            this.txtPropertyValue.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Свойство:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Значение:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpMain);
            this.tabControl1.Controls.Add(this.tpProperties);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(274, 112);
            this.tabControl1.TabIndex = 18;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.label1);
            this.tpMain.Controls.Add(this.txtName);
            this.tpMain.Controls.Add(this.txtCode);
            this.tpMain.Controls.Add(this.cbxWhole);
            this.tpMain.Controls.Add(this.cbxReg);
            this.tpMain.Controls.Add(this.label2);
            this.tpMain.Location = new System.Drawing.Point(4, 22);
            this.tpMain.Name = "tpMain";
            this.tpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tpMain.Size = new System.Drawing.Size(266, 86);
            this.tpMain.TabIndex = 0;
            this.tpMain.Text = "Основной";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // tpProperties
            // 
            this.tpProperties.Controls.Add(this.cbxMustExist);
            this.tpProperties.Controls.Add(this.label4);
            this.tpProperties.Controls.Add(this.label3);
            this.tpProperties.Controls.Add(this.txtPropertyValue);
            this.tpProperties.Controls.Add(this.txtPropertyName);
            this.tpProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(266, 86);
            this.tpProperties.TabIndex = 1;
            this.tpProperties.Text = "По свойствам";
            this.tpProperties.UseVisualStyleBackColor = true;
            // 
            // cbxMustExist
            // 
            this.cbxMustExist.AutoSize = true;
            this.cbxMustExist.Location = new System.Drawing.Point(9, 59);
            this.cbxMustExist.Name = "cbxMustExist";
            this.cbxMustExist.Size = new System.Drawing.Size(181, 17);
            this.cbxMustExist.TabIndex = 18;
            this.cbxMustExist.Text = "Обязательное существование";
            this.cbxMustExist.UseVisualStyleBackColor = true;
            // 
            // chbxShaitan
            // 
            this.chbxShaitan.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbxShaitan.AutoSize = true;
            this.chbxShaitan.Location = new System.Drawing.Point(301, 28);
            this.chbxShaitan.Name = "chbxShaitan";
            this.chbxShaitan.Size = new System.Drawing.Size(55, 23);
            this.chbxShaitan.TabIndex = 19;
            this.chbxShaitan.Text = "Шайтан";
            this.chbxShaitan.UseVisualStyleBackColor = true;
            this.chbxShaitan.CheckedChanged += new System.EventHandler(this.chbxShaitan_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 118);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvParameters);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.txtCopyMask);
            this.splitContainer1.Panel2.Controls.Add(this.btnCopyFormula);
            this.splitContainer1.Panel2.Controls.Add(this.btnReplaceAll);
            this.splitContainer1.Panel2.Controls.Add(this.txtFormula);
            this.splitContainer1.Panel2.Controls.Add(this.btnFind);
            this.splitContainer1.Panel2.Controls.Add(this.btnReplace);
            this.splitContainer1.Panel2.Controls.Add(this.chbxParamsOnly);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.txtReplace);
            this.splitContainer1.Panel2.Controls.Add(this.txtFind);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.btnApply);
            this.splitContainer1.Panel2.Controls.Add(this.btnEditor);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Size = new System.Drawing.Size(498, 350);
            this.splitContainer1.SplitterDistance = 172;
            this.splitContainer1.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(411, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "По маске:";
            // 
            // txtCopyMask
            // 
            this.txtCopyMask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCopyMask.Location = new System.Drawing.Point(411, 109);
            this.txtCopyMask.Name = "txtCopyMask";
            this.txtCopyMask.Size = new System.Drawing.Size(75, 20);
            this.txtCopyMask.TabIndex = 15;
            this.txtCopyMask.Text = "К-\\d+";
            // 
            // btnCopyFormula
            // 
            this.btnCopyFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyFormula.Location = new System.Drawing.Point(411, 67);
            this.btnCopyFormula.Name = "btnCopyFormula";
            this.btnCopyFormula.Size = new System.Drawing.Size(75, 23);
            this.btnCopyFormula.TabIndex = 14;
            this.btnCopyFormula.Text = "Расплодить";
            this.btnCopyFormula.UseVisualStyleBackColor = true;
            this.btnCopyFormula.Click += new System.EventHandler(this.btnCopyFormula_Click);
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(14, 125);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(86, 23);
            this.btnReplaceAll.TabIndex = 13;
            this.btnReplaceAll.Text = "Зам. все";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // txtFormula
            // 
            this.txtFormula.AcceptsReturn = true;
            this.txtFormula.AcceptsTab = true;
            this.txtFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFormula.HideSelection = false;
            this.txtFormula.Location = new System.Drawing.Point(106, 5);
            this.txtFormula.Multiline = true;
            this.txtFormula.Name = "txtFormula";
            this.txtFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFormula.Size = new System.Drawing.Size(299, 166);
            this.txtFormula.TabIndex = 12;
            this.txtFormula.WordWrap = false;
            this.txtFormula.Leave += new System.EventHandler(this.txtFormula_Leave);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(14, 67);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(86, 23);
            this.btnFind.TabIndex = 11;
            this.btnFind.Text = "Поиск";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(14, 96);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(86, 23);
            this.btnReplace.TabIndex = 9;
            this.btnReplace.Text = "Замена";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // chbxParamsOnly
            // 
            this.chbxParamsOnly.AutoSize = true;
            this.chbxParamsOnly.Enabled = false;
            this.chbxParamsOnly.Location = new System.Drawing.Point(13, 154);
            this.chbxParamsOnly.Name = "chbxParamsOnly";
            this.chbxParamsOnly.Size = new System.Drawing.Size(96, 17);
            this.chbxParamsOnly.TabIndex = 8;
            this.chbxParamsOnly.Text = "В параметрах";
            this.chbxParamsOnly.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Замена:";
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(65, 41);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(35, 20);
            this.txtReplace.TabIndex = 6;
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(65, 15);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(35, 20);
            this.txtFind.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Поиск:";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(411, 32);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Применить";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnEditor
            // 
            this.btnEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditor.Location = new System.Drawing.Point(411, 3);
            this.btnEditor.Name = "btnEditor";
            this.btnEditor.Size = new System.Drawing.Size(75, 23);
            this.btnEditor.TabIndex = 2;
            this.btnEditor.Text = "Редактор";
            this.btnEditor.UseVisualStyleBackColor = true;
            this.btnEditor.Click += new System.EventHandler(this.btnEditor_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Формула:";
            // 
            // ParamSearchForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 493);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.chbxShaitan);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 270);
            this.Name = "ParamSearchForm";
            this.Text = "Поиск";
            this.Load += new System.EventHandler(this.param_search_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.param_search_FormClosing);
            this.Controls.SetChildIndex(this.btnSearch, 0);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.Controls.SetChildIndex(this.chbxShaitan, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.tabControl1.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.tpMain.PerformLayout();
            this.tpProperties.ResumeLayout(false);
            this.tpProperties.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvParameters;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox cbxWhole;
        private System.Windows.Forms.CheckBox cbxReg;
        private System.Windows.Forms.TextBox txtPropertyName;
        private System.Windows.Forms.TextBox txtPropertyValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.TabPage tpProperties;
        private System.Windows.Forms.CheckBox cbxMustExist;
        private System.Windows.Forms.CheckBox chbxShaitan;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnEditor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.CheckBox chbxParamsOnly;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox txtFormula;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCopyMask;
        private System.Windows.Forms.Button btnCopyFormula;
    }
}