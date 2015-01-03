namespace COTES.ISTOK.Client
{
    partial class FormulaEditControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormulaEditControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtbFormula = new COTES.ISTOK.Client.FormulaTextBox(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calcMessageViewControl1 = new COTES.ISTOK.Client.Calc.CalcMessageViewControl();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLock = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.btnCheck = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.snippetToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.functionToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.constsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.argsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cbxAggregation = new System.Windows.Forms.ToolStripComboBox();
            this.btnCut = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.showImageToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.revisionToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Panel1.Controls.Add(this.rtbFormula);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.calcMessageViewControl1);
            this.splitContainer1.Size = new System.Drawing.Size(549, 349);
            this.splitContainer1.SplitterDistance = 144;
            this.splitContainer1.TabIndex = 11;
            // 
            // rtbFormula
            // 
            this.rtbFormula.AcceptsTab = true;
            this.rtbFormula.ContextMenuStrip = this.contextMenuStrip1;
            this.rtbFormula.DetectUrls = false;
            this.rtbFormula.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbFormula.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbFormula.HideSelection = false;
            this.rtbFormula.Location = new System.Drawing.Point(0, 0);
            this.rtbFormula.Name = "rtbFormula";
            this.rtbFormula.ShowCodeImages = true;
            this.rtbFormula.Size = new System.Drawing.Size(549, 144);
            this.rtbFormula.TabIndex = 0;
            this.rtbFormula.Text = "f(124);";
            this.rtbFormula.WordWrap = false;
            this.rtbFormula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbFormula_KeyPress);
            this.rtbFormula.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtbFormula_KeyUp);
            this.rtbFormula.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtbFormula_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRefToolStripMenuItem,
            this.showDepToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(199, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // showRefToolStripMenuItem
            // 
            this.showRefToolStripMenuItem.Name = "showRefToolStripMenuItem";
            this.showRefToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showRefToolStripMenuItem.Text = "Показать ссылки";
            this.showRefToolStripMenuItem.Click += new System.EventHandler(this.showRefToolStripMenuItem_Click);
            // 
            // showDepToolStripMenuItem
            // 
            this.showDepToolStripMenuItem.Name = "showDepToolStripMenuItem";
            this.showDepToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showDepToolStripMenuItem.Text = "Показать зависимости";
            this.showDepToolStripMenuItem.Click += new System.EventHandler(this.showDepToolStripMenuItem_Click);
            // 
            // calcMessageViewControl1
            // 
            this.calcMessageViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calcMessageViewControl1.GridContextMenu = null;
            this.calcMessageViewControl1.Location = new System.Drawing.Point(0, 0);
            this.calcMessageViewControl1.Name = "calcMessageViewControl1";
            this.calcMessageViewControl1.PageCount = -1;
            this.calcMessageViewControl1.Size = new System.Drawing.Size(549, 201);
            this.calcMessageViewControl1.StatusString = null;
            this.calcMessageViewControl1.strucProvider = null;
            this.calcMessageViewControl1.TabIndex = 0;
            // 
            // ttText
            // 
            this.ttText.AutoPopDelay = 5000;
            this.ttText.InitialDelay = 500;
            this.ttText.ReshowDelay = 1000;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLock,
            this.saveToolStripButton,
            this.btnCancel,
            this.btnCheck,
            this.toolStripSeparator1,
            this.snippetToolStripDropDownButton,
            this.functionToolStripDropDownButton,
            this.constsToolStripDropDownButton,
            this.argsToolStripDropDownButton,
            this.toolStripLabel1,
            this.cbxAggregation,
            this.btnCut,
            this.btnCopy,
            this.btnPaste,
            this.showImageToolStripButton,
            this.revisionToolStripComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(549, 27);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLock
            // 
            this.btnLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLock.Image = global::COTES.ISTOK.Client.Properties.Resources.edit;
            this.btnLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(23, 24);
            this.btnLock.Text = "toolStripButton1";
            this.btnLock.ToolTipText = "Изменить";
            this.btnLock.Click += new System.EventHandler(this.btnLock_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::COTES.ISTOK.Client.Properties.Resources.filesave;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.saveToolStripButton.Text = "Сохранить изменения";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCancel.Image = global::COTES.ISTOK.Client.Properties.Resources.cancel;
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(23, 24);
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.ToolTipText = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCheck.Image = ((System.Drawing.Image)(resources.GetObject("btnCheck.Image")));
            this.btnCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(23, 24);
            this.btnCheck.Text = "Проверка формулы";
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // snippetToolStripDropDownButton
            // 
            this.snippetToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.snippetToolStripDropDownButton.Image = global::COTES.ISTOK.Client.Properties.Resources.snippet;
            this.snippetToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.snippetToolStripDropDownButton.Name = "snippetToolStripDropDownButton";
            this.snippetToolStripDropDownButton.Size = new System.Drawing.Size(29, 24);
            this.snippetToolStripDropDownButton.Text = "toolStripDropDownButton1";
            this.snippetToolStripDropDownButton.ToolTipText = "Втсавить образец текста";
            // 
            // functionToolStripDropDownButton
            // 
            this.functionToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.functionToolStripDropDownButton.Image = global::COTES.ISTOK.Client.Properties.Resources.function;
            this.functionToolStripDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.functionToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.functionToolStripDropDownButton.Name = "functionToolStripDropDownButton";
            this.functionToolStripDropDownButton.Size = new System.Drawing.Size(45, 24);
            this.functionToolStripDropDownButton.Text = "Вставить функцию";
            this.functionToolStripDropDownButton.DropDownOpening += new System.EventHandler(this.functionToolStripDropDownButton_DropDownOpening);
            // 
            // constsToolStripDropDownButton
            // 
            this.constsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.constsToolStripDropDownButton.Image = global::COTES.ISTOK.Client.Properties.Resources.constant;
            this.constsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.constsToolStripDropDownButton.Name = "constsToolStripDropDownButton";
            this.constsToolStripDropDownButton.Size = new System.Drawing.Size(29, 24);
            this.constsToolStripDropDownButton.Text = "toolStripDropDownButton1";
            this.constsToolStripDropDownButton.ToolTipText = "Вставить константу";
            // 
            // argsToolStripDropDownButton
            // 
            this.argsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.argsToolStripDropDownButton.Image = global::COTES.ISTOK.Client.Properties.Resources.calcargument;
            this.argsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.argsToolStripDropDownButton.Name = "argsToolStripDropDownButton";
            this.argsToolStripDropDownButton.Size = new System.Drawing.Size(29, 24);
            this.argsToolStripDropDownButton.Text = "toolStripDropDownButton2";
            this.argsToolStripDropDownButton.ToolTipText = "Аргументы";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(67, 24);
            this.toolStripLabel1.Text = "Агрегация:";
            // 
            // cbxAggregation
            // 
            this.cbxAggregation.Name = "cbxAggregation";
            this.cbxAggregation.Size = new System.Drawing.Size(121, 27);
            // 
            // btnCut
            // 
            this.btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCut.Image = global::COTES.ISTOK.Client.Properties.Resources.editcut;
            this.btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(23, 24);
            this.btnCut.Text = "Вырезать";
            this.btnCut.Click += new System.EventHandler(this.btnCut_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.Image = global::COTES.ISTOK.Client.Properties.Resources.editcopy;
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 24);
            this.btnCopy.Text = "Копировать";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPaste.Image = global::COTES.ISTOK.Client.Properties.Resources.editpaste;
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(23, 24);
            this.btnPaste.Text = "Вставить";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // showImageToolStripButton
            // 
            this.showImageToolStripButton.CheckOnClick = true;
            this.showImageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showImageToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("showImageToolStripButton.Image")));
            this.showImageToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showImageToolStripButton.Name = "showImageToolStripButton";
            this.showImageToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.showImageToolStripButton.Text = "toolStripButton1";
            this.showImageToolStripButton.ToolTipText = "Отображать коды параметртров изображением";
            this.showImageToolStripButton.CheckedChanged += new System.EventHandler(this.showImageToolStripButton_CheckedChanged);
            // 
            // revisionToolStripComboBox
            // 
            this.revisionToolStripComboBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.revisionToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.revisionToolStripComboBox.Name = "revisionToolStripComboBox";
            this.revisionToolStripComboBox.Size = new System.Drawing.Size(121, 23);
            this.revisionToolStripComboBox.Visible = false;
            this.revisionToolStripComboBox.DropDown += new System.EventHandler(this.revisionToolStripComboBox_DropDown);
            this.revisionToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.revisionToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(78, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(78, 22);
            this.toolStripLabel3.Text = "toolStripLabel3";
            // 
            // FormulaEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormulaEditControl";
            this.Size = new System.Drawing.Size(549, 376);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip ttText;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showRefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDepToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnCheck;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbxAggregation;
        private System.Windows.Forms.ToolStripButton btnCut;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private FormulaTextBox rtbFormula;
        private System.Windows.Forms.ToolStripButton showImageToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton functionToolStripDropDownButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton constsToolStripDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton argsToolStripDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton snippetToolStripDropDownButton;
        private COTES.ISTOK.Client.Calc.CalcMessageViewControl calcMessageViewControl1;
        private System.Windows.Forms.ToolStripButton btnLock;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.ToolStripComboBox revisionToolStripComboBox;
    }
}
