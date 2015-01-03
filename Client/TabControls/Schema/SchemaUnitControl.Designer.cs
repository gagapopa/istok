namespace COTES.ISTOK.Client
{
    partial class SchemaUnitControl
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmsPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cmsPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cmsPicture
            // 
            this.cmsPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParameterToolStripMenuItem,
            this.deleteParameterToolStripMenuItem,
            this.toolStripMenuItem1,
            this.loadPictureToolStripMenuItem,
            this.savePictureToolStripMenuItem,
            this.clearPictureToolStripMenuItem});
            this.cmsPicture.Name = "cmsPicture";
            this.cmsPicture.Size = new System.Drawing.Size(205, 142);
            this.cmsPicture.Opening += new System.ComponentModel.CancelEventHandler(this.cmsPicture_Opening);
            // 
            // addParameterToolStripMenuItem
            // 
            this.addParameterToolStripMenuItem.Name = "addParameterToolStripMenuItem";
            this.addParameterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.addParameterToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.addParameterToolStripMenuItem.Text = "Добавить параметр";
            this.addParameterToolStripMenuItem.Click += new System.EventHandler(this.addParameterToolStripMenuItem_Click);
            // 
            // deleteParameterToolStripMenuItem
            // 
            this.deleteParameterToolStripMenuItem.Name = "deleteParameterToolStripMenuItem";
            this.deleteParameterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteParameterToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.deleteParameterToolStripMenuItem.Text = "Удалить параметр";
            this.deleteParameterToolStripMenuItem.Click += new System.EventHandler(this.deleteParameterToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            // 
            // loadPictureToolStripMenuItem
            // 
            this.loadPictureToolStripMenuItem.Name = "loadPictureToolStripMenuItem";
            this.loadPictureToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.loadPictureToolStripMenuItem.Text = "Загрузить фон";
            this.loadPictureToolStripMenuItem.Click += new System.EventHandler(this.loadPictureToolStripMenuItem_Click);
            // 
            // savePictureToolStripMenuItem
            // 
            this.savePictureToolStripMenuItem.Name = "savePictureToolStripMenuItem";
            this.savePictureToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.savePictureToolStripMenuItem.Text = "Сохранить фон";
            this.savePictureToolStripMenuItem.Click += new System.EventHandler(this.savePictureToolStripMenuItem_Click);
            // 
            // clearPictureToolStripMenuItem
            // 
            this.clearPictureToolStripMenuItem.Name = "clearPictureToolStripMenuItem";
            this.clearPictureToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.clearPictureToolStripMenuItem.Text = "Очистить фон";
            this.clearPictureToolStripMenuItem.Click += new System.EventHandler(this.clearPictureToolStripMenuItem_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.ContextMenuStrip = this.cmsPicture;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(150, 150);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.DoubleClick += new System.EventHandler(this.pictureBox_DoubleClick);
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            // 
            // SchemaUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Name = "SchemaUnitControl";
            this.cmsPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip cmsPicture;
        private System.Windows.Forms.ToolStripMenuItem addParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPictureToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}
