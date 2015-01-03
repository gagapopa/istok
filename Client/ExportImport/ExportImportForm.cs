using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    public enum ExportImportMode { Export, Import }

    /// <summary>
    /// Окно экспорта и импорта
    /// </summary>
    partial class ExportImportForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        ExportImportMode mode;

        public ExportImportForm(ExportImportMode mode, StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            this.mode = mode;
            unitNodeTreeView.ImageList = Program.MainForm.Icons;
            unitPropertyGrid.Site = new FakeStructureProvider(strucProvider.Session).GetServiceContainer();

            // видимость настроек экспорта значений
            panel2.Visible = mode == ExportImportMode.Export;
            // видимость выбора родительского узла
            panel3.Visible = mode == ExportImportMode.Import;
            UpdateRootText();

            switch (this.mode)
            {
                case ExportImportMode.Export:
                    Text = "Экспорт элементов";
                    okButton.Text = "Экспорт";
                    break;
                case ExportImportMode.Import:
                    Text = "Импорт элементов";
                    okButton.Text = "Импорт";
                    break;
                default:
                    break;
            }
            valuesEnabledCheckBox_CheckedChanged(valuesEnabledCheckBox, EventArgs.Empty);
        }

        List<TreeWrapp<UnitNode>> unitNodeTreeRoot;
        bool closing;
        protected override async void OnLoad(EventArgs e)
        {
            try
            {
                if (mode == ExportImportMode.Export)
                {
                    //AsyncOperationWatcher<Object> watcher = 
                    var tree = await Task.Factory.StartNew(() => strucProvider.GetUnitNodeTree(null, new int[] { }, Privileges.Read));

                    unitNodeTreeRoot = new List<TreeWrapp<UnitNode>>();
                    //watcher.AddValueRecivedHandler(x => unitNodeTreeRoot.AddRange(x as TreeWrapp<UnitNode>[]));
                    //watcher.AddFinishHandler(() => TreeLoaded(unitNodeTreeRoot));
                    //RunWatcher(watcher);
                    unitNodeTreeRoot.AddRange(tree);
                    TreeLoaded(unitNodeTreeRoot);
                }
                else if (mode == ExportImportMode.Import)
                {
                    ExportFormat exportFormat;
                    openFileDialog1.Filter = "XML File|*.xml|Zipped XML File|*.xml.gz";// +
#if DEBUG
                    openFileDialog1.Filter += "|Excel XML files (*.xlsx)|*.xlsx" +
                                              "|Word XML files (*.docx)|*.docx";
#endif
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        byte[] buf = null;
                        bool abort = false;
                        String fileName = openFileDialog1.FileName;
                        switch (openFileDialog1.FilterIndex)
                        {
                            case 1: 
                                exportFormat = ExportFormat.XML; 
                                break;
                            case 2:
                                exportFormat = ExportFormat.ZippedXML; 
                                break;
                            case 3: 
                                exportFormat = ExportFormat.Excel;
                                exportFormat = ExportFormat.Excel;
                                ExcelImportForm eif = new ExcelImportForm(strucProvider);
                                eif.LoadFile(openFileDialog1.FileName);
                                eif.ShowDialog();
                                if (eif.DialogResult == DialogResult.OK && eif.Result != null)
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        BinaryFormatter bf = new BinaryFormatter();
                                        bf.Serialize(ms, eif.Result);
                                        buf = ms.ToArray();
                                    }
                                }
                                else
                                    abort = true;
                                break;
                            case 4:
                                exportFormat = ExportFormat.WordX;
                                WordImportForm wif = new WordImportForm(strucProvider);
                                wif.LoadFile(openFileDialog1.FileName);
                                wif.ShowDialog();
                                if (wif.DialogResult == DialogResult.OK && wif.Result != null)
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        BinaryFormatter bf = new BinaryFormatter();
                                        bf.Serialize(ms, wif.Result);
                                        buf = ms.ToArray();
                                    }
                                }
                                else
                                    abort = true;
                                break;
                            default:
                                throw new NotSupportedException();
                        }

                        if (!abort)
                        {
                            if (buf == null)
                            {
                                buf = new byte[1 << 14];
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                                    {
                                        int count;
                                        while ((count = fs.Read(buf, 0, buf.Length)) > 0)
                                            ms.Write(buf, 0, count);
                                    }
                                    buf = new byte[ms.Length];
                                    ms.Position = 0;
                                    ms.Read(buf, 0, buf.Length);
                                }
                            }

                            //AsyncOperationWatcher<ImportDataContainer> watcher = strucProvider.BeginImport(buf, exportFormat);

                            //ImportDataContainer importContainer = null;
                            //watcher.AddValueRecivedHandler(x =>
                            //{
                            //    importContainer = x;
                            //});
                            //watcher.AddSuccessFinishHandler(() => TreeLoaded(importContainer));
                            //RunWatcher(watcher);
                            ImportDataContainer importContainer = await Task.Factory.StartNew(() => strucProvider.Import(buf, exportFormat));
                            TreeLoaded(importContainer);
                            //MessageBox.Show("Импорт типа начался");
                            //throw new NotImplementedException();
                        }
                    }
                    else closing = true;
                }
                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                log.WarnException("Ошибка импорта.", ex);
                ShowError(ex);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (closing)
                this.Close();
            base.OnShown(e);
        }
        private void BuildTree(TreeNodeCollection nodeCollection, List<TreeWrapp<UnitNode>> unitNodeCollection)
        {
            UnitTreeNode treeNode;

            if (unitNodeCollection != null)
            {
                unitNodeCollection.Sort((a, b) =>
                {
                    int ret = 0;
                    ret = a.Item.Index.CompareTo(b.Item.Index);
                    if (ret == 0)
                        ret = String.Compare(a.Item.DocIndex, b.Item.DocIndex);
                    return ret;
                });
                foreach (TreeWrapp<UnitNode> unit in unitNodeCollection)
                {
                    treeNode = CreateTreeNode(unit.Item);
                    nodeCollection.Add(treeNode);
                    List<TreeWrapp<UnitNode>> childNodes = null;
                    if (unit.ChildNodes != null)
                        childNodes = new List<TreeWrapp<UnitNode>>(unit.ChildNodes);
                    BuildTree(treeNode.Nodes, childNodes);
                }
            }
        }

        private UnitTreeNode CreateTreeNode(UnitNode unit)
        {
            UnitTreeNode treeNode;
            treeNode = new UnitTreeNode();
            treeNode.Text = unit.GetNodeText(); //.Text;
            treeNode.ImageKey = ((int)unit.Typ).ToString();
            treeNode.SelectedImageKey = treeNode.ImageKey;
            treeNode.Node = unit;
            return treeNode;
        }

        private void TreeLoaded(List<TreeWrapp<UnitNode>> wrappColection)
        {
            if (InvokeRequired) Invoke((Action<List<TreeWrapp<UnitNode>>>)TreeLoaded, wrappColection);
            else BuildTree(unitNodeTreeView.Nodes, wrappColection);
        }

        private void TreeLoaded(ImportDataContainer importContainer)
        {
            if (InvokeRequired) Invoke((Action<ImportDataContainer>)TreeLoaded, importContainer);
            else
            {
                if (importContainer != null)
                {
                    // Дополняем узлы со значениями
                    if (importContainer.Values != null)
                        RecursiveSetValues(importContainer.Nodes, importContainer.Values);

                    BuildTree(unitNodeTreeView.Nodes, new List<TreeWrapp<UnitNode>>(importContainer.Nodes ?? new TreeWrapp<UnitNode>[0]));

                    // Значения бес структуры
                    if (importContainer.Values != null)
                    {
                        TreeNode valuesOnlyNode = new TreeNode("Только значения");

                        foreach (var code in importContainer.Values.Keys)
                        {
                            var unitNode = new ImportUnitNode(importContainer.Values[code], code);
                            var treeNode = CreateTreeNode(unitNode);
                            valuesOnlyNode.Nodes.Add(treeNode);
                        }
                        if (valuesOnlyNode.Nodes.Count > 0)
                            unitNodeTreeView.Nodes.Add(valuesOnlyNode);
                    }
                }
            }
        }

        private static void RecursiveSetValues(TreeWrapp<UnitNode>[] nodes, Dictionary<String, Package[]> valuesDictionary)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    Package[] packs;

                    String code = node.Item.Code;
                    if (valuesDictionary.TryGetValue(code, out packs))
                    {
                        node.Item = new ImportUnitNode(node.Item, packs);
                        valuesDictionary.Remove(code);
                    }
                    if (node.ChildNodes != null && node.ChildNodes.Length > 0)
                        RecursiveSetValues(node.ChildNodes, valuesDictionary);
                }
            }
        }

        private void unitNodeTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UnitTreeNode treeNode = e.Node as UnitTreeNode;

            if (treeNode != null)
            {
                unitPropertyGrid.SelectedObject = treeNode.Node;
            }
            else unitPropertyGrid.SelectedObject = null;
        }

        private void unitNodeTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByKeyboard
                || e.Action == TreeViewAction.ByMouse)
            {
                bool check = e.Node.Checked;

                RecursiveCheckChildNodes(e.Node.Nodes, check);
                TreeNode parentNode = e.Node;
                while (check && (parentNode = parentNode.Parent) != null)
                    parentNode.Checked = check;
            }
        }

        private static void RecursiveCheckChildNodes(TreeNodeCollection nodes, bool check)
        {
            foreach (TreeNode treeNode in nodes)
            {
                treeNode.Checked = check;
                if(nodes!=null)
                    RecursiveCheckChildNodes(treeNode.Nodes, check);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int[] nodeIds = GetUnitNodes(unitNodeTreeView.Nodes);

            try
            {
                if (mode == ExportImportMode.Export)
                {
                    ExportFormat exportFormat;
                    saveFileDialog1.Filter = "XML File|*.xml|Zipped XML File|*.xml.gz";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        String fileName = saveFileDialog1.FileName;
                        if (saveFileDialog1.FilterIndex == 1) exportFormat = ExportFormat.XML;
                        else
                        {
                            exportFormat = ExportFormat.ZippedXML;
                            fileName = Path.ChangeExtension(fileName, "xml.gz");
                        }
                        DateTime beginTime, endTime;
                        if (valuesEnabledCheckBox.Checked)
                        {
                            beginTime = valuesBeginDateTimePicker.Value;
                            endTime = valuesEndDateTimePicker.Value;
                        }
                        else
                        {
                            beginTime = DateTime.MaxValue;
                            endTime = DateTime.MinValue;
                        }
                        AsyncOperationWatcher<UAsyncResult> watcher = strucProvider.BeginExport(nodeIds, beginTime, endTime, exportFormat);
                        List<byte> bytes = new List<byte>();
                        watcher.AddValueRecivedHandler(x => bytes.AddRange(x.Bytes));
                        watcher.AddSuccessFinishHandler(() =>
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                fs.Write(bytes.ToArray(), 0, bytes.Count);
                            }
                        });
                        RunWatcher(watcher);
                        //MessageBox.Show("Экспорт типа начался");
                        //throw new NotImplementedException();
                        CloseOnFinishWatcher = true;
                    }
                }
                else if (mode == ExportImportMode.Import)
                {
                    ImportDataContainer importContainer = GetUnitNodeWrapp();
                    AsyncOperationWatcher watcher = strucProvider.BeginApplyImport(rootNode, importContainer);
                    RunWatcher(watcher);
                    //MessageBox.Show("BeginApplyImport");
                    //throw new NotImplementedException();
                    this.CloseOnFinishWatcher = true;
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка импорта.", exc);
                ShowError(exc);
            }
        }

        private ImportDataContainer GetUnitNodeWrapp()
        {
            Dictionary<String, Package[]> valuesDictionary = new Dictionary<String, Package[]>();

            TreeWrapp<UnitNode>[] wrapps = GetUnitNodeWrapp(unitNodeTreeView.Nodes, valuesDictionary);

            return new ImportDataContainer(wrapps, valuesDictionary);
        }

        private TreeWrapp<UnitNode>[] GetUnitNodeWrapp(TreeNodeCollection treeNodeCollection, Dictionary<String, Package[]> valuesDictionary)
        {
            List<TreeWrapp<UnitNode>> wrappList = new List<TreeWrapp<UnitNode>>();
            TreeWrapp<UnitNode> unitNodeWrapp;

            foreach (TreeNode treeNode in treeNodeCollection)
            {
                if (treeNode.Checked)
                {
                    UnitNode node = null;
                    UnitTreeNode unitTreeNode = treeNode as UnitTreeNode;

                    if (unitTreeNode != null)
                    {
                        node = unitTreeNode.Node;
                        ImportUnitNode importUnitNode;

                        if ((importUnitNode = node as ImportUnitNode) != null)
                        {
                            node = importUnitNode.Node;
                            String code = node == null ? importUnitNode.ImportCode : node.Code;

                            valuesDictionary[code] = importUnitNode.ImportValues;
                        }
                    }

                    var childNodes = GetUnitNodeWrapp(treeNode.Nodes, valuesDictionary);

                    if (node == null)
                    {
                        wrappList.AddRange(childNodes);
                    }
                    else
                    {
                        unitNodeWrapp = new TreeWrapp<UnitNode>(node, childNodes);
                        wrappList.Add(unitNodeWrapp);
                    }
                }
            }
            return wrappList.ToArray();
        }

        private  int[] GetUnitNodes(TreeNodeCollection treeNodes)
        {
            UnitTreeNode unitTreeNode;
            List<int> unitNodesList = new List<int>();

            foreach (TreeNode treeNode in treeNodes)
            {
                if (treeNode.Checked&&(unitTreeNode = treeNode as UnitTreeNode) != null)
                {
                    unitNodesList.Add(unitTreeNode.Node.Idnum);
                    unitTreeNode.Node.ClearNodes();
                    unitNodesList.AddRange(GetUnitNodes(unitTreeNode.Nodes));
                }
            }
            return unitNodesList.ToArray();
        }

        private void valuesEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = valuesEnabledCheckBox.Checked;
        }

        UnitNode rootNode;

        private void chooseParentButton_Click(object sender, EventArgs e)
        {
            SelectForm selectForm = new SelectForm(strucProvider);

            selectForm.MultiSelect = false;

            if (selectForm.ShowDialog()== DialogResult.OK)
            {
                if (selectForm.SelectedObjects.Length > 0)
                    rootNode = selectForm.SelectedObjects[0];
                else 
                    rootNode = null;
            }

            UpdateRootText();
        }

        private void UpdateRootText()
        {
            if (rootNode == null)
                importRootTextBox.Text = "Корень";
            else
            {
                importRootTextBox.Text = rootNode.FullName;
            }
        }
    }
}
