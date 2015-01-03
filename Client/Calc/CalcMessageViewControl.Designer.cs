namespace COTES.ISTOK.Client.Calc
{
    partial class CalcMessageViewControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvMessages = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.calcStatusLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nextMessagesButton = new System.Windows.Forms.Button();
            this.prevMessagesButton = new System.Windows.Forms.Button();
            this.clmMessNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmNode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMessages
            // 
            this.dgvMessages.AllowUserToAddRows = false;
            this.dgvMessages.AllowUserToDeleteRows = false;
            this.dgvMessages.AllowUserToResizeRows = false;
            this.dgvMessages.AutoGenerateColumns = false;
            this.dgvMessages.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMessages.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmMessNo,
            this.clmType,
            this.clmMessage,
            this.clmNode,
            this.clmLine});
            this.dgvMessages.DataSource = this.bindingSource1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMessages.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMessages.Location = new System.Drawing.Point(0, 29);
            this.dgvMessages.MultiSelect = false;
            this.dgvMessages.Name = "dgvMessages";
            this.dgvMessages.ReadOnly = true;
            this.dgvMessages.RowHeadersVisible = false;
            this.dgvMessages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMessages.Size = new System.Drawing.Size(331, 164);
            this.dgvMessages.TabIndex = 13;
            // 
            // calcStatusLabel
            // 
            this.calcStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.calcStatusLabel.Location = new System.Drawing.Point(3, 3);
            this.calcStatusLabel.Name = "calcStatusLabel";
            this.calcStatusLabel.Size = new System.Drawing.Size(287, 23);
            this.calcStatusLabel.TabIndex = 14;
            this.calcStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nextMessagesButton);
            this.panel1.Controls.Add(this.prevMessagesButton);
            this.panel1.Controls.Add(this.calcStatusLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 29);
            this.panel1.TabIndex = 15;
            // 
            // nextMessagesButton
            // 
            this.nextMessagesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nextMessagesButton.Location = new System.Drawing.Point(309, 3);
            this.nextMessagesButton.Name = "nextMessagesButton";
            this.nextMessagesButton.Size = new System.Drawing.Size(16, 23);
            this.nextMessagesButton.TabIndex = 19;
            this.nextMessagesButton.Text = ">";
            this.nextMessagesButton.UseVisualStyleBackColor = true;
            this.nextMessagesButton.Click += new System.EventHandler(this.nextMessagesButton_Click);
            // 
            // prevMessagesButton
            // 
            this.prevMessagesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prevMessagesButton.Location = new System.Drawing.Point(287, 3);
            this.prevMessagesButton.Name = "prevMessagesButton";
            this.prevMessagesButton.Size = new System.Drawing.Size(16, 23);
            this.prevMessagesButton.TabIndex = 18;
            this.prevMessagesButton.Text = "<";
            this.prevMessagesButton.UseVisualStyleBackColor = true;
            this.prevMessagesButton.Click += new System.EventHandler(this.prevMessagesButton_Click);
            // 
            // clmMessNo
            // 
            this.clmMessNo.DataPropertyName = "Position";
            this.clmMessNo.FillWeight = 20.55838F;
            this.clmMessNo.HeaderText = "#";
            this.clmMessNo.Name = "clmMessNo";
            this.clmMessNo.ReadOnly = true;
            // 
            // clmType
            // 
            this.clmType.DataPropertyName = "Category";
            this.clmType.FillWeight = 29.34607F;
            this.clmType.HeaderText = "Тип";
            this.clmType.Name = "clmType";
            this.clmType.ReadOnly = true;
            // 
            // clmMessage
            // 
            this.clmMessage.DataPropertyName = "Text";
            this.clmMessage.FillWeight = 146.7304F;
            this.clmMessage.HeaderText = "Сообщение";
            this.clmMessage.Name = "clmMessage";
            this.clmMessage.ReadOnly = true;
            // 
            // clmNode
            // 
            this.clmNode.DataPropertyName = "Node";
            this.clmNode.FillWeight = 44.01911F;
            this.clmNode.HeaderText = "Узел";
            this.clmNode.Name = "clmNode";
            this.clmNode.ReadOnly = true;
            // 
            // clmLine
            // 
            this.clmLine.DataPropertyName = "Line";
            this.clmLine.FillWeight = 29.34607F;
            this.clmLine.HeaderText = "Строка";
            this.clmLine.Name = "clmLine";
            this.clmLine.ReadOnly = true;
            // 
            // CalcMessageViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvMessages);
            this.Controls.Add(this.panel1);
            this.Name = "CalcMessageViewControl";
            this.Size = new System.Drawing.Size(331, 193);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMessages;
        private System.Windows.Forms.Label calcStatusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button nextMessagesButton;
        private System.Windows.Forms.Button prevMessagesButton;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMessNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNode;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmLine;
    }
}
