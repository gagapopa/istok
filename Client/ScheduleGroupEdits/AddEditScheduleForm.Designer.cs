namespace COTES.ISTOK.Client
{
    partial class AddEditScheduleForm
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
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.scheduleTimePicker = new System.Windows.Forms.DateTimePicker();
            this.dayScheduleNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dayOfWeekScheduleComboBox = new System.Windows.Forms.ComboBox();
            this.periodScheduleComboBox = new System.Windows.Forms.ComboBox();
            this.cbxRepeatEach = new System.Windows.Forms.ComboBox();
            this.cbxRepeatFor = new System.Windows.Forms.ComboBox();
            this.chbxRepeat = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dtp = new System.Windows.Forms.DateTimePicker();
            this.dtpLastCall = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dayScheduleNumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(57, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Название";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(75, 6);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(186, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(182, 275);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(101, 275);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Сохранить";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(164, 62);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(39, 13);
            this.label23.TabIndex = 32;
            this.label23.Text = "Число";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(164, 35);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(40, 13);
            this.label22.TabIndex = 31;
            this.label22.Text = "Время";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 62);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(34, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "День";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Период";
            // 
            // scheduleTimePicker
            // 
            this.scheduleTimePicker.CustomFormat = "HH:mm";
            this.scheduleTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.scheduleTimePicker.Location = new System.Drawing.Point(210, 33);
            this.scheduleTimePicker.Name = "scheduleTimePicker";
            this.scheduleTimePicker.ShowUpDown = true;
            this.scheduleTimePicker.Size = new System.Drawing.Size(51, 20);
            this.scheduleTimePicker.TabIndex = 28;
            // 
            // dayScheduleNumericUpDown
            // 
            this.dayScheduleNumericUpDown.Location = new System.Drawing.Point(210, 60);
            this.dayScheduleNumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.dayScheduleNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.dayScheduleNumericUpDown.Name = "dayScheduleNumericUpDown";
            this.dayScheduleNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this.dayScheduleNumericUpDown.TabIndex = 27;
            this.dayScheduleNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // dayOfWeekScheduleComboBox
            // 
            this.dayOfWeekScheduleComboBox.FormattingEnabled = true;
            this.dayOfWeekScheduleComboBox.Location = new System.Drawing.Point(75, 59);
            this.dayOfWeekScheduleComboBox.Name = "dayOfWeekScheduleComboBox";
            this.dayOfWeekScheduleComboBox.Size = new System.Drawing.Size(83, 21);
            this.dayOfWeekScheduleComboBox.TabIndex = 26;
            // 
            // periodScheduleComboBox
            // 
            this.periodScheduleComboBox.FormattingEnabled = true;
            this.periodScheduleComboBox.Location = new System.Drawing.Point(75, 32);
            this.periodScheduleComboBox.Name = "periodScheduleComboBox";
            this.periodScheduleComboBox.Size = new System.Drawing.Size(83, 21);
            this.periodScheduleComboBox.TabIndex = 25;
            this.periodScheduleComboBox.SelectedIndexChanged += new System.EventHandler(this.periodScheduleComboBox_SelectedIndexChanged);
            // 
            // cbxRepeatEach
            // 
            this.cbxRepeatEach.FormattingEnabled = true;
            this.cbxRepeatEach.Location = new System.Drawing.Point(167, 109);
            this.cbxRepeatEach.Name = "cbxRepeatEach";
            this.cbxRepeatEach.Size = new System.Drawing.Size(94, 21);
            this.cbxRepeatEach.TabIndex = 33;
            this.cbxRepeatEach.Leave += new System.EventHandler(this.cbxRepeatEach_Leave);
            // 
            // cbxRepeatFor
            // 
            this.cbxRepeatFor.FormattingEnabled = true;
            this.cbxRepeatFor.Location = new System.Drawing.Point(167, 136);
            this.cbxRepeatFor.Name = "cbxRepeatFor";
            this.cbxRepeatFor.Size = new System.Drawing.Size(94, 21);
            this.cbxRepeatFor.TabIndex = 34;
            this.cbxRepeatFor.Leave += new System.EventHandler(this.cbxRepeatFor_Leave);
            // 
            // chbxRepeat
            // 
            this.chbxRepeat.AutoSize = true;
            this.chbxRepeat.Location = new System.Drawing.Point(15, 86);
            this.chbxRepeat.Name = "chbxRepeat";
            this.chbxRepeat.Size = new System.Drawing.Size(87, 17);
            this.chbxRepeat.TabIndex = 35;
            this.chbxRepeat.Text = "Повторение";
            this.chbxRepeat.UseVisualStyleBackColor = true;
            this.chbxRepeat.CheckedChanged += new System.EventHandler(this.chbxRepeat_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Повторять каждые:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "В течение:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(168, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 38;
            this.button1.Text = "Check";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dtp
            // 
            this.dtp.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp.Location = new System.Drawing.Point(107, 19);
            this.dtp.Name = "dtp";
            this.dtp.Size = new System.Drawing.Size(136, 20);
            this.dtp.TabIndex = 39;
            // 
            // dtpLastCall
            // 
            this.dtpLastCall.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpLastCall.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpLastCall.Location = new System.Drawing.Point(107, 45);
            this.dtpLastCall.Name = "dtpLastCall";
            this.dtpLastCall.Size = new System.Drawing.Size(136, 20);
            this.dtpLastCall.TabIndex = 40;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtp);
            this.groupBox1.Controls.Add(this.dtpLastCall);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(249, 102);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CheckTimes";
            this.groupBox1.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Last call time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 41;
            this.label3.Text = "Current time:";
            // 
            // AddEditScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 310);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chbxRepeat);
            this.Controls.Add(this.cbxRepeatFor);
            this.Controls.Add(this.cbxRepeatEach);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.scheduleTimePicker);
            this.Controls.Add(this.dayScheduleNumericUpDown);
            this.Controls.Add(this.dayOfWeekScheduleComboBox);
            this.Controls.Add(this.periodScheduleComboBox);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.groupBox1);
            this.MinimizeBox = false;
            this.Name = "AddEditScheduleForm";
            this.Text = "Расписание";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddEditScheduleForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dayScheduleNumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker scheduleTimePicker;
        private System.Windows.Forms.NumericUpDown dayScheduleNumericUpDown;
        private System.Windows.Forms.ComboBox dayOfWeekScheduleComboBox;
        private System.Windows.Forms.ComboBox periodScheduleComboBox;
        private System.Windows.Forms.ComboBox cbxRepeatEach;
        private System.Windows.Forms.ComboBox cbxRepeatFor;
        private System.Windows.Forms.CheckBox chbxRepeat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dtp;
        private System.Windows.Forms.DateTimePicker dtpLastCall;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}