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
    public partial class SelectTimeForm : Form
    {

        public DateTime Time { get { return dateTimePicker1.Value; } }

        public SelectTimeForm()
        {
            InitializeComponent();
        }
    }
}
