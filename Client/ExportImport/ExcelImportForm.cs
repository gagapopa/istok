using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    public partial class ExcelImportForm : Form
    {
        Dictionary<string, string> dicSharedStrings = new Dictionary<string, string>();
        List<Page> lstPages = new List<Page>();

        string strError = "";

        StructureProvider strucProvider;

        public ExcelImportForm(StructureProvider strucProvider)
        {
            InitializeComponent();

            this.strucProvider = strucProvider;

            foreach (var revision in strucProvider.Session.Revisions)
                revisionComboBox.Items.Add(revision);
            revisionComboBox.SelectedItem = RevisionInfo.Default;
        }

        public TreeWrapp<UnitNode>[] Result { get; private set; }

        public void LoadFile(string filename)
        {
            SpreadsheetDocument doc = null;
            try
            {
                Parameter param;
                Page pg;
                uint idx;
                string rid;
                
                doc = SpreadsheetDocument.Open(filename, false);

                idx = 0;
                dicSharedStrings.Clear();
                foreach (SharedStringItem item in doc.WorkbookPart.SharedStringTablePart.SharedStringTable)
                {
                    if (item.Text != null)
                        dicSharedStrings[idx.ToString()] = item.Text.Text;
                    else
                        dicSharedStrings[idx.ToString()] = GetText(item);
                    idx++;
                }

                foreach (var wsh in doc.WorkbookPart.WorksheetParts)
                {
                    pg = new Page();
                    param = null;

                    rid = doc.WorkbookPart.GetIdOfPart(wsh);
                    var sh = (from elem in doc.WorkbookPart.Workbook.Sheets
                              where ((Sheet)elem).Id.Value == rid
                              select (Sheet)elem).First();

                    pg.Name = sh.Name;
                    pg.Index = rid;

                    var data = wsh.Worksheet.GetFirstChild<SheetData>();
                    if (data != null)
                    {
                        foreach (var row in data.Elements<Row>())
                        {
                            foreach (var cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                            {
                                uint colIndex, rowIndex;

                                rowIndex = SplitCellReference(cell.CellReference.Value, out colIndex);

                                if (cell.CellValue != null)
                                {
                                    switch (colIndex)
                                    {
                                        case 0:
                                            if (param != null) pg.AddParameter(param);
                                            param = new Parameter();
                                            param.Index = GetCellValue(cell);
                                            break;
                                        case 1:
                                            if (param != null) param.Name = GetCellValue(cell);
                                            break;
                                        case 2:
                                            if (param != null) param.Code = GetCellValue(cell);
                                            break;
                                        case 3:
                                            if (param != null) param.Unit = GetCellValue(cell);
                                            break;
                                        case 4:
                                            //Источник
                                            break;
                                        default:
                                            if (param != null) param.AddData(GetCellValue(cell), rowIndex, colIndex);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    if (param != null) pg.AddParameter(param);
                    lstPages.Add(pg);
                }
                lstPages.Sort((Page p1, Page p2) => { return p1.Index.CompareTo(p2.Index); });

                LoadPages();
            }
            finally
            {
                if (doc != null) doc.Close();
            }
        }

        private string GetText(OpenXmlElement elem)
        {
            StringBuilder sb = new StringBuilder();

            if (elem is Text) sb.Append(((Text)elem).Text);
            if (elem.ChildElements != null)
                foreach (var item in elem.ChildElements)
                    sb.Append(GetText(item));

            return sb.ToString();
        }
        private string GetCellValue(DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            string res = "";
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        dicSharedStrings.TryGetValue(cell.CellValue.Text, out res);
                        break;
                    case CellValues.String:
                        res = cell.CellValue.Text;
                        break;
                    default:
                        res = cell.CellValue.Text;
                        break;
                }
            }
            else
                res = cell.CellValue.Text;
            return res;
        }
        /// <summary>
        /// CellReference splitter
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="cell">out - column index</param>
        /// <returns>row index</returns>
        private uint SplitCellReference(string reference, out uint cell)
        {
            StringBuilder sbrow = new StringBuilder(), sbcol = new StringBuilder();
            uint res = 0;
            bool d = false;

            foreach (var item in reference)
            {
                if (char.IsDigit(item))
                {
                    d = true;
                    sbrow.Append(item);
                }
                else
                    if (char.IsLetter(item) && !d)
                        sbcol.Append(item);
                    else
                        throw new ArgumentException("Reference is invalid", "reference");
            }
            cell = 0;
            foreach (var ch in sbcol.ToString())
                cell = cell * ('Z' - 'A' + 1) + ch - 'A' + 1;
            //cell = sbcol.ToString();
            cell--;
            uint.TryParse(sbrow.ToString(), out res);
            res--;
            return res;
        }

        private void LoadPages()
        {
            cbxPages.Items.Clear();
            foreach (var pg in lstPages)
            {
                cbxPages.Items.Add(pg);
                if (cbxPages.SelectedItem == null) cbxPages.SelectedItem = pg;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Init();
            //LoadPages();
        }

        private void cbxPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page pg;
            cbxParameters.Items.Clear();
            if (cbxPages.SelectedItem != null && (pg = cbxPages.SelectedItem as Page) != null)
            {
                cbxParameters.Items.Clear();
                foreach (var par in pg.Parameters)
                {
                    cbxParameters.Items.Add(par);
                    if (cbxParameters.SelectedItem == null) cbxParameters.SelectedItem = par;
                }
            }
        }

        private void cbxParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            Parameter param;

            dgv.DataSource = null;
            dgv.Rows.Clear();
            dgv.Columns.Clear();            
            if (cbxParameters.SelectedItem != null && (param = cbxParameters.SelectedItem as Parameter) != null)
            {
                dgv.DataSource = param.Table;
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(param.Table.Columns[i].Caption))
                        dgv.Columns[i].HeaderText = param.Table.Columns[i].Caption;
                }
                ProcessData();
            }
        }

        private void addMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMeta(tsMetaName.Text);
        }

        private void deleteMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMeta("");
        }

        private void SetMeta(string meta)
        {
            DataTable table;
            if (dgv.CurrentCell == null) return;
            table = dgv.DataSource as DataTable;
            if (table != null)
            {
                table.Columns[dgv.CurrentCell.ColumnIndex].Caption = meta;
                if (!string.IsNullOrEmpty(meta))
                    dgv.Columns[dgv.CurrentCell.ColumnIndex].HeaderText = meta;
                else
                    dgv.Columns[dgv.CurrentCell.ColumnIndex].HeaderText = table.Columns[dgv.CurrentCell.ColumnIndex].ColumnName;
            }
        }

        private void ProcessData()
        {
            IDictionary dicData;
            DimensionInfo[] dims;
            DataTable table;

            RevisionInfo revision = revisionComboBox.SelectedItem as RevisionInfo;

            strError = "";
            splitContainer1.Panel2.Controls.Clear();
            if (dgv.DataSource != null && (table = dgv.DataSource as DataTable) != null)
            {
                if (table.Rows.Count > 0)
                {
                    try
                    {
                        dims = ProcessData(table, out dicData);

                        NormFuncNode node = new NormFuncNode();
                        node.Typ = (int)COTES.ISTOK.ASC.UnitTypeId.NormFunc;
                        //node.MDTable = GetMDT(dims, dicData);
                        node.SetMDTable(revision, GetMDT(dims, dicData));
                        NormFuncUnitProvider up = new NormFuncUnitProvider(strucProvider, node);
                        NormFuncUnitControl control = new NormFuncUnitControl(up, chbxEditor.Checked);
                        control.Dock = DockStyle.Fill;
                        splitContainer1.Panel2.Controls.Add(control);
                        control.InitiateProcess();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fatal error: " + ex.Message);
                    }
                }
            }
            if (string.IsNullOrEmpty(strError))
                lblStatus.Text = "Status: OK";
            else
                lblStatus.Text = "Status: " + strError;
        }

        private MultiDimensionalTable GetMDT(DimensionInfo[] dimensions, IDictionary data)
        {
            MultiDimensionalTable mdt = new MultiDimensionalTable();
            mdt.RemoveDimension(mdt.DimensionInfo[0]);
            //for (int i = dimensions.Length - 2; i >= 0; i--)
            //{
            //    if (i == dimensions.Length - 2)
            //    {
            //        mdt.DimensionInfo[0].Name = dimensions[i].Name;
            //        mdt.DimensionInfo[0].Measure = dimensions[i].Measure;
            //    }
            //    else
            //        mdt.AddDimension(dimensions[i].Name, dimensions[i].Measure, 0);
            //}
            FillMDT(mdt, dimensions, data, null);

            return mdt;
        }
        private void FillMDT(MultiDimensionalTable mdt, DimensionInfo[] dimensions, IDictionary data, object[] coords)
        {
            object[] arrCoords;
            if (coords != null)
            {
                arrCoords = new object[coords.Length + 1];
                coords.CopyTo(arrCoords, 1);
            }
            else
                arrCoords = new object[1];
            foreach (var key in data.Keys)
            {
                arrCoords[0] = key;
                if (data[key] is IDictionary)
                    FillMDT(mdt, dimensions, (IDictionary)data[key], arrCoords);
                else
                    if (data[key] is string)
                    {
                        for (int i = 0; i < arrCoords.Length; i++)
                        {
                            if (i >= mdt.DimensionInfo.Length)
                            {
                                int idx = arrCoords.Length - i - 1;
                                mdt.AddDimension(dimensions[idx].Name, dimensions[idx].Measure, ObjectToDouble(arrCoords[i]));
                            }
                        }
                        mdt.SetValue(ObjectToDouble(data[key]), ObjectToDouble(arrCoords));
                    }
            }
        }

        private double[] ObjectToDouble(object[] items)
        {
            List<double> lstRes = new List<double>();
            foreach (var item in items)
                lstRes.Add(ObjectToDouble(item));
            return lstRes.ToArray();
        }
        private double ObjectToDouble(object item)
        {
            if (item is double) return (double)item;
            double res;
            if (!double.TryParse(item.ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                strError = "Type cast error: " + item.ToString();
                res = 0;
            }
            return res;
        }

        private DimensionInfo[] ProcessData(DataTable table, out IDictionary data)
        {
            return ProcessData(table, null, out data);
        }
        private DimensionInfo[] ProcessData(DataTable table, DataRow[] valueRows, out IDictionary data)
        {
            List<IDictionary> lstDics = new List<IDictionary>();
            //Dictionary<object, object> dicData = new Dictionary<object, object>();
            List<DimensionInfo> lstDimensions = new List<DimensionInfo>();
            List<string> lstRepeats = new List<string>();
            DataTable tbl = null;
            DataRow valRow = null;
            bool repeats = false;

            if (table == null) throw new ArgumentNullException("table");

            data = null;

            string firstItem = "", curItem;
            
            foreach (DataRow row in table.Rows)
            {
                if (row.ItemArray.Length > 0)
                    curItem = row[0].ToString();
                else
                    continue;

                if (string.IsNullOrEmpty(firstItem))
                {
                    valRow = row;
                    firstItem = curItem;
                    tbl = table.Clone();
                    //tbl.Rows.Add(row.ItemArray);
                    //lstDimensions.Clear();
                    //lstDimensions.Add(GetDimensionInfo(row));
                }
                else
                {
                    if (curItem == firstItem)
                    {
                        List<DataRow> lstRows = new List<DataRow>();
                        IDictionary tmp;
                        
                        lstRepeats.Clear();
                        repeats = false;
                        
                        if (valueRows != null) lstRows.AddRange(valueRows);
                        lstRows.Add(valRow);
                        DimensionInfo[] dims = ProcessData(tbl, lstRows.ToArray(), out tmp);
                        lstDics.Add(tmp);
                        foreach (var dm in dims)
                            if (!lstDimensions.Contains(dm)) lstDimensions.Add(dm);
                        valRow = row;
                        tbl = table.Clone();
                        //tbl.Rows.Add(row.ItemArray);
                    }
                    else
                    {
                        tbl.Rows.Add(row.ItemArray);
                    }
                }

                if (!lstRepeats.Contains(curItem)) lstRepeats.Add(curItem);
                else repeats = true;
            }
            
            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (repeats)
                {
                    if (valRow != null)
                    {
                        List<DataRow> lstRows = new List<DataRow>();
                        IDictionary tmp;

                        if (valueRows != null) lstRows.AddRange(valueRows);
                        lstRows.Add(valRow);
                        DimensionInfo[] dims = ProcessData(tbl, lstRows.ToArray(), out tmp);
                        foreach (var dm in dims)
                            if (!lstDimensions.Contains(dm)) lstDimensions.Add(dm);
                        lstDics.Add(tmp);
                    }
                }
                else
                {
                    DataRow row;
                    if (valRow != null)
                    {
                        row = tbl.NewRow();
                        row.ItemArray = valRow.ItemArray;
                        tbl.Rows.InsertAt(row, 0);
                    }
                    if (valueRows != null)
                    {
                        for (int i = valueRows.Length - 1; i >= 0; i--)
                        {
                            row = tbl.NewRow();
                            row.ItemArray = valueRows[i].ItemArray;
                            tbl.Rows.InsertAt(row, 0);
                        }
                    }
                    foreach (DataRow item in tbl.Rows)
                    {
                        DimensionInfo di = GetDimensionInfo(item);
                        bool found = false;
                        foreach (var dm in lstDimensions)
                            if (dm.Name == di.Name) { found = true; break; }
                        if (!found) lstDimensions.Add(di);
                    }

                    Dictionary<int, object> dicLast = new Dictionary<int, object>();
                    for (int i = 2; i < tbl.Columns.Count; i++)
                    {
                        Dictionary<object, object> dicTmp = null;
                        string val = "";
                        for (int r = 0; r < tbl.Rows.Count; r++)
                        {
                            val = tbl.Rows[r][i].ToString();
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (val == "ClR")
                                    dicLast.Remove(r);
                                else
                                    dicLast[r] = val;
                            }
                        }
                        for (int r = tbl.Rows.Count - 2; r >= 0; r--)
                        {
                            if (!dicLast.ContainsKey(r))
                            {
                                dicTmp = null;
                                break;
                            }

                            if (dicTmp != null)
                            {
                                Dictionary<object, object> dicT = new Dictionary<object, object>();
                                dicT[dicLast[r]] = dicTmp;
                                dicTmp = dicT;
                            }
                            else
                            {
                                dicTmp = new Dictionary<object, object>();
                                if (dicLast.ContainsKey(r) && dicLast.ContainsKey(r + 1))
                                    dicTmp[dicLast[r]] = dicLast[r + 1];
                                else
                                    strError = "Dictionary creation error";
                            }
                        }
                        if (dicTmp != null) lstDics.Add(dicTmp);
                    }
                }
            }
            data = MergeDictionaries(lstDics.ToArray());
            
            return lstDimensions.ToArray();
        }
        private IDictionary MergeDictionaries(IDictionary[] arrDics)
        {
            Dictionary<object, object> res = new Dictionary<object, object>();

            foreach (var item in arrDics)
            {
                foreach (var key in item.Keys)
                {
                    if (res.ContainsKey(key))
                    {
                        if (res[key] is IDictionary && item[key] is IDictionary)
                        {
                            res[key] = MergeDictionaries(new IDictionary[] { (IDictionary)res[key], (IDictionary)item[key] });
                        }
                    }
                    else
                    {
                        res[key] = item[key];
                    }
                }
            }

            return res;
        }
        private DimensionInfo GetDimensionInfo(DataRow row)
        {
            DimensionInfo di = new DimensionInfo();
            string tmp;

            if (row == null) throw new ArgumentNullException("row");
            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                tmp = row[i].ToString();
                if (i == 0)
                    di.Name = tmp;
                else
                {
                    if (i == 1) di.Measure = tmp;
                }
            }

            return di;
        }

        private void splitContainer1_Panel2_ControlRemoved(object sender, ControlEventArgs e)
        {
            splitContainer1.Panel2Collapsed = splitContainer1.Panel2.Controls.Count == 0;
        }

        private void splitContainer1_Panel2_ControlAdded(object sender, ControlEventArgs e)
        {
            splitContainer1.Panel2Collapsed = splitContainer1.Panel2.Controls.Count == 0;
        }

        private void chbxEditor_CheckedChanged(object sender, EventArgs e)
        {
            ProcessData();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                List<TreeWrapp<UnitNode>> lstRes = new List<TreeWrapp<UnitNode>>();

                RevisionInfo revision = revisionComboBox.SelectedItem as RevisionInfo;

                if (revision == null)
                    revision = RevisionInfo.Default;

                foreach (var pg in lstPages)
                {
                    UnitNode folder = new UnitNode();
                    folder.Typ = (int)UnitTypeId.Folder;
                    folder.Text = pg.Name;
                    TreeWrapp<UnitNode> item = new TreeWrapp<UnitNode>(folder);
                    foreach (var par in pg.Parameters)
                    {
                        try
                        {
                            if (par.Table.Rows.Count > 0)
                            {
                                IDictionary dicData;
                                DimensionInfo[] dims = ProcessData(par.Table, out dicData);
                                NormFuncNode node = new NormFuncNode();
                                node.Typ = (int)COTES.ISTOK.ASC.UnitTypeId.NormFunc;
                                //node.MDTable = GetMDT(dims, dicData);
                                node.SetMDTable(revision, GetMDT(dims, dicData));
                                node.Text = par.Name;
                                node.Code = par.Code;
                                node.DocIndex = par.Index;
                                if (!string.IsNullOrEmpty(par.Unit))
                                {
                                    string tmp = par.Unit.Replace(new string(new char[] { Convert.ToChar(8211) }), "");//один из вариантов '-'
                                    tmp = tmp.Replace("-", "");
                                    if (!string.IsNullOrEmpty(tmp))
                                        node.ResultUnit = par.Unit;
                                }
                                folder.AddNode(node);
                                item.AddChild(node);
                            }
                        }
                        catch 
                        {
                            //
                        }
                    }
                    lstRes.Add(item);
                }

                Result = lstRes.ToArray();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    class Parameter
    {
        public string Name { get; set; }
        public string Index { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }

        List<DataItem> lstDataItems = new List<DataItem>();

        DataTable table = null;

        public DataTable Table
        {
            get
            {
                if (table != null) return table;
                table = new DataTable();

                uint minRow = uint.MaxValue, minCol = uint.MaxValue;
                foreach (var item in lstDataItems)
                {
                    if (item.RowIndex < minRow) minRow = item.RowIndex;
                    if (item.ColIndex < minCol) minCol = item.ColIndex;
                }
                uint curRow, curCol;
                foreach (var item in lstDataItems)
                {
                    curRow = item.RowIndex - minRow;
                    curCol = item.ColIndex - minCol;

                    while (table.Columns.Count <= curCol) table.Columns.Add();
                    while (table.Rows.Count <= curRow) table.Rows.Add("");
                    table.Rows[(int)curRow][(int)curCol] = item.Value;
                }

                return table;
            }
            set { table = value; }
        }

        public Parameter()
        {
            Table = new DataTable();
        }

        public void AddData(string value, uint rowindex, uint colIndex)
        {
            table = null;
            lstDataItems.Add(new DataItem() { Value = value, RowIndex = rowindex, ColIndex = colIndex });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Index); sb.Append(" | ");
            sb.Append(Name); sb.Append(" | ");
            sb.Append(Unit); sb.Append(" | ");
            sb.Append(Code); //sb.Append(" | ");
            return sb.ToString();
        }

        class DataItem
        {
            public string Value { get; set; }
            public uint RowIndex { get; set; }
            public uint ColIndex { get; set; }

            public DataItem() { }
        }
    }
    class Page
    {
        List<Parameter> lstParameters = new List<Parameter>();

        public string Name { get; set; }
        public string Index { get; set; }
        public Parameter[] Parameters { get { return lstParameters.ToArray(); } }

        public Page()
        {
            //
        }

        public void AddParameter(Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");

            lstParameters.Add(parameter);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
