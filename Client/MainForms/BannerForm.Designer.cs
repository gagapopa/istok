namespace COTES.ISTOK.Client
{
    partial class BannerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BannerForm));
            this.loadProgressBar = new System.Windows.Forms.ProgressBar();
            this.logoTypePictureBox = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logoTypePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // loadProgressBar
            // 
            this.loadProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.loadProgressBar.Location = new System.Drawing.Point(0, 317);
            this.loadProgressBar.Name = "loadProgressBar";
            this.loadProgressBar.Size = new System.Drawing.Size(352, 23);
            this.loadProgressBar.TabIndex = 0;
            // 
            // logoTypePictureBox
            // 
            this.logoTypePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logoTypePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoTypePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoTypePictureBox.Image")));
            this.logoTypePictureBox.Location = new System.Drawing.Point(0, 0);
            this.logoTypePictureBox.Name = "logoTypePictureBox";
            this.logoTypePictureBox.Size = new System.Drawing.Size(352, 317);
            this.logoTypePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoTypePictureBox.TabIndex = 1;
            this.logoTypePictureBox.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusLabel.Location = new System.Drawing.Point(1, 303);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(350, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Обождите. Идет загрузка нужных штук...";
            // 
            // BannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(352, 340);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.logoTypePictureBox);
            this.Controls.Add(this.loadProgressBar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BannerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Обождите. Идет загрузка нужных штук...";
            ((System.ComponentModel.ISupportInitialize)(this.logoTypePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar loadProgressBar;
        private System.Windows.Forms.PictureBox logoTypePictureBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label statusLabel;
    }
}