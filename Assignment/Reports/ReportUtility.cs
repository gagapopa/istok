using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Extension;
using DocumentFormat.OpenXml.Packaging;
using NLog;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Работа с FastReport-отчётами.
    /// </summary>
    class ReportUtility
    {
        IUnitManager umanager;
        ValueReceiver vreceiver;

        Logger log = LogManager.GetCurrentClassLogger();

        protected MyDBdata dbwork = null;

        List<IReportSource> reportSources;

        public ReportUtility(MyDBdata dbwork,
                             IUnitManager umanager,
                             IUnitTypeManager typeManager,
                             ExtensionManager extensionManager,
                             ValueReceiver vreciever,
                             ISecurityManager smanager,
                             IUserManager userManager)
        {
            this.dbwork = dbwork.Clone();

            this.umanager = umanager;
            this.vreceiver = vreciever;
            umanager.UnitNodeLoaded += new EventHandler<UnitNodeEventArgs>(umanager_UnitNodeLoaded);
            dependeceDictionary = new Dictionary<Guid, Guid[]>();
            InitiateReportSources(umanager, typeManager, extensionManager, vreciever, smanager, userManager);
        }

        void umanager_UnitNodeLoaded(object sender, UnitNodeEventArgs e)
        {
            ReportNode reportNode = e.UnitNode as ReportNode;

            if (reportNode != null)
                OnLoadReportNode(reportNode);
        }

        /// <summary>
        /// События на загрузку в структуре узла отчёта.
        /// Заполняет в узле информацию о ReportSourceSettings на основании его свойств
        /// </summary>
        /// <param name="reportNode"></param>
        private void OnLoadReportNode(ReportNode reportNode)
        {
            foreach (var guid in reportNode.GetReportSourcesGuids())
            {
                try
                {
                    byte[] settingBinary = reportNode.GetReportSettingBinary(guid);
                    ReportSourceSettings setting = GetReportSourceSettings(guid);
                    if (setting != null)
                    {
                        setting.FromBytes(settingBinary);
                        reportNode.SetReportSetting(guid, setting);
                    }
                }
                catch { }
            }
        }

        Dictionary<Guid, Guid[]> dependeceDictionary;

        /// <summary>
        /// Заполнить список доступных источников данных
        /// </summary>
        private void InitiateReportSources(IUnitManager umanager,
                                           IUnitTypeManager typeManager,
                                           ExtensionManager extensionManager,
                                           ValueReceiver vreciever,
                                           ISecurityManager smanager,
                                           IUserManager userManager)
        {
            reportSources = new List<IReportSource>();

            reportSources.Add(new TimeReportSource());
            reportSources.Add(new ParameterReportSource(umanager, vreciever));
            reportSources.Add(new StructureReportSource(umanager, typeManager, smanager, vreciever, extensionManager));
#if DEBUG
            // TODO локальные сообщения не передаются
            reportSources.Add(new MessageReportSource(extensionManager)); 
#endif
            reportSources.Add(new UserReportSource(umanager, userManager));

            // данные из расширений
            //reportSources.Add(new ExtensionDataReportSource(umanager, extensionManager));
            // может когда ни будь будет сделанно именно так
            //reportSources.AddRange(extensionManager.GetReportSources(umanager, smanager, vreciever));

            // взаимные зависимости источников данных
            foreach (var item in reportSources)
            {
                try
                {
                    Guid[] guids = item.References;

                    dependeceDictionary[item.Info.ReportSourceId] = guids;

                    if (guids != null)
                    {
                        List<Guid> ids = new List<Guid>(guids);
                        var refSources = reportSources.FindAll(s => ids.Contains(s.Info.ReportSourceId));
                        foreach (var refItem in refSources)
                        {
                            item.SetReference(refItem);
                        }

                    }
                }
                catch (NotImplementedException) { }
            }
        }

        /// <summary>
        /// Получить список доступных источников данных
        /// </summary>
        /// <returns></returns>
        public ReportSourceInfo[] GetSupportedSources()
        {
            return reportSources.ConvertAll(s => s.Info).ToArray();
        }

        /// <summary>
        /// Получить ReportSourceSettings для запрашиваемого источника данных
        /// </summary>
        /// <param name="reportSourceGuid">ИД источника данных</param>
        /// <returns></returns>
        public ReportSourceSettings GetReportSourceSettings(Guid reportSourceGuid)
        {
            IReportSource source = reportSources.Find(s => s.Info.ReportSourceId.Equals(reportSourceGuid));

            if (source != null)
                return source.CreateSettingsObject();

            return null;
        }

        /// <summary>
        /// Подготовить данные для указанных источников данных
        /// </summary>
        /// <param name="settings">Настройки запрашиваемых источников</param>
        /// <param name="reportParameters">Параметры отчёта</param>
        /// <returns></returns>
        public FastReportWrap GenerateReportData(OperationState state, ReportSourceSettings[] settings, ReportParameter[] reportParameters)
        {
            FastReportWrap report = new FastReportWrap();
            //report.ReportParameters = reportParameters;
            List<ReportParameter> reportParameterList = new List<ReportParameter>();
            if (reportParameters != null)
                reportParameterList.AddRange(reportParameters);
            reportParameterList.Add(new ReportParameter("user_name", "Имя пользователя", "", "", typeof(String).FullName, "Имя пользователя"));
            report.ReportParameters = reportParameterList.ToArray();

            var sourceSettings = new List<ReportSourceSettings>(settings);

            GenerateData(state, report, sourceSettings, reportParameters);

            return report;
        }

        public FastReportWrap GenerateEmptyReportData(OperationState state, ReportSourceSettings[] settings)
        {
            FastReportWrap report = new FastReportWrap();

            List<ReportParameter> reportParameterList = new List<ReportParameter>();

            foreach (var item in settings)
            {
                var reportParameters = item.GetReportParameters();
                if (reportParameters != null)
                    reportParameterList.AddRange(reportParameters);
            }

            reportParameterList.Add(new ReportParameter("user_name", "Имя пользователя", "", "", typeof(String).FullName, "Имя пользователя"));
            report.ReportParameters = reportParameterList.ToArray();

            var sourceSettings = new List<ReportSourceSettings>(settings);

            GenerateEmptyData(state, report, sourceSettings);

            return report;
        }

        private void GenerateEmptyData(OperationState state, FastReportWrap report, List<ReportSourceSettings> sourceSettings)
        {
            IReportSource source;

            // работа с зависимостями источников данных
            foreach (var item in sourceSettings)
            {
                if (item.Enabled)
                {
                    Guid[] guids;
                    if (dependeceDictionary.TryGetValue(item.Info.ReportSourceId, out guids) && guids != null)
                    {
                        foreach (var reportSourceID in guids)
                        {
                            ReportSourceSettings referenceSettings = sourceSettings.Find(s => s.Info.ReportSourceId == reportSourceID);
                            if (referenceSettings != null)
                                item.SetReference(reportSourceID, referenceSettings);
                        }
                    }
                }
            }

            // сортировка источников данных по зависимостям
            sourceSettings.Sort((x, y) => CompareMethod(x.Info.ReportSourceId, y.Info.ReportSourceId));
            // конец сортировки

            DataSet dataSet = new DataSet();
            // передаем отчёту данные
            foreach (var item in sourceSettings)
            {
                if (item.Enabled)
                {
                    source = reportSources.Find(s => item.Info.ReportSourceId.Equals(s.Info.ReportSourceId));
                    //DataSet dataSet = source.GenerateData(item, reportParameters);
                    source.GenerateEmptyData(state, dataSet, item);
                    //if (dataSet != null)
                    //{
                    //    dataSet.DataSetName = item.Info.Caption;

                    //    report.RegisterData(item.Info.ReportSourceId, dataSet);
                    //}
                }
            }
            report.RegisterData(Guid.Empty, dataSet);
        }

        /// <summary>
        /// Сформировать отчёт
        /// </summary>
        /// <param name="user">Формирующий отчёт пользователь</param>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="reportNode">Узел отчёта</param>
        /// <param name="saveInSystem">Флаг, сохранять ли готовый отчёт в системе</param>
        /// <param name="reportParameters">Параметры отчёта</param>
        /// <returns></returns>
        public byte[] GenerateReport(UserNode user, OperationState state, ReportNode reportNode, bool saveInSystem, ReportParameter[] reportParameters)
        {
            state.StateString = "Подготовка данных отчёта";

            FastReportWrap report = new FastReportWrap();
            report.ReportName = reportNode.FileName;
            report.ReportBody = reportNode.ReportBody;

            List<ReportParameter> reportParameterList = new List<ReportParameter>();
            if (reportParameters != null)
                reportParameterList.AddRange(reportParameters);
            String userName = user.UserFullName ?? user.Text;
            if (String.IsNullOrEmpty(user.UserFullName))
                userName = user.Text;
            else
                userName = user.UserFullName;
            reportParameterList.Add(new ReportParameter("user_name", "Имя пользователя", "", "", typeof(String).FullName, userName));

            report.ReportParameters = reportParameterList.ToArray();

            List<ReportSourceSettings> sourceSettings = new List<ReportSourceSettings>();
            foreach (var sourceGuid in reportNode.GetReportSourcesGuids())
            {
                ReportSourceSettings settings = reportNode.GetReportSetting(sourceGuid);
                if (settings != null && settings.Enabled)
                    sourceSettings.Add(settings);
            }

            state.StateString = "Формирование отчёта";

            GenerateData(state, report, sourceSettings, reportParameters);

            byte[] retReport;
            using (MemoryStream resStream = new MemoryStream())
            {
                FastReport.Report frx = report.UpdateReport(null);
                frx.Prepare();
                frx.SavePrepared(resStream);

                retReport = resStream == null ? null : resStream.ToArray();
            }
            if (saveInSystem)
                AddReportToSystem(user, reportNode, retReport, DateTime.Now, reportParameters);

            state.StateString = "Формирование отчёта завершено";

            return retReport;
        }

        private void GenerateData(OperationState state, FastReportWrap report, List<ReportSourceSettings> sourceSettings, ReportParameter[] reportParameters)
        {
            IReportSource source;

            // работа с зависимостями источников данных
            foreach (var item in sourceSettings)
            {
                if (item.Enabled)
                {
                    Guid[] guids;
                    if (dependeceDictionary.TryGetValue(item.Info.ReportSourceId, out guids) && guids != null)
                    {
                        foreach (var reportSourceID in guids)
                        {
                            ReportSourceSettings referenceSettings = sourceSettings.Find(s => s.Info.ReportSourceId == reportSourceID);
                            if (referenceSettings != null)
                                item.SetReference(reportSourceID, referenceSettings);
                        }
                    }
                }
            }

            // сортировка источников данных по зависимостям
            sourceSettings.Sort((x, y) => CompareMethod(x.Info.ReportSourceId, y.Info.ReportSourceId));
            // конец сортировки

            DataSet dataSet = new DataSet();
            // передаем отчёту данные
            foreach (var item in sourceSettings)
            {
                if (item.Enabled)
                {
                    source = reportSources.Find(s => item.Info.ReportSourceId.Equals(s.Info.ReportSourceId));
                    //DataSet dataSet = source.GenerateData(item, reportParameters);
                    source.GenerateData(state, dataSet, item, reportParameters);
                    //if (dataSet != null)
                    //{
                    //    dataSet.DataSetName = item.Info.Caption;

                    //    report.RegisterData(item.Info.ReportSourceId, dataSet);
                    //}
                }
            }
            report.RegisterData(Guid.Empty, dataSet);
        }

        /// <summary>
        /// Генерация Excel-отчета
        /// </summary>
        /// <param name="user"></param>
        /// <param name="state"></param>
        /// <param name="reportNode"></param>
        /// <param name="saveInSystem"></param>
        /// <returns></returns>
        public byte[] GenerateReport(UserNode user, OperationState state, ExcelReportNode reportNode, DateTime dateFrom, DateTime dateTo, bool saveInSystem)
        {
            state.StateString = "Подготовка данных отчёта";

            state.StateString = "Формирование отчёта";
            if (reportNode.ExcelReportBody != null && reportNode.ExcelReportBody.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(reportNode.ExcelReportBody, 0, reportNode.ExcelReportBody.Length);
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(ms, true))
                    {
                        var lst = new List<DocumentFormat.OpenXml.Spreadsheet.Comment>();
                        foreach (var sheet in doc.WorkbookPart.WorksheetParts)
                        {
                            lst.Clear();
                            foreach (var comment in sheet.WorksheetCommentsPart.Comments.Descendants<DocumentFormat.OpenXml.Spreadsheet.Comment>())
                            {
                                foreach (var text in comment.Descendants<DocumentFormat.OpenXml.Spreadsheet.Text>())
                                {
                                    Regex r = new Regex(@"\[(.*)\]");
                                    Match m = r.Match(text.Text);
                                    if (m.Success)
                                    {
                                        var cells = sheet.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>()
                                            .Where(c => c.CellReference.Value == comment.Reference);
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = null;
                                        if (cells.Count() > 0) cell = cells.First();
                                        if (cell != null)
                                        {
                                            var val = m.Groups[1].Value;
                                            switch (val)
                                            {
                                                case "-1":
                                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dateTo.ToOADate().ToString(
                                                        System.Globalization.CultureInfo.InvariantCulture));
                                                    cell.DataType = new DocumentFormat.OpenXml.EnumValue<DocumentFormat.OpenXml.Spreadsheet.CellValues>(
                                                                    DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);
                                                    break;
                                                default:
                                                    if (umanager != null && vreceiver != null)
                                                    {
                                                        OperationState opstate = new OperationState(state.UserGUID);
                                                        var pnode = umanager.GetParameter(opstate, val);
                                                        if (pnode != null)
                                                        {
                                                            vreceiver.AsyncGetValues(
                                                                opstate,
                                                                0f,
                                                                new ParameterValuesRequest[]
                                                                {
                                                                    new ParameterValuesRequest()
                                                                    {
                                                                        Parameters = new Tuple<ParameterNode,ArgumentsValues>[]
                                                                        {
                                                                            Tuple.Create(pnode, (ArgumentsValues)null)
                                                                        },
                                                                        StartTime = dateFrom, 
                                                                        EndTime = dateTo,
                                                                    }
                                                                },
                                                                true,
                                                                false);

                                                            Package[] packageValues;
                                                            if (opstate.AsyncResult != null)
                                                                packageValues = (from x in opstate.AsyncResult where x is Package select x as Package).ToArray();
                                                            else
                                                                packageValues = new Package[0];
                                                            if (packageValues.Length > 0)
                                                            {
                                                                var p = packageValues.OrderBy(pk => pk.DateTo).Last();
                                                                var vl = p.Values.OrderBy(v => v.Time).Last();
                                                                if (double.IsNaN(vl.Value))
                                                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");
                                                                else
                                                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(vl.Value.ToString(
                                                                        System.Globalization.CultureInfo.InvariantCulture));
                                                                cell.DataType = new DocumentFormat.OpenXml.EnumValue<DocumentFormat.OpenXml.Spreadsheet.CellValues>(
                                                                    DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);
                                                            }
                                                            else
                                                            {
                                                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");
                                                                cell.DataType = new DocumentFormat.OpenXml.EnumValue<DocumentFormat.OpenXml.Spreadsheet.CellValues>(
                                                                    DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);

                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                lst.Add(comment);
                            }
                            foreach (var item in lst)
                                item.Remove();
                            sheet.WorksheetCommentsPart.Comments.Save();
                            sheet.Worksheet.Save();
                            //doc.WorkbookPart.Workbook.Save();
                        }
                        doc.Close();
                    }
                    state.StateString = "Формирование отчёта завершено";

                    return ms.ToArray();
                }
                
            }
            return new byte[0];

            //List<ReportParameter> reportParameterList = new List<ReportParameter>();
            //if (reportParameters != null)
            //    reportParameterList.AddRange(reportParameters);
            //String userName = user.UserFullName ?? user.Text;
            //if (String.IsNullOrEmpty(user.UserFullName))
            //    userName = user.Text;
            //else
            //    userName = user.UserFullName;
            //reportParameterList.Add(new ReportParameter("user_name", "Имя пользователя", "", "", typeof(String), userName));

            //report.ReportParameters = reportParameterList.ToArray();

            //List<ReportSourceSettings> sourceSettings = new List<ReportSourceSettings>();
            //foreach (var sourceGuid in reportNode.GetReportSourcesGuids())
            //{
            //    ReportSourceSettings settings = reportNode.GetReportSetting(sourceGuid);
            //    if (settings != null && settings.Enabled)
            //        sourceSettings.Add(settings);
            //}

            //state.StateString = "Формирование отчёта";

            //byte[] retReport;
            //using (MemoryStream resStream = new MemoryStream())
            //{
            //    FastReport.Report frx = report.UpdateReport(null);
            //    frx.Prepare();
            //    frx.SavePrepared(resStream);

            //    retReport = resStream == null ? null : resStream.ToArray();
            //}
            
            //if (saveInSystem)
            //    AddReportToSystem(user, reportNode, retReport, DateTime.Now, reportParameters);

            //state.StateString = "Формирование отчёта завершено";

            //return retReport;
        }

        /// <summary>
        /// Опасность! Не обрабатывает цикличиские зависимости
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CompareMethod(Guid x, Guid y)
        {
            Guid[] guids;

            if (dependeceDictionary.TryGetValue(x, out guids) && guids != null)
            {
                foreach (var item in guids)
                {
                    if (item.Equals(y) || CompareMethod(item, y) > 0)
                        return 1;
                }
            }
            if (dependeceDictionary.TryGetValue(y, out guids) && guids != null)
            {
                foreach (var item in guids)
                {
                    if (item.Equals(x) || CompareMethod(x, item) < 0)
                        return -1;
                }
            }
            return 0;
        }

        #region Коллекционирование готовых отчетов
        /// <summary>
        /// Сохранить готовый отчёт в системе
        /// </summary>
        /// <param name="user">Пользователь сформировавший отчёт</param>
        /// <param name="reportNode">Узел отчёта</param>
        /// <param name="reportBody">Тело готового отчёта</param>
        /// <param name="date">Дата формирования отчёта</param>
        /// <param name="reportParameters">Параметры отчёта</param>
        private void AddReportToSystem(UserNode user, ReportNode reportNode, byte[] reportBody, DateTime date, ReportParameter[] reportParameters)
        {
            byte[] properties = SerializeReportProperties(reportParameters);

            try
            {
                const String query = "insert into reports (reportId,userId,date,report,props) values (@reportId,@userId,@date,@report,@props)";

                int res;

                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
                DB_Parameters Param = new DB_Parameters();
                Param.Add("reportId", DbType.Int32, reportNode.Idnum);
                Param.Add("userId", DbType.Int32, user.Idnum);
                Param.Add("date", DbType.DateTime, date);
                Param.Add("report", DbType.Binary, reportBody);
                Param.Add("props", DbType.Binary, properties);
                res = dbwork.ExecSQL(query, Param);

                if (res == 0) throw new Exception("Ошибка добавления отчета");
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
            }
        }

        /// <summary>
        /// Сереализовать параметры отчёта в бинарный формат для хранения в БД
        /// </summary>
        /// <param name="reportParameters">Параметры отчёта</param>
        /// <returns></returns>
        private byte[] SerializeReportProperties(ReportParameter[] reportParameters)
        {
            byte[] properties;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    foreach (var item in reportParameters)
                    {
                        Object value = item.GetValue();
                        String valueString = null;

                        if (!String.IsNullOrEmpty(item.StringValue))
                        {
                            valueString = item.StringValue;
                        }
                        else if (item.TypeConverter != null)
                        {
                            Type converterType = Type.GetType(item.TypeConverter);

                            System.ComponentModel.TypeConverter converter = converterType.GetConstructor(new Type[] { }).Invoke(null) as System.ComponentModel.TypeConverter;
                            if (converter != null)
                            {
                                valueString = converter.ConvertToString(value);
                            }
                        }
                        if (String.IsNullOrEmpty(valueString))
                            valueString = value == null ? String.Empty : value.ToString();

                        bw.Write(item.Name);
                        bw.Write(item.DisplayName);
                        bw.Write(item.Category);
                        bw.Write(valueString);
                    }
                }
                properties = ms.ToArray();
            }
            return properties;
        }

        /// <summary>
        /// Десереализовать парамаметры отчёта из бинарного формата
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private ReportParameter[] DeserializeReportProperties(byte[] bytes)
        {
            List<ReportParameter> reportParameters = new List<ReportParameter>(); ;

            try
            {
                System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        while (true)
                        {
                            String parameterName = br.ReadString();
                            String parameterDisplayName = br.ReadString();
                            String parameterCategory = br.ReadString();
                            String parameterValue = br.ReadString();

                            reportParameters.Add(new ReportParameter(parameterName,
                                                parameterDisplayName,
                                                String.Empty,
                                                parameterCategory,
                                                typeof(String).FullName,
                                                parameterValue));
                        }
                    }
                }
            }
            catch (EndOfStreamException) { }

            return reportParameters.ToArray();
        }

        /// <summary>
        /// Получить готовые отчёты за указынный интервал времени
        /// </summary>
        /// <param name="dateFrom">Начальное время запроса</param>
        /// <param name="dateTo">Конечное время запроса</param>
        /// <returns></returns>
        public PreferedReportInfo[] GetReportsFromSystem(DateTime dateFrom, DateTime dateTo)
        {
            List<PreferedReportInfo> lstRes = new List<PreferedReportInfo>();
            PreferedReportInfo ri;

            try
            {
                DB_Parameters par = new DB_Parameters();
                DataTable dt = null;
                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                string query = "select id,reportId,userId,date,props from reports where ";
                query += "(date between @date1 and @date2) order by date";

                //if (dateFrom < Interval.MinDateTime)
                //    dateFrom = Interval.MinDateTime;
                //if (dateTo < Interval.MinDateTime)
                //    dateTo = Interval.MinDateTime;

                par.Add("date1", DbType.DateTime2, dateFrom);
                par.Add("date2", DbType.DateTime2, dateTo);
                dt = dbwork.ExecSQL_toTable(query, par);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ri = null;
                        try
                        {
                            byte[] propsBuffer = (byte[])row["props"];

                            ReportParameter[] reportParameters = DeserializeReportProperties(propsBuffer);

                            ri = new PreferedReportInfo((int)row["id"],
                                                (int)row["userId"],
                                                (int)row["reportId"],
                                                (DateTime)row["date"],
                                                reportParameters);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorException("", ex);
                        }

                        if (ri != null) lstRes.Add(ri);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
            }

            return lstRes.ToArray();
        }

        /// <summary>
        /// Получить тело готового отчёта
        /// </summary>
        /// <param name="report">Информация о готовом отчёте</param>
        /// <returns></returns>
        public byte[] GetReportBody(PreferedReportInfo report)
        {
            byte[] res = null;
            try
            {
                DataTable dt = null;
                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                DB_Parameters Param = new DB_Parameters();
                Param.Add("id", DbType.Int32, report.Idnum, ParameterDirection.Input);
                string query = "select report from reports where id=@id";
                dt = dbwork.ExecSQL_toTable(query, Param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    res = dt.Rows[0]["report"] as byte[];
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
            }

            return res;
        }

        /// <summary>
        /// Удалить готовый отчёт из системы
        /// </summary>
        /// <param name="report">Информация о готовом отчёте</param>
        public void DeleteReport(PreferedReportInfo report)
        {
            try
            {
                int res;
                if (report == null) throw new ArgumentNullException("report");

                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                DB_Parameters Param = new DB_Parameters();
                Param.Add("id", DbType.Int32, report.Idnum, ParameterDirection.Input);
                string query = "delete from reports where id=@id";
                res = dbwork.ExecSQL(query, Param);
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
            }
        }
        #endregion
    }
}
