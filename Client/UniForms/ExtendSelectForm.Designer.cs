namespace COTES.ISTOK.Client
{
    partial class ExtendSelectForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addSelectedToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeSelectedToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.moveUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.docIndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.structurePanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(483, 270);
            // 
            // treeViewUnitObjects
            // 
            this.treeViewUnitObjects.LineColor = System.Drawing.Color.Black;
            this.treeViewUnitObjects.MultiSelect = true;
            this.treeViewUnitObjects.SelectedNodes = new System.Windows.Forms.TreeNode[0];
            this.treeViewUnitObjects.Size = new System.Drawing.Size(200, 270);
            // 
            // structurePanel
            // 
            this.structurePanel.Size = new System.Drawing.Size(200, 270);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSelectedToolStripButton,
            this.removeSelectedToolStripButton,
            this.moveUpToolStripButton,
            this.moveDownToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(24, 270);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addSelectedToolStripButton
            // 
            this.addSelectedToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addSelectedToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_add;
            this.addSelectedToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addSelectedToolStripButton.Name = "addSelectedToolStripButton";
            this.addSelectedToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.addSelectedToolStripButton.Text = "toolStripButton1";
            this.addSelectedToolStripButton.Click += new System.EventHandler(this.addSelectedToolStripButton_Click);
            // 
            // removeSelectedToolStripButton
            // 
            this.removeSelectedToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeSelectedToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_remove;
            this.removeSelectedToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeSelectedToolStripButton.Name = "removeSelectedToolStripButton";
            this.removeSelectedToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.removeSelectedToolStripButton.Text = "toolStripButton2";
            this.removeSelectedToolStripButton.Click += new System.EventHandler(this.removeSelectedToolStripButton_Click);
            // 
            // moveUpToolStripButton
            // 
            this.moveUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources._1uparrow;
            this.moveUpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpToolStripButton.Name = "moveUpToolStripButton";
            this.moveUpToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.moveUpToolStripButton.Text = "toolStripButton3";
            this.moveUpToolStripButton.Click += new System.EventHandler(this.moveUpToolStripButton_Click);
            // 
            // moveDownToolStripButton
            // 
            this.moveDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources._1downarrow;
            this.moveDownToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDownToolStripButton.Name = "moveDownToolStripButton";
            this.moveDownToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.moveDownToolStripButton.Text = "toolStripButton4";
            this.moveDownToolStripButton.Click += new System.EventHandler(this.moveDownToolStripButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowDrop = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.docIndexColumn,
            this.nameColumn,
            this.codeColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(24, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(255, 270);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
            this.dataGridView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
            // 
            // docIndexColumn
            // 
            this.docIndexColumn.HeaderText = "Индекс";
            this.docIndexColumn.Name = "docIndexColumn";
            this.docIndexColumn.ReadOnly = true;
            // 
            // nameColumn
            // 
            this.nameColumn.HeaderText = "Наименование";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            // 
            // codeColumn
            // 
            this.codeColumn.HeaderText = "Код";
            this.codeColumn.Name = "codeColumn";
            this.codeColumn.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 292);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(483, 32);
            this.panel1.TabIndex = 15;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(315, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Готово";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(396, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ExtendSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 324);
            this.Controls.Add(this.panel1);
            this.Name = "ExtendSelectForm";
            this.Text = "Выбор элементов";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.structurePanel.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addSelectedToolStripButton;
        private System.Windows.Forms.ToolStripButton removeSelectedToolStripButton;
        private System.Windows.Forms.ToolStripButton moveUpToolStripButton;
        private System.Windows.Forms.ToolStripButton moveDownToolStripButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn docIndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeColumn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}