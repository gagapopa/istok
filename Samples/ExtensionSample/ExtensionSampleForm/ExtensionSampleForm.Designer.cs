namespace ExtensionSample
{
    partial class ExtensionSampleForm
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
            this.receiverDataGridView = new System.Windows.Forms.DataGridView();
            this.receivedIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedBoilerIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedQualityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receivedTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.clearButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.receiverDataGridView)).BeginInit();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // receiverDataGridView
            // 
            this.receiverDataGridView.AllowUserToAddRows = false;
            this.receiverDataGridView.AllowUserToDeleteRows = false;
            this.receiverDataGridView.AllowUserToResizeRows = false;
            this.receiverDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.receiverDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.receiverDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.receivedIDColumn,
            this.receivedBoilerIDColumn,
            this.receivedCodeColumn,
            this.receivedValueColumn,
            this.receivedQualityColumn,
            this.receivedTimeColumn});
            this.receiverDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.receiverDataGridView.Location = new System.Drawing.Point(0, 0);
            this.receiverDataGridView.Name = "receiverDataGridView";
            this.receiverDataGridView.ReadOnly = true;
            this.receiverDataGridView.Size = new System.Drawing.Size(560, 391);
            this.receiverDataGridView.TabIndex = 0;
            // 
            // receivedIDColumn
            // 
            this.receivedIDColumn.HeaderText = "ID";
            this.receivedIDColumn.Name = "receivedIDColumn";
            this.receivedIDColumn.ReadOnly = true;
            // 
            // receivedBoilerIDColumn
            // 
            this.receivedBoilerIDColumn.HeaderText = "Boiler ID";
            this.receivedBoilerIDColumn.Name = "receivedBoilerIDColumn";
            this.receivedBoilerIDColumn.ReadOnly = true;
            // 
            // receivedCodeColumn
            // 
            this.receivedCodeColumn.HeaderText = "Code";
            this.receivedCodeColumn.Name = "receivedCodeColumn";
            this.receivedCodeColumn.ReadOnly = true;
            // 
            // receivedValueColumn
            // 
            this.receivedValueColumn.HeaderText = "Value";
            this.receivedValueColumn.Name = "receivedValueColumn";
            this.receivedValueColumn.ReadOnly = true;
            // 
            // receivedQualityColumn
            // 
            this.receivedQualityColumn.HeaderText = "Quality";
            this.receivedQualityColumn.Name = "receivedQualityColumn";
            this.receivedQualityColumn.ReadOnly = true;
            // 
            // receivedTimeColumn
            // 
            this.receivedTimeColumn.HeaderText = "Time";
            this.receivedTimeColumn.Name = "receivedTimeColumn";
            this.receivedTimeColumn.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.clearButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 391);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(560, 35);
            this.panel2.TabIndex = 1;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(5, 6);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 0;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(560, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // ExtensionSampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 448);
            this.Controls.Add(this.receiverDataGridView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ExtensionSampleForm";
            this.Text = "Пример обмена данных с ИСТОК-СБК";
            ((System.ComponentModel.ISupportInitialize)(this.receiverDataGridView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView receiverDataGridView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedBoilerIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedCodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedQualityColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn receivedTimeColumn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

