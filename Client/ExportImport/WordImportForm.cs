using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text.RegularExpressions;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class WordImportForm : Form
    {
        StructureProvider strucProvider;
        Dictionary<int, NumberingInfo> dicNumbering = new Dictionary<int, NumberingInfo>();
        DataSet ds = null;

        const string strParent = "parent_";
        const string strFormulaPrefix = "formula:";

        const string strColumnIndex = "Index";
        const string strColumnName = "Name";
        const string strColumnCode = "Code";
        const string strColumnUnit = "Unit";
        const string strColumnType = "Type";
        const string strColumnFormula = "Formula";
        const string strColumnSource = "Source";
        const string strColumnPostfix = "Postfix";
        const string strColumnSortIndex = "SortIndex";

        string[] arrGroupParents = new string[] { "гр", "тэц" };

        public DataSet Result
        {
            get 
            {
                return ds;//.Clone();
            }
        }

        public WordImportForm(StructureProvider strucProvider)
        {
            this.strucProvider = strucProvider;
            InitializeComponent();

            toolTip1.SetToolTip(btnClearErrorStopList, "Clear IgnoreError list");
            toolTip1.SetToolTip(btnCheckAllFormulas, "Check all formulas");
            toolTip1.SetToolTip(btnCheckFormula, "Check formula");
            toolTip1.SetToolTip(btnCheckDoubles, "Check code duplication");
            toolTip1.SetToolTip(btnSaveFormula, "Save changes to table");
            toolTip1.SetToolTip(btnIndex, "Replace codes by indexes from Source column");
            toolTip1.SetToolTip(btnFormula, "Set Formula to 'parent_' column");
            //toolTip1.SetToolTip(btnClearErrorStopList, "Clear IgnoreError list");
            //toolTip1.SetToolTip(btnClearErrorStopList, "Clear IgnoreError list");
            this.DoubleBuffered = true;
        }

        public void LoadFile(string filename)
        {
            //Microsoft.Office.Interop.Word.Application app = null;
            //StringBuilder sb = new StringBuilder();
            //object file = filename;
            //object dummy = Missing.Value;
            //Range rng;
            ////List<string> lstStr = new List<string>();
            //FileStream fstream = null;
            //StreamWriter sw = null;

            //DateTime start = DateTime.Now;

            //try
            //{
            //    ds.Tables.Clear();
            //    cbxTables.Items.Clear();

            //    fstream = new FileStream("C:\\parse.txt", FileMode.Create);
            //    sw = new StreamWriter(fstream, Encoding.UTF8);

            //    app = new Microsoft.Office.Interop.Word.Application();
            //    app.Visible = true;
            //    Document doc = app.Documents.Open(ref file, ref dummy, ref dummy, ref dummy,
            //        ref dummy, ref dummy, ref dummy,
            //        ref dummy, ref dummy, ref dummy,
            //        ref dummy, ref dummy, ref dummy,
            //        ref dummy, ref dummy, ref dummy);

            //    System.Data.DataTable table = null;
            //    foreach (Table item in doc.Tables)
            //    {
            //        try
            //        {
            //            table = new System.Data.DataTable();
            //            rng = item.Range;
            //            foreach (Cell cell in rng.Cells)
            //            {
            //                AddCell(table, cell);
            //                Range ptr = cell.Range;
            //                sw.Write(string.Format("{0}({1},{2});",
            //                    ptr.Text,
            //                    cell.RowIndex.ToString(),
            //                    cell.ColumnIndex.ToString()));
            //            }
            //            sw.WriteLine();
            //        }
            //        catch (Exception ex)
            //        {
            //            sw.Write("%bug{" + ex.Message + "}%");
            //        }
            //        finally
            //        {
            //            if (table != null)
            //                ds.Tables.Add(table);
            //            sw.WriteLine();
            //        }
            //    }

            //    MessageBox.Show("OK " + (DateTime.Now - start).ToString());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    if (sw != null) sw.Close();
            //    if (app != null)
            //        app.Quit(ref dummy, ref dummy, ref dummy);

            //    foreach (System.Data.DataTable item in ds.Tables)
            //        cbxTables.Items.Add(ds.Tables.IndexOf(item).ToString());
            //    if (cbxTables.Items.Count > 0) cbxTables.SelectedIndex = 0;
            //}

            LoadFile2(filename);
        }

        private void LoadFile2(string filename)
        {
            try
            {
                ds = new DataSet();
                System.Data.DataTable table;
                //string filename = "C:\\light.docx";
                using (WordprocessingDocument wpd = WordprocessingDocument.Open(filename, false))
                {
                    MainDocumentPart md = wpd.MainDocumentPart;
                    Numbering numb = md.NumberingDefinitionsPart.Numbering;
                    var numbs = numb.Elements<NumberingInstance>();
                    foreach (var item in numbs)
                    {
                        NumberingInfo ni = new NumberingInfo(item.NumberID);

                        var abnums = from elem in numb.Elements<AbstractNum>()
                                     where elem.AbstractNumberId == item.AbstractNumId.Val.Value
                                     select elem;
                        AbstractNum abnum = null;
                        if (abnums.Count() > 0) abnum = abnums.First();
                        if (abnum != null)
                        {
                            var levels = abnum.Elements<Level>();
                            if (levels.Count() > 0)
                            {
                                foreach (var level in abnum.Elements<Level>())
                                {
                                    if (level.LevelIndex != null && level.LevelText != null)
                                    {
                                        if (level.StartNumberingValue != null)
                                            ni.AddLevel(level.LevelIndex.Value, level.StartNumberingValue.Val.Value, level.LevelText.Val.Value);
                                        else
                                            ni.AddLevel(level.LevelIndex.Value, level.LevelText.Val.Value);
                                    }
                                }
                            }
                            //abnum.
                        }
                        dicNumbering[item.NumberID] = ni;
                    }
                    var tables = md.Document.Body.ChildElements.OfType<DocumentFormat.OpenXml.Wordprocessing.Table>();
                    //ds.Tables.Clear();
                    foreach (var item in tables)
                    {
                        table = CreateTable(item);
                        ds.Tables.Add(table);
                    }

                    UpdateCombo();
                }
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
        }

        private void UpdateCombo()
        {
            cbxTables.Items.Clear();
            foreach (System.Data.DataTable item in ds.Tables)
                cbxTables.Items.Add(ds.Tables.IndexOf(item).ToString());
            if (cbxTables.Items.Count > 0) cbxTables.SelectedIndex = 0;
        }

        private System.Data.DataTable CreateTable(DocumentFormat.OpenXml.Wordprocessing.Table wtable)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            bool colfilled = false;

            if (wtable == null) throw new ArgumentNullException("wtable");

            TableGrid tblGrid = wtable.ChildElements.First<TableGrid>();
            if (tblGrid != null)
            {
                IEnumerable<GridColumn> cols = tblGrid.ChildElements.OfType<GridColumn>();
                DataColumn column;
                foreach (var item in cols)
                {
                    column = table.Columns.Add();
                    column.Caption = "";
                }
            }
            IEnumerable<TableRow> rows = wtable.ChildElements.OfType<TableRow>();
            IEnumerable<TableCell> cells;
            DataRow row = null;
            int cellindex;
            //int minrowindex = 0;
            bool skiprow;

            foreach (var item in rows)
            {
                skiprow = false;
                cells = item.ChildElements.OfType<TableCell>();

                //if (colfilled)
                //{
                row = table.NewRow();
                //}
                foreach (var cell in cells)
                    if (cell.TableCellProperties != null && cell.TableCellProperties.VerticalMerge != null)
                    {
                        skiprow = true;
                        break;
                    }
                colfilled &= !skiprow;

                cellindex = 0;
                foreach (var cell in cells)
                {
                    if (colfilled)
                    {
                        if (cellindex >= table.Columns.Count) throw new Exception("Row index out of range");
                        if (row == null) throw new Exception("Fatal: Row is null");
                        row[cellindex] = GetText(cell);
                    }
                    else
                    {
                        //if (cell.TableCellProperties != null && cell.TableCellProperties.VerticalMerge != null)
                        //    skiprow = true;
                        if (!string.IsNullOrEmpty(cell.InnerText))
                        {
                            table.Columns[cellindex].Caption = cell.InnerText;
                            //
                        }
                    }

                    if (cell.TableCellProperties != null && cell.TableCellProperties.GridSpan != null)
                        cellindex += cell.TableCellProperties.GridSpan.Val.Value;
                    else
                        cellindex++;
                }

                if (!colfilled || skiprow)
                {
                    colfilled = true;
                    foreach (DataColumn col in table.Columns)
                    {
                        if (string.IsNullOrEmpty(col.Caption))
                        {
                            colfilled = false;
                            break;
                        }
                        else
                        {
                            if (!table.Columns.Contains(col.Caption))
                                col.ColumnName = col.Caption;
                        }
                    }
                }
                //colfilled &= !skiprow;

                if (colfilled && row != null && !skiprow)
                    table.Rows.Add(row);
                //rowindex++;
            }

            foreach (DataColumn item in table.Columns) item.Caption = "";
            return table;
        }

        private string GetText(OpenXmlElement cell)
        {
            return GetText(cell, false);
        }
        private string GetText(OpenXmlElement cell, bool wrap)
        {
            StringBuilder sb = new StringBuilder();
            string tmp = "";
            
            if (cell == null) return "";

            switch (cell.XmlQualifiedName.Name)
            {
                case "tc":
                case "p":
                case "pPr":
                case "oMathPara":
                    foreach (var item in cell.ChildElements)
                    {
                        if (item is NumberingProperties)
                        {
                            NumberingProperties np = (NumberingProperties)item;
                            if (np.NumberingId != null && np.NumberingLevelReference != null)
                            {
                                if (dicNumbering.ContainsKey(np.NumberingId.Val.Value))
                                {
                                    tmp = dicNumbering[np.NumberingId.Val].GetNumberValue(np.NumberingLevelReference.Val.Value);
                                    dicNumbering[np.NumberingId.Val].IncreaseValue(np.NumberingLevelReference.Val.Value);
                                }
                            }
                        }
                        else tmp = GetText(item);
                        //if (sb.Length > 0)
                        //    sb.AppendLine(tmp);
                        //else
                            sb.Append(tmp);
                    }
                    break;
                case "den":
                case "num":
                    StringBuilder ssb = new StringBuilder();
                    string str;
                    bool m = false;
                    foreach (var item in cell.ChildElements)
                    {
                        str = GetText(item);
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (ssb.Length > 0) m = true;
                            ssb.Append(str);
                        }
                    }
                    if (m) sb.Append(string.Format("({0})", ssb.ToString()));
                    else sb.Append(ssb.ToString());
                    break;
                case "oMath":
                case "e":
                case "sub":
                case "sup":                
                case "deg":
                    foreach (var item in cell.ChildElements)
                        sb.Append(GetText(item));
                    break;

                case "d":
                    Delimiter delimiter = cell as Delimiter;
                    string beg = null, end = null;

                    if (delimiter != null)
                    {
                        if (delimiter.DelimiterProperties != null)
                        {
                            if (delimiter.DelimiterProperties.BeginChar != null &&
                                delimiter.DelimiterProperties.BeginChar.Val != null)
                                beg = delimiter.DelimiterProperties.BeginChar.Val.Value;
                            if (delimiter.DelimiterProperties.EndChar != null &&
                                delimiter.DelimiterProperties.EndChar.Val != null)
                                end = delimiter.DelimiterProperties.EndChar.Val.Value;
                        }

                        if (string.IsNullOrEmpty(beg)) beg = "(";
                        if (string.IsNullOrEmpty(end)) end = ")";
                    }

                    sb.Append(beg);
                    foreach (var item in delimiter.ChildElements)
                    {
                        //if (sb.Length > 0)
                        //    sb.AppendLine(GetText(item));
                        //else
                            sb.Append(GetText(item));
                    }
                    sb.Append(end);

                    break;

                case "sSubSup":
                    sb.Append(GetText(cell.ChildElements.First<Base>()));
                    sb.Append("^" + GetText(cell.ChildElements.First<SuperArgument>(), true));
                    sb.Append("_" + GetText(cell.ChildElements.First<SubArgument>(), true));
                    break;
                case "sSub":
                    sb.Append(GetText(cell.ChildElements.First<Base>()));
                    sb.Append("_" + GetText(cell.ChildElements.First<SubArgument>(), true));
                    break;
                case "sSup":
                    sb.Append(GetText(cell.ChildElements.First<Base>()));
                    sb.Append("^" + GetText(cell.ChildElements.First<SuperArgument>(), true));
                    break;

                //case "sub":
                //    sb.Append("_");
                //    break;
                //case "sup":
                //    sb.Append("^");
                //    break;
                case "bar":
                case "acc":
                    sb.Append("\\bar " + GetText(cell.ChildElements.First<Base>(), true));
                    break;
                //case "e":
                //    sb.Append(GetText(cell.ChildElements.First<DocumentFormat.OpenXml.Math.Run>()));
                //    break;
                case "r":
                    if (cell.Prefix == "w")
                        sb.Append(GetText(cell.ChildElements.First<DocumentFormat.OpenXml.Wordprocessing.Text>()));
                    else
                        if (cell.Prefix == "m")
                            sb.Append(GetText(cell.ChildElements.First<DocumentFormat.OpenXml.Math.Text>()));
                    break;
                case "t"://Text
                    sb.Append(ReplaceTeX(cell.InnerText));
                    break;
                case "f"://fraction
                    sb.Append(GetText(cell.ChildElements.First<Numerator>()));
                    sb.Append("/");
                    sb.Append(GetText(cell.ChildElements.First<Denominator>()));
                    break;
                case "rad":
                    sb.Append(string.Format("sqrt({0})", GetText(cell.ChildElements.First<Base>())));
                    break;
                case "eqArr":
                    //sb.Append("\\begin{eqnarray} ");
                    foreach (var item in cell.ChildElements)
                        sb.Append(GetText(item));
                    //sb.Append("\\end{eqnarray} ");
                    break;
                case "nary":
                    var sub = GetText(cell.ChildElements.First<SubArgument>());
                    var sup = GetText(cell.ChildElements.First<SuperArgument>());
                    var bas = GetText(cell.ChildElements.First<Base>());
                    sb.Append(string.Format("nary({0},{1},{2})", sub, sup, bas));
                    break;
                case "tcPr":
                case "rPr":
                case "dPr":
                case "jc":
                case "ind":
                case "ctrlPr":
                case "bookmarkStart":
                case "spacing":
                case "contextualSpacing":
                case "pStyle":
                    //nothing
                    break;
                default:
                    //sb.Append("%n/a%");
                    break;
            }

            tmp = sb.ToString();
            if (wrap && tmp.Length > 1) return "{" + tmp + "}";
            return tmp;
        }

        private void cbxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTableName.Text = "";
            dgv.DataSource = null;
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            if (cbxTables.SelectedIndex == -1) return;
            dgv.DataSource = ds.Tables[cbxTables.SelectedIndex];
            txtTableName.Text = ds.Tables[cbxTables.SelectedIndex].TableName;
        }

        private string GetText(DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            //Range rng;
            //string res;

            //if (cell == null) throw new ArgumentNullException("cell");

            //rng = cell.Range;
            //string xml = rng.get_XML(false);
            //res = rng.Text.Replace("\r", "");
            //res = res.Replace("\a", "");
            ////foreach (OMath item in rng.OMaths)
            ////{
            ////    res = ConvertMath(item);
            ////}
            ////cell.Tables[0].Application.ActiveDocument.Fields
            //return res;

            return "text";
        }

        private void AddStringBefore(StringBuilder sb, string text)
        {
            int s = 0;
            int i;

            if (sb == null) throw new ArgumentNullException("sb");

            for (i = sb.Length - 1; i >= 0; i--)
            {
                if (sb[i] == ')') s--;
                else
                    if (sb[i] == '(')
                    {
                        s++;
                        if (s == 0) break;
                    }
                    else
                        if (sb[i] == ' ' && i < sb.Length - 1 && s == 0)
                            break;
            }
            if (i < 0) i = 0;
            sb.Insert(i, text);
        }

        private string ReplaceTeX(string sTeX)
        {
            StringBuilder sb = new StringBuilder();
            string res = sTeX;
            const char c773 = (char)773; //" ̅"

            foreach (var ch in sTeX)
            {
                switch (ch)
                {
                    case '〖': break;
                    case '〗': break;
                    case c773: AddStringBefore(sb, "\\bar "); /*sb.Append("\\bar");*/ break;
                    case '^': sb.Append(@"\^"); break;

                    //case 'A':
                    case 'Α': sb.Append("\\Alpha "); break;
                    case 'Β': sb.Append("\\Beta "); break;
                    case 'Γ': sb.Append("\\Gamma "); break;
                    case 'Δ':
                    case '∆': sb.Append("\\Delta "); break;
                    case 'Ε': sb.Append("\\Epsilon "); break;
                    case 'Ζ': sb.Append("\\Zeta "); break;
                    case 'Η': sb.Append("\\Eta "); break;
                    case 'Θ': sb.Append("\\Theta "); break;
                    case 'Ι': sb.Append("\\Iota "); break;
                    case 'Κ': sb.Append("\\Kappa "); break;
                    case 'Λ': sb.Append("\\Lambda "); break;
                    case 'Μ': sb.Append("\\Mu "); break;
                    case 'Ν': sb.Append("\\Nu "); break;
                    case 'Ξ': sb.Append("\\Xi "); break;
                    case 'Π': sb.Append("\\Pi "); break;
                    case 'Ρ': sb.Append("\\Pho "); break;
                    case 'Σ': sb.Append("\\Sigma "); break;
                    case 'Τ': sb.Append("\\Tau "); break;
                    case 'Υ': sb.Append("\\Upsilon "); break;
                    case 'Φ': sb.Append("\\Phi "); break;
                    case 'Χ': sb.Append("\\Chi "); break;
                    case 'Ψ': sb.Append("\\Psi "); break;
                    case 'Ω': sb.Append("\\Omega "); break;

                    case 'α': sb.Append("\\alpha "); break;
                    case 'β': sb.Append("\\beta "); break;
                    case 'γ': sb.Append("\\gamma "); break;
                    case 'δ': sb.Append("\\delta "); break;
                    case 'ε': sb.Append("\\epsilon "); break;
                    case 'ζ': sb.Append("\\zeta "); break;
                    case 'η': sb.Append("\\eta "); break;
                    case 'θ': sb.Append("\\theta "); break;
                    case 'ι': sb.Append("\\iota "); break;
                    case 'κ': sb.Append("\\kappa "); break;
                    case 'λ': sb.Append("\\lambda "); break;
                    case 'μ': sb.Append("\\mu "); break;
                    case 'ν': sb.Append("\\nu "); break;
                    case 'ξ': sb.Append("\\xi "); break;
                    case 'π': sb.Append("\\pi "); break;
                    case 'ρ': sb.Append("\\pho "); break;
                    case 'σ': sb.Append("\\sigma "); break;
                    case 'τ': sb.Append("\\tau "); break;
                    case 'υ': sb.Append("\\upsilon "); break;
                    case 'φ': sb.Append("\\phi "); break;
                    case 'χ': sb.Append("\\chi "); break;
                    case 'ψ': sb.Append("\\psi "); break;
                    case 'ω': sb.Append("\\omega "); break;

                    //case '1': sb.Append(""); break;

                    default: sb.Append(ch); break;
                }
            }

            //ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ
            return CleanTeX(sb.ToString());
        }

        private bool IsSpecChar(char c)
        {
            switch (c)
            {
                case '{':
                case '}':
                case '_':
                case '^':
                    return true;
                default:
                    return false;
            }
        }
        private bool IsDelimiter(char c)
        {
            return char.IsPunctuation(c) || char.IsSeparator(c);
        }

        private string CleanTeX(string txt)
        {
            StringBuilder sb = new StringBuilder();
            bool skip;
            char prev = '0';
            int s = 0;
            int i, j;

            for (i = 0; i < txt.Length; i++)
            {
                skip = false;
                switch (txt[i])
                {
                    case ' ':
                        if ((i > 0 && IsSpecChar(txt[i - 1]))
                            || (i + 1 < txt.Length && IsSpecChar(txt[i + 1])))
                            skip = true;
                        break;
                    //case '^':
                    //case '_':

                    //    break;
                    case '(':
                        if (prev == '^' || prev == '_')
                        {
                            s = 1;
                            for (j = i + 1; j < txt.Length; j++)
                            {
                                if (txt[j] == '(')
                                    s++;
                                else
                                    if (txt[j] == ')') s--;
                                if (s == 0) break;
                            }
                            if (s == 0)
                            {
                                string ss = txt.Substring(i + 1, j - i - 1);
                                ss = CleanTeX(ss);
                                if (ss.Length > 1)
                                {
                                    sb.Append("{");
                                    sb.Append(ss);
                                    sb.Append("}");
                                }
                                else
                                    sb.Append(ss);
                                i = j;
                                skip = true;
                            }
                        }
                        break;
                    case ')':
                        break;
                    default:
                        if (prev == '^' || prev == '_')
                        {
                            if (i > 1 && txt[i - 2] == '\\') break;
                            string ss;

                            for (j = i; j < txt.Length; j++)
                            {
                                if (IsSpecChar(txt[j])
                                    || char.IsSeparator(txt[j])
                                    || char.IsPunctuation(txt[i])) break;
                            }

                            ss = txt.Substring(i, j - i);
                            if (ss.Length > 0)
                            {
                                ss = CleanTeX(ss);
                                if (ss.Length > 1)
                                {
                                    sb.Append("{");
                                    sb.Append(ss);
                                    sb.Append("}");
                                }
                                else
                                    sb.Append(ss);
                                if (j < txt.Length)
                                    i = j - 1;
                                else
                                    i = j;
                                skip = true;
                            }
                        }
                        break;
                }

                if (!skip)
                {
                    prev = txt[i];
                    sb.Append(prev);
                }
            }

            return sb.ToString();
        }

        private void UpdateColumns()
        {
            System.Data.DataTable table;

            table = dgv.DataSource as System.Data.DataTable;
            if (table == null) return;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (!string.IsNullOrEmpty(table.Columns[i].Caption))
                {
                    dgv.Columns[i].HeaderText =
                        string.Format("[{0}] {1}", table.Columns[i].Caption,
                        table.Columns[i].ColumnName);
                }
                else
                    dgv.Columns[i].HeaderText = table.Columns[i].ColumnName;
            }
        }

        private void AddColumnInfo(string info)
        {
            DataTable table;
            string col;

            if (dgv.SelectedCells.Count > 0)
            {
                DataGridViewCell cell = dgv.SelectedCells[0];
                
                table = dgv.DataSource as DataTable;
                if (table == null) return;

                col = table.Columns[cell.ColumnIndex].ColumnName;

                if (chbxAllColumns.Checked)
                {
                    foreach (DataTable item in ds.Tables)
                    {
                        if (item.Columns.Contains( col) && !item.Columns.Contains(info))
                        {
                            item.Columns[col].ColumnName = info;
                        }
                    }
                }
                else
                {
                    if (table.Columns.Contains(info))
                    {
                        MessageBox.Show("Column with the same name is already exists.");
                        return;
                    }
                    table.Columns[cell.ColumnIndex].ColumnName = info;
                    //switch (table.Columns[cell.ColumnIndex].ColumnName)
                    //{
                    //    case "Code":
                    //        foreach (DataRow row in table.Rows)
                    //            row[cell.ColumnIndex] = ReplaceTeX(row[cell.ColumnIndex].ToString());
                    //        break;
                    //}
                }
                //UpdateColumns();
            }
        }

        private void codeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnCode);
        }

        private void unitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnUnit);
        }

        private void customInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripTextBox1.Text)) return;
            AddColumnInfo(toolStripTextBox1.Text);
        }

        private void clearInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo("");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnIndex);
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnName);
        }

        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnType);
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            if (cbxTables.SelectedIndex == -1) return;
            if (!ds.Tables.Contains(txtTableName.Text))
            {
                ds.Tables[cbxTables.SelectedIndex].TableName = txtTableName.Text;
            }
            else
                MessageBox.Show("Table with the same name is already exists.");
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            if (cbxTables.SelectedIndex == -1) return;
            if (MessageBox.Show("Are you sure?", "Deleting", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ds.Tables.Remove(ds.Tables[cbxTables.SelectedIndex]);
                UpdateCombo();
            }
        }

        private void btnAddPostfix_Click(object sender, EventArgs e)
        {
            DataTable table;
            
            table = dgv.DataSource as DataTable;
            if (cbxTables.SelectedIndex == -1 || table == null) return;
            if (!table.Columns.Contains(strColumnPostfix)) table.Columns.Add(strColumnPostfix);

            foreach (DataRow item in table.Rows)
            {
                if (!chbxEmptyPostfix.Checked || string.IsNullOrEmpty(item[strColumnPostfix].ToString()))
                    item[strColumnPostfix] = txtPostfix.Text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileStream fstream = null;

            try
            {
                if (ds == null) return;

                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fstream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fstream, ds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (fstream != null) fstream.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fstream = null;

            try
            {
                if (ds == null) return;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fstream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    ds = (DataSet)bf.Deserialize(fstream);
                    UpdateCombo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (fstream != null) fstream.Close();
            }
        }

        private void btnAddBefore_Click(object sender, EventArgs e)
        {
            AddCellText(txtAddText.Text, true);
        }

        private void AddCellText(string text, bool before)
        {
            if ((dgv.DataSource as DataTable) == null) return;
            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                if (before)
                    cell.Value = text + cell.Value;
                else
                    cell.Value += text;
            }
        }

        private void btnAddAfter_Click(object sender, EventArgs e)
        {
            AddCellText(txtAddText.Text, false);
        }

        private void btnClearNewLine_Click(object sender, EventArgs e)
        {
            if ((dgv.DataSource as DataTable) == null) return;
            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                if (cell.Value != null)
                {
                    string tmp = cell.Value.ToString();

                    tmp = tmp.Replace("\r", "");
                    tmp = tmp.Replace("\n", "");

                    cell.Value = tmp;
                }
            }
        }

        private void formulaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnFormula);
        }

        private void sourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumnInfo(strColumnSource);
        }

        private void ReplaceIndex()
        {
            foreach (DataTable table in ds.Tables)
            {
                if (!chbxIndexAllTables.Checked || !chbxAllRows.Checked)
                    if (dgv.DataSource != table) continue;
                if (!table.Columns.Contains(strColumnSource)
                    || !table.Columns.Contains(strColumnCode)
                    || !table.Columns.Contains(strColumnFormula)
                    || !table.Columns.Contains(strColumnIndex))
                    //throw new Exception(string.Format("Table {0} not contains required column", table.TableName));
                    continue;
                foreach (DataRow row in table.Rows)
                {
                    if (!chbxAllRows.Checked)
                        if (table.Rows.IndexOf(row) != dgv.SelectedCells[0].OwningRow.Index)
                            continue;

                    var formula = row[strColumnFormula].ToString();
                    formula = formula.Replace('∙', '*');//8729
                    formula = formula.Replace('·', '*');//183
                    var idxpairs = SplitSource(row[strColumnSource].ToString());
                    var pairs = idxpairs.OrderByDescending(x => x.Item1.Length);
                    foreach (var pair in pairs)
                    {
                        if (string.IsNullOrEmpty(pair.Item1) || string.IsNullOrEmpty(pair.Item2))
                            throw new Exception(string.Format("Index pair parse error (table - {0};row - {1}): {2};{3}",
                                table.TableName,
                                table.Rows.IndexOf(row),
                                pair.Item1,
                                pair.Item2));
                        //Regex reg = new Regex("");
                        //reg.Match(formula);
                        int start = 0;
                        int idx = 0;
                        int codelen = pair.Item1.Length;
                        string newcode = "$" + pair.Item2 + "$";
                        int newcodelen = newcode.Length;
                        bool ok;
                        while (idx != -1)
                        {
                            ok = false;
                            idx = formula.IndexOf(pair.Item1, start);

                            if (idx != -1)
                            {
                                if (idx != 0)
                                {
                                    if (char.IsWhiteSpace(formula[idx - 1]))
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        for (int i = idx - 2; i >= 0; i--)
                                        {
                                            if (formula[i] == '\\')
                                            {
                                                //
                                                break;
                                            }
                                            if (char.IsWhiteSpace(formula[i])
                                                || char.IsPunctuation(formula[i])
                                                || char.IsSymbol(formula[i]))
                                            {
                                                ok = true;
                                                break;
                                            }
                                            sb.Append(formula[i]);
                                        }
                                        if (!ok)
                                        {
                                            char[] ss = sb.ToString().ToCharArray();
                                            Array.Reverse(ss);
                                            string s = new string(ss).ToUpper();
                                            if (s == "ВЕС" || s == "СУММА")
                                                ok = true;
                                        }
                                    }
                                    else
                                        if (char.IsPunctuation(formula[idx - 1])
                                            || char.IsSymbol(formula[idx - 1]))
                                            ok = true;
                                }
                                else
                                    ok = true;
                                if (ok)
                                {
                                    formula = formula.Remove(idx, codelen);
                                    formula = formula.Insert(idx, newcode);
                                    //formula = formula.Replace(pair.Item1, "$" + pair.Item2 + "$");
                                    start = idx + newcodelen;
                                }
                                else
                                    start = idx + codelen;
                            }
                        }
                    }
                    if (chbxIndexChangeFormula.Checked)
                        row[strColumnFormula] = formula;
                }
            }
        }

        private void btnIndex_Click(object sender, EventArgs e)
        {
            try
            {
                ReplaceIndex();
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataRow FindRow(string column, string value)
        {
            var val = value.Trim();

            foreach (DataTable table in ds.Tables)
            {
                if (!table.Columns.Contains(column)) continue;
                foreach (DataRow row in table.Rows)
                {
                    if (row[column].ToString().Trim() == val)
                        return row;
                }
            }

            return null;
        }
        private string[] FindParents(DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            var res = new List<string>();

            foreach (DataColumn column in row.Table.Columns)
            {
                if (column.ColumnName.StartsWith(strParent))
                {
                    if (row[column].ToString().Length > 0)
                    {
                        var col = column.ColumnName.Substring(strParent.Length);
                        if (col.Length > 0) res.Add(col);
                    }
                }
            }

            return res.ToArray();
        }
        private string FormatCode(DataRow row, string parent, bool addParamQuotes = true, bool checkExisting = true)
        {
            string res;
            if (row == null) throw new ArgumentNullException("row");

            if (!row.Table.Columns.Contains(strColumnCode))
                throw new Exception(string.Format("Table {0} not contains column {1}", row.Table.TableName, strColumnCode));
            var code = row[strColumnCode].ToString();
            var postfix = "";
            if (checkExisting)
            {
                var parents = FindParents(row);
                if (!parents.Contains(parent))
                {   
                    if (parents.Length == 1 && arrGroupParents.Contains(parents[0]))
                        parent = parents[0];
                    else
                        if (parents.Length == 0)
                            parent = "";
                        else
                            throw new Exception(string.Format("Param not found: code:{0}; parent:{1}", code, parent));
                }
            }
            var par = string.IsNullOrEmpty(parent) ? "" : "," + parent;
            if (row.Table.Columns.Contains(strColumnPostfix))
                postfix = "," + row[strColumnPostfix].ToString();

            res = string.Format("{0}{1}{2}", code, par, postfix);
            if (addParamQuotes)
                return string.Format("${0}$", res);
            else
                return res;
        }

        private Tuple<string, string>[] SplitSource(string source)
        {
            var res = new List<Tuple<string, string>>();
            StringBuilder sb = new StringBuilder();
            var idx = new StringBuilder();
            int lastpos = 0;
            int state = 0;
            string x;
            string txt = source;

            bool hasPoint = false;

            Regex reg = new Regex(@"REF_\{.+?\} \\r \\h");
            var m = reg.Match(txt);
            while (m != null && m.Success)
            {
                txt = txt.Replace(m.Value, "");
                m = m.NextMatch();
            }
            txt = txt.Replace(@"\* MERGEFORMAT", "");
            //foreach (Match match in reg.Matches(txt))
            //{
            //    if (match.Success)
            //    {
            //        //
            //    }
            //}
            //int p;
            //while ((p = txt.IndexOf("REF_{")) != -1)
            //{
            //    //
            //}

            for (int i = 0; i < txt.Length; i++)
            {
                if (!char.IsWhiteSpace(txt[i]))
                {
                    if (state == 1 && txt[i] == '.')
                    {
                        hasPoint = true;
                    }
                    else
                        if (char.IsDigit(txt[i]) && lastpos > 0) state = 1;
                        else
                            if (txt[i] != '-' && txt[i] != '–' && txt[i] != '+') state = 0;
                }

                switch (state)
                {
                    case 0://char
                        if (idx.Length >= 3 && hasPoint)
                        {
                            if (lastpos > 0) x = sb.ToString().Substring(0, lastpos);
                            else x = sb.ToString();
                            res.Add(new Tuple<string, string>(x.Trim(), idx.ToString().Trim()));
                            sb.Clear();
                            lastpos = 0;
                        }
                        else
                            sb.Append(idx.ToString());
                        idx.Clear();
                        hasPoint = false;
                        if (txt[i] == '-' || txt[i] == '–') lastpos = sb.Length;
                        sb.Append(txt[i]);
                        break;
                    case 1://digit
                        //
                        idx.Append(txt[i]);
                        break;
                }
            }
            if (lastpos > 0) x = sb.ToString().Substring(0, lastpos);
            else x = sb.ToString();
            x = x.Trim();
            var id = idx.ToString().Trim();
            if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(id))
                res.Add(new Tuple<string, string>(x.Trim(), idx.ToString().Trim()));

            return res.ToArray();
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 0 || !dgv.Columns.Contains(strColumnFormula))
                txtFormula.Text = "";
            else
            {
                var row = dgv.SelectedCells[0].OwningRow;
                txtFormula.Text = row.Cells[strColumnFormula].Value.ToString();
            }
        }

        private void ReplaceFormula()
        {
            var selectedRows = new List<int>();
            var sb = new StringBuilder();

            if (!chbxAllRows.Checked)
            {
                for (int i = 0; i < dgv.SelectedCells.Count; i++)
                {
                    var ri = dgv.SelectedCells[i].RowIndex;
                    if (!selectedRows.Contains(ri))
                        selectedRows.Add(ri);
                }
            }

            foreach (DataTable table in ds.Tables)
            {
                var row_index = "";

                if (!chbxIndexAllTables.Checked || !chbxAllRows.Checked)
                    if (dgv.DataSource != table) continue;
                if (!table.Columns.Contains(strColumnFormula))
                    continue;
                foreach (DataRow row in table.Rows)
                {
                    if (!chbxAllRows.Checked)
                        if (!selectedRows.Contains(table.Rows.IndexOf(row))) continue;

                    if (table.Columns.Contains(strColumnIndex))
                        row_index = row[strColumnIndex].ToString();
                    else
                        row_index = "-";
                    var parents = FindParents(row);
                    var formula = row[strColumnFormula].ToString();
                    var regp = new Regex(@"\[\s*(.+?)\s*\]:\s*(.*)");
                    Match mp;
                    string pf;
                    foreach (var parent in parents)
                    {
                        pf = formula;
                        mp = regp.Match(pf);
                        while (mp != null && mp.Success)
                        {
                            var ps = mp.Groups[1].Value.Split(',');
                            bool ok = false;
                            foreach (var par in ps)
                            {
                                if (par.Trim().ToLower() == parent.Trim().ToLower())
                                {
                                    pf = pf.Replace(mp.Value, mp.Groups[2].Value);
                                    ok = true;
                                    break;
                                }
                            }
                            if (!ok)
                                pf = pf.Replace(mp.Value, "");
                            mp = mp.NextMatch();
                        }
                        row[strParent + parent] = strFormulaPrefix + pf;
                    }
                    string index;
                    regp = new Regex(@"\$.+?\$");
                    var m = regp.Match(formula);
                    while (m != null && m.Success)
                    {
                        index = m.Value.Substring(1, m.Value.Length - 2);
                        if (index.Length > 0 && char.IsDigit(index[0]))
                        {
                            DataRow[] r;
                            var idx_sep = '-';
                            if (index.Contains(idx_sep))
                            {
                                var idxs = index.Split(idx_sep);
                                if (idxs.Length != 2)
                                {
                                    sb.AppendLine(string.Format("MultiIdxParseError: r_idx:{0};idx:{1}", row_index, index));
                                    r = null;
                                }
                                else
                                {
                                    var r1 = FindRow(strColumnIndex, idxs[0]);
                                    var r2 = FindRow(strColumnIndex, idxs[1]);
                                    if (r1 == null || r2 == null || r1.Table != r2.Table)
                                    {
                                        sb.AppendLine(string.Format("MultiIdxDiff: r_idx:{0};idx:{1}", row_index, index));
                                        r = null;
                                    }
                                    else
                                    {
                                        var lst = new List<DataRow>();
                                        var mx = r2.Table.Rows.IndexOf(r2);
                                        for (int ri = r1.Table.Rows.IndexOf(r1); ri <= mx; ri++)
                                            lst.Add(r1.Table.Rows[ri]);
                                        r = lst.ToArray();
                                    }
                                }
                            }
                            else
                                r = new DataRow[] { FindRow(strColumnIndex, index) };
                            
                            //var r = FindRow(strColumnIndex, index);
                            if (r != null && r.Length > 0)
                            {
                                var found_r = false;
                                for (int par_i = 0; par_i < parents.Length; par_i++)
                                {
                                    var parent = parents[par_i];
                                    var s_parent = "";
                                    var found_p = false;
                                    var current_row = par_i < r.Length ? par_i : r.Length - 1;
                                    if (r[current_row] == null) continue;//параметр отсутствует?
                                    var r_parents = FindParents(r[current_row]);
                                    if (r_parents.Length > 0)
                                    {
                                        foreach (var r_parent in r_parents)
                                        {
                                            if (parent == r_parent)
                                            {
                                                s_parent = r_parent;
                                                found_p = true;
                                                break;
                                            }
                                        }
                                        if (!found_p && r_parents.Length == 1 &&
                                            (arrGroupParents.Contains(r_parents[0].ToLower())))
                                        {
                                            s_parent = r_parents[0];
                                            found_p = true;
                                        }
                                    }
                                    else
                                    {
                                        s_parent = null;
                                        found_p = true;
                                    }
                                    if (found_p)
                                    {
                                        var code = FormatCode(r[current_row], s_parent);
                                        formula = row[strParent + parent].ToString();
                                        row[strParent + parent] = formula.Replace(m.Value, code);
                                        found_r = true;
                                    }
                                }
                                if (!found_r)
                                    sb.AppendLine(string.Format("Can't change: r_idx:{0};idx:{1}", row_index, index));
                            }
                            else
                                sb.AppendLine(string.Format("NotFound: r_idx:{0};idx:{1}", row_index, index));
                        }
                        m = m.NextMatch();
                    }
                }
            }

            if (sb.Length != 0) MessageBox.Show("index errors: " + sb.ToString());
        }

        private void btnFormula_Click(object sender, EventArgs e)
        {
            try
            {
                ReplaceFormula();
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveFormula_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count != 0 && dgv.Columns.Contains(strColumnFormula))
            {
                var row = dgv.SelectedCells[0].OwningRow;
                row.Cells[strColumnFormula].Value = txtFormula.Text;
            }
        }

        private void btnCheckDoubles_Click(object sender, EventArgs e)
        {
            try
            {
                var repeats = new List<string>();
                //var dic = new Dictionary<string, int>();
                var res = new HashSet<string>();
                string code;

                foreach (DataTable table in ds.Tables)
                {
                    if (!table.Columns.Contains(strColumnCode))
                        continue;
                    foreach (DataRow row in table.Rows)
                    {
                        var parents = FindParents(row);
                        string[] pars;
                        if (parents == null || parents.Length == 0)
                            pars = new string[] { null };
                        else
                            pars = parents;
                        foreach (var parent in pars)
                        {
                            code = FormatCode(row, parent);
                            if (res.Contains(code))
                                repeats.Add(code);
                            else
                                res.Add(code);
                        }
                    }
                }
                if (repeats.Count > 0)
                {
                    var sb = new StringBuilder();
                    var f = (from elem in repeats
                            select elem).Take(50);
                    foreach (var item in f)
                        sb.AppendLine(item);
                    sb.AppendLine(string.Format("Total: {0}", repeats.Count));
                    MessageBox.Show(sb.ToString());
                }
                else
                    MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCheckFormula_Click(object sender, EventArgs e)
        {
            CheckFormula();
        }

        private void btnCheckAllFormulas_Click(object sender, EventArgs e)
        {
            try
            {
                CheckFormula(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        List<FormulaError> lstErrorStopList = new List<FormulaError>();
        private void CheckFormula(bool all = false)
        {
            try
            {
                var lstFormula = new List<string>();
                var lst = new List<FormulaError>();
                string code, idx;
                
                lbxReport.Items.Clear();
                foreach (DataTable table in ds.Tables)
                {
                    if (!table.Columns.Contains(strColumnFormula) || (!all && table != dgv.DataSource))
                        continue;
                    foreach (DataRow row in table.Rows)
                    {
                        if (!all && dgv.SelectedCells.Count > 0 && table.Rows.IndexOf(row) != dgv.SelectedCells[0].RowIndex) continue;
                        lstFormula.Clear();
                        
                        code = idx = "";
                        if (table.Columns.Contains(strColumnIndex)) idx = row[strColumnIndex].ToString();
                        if (table.Columns.Contains(strColumnCode)) code = row[strColumnCode].ToString();
                        //lstFormula.Add(row[strColumnFormula].ToString());

                        foreach (DataColumn col in table.Columns)
                            if (col.ColumnName.StartsWith(strParent))
                            {
                                var tmp = row[col].ToString();
                                if (tmp.StartsWith(strFormulaPrefix)) tmp = tmp.Substring(strFormulaPrefix.Length);
                                lstFormula.Add(tmp);
                            }

                        foreach (var formula in lstFormula)
                            lst.AddRange(CheckCalc(formula, idx, code));
                    }
                }
                foreach (var message in lst)
                    lbxReport.Items.Add(message);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private FormulaError[] CheckCalc(string formula, string idx, string code)
        {
            FormulaUnitProvider prov = new FormulaUnitProvider(strucProvider);
            var res = new List<FormulaError>();
            string f;
            var reg = new Regex(@"\$(.+?)\$");
            var reg_idx = new Regex(@"(\d+(\.\d+)+)");
            Match m = reg.Match(formula);
            Match m_idx;

            f = formula;
            while (m != null && m.Success)
            {
                m_idx = reg_idx.Match(m.Groups[1].Value);
                if(!m_idx.Success)
                    f = f.Replace(m.Value, "$xc$");
                m = m.NextMatch();
            }
            prov.Formula = f;
            var mes = prov.CheckFormula();
            foreach (var item in mes)
            {
                var err = from elem in lstErrorStopList
                          where (!string.IsNullOrEmpty(elem.Text) && elem.Text == item.Text)
                          || (!string.IsNullOrEmpty(elem.Index) && elem.Index == idx)
                          select elem;
                if (!err.Any())
                    res.Add(new FormulaError(idx, code, item.Text));
            }

            return res.ToArray();
        }

        private void btnClearErrorStopList_Click(object sender, EventArgs e)
        {
            lstErrorStopList.Clear();
        }

        private void ignoreErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (FormulaError item in lbxReport.SelectedItems)
            {
                var err = from elem in lstErrorStopList
                          where (!string.IsNullOrEmpty(elem.Text) && elem.Text == item.Text)
                          select elem;
                if (!err.Any())
                    //if (!lstErrorStopList.Contains(item.Text))
                    lstErrorStopList.Add(new FormulaError("", "", item.Text));
            }
        }

        private void ignoreIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (FormulaError item in lbxReport.SelectedItems)
            {
                var err = from elem in lstErrorStopList
                                          where (!string.IsNullOrEmpty(elem.Index) && elem.Index == item.Index)
                                          select elem;
                if (!err.Any())
                    lstErrorStopList.Add(new FormulaError(item.Index, "", ""));
            }
        }

        private void SelectRow(DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            cbxTables.SelectedIndex = ds.Tables.IndexOf(row.Table);
            dgv.ClearSelection();
            dgv.Rows[row.Table.Rows.IndexOf(row)].Selected = true;
        }

        private void lbxReport_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var mes = lbxReport.SelectedItem as FormulaError;
                if (mes == null) return;

                var r = FindRow(strColumnIndex, mes.Index);
                if (r != null)
                    SelectRow(r);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCommaPointReplace_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.CommaPointReplace);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReplaceMacro_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.MacroReplace);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Formula Replace methods
        private string CommaPointReplace(string formula, out string info)
        {
            var reg = new Regex(@"\d+,\d+");
            Match m;
            string tmp, res = formula;

            info = "";

            m = reg.Match(res);
            while (m != null && m.Success)
            {
                tmp = m.Value.Replace(',', '.');
                res = res.Remove(m.Index, m.Length).Insert(m.Index, tmp);
                m = m.NextMatch();
            }

            return res;
        }

        private string DashReplace(string formula, out string info)
        {
            info = "";
            return formula.Replace('–', '-');
        }

        private string MacroReplace(string formula, out string info)
        {
            string res = formula;
            string[] parents;
            string p1, p2;
            string tmp;
            string g1, g2;
            var dicR1 = new Dictionary<string, DataRow>();
            var dicR2 = new Dictionary<string, DataRow>();
            var sb = new StringBuilder();
            var sbw = new StringBuilder();
            var sbv = new StringBuilder();
            int cntv = 1;
            int correction = 0;

            info = "";

            try
            {
                var regp = new Regex(@"\$(.+?)\$");
                Match mp;
                var reg = new Regex(@"ВЕС(.+?),(.+?)\[(.+?)\]");
                var m = reg.Match(formula);
                while (m != null && m.Success)
                {
                    sbv.Clear();
                    sbw.Clear();
                    sb.Clear();
                    dicR1.Clear();
                    dicR2.Clear();
                    cntv = 1;

                    g1 = m.Groups[1].Value.Trim();
                    g2 = m.Groups[2].Value.Trim();
                    mp = regp.Match(g1);
                    while (mp != null && mp.Success)
                    {
                        dicR1[mp.Groups[1].Value] = FindRow(strColumnIndex, mp.Groups[1].Value);
                        mp = mp.NextMatch();
                    }
                    mp = regp.Match(g2);
                    while (mp != null && mp.Success)
                    {
                        dicR2[mp.Groups[1].Value] = FindRow(strColumnIndex, mp.Groups[1].Value);
                        mp = mp.NextMatch();
                    }

                    parents = m.Groups[3].Value.Split(',');
                    foreach (var parent in parents)
                    {
                        p1 = g1;
                        p2 = g2;
                        foreach (var par in dicR1.Keys)
                        {
                            tmp = FormatCode(dicR1[par], parent.Trim(), false);
                            p1 = p1.Replace(par, tmp);
                        }
                        foreach (var par in dicR2.Keys)
                        {
                            tmp = FormatCode(dicR2[par], parent.Trim(), false);
                            p2 = p2.Replace(par, tmp);
                        }
                        sbv.AppendLine(string.Format("var p{0}={1};\r\nvar w{0}={2};", cntv, p1, p2));
                        if (sb.Length > 0) sb.Append("+");
                        sb.Append(string.Format("p{0}*w{0}", cntv));
                        if (sbw.Length > 0) sbw.Append("+");
                        sbw.Append(string.Format("w{0}", cntv));
                        cntv++;
                    }

                    tmp = string.Format("({0})/({1})", sb.ToString(), sbw.ToString());
                    res = res.Remove(m.Index + correction, m.Length);
                    res = res.Insert(m.Index + correction, tmp);
                    correction += tmp.Length - m.Length;
                    m = m.NextMatch();
                }

                reg = new Regex(@"СУММА(.+?)\[(.+?)\]");
                m = reg.Match(res);
                correction = 0;

                //sbv.Clear();
                //cntv = 1;
                while (m != null && m.Success)
                {
                    sb.Clear();
                    dicR1.Clear();

                    g1 = m.Groups[1].Value.Trim();
                    mp = regp.Match(g1);
                    while (mp != null && mp.Success)
                    {
                        dicR1[mp.Groups[1].Value] = FindRow(strColumnIndex, mp.Groups[1].Value);
                        mp = mp.NextMatch();
                    }

                    parents = m.Groups[2].Value.Split(',');
                    foreach (var parent in parents)
                    {
                        p1 = g1;
                        foreach (var par in dicR1.Keys)
                        {
                            tmp = FormatCode(dicR1[par], parent.Trim(), false);
                            p1 = p1.Replace(par, tmp);
                        }
                        sbv.AppendLine(string.Format("var p{0}={1};", cntv, p1));
                        if (sb.Length > 0) sb.Append("+");
                        sb.Append(string.Format("p{0}", cntv));
                        cntv++;
                    }

                    tmp = string.Format("{0}", sb.ToString());
                    res = res.Remove(m.Index + correction, m.Length);
                    res = res.Insert(m.Index + correction, tmp);
                    correction += tmp.Length - m.Length;
                    m = m.NextMatch();
                }
                if (sbv.Length > 0)
                    res = string.Format("{0}\r\n{1}", sbv, res);
                return res;
                //return formula;
            }
            catch (Exception ex)
            {
                info = ex.Message;
                return formula;
            }
        }

        private string AddEOL(string formula, out string info)
        {
            info = "";
            for (int i = formula.Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(formula[i]))
                {
                    if (formula[i] == ';')
                        return formula;
                    else
                        return formula + ";";
                }
            }
            return formula;
        }

        private string ReplaceBrackets(string formula, out string info)
        {
            var lst = new List<Tuple<int, int>>();
            var regp = new Regex(@"\[\s*(.+?)\s*\]:\s*(.*)");
            Match mp;

            mp = regp.Match(formula);
            while (mp != null && mp.Success)
            {
                lst.Add(new Tuple<int, int>(mp.Index, mp.Length));
                mp = mp.NextMatch();
            }

            info = "";
            var sb = new StringBuilder(formula);
            Tuple<int, int> ptr;
            int idx = -1;
            if (lst.Count > 0) idx = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                ptr = idx == -1 ? null : lst[idx];
                if (ptr != null)
                {
                    if (i == ptr.Item1)
                    {
                        i += ptr.Item2 - 1;
                        if (idx + 1 < lst.Count)
                            idx++;
                        else
                            idx = -1;
                        continue;
                    }
                }
                if (sb[i] == '[') sb[i] = '(';
                else
                    if (sb[i] == ']') sb[i] = ')';
            }
            
            return sb.ToString();
        }

        private string ReplaceCaret(string formula, out string info)
        {
            info = "";
            return formula.Replace(@"\^", "^");
        }

        private string ReplaceIf01(string formula, out string info)
        {
            var sb = new StringBuilder();
            string res = formula;
            string tmp;
            var reg = new Regex(@"([I|i][F|f]\s*\([^\(\)]*(\(.*?\))*[^\(\)]*\))(?!\s*return )(.*?;)");
            var m = reg.Match(formula);
            while (m != null && m.Success)
            {
                tmp = string.Format("{0}return {1}", m.Groups[1].Value, m.Groups[3].Value);
                res = res.Replace(m.Value, tmp);
                sb.AppendLine(tmp);
                m = m.NextMatch();
            }

            info = sb.ToString();
            return res;
        }

        private void FormulaChange(FormulaChangeType type)
        {
            var dicInfo = new Dictionary<FormulaChangeType, string>();
            string info;
            string tmp;

            foreach (FormulaChangeType item in Enum.GetValues(typeof(FormulaChangeType)))
                dicInfo[item] = "";

            foreach (DataTable table in ds.Tables)
            {
                if (!chbxIndexAllTables.Checked || !chbxAllRows.Checked)
                    if (dgv.DataSource != table) continue;
                if (!table.Columns.Contains(strColumnFormula)) continue;
                foreach (DataRow row in table.Rows)
                {
                    if (!chbxAllRows.Checked)
                        if (table.Rows.IndexOf(row) != dgv.SelectedCells[0].OwningRow.Index)
                            continue;
                    tmp = row[strColumnFormula].ToString();
                    if ((type & FormulaChangeType.CommaPointReplace) == FormulaChangeType.CommaPointReplace)
                    {
                        tmp = CommaPointReplace(tmp, out info);
                        dicInfo[FormulaChangeType.CommaPointReplace] = info;
                    }
                    if ((type & FormulaChangeType.DashReplace) == FormulaChangeType.DashReplace)
                    {
                        tmp = DashReplace(tmp, out info);
                        dicInfo[FormulaChangeType.DashReplace] = info;
                    }
                    if ((type & FormulaChangeType.MacroReplace) == FormulaChangeType.MacroReplace)
                    {
                        tmp = MacroReplace(tmp, out info);
                        dicInfo[FormulaChangeType.MacroReplace] = info;
                    }
                    if ((type & FormulaChangeType.AddEOL) == FormulaChangeType.AddEOL)
                    {
                        tmp = AddEOL(tmp, out info);
                        dicInfo[FormulaChangeType.AddEOL] = info;
                    }
                    if ((type & FormulaChangeType.ReplaceBrackets) == FormulaChangeType.ReplaceBrackets)
                    {
                        tmp = ReplaceBrackets(tmp, out info);
                        dicInfo[FormulaChangeType.ReplaceBrackets] = info;
                    }
                    if ((type & FormulaChangeType.ReplaceIf01) == FormulaChangeType.ReplaceIf01)
                    {
                        tmp = ReplaceIf01(tmp, out info);
                        dicInfo[FormulaChangeType.ReplaceIf01] += info;
                    }
                    if ((type & FormulaChangeType.ReplaceCaret) == FormulaChangeType.ReplaceCaret)
                    {
                        tmp = ReplaceCaret(tmp, out info);
                        dicInfo[FormulaChangeType.ReplaceCaret] += info;
                    }

                    if (chbxIndexChangeFormula.Checked) row[strColumnFormula] = tmp;
                }
            }
        }
        #endregion

        private void btnAddEOL_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.AddEOL);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReplaceBrackets_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.ReplaceBrackets);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReplaceIf01_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.ReplaceIf01);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReplaceCaret_Click(object sender, EventArgs e)
        {
            try
            {
                FormulaChange(FormulaChangeType.ReplaceCaret);
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            try
            {
                var table = dgv.DataSource as DataTable;
                
                if (table == null) return;
                table.Columns.Add();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAllInOne_Click(object sender, EventArgs e)
        {
            try
            {
                ReplaceIndex();
                FormulaChange(FormulaChangeType.CommaPointReplace
                    | FormulaChangeType.DashReplace
                    | FormulaChangeType.MacroReplace
                    | FormulaChangeType.AddEOL
                    | FormulaChangeType.ReplaceBrackets
                    | FormulaChangeType.ReplaceIf01
                    | FormulaChangeType.ReplaceCaret);
                ReplaceFormula();
                MessageBox.Show("Finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                foreach (FormulaError err in lbxReport.Items)
                    sb.AppendLine(err.ToString());
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    [Flags]
    enum FormulaChangeType
    {
        MacroReplace = 1,
        CommaPointReplace = 2,
        AddEOL = 4,
        ReplaceBrackets = 8,
        ReplaceIf01 = 16,
        ReplaceCaret = 32,
        DashReplace = 64
    }

    class FormulaError
    {
        public string Code { get; private set; }
        public string Index { get; private set; }
        public string Text { get; private set; }

        public FormulaError()
        {
            //
        }
        public FormulaError(string index, string code, string text)
        {
            Index = index;
            Code = code;
            Text = text;
        }

        public override string ToString()
        {
            return string.Format("[{0}],${1}$: {2}", Index, Code, Text);
        }
    }

    class NumberingInfo
    {
        public int Id { get; private set; }
        
        Dictionary<int, LevelInfo> dicLevels = new Dictionary<int, LevelInfo>();
        
        public NumberingInfo(int id)
        {
            Id = id;
        }

        public LevelInfo AddLevel(int levelId, int startValue, string format)
        {
            LevelInfo li = new LevelInfo(levelId, startValue, format);
            dicLevels[levelId] = li;
            return li;
        }
        public LevelInfo AddLevel(int levelId, string format)
        {
            LevelInfo li = new LevelInfo(levelId, format);
            dicLevels[levelId] = li;
            return li;
        }
        public LevelInfo AddLevel(LevelInfo level)
        {
            if (level == null) throw new ArgumentNullException("level");
            dicLevels[level.Id] = level;
            return level;
        }

        public string GetNumberValue(int levelId)
        {
            LevelInfo li;
            StringBuilder buf = new StringBuilder();
            StringBuilder res = new StringBuilder();
            bool inP = false;

            if (!dicLevels.ContainsKey(levelId)) throw new Exception("Numbering level not found: " + levelId.ToString());
            li = dicLevels[levelId];
            if (li.Format == null) return "";
            
            foreach (var c in li.Format)
            {
                if (c == '%')
                {
                    if (inP) res.Append(c);
                    inP = true;
                    buf = new StringBuilder();
                }
                else
                {
                    if (inP)
                    {
                        if (char.IsDigit(c)) buf.Append(c);
                        else
                        {
                            res.Append(FormatBuffer(buf.ToString()));
                            inP = false;
                            buf = new StringBuilder();
                        }
                    }
                    else res.Append(c);
                }
            }

            if (buf.Length > 0) res.Append(FormatBuffer(buf.ToString()));
            return res.ToString();
        }

        private string FormatBuffer(string buf)
        {
            string res = "";
            int levId;
            bool ok = false;
            if (int.TryParse(buf, out levId))
            {
                levId--;
                if (dicLevels.ContainsKey(levId))
                {
                    res = dicLevels[levId].CurrentValue.ToString();
                    ok = true;
                }
            }
            if (!ok) res = "%" + buf;
            return res;
        }

        public int GetValueAndIncrease(int levelId)
        {
            if (dicLevels.ContainsKey(levelId))
                return dicLevels[levelId].GetValueAndIncrease();
            throw new Exception("Numbering level not found: " + levelId.ToString());
        }
        public void IncreaseValue(int levelId)
        {
            GetValueAndIncrease(levelId);
        }
    }

    class LevelInfo
    {
        public int Id { get; private set; }
        public int StartValue { get; private set; }
        public string Format { get; private set; }
        public int CurrentValue { get; private set; }

        public LevelInfo(int id, string format)
        {
            Id = id;
            StartValue = CurrentValue = 0;
            Format = format;
        }
        public LevelInfo(int id, int startValue, string format)
            : this(id, format)
        {
            StartValue = startValue;
            CurrentValue = StartValue;
        }

        public int GetValueAndIncrease()
        {
            return CurrentValue++;
        }
    }
}
