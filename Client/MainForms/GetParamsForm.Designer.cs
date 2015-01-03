namespace COTES.ISTOK.Client
{
    partial class GetParamsForm
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
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьВыделенныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьВсеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParamsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.contextMenuStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(292, 251);
            this.dataGridView2.TabIndex = 4;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьToolStripMenuItem,
            this.сохранитьВыделенныеToolStripMenuItem,
            this.сохранитьВсеToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip1";
            this.contextMenuStrip3.Size = new System.Drawing.Size(209, 70);
            // 
            // сохранитьToolStripMenuItem
            // 
            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.сохранитьToolStripMenuItem.Text = "Сохранить текущую";
            this.сохранитьToolStripMenuItem.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
            // 
            // сохранитьВыделенныеToolStripMenuItem
            // 
            this.сохранитьВыделенныеToolStripMenuItem.Name = "сохранитьВыделенныеToolStripMenuItem";
            this.сохранитьВыделенныеToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.сохранитьВыделенныеToolStripMenuItem.Text = "Сохранить выделенные";
            this.сохранитьВыделенныеToolStripMenuItem.Click += new System.EventHandler(this.сохранитьВыделенныеToolStripMenuItem_Click);
            // 
            // сохранитьВсеToolStripMenuItem
            // 
            this.сохранитьВсеToolStripMenuItem.Name = "сохранитьВсеToolStripMenuItem";
            this.сохранитьВсеToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.сохранитьВсеToolStripMenuItem.Text = "Сохранить все";
            this.сохранитьВсеToolStripMenuItem.Click += new System.EventHandler(this.сохранитьВсеToolStripMenuItem_Click);
            // 
            // addParamsBackgroundWorker
            // 
            this.addParamsBackgroundWorker.WorkerReportsProgress = true;
            this.addParamsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addParamsBackgroundWorker_DoWork);
            this.addParamsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.addParamsBackgroundWorker_RunWorkerCompleted);
            this.addParamsBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.addParamsBackgroundWorker_ProgressChanged);
            // 
            // GetParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.dataGridView2);
            this.Name = "GetParamsForm";
            this.Text = "Запрос параметров";
            this.Controls.SetChildIndex(this.dataGridView2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.contextMenuStrip3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьВыделенныеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьВсеToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker addParamsBackgroundWorker;
    }
}
