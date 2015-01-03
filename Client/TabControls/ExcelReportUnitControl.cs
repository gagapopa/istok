using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Runtime.InteropServices;
using COTES.ISTOK.ClientCore.UnitProviders;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    partial class ExcelReportUnitControl : BaseUnitControl
    {
        private ExcelReportNode excelNode = null;
        private ExcelReportUnitProvider excelUnitProvider = null;

        private volatile bool synchronizer = true;
        private volatile object syncData = null;

        byte[] reportBody = null;

        public ExcelReportUnitControl(ExcelReportUnitProvider unitProvider, bool editMode)
            : base(unitProvider)
        {
            InitializeComponent();
            //EditMode = editMode;
        }
        public ExcelReportUnitControl(ExcelReportUnitProvider unitProvider)
            : this(unitProvider, false)
        {
            //
        }

        public override void InitiateProcess()
        {
            if (UnitProvider is ExcelReportUnitProvider)
            {
                excelUnitProvider = (ExcelReportUnitProvider)unitProvider;
                excelNode = (ExcelReportNode)excelUnitProvider.UnitNode;

                if (excelUnitProvider.DataReady)
                {
                    dtpFrom.Value = excelUnitProvider.DatFrom;
                    dtpTo.Value = excelUnitProvider.DatTo;
                }
                else
                {
                    dtpTo.Value = DateTime.Now.Date;
                    //double interv = excelNode.Interval.ToDouble();
                    Interval interv = excelNode.Interval;
                    //if (interv != 0) interv = 86400;
                    if (interv!=Interval.Zero)
                    {
                        interv = Interval.Day;
                    }
                    //dtpFrom.Value = dtpTo.Value.AddSeconds(-interv);
                    dtpFrom.Value = interv.GetPrevTime(dtpTo.Value);
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            excelUnitProvider.DatFrom = dtpFrom.Value;
            excelUnitProvider.DatTo = dtpTo.Value;
            excelUnitProvider.DataReady = true;
            CreateReport();
        }

        private async void CreateReport()
        {
            //AsyncOperationWatcher<Object> watcher = new AsyncOperationWatcher<Object>(
            //    RemoteDataService.Instance.localAsyncOperation.BeginAsyncOperation((AsyncDelegate)AsyncFillExcelReport, excelNode),
            //    RemoteDataService.Instance.localAsyncOperation);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel files (*.xlsx)| *.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                excelUnitProvider.DatFrom = dtpFrom.Value;
                excelUnitProvider.DatTo = dtpTo.Value;
                var body = await Task.Factory.StartNew(() => excelUnitProvider.GenerateReport(true));
                using (var fs = new FileStream(dialog.FileName, FileMode.Create))
                {
                    fs.Write(body, 0, body.Length);
                    fs.Close();
                }
            }

            //watcher.AddStartHandler(Start);
            //watcher.AddValueRecivedHandler(ValueReceive);
            //watcher.AddFinishHandler(Finish);
            //excelUnitProvider.UniForm.RunWatcher(watcher);
        }

        private void ValueReceive(object value)
        {
            reportBody = value as byte[];
        }
        private void Start()
        {
            LockControls();
        }
        private void Finish()
        {
            //try
            //{
            //    if (reportBody == null || reportBody.Length == 0)
            //    {
            //        MessageBox.Show("Ошибка: Отчет не создан.");
            //        return;
            //    }
            //    String name, name_pattern = Path.GetTempPath() + excelNode.Text;
            //    name = name_pattern + "_.xls";
            //    int i = 0;
            //    while (File.Exists(name)) name = String.Format(name_pattern + "_{0}.xls", ++i);

            //    Excel.Application app = null;
            //    try
            //    {
            //        app = new Excel.ApplicationClass();
            //        app.Visible = false;

            //        //создаем файл
            //        FileStream xlFile = new FileStream(name, FileMode.Create);
            //        xlFile.Write(reportBody, 0, reportBody.Length);
            //        xlFile.Close();

            //        if (app != null && app.Workbooks != null)
            //        {
            //            //открываем файл
            //            Excel.Workbook workbook = app.Workbooks.Add(name);
            //            //xlSheets = workbook.Worksheets;
            //            //worksheet = (Excel.Worksheet)xlSheets.get_Item(1);
            //            workbook.Saved = false;
            //            app.DisplayAlerts = false;
            //        }
            //        //удаляем файл
            //        FileInfo fi = new FileInfo(name);
            //        fi.Delete();
            //        app.Visible = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        if (app != null)
            //        {
            //            app.Workbooks.Close();
            //            app.Quit();
            //            while (Marshal.ReleaseComObject(app) > 0) { };
            //            app = null;
            //        }
            //        MessageBox.Show(ex.Message);
            //    }
            //    finally
            //    {
            //        //
            //    }
            //}
            //finally
            //{
            //    UnlockControls();
            //}
        }

        #region Excel Hack
        private COTES.ISTOK.AsyncOperation operation = new COTES.ISTOK.AsyncOperation();
        //private Object AsyncFillExcelReport(OperationState context, params Object[] parameters)
        //{
        //    OperationState state = context;

        //    if (parameters.Length > 0)
        //    {
        //        ExcelReportNode node = (ExcelReportNode)parameters[0];
        //        //ReportParamList pars = (ReportParamList)parameters[1];
        //        return FillExcelReport(state, node);
        //    }
        //    else throw new ArgumentException();
        //}
        //private byte[] FillExcelReport(OperationState state, ExcelReportNode currentReport)//ProgressBar procces)
        //{
        //    int count = 0;

        //    state.Progress = 0;
        //    ///////////////////////////////
        //    //Проверяем введено ли время
        //    ////////////////////////////
        //    //double Interval = currentReport.Interval.ToDouble();
        //    Interval Interval = currentReport.Interval;
        //    String fileName;

        //    DateTime data1 = new DateTime(),
        //             data2 = new DateTime();

        //    data1 = data2 = DateTime.MinValue;
            
        //    ///////////////////////////////////
        //    //загружаем Excel и файл в него
        //    /////////////////////////////////
        //    state.Progress += 2;

        //    Excel.Application app = null;
        //    Excel.Workbook workbook;
        //    Excel.Sheets xlSheets;
        //    Excel.Worksheet worksheet;
        //    byte[] retFile;
        //    try
        //    {
        //        state.StateString = "Загрузка Excel";
        //        fileName = LoadDocument(currentReport, out app, out workbook, out xlSheets, out worksheet);

        //        state.Progress += 3;

        //        data1 = dtpFrom.Value;
        //        data2 = dtpTo.Value;
        //        //проверяем задано ли время
        //        //if (currentParam != null)
        //        //{
        //        //    foreach (ReportParamNode item in currentParam)
        //        //    {
        //        //        if (item.Checked)
        //        //        {
        //        //            //if(cbxAggregation.Text=="   ")
        //        //            //{
        //        //            if (item.Name == "@STARTTIME") data1 = Convert.ToDateTime(item.Value);
        //        //            if (item.Name == "@ENDTIME") data2 = Convert.ToDateTime(item.Value);
        //        //            //}
        //        //            //else
        //        //            if (item.Name == "@TIME")
        //        //            {
        //        //                data1 = Convert.ToDateTime(item.Value);

        //        //                if (Interval < 0) data2 = data1.AddMonths(-(int)Interval);
        //        //                else data2 = data1.AddSeconds(Interval);
        //        //            }
        //        //            if (item.Name == "@MONTH")
        //        //            {
        //        //                data1 = Convert.ToDateTime(item.Value);

        //        //                if (Interval < 0) data2 = data1.AddMonths(-(int)Interval);
        //        //                else data2 = data1.AddSeconds(Interval);
        //        //            }
        //        //        }
        //        //    }
        //        //}

        //        if (data1 == DateTime.MinValue)
        //        {
        //            throw new ArgumentException("Не задано начальное время!");//, "Ошибка");
        //        }

        //        if (data2 == DateTime.MinValue)
        //        {
        //            throw new ArgumentException("Не задано конечное время!");
        //        }

        //        ////////////////////
        //        //Формируем отчет
        //        //////////////////
        //        state.Progress += 5;
        //        state.StateString = "Удаление старых данных";
        //        //перебор всех ячеек в документе , которые заполнены циклически находими и удаляем их
        //        for (int i = 1; i <= xlSheets.Count; i++) //перебор всех листов в документе
        //        {
        //            if (state.IsInterrupted) break;

        //            Excel.Worksheet xlWrkSh1 = (Excel.Worksheet)xlSheets.get_Item(i);
        //            //количество коментариев в листах
        //            count = count + xlWrkSh1.Comments.Count;
        //            foreach (Excel.Comment Comm1 in xlWrkSh1.Comments)
        //            {
        //                Excel.Range rng1 = (Excel.Range)Comm1.Parent;
        //                int[] id = new int[3];
        //                string[] str = (string[])CommentParsingS(rng1.NoteText(Type.Missing, Type.Missing, Type.Missing));

        //                //if (str[0] != "")
        //                //    id[0] = Convert.ToInt32(str[0]);
        //                //else
        //                //    id[0] = 0;

        //                if (id[1] == -100)
        //                {
        //                    rng1.Value2 = "";
        //                    // rng1.ClearComments();
        //                    rng1.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
        //                }
        //            }
        //        }
        //        state.Progress += 5;

        //        state.StateString = "Формирование отчета";
        //        double sum = 0;
        //        int begin = (int)state.Progress;
        //        ////////////////////////////////////
        //        // костыль имени отчётов оптимизации
        //        OptimizationUnitProvider optimizationProvider = ((UniForm)UnitProvider.UniForm).GetUnitProvider(currentReport.OptimizationID) as OptimizationUnitProvider;
        //        if (optimizationProvider != null)
        //        {
        //            //    AsyncOperationWatcher watcher = RemoteDataService.Instance.(parameterId,
        //            //data1, data2, interval, calcAggregation, false);
        //            synchronizer = false;
        //            syncData = null;
        //            ////COTES.ISTOK.Calc.OptimizationBackInfo optimizationResult = null;
        //            ////CalcMessages = null;
        //            //AsyncOperationWatcher watcher = RemoteDataService.Instance.BeginGetOptimizationInfo(optimizationProvider.UnitNode as OptimizationGateNode, data1);
        //            //watcher.AddValueRecivedHandler(x => optimizationResult = x as COTES.ISTOK.Calc.OptimizationBackInfo);
        //            ////watcher.AddMessageReceivedHandler(MessageRetrieved);
        //            //watcher.AddFinishHandler(() => synchronizer = true);
        //            //watcher.Run();

        //            List<ArgumentsValues> optimizationArguments = new List<ArgumentsValues>();
        //            AsyncOperationWatcher<Object> watcher = RemoteDataService.Instance.BeginGetOptimizationArgsForReport(optimizationProvider.UnitNode as OptimizationGateNode, data1);
        //            watcher.AddFinishHandler(() => synchronizer = true);
        //            watcher.AddValueRecivedHandler(x => { if (x != null) optimizationArguments.AddRange(x as ArgumentsValues[]); });
        //            watcher.Run();
        //            WaitSynchronizer();
        //            //paramex = GetSyncData();

        //            System.Text.RegularExpressions.Regex rangeRegex = new System.Text.RegularExpressions.Regex("([^!]+!)?([^:]+):([^:]+)");
        //            System.Text.RegularExpressions.Match rangeMatch = rangeRegex.Match(currentReport.OptimizationExcelRange);
        //            if (rangeMatch.Success)
        //            {
        //                Excel.Worksheet xlOptimSheet;
        //                if (rangeMatch.Groups[1].Success)
        //                {
        //                    String sheetName = rangeMatch.Groups[1].Value.Substring(0, rangeMatch.Groups[1].Value.Length - 1);
        //                    xlOptimSheet = (Excel.Worksheet)xlSheets.get_Item(sheetName);
        //                }
        //                else xlOptimSheet = (Excel.Worksheet)xlSheets.get_Item(1);
        //                String cell1 = rangeMatch.Groups[2].Value;
        //                String cell2 = rangeMatch.Groups[3].Value;
        //                //Excel.Worksheet xlOptimSheet = (Excel.Worksheet)xlSheets.get_Item(sheetName);
        //                // Редактируемый участок
        //                Excel.Range rngOptim;//= xlOptimSheet.get_Range(cell1, cell2);
        //                // Дополнительный участок
        //                Excel.Range tmpRange;//= rngOptim.get_Offset(rngOptim.Rows.Count, 0);

        //                tmpRange = xlOptimSheet.get_Range(cell1, cell2);

        //                Excel.XlInsertShiftDirection insertShift;
        //                Excel.XlDeleteShiftDirection deleteShift;
        //                int xOffset, yOffst;
        //                if (currentReport.OptimizationCopyDirection)
        //                {
        //                    insertShift = Excel.XlInsertShiftDirection.xlShiftToRight;
        //                    deleteShift = Excel.XlDeleteShiftDirection.xlShiftToLeft;
        //                    xOffset = tmpRange.Columns.Count;
        //                    yOffst = 0;
        //                }
        //                else
        //                {
        //                    insertShift = Excel.XlInsertShiftDirection.xlShiftDown;
        //                    deleteShift = Excel.XlDeleteShiftDirection.xlShiftUp;
        //                    xOffset = 0;
        //                    yOffst = tmpRange.Rows.Count;
        //                }

        //                //int maxCount = 20;

        //                foreach (ArgumentsValues item in optimizationArguments)
        //                //for (int i = 0; i < maxCount && i < optimizationArguments.Count; i++)
        //                {
        //                    //ArgumentsValues item = optimizationArguments[i];
        //                    rngOptim = tmpRange;

        //                    tmpRange = rngOptim.get_Offset(yOffst, xOffset);
        //                    tmpRange.Insert(insertShift, null);
        //                    tmpRange = tmpRange.get_Offset(-yOffst, -xOffset);
        //                    rngOptim.Copy(tmpRange);

        //                    GetValueDelegate getValue = (paramId, d1, d2, inter, aggreg) =>
        //                    {
        //                        //optimizationProvider.OptimizationResult.Con
        //                        //if (optimizationProvider.OptimizationResult.ContainsValue(paramId, item))
        //                        //    return optimizationProvider.OptimizationResult.GetValue(paramId, item);
        //                        //if (optimizationResult.ContainsValue(paramId, item))
        //                        //    return optimizationResult.GetValue(paramId, item);
        //                        return GetValue(paramId, item, d1,d2, inter, aggreg);
        //                        //return GetValue(paramId, d1, d2, inter, aggreg);
        //                    };
        //                    foreach (Excel.Range cellRange in rngOptim.Cells)
        //                    {
        //                        if (cellRange.Comment != null)
        //                        {
        //                            String noteText = cellRange.NoteText(Type.Missing, Type.Missing, Type.Missing);
        //                            if (!String.IsNullOrEmpty(noteText)
        //                                && noteText[0] == '{' && noteText[noteText.Length - 1] == '}')
        //                            {
        //                                double value;
        //                                Object objValue = item[noteText.Substring(1, noteText.Length - 2)];
        //                                if (objValue == null)
        //                                    value = double.NaN;
        //                                else value = (double)objValue;
        //                                //double value = optimizationProvider.GetArgumentValue(item, noteText.Substring(1, noteText.Length - 2));
        //                                cellRange.Value2 = value;
        //                                cellRange.ClearComments();
        //                            }
        //                            else
        //                                ProccessCell(state, Interval, data1, data2, xlOptimSheet, cellRange, getValue);
        //                        }
        //                    }
        //                    //rngOptim.ClearComments();
        //                }

        //                tmpRange.Delete(deleteShift);
        //            }

        //        }
        //        // конец костыля имени отчётов оптимизации
        //        //////////////////////////////////////////
        //        //перебор всех ячеек в документе, у которых есть комментарий
        //        for (int i = 1; i <= xlSheets.Count; i++) //перебор всех листов в документе
        //        {
        //            if (state.IsInterrupted) break;

        //            Excel.Worksheet xlWrkSh = (Excel.Worksheet)xlSheets.get_Item(i);

        //            double step = (double)(100 - begin) / (double)count;
                    
        //            foreach (Excel.Comment Comm in xlWrkSh.Comments)
        //            {
        //                if (state.IsInterrupted) break;
        //                Excel.Range rng = (Excel.Range)Comm.Parent;

        //                sum += step;

        //                if (state.Progress + step > 100)
        //                    state.Progress = 100;
        //                else state.Progress = sum + begin;

        //                ProccessCell(state, Interval, data1, data2, xlWrkSh, rng, GetValue);
        //            }
        //        }
        //        //workbook.Application.Cursor = Excel.XlMousePointer.xlDefault;
        //        FileInfo fi = new FileInfo(fileName);
        //        workbook.Close(true, fileName, false);
        //        //app.Quit();
        //        using (FileStream fs = fi.OpenRead())
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                int cnt;
        //                byte[] buffer = new byte[1 << 14];
        //                while ((cnt = fs.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, cnt);
        //                retFile = ms.ToArray();
        //            }
        //        }
        //        //удаляем файл
        //        //FileInfo fi = new FileInfo(name);
        //        fi.Delete();
        //    }
        //    finally
        //    {
        //        if (app != null)
        //        {
        //            app.Workbooks.Close();
        //            app.Quit();
        //            while (Marshal.ReleaseComObject(app) > 0) { };
        //            app = null;
        //        }
        //    }
        //    return retFile;
        //}
        delegate double GetValueDelegate(int paramId, DateTime date1, DateTime date2, Interval interval, CalcAggregation aggregation);

        //private void ProccessCell(OperationState state,
        //                            Interval Interval,
        //                            DateTime data1,
        //                            DateTime data2,
        //                            Excel.Worksheet xlWrkSh,
        //                            Excel.Range rng,
        //                            GetValueDelegate getValueFunc)
        //{
        //    double paramex;
        //    try
        //    {
        //        int j = 0;
        //        int places = -1;
        //        DateTime datatmp = new DateTime();
        //        DateTime datanxt = new DateTime();
        //        datatmp = data1;

        //        int[] id = new int[3];
        //        string[] str = (string[])CommentParsingS(rng.NoteText(Type.Missing, Type.Missing, Type.Missing));

        //        ParameterNode p = RemoteDataService.Instance.GetUnitNode(str[1]) as ParameterNode;

        //        if (p != null)
        //            id[1] = p.Idnum;
        //        else
        //            id[1] = Convert.ToInt32(str[1]);

        //        if (str[0] != "")
        //            id[0] = Convert.ToInt32(str[0]);
        //        else
        //            id[0] = 0;

        //        if (id[1] != 0)
        //        {
        //            if (str[3] == null || !int.TryParse(str[3], out places)) places = -1;
        //            //добавляем время отчета                            
        //            if (id[1] == -1) rng.Value2 = data1.ToString();
        //            //добавляем циклич. время
        //            else if (id[1] == -2)
        //            {
        //                rng.Value2 = datatmp.ToString();
        //                //не больше 61 значечений в строчку
        //                while (j <= 60)
        //                {
        //                    if (state.IsInterrupted) break;
        //                    Excel.Range rv;
        //                    rv = rng;
        //                    //вставляем значения

        //                    //if (Interval < 0) datatmp = datatmp.AddMonths(-(int)Interval);
        //                    //else datatmp = datatmp.AddSeconds(Interval);
        //                    datatmp = Interval.GetNextTime(datatmp);
        //                    if (datatmp >= data2) break;
        //                    rng = rng.get_Offset(1, 0);

        //                    rng.Insert(Excel.XlInsertShiftDirection.xlShiftDown, null);
        //                    rng = rng.get_Offset(-1, 0);
        //                    rv.Copy(rng);
        //                    rng.ClearComments();

        //                    rng.Value2 = datatmp.ToString();
        //                    j++;
        //                }
        //            }
        //            else if (id[1] == -3) //агрегация
        //            {
        //                string buf = rng.FormulaR1C1.ToString();
        //                string buf2;

        //                if (buf.LastIndexOf(":") == -1)
        //                {
        //                    buf2 = buf.Replace(")", ":R[-1]C)");
        //                    rng.FormulaR1C1 = buf2;
        //                }
        //            }
        //            else if (id[1] == -4) //копирование формулы
        //            {
        //                double datacur = data2.Ticks - data1.Ticks;
        //                double y = datacur / (Interval.ToDouble() * 10000000);
        //                Excel.Range range = (Excel.Range)xlWrkSh.get_Range(xlWrkSh.Cells[rng.Row, rng.Column],
        //                                                                   xlWrkSh.Cells[rng.Row + (int)y - 1, rng.Column]);
        //                rng.AutoFill(range, Excel.XlAutoFillType.xlFillDefault);
        //            }
        //            //вставляем текущие данные
        //            else if (id[1] > 0 && id[0] == 0)
        //            {
        //                paramex = getValueFunc(id[1], data1, data2, excelNode.Interval, excelNode.Aggregation);

        //                if (double.IsNaN(paramex)) rng.Value2 = "";
        //                else
        //                {
        //                    if (places >= 0) paramex = Math.Round(paramex, places, MidpointRounding.ToEven);
        //                    rng.Value2 = paramex;
        //                }
        //            }

        //            //циклические параметры
        //            if (id[1] > 0 && id[0] > 0)
        //            {
        //                datatmp = data1;
        //                j = 0;
        //                //не больше 100 значечений в строчку
        //                while (j <= 60)
        //                {
        //                    if (state.IsInterrupted) break;
        //                    //находим диапазон
        //                    datanxt = Interval.GetNextTime(datatmp);
        //                    if (datanxt > data2)
        //                    {
        //                        rng.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

        //                        break;
        //                    }

        //                    paramex = getValueFunc(id[1], datatmp, datanxt, Interval, excelNode.Aggregation);

        //                    if (j != 0) rng.ClearComments();
        //                    Excel.Range rv;
        //                    rv = rng;
        //                    //вставляем значения
        //                    if (double.IsNaN(paramex)) rng.Value2 = String.Empty;
        //                    else
        //                    {
        //                        if (places >= 0) paramex = Math.Round(paramex, places, MidpointRounding.ToEven);
        //                        rng.Value2 = paramex;
        //                    }
        //                    datatmp = datanxt;

        //                    rng = rng.get_Offset(1, 0);
        //                    rng.Insert(Excel.XlInsertShiftDirection.xlShiftDown, null);
        //                    rng = rng.get_Offset(-1, 0);
        //                    rv.Copy(rng);
        //                    j++;
        //                }
        //            }

        //            rng.ClearComments();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        String mess = ex.Message;
        //        if (rng != null) mess = ex.Message + String.Format(" [{0}!R{1}C{2}]", rng.Worksheet.Name, rng.Row, rng.Column);
        //        //MessageBox.Show(mess, "Ошибка в отчете", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        //Exception nexc = new Exception(mess, ex);
        //        //nexc.Data.Add("StackTrace", ex.StackTrace);
        //        //throw nexc; //new Exception(mess, ex);
        //        state.AddMessage(new Message(MessageCategory.Error, mess));
        //    }
        //    //return paramex;
        //}

        private double GetValue(int parameterID, ArgumentsValues arguments, DateTime data1, DateTime data2, Interval interval, CalcAggregation calcAggregation)
        {
            double paramex;
            //AsyncOperationWatcher<Package> watcher = 
            //unitProvider.BeginGetValues(parameterID, arguments, data1, data2, interval, calcAggregation);
            //synchronizer = false;
            //syncData = null;
            //watcher.AddValueRecivedHandler(WatcherReceive);
            //watcher.AddFinishHandler(WatcherFinish);
            //watcher.Run();
            //WaitSynchronizer();
            //paramex = GetSyncData();
            throw new NotImplementedException();
            return paramex;
        }

        private double GetValue(int parameterId, DateTime data1,  DateTime data2, Interval interval, CalcAggregation calcAggregation)
        {
            double paramex;
            //AsyncOperationWatcher<Package> watcher = RemoteDataService.Instance.BeginGetValues(parameterId,
            //    data1, data2, interval, calcAggregation, false);
            //synchronizer = false;
            //syncData = null;
            //watcher.AddValueRecivedHandler(WatcherReceive);
            //watcher.AddFinishHandler(WatcherFinish);
            //watcher.Run();
            //WaitSynchronizer();
            //paramex = GetSyncData();
            throw new NotImplementedException();
            return paramex;
        }

        //private String LoadDocument(ExcelReportNode currentReport, out Excel.Application app,
        //    out Excel.Workbook workbook, out Excel.Sheets xlSheets, out Excel.Worksheet worksheet)
        //{
        //    String name;
        //    workbook = null;
        //    xlSheets = null;
        //    worksheet = null;
        //    //имя создаваемого файла
        //    String name_pattern = Path.GetTempPath() + currentReport.Text;
        //    name = name_pattern + "_.xls";


        //    app = new Excel.ApplicationClass();
        //    //app.
        //    app.Visible = false;
        //    //создаем файл
        //    using (FileStream xlFile = new FileStream(name, FileMode.Create))
        //    {
        //        xlFile.Write(currentReport.ExcelReportBody, 0, currentReport.ExcelReportBody.Length);
        //        xlFile.Close();
        //    }

        //    if (app != null && app.Workbooks != null)
        //    {
        //        //открываем файл
        //        workbook = app.Workbooks.Add(name);
        //        xlSheets = workbook.Worksheets;
        //        worksheet = (Excel.Worksheet)xlSheets.get_Item(1);
        //        workbook.Saved = false;
        //        app.DisplayAlerts = false;
        //    }
        //    //удаляем файл
        //    FileInfo fi = new FileInfo(name);
        //    fi.Delete();
        //    return name;
        //}

        private object CommentParsingS(string str)
        {
            string[] res = new string[4];
            char[] separator = new char[] { '[', ']' };
            string[] result = str.Split(separator, 3);
            // res = null;

            try
            {
                res[2] = result[2];
                res[1] = result[1];
                res[0] = result[0];
                int startindex = result[2].IndexOf('%');
                if (startindex >= 0)
                {
                    int stopindex = result[2].IndexOf(' ');
                    res[3] = result[2].Substring(++startindex, stopindex - +startindex);
                }
                else res[3] = null;
                return res;
            }
            catch { return res; } //не наш комментарий
        }
        #endregion

        #region Костыль для синхронизации асинхронных запросов
        private void WatcherReceive(Package value)
        {
            List<ParamValueItemWithID> lstParams = new List<ParamValueItemWithID>();
            Package[] arrPackages = null;

            //if (value is Package[])
            //    arrPackages = (Package[])value;
            //else
            //    if (value is Package)
            arrPackages = new Package[] { value };
            if (arrPackages == null) return;
            foreach (var item in arrPackages)
                foreach (var valueItem in item.Values)
                    lstParams.Add(new ParamValueItemWithID(valueItem, item.Id));

            if (syncData != null)
            {
                lock (syncData)
                {
                    List<ParamValueItemWithID> lst = new List<ParamValueItemWithID>(syncData as ParamValueItemWithID[]);
                    lst.AddRange(lstParams);
                    syncData = lst.ToArray();
                }
            }
            else
                syncData = lstParams.ToArray();
        }
        private void WatcherFinish()
        {
            synchronizer = true;
        }
        private void WaitSynchronizer()
        {
            while (!synchronizer) Thread.Sleep(10);
        }
        private double GetSyncData()
        {
            ParamValueItemWithID[] arrParams;

            if (syncData == null || (arrParams = syncData as ParamValueItemWithID[]) == null || arrParams.Length == 0)
                return double.NaN;
            return arrParams[0].Value;
        }
        #endregion                
    }
}
