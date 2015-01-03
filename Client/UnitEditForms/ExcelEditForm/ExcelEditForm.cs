using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using COTES.ISTOK.ASC;
using Microsoft.Office.Interop.Excel;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class ExcelEditForm : ParamEditForm
    {
        ExcelReportNode excelNode = null;

        private Sheets xlSheets = null;
        private Worksheet xlWorksheet = null;
        private Workbook xlWorkbook = null;

        private string TmpXlFilePath = null;
        private string TmpXlCopyFilePath = null;

        public ExcelEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }
        public ExcelEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
        }

        private void Init()
        {
            try
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("item");
                table.Columns.Add("value", typeof(ParamInsertType));
                table.Rows.Add("Одиночное значение", ParamInsertType.Single);
                //table.Rows.Add("Циклическое значение", ParamInsertType.Cycle);
                //table.Rows.Add("Агрегирование диапазона", ParamInsertType.Aggregate);
                table.Rows.Add("Время отчета", ParamInsertType.ReportTime);
                //table.Rows.Add("Диапазон времени", ParamInsertType.TimeCycle);
                //table.Rows.Add("Копирование ячейки", ParamInsertType.AutoFill);
                tscbInsertType.ComboBox.DisplayMember = "item";
                tscbInsertType.ComboBox.ValueMember = "value";
                tscbInsertType.ComboBox.DataSource = table;
                tscbInsertType.ComboBox.SelectedItem = 0;
                var prov = strucProvider.GetUnitProvider(UnitNode) as ExcelReportUnitProvider;
                UnitNode = prov.NewUnitNode;
                excelNode = UnitNode as ExcelReportNode;
                pgNode.SelectedObject = excelNode;
                excelControl.SelectionChanged += new WinExcelControl.Worksheet_SelectionChangedDelegate(excelControl_SelectionChanged);
                TmpXlFilePath = Path.GetTempFileName() + ".xls";
                TmpXlCopyFilePath = Path.GetTempFileName() + ".xls";
            }
            catch (Exception ex) //запасной вариант
            {
                MessageBox.Show(ex.Message, "Ошибка создания временного файла");
                TmpXlFilePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\_$";
            }
        }

        void excelControl_SelectionChanged(object sh, Range target)
        {
            if (target.Columns.Count > 0 && target.Rows.Count > 0)
            {
                string res = target.NoteText(Type.Missing, Type.Missing, Type.Missing);
                char[] separator = new char[] { '[', ']' };
                string[] str = res.Split(separator, 3);
                ParameterNode p = null;
                if (str.Length > 1)
                {
                    //p = (await Task.Factory.StartNew(() => strucProvider.GetUnitNode(str[1]))) as ParameterNode;
                    p = strucProvider.GetUnitNode(str[1]) as ParameterNode;
                    //if (p != null) p.ReadOnly = true;
                }
                if (this.InvokeRequired)
                    this.Invoke((System.Action)(() => pgParameter.SelectedObject = p));
                else
                    pgParameter.SelectedObject = p;
            }
            else
                pgParameter.SelectedObject = null;
        }
        private void InitExcel()
        {
            try
            {
                xlWorkbook = excelControl.workbook;
                xlSheets = excelControl.xlSheets;
                xlWorksheet = excelControl.worksheet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка инициализации Excel");
            }
        }

        private void CreateReport()
        {
            if (excelNode.ExcelReportBody == null || excelNode.ExcelReportBody.Length == 0)
            {
                try
                {
                    excelControl.CreateNew(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка создания нового файла");
                    return;
                }
            }
            else
            {
                try
                {
                    FileStream xlFile = new FileStream(TmpXlFilePath, FileMode.Create);
                    xlFile.Write(excelNode.ExcelReportBody, 0,
                        excelNode.ExcelReportBody.Length);
                    xlFile.Close();
                    excelControl.LoadDocument(TmpXlFilePath, XlWindowState.xlNormal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка открытия файла");
                    return;
                }
            }
            try
            {
                InitExcel();
                excelControl.ActivateControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertParameter(ParamInsertType type)
        {
            try
            {
                SelectForm frm = new SelectForm(strucProvider);
                frm.Filter.Add((int)UnitTypeId.Parameter);
                frm.Filter.Add((int)UnitTypeId.TEP);
                frm.Filter.Add((int)UnitTypeId.ManualParameter);
                frm.MultiSelect = false;
                //if (frm.SelectedObjects != null && frm.SelectedObjects.Length > 0)
                {
                    ParameterNode param;
                    Microsoft.Office.Interop.Excel.Range xlCell = excelControl.worksheet.Application.ActiveCell;
                    string xlParamNote = "";
                    switch (type)
                    {
                        case ParamInsertType.Single:
                            frm.ShowDialog();
                            if (frm.SelectedObjects == null || frm.SelectedObjects.Length == 0) return;
                            param = (ParameterNode)frm.SelectedObjects[0];
                            xlParamNote = "[" + param.Code + "] - " + param.Text;
                            break;
                        case ParamInsertType.Cycle:
                            frm.ShowDialog();
                            if (frm.SelectedObjects == null || frm.SelectedObjects.Length == 0) return;
                            param = (ParameterNode)frm.SelectedObjects[0];
                            xlParamNote = "1" + "[" + param.Code + "] - " + param.Text;
                            break;
                        case ParamInsertType.Aggregate:
                            xlParamNote = "[-3] aggregation";
                            break;
                        case ParamInsertType.ReportTime:
                            xlParamNote = "[-1] time";
                            break;
                        case ParamInsertType.TimeCycle:
                            xlParamNote = "[-2] timecycl";
                            break;
                        case ParamInsertType.AutoFill:
                            xlParamNote = "[-4] AutoFill";
                            break;
                        default:
                            frm.ShowDialog();
                            if (frm.SelectedObjects == null || frm.SelectedObjects.Length == 0) return;
                            param = (ParameterNode)frm.SelectedObjects[0];
                            xlParamNote = "[" + param.Code + "] - " + param.Text;
                            break;
                    }
                    xlCell.NoteText(xlParamNote, Type.Missing, Type.Missing); //заполнить примечание
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void RemoveParameter()
        {
            try
            {
                Microsoft.Office.Interop.Excel.Range xlCell = excelControl.worksheet.Application.ActiveCell;
                xlCell.NoteText("", Type.Missing, Type.Missing); //заполнить примечание
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void ExcelEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!DesignMode)
                {
                    Init();
                    InitExcel();
                    CreateReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        protected override void SaveUnit()
        {
            try
            {
                if (excelNode != null)
                {
                    excelControl.Save(TmpXlFilePath, false, XlFileFormat.xlOpenXMLWorkbook);
                    //if (excelControl.app.Version == "11.0")
                    //    excelControl.Save(TmpXlFilePath, false, XlFileFormat.xlExcel9795); //сохранить документ
                    //else if (excelControl.app.Version == "12.0")
                    //    //excelControl.Save(TmpXlFilePath, false, XlFileFormat.xlExcel8); //сохранить документ
                    //    excelControl.Save(TmpXlFilePath, false, XlFileFormat.xlExcel12); //сохранить документ
                    //else excelControl.Save(TmpXlFilePath, false, XlFileFormat.xlExcel7); //сохранить документ

                    //excelControl.CloseDocument(); //освобождалово
                    File.Copy(TmpXlFilePath, TmpXlCopyFilePath, true);
                    FileStream xlFile = new FileStream(TmpXlCopyFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);

                    byte[] bt = new byte[xlFile.Length];
                    xlFile.Read(bt, 0, (int)xlFile.Length);
                    xlFile.Close();
                    excelNode.ExcelReportBody = bt;
                    //excelControl.LoadDocument(TmpXlFilePath, Excel.XlWindowState.xlNormal);
                    File.Delete(TmpXlCopyFilePath);
                    InitExcel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            base.SaveUnit();
        }

        private void ExcelEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                excelControl.CloseControl();
                File.Delete(TmpXlFilePath);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void addSingleParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.Single);
        }

        private void addCycleParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.Cycle);
        }

        private void addAggregParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.Aggregate);
        }

        private void addReportTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.ReportTime);
        }

        private void addTimeCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.TimeCycle);
        }

        private void addAutoFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertParameter(ParamInsertType.AutoFill);
        }

        private void tsbAddParameter_Click(object sender, EventArgs e)
        {
            InsertParameter((ParamInsertType)tscbInsertType.ComboBox.SelectedValue);
        }

        private void tscbInsertType_SelectedIndexChanged(object sender, EventArgs e)
        {
            excelControl.InsertType = (ParamInsertType)tscbInsertType.ComboBox.SelectedValue;
        }

        private void tsbRemoveParam_Click(object sender, EventArgs e)
        {
            RemoveParameter();
        }

        private void tsbLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog1.FileName))
                {
                    excelControl.LoadDocument(openFileDialog1.FileName, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }

    public enum ParamInsertType
    {
        Single, Cycle, Aggregate, ReportTime, TimeCycle, AutoFill
    }
}
