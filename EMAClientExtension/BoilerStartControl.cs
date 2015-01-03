using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client.Extension;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.EMA
{
    /// <summary>
    /// Панель пусков котла
    /// </summary>
    public partial class BoilerStartControl : UserControl
    {
        //ClientEMAExtension clientExtension;
        IUniForm uniForm;

        DataGridViewRow coldStateGridRow;
        DataGridViewRow notCoolStateGridRow;
        DataGridViewRow hotStateGridRow;
        DataGridViewRow hydrotestingGridRow;
        DataGridViewRow totalGridRow;

        public SplitContainer BoilerSplitContainer { get; set; }

        public BoilerStartControl(/*ClientEMAExtension*/Object clientExtension, IUniForm uniForm)
        {
            //this.clientExtension = clientExtension;
            this.uniForm = uniForm;

            InitializeComponent();
            // создадим строчки в таблице
            coldStateGridRow = dataGridView1.Rows[dataGridView1.Rows.Add("из холодного", "--", "--")];
            notCoolStateGridRow = dataGridView1.Rows[dataGridView1.Rows.Add("из неостывшего", "--", "--")];
            hotStateGridRow = dataGridView1.Rows[dataGridView1.Rows.Add("из горячего", "--", "--")];
            hydrotestingGridRow = dataGridView1.Rows[dataGridView1.Rows.Add("из гидроопресовок", "--", "--")];
            totalGridRow = dataGridView1.Rows[dataGridView1.Rows.Add("всего", "*", "*")];
        }

        /// <summary>
        /// Высота панели, что бы не было видно пустого места под табличкой
        /// </summary>
        public int RecomendedHeight
        {
            get
            {
                return totalGridRow.Height * 5 + 43;
            }
        }

        private void BoilerStartControl_Load(object sender, EventArgs e)
        {
            uniForm.StructureTree.AfterSelect += new TreeViewEventHandler(StructureTree_AfterSelect);
            StructureTree_AfterSelect(uniForm.StructureTree, new TreeViewEventArgs(uniForm.StructureTree.SelectedNode));
        }

        ExtensionUnitNode CurrentUnitNode;

        const String startCountTabName = "GetStartCount";
        const String totalFactName = "totalFact";
        const String coldStateFactName = "coldStateFact";
        const String notCoolStateFactName = "notCoolStateFact";
        const String hotStateFactName = "hotStateFact";
        const String hydrotestingName = "hydrotesting";
        const String coldStatePermitName = "coldStatePermit";
        const String notCoolHotStatePermitName = "notCoolHotStatePermit";

        /// <summary>
        /// Делегат, который должен прятать панель с текущей униформы
        /// </summary>
        public Action HidePanel { get; set; }

        /// <summary>
        /// Делегат, который должен показывать панель на текущей униформы
        /// </summary>
        public Action ShowPanel { get; set; }

        /// <summary>
        /// Делегат, который должен возвращать показана ли панель на текущей униформе
        /// </summary>
        public Func<bool> PanelShown { get; set; }

        /// <summary>
        /// При выделении нового узла в структуре, обновить панель при смене котла 
        /// или спрятать её, если текущий котёл не выбран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StructureTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UnitNode unitNode = uniForm.GetUnitNode(e.Node);
            ExtensionUnitNode boilerUnitNode = null;// clientExtension.State.GetParent(unitNode, clientExtension.BoilerTypeID) as ExtensionUnitNode;
            if (boilerUnitNode == null)
            {
                if (HidePanel != null)
                    HidePanel();
            }
            else if (boilerUnitNode != CurrentUnitNode)
            {
                CurrentUnitNode = boilerUnitNode;
                //AsyncOperationWatcher<ExtensionData> watcher = 
                var table =(ExtensionData)null;// clientExtension.State.GetExtensionExtendedTable(boilerUnitNode, startCountTabName);
                //watcher.AddValueRecivedHandler(startCountReceived);
                //watcher.Run();
                startCountReceived(table);
                if (ShowPanel != null)
                    ShowPanel();
            }
        }

        /// <summary>
        /// Отобразить данные пусков котла
        /// </summary>
        /// <param name="extendedTable"></param>
        private void startCountReceived(ExtensionData extendedTable)
        {
            if (extendedTable.Table.Rows.Count > 0)
            {
                DataRow dataRow = extendedTable.Table.Rows[0];

                coldStateGridRow.Cells[factColumn.Index].Value = dataRow[coldStateFactName];
                notCoolStateGridRow.Cells[factColumn.Index].Value = dataRow[notCoolStateFactName];
                hotStateGridRow.Cells[factColumn.Index].Value = dataRow[hotStateFactName];
                hydrotestingGridRow.Cells[factColumn.Index].Value = dataRow[hydrotestingName];
                totalGridRow.Cells[factColumn.Index].Value = dataRow[totalFactName];

                coldStateGridRow.Cells[permitColumn.Index].Value = dataRow[coldStatePermitName];
                notCoolStateGridRow.Cells[permitColumn.Index].Value = dataRow[notCoolHotStatePermitName];
            }
        }
    }
}
