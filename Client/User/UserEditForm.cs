using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования пользователя
    /// </summary>
    partial class UserEditForm : Form
    {
        /// <summary>
        /// Режим добавления нового пользователя
        /// </summary>
        public bool AddNew { get; protected set; }

        /// <summary>
        /// Режим добавления первого пользователя - администратора
        /// </summary>
        public bool NewAdminMode { get; protected set; }

        /// <summary>
        /// Редактируемый узел пользователя
        /// </summary>
        public UserNode EditingNode { get; protected set; }

        /// <summary>
        /// типы оборудования системы
        /// </summary>
        private UTypeNode[] unitTypeNodes;

        /// <summary>
        /// Список групп пользователей системы
        /// </summary>
        private List<GroupNode> groupNodeList;

        /// <summary>
        /// Указывает изменился ли пароль
        /// </summary>
        private bool passwordChanged = false;

        /// <summary>
        /// Пароль, отображаемый до изменения пароля
        /// </summary>
        private const string passString = "123456";

        public UserEditForm(UserNode userNode,
            UTypeNode[] unitTypeNodes,
            List<GroupNode> groupNodeList, bool addNew)
            : this(userNode, unitTypeNodes, groupNodeList, addNew, false)
        { }

        public UserEditForm(UserNode userNode,
                            UTypeNode[] unitTypeNodes,
                            List<GroupNode> groupNodeList,
                            bool addNew,
                            bool newAdminMode)
            : base()
        {
            InitializeComponent();
            this.EditingNode = userNode.Clone() as UserNode;
            this.unitTypeNodes = unitTypeNodes ?? new UTypeNode[] { };
            this.groupNodeList = groupNodeList ?? new List<GroupNode>();
            this.AddNew = addNew;
            this.NewAdminMode = newAdminMode;

            if (newAdminMode)
            {
                tabControl1.Visible = false;
                this.Height -= tabControl1.Height;
            }

            if (AddNew)
                okButton.Text = "Добавить";
            else okButton.Text = "Изменить";
        }

        protected override void OnLoad(EventArgs e)
        {
            loginTextBox.Text = EditingNode.Text;
            fullNameTextBox.Text = EditingNode.UserFullName;
            positionTextBox.Text = EditingNode.Position;
            if (!AddNew)
                passwordTextBox.Text = passString;
            else passwordChanged = true;
            if (NewAdminMode)
            {
                adminCheckBox.Checked = true;
                adminCheckBox.Enabled = false;
            }
            else adminCheckBox.Checked = EditingNode.IsAdmin;
            structureHideCheckBox.Checked = EditingNode.StructureHide;
            canLockCheckBox.Checked = EditingNode.CanLockValues;

            if (NewAdminMode)
            {
                tabControl1.Visible = false;
                this.MinimumSize = new Size(this.MinimumSize.Width, this.Height - tabControl1.Height);
                this.Height -= tabControl1.Height;
            }

            DisplayUnitTypePrivileges();

            DisplayGroupPrivileges();

            base.OnLoad(e);
        }

        /// <summary>
        /// Построить таблицу прав пользователя по отношению к типам оборудования
        /// </summary>
        private void DisplayUnitTypePrivileges()
        {
            DataTable table = new DataTable();
            table.Columns.Add("typeID", typeof(int));
            table.Columns.Add("type", typeof(String));
            table.Columns.Add("canRead", typeof(bool));
            table.Columns.Add("canWrite", typeof(bool));
            table.Columns.Add("canExecute", typeof(bool));
            foreach (UTypeNode unitTypeNode in unitTypeNodes)
            {
                if (unitTypeNode.Idnum != (int)UnitTypeId.Unknown)
                {
                    DataRow row = table.NewRow();
                    row["type"] = unitTypeNode.Text;
                    int type = unitTypeNode.Idnum;
                    row["typeID"] = type;
                    row["canRead"] = EditingNode.CheckPrivileges(type, Privileges.Read);
                    row["canWrite"] = EditingNode.CheckPrivileges(type, Privileges.Write);
                    row["canExecute"] = EditingNode.CheckPrivileges(type, Privileges.Execute);
                    table.Rows.Add(row);
                }
            }
            privEditDataGridView.DataSource = table;
        }

        /// <summary>
        /// Построить таблицу прав пользователя по отношению групп пользователей
        /// </summary>
        private void DisplayGroupPrivileges()
        {
            DataTable groupTable = new DataTable();
            groupTable.Columns.Add("groupID", typeof(int));
            groupTable.Columns.Add("group", typeof(String));
            groupTable.Columns.Add("canRead", typeof(bool));
            groupTable.Columns.Add("canWrite", typeof(bool));
            groupTable.Columns.Add("canExecute", typeof(bool));

            groupComboBox.Items.Clear();
            groupComboBox.Text = "";
            if (groupNodeList != null)
                foreach (GroupNode groupNode in groupNodeList)
                {
                    if (EditingNode.CheckGroupPrivilegies(groupNode.Idnum, Privileges.Read))
                    {
                        DataRow row = groupTable.NewRow();
                        row["groupID"] = groupNode.Idnum;
                        row["group"] = groupNode.Text;
                        row["canRead"] = true;
                        row["canWrite"] = EditingNode.CheckGroupPrivilegies(groupNode.Idnum, Privileges.Write);
                        row["canExecute"] = EditingNode.CheckGroupPrivilegies(groupNode.Idnum, Privileges.Execute);
                        groupTable.Rows.Add(row);
                    }
                    else groupComboBox.Items.Add(groupNode);
                }
            groupPrivDataGridView.DataSource = groupTable;
        }

        /// <summary>
        /// Обновить редактируемый узел пользователя
        /// </summary>
        public void UpdateNode()
        {
            EditingNode.Text = loginTextBox.Text;
            EditingNode.UserFullName = fullNameTextBox.Text;
            EditingNode.Position = positionTextBox.Text;
            if (passwordChanged)            	
                EditingNode.Password = passwordTextBox.Text;
            EditingNode.IsAdmin = adminCheckBox.Checked;
            EditingNode.StructureHide = structureHideCheckBox.Checked;
            EditingNode.CanLockValues = canLockCheckBox.Checked;

            DataTable privTable = (DataTable)privEditDataGridView.DataSource;
            DataTable groupPrivTable = (DataTable)groupPrivDataGridView.DataSource;

            Privileges priv;
            int id;
            if (privTable != null)
                foreach (DataRow row in privTable.Rows)
                {
                    priv = Privileges.NothingDo;
                    id = (int)row["typeID"];
                    if ((bool)row["canRead"]) priv |= Privileges.Read;
                    if ((bool)row["canWrite"]) priv |= Privileges.Write;
                    if ((bool)row["canExecute"]) priv |= Privileges.Execute;
                    EditingNode.SetPrivileges(id, priv);
                }

            if (groupPrivTable != null)
                foreach (DataRow row in groupPrivTable.Rows)
                {
                    priv = Privileges.NothingDo;
                    id = (int)row["groupID"];
                    if ((bool)row["canRead"]) priv |= Privileges.Read;
                    if ((bool)row["canWrite"]) priv |= Privileges.Write;
                    if ((bool)row["canExecute"]) priv |= Privileges.Execute;
                    EditingNode.SetGroupPrivilegies(id, priv);
                }
        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {
            try
            {
                GroupNode group = (GroupNode)groupComboBox.SelectedItem;

                EditingNode.SetGroupPrivilegies(group.Idnum, Privileges.Read);
                DisplayGroupPrivileges(); 
            }
            catch { }
        }

        private void removeGroupButton_Click(object sender, EventArgs e)
        {
            int groupId = 0;

            foreach (DataGridViewCell cell in groupPrivDataGridView.SelectedCells)
            {
                DataGridViewRow row = groupPrivDataGridView.Rows[cell.RowIndex];
                groupId = (int)row.Cells["groupIDColumn"].Value;
                EditingNode.SetGroupPrivilegies(groupId, Privileges.NothingDo);
                DisplayGroupPrivileges(); // (user);
            }
        }

        private void addAllGroupsButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GroupNode group in groupComboBox.Items)
                {
                    EditingNode.SetGroupPrivilegies(group.Idnum, Privileges.Read);
                }
                DisplayGroupPrivileges(); 
            }
            catch { }
        }

        private void removeAllGroupsButton_Click(object sender, EventArgs e)
        {
            int groupId = 0;

            foreach (DataGridViewRow row in groupPrivDataGridView.Rows)
            {
                groupId = (int)row.Cells["groupIDColumn"].Value;
                EditingNode.SetGroupPrivilegies(groupId, Privileges.NothingDo);
            }
            DisplayGroupPrivileges(); // (user);
        }

        private void privEditDataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            System.Windows.Forms.DataGridView.HitTestInfo test =
                grid.HitTest(e.X, e.Y);
            if (test.RowIndex < 0 && !grid.Columns[test.ColumnIndex].ReadOnly)
            {
                bool val = true;
                foreach (DataGridViewRow row in grid.Rows)
                    val = val && (bool)row.Cells[test.ColumnIndex].Value;
                val = !val;

                foreach (DataGridViewRow row in grid.Rows)
                    row.Cells[test.ColumnIndex].Value = val;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (!passwordChanged) passwordTextBox.Text = "";
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            if (!passwordChanged)
            {
                if (String.IsNullOrEmpty(passwordTextBox.Text))
                    passwordTextBox.Text = passString;
                else passwordChanged = true;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            UpdateNode();

            DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Проверить корректность заполнения полей на форме, 
        /// если чё, оповестить пользователя МессаджьБоксом
        /// </summary>
        /// <returns>
        /// Еслие все поля заполнены корректно, true.
        /// В противном случае - false.
        /// </returns>
        private bool ValidateFields()
        {
            if (String.IsNullOrEmpty(loginTextBox.Text))
            {
                MessageBox.Show("Имя пользователя не может быть пустым", "Сообщение");
                return false;
            }
            if (passwordChanged && String.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("Пароль не может быть пустым", "Сообщение");
                return false;
            }
            if (passwordTextBox.Text != "admin") {
            	
            	if (passwordChanged && passwordTextBox.Text.Length < 8 ){
                MessageBox.Show("Пароль не может быть менее 8-ми символов", "Сообщение");
                return false;
            	}
            	
            	if (passwordChanged && !Regex.IsMatch(passwordTextBox.Text,@"[A-Z,А-Я]")){
                MessageBox.Show("В Пароле должны быть буквы в верхнем регистре", "Сообщение");
                return false;
            	}
            	
            	if (passwordChanged && !Regex.IsMatch(passwordTextBox.Text,@"[a-z,а-я]")){
                MessageBox.Show("В Пароле должны быть буквы в нижнем регистре", "Сообщение");
                return false;
            	}
            	
            	if (passwordChanged && !Regex.IsMatch(passwordTextBox.Text,@"[\W]")){
                MessageBox.Show("В Пароле должны быть спецсимволы", "Сообщение");
                return false;
            	}           
            } 
            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}