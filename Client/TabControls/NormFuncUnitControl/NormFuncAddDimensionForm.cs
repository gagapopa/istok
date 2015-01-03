using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class NormFuncAddDimensionForm : Form
    {
        public NormFuncAddDimensionForm()
        {
            InitializeComponent();
        }

        //public MultiDimensionalTable MDTable { get; set; }
        public string DimensionName { get; set; }
        public string DimensionMeasure { get; set; }
        public double DimensionValue { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                DimensionName = txtName.Text;
                DimensionMeasure = txtMeasure.Text;
                DimensionValue = double.Parse(txtValue.Text);
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат значения.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
