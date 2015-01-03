using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Форма для редактирования типа оборудования
    /// </summary>
    partial class UnitTypeEditForm : Form
    {
        List<UTypeNode> typeList;

        /// <summary>
        /// Редактируемый тип оборудования
        /// </summary>
        public UTypeNode EditingType { get; protected set; }

        /// <summary>
        /// Идет добавление нового типа
        /// </summary>
        public bool AddNew { get; protected set; }

        public UnitTypeEditForm(List<UTypeNode> typeList, UTypeNode editingType, bool addNew)
            : base()
        {
            InitializeComponent();
            AddNew = addNew;
            if (addNew) okButton.Text = "Добавить";
            else okButton.Text = "Изменить";
            openFileDialog1.FileName = "";
            this.typeList = typeList;
            this.EditingType = editingType;
        }

        private void UnitTypeEditForm_Load(object sender, EventArgs e)
        {
            nameTextBox.Text = EditingType.Text;
            //visibilityComboBox.SelectedIndex = EditingType.Tree_visible;

            if (EditingType.Icon != null)
                using (MemoryStream ms = new MemoryStream(EditingType.Icon))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            String[] props = EditingType.Props.Split(new char[] { ';' });
            String propertyName = null;
            foreach (String propName in props)
            {
                if (propName != null) propertyName = propName.Trim();
                if (!String.IsNullOrEmpty(propertyName)) propertiesDataGridView.Rows.Add(propertyName);
            }

            allTypesCheckBox.Checked = EditingType.ChildFilterAll;
            foreach (UTypeNode typeNode in typeList)
                typeFilterCheckedListBox.Items.Add(typeNode, EditingType.ChildFilter.Contains(typeNode.Idnum));
        }

        private void pickImageButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            try
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, ImageFormat.Png);
                    EditingType.Icon = ms.ToArray();
                }
            }
            //catch (System.Net.Sockets.SocketException) { main.CloseConnection(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            EditingType.Text = nameTextBox.Text;
            //EditingType.Tree_visible = visibilityComboBox.SelectedIndex;

            StringBuilder propsBuilder = new StringBuilder();

            foreach (DataGridViewRow propRow in propertiesDataGridView.Rows)
            {
                if (!propRow.IsNewRow && propRow.Cells[0].Value != null)
                {
                    String propName = propRow.Cells[0].Value.ToString().Trim();
                    if (!String.IsNullOrEmpty(propName))
                    {
                        if (propsBuilder.Length > 0) propsBuilder.Append(';');
                        propsBuilder.Append(propName);
                    }
                }
            }
            EditingType.Props = propsBuilder.ToString();

            UTypeNode filteredType;
            List<int> checkedFilter = new List<int>();
            foreach (Object item in typeFilterCheckedListBox.CheckedItems)
            {
                if ((filteredType = item as UTypeNode) != null)
                    checkedFilter.Add(filteredType.Idnum);
            }
            EditingType.ChildFilter = checkedFilter;
            EditingType.ChildFilterAll = allTypesCheckBox.Checked;

            if (String.IsNullOrEmpty(EditingType.Text))
            {
                MessageBox.Show("Не допускаются типы с пустым именем");
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close(); 
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < typeFilterCheckedListBox.Items.Count; i++)
                typeFilterCheckedListBox.SetItemChecked(i, true);
        }

        private void checkNothingButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < typeFilterCheckedListBox.Items.Count; i++)
                typeFilterCheckedListBox.SetItemChecked(i, false);
        }
    }
}