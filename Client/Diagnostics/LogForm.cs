using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class LogForm : Form
    {
        MessageManager messageManager = null;
        //DataTable table = null;

        public LogForm()
        {
            InitializeComponent();

            //table = new DataTable();
            //table.Columns.Add("Time");
            //table.Columns.Add("Text");
            //table.Columns.Add("Node");
            //table.Columns.Add("");

            //dgv.DataSource = table;
        }

        public MessageManager MessageManager
        {
            get { return messageManager; }
            set
            {
                messageManager = value;

                if (messageManager != null)
                    messageManager.OnStateChange += new InfoMessageStateHandler(StateChanged);
            }
        }

        private void StateChanged(object sender, InfoMessageStateEventArgs e)
        {
            DataGridViewRow row;
            //TreeNode ptr;
            Color color;
            //string str = "";
            //DataRow row = table.NewRow();
            
            if (e.Message == null) return;

            if (dgv.InvokeRequired)
                dgv.Invoke((Action<object, InfoMessageStateEventArgs>)StateChanged, sender, e);
            else
            {
                row = dgv.Rows[dgv.Rows.Add()];

                row.Cells["clmTime"].Value = e.Message.Time.ToString();
                row.Cells["clmNode"].Value = e.Message.NodeText;
                row.Cells["clmText"].Value = e.Message.FormatMessageState();
                row.Tag = e.Message;
                if (!e.Message.Committed)
                {
                    color = GetStateColor(e.Message);
                    foreach (DataGridViewCell item in row.Cells)
                    {
                        item.Style.BackColor = color;
                    }
                }

                //dgv.Rows.Add(row);
                //table.Rows.Add(row);
            }
        }

        private Color GetStateColor(InfoMessage mes)
        {
            Color color = Color.Empty;

            switch (mes.State)
            {
                case InfoMessageState.NodeDataError:
                    color = Color.LightBlue;
                    break;
                case InfoMessageState.NodeDataWarning:
                    color = Color.Yellow;
                    break;
                case InfoMessageState.NodeOffline:
                    color = Color.Pink;
                    break;
                case InfoMessageState.NodeOnline:
                    color = Color.LightGreen;
                    break;
                case InfoMessageState.NodeDisabled:
                    color = Color.Brown;
                    break;
            }

            return color;
        }

        private void UpdateDGV()
        {
            InfoMessage tag;
            Color color;

            foreach (DataGridViewRow item in dgv.Rows)
            {
                tag = item.Tag as InfoMessage;

                if (tag != null && !messageManager.Contains(tag))
                {
                    item.Tag = null;
                    tag = null;
                }

                foreach (DataGridViewCell cell in item.Cells)
                {
                    color = Color.Empty;

                    if (tag == null)
                        cell.Style.BackColor = color;
                    else
                    {
                        if (!tag.Committed)
                        {
                            color = GetStateColor(tag);
                        }
                        cell.Style.BackColor = color;
                    }
                }
            }
        }

        //private string FormatMessageState(InfoMessageState infoMessageState)
        //{
        //    switch (infoMessageState)
        //    {
        //        case InfoMessageState.NodeOnline:
        //            return "Узел перешел в состояние 'Онлайн'";
        //        case InfoMessageState.NodeOffline:
        //            return "Узел перешел в состояние 'Оффлайн'";
        //        case InfoMessageState.NodeDataWarning:
        //            return "Узел перешел в состояние 'Данные: Предупреждение'";
        //        case InfoMessageState.NodeDataError:
        //            return "Узел перешел в состояние 'Данные: Ошибка'";
        //    }

        //    return "";
        //}

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            this.Hide();
        }

        private void commitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (messageManager != null)
            {
                messageManager.CommitAll();
                UpdateDGV();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
        }
    }
}