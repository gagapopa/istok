using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования пользователей и групп пользователей
    /// </summary>
    partial class UserFindForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        List<UserNode> userNodeList;
        List<GroupNode> groupNodeList;

        public UserFindForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadUsers();
            LoadGroups();
            base.OnLoad(e);
        }

        /// <summary>
        /// Загрузить и отобразить список групп пользователя
        /// </summary>
        private async void LoadGroups()
        {
            if (InvokeRequired)
                Invoke((Action)LoadGroups);
            else
            {
                groupNodeList = new List<GroupNode>();
                groupDataGridView.Rows.Clear();
                try
                {
                    var res = await Task.Factory.StartNew(() => strucProvider.Session.GetGroupNodes());
                    UserNodeReceive(res);
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка загрузки группы пользователей.", exc);
                    ShowError(exc);
                }
            }
        }

        /// <summary>
        /// Загрузить и отобразить список пользователей
        /// </summary>
        /// <returns>Вотчер</returns>
        private async void LoadUsers()
        {
            if (InvokeRequired)
                Invoke((Action)LoadUsers);
            else
            {
                userNodeList = new List<UserNode>();
                userDataGridView.Rows.Clear();
                try
                {
                    var res = await Task.Factory.StartNew(() => strucProvider.Session.GetUserNodes());
                    UserNodeReceive(res);
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка загрузки пользователей.", exc);
                    ShowError(exc);
                }
            }
        }

        /// <summary>
        /// Получения пользователя и группы из асинхронной операции
        /// </summary>
        /// <param name="x">Пользователь или группа</param>
        private void UserNodeReceive(Object x)
        {
            if (InvokeRequired)
                BeginInvoke((Action<Object>)UserNodeReceive, x);
            else
            {
                UserNode[] userNodeArray;
                GroupNode[] groupNodeArray;
                if ((userNodeArray = x as UserNode[]) != null)
                {
                    foreach (var userNode in userNodeArray)
                    {
                        int index = userDataGridView.Rows.Add();
                        userNodeList.Add(userNode);
                        DataGridViewRow row = userDataGridView.Rows[index];
                        row.Tag = userNode;
                        row.Cells[Column2.Index].Value = userNode.Text;
                    }
                }
                else if ((groupNodeArray = x as GroupNode[]) != null)
                {
                    foreach (var groupNode in groupNodeArray)
                    {
                        int index = groupDataGridView.Rows.Add();
                        groupNodeList.Add(groupNode);
                        DataGridViewRow row = groupDataGridView.Rows[index];
                        row.Tag = groupNode;
                        row.Cells[groupNameColumn.Index].Value = groupNode.Text;
                        row.Cells[groupDescriptionColumn.Index].Value = groupNode.Description;
                    }
                }
            }
        }

        UTypeNode[] unitTypeNodeList;
        private void editUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserNode userNode = userDataGridView.CurrentRow.Tag as UserNode;
            if (userNode != null)
            {
                if (unitTypeNodeList == null)
                {
                    unitTypeNodeList = strucProvider.Session.Types;
                }
                UserEditForm userEditForm = new UserEditForm(userNode, unitTypeNodeList, groupNodeList, false);
                userEditForm.FormClosed += new FormClosedEventHandler(userEditForm_FormClosed);
                userEditForm.MdiParent = MdiParent;
                userEditForm.Show();
            }
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserNode userNode = new UserNode();
            if (unitTypeNodeList == null)
            {
                unitTypeNodeList = strucProvider.Session.Types;
            }
            UserEditForm userEditForm = new UserEditForm(userNode, unitTypeNodeList, groupNodeList, true);
            userEditForm.FormClosed += new FormClosedEventHandler(userEditForm_FormClosed);
            userEditForm.MdiParent = MdiParent;
            userEditForm.Show();
        }

        void userEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UserEditForm userEditForm = sender as UserEditForm;

            if (userEditForm != null && userEditForm.DialogResult == DialogResult.OK)
            {
                try
                {
                    //await Task.Factory.StartNew(() =>
                    //{
                        if (userEditForm.AddNew)
                            strucProvider.Session.AddUserNode(userEditForm.EditingNode);
                        else strucProvider.Session.UpdateUserNode(userEditForm.EditingNode);
                    //});
                    LoadUsers();
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка редактирования пользователя.", exc);
                    ShowError(exc);
                }
            }
        }

        private void removeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<UserNode> userNodeToRemove = new List<UserNode>();
            UserNode userNode;
            foreach (DataGridViewCell selectedCell in userDataGridView.SelectedCells)
            {
                DataGridViewRow gridRow = userDataGridView.Rows[selectedCell.RowIndex];
                if ((userNode = gridRow.Tag as UserNode) != null
                    && !userNodeToRemove.Contains(userNode))
                    userNodeToRemove.Add(userNode);
            }

            if (userNodeToRemove.Count > 0)
            {
                String message;
                if (userNodeToRemove.Count == 1)
                {
                    message = String.Format("Удалить пользователя {0}?", userNodeToRemove[0].Text);
                }
                else
                {
                    message = String.Format("Удалить {0} пользователей?", userNodeToRemove.Count);
                }

                if (MessageBox.Show(message, "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        //await Task.Factory.StartNew(() =>
                        //{
                            foreach (var item in userNodeToRemove)
                                strucProvider.Session.RemoveUserNode(item);
                        //});
                        LoadUsers();
                    }
                    catch (Exception exc)
                    {
                        log.WarnException("Ошибка удаления пользователя.", exc);
                        ShowError(exc);
                    }
                }
            }
        }

        private void editGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupDataGridView.CurrentRow != null)
            {
                GroupNode groupNode = groupDataGridView.CurrentRow.Tag as GroupNode;
                if (groupNode != null)
                {
                    GroupEditForm groupEditForm = new GroupEditForm(groupNode, false);
                    groupEditForm.FormClosed += new FormClosedEventHandler(groupEditForm_FormClosed);
                    groupEditForm.MdiParent = MdiParent;
                    groupEditForm.Show();
                }
            }
        }

        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupNode groupNode = new GroupNode();
            GroupEditForm groupEditForm = new GroupEditForm(groupNode, true);
            groupEditForm.FormClosed += new FormClosedEventHandler(groupEditForm_FormClosed);
            groupEditForm.MdiParent = MdiParent;
            groupEditForm.Show();
        }

         void groupEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GroupEditForm userEditForm = sender as GroupEditForm;

            if (userEditForm != null && userEditForm.DialogResult == DialogResult.OK)
            {
                try
                {
                    //await Task.Factory.StartNew(() =>
                    //       {
                               if (userEditForm.AddNew)
                                   strucProvider.Session.AddGroupNode(userEditForm.EditingNode);
                               else strucProvider.Session.UpdateGroupNode(userEditForm.EditingNode);
                           //});
                    LoadGroups();
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка редактирования группы пользователей.", exc);
                    ShowError(exc);
                }
            }
        }

        private  void removeGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<GroupNode> groupNodeToRemove = new List<GroupNode>();
            GroupNode groupNode;
            foreach (DataGridViewCell selectedCell in groupDataGridView.SelectedCells)
            {
                DataGridViewRow gridRow = groupDataGridView.Rows[selectedCell.RowIndex];
                if ((groupNode = gridRow.Tag as GroupNode) != null
                    && !groupNodeToRemove.Contains(groupNode))
                    groupNodeToRemove.Add(groupNode);
            }

            if (groupNodeToRemove.Count > 0)
            {
                String message;
                if (groupNodeToRemove.Count == 1)
                {
                    message = String.Format("Удалить группу {0}?", groupNodeToRemove[0].Text);
                }
                else
                {
                    message = String.Format("Удалить {0} групп?", groupNodeToRemove.Count);
                }

                if (MessageBox.Show(message, "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        var gr = from elem in groupNodeToRemove
                                 select elem.Idnum;
                        //await Task.Factory.StartNew(() => strucProvider.Session.RemoveGroupNode(gr.ToArray()));
                        strucProvider.Session.RemoveGroupNode(gr.ToArray());
                        LoadGroups();
                    }
                    catch (Exception exc)
                    {
                        log.WarnException("Ошибка удаления группы пользователей.", exc);
                        ShowError(exc);
                    }
                }
            }
        }

        private void userContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UserNode userNode;
            List<UserNode> userNodeToRemove = new List<UserNode>();

            editUserToolStripMenuItem.Enabled = userDataGridView.CurrentRow != null;

            foreach (DataGridViewCell selectedCell in userDataGridView.SelectedCells)
            {
                DataGridViewRow gridRow = userDataGridView.Rows[selectedCell.RowIndex];
                if ((userNode = gridRow.Tag as UserNode) != null
                    && !userNodeToRemove.Contains(userNode))
                    userNodeToRemove.Add(userNode);
            }

            removeUserToolStripMenuItem.Enabled = userNodeToRemove.Count > 0;
        }

        private void groupContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            GroupNode groupNode;
            List<GroupNode> groupNodeToRemove = new List<GroupNode>();

            editGroupToolStripMenuItem.Enabled = groupDataGridView.CurrentRow != null;

            foreach (DataGridViewCell selectedCell in groupDataGridView.SelectedCells)
            {
                DataGridViewRow gridRow = groupDataGridView.Rows[selectedCell.RowIndex];
                if ((groupNode = gridRow.Tag as GroupNode) != null
                    && !groupNodeToRemove.Contains(groupNode))
                    groupNodeToRemove.Add(groupNode);
            }

            removeGroupToolStripMenuItem.Enabled = groupNodeToRemove.Count > 0;
        }
    }
}