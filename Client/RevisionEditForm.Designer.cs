namespace COTES.ISTOK.Client
{
    partial class RevisionEditForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.revisionTreeView = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.removeRevisionButton = new System.Windows.Forms.Button();
            this.addRevisionButton = new System.Windows.Forms.Button();
            this.revisionCommentTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.revisionTimeFromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.revisionBriefTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.clearButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.revisionTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.revisionCommentTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(603, 308);
            this.splitContainer1.SplitterDistance = 259;
            this.splitContainer1.TabIndex = 0;
            // 
            // revisionTreeView
            // 
            this.revisionTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.revisionTreeView.FullRowSelect = true;
            this.revisionTreeView.HideSelection = false;
            this.revisionTreeView.Location = new System.Drawing.Point(0, 0);
            this.revisionTreeView.Name = "revisionTreeView";
            this.revisionTreeView.ShowLines = false;
            this.revisionTreeView.ShowPlusMinus = false;
            this.revisionTreeView.ShowRootLines = false;
            this.revisionTreeView.Size = new System.Drawing.Size(259, 273);
            this.revisionTreeView.TabIndex = 1;
            this.revisionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.revisionTreeView_AfterSelect);
            this.revisionTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.revisionTreeView_BeforeSelect);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.removeRevisionButton);
            this.panel2.Controls.Add(this.addRevisionButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 273);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(259, 35);
            this.panel2.TabIndex = 0;
            // 
            // removeRevisionButton
            // 
            this.removeRevisionButton.Location = new System.Drawing.Point(93, 6);
            this.removeRevisionButton.Name = "removeRevisionButton";
            this.removeRevisionButton.Size = new System.Drawing.Size(75, 23);
            this.removeRevisionButton.TabIndex = 1;
            this.removeRevisionButton.Text = "Удалить ревизию";
            this.removeRevisionButton.UseVisualStyleBackColor = true;
            this.removeRevisionButton.Click += new System.EventHandler(this.removeRevisionButton_Click);
            // 
            // addRevisionButton
            // 
            this.addRevisionButton.Location = new System.Drawing.Point(12, 6);
            this.addRevisionButton.Name = "addRevisionButton";
            this.addRevisionButton.Size = new System.Drawing.Size(75, 23);
            this.addRevisionButton.TabIndex = 0;
            this.addRevisionButton.Text = "Добавить ревизию";
            this.addRevisionButton.UseVisualStyleBackColor = true;
            this.addRevisionButton.Click += new System.EventHandler(this.addRevisionButton_Click);
            // 
            // revisionCommentTextBox
            // 
            this.revisionCommentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.revisionCommentTextBox.Location = new System.Drawing.Point(0, 101);
            this.revisionCommentTextBox.Multiline = true;
            this.revisionCommentTextBox.Name = "revisionCommentTextBox";
            this.revisionCommentTextBox.Size = new System.Drawing.Size(340, 207);
            this.revisionCommentTextBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.revisionTimeFromDateTimePicker);
            this.panel1.Controls.Add(this.revisionBriefTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 101);
            this.panel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Описание:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Время начала:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Краткое описание:";
            // 
            // revisionTimeFromDateTimePicker
            // 
            this.revisionTimeFromDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionTimeFromDateTimePicker.Location = new System.Drawing.Point(91, 9);
            this.revisionTimeFromDateTimePicker.Name = "revisionTimeFromDateTimePicker";
            this.revisionTimeFromDateTimePicker.Size = new System.Drawing.Size(246, 20);
            this.revisionTimeFromDateTimePicker.TabIndex = 1;
            // 
            // revisionBriefTextBox
            // 
            this.revisionBriefTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionBriefTextBox.Location = new System.Drawing.Point(3, 56);
            this.revisionBriefTextBox.Name = "revisionBriefTextBox";
            this.revisionBriefTextBox.Size = new System.Drawing.Size(334, 20);
            this.revisionBriefTextBox.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.clearButton);
            this.panel3.Controls.Add(this.saveButton);
            this.panel3.Controls.Add(this.closeButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 308);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(603, 37);
            this.panel3.TabIndex = 1;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(435, 6);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 2;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(354, 6);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(516, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // RevisionEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 367);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel3);
            this.Name = "RevisionEditForm";
            this.Text = "Ревизии";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RevisionEditForm_FormClosing);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox revisionCommentTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker revisionTimeFromDateTimePicker;
        private System.Windows.Forms.TextBox revisionBriefTextBox;
        private System.Windows.Forms.TreeView revisionTreeView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button removeRevisionButton;
        private System.Windows.Forms.Button addRevisionButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button clearButton;
    }
}