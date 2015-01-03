using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using NLog;

namespace COTES.ISTOK.Assignment
{
    public class ParameterValuesRequest
    {
        public Tuple<ParameterNode, ArgumentsValues>[] Parameters { get; set; }

        //public ArgumentsValues Arguments { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public CalcAggregation Aggregation { get; set; }

        public Interval AggregationInterval { get; set; }

        public ParameterValuesRequest()
        {
            AggregationInterval = Interval.Zero;
            Aggregation = CalcAggregation.Nothing;
        }
    }

    /// <summary>
    /// Сохранить/получить значения
    /// </summary>
    class ValueReceiver : BaseValueReceiver
    {
        private MyDBdata dbwork;

        public ISecurityManager SecurityManager { get; set; }

        public IUnitManager UnitManager { get; set; }

        public Audit.IAuditServer Audit { get; set; }

        public BlockProxy BlockProxy { get; set; }

        /// <summary>
        /// Информация о заблокированных (за давностью лет) значениях
        /// </summary>
        private DateTime lockedTime = DateTime.MinValue;

        private ValueAggregator aggregator;

        Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Запрашивать ли значения с блочных по умолчанию
        /// </summary>
        public const bool UseBlock =
#if EMA
 false;
#else
 true;
#endif

        public ValueReceiver(MyDBdata dbwork)//,
            //ILogger messageLog,
                             //Audit.IAuditServer auditServer,
                             //ISecurityManager securityManager,
                             //IUnitManager unitManager,
                             //BlockProxy blockProxy)
        {
            this.dbwork = dbwork.Clone();
            //this.log = messageLog;
            //this.auditServer = auditServer;
            //this.securityManager = securityManager;
            //this.unitManager = unitManager;
            //this.blockProxy = blockProxy;
            aggregator = new ValueAggregator();
        }

