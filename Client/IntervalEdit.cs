using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class IntervalEdit : Form
    {
        public IEnumerable<IntervalDescription> OriginInterval { get; private set; }

        public List<IntervalDescription> RemovedInterval { get; private set; }

        public List<IntervalDescription> ModifiedIntervals { get; private set; }

        Session session;

        public IntervalEdit(Session session)
        {
            this.session = session;
            InitializeComponent();
        }

        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OriginInterval = await Task.Factory.StartNew(() => session.GetStandardIntervals());
            ModifiedIntervals = new List<IntervalDescription>();
            RemovedInterval = new List<IntervalDescription>();

            ShowIntervals();
        }

        private void ShowIntervals()
        {
            intervalsTreeView.Nodes.Clear();
            foreach (var interval in OriginInterval)
            {
                var node = intervalsTreeView.Nodes.Add(interval.Header);
                node.Checked = interval.IsStandard;
                node.Tag = interval;
            }
        }

        //IntervalDescription selectedDescription;

        private void intervalsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var interval = e.Node.Tag as IntervalDescription;
            //selectedDescription = interval;

            intervalHeaderTextBox.Text = interval.Header;
            intervalStringTextBox.Text = interval.interval.ToString();
        }

        private void intervalsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (intervalsTreeView.SelectedNode != null)
            {
                try
                {
                    var interval = intervalsTreeView.SelectedNode.Tag as IntervalDescription;

                    if (interval != null)
                    {
                        interval.interval = Interval.FromString(intervalStringTextBox.Text);
                        interval.Header = intervalHeaderTextBox.Text;
                        intervalsTreeView.SelectedNode.Text = interval.Header;
                    }
                }
                catch (Exception exc)
                {
                    e.Cancel = true;
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var interval = new IntervalDescription()
            {
                Header = "Новый",
                interval = Interval.Zero
            };
            var node = intervalsTreeView.Nodes.Add(interval.Header);
            node.Tag = interval;
            node.Checked = interval.IsStandard;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var node = intervalsTreeView.SelectedNode;
            var interval = node.Tag as IntervalDescription;

            if (interval != null)
            {
                RemovedInterval.Add(interval);
                intervalsTreeView.Nodes.Remove(node);
            }
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            var args = new TreeViewCancelEventArgs(null, false, TreeViewAction.Unknown);

            if (!args.Cancel)
            {
                if (RemovedInterval.Count > 0)
                {
                    await Task.Factory.StartNew(() => session.RemoveStandardIntervals(RemovedInterval.ToArray()));
                }

                List<IntervalDescription> modified = new List<IntervalDescription>();
                IntervalDescription interval;

                foreach (TreeNode node in intervalsTreeView.Nodes)
                {
                    if ((interval = node.Tag as IntervalDescription) != null)
                    {
                        interval.IsStandard = node.Checked;
                        modified.Add(interval);
                    }
                }              
                	

                await Task.Factory.StartNew(() => session.SaveStandardIntervals(modified.ToArray()));
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
