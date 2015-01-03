namespace COTES.ISTOK.Client
{
    partial class ExportImportForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.unitNodeTreeView = new COTES.ISTOK.Client.MultiSelectTreeViewControl();
            this.unitPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.valuesEndDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.valuesBeginDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.valuesEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chooseParentButton = new System.Windows.Forms.Button();
            this.importRootTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 35);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.unitNodeTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.unitPropertyGrid);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(396, 231);
            this.splitContainer1.SplitterDistance = 132;
            this.splitContainer1.TabIndex = 0;
            // 
            // unitNodeTreeView
            // 
            this.unitNodeTreeView.CheckBoxes = true;
            this.unitNodeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitNodeTreeView.HideSelection = false;
            this.unitNodeTreeView.Location = new System.Drawing.Point(0, 0);
            this.unitNodeTreeView.Name = "unitNodeTreeView";
            this.unitNodeTreeView.SelectedNodes = new System.Windows.Forms.TreeNode[0];
            this.unitNodeTreeView.Size = new System.Drawing.Size(132, 231);
            this.unitNodeTreeView.TabIndex = 0;
            this.unitNodeTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.unitNodeTreeView_AfterCheck);
            this.unitNodeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.unitNodeTreeView_AfterSelect);
            // 
            // unitPropertyGrid
            // 
            this.unitPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitPropertyGrid.Location = new System.Drawing.Point(0, 93);
            this.unitPropertyGrid.Name = "unitPropertyGrid";
            this.unitPropertyGrid.Size = new System.Drawing.Size(260, 138);
            this.unitPropertyGrid.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.valuesEnabledCheckBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(260, 93);
            this.panel2.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.valuesEndDateTimePicker);
            this.groupBox1.Controls.Add(this.valuesBeginDateTimePicker);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 76);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Значения";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "По";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "От";
            // 
            // valuesEndDateTimePicker
            // 
            this.valuesEndDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valuesEndDateTimePicker.Location = new System.Drawing.Point(71, 45);
            this.valuesEndDateTimePicker.Name = "valuesEndDateTimePicker";
            this.valuesEndDateTimePicker.Size = new System.Drawing.Size(177, 20);
            this.valuesEndDateTimePicker.TabIndex = 1;
            // 
            // valuesBeginDateTimePicker
            // 
            this.valuesBeginDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valuesBeginDateTimePicker.Location = new System.Drawing.Point(71, 19);
            this.valuesBeginDateTimePicker.Name = "valuesBeginDateTimePicker";
            this.valuesBeginDateTimePicker.Size = new System.Drawing.Size(177, 20);
            this.valuesBeginDateTimePicker.TabIndex = 0;
            // 
            // valuesEnabledCheckBox
            // 
            this.valuesEnabledCheckBox.AutoSize = true;
            this.valuesEnabledCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.valuesEnabledCheckBox.Location = new System.Drawing.Point(0, 0);
            this.valuesEnabledCheckBox.Name = "valuesEnabledCheckBox";
            this.valuesEnabledCheckBox.Size = new System.Drawing.Size(260, 17);
            this.valuesEnabledCheckBox.TabIndex = 2;
            this.valuesEnabledCheckBox.Text = "Включать значения";
            this.valuesEnabledCheckBox.UseVisualStyleBackColor = true;
            this.valuesEnabledCheckBox.CheckedChanged += new System.EventHandler(this.valuesEnabledCheckBox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 266);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 36);
            this.panel1.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(309, 6);
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
            this.okButton.Location = new System.Drawing.Point(228, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chooseParentButton);
            this.panel3.Controls.Add(this.importRootTextBox);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(396, 35);
            this.panel3.TabIndex = 14;
            // 
            // chooseParentButton
            // 
            this.chooseParentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseParentButton.AutoSize = true;
            this.chooseParentButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chooseParentButton.Location = new System.Drawing.Point(358, 4);
            this.chooseParentButton.Name = "chooseParentButton";
            this.chooseParentButton.Size = new System.Drawing.Size(26, 23);
            this.chooseParentButton.TabIndex = 2;
            this.chooseParentButton.Text = "...";
            this.chooseParentButton.UseVisualStyleBackColor = true;
            this.chooseParentButton.Click += new System.EventHandler(this.chooseParentButton_Click);
            // 
            // importRootTextBox
            // 
            this.importRootTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importRootTextBox.Location = new System.Drawing.Point(84, 6);
            this.importRootTextBox.Name = "importRootTextBox";
            this.importRootTextBox.ReadOnly = true;
            this.importRootTextBox.Size = new System.Drawing.Size(268, 20);
            this.importRootTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Вставить в:";
            // 
            // ExportImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 324);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "ExportImportForm";
            this.Text = "ExportImportForm";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid unitPropertyGrid;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private MultiSelectTreeViewControl unitNodeTreeView;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox valuesEnabledCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker valuesEndDateTimePicker;
        private System.Windows.Forms.DateTimePicker valuesBeginDateTimePicker;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button chooseParentButton;
        private System.Windows.Forms.TextBox importRootTextBox;
        private System.Windows.Forms.Label label3;
    }
}