        public ParamValueItem GetLastValueParameter(OperationState state, int ParameterID)
        {
            if (dbwork == null) throw new Exception("ValueParam.GetLastTimeValueParameter: Нет подключения к базе данных");
            try
            {
                DB_Parameters a = new DB_Parameters();
                a.Add("idparam", DbType.Int32, ParameterID);
                using (DataTable t = dbwork.ExecSQL_toTable("select TOP 1 * from value_mparam " +
                                                            "where idparam=@idparam and not(value is NULL) order by time desc", a))
                {
                    if (t.Rows.Count > 0) return CreateParamReceiveItem(t.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                //log.Message(MessageLevel.Error, "ValueParam.GetLastTimeValueParameter: " + ex.Message);
                log.ErrorException("ValueParam.GetLastTimeValueParameter: ", ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// время последнего полученного значения параметра
        /// </summary>
        /// <param name="ParameterID">ИД параметра</param>
        /// <returns></returns>
        public DateTime GetLastTimeValueParameter(OperationState state, int ParameterID)
        {
            if (dbwork == null) throw new Exception("ValueParam.GetLastTimeValueParameter: Нет подключения к базе данных");
            try
            {
                DB_Parameters a = new DB_Parameters();
                a.Add("idparam", DbType.Int32, ParameterID);
                using (DataTable t = dbwork.ExecSQL_toTable("select max(time) from value_mparam " +
                                                            "where idparam=@idparam and  not(value is NULL) ", a))
                {
                    if (t.Rows[0].IsNull(0)) return DateTime.MinValue;
                    return Convert.ToDateTime(t.Rows[0][0]);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("ValueParam.GetLastTimeValueParameter", ex);
                throw;
            }
        }

        public void Start()
        {
            if (maintenanceThread != null) return;
            dbMaintenanceState = true;
            maintenanceThread = new Thread(MaintenanceMethod);
            maintenanceThread.Start();
        }

        public void Stop()
        {
            dbMaintenanceState = false;
            if (maintenanceThread != null)
                maintenanceThread.Abort();
            maintenanceThread = null;
        }

        #region Обслуживание (удаление значения) базы
        volatile bool dbMaintenanceState;
        Thread maintenanceThread;
        private void MaintenanceMethod(Object state)
        {
            TimeSpan bufferFlushInterval = TimeSpan.FromMinutes(6);

            try
            {
                while (dbMaintenanceState)
                {
                    Thread.Sleep(bufferFlushInterval);
                    int transactionID = 0;
                    try
                    {
                        int count = 1000;
                        transactionID = dbwork.StartTransaction();

                        count = DeleteExcessMValues(transactionID, count);
                        count = DeleteExcessCValues(transactionID, count);

                        dbwork.Commit(transactionID);
                    }
                    catch (ThreadInterruptedException) { dbwork.Rollback(transactionID); throw; }
                    catch (ThreadAbortException) { dbwork.Rollback(transactionID); throw; }
                    catch (Exception ex)
                    {
                        dbwork.Rollback(transactionID);
                        log.ErrorException("ValueParam.MaintenanceMethod", ex);
                    }
                    finally
                    {
                        dbwork.CloseTransaction(transactionID);
                    }
                }
            }
            catch (ThreadInterruptedException) { dbMaintenanceState = false; }
            catch (ThreadAbortException) { dbMaintenanceState = false; }
        }

        private int DeleteExcessMValues(int transactionID, int count)
        {
            return DeleteExcessValues(transactionID, "value_mparam", count);
        }

        private int DeleteExcessCValues(int transactionID, int count)
        {
            return DeleteExcessValues(transactionID, "value_aparam", count);
        }

        private int DeleteExcessValues(int transactionID, String tableName, int count)
        {
            const String selectQuery = "SELECT TOP {1} [idnum]  FROM [{0}] " +
                "WHERE [idparam] not in (SELECT [idnum]  FROM [unit])";

            using (DataTable t = dbwork.ExecSQL_toTable(transactionID, String.Format(selectQuery, tableName, count), null))
            {
                if (t.Rows.Count > 0)
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.AppendFormat("DELETE FROM [{0}] WHERE ", tableName);
                    for (int i = 0; i < t.Rows.Count; i++)
                    {
                        int rowID = Convert.ToInt32(t.Rows[i][0]);
                        if (i > 0)
                            queryBuilder.Append(" OR ");
                        queryBuilder.AppendFormat(" idnum = {0}", rowID);
                    }
                    int rowAffected = dbwork.ExecSQL(transactionID, queryBuilder.ToString(), null);
                    count -= rowAffected;
                }
            }
            return count;
        }
        #endregion
        #region AsyncGetValues()

        /// <summary>
        /// Запрос значений из БД
        /// </summary>
        /// <param name="parameter">Запрашиваемый параметр</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <returns>ParamReceiveItem'ы со значениями</returns>
        private List<ParamValueItem> GetAllValues(
            OperationState state,
            UnitNode parameter,
            DateTime beginTime,
            DateTime endTime,
            ref bool multiParse)
        {
            String valueLastQuery, valueQuery;

            if (dbwork == null)
                throw new Exception("ValueParam._getAllValues: Нет подключения к базе данных");

            Interval parameterInterval = GetInterval(state, parameter);
            //DateTime parameterStartTime = GetStartTime(state, parameter);

            if (!multiParse
                && parameterInterval.GetQueryValues(/*parameterStartTime, */beginTime, endTime) > GlobalSettings.Instance.ValuesMaxCount)
                throw new Exception(String.Format("Расчетное количество значений параметра '{0}' за интервал ['{1:yyyy-MM-dd HH:mm:ss}','{2:yyyy-MM-dd HH:mm:ss}') больше настроечного", parameter.FullName, beginTime, endTime));

            List<ParamValueItem> vals = new List<ParamValueItem>();

            if (beginTime < minDateTime)
                beginTime = minDateTime;
            try
            {
                DataTable tempTable;
                DB_Parameters a = new DB_Parameters();
                a.Add("idparam", DbType.Int32, parameter.Idnum);
                a.Add("dat1", DbType.DateTime, beginTime);
                a.Add("dat2", DbType.DateTime, endTime);

                valueLastQuery = "select TOP 1  idparam, time, ch_time, value, value_corr, 100 as quality from value_mparam where idparam=@idparam and time<=@dat1 order by time desc";
                valueQuery = "select TOP " + (GlobalSettings.Instance.ValuesMaxCount + 1) + " v1.idparam,v1.time,v1.ch_time,v1.value,v1.value_corr, 100 as quality from value_mparam v1 " +
                                                        " where v1.idparam=@idparam " +
                                                        " and v1.time > @dat1 and v1.time <= @dat2 " +
                                                        " order by time";
                tempTable = dbwork.ExecSQL_toTable(valueLastQuery, a);
                tempTable.Merge(dbwork.ExecSQL_toTable(valueQuery, a));

                foreach (DataRow row in tempTable.Rows)
                    vals.Add(CreateParamReceiveItem(row));

                valueQuery = "select TOP " + (GlobalSettings.Instance.ValuesMaxCount + 1) + " v1.idparam, v1.arg_set_id,v1.time,v1.ch_time,v1.value,v1.value_corr, 100 as quality from value_cparam v1 " +
                                                        " where v1.idparam=@idparam" +
                                                        " and v1.time >= @dat1 and v1.time <= @dat2 " +
                                                        " order by time";

                if (!multiParse
                    && vals.Count > GlobalSettings.Instance.ValuesMaxCount)
                    throw new Exception(String.Format("Расчетное количество значений параметра '{0}' за интервал ['{1:yyyy-MM-dd HH:mm:ss}','{2:yyyy-MM-dd HH:mm:ss}') больше настроечного", parameter.FullName, beginTime, endTime));

                // требуется ли дополнительный проход
                multiParse = multiParse && vals.Count > GlobalSettings.Instance.ValuesMaxCount;
                return vals;
            }
            catch (Exception ex)
            {
                log.ErrorException("ValueParam._getAllValues", ex);
                throw;
            }
        }

        /// <summary>
        /// Получить интервал узла
        /// </summary>
        /// <param name="node">Узел</param>
        /// <returns></returns>
        private Interval GetInterval(OperationState state, UnitNode node)
        {
            LoadParameterNode loadParameter;
            ParameterGateNode gateNode;
            //UnitNode curNode;
            if ((loadParameter = node as LoadParameterNode) != null)
                return loadParameter.Interval;

            gateNode = UnitManager.CheckParentNode<ParameterGateNode>(state, node.ParentId, Privileges.Read);

            if (gateNode != null)
                //curNode = node;
                //while (curNode != null)
                //{
                //    if ((gateNode = curNode as ParameterGateNode) != null)
                return gateNode.Interval;
            //    curNode = UnitManager.GetUnitNode(state, curNode.ParentId);
            //}
            return Interval.Zero;
        }

        ///// <summary>
        ///// Получить время начала узла
        ///// </summary>
        ///// <param name="node">Узел</param>
        ///// <returns></returns>
        //private DateTime GetStartTime(OperationState state, UnitNode node)
        //{
        //    LoadParameterNode loadParameter;
        //    ParameterGateNode gateNode;
        //    UnitNode curNode;
        //    if ((loadParameter = node as LoadParameterNode) != null)
        //        return loadParameter.StartTime;
        //    curNode = node;
        //    while (curNode != null)
        //    {
        //        if ((gateNode = curNode as ParameterGateNode) != null)
        //            return gateNode.StartTime;
        //        curNode = UnitManager.GetUnitNode(state, curNode.ParentId);
        //    }
        //    return DateTime.MinValue;
        //}

        /// <summary>
        /// Создание ParamReceiveItem'а по строке из БД
        /// </summary>
        /// <param name="row">строка БД</param>
        /// <returns>ParamReceiveItem</returns>
        private ParamValueItem CreateParamReceiveItem(DataRow row)
        {
            ParamValueItem retItem;
            DateTime time, changeTime;
            double val, corrval;
            Quality quality;

            time = Convert.ToDateTime(row["time"]);
            changeTime = Convert.ToDateTime(row["ch_time"]);

            quality = Quality.Good;
            if (DBNull.Value.Equals(row["value"]))
            {
                val = double.NaN;
                quality = Quality.Bad;
            }
            else val = Convert.ToDouble(row["value"]);
            if (DBNull.Value.Equals(row["value_corr"]))
                retItem = new ParamValueItem(time, quality, val);
            else
            {
                corrval = Convert.ToDouble(row["value_corr"]);
                retItem = new CorrectedParamValueItem(new ParamValueItem(time, quality, val), corrval);
            }
            retItem.ChangeTime = changeTime;

            return retItem;
        }

        private static int RetreivePackageSize = 720;

        // запросить значение регулярного параметра
        public void AsyncGetParameterValues(
            OperationState state,
            double progressToLoad,
            ParameterValuesRequest valuesRequest,
            //ParameterNode parameter,
            //ArgumentsValues args,
            //DateTime beginTime,
            //DateTime endTime,
            //Interval interval,
            //CalcAggregation aggregation,
            bool fillBads,
            ref bool multiParse)
        {
            DateTime beginTimeCorrected, endTimeCorrected;

            Interval sourceInterval = null;

            foreach (var parameterNode in valuesRequest.Parameters)
            {
                Interval parameterInterval = GetInterval(state, parameterNode.Item1);

                if (sourceInterval == null)
                {
                    sourceInterval = parameterInterval;
                }
                else if (!sourceInterval.Equals(parameterInterval))
                {
                    log.Warn("Для расчёта агрегации {0} требуются параметры с одинаковой дискретностью", valuesRequest.Aggregation);
                    state.AddMessage(new Message(MessageCategory.Warning, "Для расчёта агрегации {0} требуются параметры с одинаковой дискретностью", valuesRequest.Aggregation));
                    return;
                }
            }

            // корректирум запрашиваемое время исходных значений
            beginTimeCorrected = valuesRequest.StartTime;
            endTimeCorrected = valuesRequest.EndTime;
            aggregator.GetSourceRange(sourceInterval,
                                      valuesRequest.AggregationInterval,
                                      ref beginTimeCorrected,
                                      ref endTimeCorrected);

            //Dictionary<ArgumentsValues, Dictionary<ParameterNode,List<ParamValueItem>>> 
            Dictionary<ArgumentsValues, List<ParamValueItem>>[] sourceValues = new Dictionary<ArgumentsValues, List<ParamValueItem>>[valuesRequest.Parameters.Length];
            Dictionary<ArgumentsValues, List<ParamValueItem>> valueDictionary;
            HashSet<ArgumentsValues> argumentsList = new HashSet<ArgumentsValues>();
            //List<ParamValueItem>[] sourceValues;

            //foreach (var parameterTuple in valuesRequest.Parameters)
            for (int i = 0; i < valuesRequest.Parameters.Length; i++)
            {
                var parameterNode = valuesRequest.Parameters[i].Item1;
                var parameterArgs = valuesRequest.Parameters[i].Item2;

                sourceValues[i] = valueDictionary = new Dictionary<ArgumentsValues, List<ParamValueItem>>();

                // запросить все значения условного параметра за требуемое время
                var argumentedList = LoadArgumentedValues(state, MyDBdata.WithoutTransactionID, parameterNode, beginTimeCorrected, endTimeCorrected);

                // если заданны конкретные аргументы, выбрать для результата нужные
                if (parameterArgs != null)
                {
                    var values = (from v in argumentedList where v.Arguments.Equals(parameterArgs) select v).ToList();
                    values.Sort();

                    //if (!valueDictionary.TryGetValue(parameterArgs, out sourceValues))
                    //{
                    //    valueDictionary[parameterArgs] = sourceValues = new List<ParamValueItem>[valuesRequest.Parameters.Length];
                    //}
                    argumentsList.Add(parameterArgs);
                    valueDictionary[parameterArgs] = values;
                    //sourceValues[i] = values;
                }
                // в противном случае запрашиваем значения без аргументов
                else
                {
                    argumentsList.Add(ArgumentsValues.Empty);
                    valueDictionary[ArgumentsValues.Empty] = GetAllValues(state, parameterNode, beginTimeCorrected, endTimeCorrected, ref multiParse);
                    valueDictionary[ArgumentsValues.Empty].Sort();

                    // группируем значения по аргументам
                    List<ArgumentsValues> argumentList = new List<ArgumentsValues>();
                    List<ParamValueItem> valuesList;

                    foreach (var item in argumentedList)
                    {
                        ArgumentsValues itemArgument = item.Arguments;
                        if (itemArgument == null)
                            itemArgument = new ArgumentsValues();

                        if (!valueDictionary.TryGetValue(itemArgument, out valuesList))
                        {
                            argumentsList.Add(itemArgument);
                            valueDictionary[itemArgument] = valuesList = new List<ParamValueItem>();
                        }

                        valuesList.Add(item);//.ValueItem);
                    }
                }
            }

            // для каждого аргумента
            foreach (ArgumentsValues valueArgs in argumentsList)
            {
                List<ParamValueItem>[] argumentSourceValues;

                //List<ParamValueItem> d = valueDictionary[valueArgs];
                //DateTime parameterStartTime = GetStartTime(state, parameter);
                //Interval parameterInterval = GetInterval(state, parameter);
                //Interval valueInterval;
                //DateTime startTime;

                //// выравниваем значения согластно интервала параметра
                //if (parameterInterval != Interval.Zero)
                //    startTime = parameterInterval.NearestEarlierTime(/*parameterStartTime,*/ beginTime);
                //else startTime = parameterStartTime;

                argumentSourceValues = new List<ParamValueItem>[valuesRequest.Parameters.Length];
                for (int i = 0; i < valuesRequest.Parameters.Length; i++)
                {
                    if (!sourceValues[i].TryGetValue(valueArgs, out argumentSourceValues[i]))
                    {
                        argumentSourceValues[i] = new List<ParamValueItem>();
                    }

                    if (fillBads)
                    {
                        //if (valuesRequest.AggregationInterval == Interval.Zero)
                        //    valueInterval = sourceInterval;
                        //else
                        //    valueInterval = valuesRequest.AggregationInterval;

                        argumentSourceValues[i] = FillSourceBad(beginTimeCorrected, endTimeCorrected, sourceInterval, /*valueInterval,*/ argumentSourceValues[i]);
                    } 
                }

                // агрегируем значения
                List<ParamValueItem> d = aggregator.Aggregate(valuesRequest.Aggregation, 
                                                              sourceInterval, 
                                                              valuesRequest.AggregationInterval, 
                                                              argumentSourceValues).ToList();

                // удалить лишние значение в начале
                if (d.Count > 0 && d[0].Time < valuesRequest.StartTime)
                    d.RemoveAt(0);

                // выставляем значение аргумента после агрегации
                if (valueArgs != ArgumentsValues.Empty)
                    foreach (var item in d)
                    {
                        item.Arguments = valueArgs;
                    }

                // сформировать пачки
                Package[] packages = AddParametersToAsyncResult(valuesRequest.Parameters.First().Item1.Idnum, valuesRequest.EndTime, d, RetreivePackageSize);

                // удалить последнию пачку, если получены не все данные
                if (multiParse)
                {
                    Package[] packs = new Package[packages.Length - 1];
                    Array.Copy(packages, 0, packs, 0, packs.Length);
                    packages = packs;
                }

                // вернуть полученные значения
                foreach (Package pack in packages)
                    state.AddAsyncResult(pack);
            }
        }

        // запросить значение сменяемой константы
        public void AsyncGetVCValues(
            OperationState state,
            ParameterValuesRequest valueRequest)
            //ParameterNode parameter,
            //DateTime beginTime,
            //DateTime endTime,
            //Interval interval,
            //CalcAggregation aggregation)
        {
            bool multiParse = false;
            double curProgress = state.Progress;

            List<List<ParamValueItem>> sourceValues = new List<List<ParamValueItem>>();
            //List<ParamValueItem> firstValues = new List<ParamValueItem>();

            foreach (var parameterTuple in valueRequest.Parameters)
            {
                var parameterNode = parameterTuple.Item1;

                // запросить исходные данные
                List<ParamValueItem> values = GetAllValues(state, parameterNode, valueRequest.StartTime, valueRequest.EndTime, ref multiParse);
                if (valueRequest.Aggregation != CalcAggregation.Nothing)
                {
                    if (values.Count == 0 || values.First().Time > valueRequest.StartTime)
                    {
                        values.Insert(0, new ParamValueItem(valueRequest.StartTime, Quality.Bad, double.NaN));
                    }
                    if (values[values.Count - 1].Time < valueRequest.EndTime)
                    {
                        values.Add(new ParamValueItem(valueRequest.EndTime.AddMilliseconds(1), Quality.Bad, double.NaN));
                    }
                }
                sourceValues.Add(values);
            }

            ParamValueItem first = null;
            if (sourceValues.Count == 1
                && sourceValues[0].Count > 0)
            {
                first = sourceValues[0][0];
            }

            //DateTime parameterStartTime = GetStartTime(state, parameter);
            //DateTime startTime;

            // выравниваем значения согластно интервала параметра
            //if (interval != Interval.Zero)
            //    startTime = interval.NearestEarlierTime(parameterStartTime, beginTime);
            //else startTime = parameterStartTime;

            List<ParamValueItem> d = aggregator.Aggregate(valueRequest.Aggregation,
                                                          Interval.Zero,
                                                          valueRequest.AggregationInterval,
                                                          sourceValues.ToArray()).ToList();

            if (first != null
                && d.Count == 0)
            {
                d.Insert(0, first);
            }
            else
            {
                // remove values before starttime
                DateTime minTime = (from p in d
                                    where p.Time <= valueRequest.StartTime
                                    orderby p.Time descending
                                    select p.Time).FirstOrDefault();
                if (first != null)
                {
                    d.RemoveAll(p => p.Time < minTime);
                }
            }


            Package[] packages = AddParametersToAsyncResult(valueRequest.Parameters.First().Item1.Idnum, valueRequest.EndTime, d, RetreivePackageSize);

            foreach (Package pack in packages)
                state.AddAsyncResult(pack);
        }

        /// <summary>
        /// Запросить значения параметроа
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="progressToLoad"></param>
        /// <param name="parameters">Параметры, для которых запрашивается значение</param>
        /// <param name="args">Аргуметы условных параметров, null для нормальных значений</param>
        /// <param name="beginTime">Начальное запрашиваемое время</param>
        /// <param name="endTime">Конечное запрашиваемое время</param>
        /// <param name="interval">Интервал агрегации</param>
        /// <param name="aggregation">Алгоритм агрегации</param>
        /// <param name="fillBads">Заполнять ли пропуски значениями с плохим качеством</param>
        /// <param name="multiParse">
        /// true что бы при большом количестве значений получить только настроечное количество значений.
        /// Если по завершению метода значение параметра == true, то переданы не все значения.
        /// </param>
        /// <param name="useBlockValues">Передавать запрос на сервер сбора данных</param>
        /// <param name="multiArgs">Запрасить все значения аргументов для условных параметров</param>
        public void AsyncGetValues(
            OperationState state,
            double progressToLoad,
            //ParameterNode[] parameters,
            //ArgumentsValues args,
            //DateTime beginTime,
            //DateTime endTime,
            //Interval interval,
            //CalcAggregation aggregation,
            ParameterValuesRequest[] valuesRequest,
            bool fillBads,
            bool useBlockValues)
        {
            // параметры, значения которых надо перезапрашивать у блочных
            List<ParameterValuesRequest> loadParameters = new List<ParameterValuesRequest>();

            state.AllowStartAsyncResult = true;

            double currProgress = state.Progress;

            double step = progressToLoad / valuesRequest.Length;

            // распределяем параметры по группам
            foreach (ParameterValuesRequest param in valuesRequest)
            {
                var types = (from p in param.Parameters select (UnitTypeId)p.Item1.Typ).Distinct().ToArray();
                ParameterGateNode paramGateNode;
                //if (useBlockValues && param.Typ == (int)UnitTypeId.Parameter)

                // если все параметры собираемые (и запрос с блочного разрешён), направлям запрос на BlockProxy
                if (useBlockValues && types.Length > 1 && types.Contains(UnitTypeId.Parameter))
                {
                    log.Warn("Попытка расчёта многопараметровой аггрегации для параметров из смешанных источников данных");
                }
                else if (useBlockValues && types.Length == 1 && types[0] == UnitTypeId.Parameter)
                {
                    loadParameters.Add(param);
                }
                else
                {
                    bool isReg = false;
                    // Если параметры не содержат регулярных, запросить как сменяемые константы
                    foreach (var paramTuple in param.Parameters)
                    {
                        var paramNode = paramTuple.Item1;
                        if (paramNode.Typ == (int)UnitTypeId.TEP
                            || (paramNode.Typ == (int)UnitTypeId.ManualParameter
                            && (paramGateNode = UnitManager.GetParentNode(state, paramNode, (int)UnitTypeId.ManualGate) as ParameterGateNode) != null
                            && paramGateNode.IsReg))
                        {
                            isReg = true;
                            break;
                        }
                    }

                    // Иначе как простые параметры
                    if (isReg)
                    {
                        bool multiParse = false;

                        AsyncGetParameterValues(
                            state,
                            step,
                            param,
                            //args,
                            //beginTime,
                            //endTime,
                            //interval,
                            //aggregation,
                            fillBads,
                            ref  multiParse);
                    }
                    else //if (!useBlockValues || param.Typ == (int)UnitTypeId.ManualParameter)
                    {
                        AsyncGetVCValues(
                            state,
                            param);
                        //param,
                        //beginTime,
                        //endTime,
                        //interval,
                        //aggregation);
                    }
                }
            }

            // запрос данных с серверов сбора данных
            if (loadParameters.Count > 0)
            {
                BlockProxy.AsyncGetValues(state, step * loadParameters.Count, loadParameters.ToArray());//, beginTime, endTime, interval, aggregation);
            }
        }

        public void AsyncGetValues(
            OperationState state,
            double progressToLoad,
            //ParameterNode[] parameters,
            //ArgumentsValues args,
            //DateTime beginTime,
            //DateTime endTime,
            //Interval interval,
            //CalcAggregation aggregation,
            ParameterValuesRequest[] valuesRequest,
            bool fillBads)
        {
            //AsyncGetValues(state, progressToLoad, parameters, args, beginTime, endTime, interval, aggregation, fillBads, UseBlock);//, multiArgs);
            AsyncGetValues(state, progressToLoad, valuesRequest, fillBads, UseBlock);//, multiArgs);
        }
        #endregion

        #region Работа с аргументами условных параметров

        /// <summary>
        /// Запросить отсортированные аргументы оптимизационного расчета по убыванию оптимальности
        /// </summary>
        /// <param name="optimizationNode">Узел оптимизации</param>
        /// <param name="time">Запрашиваемое время</param>
        /// <returns></returns>
        public ArgumentsValues[] GetSortedArgs(OperationState state, OptimizationGateNode optimizationNode, DateTime time)
        {
            DateTime beginTime, endTime;

            if (optimizationNode == null || String.IsNullOrEmpty(optimizationNode.Expression) || time == DateTime.MinValue)
                return null;

            beginTime = optimizationNode.Interval.NearestEarlierTime(/*optimizationNode.StartTime,*/ time);
            endTime = optimizationNode.Interval.GetNextTime(beginTime);

            // исключаем плохие результаты
            List<ParamValueItem> argumentedList;
            argumentedList = (from v in LoadArgumentedValues(new OperationState(), MyDBdata.WithoutTransactionID, optimizationNode, beginTime, endTime)
                              where v.Quality != Quality.Bad && !double.IsNaN(v.Value)
                              select v).ToList();

            if (optimizationNode.Maximalize)
                argumentedList.Sort((a, b) => -a.Value.CompareTo(b.Value));
            else
                argumentedList.Sort((a, b) => a.Value.CompareTo(b.Value));
            return argumentedList.ConvertAll(a => a.Arguments).ToArray();
        }

        public ArgumentsValues[] GetOptimizationArguments(OperationState state, OptimizationGateNode optimizationNode, DateTime time)
        {
            List<ParameterNode> parameterList = new List<ParameterNode>();

            Queue<UnitNode> unitNodeQueue = new Queue<UnitNode>();
            UnitNode unitNode, tmpNode;
            ParameterNode parameterNode;
            unitNodeQueue.Enqueue(optimizationNode);

            while (unitNodeQueue.Count > 0)
            {
                unitNode = unitNodeQueue.Dequeue();
                if ((parameterNode = unitNode as ParameterNode) != null)
                    parameterList.Add(parameterNode);
                foreach (int nodeID in unitNode.NodesIds)
                {
                    //tmpNode = UnitManager.GetUnitNode(state, nodeID);
                    tmpNode = UnitManager.ValidateUnitNode(state, nodeID, Privileges.Read);
                    if (tmpNode.Typ != (int)UnitTypeId.OptimizeCalc)
                        unitNodeQueue.Enqueue(tmpNode);
                }
            }
            if (parameterList.Count == 0)
                return null;

            return GetManualArgs(state, parameterList, time);
        }

        // уходит в расчет
        public ArgumentsValues[] GetManualArgs(OperationState state, List<ParameterNode> parameterIdList, DateTime time)
        {
            List<ArgumentsValues> argumentsList = new List<ArgumentsValues>();
            IEnumerable<ParamValueItem> values;
            Interval interval;
            DateTime beginTime, endTime;

            foreach (UnitNode unitNode in parameterIdList)
            {
                interval = GetInterval(state, unitNode);
                beginTime = interval.NearestEarlierTime(/*GetStartTime(state, unitNode),*/ time);
                endTime = interval.GetNextTime(beginTime);
                values = LoadArgumentedValues(state, MyDBdata.WithoutTransactionID, unitNode, beginTime, endTime);

                foreach (var item in values)
                    if (!argumentsList.Contains(item.Arguments))
                        argumentsList.Add(item.Arguments);
            }
            return argumentsList.ToArray();
        }
        #endregion

        #region SaveValues()
        private static decimal maxDecimal = Convert.ToDecimal(1e18) - 1;//Convert.ToDouble(Decimal.MaxValue);
        private static decimal minDecimal = -maxDecimal;//Convert.ToDouble(Decimal.MinValue);
        private static double maxDouble = Convert.ToDouble(maxDecimal);//Convert.ToDouble(Decimal.MaxValue);
        private static double minDouble = Convert.ToDouble(minDecimal);//Convert.ToDouble(Decimal.MinValue);
        private static DateTime minDateTime = new DateTime(2000, 01, 01);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="progressToLoad">На сколько изменить текущий прогресс</param>
        /// <param name="packages">Пакеты с сохраняемыми значениями</param>
        public void AsyncSaveValues(OperationState state, Guid userGUID, double progressToLoad, Package[] packages)
        {
            List<Package> lstPackages = new List<Package>();
            ParameterNode node;

            foreach (var item in packages)
            {
                //node = UnitManager.GetUnitNode(state, item.Id) as ParameterNode;
                node = UnitManager.CheckUnitNode<ParameterNode>(state, item.Id, Privileges.Execute);
                if (node != null)
                {
                    if (node.Typ != (int)UnitTypeId.ManualParameter && node.Typ != (int)UnitTypeId.TEP)
                        state.AddMessage(new Message(MessageCategory.Error, "Значение параметра {0} нельзя редактировать вручную", node.Text));
                    else if (SecurityManager.CheckAccess(state, node, Privileges.Write | Privileges.Execute))
                        lstPackages.Add(item);
                }
                else state.AddMessage(new Message(MessageCategory.Error, "{0}", item));
            }

            SaveValues(state, progressToLoad, lstPackages.ToArray(), false);
        }

        public void DeleteValues(OperationState state, double progressToLoad,
            IEnumerable<Tuple<UnitNode, ArgumentsValues, DateTime>> toDelete)
        {
            if (dbwork == null) throw new Exception("ValueParam.DeleteValues: Нет подключения к базе данных");

            int transactionID = 0;
            try
            {
                transactionID = dbwork.StartTransaction();
                Dictionary<UnitNode, List<DateTime>> pureValuesDictionary = new Dictionary<UnitNode, List<DateTime>>();
                Dictionary<UnitNode, Dictionary<DateTime, List<ArgumentsValues>>> arguedValuesDictionary = new Dictionary<UnitNode, Dictionary<DateTime, List<ArgumentsValues>>>();
                List<DateTime> timeList;
                Dictionary<DateTime, List<ArgumentsValues>> tempDictionary;
                List<ArgumentsValues> argsList;

                foreach (var item in toDelete)
                {
                    if (item.Item2 == null || item.Item2.Count == 0)
                    {
                        if (!pureValuesDictionary.TryGetValue(item.Item1, out timeList))
                        {
                            pureValuesDictionary[item.Item1] = timeList = new List<DateTime>();
                        }
                        timeList.Add(item.Item3);
                    }
                    else
                    {
                        if (!arguedValuesDictionary.TryGetValue(item.Item1, out tempDictionary))
                        {
                            arguedValuesDictionary[item.Item1] = tempDictionary = new Dictionary<DateTime, List<ArgumentsValues>>();
                        }
                        if (!tempDictionary.TryGetValue(item.Item3, out argsList))
                        {
                            tempDictionary[item.Item3] = argsList = new List<ArgumentsValues>();
                        }
                        argsList.Add(item.Item2);
                    }
                }

                foreach (var unitNode in pureValuesDictionary.Keys)
                {
                    DeletePureValues(state, transactionID, unitNode, pureValuesDictionary[unitNode]);

                    AuditDeleteValues(state, unitNode, pureValuesDictionary[unitNode]);
                }
                foreach (var unitNode in arguedValuesDictionary.Keys)
                {
                    foreach (var time in arguedValuesDictionary[unitNode].Keys)
                    {
                        DeleteArgumentedValues(state, transactionID, unitNode, time, arguedValuesDictionary[unitNode][time]);

                        AuditDeleteValues(state, unitNode, time, arguedValuesDictionary[unitNode][time]);
                    }
                }
                SaveAudit(state);

                dbwork.Commit(transactionID);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transactionID);
                log.ErrorException("ValueParam.SaveValueParameter", ex);
                throw;
            }
            finally
            {
                if (transactionID > 0)
                    dbwork.CloseTransaction(transactionID);
            }
        }


        private void DeletePureValues(OperationState state, int transactionID, UnitNode unitNode, List<DateTime> list)
        {
            foreach (DateTime dateTime in list)
            {
                if (lockedTime > dateTime)
                {
                    state.AddMessage(new Message(MessageCategory.Error,
                        "Данное значение зафиксированно и не может быть изменено {0} за '{1:yyyy-MM-dd HH:mm:ss}'",
                            unitNode.Text, dateTime));
                    continue;
                }

                DB_Parameters a = new DB_Parameters();
                a.Add("idparam", DbType.Int32, unitNode.Idnum);
                a.Add("time", DbType.DateTime, dateTime);

                dbwork.ExecSQL("delete from value_mparam where idparam=@idparam AND time=@time", a);
            }
        }

        private void DeleteArgumentedValues(OperationState state, int transactionID, UnitNode unitNode, DateTime time, List<ArgumentsValues> list)
        {
            var currentValues = LoadArgumentedValues(state, transactionID, unitNode, time);
            // объединяем значения
            var argumentedList = from v in currentValues where !list.Contains(v.Arguments) select v;
            // сохраняем значения
            SaveArgumentedValues(state, transactionID, unitNode, time, argumentedList);
        }

        private void OnValuesChanged(Dictionary<UnitNode, ParamValueItem> lastChangedDictionary)
        {
            if (ValuesChanged != null)
                ValuesChanged(this, new ValueChangedEventArgs(lastChangedDictionary));
        }

        [Serializable]
        public class ValueChangedEventArgs : EventArgs
        {
            private Dictionary<UnitNode, ParamValueItem> lastChangedDictionary;

            public UnitNode[] Nodes { get { return lastChangedDictionary.Keys.ToArray(); } }

            public ParamValueItem GetLastValue(UnitNode unitNode)
            {
                return lastChangedDictionary[unitNode];
            }

            public ValueChangedEventArgs(Dictionary<UnitNode, ParamValueItem> lastChangedDictionary)
            {
                this.lastChangedDictionary = lastChangedDictionary;
            }
        }

        public event EventHandler<ValueChangedEventArgs> ValuesChanged;

        /// <summary>
        /// Сохранить значения
        /// </summary>
        /// <param name="packages">Значения</param>
        public void SaveValues(OperationState state, Package[] packages)
        {
            SaveValues(state, AsyncOperation.MaxProgressValue, packages, false);
        }

        public void SaveValues(OperationState state, double progressToLoad, Package[] package, bool excessArgs)
        {
            int transactionID = 0;

            try
            {
                Dictionary<UnitNode, ParamValueItem> changedValuesDictionary = new Dictionary<UnitNode, ParamValueItem>();

                transactionID = dbwork.StartTransaction();
                foreach (var pack in package)
                {
                    SaveValues(state, transactionID, pack, excessArgs, changedValuesDictionary);
                }

                dbwork.Commit(transactionID);

                SaveAudit(state);

                OnValuesChanged(changedValuesDictionary);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transactionID);
                log.ErrorException("ValueParam.SaveValueParameter: ", ex);
                throw;
            }
            finally
            {
                dbwork.CloseTransaction(transactionID);
            }
        }

        private void SaveAudit(OperationState state)
        {
            var entry = new AuditEntry(SecurityManager.GetUserInfo(state.UserGUID));

            foreach (var item in state.AsyncResult)
            {
                var auditValue = item as AuditValue;

                if (auditValue != null)
                {
                    entry.AuditValues.Add(auditValue);
                }
            }

            Audit.WriteAuditEntry(entry);
        }

        private void SaveValues(OperationState state,
                                int transactionID,
                                Package pack,
                                bool excessArgs,
                                Dictionary<UnitNode, ParamValueItem> lastChangedValues)
        {
            //UnitNode unitNode = UnitManager.GetUnitNode(state, pack.Id);
            UnitNode unitNode = UnitManager.ValidateUnitNode(state, pack.Id, Privileges.Execute);

            String[] args = UnitManager.GetRequiredArguments(state, unitNode);

            // значения с хороошим качеством и Arguments == null пропускаются без проблем
            var errorValues = (from p in pack.Values
                               where p.Arguments != ArgumentsValues.BadArguments 
                               && ((p.Arguments == null && args != null && args.Length > 0)
                               || (p.Arguments != null && !p.Arguments.CorrespondTo(args)))
                               select p).ToArray();
            IEnumerable<ParamValueItem> values;

            if (errorValues.Count() > 0)
            {
                state.AddMessage(new Message(MessageCategory.Error, "Попытка сохранить значения параметра '{0}' {1} значений с некоректным набором аргументов. Требуются аргументы: {2}", unitNode, errorValues.Count(), ArgumentsString(args)));

                values = from v in pack.Values where !errorValues.Contains(v) select v;
            }
            else
                values = pack.Values;

            // запоминаем последнее изменённое значение
            // пока только для обычных параметров
            if (lastChangedValues != null && (args == null || args.Length == 0))
            {
                ParamValueItem oldVlaue;
                ParamValueItem valItem = (from v in values where v.Quality != Quality.Bad orderby v.Time descending select v).FirstOrDefault();

                if (valItem != null &&
                    (!lastChangedValues.TryGetValue(unitNode, out oldVlaue)
                    || valItem.Time > oldVlaue.Time))
                    lastChangedValues[unitNode] = valItem;
            }

            if (args == null || args.Length == 0)
                SavePureValues(state, transactionID, unitNode, values);
            else
                SaveArguredValues(state, transactionID, unitNode, values, excessArgs);

                AuditValues(state, unitNode, values);
        }

        private static void AuditValues(OperationState state, UnitNode unitNode, IEnumerable<ParamValueItem> values)
        {
            foreach (var item in values)
            {
                if (unitNode.Typ == (int)UnitTypeId.ManualParameter || item is CorrectedParamValueItem)
                {
                    var corrected = item as CorrectedParamValueItem;

                    var auditValue = new AuditValue()
                    {
                        UnitNodeID = unitNode.Idnum,
                        UnitNodeFullPath = unitNode.FullName,
                        ValueTime = item.Time,
                        ValueArgs = item.Arguments == null ? null : item.Arguments.ToString(),
                        ValueNew = double.IsNaN(item.Value) ? null : (decimal?)item.Value
                    };

                    if (corrected != null)
                    {
                        auditValue.ValueOriginal = double.IsNaN(corrected.OriginalValueItem.Value) ? null : (decimal?)corrected.OriginalValueItem.Value;
                        auditValue.ValueNew = double.IsNaN(corrected.CorrectedValueItem.Value) ? null : (decimal?)corrected.CorrectedValueItem.Value;
                    }

                    state.AddAsyncResult(auditValue);
                }
            }
        }

        private void AuditDeleteValues(OperationState state, UnitNode unitNode, List<DateTime> list)
        {
            if (unitNode.Typ == (int)UnitTypeId.ManualParameter 
                || unitNode.Typ == (int)UnitTypeId.TEP)
            {
                foreach (var time in list)
                {
                    var auditValue = new AuditValue()
                    {
                        UnitNodeID = unitNode.Idnum,
                        UnitNodeFullPath = unitNode.FullName,
                        ValueTime = time,
                    };

                    state.AddAsyncResult(auditValue);
                }
            }
        }

        private void AuditDeleteValues(OperationState state, UnitNode unitNode, DateTime time, List<ArgumentsValues> list)
        {
            if (unitNode.Typ == (int)UnitTypeId.ManualParameter
                || unitNode.Typ == (int)UnitTypeId.TEP)
            {
                foreach (var args in list)
                {
                    var auditValue = new AuditValue()
                    {
                        UnitNodeID = unitNode.Idnum,
                        UnitNodeFullPath = unitNode.FullName,
                        ValueTime = time,
                        ValueArgs = args == null ? null : args.ToString(),
                    };

                    state.AddAsyncResult(auditValue);
                }
            }
        }

        private String ArgumentsString(string[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            if (args != null)
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        builder.Append(", ");
                    builder.Append(args[i]);
                }
            builder.Append(")");
            return builder.ToString();
        }

        private void SavePureValues(OperationState state, int transactionID, UnitNode unitNode, IEnumerable<ParamValueItem> values)
        {
            const String updateValueQuery = "update value_mparam set ch_time=@ch_time, value=@value, time=@time " +
                                           "where idparam=@idparam AND time=@orign_time";
            const String insertValueQuery = "insert into value_mparam(idparam,time,ch_time,value) " +
                                           "values(@idparam,@time,@ch_time,@value)";
            const String updateCorrectedValueQuery = "update value_mparam set ch_time=@ch_time,value_corr=@value, time=@time " +
                                           "where idparam=@idparam AND time=@orign_time";
            const String insertCorrectedValueQuery = "insert into value_mparam(idparam,time,ch_time,value_corr) " +
                                           "values(@idparam,@time,@ch_time,@value)";

            if (dbwork == null) throw new Exception("ValueParam.SaveValueParameter: Нет подключения к базе данных");

            decimal valueToWrite;
            String updateQuery, insertQuery;
            CorrectedParamValueItem correctedValue;
            DateTime dtnow = DateTime.Now;

            foreach (ParamValueItem receiveItem in values)
            {
                if (lockedTime > receiveItem.Time)
                {
                    state.AddMessage(new Message(MessageCategory.Error,
                        "Данное значение зафиксированно и не может быть изменено {0} за '{1:yyyy-MM-dd HH:mm:ss}'",
                        unitNode.Idnum, receiveItem.Time));
                    continue;
                }

                correctedValue = receiveItem as CorrectedParamValueItem;
                DB_Parameters a = new DB_Parameters();
                int rows;
                a.Add("idparam", DbType.Int32, unitNode.Idnum);
                a.Add("time", DbType.DateTime, receiveItem.Time);
                if (correctedValue == null || DateTime.MinValue.Equals(correctedValue.OriginalValueItem.Time))
                    a.Add("orign_time", DbType.DateTime, receiveItem.Time);
                else
                    a.Add("orign_time", DbType.DateTime, correctedValue.OriginalValueItem.Time);
                a.Add("ch_time", DbType.DateTime, dtnow);

                double savingValue;
                // сохранить корректировку
                if (unitNode.Typ == (int)UnitTypeId.TEP && correctedValue != null)
                {
                    updateQuery = updateCorrectedValueQuery;
                    insertQuery = insertCorrectedValueQuery;
                    savingValue = correctedValue.CorrectedValueItem.Value;
                }
                else // сохранение обычного значения
                {
                    updateQuery = updateValueQuery;
                    insertQuery = insertValueQuery;
                    savingValue = receiveItem.Value;
                }
                // подготавливаем сохраняемое значение
                if (double.IsNaN(savingValue) || double.IsInfinity(savingValue))
                    a.Add("value", DbType.Decimal, DBNull.Value);
                else
                {
                    if (savingValue > maxDouble) valueToWrite = maxDecimal;
                    else if (savingValue < minDouble) valueToWrite = minDecimal;
                    else valueToWrite = Convert.ToDecimal(savingValue);
                    a.Add("value", DbType.Decimal, valueToWrite);
                }
                rows = dbwork.ExecSQL(transactionID, updateQuery, a);
                if (rows == 0)
                    dbwork.ExecSQL(transactionID, insertQuery, a);
            }
        }

        private void SaveArguredValues(OperationState state, int transactionID, UnitNode unitNode, IEnumerable<ParamValueItem> values, bool excessArgs)
        {
            // группируем значения по времени
            Dictionary<DateTime, List<ParamValueItem>> timeDictionary = new Dictionary<DateTime, List<ParamValueItem>>();
            List<ParamValueItem> argumentedList;

            foreach (var item in values)
            {
                if (!timeDictionary.TryGetValue(item.Time, out argumentedList))
                    timeDictionary[item.Time] = argumentedList = new List<ParamValueItem>();
                argumentedList.Add(item);
            }

            foreach (DateTime time in timeDictionary.Keys)
            {
                if (!excessArgs)
                {
                    // получаем имеющиеся значения
                    var currentValues = LoadArgumentedValues(state, transactionID, unitNode, time);
                    // объединяем значения
                    argumentedList = MergeArgumentedValues(state, currentValues, timeDictionary[time]);
                }
                else argumentedList = timeDictionary[time];
                // сохраняем значения
                SaveArgumentedValues(state, transactionID, unitNode, time, argumentedList);
            }
        }

        private List<ParamValueItem> MergeArgumentedValues(
            OperationState state,
            IEnumerable<ParamValueItem> currentValues,
            IEnumerable<ParamValueItem> list)
        {
            List<ParamValueItem> paramList = new List<ParamValueItem>(currentValues);

            foreach (var item in list)
            {
                ArgumentsValues args = item.Arguments;
                if (item is CorrectedParamValueItem)
                    args = ((CorrectedParamValueItem)item).OriginalValueItem.Arguments;

                ParamValueItem valueItem = paramList.Find(v => v.Arguments.Equals(args));
                if (valueItem != null)
                    paramList.Remove(valueItem);

                paramList.Add(item);
            }
            return paramList;
        }

        private IEnumerable<ParamValueItem> LoadArgumentedValues(OperationState state, int transactionID, UnitNode unitNode, DateTime time)
        {
            String selectQuery = "SELECT [idnum], [idparam], [time], [pack], [ch_time] FROM [value_aparam] WHERE [idparam] = @idparam AND @time = [time]";

            DB_Parameters a = new DB_Parameters();

            a.Add("idparam", DbType.Int32, unitNode.Idnum);
            a.Add("time", DbType.DateTime, time);

            DataTable tempTable = dbwork.ExecSQL_toTable(transactionID, selectQuery, a);
            List<ParamValueItem> retList = new List<ParamValueItem>();

            foreach (DataRow dataRow in tempTable.Rows)
            {
                DateTime changeTime = Convert.ToDateTime(dataRow["ch_time"]);
                var values = argumentedValuesFromBinary(time, (byte[])dataRow["pack"]);

                foreach (var item in values)
                {
                    item.ChangeTime = changeTime;
                }
                retList.AddRange(values);
            }
            return retList;
        }

        private IEnumerable<ParamValueItem> LoadArgumentedValues(OperationState state, int transactionID, UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            String selectQuery = "SELECT [idnum], [idparam], [time], [pack], [ch_time] FROM [value_aparam] WHERE [idparam] = @idparam AND @start <= [time] AND [time] <= @end";

            DB_Parameters a = new DB_Parameters();

            a.Add("idparam", DbType.Int32, unitNode.Idnum);
            a.Add("start", DbType.DateTime, startTime);
            a.Add("end", DbType.DateTime, endTime);

            DataTable tempTable = dbwork.ExecSQL_toTable(transactionID, selectQuery, a);
            List<ParamValueItem> retList = new List<ParamValueItem>();

            foreach (DataRow dataRow in tempTable.Rows)
            {
                DateTime time = Convert.ToDateTime(dataRow["time"]);
                DateTime changeTime = Convert.ToDateTime(dataRow["ch_time"]);

                var values = argumentedValuesFromBinary(time, (byte[])dataRow["pack"]);
                foreach (var item in values)
                {
                    item.ChangeTime = changeTime;
                }
                retList.AddRange(values);
            }
            return retList;
        }

        private void SaveArgumentedValues(OperationState state, int transactionID, UnitNode unitNode, DateTime time, IEnumerable<ParamValueItem> argumentedList)
        {
            String insertQuery = "INSERT INTO [value_aparam] ([idparam], [time], [pack], [ch_time]) VALUES (@idparam, @time, @pack, @ch_time)";
            String updateQuery = "UPDATE [value_aparam] SET [idparam] = @idparam, [time] = @time, [pack] = @pack, [ch_time] = @ch_time WHERE [time] = @time AND [idparam] = @idparam";

            DB_Parameters a = new DB_Parameters();
            int rows;
            a.Add("idparam", DbType.Int32, unitNode.Idnum);
            a.Add("time", DbType.DateTime, time);
            a.Add("ch_time", DbType.DateTime, DateTime.Now);

            a.Add("pack", DbType.Binary, argumentedValuesToBinary(argumentedList));

            rows = dbwork.ExecSQL(transactionID, updateQuery, a);
            if (rows == 0)
                dbwork.ExecSQL(transactionID, insertQuery, a);
        }

        private IEnumerable<ParamValueItem> argumentedValuesFromBinary(DateTime time, byte[] buffer)
        {
            List<ParamValueItem> listValues = new List<ParamValueItem>();
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    String name;
                    double value = double.NaN, argumentValue;
                    double correctedValue = double.NaN;
                    ArgumentsValues arguments = new ArgumentsValues();
                    bool valueReaded = false;

                    while (ms.Length > ms.Position)
                    {
                        ArgumentedValuesBynaryKeyword keyword = (ArgumentedValuesBynaryKeyword)br.ReadByte();
                        switch (keyword)
                        {
                            case ArgumentedValuesBynaryKeyword.Argument:
                                if (valueReaded)
                                {
                                    if (!double.IsNaN(correctedValue))
                                        listValues.Add(new CorrectedParamValueItem(new ParamValueItem(arguments, time, Quality.Good, value), correctedValue));
                                    else
                                        listValues.Add(new ParamValueItem(arguments, time, Quality.Good, value));
                                    arguments = new ArgumentsValues();
                                    valueReaded = false;
                                    value = correctedValue = double.NaN;
                                }
                                name = br.ReadString();
                                argumentValue = br.ReadDouble();
                                arguments[name] = argumentValue;
                                break;
                            case ArgumentedValuesBynaryKeyword.Value:
                                value = br.ReadDouble();
                                valueReaded = true;
                                break;
                            case ArgumentedValuesBynaryKeyword.Correct:
                                correctedValue = br.ReadDouble();
                                valueReaded = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (valueReaded)
                        if (!double.IsNaN(correctedValue))
                            listValues.Add(new CorrectedParamValueItem(new ParamValueItem(arguments, time, Quality.Good, value), correctedValue));
                        else
                            listValues.Add(new ParamValueItem(arguments, time, Quality.Good, value));
                }
            }
            return listValues;
        }

        private byte[] argumentedValuesToBinary(IEnumerable<ParamValueItem> argumentedList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    CorrectedParamValueItem correctedValue;
                    foreach (var item in argumentedList)
                    {
                        //if (item.Quality != Quality.Bad)
                        if (item.Arguments != ArgumentsValues.BadArguments)
                        {
                            foreach (var name in item.Arguments)
                            {
                                bw.Write((byte)ArgumentedValuesBynaryKeyword.Argument);
                                bw.Write(name);
                                bw.Write(item.Arguments[name]);
                            }
                            bw.Write((byte)ArgumentedValuesBynaryKeyword.Value);
                            if ((correctedValue = item as CorrectedParamValueItem) != null)
                            {
                                bw.Write(correctedValue.OriginalValueItem.Value);
                                bw.Write((byte)ArgumentedValuesBynaryKeyword.Correct);
                                bw.Write(correctedValue.CorrectedValueItem.Value);
                            }
                            else
                            {
                                bw.Write(item.Value);
                            }
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        #endregion

        #region Work with arguments

        enum ArgumentedValuesBynaryKeyword : byte
        {
            Argument,
            Value,
            Correct,
            //LevelDown,
            //LevelUp
        }
        #endregion

        private static List<ParamValueItem> FillSourceBad(DateTime beginTime, DateTime endTime, Interval sourceInterval/*, Interval destInterval*/, List<ParamValueItem> values)
        {
            if (sourceInterval == Interval.Zero)
                return values;

            DateTime time = sourceInterval.NearestEarlierTime(beginTime);
            DateTime nextTime = sourceInterval.GetNextTime(time);

            List<ParamValueItem> retValues = new List<ParamValueItem>();

            foreach (var value in values)
            {
                while (time < value.Time)
                {
                    retValues.Add(new ParamValueItem(time, Quality.Bad, double.NaN));
                    time = nextTime;
                    nextTime = sourceInterval.GetNextTime(time);
                }

                if (value.Time >= time)
                {
                    time = nextTime;
                    nextTime = sourceInterval.GetNextTime(time);
                }
                retValues.Add(value);
            }
            while (time <= endTime)
            {
                retValues.Add(new ParamValueItem(time, Quality.Bad, double.NaN));
                time = nextTime;
                nextTime = sourceInterval.GetNextTime(time);
            }
            return retValues;
        }
    }
}
