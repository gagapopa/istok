using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования группы пользователей
    /// </summary>
    partial class GroupEditForm : Form
    {
        /// <summary>
        /// Режим добавления новой группы
        /// </summary>
        public bool AddNew { get; protected set; }

        /// <summary>
        /// Редактируемый узел группы
        /// </summary>
        public GroupNode EditingNode { get; protected set; }

        public GroupEditForm(GroupNode group, bool addNew)
        {
            InitializeComponent();
            EditingNode = group;
            AddNew = addNew;
            if (AddNew)
                okButton.Text = "Добавить";
            else okButton.Text = "Изменить";
        }

        protected override void OnLoad(EventArgs e)
        {
            nameTextBox.Text = EditingNode.Text;
            descriptionTextBox.Text = EditingNode.Description;
            base.OnLoad(e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            EditingNode.Text = nameTextBox.Text;
            EditingNode.Description = descriptionTextBox.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}