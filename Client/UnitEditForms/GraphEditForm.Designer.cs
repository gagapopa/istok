namespace COTES.ISTOK.Client
{
    partial class GraphEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphEditForm));
            this.dgv = new System.Windows.Forms.DataGridView();
            this.clmParam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmLine = new System.Windows.Forms.DataGridViewImageColumn();
            this.cmsParameters = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.cmsParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnDel);
            this.splitContainer1.Panel1.Controls.Add(this.btnAdd);
            this.splitContainer1.Panel1.Controls.Add(this.btnDown);
            this.splitContainer1.Panel1.Controls.Add(this.btnUp);
            this.splitContainer1.Panel1.Controls.Add(this.dgv);
            this.splitContainer1.Size = new System.Drawing.Size(525, 321);
            this.splitContainer1.SplitterDistance = 275;
            // 
            // pgNode
            // 
            this.pgNode.Size = new System.Drawing.Size(246, 142);
            // 
            // pgParameter
            // 
            this.pgParameter.Size = new System.Drawing.Size(246, 162);
            this.pgParameter.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgParameter_PropertyValueChanged);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmParam,
            this.clmLine});
            this.dgv.ContextMenuStrip = this.cmsParameters;
            this.dgv.Location = new System.Drawing.Point(0, 30);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(274, 291);
            this.dgv.TabIndex = 0;
            this.dgv.SelectionChanged += new System.EventHandler(this.dgv_SelectionChanged);
            // 
            // clmParam
            // 
            this.clmParam.HeaderText = "Параметр";
            this.clmParam.Name = "clmParam";
            this.clmParam.ReadOnly = true;
            // 
            // clmLine
            // 
            this.clmLine.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmLine.FillWeight = 25F;
            this.clmLine.HeaderText = "Линия";
            this.clmLine.Name = "clmLine";
            this.clmLine.ReadOnly = true;
            this.clmLine.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmLine.Width = 58;
            // 
            // cmsParameters
            // 
            this.cmsParameters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParameterToolStripMenuItem,
            this.deleteParameterToolStripMenuItem});
            this.cmsParameters.Name = "cmsParameters";
            this.cmsParameters.Size = new System.Drawing.Size(221, 48);
            // 
            // addParameterToolStripMenuItem
            // 
            this.addParameterToolStripMenuItem.Name = "addParameterToolStripMenuItem";
            this.addParameterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.addParameterToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addParameterToolStripMenuItem.Text = "Добавить параметр...";
            this.addParameterToolStripMenuItem.Click += new System.EventHandler(this.addParameterToolStripMenuItem_Click);
            // 
            // deleteParameterToolStripMenuItem
            // 
            this.deleteParameterToolStripMenuItem.Name = "deleteParameterToolStripMenuItem";
            this.deleteParameterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteParameterToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.deleteParameterToolStripMenuItem.Text = "Удалить параметр";
            this.deleteParameterToolStripMenuItem.Click += new System.EventHandler(this.deleteParameterToolStripMenuItem_Click);
            // 
            // btnUp
            // 
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.Location = new System.Drawing.Point(85, 0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(32, 24);
            this.btnUp.TabIndex = 1;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(123, 0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(32, 24);
            this.btnDown.TabIndex = 2;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_add;
            this.btnAdd.Location = new System.Drawing.Point(10, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(32, 24);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.addParameterToolStripMenuItem_Click);
            // 
            // btnDel
            // 
            this.btnDel.Image = global::COTES.ISTOK.Client.Properties.Resources.edit_remove;
            this.btnDel.Location = new System.Drawing.Point(47, 1);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(32, 24);
            this.btnDel.TabIndex = 4;
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.deleteParameterToolStripMenuItem_Click);
            // 
            // GraphEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(525, 378);
            this.Name = "GraphEditForm";
            this.Load += new System.EventHandler(this.GraphEditForm_Load);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.cmsParameters.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.ContextMenuStrip cmsParameters;
        private System.Windows.Forms.ToolStripMenuItem addParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteParameterToolStripMenuItem;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmParam;
        private System.Windows.Forms.DataGridViewImageColumn clmLine;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAdd;
    }
}
