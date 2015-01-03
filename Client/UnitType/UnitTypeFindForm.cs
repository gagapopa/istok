using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Форма для отображения и редактирования типов оборудования
    /// </summary>
    partial class UnitTypeFindForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        List<UTypeNode> typeNodes;

        public UnitTypeFindForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        private void unitTypeFindForm_Load(object sender, EventArgs e)
        {
            LoadTypes();
        }

        /// <summary>
        /// Запросить и отобразить список типов оборудования
        /// </summary>
        private async void LoadTypes()
        {
            if (InvokeRequired)
                Invoke((Action)LoadTypes);
            else
            {
                typeNodes = new List<UTypeNode>();
                dataGridView1.Rows.Clear();

                try
                {
                    var types = await Task.Factory.StartNew(() => strucProvider.Session.GetUnitTypes());
                    TypeNodeReceived(types);
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка загрузки типов структуры.", exc);
                    ShowError(exc);
                }
            }
        }

        /// <summary>
        /// Отобразитьт полученный асинхронно тип оборудования
        /// </summary>
        /// <param name="value">тип оборудования, полученное из асинхронной операции</param>
        private void TypeNodeReceived(UTypeNode[] typeArray)
        {
            if (typeArray != null)// && typeNodes.Idnum != (int)UnitTypeId.Unknown)
            {
                if (InvokeRequired)
                    Invoke((Action<UTypeNode[]>)TypeNodeReceived, new Object[] { typeArray });
                else
                {
                    foreach (var typeNode in typeArray)
                    {
                        if (typeNode != null && typeNode.Idnum != (int)UnitTypeId.Unknown)
                        {
                            int index = dataGridView1.Rows.Add();
                            typeNodes.Add(typeNode);
                            DataGridViewRow row = dataGridView1.Rows[index];
                            row.Tag = typeNode;
                            row.Cells["Column2"].Value = typeNode.Text;
                        }
                    }
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UTypeNode unitType = dataGridView1.CurrentRow.Tag as UTypeNode;

            if (unitType != null)
            {
                UnitTypeEditForm editForm = new UnitTypeEditForm(typeNodes, unitType.Clone() as UTypeNode, false);
                editForm.FormClosing += new FormClosingEventHandler(editForm_FormClosing);
                editForm.MdiParent = MdiParent;
                editForm.Show();
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UTypeNode newNode = new UTypeNode();
            using (MemoryStream mem = new MemoryStream())
            {
                Properties.Resources.unittype_default.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                newNode.Icon = mem.ToArray();
            }
            UnitTypeEditForm editForm = new UnitTypeEditForm(typeNodes, newNode, true);
            editForm.FormClosing += new FormClosingEventHandler(editForm_FormClosing);
            editForm.MdiParent = MdiParent;
            editForm.Show();
        }

        private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<UTypeNode> typeNodeToRemove = new List<UTypeNode>();
            UTypeNode typeNode;
            foreach (DataGridViewCell selectedCell in dataGridView1.SelectedCells)
            {
                DataGridViewRow gridRow = dataGridView1.Rows[selectedCell.RowIndex];
                if ((typeNode = gridRow.Tag as UTypeNode) != null
                    && !typeNodeToRemove.Contains(typeNode))
                    typeNodeToRemove.Add(typeNode);
            }

            if (typeNodeToRemove.Count > 0)
            {
                String message;
                if (typeNodeToRemove.Count == 1)
                {
                    message = String.Format("Удалить тип оборудования {0}?", typeNodeToRemove[0].Text);
                }
                else
                {
                    message = String.Format("Удалить {0} типов оборудования?", typeNodeToRemove.Count);
                }

                if (MessageBox.Show(message, "Удаление параметров", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var uts = from elem in typeNodeToRemove
                              select elem.Idnum;
                    try
                    {
                        await Task.Factory.StartNew(() => strucProvider.Session.RemoveUnitType(uts.ToArray()));
                        LoadTypes();
                    }
                    catch (Exception exc)
                    {
                        log.WarnException("Ошибка удаления типа структуры.", exc);
                        ShowError(exc);
                    }
                }
            }
        }

        async void editForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnitTypeEditForm editForm;
            if ((editForm = sender as UnitTypeEditForm) != null
                && editForm.DialogResult == DialogResult.OK)
            {
                try
                {
                    await Task.Factory.StartNew(() =>
                           {
                               if (editForm.AddNew)
                                   strucProvider.Session.AddUnitType(editForm.EditingType);
                               else
                                   strucProvider.Session.UpdateUnitType(editForm.EditingType);
                               LoadTypes();
                           });
                }
                catch (Exception exc)
                {
                    log.WarnException("Ошибка редактирования типа структуры.", exc);
                    ShowError(exc);
                }
            }
        }
    }
}