using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client;
using COTES.ISTOK.Extension;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.EMA
{
    /// <summary>
    /// Окно "Параметры", для отображения констант и пр.
    /// </summary>
    public partial class ParametersForm : BaseAsyncWorkForm
    {
        //ClientEMAExtension clientExtension;

        /// <summary>
        /// Название расширения на ДКСМ-Клиенте, данные из которого будут отображаться в данном окне
        /// </summary>
        const String EMAExtensionName = "АСТДК";

        public ParametersForm(StructureProvider strucProvider, /*ClientEMAExtension*/Object clientExtension)
            : base(strucProvider)
        {
            //this.clientExtension = clientExtension;
            InitializeComponent();
        }

        private void ParametersForm_Load(object sender, EventArgs e)
        {
            ExtensionDataInfo[] tabInfos = null;// clientExtension.State.GetExtensionExtendedTableInfo(EMAExtensionName);
            TreeNode treeNode;

            if (tabInfos != null)
                foreach (var item in tabInfos)
                {
                    treeNode = new TreeNode(item.Caption);
                    treeNode.Tag = item;
                    treeView1.Nodes.Add(treeNode);
                }
        }

        /// <summary>
        /// Запросить данные с ДКСМ-Клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ExtensionDataInfo tabInfo = e.Node.Tag as ExtensionDataInfo;

            this.Text = String.Format("Параметры: {0}", tabInfo.Caption);

            //AsyncOperationWatcher<ExtensionData> watcher = 
            var table = (ExtensionData)null; //clientExtension.State.GetExtensionExtendedTable(EMAExtensionName, tabInfo);
            //watcher.AddValueRecivedHandler(tableReceived);
            //RunWatcher(watcher);
            tableReceived(table);
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="tab"></param>
        void tableReceived(ExtensionData tab)
        {
            if (InvokeRequired) Invoke((Action<ExtensionData>)tableReceived, tab);
            else
            {
                if (tab != null)
                {
                    dataGridView1.DataSource = tab.Table;
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        ExtensionDataColumn columnDescription = tab.Info.Columns.First(l => l.Name.Equals(column.Name));
                        if (columnDescription != null)
                        {
                            if (!String.IsNullOrEmpty(columnDescription.Unit))
                                column.HeaderText = String.Format("{0}, {1}");
                            else column.HeaderText = columnDescription.Caption;
                        }
                    }
                }
            }
        }
    }
}
