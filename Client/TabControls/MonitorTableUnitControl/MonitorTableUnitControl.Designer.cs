namespace COTES.ISTOK.Client
{
    partial class MonitorTableUnitControl
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.cmsTable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.setParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.setLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.cmsTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ContextMenuStrip = this.cmsTable;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.Size = new System.Drawing.Size(150, 150);
            this.dgv.TabIndex = 0;
            this.dgv.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellValueChanged);
            this.dgv.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgv_UserDeletingRow);
            this.dgv.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellMouseLeave);
            this.dgv.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_CellMouseMove);
            this.dgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgv_CellPainting);
            this.dgv.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_CellMouseDoubleClick);
            this.dgv.CurrentCellChanged += new System.EventHandler(this.dgv_CurrentCellChanged);
            this.dgv.Paint += new System.Windows.Forms.PaintEventHandler(this.dgv_Paint);
            this.dgv.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgv_RowsRemoved);
            // 
            // cmsTable
            // 
            this.cmsTable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addColumnToolStripMenuItem,
            this.removeColumnToolStripMenuItem,
            this.toolStripMenuItem1,
            this.setParameterToolStripMenuItem,
            this.clearParameterToolStripMenuItem,
            this.toolStripMenuItem2,
            this.setLinkToolStripMenuItem,
            this.clearLinkToolStripMenuItem});
            this.cmsTable.Name = "cmsTable";
            this.cmsTable.Size = new System.Drawing.Size(202, 148);
            this.cmsTable.Opening += new System.ComponentModel.CancelEventHandler(this.cmsTable_Opening);
            // 
            // addColumnToolStripMenuItem
            // 
            this.addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
            this.addColumnToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.addColumnToolStripMenuItem.Text = "Добавить колонку...";
            this.addColumnToolStripMenuItem.Click += new System.EventHandler(this.addColumnToolStripMenuItem_Click);
            // 
            // removeColumnToolStripMenuItem
            // 
            this.removeColumnToolStripMenuItem.Name = "removeColumnToolStripMenuItem";
            this.removeColumnToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeColumnToolStripMenuItem.Text = "Удалить колонку";
            this.removeColumnToolStripMenuItem.Click += new System.EventHandler(this.removeColumnToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(198, 6);
            // 
            // setParameterToolStripMenuItem
            // 
            this.setParameterToolStripMenuItem.Name = "setParameterToolStripMenuItem";
            this.setParameterToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.setParameterToolStripMenuItem.Text = "Установить параметр...";
            this.setParameterToolStripMenuItem.Click += new System.EventHandler(this.setParameterToolStripMenuItem_Click);
            // 
            // clearParameterToolStripMenuItem
            // 
            this.clearParameterToolStripMenuItem.Name = "clearParameterToolStripMenuItem";
            this.clearParameterToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.clearParameterToolStripMenuItem.Text = "Удалить параметр";
            this.clearParameterToolStripMenuItem.Click += new System.EventHandler(this.clearParameterToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(198, 6);
            // 
            // setLinkToolStripMenuItem
            // 
            this.setLinkToolStripMenuItem.Name = "setLinkToolStripMenuItem";
            this.setLinkToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.setLinkToolStripMenuItem.Text = "Установить ссылку...";
            this.setLinkToolStripMenuItem.Click += new System.EventHandler(this.setLinkToolStripMenuItem_Click);
            // 
            // clearLinkToolStripMenuItem
            // 
            this.clearLinkToolStripMenuItem.Name = "clearLinkToolStripMenuItem";
            this.clearLinkToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.clearLinkToolStripMenuItem.Text = "Удалить ссылку";
            this.clearLinkToolStripMenuItem.Click += new System.EventHandler(this.clearLinkToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MonitorTableUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv);
            this.Name = "MonitorTableUnitControl";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.cmsTable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip cmsTable;
        private System.Windows.Forms.ToolStripMenuItem addColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem setLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLinkToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
