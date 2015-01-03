using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class NormFuncAddBranchForm : Form
    {
        public NormFuncAddBranchForm()
        {
            InitializeComponent();
        }

        public double Value { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Value = double.Parse(txtValue.Text);
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch(FormatException)
            {
                MessageBox.Show("Ошибка формата.", "Ошибка");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
