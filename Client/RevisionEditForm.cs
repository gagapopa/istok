using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно для редактирования ревизий
    /// </summary>
    partial class RevisionEditForm : BaseAsyncWorkForm
    {
        public RevisionEditForm(StructureProvider strucProvider)
            :base(strucProvider)
        {
            removeList = new List<RevisionInfo>();
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadRevision();
            base.OnLoad(e);
        }

        /// <summary>
        /// Загрузить список ревизий с сервера
        /// </summary>
        private void LoadRevision()
        {
            var res = strucProvider.Session.Revisions;
            ShowRevisions(res);
            //AsyncOperationWatcher<IEnumerable<RevisionInfo>> watcher = strucProvider.Session.Revisions;
            //watcher.AddValueRecivedHandler(ShowRevisions);
            //RunWatcher(watcher);
        }

        /// <summary>
        /// Отобразить загруженные ревизии на форме
        /// </summary>
        /// <param name="revisions"></param>
        private void ShowRevisions(IEnumerable<RevisionInfo> revisions)
        {
            if (InvokeRequired) Invoke((Action<IEnumerable<RevisionInfo>>)ShowRevisions, revisions);
            else
            {
                removeList.Clear();
                revisionTreeView.Nodes.Clear();
                foreach (var revision in revisions)
                {
                    TreeNode treeNode = new TreeNode(revision.ToString());
                    treeNode.Tag = revision;
                    revisionTreeView.Nodes.Add(treeNode);
                }
                changed = false;
                ShowRevision(null);
            }
        }

        private void revisionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RevisionInfo revision = e.Node.Tag as RevisionInfo;

            ShowRevision(revision);
        }

        private void ShowRevision(RevisionInfo revision)
        {
            if (revision != null)
            {
                revisionBriefTextBox.Text = revision.Brief;
                revisionCommentTextBox.Text = revision.Comment;
                if (revision.Time < revisionTimeFromDateTimePicker.MinDate)
                    revisionTimeFromDateTimePicker.Value = revisionTimeFromDateTimePicker.MinDate;
                else
                    revisionTimeFromDateTimePicker.Value = revision.Time;
            }
            else
            {
                revisionBriefTextBox.Text = String.Empty;
                revisionCommentTextBox.Text = String.Empty;
                revisionTimeFromDateTimePicker.Value = revisionTimeFromDateTimePicker.MinDate;
            }
            bool readOnly = revision == null || revision.Equals(RevisionInfo.Default);

            revisionBriefTextBox.ReadOnly =
                revisionCommentTextBox.ReadOnly = readOnly;
            revisionTimeFromDateTimePicker.Enabled = !readOnly;
            removeRevisionButton.Enabled = !readOnly;
        }

        private bool CommitCurrent()
        {
            RevisionInfo revision;

            if (revisionTreeView.SelectedNode != null
                && (revision = revisionTreeView.SelectedNode.Tag as RevisionInfo) != null
                && !revision.Equals(RevisionInfo.Default))
            {
                if (String.IsNullOrEmpty(revisionBriefTextBox.Text))
                {
                    MessageBox.Show(
                        "Для ревизии требуется ввести краткое описание",
                        "Не полные данные",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                revision.Time = revisionTimeFromDateTimePicker.Value.Date;
                revision.Brief = revisionBriefTextBox.Text;
                revision.Comment = revisionCommentTextBox.Text;

                revisionTreeView.SelectedNode.Text = revision.ToString();
            }
            return true;
        }

        bool changed = false;

        private void revisionTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            bool hasChanges = CurrentRevisionChanged();

            e.Cancel = !CommitCurrent();

            changed |= !e.Cancel && hasChanges;
        }

        private void addRevisionButton_Click(object sender, EventArgs e)
        {
            RevisionInfo revision = new RevisionInfo { Time = DateTime.Now };

            TreeNode treeNode = new TreeNode(revision.ToString());
            treeNode.Tag = revision;

            revisionTreeView.Nodes.Add(treeNode);

            revisionTreeView.SelectedNode = treeNode;
        }

        List<RevisionInfo> removeList;

        private void removeRevisionButton_Click(object sender, EventArgs e)
        {
            RevisionInfo revision;

            if (revisionTreeView.SelectedNode!=null&&(revision=revisionTreeView.SelectedNode.Tag as RevisionInfo)!=null)
            {
                if (revision.ID != 0)
                    removeList.Add(revision);
                revisionTreeView.Nodes.Remove(revisionTreeView.SelectedNode);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private bool SaveAll()
        {
            if (CommitCurrent())
            {
                List<RevisionInfo> updateList = GetChangedRevisions();
                int[] removeArray = null;

                if (removeList.Count > 0)
                {
                    var rem = from elem in removeList
                              select elem.ID;
                    //strucProvider.Session.RemoveRevisions(rem.ToArray());
                    removeArray = rem.ToArray();
                }
                //strucProvider.Session.UpdateRevisions(updateList.ToArray());
                strucProvider.Session.UpdateRevisions(removeArray, updateList.ToArray());
                LoadRevision();

                return true;
            }
            return false;
        }

        private List<RevisionInfo> GetChangedRevisions()
        {
            RevisionInfo revision;

            List<RevisionInfo> updateList = new List<RevisionInfo>();

            foreach (TreeNode node in revisionTreeView.Nodes)
            {
                if ((revision = node.Tag as RevisionInfo) != null)
                    updateList.Add(revision);
            }
            return updateList;
        }

        private bool CurrentRevisionChanged()
        {
            RevisionInfo revision;

            if (revisionTreeView.SelectedNode != null
                && (revision = revisionTreeView.SelectedNode.Tag as RevisionInfo) != null
                && !revision.Equals(RevisionInfo.Default))
            {
                return !DateTime.Equals(revision.Time, revisionTimeFromDateTimePicker.Value.Date)
                    || !String.Equals(revision.Brief, revisionBriefTextBox.Text)
                    || !String.Equals(revision.Comment, revisionCommentTextBox.Text);
            }
            return false;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            LoadRevision();
        }

        private bool PromtClose()
        {
            bool cancel = false;
            if (changed || CurrentRevisionChanged())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения перед выходом", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    cancel = !SaveAll();
                else if (result == DialogResult.Cancel)
                    cancel = true;
            }
            return cancel;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
                this.Close();
        }

        private void RevisionEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = PromtClose();
        }
    }
}
