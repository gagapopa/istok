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
    public partial class MessagesForm : Form
    {
        public MessagesForm()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.FromHandle(Properties.Resources.bell.GetHicon());
        }

        public void ShowMessages(Control sender, Message[] messages)
        {
            if (InvokeRequired) BeginInvoke((Action<Control, Message[]>)ShowMessages, sender, messages);
            else
            {
                foreach (Message mess in messages)
                {
                    DataGridViewRow gridRow = dgvMessages.Rows[dgvMessages.Rows.Add()];
                    gridRow.Tag = mess;
                    gridRow.Cells[clmType.Name].Value = mess.Category;
                    gridRow.Cells[clmMessage.Name].Value = mess.Text;
                    gridRow.Cells[timeColumn.Name].Value = mess.Time;
                }
                Show();
                Activate();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            dgvMessages.Rows.Clear();
            this.Hide();
        }
    }
}
