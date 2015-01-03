using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class ColumnNameForm : Form
    {
        public string ColumnName { get; set; }

        public ColumnNameForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtColumnName.Text))
            {
                ColumnName = txtColumnName.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show("Название не должно быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ColumnNameForm_Load(object sender, EventArgs e)
        {
            txtColumnName.Text = ColumnName;
        }
    }
}
