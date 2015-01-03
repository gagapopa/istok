namespace COTES.ISTOK.Monitoring
{
    partial class ChannelMonitoringControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripTextBox filterToolStripTextBox;
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.monitoringDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewChannelsPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWriteBufferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushWriteBufferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnsGridToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.columnContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.columnsHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            filterToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.monitoringDataGridView)).BeginInit();
            this.dataGridContextMenuStrip.SuspendLayout();
            this.columnContextMenuStrip.SuspendLayout();
            this.headerContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            toolStripSeparator1.Visible = false;
            // 
            // filterToolStripTextBox
            // 
            filterToolStripTextBox.Name = "filterToolStripTextBox";
            filterToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            filterToolStripTextBox.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(242, 6);
            // 
            // monitoringDataGridView
            // 
            this.monitoringDataGridView.AllowUserToAddRows = false;
            this.monitoringDataGridView.AllowUserToDeleteRows = false;
            this.monitoringDataGridView.AllowUserToOrderColumns = true;
            this.monitoringDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.monitoringDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.monitoringDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.monitoringDataGridView.ContextMenuStrip = this.dataGridContextMenuStrip;
            this.monitoringDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitoringDataGridView.Enabled = false;
            this.monitoringDataGridView.Location = new System.Drawing.Point(0, 0);
            this.monitoringDataGridView.MultiSelect = false;
            this.monitoringDataGridView.Name = "monitoringDataGridView";
            this.monitoringDataGridView.ReadOnly = true;
            this.monitoringDataGridView.RowHeadersVisible = false;
            this.monitoringDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.monitoringDataGridView.Size = new System.Drawing.Size(150, 150);
            this.monitoringDataGridView.TabIndex = 1;
            this.monitoringDataGridView.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.monitoringDataGridView_CellContextMenuStripNeeded);
            this.monitoringDataGridView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.monitoringDataGridView_ColumnAdded);
            this.monitoringDataGridView.ColumnRemoved += new System.Windows.Forms.DataGridViewColumnEventHandler(this.monitoringDataGridView_ColumnRemoved);
            this.monitoringDataGridView.Sorted += new System.EventHandler(this.monitoringDataGridView_Sorted);
            this.monitoringDataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.monitoringDataGridView_MouseDoubleClick);
            // 
            // dataGridContextMenuStrip
            // 
            this.dataGridContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewChannelsPropertiesToolStripMenuItem,
            this.manageToolStripMenuItem,
            this.viewWriteBufferToolStripMenuItem,
            this.flushWriteBufferToolStripMenuItem,
            this.toolStripSeparator3,
            this.columnsGridToolStripMenuItem1});
            this.dataGridContextMenuStrip.Name = "dataGridContextMenuStrip";
            this.dataGridContextMenuStrip.Size = new System.Drawing.Size(246, 142);
            this.dataGridContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dataGridContextMenuStrip_Opening);
            // 
            // viewChannelsPropertiesToolStripMenuItem
            // 
            this.viewChannelsPropertiesToolStripMenuItem.Name = "viewChannelsPropertiesToolStripMenuItem";
            this.viewChannelsPropertiesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.viewChannelsPropertiesToolStripMenuItem.Text = "Просмотр свойств канала";
            this.viewChannelsPropertiesToolStripMenuItem.Click += new System.EventHandler(this.viewChannelsPropertiesToolStripMenuItem_Click);
            // 
            // manageToolStripMenuItem
            // 
            this.manageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.unloadToolStripMenuItem});
            this.manageToolStripMenuItem.Name = "manageToolStripMenuItem";
            this.manageToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.manageToolStripMenuItem.Text = "Управление каналом";
            this.manageToolStripMenuItem.DropDownOpening += new System.EventHandler(this.manageToolStripMenuItem_DropDownOpening);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Запустить";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Остановить";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Enabled = false;
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Загрузить";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // unloadToolStripMenuItem
            // 
            this.unloadToolStripMenuItem.Enabled = false;
            this.unloadToolStripMenuItem.Name = "unloadToolStripMenuItem";
            this.unloadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.unloadToolStripMenuItem.Text = "Выгрузить";
            this.unloadToolStripMenuItem.Click += new System.EventHandler(this.unloadToolStripMenuItem_Click);
            // 
            // viewWriteBufferToolStripMenuItem
            // 
            this.viewWriteBufferToolStripMenuItem.Name = "viewWriteBufferToolStripMenuItem";
            this.viewWriteBufferToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.viewWriteBufferToolStripMenuItem.Text = "Просмотр значений из буфера";
            this.viewWriteBufferToolStripMenuItem.Click += new System.EventHandler(this.viewWriteBufferToolStripMenuItem_Click);
            // 
            // flushWriteBufferToolStripMenuItem
            // 
            this.flushWriteBufferToolStripMenuItem.Name = "flushWriteBufferToolStripMenuItem";
            this.flushWriteBufferToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.flushWriteBufferToolStripMenuItem.Text = "Сбросить буфер";
            this.flushWriteBufferToolStripMenuItem.Click += new System.EventHandler(this.flushWriteBufferToolStripMenuItem_Click);
            // 
            // columnsGridToolStripMenuItem1
            // 
            this.columnsGridToolStripMenuItem1.DropDown = this.columnContextMenuStrip;
            this.columnsGridToolStripMenuItem1.Name = "columnsGridToolStripMenuItem1";
            this.columnsGridToolStripMenuItem1.Size = new System.Drawing.Size(245, 22);
            this.columnsGridToolStripMenuItem1.Text = "Столбцы";
            // 
            // columnContextMenuStrip
            // 
            this.columnContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem,
            this.hideAllToolStripMenuItem,
            this.toolStripSeparator2});
            this.columnContextMenuStrip.Name = "columnContextMenuStrip";
            this.columnContextMenuStrip.ShowCheckMargin = true;
            this.columnContextMenuStrip.ShowImageMargin = false;
            this.columnContextMenuStrip.Size = new System.Drawing.Size(146, 54);
            // 
            // showAllToolStripMenuItem
            // 
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            this.showAllToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.showAllToolStripMenuItem.Text = "Показать все";
            this.showAllToolStripMenuItem.Click += new System.EventHandler(this.showAllToolStripMenuItem_Click);
            // 
            // hideAllToolStripMenuItem
            // 
            this.hideAllToolStripMenuItem.Name = "hideAllToolStripMenuItem";
            this.hideAllToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.hideAllToolStripMenuItem.Text = "Скрыть все";
            this.hideAllToolStripMenuItem.Click += new System.EventHandler(this.hideAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(142, 6);
            // 
            // columnsHeaderToolStripMenuItem
            // 
            this.columnsHeaderToolStripMenuItem.DropDown = this.columnContextMenuStrip;
            this.columnsHeaderToolStripMenuItem.Name = "columnsHeaderToolStripMenuItem";
            this.columnsHeaderToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.columnsHeaderToolStripMenuItem.Text = "Столбцы";
            // 
            // headerContextMenuStrip
            // 
            this.headerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.columnsHeaderToolStripMenuItem,
            toolStripSeparator1,
            filterToolStripTextBox});
            this.headerContextMenuStrip.Name = "headerContextMenuStrip";
            this.headerContextMenuStrip.Size = new System.Drawing.Size(166, 79);
            this.headerContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.headerContextMenuStrip_Opening);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.hideToolStripMenuItem.Text = "Скрыть Столбец";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // ChannelMonitoringControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.monitoringDataGridView);
            this.DoubleBuffered = true;
            this.Name = "ChannelMonitoringControl";
            ((System.ComponentModel.ISupportInitialize)(this.monitoringDataGridView)).EndInit();
            this.dataGridContextMenuStrip.ResumeLayout(false);
            this.columnContextMenuStrip.ResumeLayout(false);
            this.headerContextMenuStrip.ResumeLayout(false);
            this.headerContextMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView monitoringDataGridView;
        private System.Windows.Forms.ContextMenuStrip headerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnsHeaderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip columnContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip dataGridContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewChannelsPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewWriteBufferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flushWriteBufferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnsGridToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
