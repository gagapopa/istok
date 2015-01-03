namespace IF97Calc
{
    partial class IF97CalcForm
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
            this.pTextBox = new System.Windows.Forms.TextBox();
            this.tTextBox = new System.Windows.Forms.TextBox();
            this.pLabel = new System.Windows.Forms.Label();
            this.TLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tsUnitComboBox = new System.Windows.Forms.ComboBox();
            this.psUnitComboBox = new System.Windows.Forms.ComboBox();
            this.tsTextBox = new System.Windows.Forms.TextBox();
            this.psTextBox = new System.Windows.Forms.TextBox();
            this.tsLabel = new System.Windows.Forms.Label();
            this.p_sLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.wUnitComboBox = new System.Windows.Forms.ComboBox();
            this.cnuUnitComboBox = new System.Windows.Forms.ComboBox();
            this.cpUnitComboBox = new System.Windows.Forms.ComboBox();
            this.hUnitComboBox = new System.Windows.Forms.ComboBox();
            this.sUnitComboBox = new System.Windows.Forms.ComboBox();
            this.uUnitComboBox = new System.Windows.Forms.ComboBox();
            this.nuUnitComboBox = new System.Windows.Forms.ComboBox();
            this.wLabel = new System.Windows.Forms.Label();
            this.cnuLabel = new System.Windows.Forms.Label();
            this.cpLabel = new System.Windows.Forms.Label();
            this.hLabel = new System.Windows.Forms.Label();
            this.sLabel = new System.Windows.Forms.Label();
            this.uLabel = new System.Windows.Forms.Label();
            this.nuLabel = new System.Windows.Forms.Label();
            this.wTextBox = new System.Windows.Forms.TextBox();
            this.cnuTextBox = new System.Windows.Forms.TextBox();
            this.cpTextBox = new System.Windows.Forms.TextBox();
            this.hTextBox = new System.Windows.Forms.TextBox();
            this.sTextBox = new System.Windows.Forms.TextBox();
            this.uTextBox = new System.Windows.Forms.TextBox();
            this.nuTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tUnitComboBox = new System.Windows.Forms.ComboBox();
            this.pUnitComboBox = new System.Windows.Forms.ComboBox();
            this.siCheckBox = new System.Windows.Forms.CheckBox();
            this.anotherCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pTextBox
            // 
            this.pTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pTextBox.Location = new System.Drawing.Point(31, 19);
            this.pTextBox.Name = "pTextBox";
            this.pTextBox.Size = new System.Drawing.Size(108, 20);
            this.pTextBox.TabIndex = 0;
            this.pTextBox.TextChanged += new System.EventHandler(this.pTextBox_TextChanged);
            // 
            // tTextBox
            // 
            this.tTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tTextBox.Location = new System.Drawing.Point(31, 45);
            this.tTextBox.Name = "tTextBox";
            this.tTextBox.Size = new System.Drawing.Size(108, 20);
            this.tTextBox.TabIndex = 1;
            this.tTextBox.TextChanged += new System.EventHandler(this.pTextBox_TextChanged);
            // 
            // pLabel
            // 
            this.pLabel.AutoSize = true;
            this.pLabel.Location = new System.Drawing.Point(6, 22);
            this.pLabel.Name = "pLabel";
            this.pLabel.Size = new System.Drawing.Size(19, 13);
            this.pLabel.TabIndex = 2;
            this.pLabel.Text = "p=";
            // 
            // TLabel
            // 
            this.TLabel.AutoSize = true;
            this.TLabel.Location = new System.Drawing.Point(6, 48);
            this.TLabel.Name = "TLabel";
            this.TLabel.Size = new System.Drawing.Size(20, 13);
            this.TLabel.TabIndex = 3;
            this.TLabel.Text = "T=";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tsUnitComboBox);
            this.groupBox1.Controls.Add(this.psUnitComboBox);
            this.groupBox1.Controls.Add(this.tsTextBox);
            this.groupBox1.Controls.Add(this.psTextBox);
            this.groupBox1.Controls.Add(this.tsLabel);
            this.groupBox1.Controls.Add(this.p_sLabel);
            this.groupBox1.Location = new System.Drawing.Point(243, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 76);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "область 4";
            // 
            // tsUnitComboBox
            // 
            this.tsUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tsUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsUnitComboBox.FormattingEnabled = true;
            this.tsUnitComboBox.Items.AddRange(new object[] {
            "°C",
            "°K"});
            this.tsUnitComboBox.Location = new System.Drawing.Point(150, 44);
            this.tsUnitComboBox.Name = "tsUnitComboBox";
            this.tsUnitComboBox.Size = new System.Drawing.Size(74, 21);
            this.tsUnitComboBox.TabIndex = 5;
            this.tsUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // psUnitComboBox
            // 
            this.psUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.psUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.psUnitComboBox.FormattingEnabled = true;
            this.psUnitComboBox.Items.AddRange(new object[] {
            "МПа",
            "кгс/см^2"});
            this.psUnitComboBox.Location = new System.Drawing.Point(150, 18);
            this.psUnitComboBox.Name = "psUnitComboBox";
            this.psUnitComboBox.Size = new System.Drawing.Size(74, 21);
            this.psUnitComboBox.TabIndex = 4;
            this.psUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // tsTextBox
            // 
            this.tsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsTextBox.Location = new System.Drawing.Point(36, 45);
            this.tsTextBox.Name = "tsTextBox";
            this.tsTextBox.ReadOnly = true;
            this.tsTextBox.Size = new System.Drawing.Size(108, 20);
            this.tsTextBox.TabIndex = 3;
            // 
            // psTextBox
            // 
            this.psTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.psTextBox.Location = new System.Drawing.Point(36, 19);
            this.psTextBox.Name = "psTextBox";
            this.psTextBox.ReadOnly = true;
            this.psTextBox.Size = new System.Drawing.Size(108, 20);
            this.psTextBox.TabIndex = 2;
            // 
            // tsLabel
            // 
            this.tsLabel.AutoSize = true;
            this.tsLabel.Location = new System.Drawing.Point(5, 48);
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(25, 13);
            this.tsLabel.TabIndex = 1;
            this.tsLabel.Text = "Ts=";
            // 
            // p_sLabel
            // 
            this.p_sLabel.AutoSize = true;
            this.p_sLabel.Location = new System.Drawing.Point(6, 22);
            this.p_sLabel.Name = "p_sLabel";
            this.p_sLabel.Size = new System.Drawing.Size(24, 13);
            this.p_sLabel.TabIndex = 0;
            this.p_sLabel.Text = "ps=";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.wUnitComboBox);
            this.groupBox2.Controls.Add(this.cnuUnitComboBox);
            this.groupBox2.Controls.Add(this.cpUnitComboBox);
            this.groupBox2.Controls.Add(this.hUnitComboBox);
            this.groupBox2.Controls.Add(this.sUnitComboBox);
            this.groupBox2.Controls.Add(this.uUnitComboBox);
            this.groupBox2.Controls.Add(this.nuUnitComboBox);
            this.groupBox2.Controls.Add(this.wLabel);
            this.groupBox2.Controls.Add(this.cnuLabel);
            this.groupBox2.Controls.Add(this.cpLabel);
            this.groupBox2.Controls.Add(this.hLabel);
            this.groupBox2.Controls.Add(this.sLabel);
            this.groupBox2.Controls.Add(this.uLabel);
            this.groupBox2.Controls.Add(this.nuLabel);
            this.groupBox2.Controls.Add(this.wTextBox);
            this.groupBox2.Controls.Add(this.cnuTextBox);
            this.groupBox2.Controls.Add(this.cpTextBox);
            this.groupBox2.Controls.Add(this.hTextBox);
            this.groupBox2.Controls.Add(this.sTextBox);
            this.groupBox2.Controls.Add(this.uTextBox);
            this.groupBox2.Controls.Add(this.nuTextBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(461, 208);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Основные";
            // 
            // wUnitComboBox
            // 
            this.wUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wUnitComboBox.FormattingEnabled = true;
            this.wUnitComboBox.Items.AddRange(new object[] {
            "м/с"});
            this.wUnitComboBox.Location = new System.Drawing.Point(372, 174);
            this.wUnitComboBox.Name = "wUnitComboBox";
            this.wUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.wUnitComboBox.TabIndex = 27;
            this.wUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // cnuUnitComboBox
            // 
            this.cnuUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cnuUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cnuUnitComboBox.FormattingEnabled = true;
            this.cnuUnitComboBox.Items.AddRange(new object[] {
            "кДж/(кг∙°K)",
            "ккал/(кг∙°K)"});
            this.cnuUnitComboBox.Location = new System.Drawing.Point(372, 148);
            this.cnuUnitComboBox.Name = "cnuUnitComboBox";
            this.cnuUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.cnuUnitComboBox.TabIndex = 26;
            this.cnuUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // cpUnitComboBox
            // 
            this.cpUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cpUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cpUnitComboBox.FormattingEnabled = true;
            this.cpUnitComboBox.Items.AddRange(new object[] {
            "кДж/(кг∙°K)",
            "ккал/(кг∙°K)"});
            this.cpUnitComboBox.Location = new System.Drawing.Point(372, 122);
            this.cpUnitComboBox.Name = "cpUnitComboBox";
            this.cpUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.cpUnitComboBox.TabIndex = 25;
            this.cpUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // hUnitComboBox
            // 
            this.hUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hUnitComboBox.FormattingEnabled = true;
            this.hUnitComboBox.Items.AddRange(new object[] {
            "кДж/кг",
            "ккал/кг"});
            this.hUnitComboBox.Location = new System.Drawing.Point(372, 96);
            this.hUnitComboBox.Name = "hUnitComboBox";
            this.hUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.hUnitComboBox.TabIndex = 24;
            this.hUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // sUnitComboBox
            // 
            this.sUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sUnitComboBox.FormattingEnabled = true;
            this.sUnitComboBox.Items.AddRange(new object[] {
            "кДж/(кг∙°K)",
            "ккал/(кг∙°K)"});
            this.sUnitComboBox.Location = new System.Drawing.Point(372, 70);
            this.sUnitComboBox.Name = "sUnitComboBox";
            this.sUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.sUnitComboBox.TabIndex = 23;
            this.sUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // uUnitComboBox
            // 
            this.uUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uUnitComboBox.FormattingEnabled = true;
            this.uUnitComboBox.Items.AddRange(new object[] {
            "кДж/кг",
            "ккал/кг"});
            this.uUnitComboBox.Location = new System.Drawing.Point(372, 44);
            this.uUnitComboBox.Name = "uUnitComboBox";
            this.uUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.uUnitComboBox.TabIndex = 22;
            this.uUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // nuUnitComboBox
            // 
            this.nuUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nuUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nuUnitComboBox.FormattingEnabled = true;
            this.nuUnitComboBox.Items.AddRange(new object[] {
            "м^3/кг"});
            this.nuUnitComboBox.Location = new System.Drawing.Point(372, 18);
            this.nuUnitComboBox.Name = "nuUnitComboBox";
            this.nuUnitComboBox.Size = new System.Drawing.Size(83, 21);
            this.nuUnitComboBox.TabIndex = 21;
            this.nuUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // wLabel
            // 
            this.wLabel.AutoSize = true;
            this.wLabel.Location = new System.Drawing.Point(118, 178);
            this.wLabel.Name = "wLabel";
            this.wLabel.Size = new System.Drawing.Size(107, 13);
            this.wLabel.TabIndex = 13;
            this.wLabel.Text = "Скорость звука, w=";
            // 
            // cnuLabel
            // 
            this.cnuLabel.AutoSize = true;
            this.cnuLabel.Location = new System.Drawing.Point(7, 152);
            this.cnuLabel.Name = "cnuLabel";
            this.cnuLabel.Size = new System.Drawing.Size(218, 13);
            this.cnuLabel.TabIndex = 12;
            this.cnuLabel.Text = "Удельная изохорная теплоемкость, cnu=";
            // 
            // cpLabel
            // 
            this.cpLabel.AutoSize = true;
            this.cpLabel.Location = new System.Drawing.Point(12, 126);
            this.cpLabel.Name = "cpLabel";
            this.cpLabel.Size = new System.Drawing.Size(213, 13);
            this.cpLabel.TabIndex = 11;
            this.cpLabel.Text = "Удельная изобарная теплоемкость, cp=";
            // 
            // hLabel
            // 
            this.hLabel.AutoSize = true;
            this.hLabel.Location = new System.Drawing.Point(94, 100);
            this.hLabel.Name = "hLabel";
            this.hLabel.Size = new System.Drawing.Size(131, 13);
            this.hLabel.TabIndex = 10;
            this.hLabel.Text = "Удельная энтальпия, h=";
            // 
            // sLabel
            // 
            this.sLabel.AutoSize = true;
            this.sLabel.Location = new System.Drawing.Point(101, 74);
            this.sLabel.Name = "sLabel";
            this.sLabel.Size = new System.Drawing.Size(124, 13);
            this.sLabel.TabIndex = 9;
            this.sLabel.Text = "Удельная энтропия, s=";
            // 
            // uLabel
            // 
            this.uLabel.AutoSize = true;
            this.uLabel.Location = new System.Drawing.Point(45, 48);
            this.uLabel.Name = "uLabel";
            this.uLabel.Size = new System.Drawing.Size(180, 13);
            this.uLabel.TabIndex = 8;
            this.uLabel.Text = "Удельная внутренняя энергия, u=";
            // 
            // nuLabel
            // 
            this.nuLabel.AutoSize = true;
            this.nuLabel.Location = new System.Drawing.Point(106, 22);
            this.nuLabel.Name = "nuLabel";
            this.nuLabel.Size = new System.Drawing.Size(119, 13);
            this.nuLabel.TabIndex = 7;
            this.nuLabel.Text = "Удельный объем, nu=";
            // 
            // wTextBox
            // 
            this.wTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wTextBox.Location = new System.Drawing.Point(231, 175);
            this.wTextBox.Name = "wTextBox";
            this.wTextBox.ReadOnly = true;
            this.wTextBox.Size = new System.Drawing.Size(135, 20);
            this.wTextBox.TabIndex = 6;
            // 
            // cnuTextBox
            // 
            this.cnuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cnuTextBox.Location = new System.Drawing.Point(231, 149);
            this.cnuTextBox.Name = "cnuTextBox";
            this.cnuTextBox.ReadOnly = true;
            this.cnuTextBox.Size = new System.Drawing.Size(135, 20);
            this.cnuTextBox.TabIndex = 5;
            // 
            // cpTextBox
            // 
            this.cpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cpTextBox.Location = new System.Drawing.Point(231, 123);
            this.cpTextBox.Name = "cpTextBox";
            this.cpTextBox.ReadOnly = true;
            this.cpTextBox.Size = new System.Drawing.Size(135, 20);
            this.cpTextBox.TabIndex = 4;
            // 
            // hTextBox
            // 
            this.hTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hTextBox.Location = new System.Drawing.Point(231, 97);
            this.hTextBox.Name = "hTextBox";
            this.hTextBox.ReadOnly = true;
            this.hTextBox.Size = new System.Drawing.Size(136, 20);
            this.hTextBox.TabIndex = 3;
            // 
            // sTextBox
            // 
            this.sTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sTextBox.Location = new System.Drawing.Point(231, 71);
            this.sTextBox.Name = "sTextBox";
            this.sTextBox.ReadOnly = true;
            this.sTextBox.Size = new System.Drawing.Size(136, 20);
            this.sTextBox.TabIndex = 2;
            // 
            // uTextBox
            // 
            this.uTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uTextBox.Location = new System.Drawing.Point(231, 45);
            this.uTextBox.Name = "uTextBox";
            this.uTextBox.ReadOnly = true;
            this.uTextBox.Size = new System.Drawing.Size(135, 20);
            this.uTextBox.TabIndex = 1;
            // 
            // nuTextBox
            // 
            this.nuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nuTextBox.Location = new System.Drawing.Point(231, 19);
            this.nuTextBox.Name = "nuTextBox";
            this.nuTextBox.ReadOnly = true;
            this.nuTextBox.Size = new System.Drawing.Size(135, 20);
            this.nuTextBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tUnitComboBox);
            this.groupBox3.Controls.Add(this.pUnitComboBox);
            this.groupBox3.Controls.Add(this.pTextBox);
            this.groupBox3.Controls.Add(this.tTextBox);
            this.groupBox3.Controls.Add(this.pLabel);
            this.groupBox3.Controls.Add(this.TLabel);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(225, 76);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Аргументы";
            // 
            // tUnitComboBox
            // 
            this.tUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tUnitComboBox.FormattingEnabled = true;
            this.tUnitComboBox.Items.AddRange(new object[] {
            "°C",
            "°K"});
            this.tUnitComboBox.Location = new System.Drawing.Point(145, 45);
            this.tUnitComboBox.Name = "tUnitComboBox";
            this.tUnitComboBox.Size = new System.Drawing.Size(74, 21);
            this.tUnitComboBox.TabIndex = 7;
            this.tUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // pUnitComboBox
            // 
            this.pUnitComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pUnitComboBox.FormattingEnabled = true;
            this.pUnitComboBox.Items.AddRange(new object[] {
            "МПа",
            "кгс/см^2"});
            this.pUnitComboBox.Location = new System.Drawing.Point(145, 19);
            this.pUnitComboBox.Name = "pUnitComboBox";
            this.pUnitComboBox.Size = new System.Drawing.Size(74, 21);
            this.pUnitComboBox.TabIndex = 6;
            this.pUnitComboBox.SelectedIndexChanged += new System.EventHandler(this.unitComboBox_SelectedIndexChanged);
            // 
            // siCheckBox
            // 
            this.siCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.siCheckBox.AutoSize = true;
            this.siCheckBox.Location = new System.Drawing.Point(12, 308);
            this.siCheckBox.Name = "siCheckBox";
            this.siCheckBox.Size = new System.Drawing.Size(32, 23);
            this.siCheckBox.TabIndex = 9;
            this.siCheckBox.Text = "СИ";
            this.siCheckBox.UseVisualStyleBackColor = true;
            this.siCheckBox.Click += new System.EventHandler(this.siButton_Click);
            // 
            // anotherCheckBox
            // 
            this.anotherCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.anotherCheckBox.AutoSize = true;
            this.anotherCheckBox.Location = new System.Drawing.Point(50, 308);
            this.anotherCheckBox.Name = "anotherCheckBox";
            this.anotherCheckBox.Size = new System.Drawing.Size(48, 23);
            this.anotherCheckBox.TabIndex = 10;
            this.anotherCheckBox.Text = "Дугое";
            this.anotherCheckBox.UseVisualStyleBackColor = true;
            this.anotherCheckBox.Click += new System.EventHandler(this.anotherButton_Click);
            // 
            // IF97CalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 341);
            this.Controls.Add(this.anotherCheckBox);
            this.Controls.Add(this.siCheckBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "IF97CalcForm";
            this.Text = "IF97";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pTextBox;
        private System.Windows.Forms.TextBox tTextBox;
        private System.Windows.Forms.Label pLabel;
        private System.Windows.Forms.Label TLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tsTextBox;
        private System.Windows.Forms.TextBox psTextBox;
        private System.Windows.Forms.Label tsLabel;
        private System.Windows.Forms.Label p_sLabel;
        private System.Windows.Forms.Label wLabel;
        private System.Windows.Forms.Label cnuLabel;
        private System.Windows.Forms.Label cpLabel;
        private System.Windows.Forms.Label hLabel;
        private System.Windows.Forms.Label sLabel;
        private System.Windows.Forms.Label uLabel;
        private System.Windows.Forms.Label nuLabel;
        private System.Windows.Forms.TextBox wTextBox;
        private System.Windows.Forms.TextBox cnuTextBox;
        private System.Windows.Forms.TextBox cpTextBox;
        private System.Windows.Forms.TextBox hTextBox;
        private System.Windows.Forms.TextBox sTextBox;
        private System.Windows.Forms.TextBox uTextBox;
        private System.Windows.Forms.TextBox nuTextBox;
        private System.Windows.Forms.ComboBox pUnitComboBox;
        private System.Windows.Forms.ComboBox tUnitComboBox;
        private System.Windows.Forms.ComboBox tsUnitComboBox;
        private System.Windows.Forms.ComboBox psUnitComboBox;
        private System.Windows.Forms.ComboBox sUnitComboBox;
        private System.Windows.Forms.ComboBox uUnitComboBox;
        private System.Windows.Forms.ComboBox nuUnitComboBox;
        private System.Windows.Forms.ComboBox hUnitComboBox;
        private System.Windows.Forms.ComboBox wUnitComboBox;
        private System.Windows.Forms.ComboBox cnuUnitComboBox;
        private System.Windows.Forms.ComboBox cpUnitComboBox;
        private System.Windows.Forms.CheckBox siCheckBox;
        private System.Windows.Forms.CheckBox anotherCheckBox;
    }
}

