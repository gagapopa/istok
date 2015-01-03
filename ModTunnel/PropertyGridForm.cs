using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.Modules;
using System.Resources;

namespace COTES.ISTOK.Modules.Tunnel
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
            ResourceManager resource = COTES.ISTOK.Modules.Tunnel.Properties.Resources.ResourceManager;
            foreach (DataGridViewColumn column in propertyDataGridView.Columns)
            {
                String name = resource.GetString(column.Name + "ColumnHeader");
                if (name != null && name != "")
                    column.HeaderText = name;
            }
        }
    }
}