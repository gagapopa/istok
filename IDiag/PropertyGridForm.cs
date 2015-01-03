using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Monitoring
{
    public sealed partial class PropertyGridForm : Form
    {
        private DataTable properties;

        public DataTable Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        public PropertyGridForm()
        {
            InitializeComponent();
        }

        private void PropertyGridForm_Shown(object sender, EventArgs e)
        {
            propertyDataGridView.DataSource = properties;
        }
    }
}