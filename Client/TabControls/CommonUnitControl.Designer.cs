namespace COTES.ISTOK.Client
{
    partial class CommonUnitControl
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.revisionToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.tsbLock = new System.Windows.Forms.ToolStripButton();
            this.tsbCalc = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.logUnitToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ControlLight;
            this.propertyGrid.Location = new System.Drawing.Point(0, 25);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(299, 194);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.ViewBackColor = System.Drawing.Color.Ivory;
            this.propertyGrid.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLock,
            this.tsbCalc,
            this.tsbSave,
            this.tsbCancel,
            this.logUnitToolStripButton,
            this.revisionToolStripComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(299, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // revisionToolStripComboBox
            // 
            this.revisionToolStripComboBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.revisionToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.revisionToolStripComboBox.Name = "revisionToolStripComboBox";
            this.revisionToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            this.revisionToolStripComboBox.Visible = false;
            this.revisionToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.revisionToolStripComboBox_SelectedIndexChanged);
            this.revisionToolStripComboBox.DropDown += new System.EventHandler(this.revisionToolStripComboBox_DropDown);
            // 
            // tsbLock
            // 
            this.tsbLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLock.Image = global::COTES.ISTOK.Client.Properties.Resources.edit;
            this.tsbLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLock.Name = "tsbLock";
            this.tsbLock.Size = new System.Drawing.Size(23, 22);
            this.tsbLock.Text = "Изменить";
            this.tsbLock.Click += new System.EventHandler(this.tsbLock_Click);
            // 
            // tsbCalc
            // 
            this.tsbCalc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCalc.Image = global::COTES.ISTOK.Client.Properties.Resources.kcalc;
            this.tsbCalc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCalc.Name = "tsbCalc";
            this.tsbCalc.Size = new System.Drawing.Size(23, 22);
            this.tsbCalc.Text = "Расчитать";
            this.tsbCalc.Click += new System.EventHandler(this.tsbCalc_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::COTES.ISTOK.Client.Properties.Resources.filesave;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Сохранить";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCancel.Image = global::COTES.ISTOK.Client.Properties.Resources.cancel;
            this.tsbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancel.Name = "tsbCancel";
            this.tsbCancel.Size = new System.Drawing.Size(23, 22);
            this.tsbCancel.Text = "Отмена";
            this.tsbCancel.Click += new System.EventHandler(this.tsbCancel_Click);
            // 
            // logUnitToolStripButton
            // 
            this.logUnitToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.logUnitToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.logUnitToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources.revision_log;
            this.logUnitToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.logUnitToolStripButton.Name = "logUnitToolStripButton";
            this.logUnitToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.logUnitToolStripButton.Text = "Просмотреть изменения";
            this.logUnitToolStripButton.Click += new System.EventHandler(this.logUnitToolStripButton_Click);
            // 
            // CommonUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.toolStrip1);
            this.Name = "CommonUnitControl";
            this.Size = new System.Drawing.Size(299, 219);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbLock;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private System.Windows.Forms.ToolStripButton tsbCalc;
        private System.Windows.Forms.ToolStripComboBox revisionToolStripComboBox;
        private System.Windows.Forms.ToolStripButton logUnitToolStripButton;
    }
}
