using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using COTES.ISTOK;
using NLog;
//using SimpleLogger;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Сохранить/получить значения
    /// </summary>
    public class ValueReceiver : BaseValueReceiver
    {
        ValueBuffer valBuffer = null;
        DALManager dalManager = null;

        Zipper packageZipper = null;
        UltimateZipper ultimateZipper = null;

        ValueAggregator aggregator = null;

        Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Создать новый экземпляр
        /// </summary>
        /// <param name="dalManager"></param>
        /// <param name="valBuffer">Буфер</param>
        public ValueReceiver(DALManager dalManager, 
                             ValueBuffer valBuffer)
        {
            this.dalManager = dalManager;
            this.valBuffer = valBuffer;
            //this.log = messageLog;
            packageZipper = new Zipper();
            ultimateZipper = new UltimateZipper();
            aggregator = new ValueAggregator();
        }

        /// <summary>
        /// Создать новый экземпляр
        /// </summary>
        /// <param name="dalManager"></param>
        /// <param name="valBuffer">Буфер</param>
        public ValueReceiver(DALManager dalManager) 
            :this(dalManager, null)
        {
            //
        }

        #region Packages: SavePackage(), LoadPackage() and RemovePackage()
        /// <summary>
        /// Сохранить пакет в БД
        /// </summary>
        /// <param name="package">Пакет</param>
        public virtual void SavePackage(Package package)
        {
            package.Normailze();
            byte[] buffer = packageZipper.ZipPackage((Package)package.Clone());
            DataTable dataTable = dalManager.CreatePackageDataTable();
            DataRow row = dataTable.NewRow();

            row["id"] = package.Id;
            row["tim1"] = package.DateFrom;
            row["tim2"] = package.DateTo;
            row["data"] = buffer;

            dataTable.Rows.Add(row);

            dalManager.SavePackage(dataTable);
        }

        /// <summary>
        /// Загрузить пакет из БД
        /// </summary>
        /// <param name="param_id">ИД параметра</param>
        /// <param name="time">Время</param>
        /// <returns></returns>
        public virtual Package LoadPackage(int param_id, DateTime time)
        {
            DataTable dataTable = dalManager.LoadPackage(param_id, time);
            Package package = null;

            foreach (DataRow row in dataTable.Rows)
            {
                package = packageZipper.UnZipPackage(param_id, (byte[])row["data"]);
                package.DateFrom = (DateTime)row["tim1"];
                package.DateTo = (DateTime)row["tim2"];
            }

            return package;
        }

        /// <summary>
        /// Загрузить пакет, предыдущий по времени
        /// </summary>
        /// <param name="param_id">ИД параметра</param>
        /// <param name="time">Время</param>
        /// <returns></returns>
        public virtual Package LoadPrevPackage(int param_id, DateTime time)
        {
            DataTable dataTable = dalManager.LoadPrevPackage(param_id, time);
            Package package = null;

            foreach (DataRow row in dataTable.Rows)
            {
                package = packageZipper.UnZipPackage(param_id, (byte[])row["data"]);
                package.DateFrom = (DateTime)row["tim1"];
                package.DateTo = (DateTime)row["tim2"];
            }

            return package;
        }

        /// <summary>
        /// Загрузить пакет, следующий по времени
        /// </summary>
        /// <param name="param_id">ИД параметра</param>
        /// <param name="time">Время</param>
        /// <returns></returns>
        public virtual Package LoadNextPackage(int param_id, DateTime time)
        {
            DataTable dataTable = dalManager.LoadNextPackage(param_id, time);
            Package package = null;

            foreach (DataRow row in dataTable.Rows)
            {
                package = packageZipper.UnZipPackage(param_id, (byte[])row["data"]);
                package.DateFrom = (DateTime)row["tim1"];
                package.DateTo = (DateTime)row["tim2"];
            }

            return package;
        }

        /// <summary>
        /// Загрузить пакеты из БД
        /// </summary>
        /// <param name="param_id">ИД параметра</param>
        /// <param name="beginTime">Требуемый интервал времени</param>
        /// <param name="endTime">Требуемый интервал времени</param>
        /// <param name="maxvalues">Максимальное количество запрашиваемых пакетов</param>
        /// <returns></returns>
        public List<Package> LoadPackage(int param_id, DateTime beginTime, DateTime endTime, int maxvalues)
        {
            return LoadPackage(param_id, beginTime, endTime, maxvalues, false);
        }

        private List<Package> LoadPackage(int param_id, DateTime beginTime, DateTime endTime, int maxvalues, bool blockException)
        {
            DataTable dataTable = dalManager.LoadPackage(param_id, beginTime, endTime, maxvalues);
            Package package;
            List<Package> packageList = new List<Package>();

            if (!blockException && dataTable.Rows.Count > maxvalues)
                throw new Exception(String.Format("Расчетное количество значений параметра '{0}' за интервал ['{1:yyyy-MM-dd HH:mm:ss}','{2:yyyy-MM-dd HH:mm:ss}') больше настроечного", param_id, beginTime, endTime));
            foreach (DataRow row in dataTable.Rows)
            {
                package = packageZipper.UnZipPackage(param_id, (byte[])row["data"]);
                package.DateFrom = (DateTime)row["tim1"];
                package.DateTo = (DateTime)row["tim2"];

                packageList.Add(package);
            }
            return packageList;
        }

        /// <summary>
        /// Удалить пакет из БД
        /// </summary>
        /// <param name="package">Пакет</param>
        public void RemovePackage(Package package)
        {
            if (package == null) return;
            dalManager.RemovePackage(package.Id, package.DateFrom);
        }
        #endregion

        #region GetValues()
        /// <summary>
        /// Получить последнее значение параметра
        /// </summary>
        /// <param name="paramID">Номер параметра</param>
        /// <returns>Параметр</returns>
        public ParamValueItem GetLastValue(int paramID)
        {
            ParamValueItem res = null;
            ParamValueItem dbRes = null;
            Package package = null;
            if (valBuffer != null) package = valBuffer.GetPackage(paramID);

            if (package != null && package.Count > 0)
            {
                res = (ParamValueItem)package.Values[package.Count - 1].Clone();
                res.Time = package.DateTo;
            }

            dbRes = GetLastValuesFromBase(paramID);
            if (res == null || (dbRes != null && dbRes.Time > res.Time)) res = dbRes;

            return res;
        }

        public ParamValueItem GetLastValuesFromBase(int paramID)
        {
            ParamValueItem res = null;
            Package package = LoadPrevPackage(paramID, DateTime.MaxValue);

            if (package == null)
                return null;

            if (package.Count > 0)
            {
                for (int i = package.Count - 1; i >= 0; i--)
                {
                    res = package.Values[i];
                    if (res.Quality != 0) break;
                }
            }

            return res;
        }

        public void GetValues(OperationState state, IEnumerable<BlockParameterValuesRequest> valuesRequestList, bool packed)
        {
            foreach (var valueRequest in valuesRequestList)
            {
                GetValues(state, valueRequest, packed);
            }
        }

        private void GetValues(OperationState state, BlockParameterValuesRequest valueRequest, bool packed)
        {
            bool multiParse = false;    // указывает если не удалось получить все результаты за один запрос
            DateTime maxValue;
            List<ParamValueItem> valuesList = null, tmpValuesList;

            log.Trace("GetValues(state, {0}, {1})", valueRequest, packed);

            if (valueRequest.AggregationInterval != Interval.Zero
                && valueRequest.Aggregation != CalcAggregation.Nothing)
            {
                multiParse = true;
            }

            DateTime maxTime = valueRequest.StartTime;
            state.StateString = "Запрос данных из БД";
            TimeSpan questSpan = valueRequest.EndTime.Subtract(valueRequest.StartTime);
            double curProgress = state.Progress;

            DateTime beginTime = valueRequest.StartTime;
            do
            {
                IEnumerable<ParamValueItem> tmp;
                List<IEnumerable<ParamValueItem>> sourceValues = new List<IEnumerable<ParamValueItem>>();

                DateTime sourceBeginTime = beginTime, sourceEndTime = valueRequest.EndTime;

                aggregator.GetSourceRange(Interval.Zero, valueRequest.AggregationInterval, ref sourceBeginTime, ref sourceEndTime);

                foreach (var param in valueRequest.Parameters)
                {
                    tmpValuesList = getValuesList(param.Idnum, sourceBeginTime, sourceEndTime, ref multiParse, out maxValue);

                    tmpValuesList = filterValues(param.Idnum, tmpValuesList);

                    if (valueRequest.Appertured)
                    {
                        if (DateTime.MinValue < maxTime && valuesList.Count > 0)
                        {
                            ParamValueItem item, paramValue = valuesList[valuesList.Count - 1];
                            if (maxTime > valueRequest.EndTime) maxTime = valueRequest.EndTime;
                            else maxTime = valueRequest.AppertureInterval.NearestEarlierTime(maxTime);

                            if (valueRequest.AppertureInterval == Interval.Zero || valueRequest.AppertureInterval.GetNextTime(maxTime) >= maxTime)
                            {
                                item = (ParamValueItem)paramValue.Clone();
                                item.Time = maxTime;
                                valuesList.Add(item);
                            }
                        }

                        if (valueRequest.AppertureInterval != Interval.Zero)
                        {
                            valuesList = generateApperure(valuesList, beginTime, valueRequest.AppertureInterval, (int)BlockSettings.Instance.MaxValuesCount);
                        }
                    }

                    sourceValues.Add(tmpValuesList);
                }


                tmp = aggregator.Aggregate(valueRequest.Aggregation, Interval.Zero, valueRequest.AggregationInterval, sourceValues.ToArray());
                if (valuesList != null)
                {
                    valuesList.AddRange(tmp);
                    valuesList.Sort(new Comparison<ParamValueItem>((x, y) => DateTime.Compare(x.Time, y.Time)));
                }
                else
                    valuesList = new List<ParamValueItem>(tmp);
                if (multiParse)
                {
                    if (valuesList != null && valuesList.Count > 0) maxTime = valuesList[valuesList.Count - 1].Time;
                    DateTime time = valueRequest.AggregationInterval.NearestEarlierTime(/*beginTime,*/ maxTime);
                    multiParse = beginTime != time; // если время не сместилось, то выход
                    if (multiParse)
                    {
                        for (int i = 0; i < valuesList.Count; i++)
                        {
                            if (valuesList[i].Time >= time)
                            {
                                valuesList.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    beginTime = time;
                    if (valuesList.Count > BlockSettings.Instance.MaxValuesCount)
                    {
                        state.AddMessage(new Message(MessageCategory.Error, "Количество запрошенных значений больше настроечного"));
                        multiParse = false;
                    }
                    state.Progress = curProgress + valueRequest.EndTime.Subtract(beginTime).TotalSeconds / questSpan.TotalSeconds * AsyncOperation.MaxProgressValue;
                }
            } while (multiParse);
            //state.Progress = curProgress + progressToLoad;

            state.StateString = "Передача данных";
            // формирование пачек 
            Package[] packages = AddParametersToAsyncResult(valueRequest.Parameters[0].Idnum, valueRequest.EndTime, valuesList, NSI.RetreivePackageSize);
            foreach (Package pack in packages)
            {
                if (packed)
                {
                    state.AddAsyncResult(ultimateZipper.Pack(pack));
                }
                else
                {
                    state.AddAsyncResult(pack);
                }
            }

            log.Trace("GetValues() end");
        }
        
        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public void AsyncGetValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime)
        //{
        //    AsyncGetValues(state, progressToLoad, paramID, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing);
        //}

        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public void AsyncGetValues(OperationState state, double progressToLoad, int paramID,
        //    DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        //{
        //    bool multiParse = false;    // указывает если не удалось получить все результаты за один запрос
        //    DateTime maxValue;
        //    List<ParamValueItem> valuesList = null, tmpValuesList;

        //    if (interval != Interval.Zero && aggregation != CalcAggregation.Nothing) multiParse = true;

        //    DateTime maxTime = beginTime;
        //    state.StateString = "Запрос данных из БД";
        //    TimeSpan questSpan = endTime.Subtract(beginTime);
        //    double curProgress = state.Progress;
        //    do
        //    {
        //        List<ParamValueItem> tmp;
        //        //DateTime startTime = beginTime;
        //        DateTime sourceBeginTime = beginTime, sourceEndTime = endTime;

        //        //GetSourceRange(ref sourceBeginTime, ref sourceEndTime, new Interval(1), interval);
        //        GetSourceRange(Interval.Second, interval, beginTime, endTime, out sourceBeginTime, out sourceEndTime);

        //        tmpValuesList = getValuesList(paramID, sourceBeginTime, sourceEndTime, ref multiParse, out maxValue);

        //        tmpValuesList = filterValues(paramID, tmpValuesList);
        //        //tmp = argregateMethod(paramID, beginTime, endTime, interval, aggregation, tmpValuesList);
        //        tmp = argregateMethod(/*beginTime,*/ interval, aggregation, tmpValuesList);
        //        if (valuesList != null)
        //        {
        //            valuesList.AddRange(tmp);
        //            valuesList.Sort(new Comparison<ParamValueItem>((x, y) => DateTime.Compare(x.Time, y.Time)));
        //        }
        //        else
        //            valuesList = new List<ParamValueItem>(tmp);
        //        if (multiParse)
        //        {
        //            if (valuesList != null && valuesList.Count > 0) maxTime = valuesList[valuesList.Count - 1].Time;
        //            DateTime time = interval.NearestEarlierTime(/*beginTime,*/ maxTime);
        //            multiParse = beginTime != time; // если время не сместилось, то выход
        //            if (multiParse)
        //            {
        //                for (int i = 0; i < valuesList.Count; i++)
        //                    if (valuesList[i].Time >= time) { valuesList.RemoveAt(i); i--; }
        //            }
        //            beginTime = time;
        //            if (valuesList.Count >BlockSettings.Instance.MaxValuesCount)
        //            {
        //                state.AddMessage(new Message(MessageCategory.Error, "Количество запрошенных значений больше настроечного"));
        //                multiParse = false;
        //            }
        //            state.Progress = curProgress + endTime.Subtract(beginTime).TotalSeconds / questSpan.TotalSeconds * progressToLoad;
        //        }
        //    } while (multiParse);
        //    state.Progress = curProgress + progressToLoad;

        //    state.StateString = "Передача данных";
        //    // формирование пачек 
        //    Package[] packages = AddParametersToAsyncResult(paramID, endTime, valuesList, NSI.RetreivePackageSize);
        //    foreach (Package pack in packages)
        //        state.AddAsyncResult(pack);
        //}

        private List<ParamValueItem> filterValues(int paramID, List<ParamValueItem> valueList)
        {
            //return NSI.chanManager.FilterValues(paramID, valueList);
            //            List<ParamValueItem> lstRes = new List<ParamValueItem>();
            //            ParamValueItem ptr;
            //            ParamItem param = null;
            //            double max = 0, min = 0;
            //            bool use_max, use_min;
            //            bool del;

            //            foreach (var item in lstParameters)
            //            {
            //                if (item.Idnum == param_id)
            //                    param = item;
            //            }

            //            if (param == null) return valueList;

            //            if (use_max = param.Properties.Contains("MaxValue"))
            //            {
            //                try
            //                {
            //                    max = double.Parse(param.Properties["MaxValue"].ToString());
            //                    //use_max = true;
            //                }
            //                catch (FormatException) { use_max = false; }
            //            }
            //            if (use_min = param.Properties.Contains("MinValue"))
            //            {
            //                try
            //                {
            //                    min = double.Parse(param.Properties["MinValue"].ToString());
            //                    //use_min = true;
            //                }
            //                catch (FormatException) { use_min = false; }
            //            }

            //            for (int i = 0; i < valueList.Count; i++)
            //            {
            //                del = false;
            //                ptr = valueList[i];

            //                if (use_max && ptr.Value > max) del = true;
            //                if (use_min && ptr.Value < min) del = true;

            //                if (del)
            //                {
            //                    valueList.RemoveAt(i);
            //                    i--;
            //                }
            //            }
            return valueList;
        }

        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public void AsyncGetValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime)
        //{
        //    AsyncGetValues(state, progressToLoad, paramIDs, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public void AsyncGetValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        //{
        //    List<Package> packageList = new List<Package>();
        //    double progresPerParam = progressToLoad / paramIDs.Length;
        //    foreach (int id in paramIDs)
        //    {
        //        AsyncGetValues(state, progresPerParam, id, beginTime, endTime, interval, aggregation);
        //    }
        //}
        #endregion

        //#region GetPackedValues()
        ///// <summary>
        ///// Запросить запакованные значения параметра за период
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns>Упакованная пачка значений</returns>
        //public void AsyncGetPackedValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime)
        //{
        //    AsyncGetValues(state, progressToLoad, paramID, beginTime, endTime);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения параметра за период
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Упакованная пачка значений</returns>
        //public void AsyncGetPackedValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation)
        //{
        //    AsyncGetValues(state, progressToLoad, paramID, beginTime, endTime, interval, aggregation);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения нескольких параметров за период
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns>Массив упакованных пачек значений</returns>
        //public void AsyncGetPackedValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime)
        //{
        //    AsyncGetValues(state, progressToLoad, paramIDs, beginTime, endTime);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения нескольких параметров за период
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Массив упакованных пачек значений</returns>
        //public void AsyncGetPackedValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        //{
        //    AsyncGetValues(state, progressToLoad, paramIDs, beginTime, endTime, interval, aggregation);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}
        //#endregion

        //#region GetAppertureValues()
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramID, beginTime, endTime, parameterInterval, Interval.Zero, CalcAggregation.Nothing);
        //}

        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    List<ParamValueItem> killList = new List<ParamValueItem>();
        //    List<ParamValueItem> valuesList;
        //    DateTime maxTime;
        //    bool multiParse = false;

        //    valuesList = getValuesList(paramID, beginTime, endTime, ref multiParse, out maxTime);

        //    if (DateTime.MinValue < maxTime && valuesList.Count > 0)
        //    {
        //        ParamValueItem item, param = valuesList[valuesList.Count - 1];
        //        if (maxTime > endTime) maxTime = endTime;
        //        else maxTime = parameterInterval.NearestEarlierTime(maxTime);

        //        if (parameterInterval == Interval.Zero || parameterInterval.GetNextTime(maxTime) >= maxTime)
        //        {
        //            item = (ParamValueItem)param.Clone();
        //            item.Time = maxTime;
        //            valuesList.Add(item);
        //        }
        //    }

        //    if (parameterInterval != Interval.Zero)
        //    {
        //        valuesList = generateApperure(valuesList, beginTime, parameterInterval, (int)BlockSettings.Instance.MaxValuesCount);
        //    }

        //    //valuesList = argregateMethod(paramID, beginTime, endTime, aggregInterval, aggregation, valuesList);
        //    valuesList = argregateMethod(/*beginTime,*/ aggregInterval, aggregation, valuesList);

        //    // формирование пачек 
        //    Package[] packages = AddParametersToAsyncResult(paramID, endTime, valuesList, NSI.RetreivePackageSize);
        //    foreach (Package pack in packages)
        //        state.AddAsyncResult(pack);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramIDs, beginTime, endTime, parameterInterval, Interval.Zero, CalcAggregation.Nothing);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    double progresPerParam = progressToLoad / paramIDs.Length;
        //    foreach (int id in paramIDs)
        //    {
        //        AsyncGetAppertureValues(state, progresPerParam, id, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    }
        //}
        //#endregion

        //#region GetPackedAppertureValues()
        ///// <summary>
        ///// Запросить запакованные значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns>Упакованная пачка значений</returns>
        //public void AsyncGetPackedAppertureValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramID, beginTime, endTime, parameterInterval);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramID">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Упакованная пачка значений</returns>
        //public void AsyncGetPackedAppertureValues(OperationState state, double progressToLoad, int paramID, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramID, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns>Массив упакованных пачек значений</returns>
        //public void AsyncGetPackedAppertureValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramIDs, beginTime, endTime, parameterInterval);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}

        ///// <summary>
        ///// Запросить запакованные значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="paramIDs">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Массив упакованных пачек значений</returns>
        //public void AsyncGetPackedAppertureValues(OperationState state, double progressToLoad, int[] paramIDs, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    AsyncGetAppertureValues(state, progressToLoad, paramIDs, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    for (int i = 0; i < state.AsyncResult.Count; i++)
        //    {
        //        state.AsyncResult[i] = ultimateZipper.Pack(state.AsyncResult[i] as Package);
        //    }
        //}
        //#endregion

        #region Private methods
        private List<ParamValueItem> getValuesList(int param_id, DateTime beginTime, DateTime endTime,
            ref bool multiParse, out DateTime maxTime)
        {
            try
            {
                int maxqueryvalues = (int)BlockSettings.Instance.MaxValuesCount / NSI.DBPackageSize;
                bool buffAdd = false;
                List<ParamValueItem> valuesList;
                List<Package> packageList = new List<Package>();
                Package bufferPackage = (valBuffer != null ? valBuffer.GetPackage(param_id) : null);
                DateTime beginTime1, endTime1, beginTime2, endTime2;

                valuesList = new List<ParamValueItem>();

                beginTime1 = beginTime; endTime1 = endTime;
                beginTime2 = endTime2 = DateTime.MinValue;

                if (bufferPackage != null 
                    && ((bufferPackage.DateFrom <= beginTime && beginTime < bufferPackage.DateTo)
                    || (bufferPackage.DateFrom <= endTime && endTime < bufferPackage.DateTo)
                    || (bufferPackage.DateFrom > beginTime && endTime > bufferPackage.DateTo)))
                {
                    //packageList.Add(bufferPackage);//(Package)bufferPackage.Clone());
                    buffAdd = true; // добавить буферную пачку когда ни будь позже
                    beginTime1 = beginTime; endTime1 = bufferPackage.DateFrom;
                    beginTime2 = bufferPackage.DateTo; endTime2 = endTime;
                    //maxqueryvalues -= bufferPackage.Values.Count;
                    --maxqueryvalues;
                }

                if (endTime1 > beginTime1)
                {
                    List<Package> loadedPackages = LoadPackage(param_id, beginTime1, endTime1, maxqueryvalues, multiParse);
                    maxqueryvalues -= loadedPackages.Count;// *NSI.PACKAGESIZE;
                    packageList.AddRange(loadedPackages);
                }
                if (endTime2 > beginTime2) packageList.AddRange(LoadPackage(param_id, beginTime2, endTime2, maxqueryvalues, multiParse));
                // TODO кривое решение, надо как то точно определить что полученная пачка является текущей в буфере
                if (buffAdd)
                {
                    Package bufferPackageClone = (Package)bufferPackage.Clone();
                    packageList.RemoveAll(pack => pack.DateFrom == bufferPackageClone.DateFrom && pack.DateTo == bufferPackageClone.DateTo);
                    packageList.Add(bufferPackageClone);
                }

                ParamValueItem prevValue = null;//, nextValue = null;
                DateTime prevTime = DateTime.MinValue, nextTime = DateTime.MaxValue;
                maxTime = DateTime.MinValue;
                foreach (Package package in packageList)
                {
                    if (package.DateTo > maxTime) maxTime = package.DateTo;
                    foreach (ParamValueItem value in package.Values)
                        if (beginTime <= value.Time && value.Time <= endTime)
                        {
                            valuesList.Add(value);
                            if (beginTime == value.Time) prevTime = beginTime;
                        }
                        else if (prevTime < value.Time && value.Time < beginTime)
                        {
                            prevValue = value;
                            prevTime = value.Time;
                        }
                }
                if (prevTime > DateTime.MinValue && prevTime < beginTime)
                {
                    //prevValue.tim = beginTime;
                    valuesList.Add(prevValue);
                }
                valuesList.Sort();

                if (multiParse) multiParse = packageList.Count >= (int)BlockSettings.Instance.MaxValuesCount / NSI.DBPackageSize;
                return valuesList;
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка запроса значений", ex);
                throw;
            }
        }
        #endregion

        public void DeleteValues(ParameterItem[] paramItem, DateTime timeFrom)
        {
            foreach (ParameterItem param in paramItem)
            {
                // удаление всех значений из буфера
                valBuffer.RemoveValue(param);

                DateTime nextTime = timeFrom;
                Package removePackage;
                // удалить все пачки со временем большим чем timeFrom
                while ((removePackage = LoadNextPackage(param.Idnum, nextTime)) != null)
                {
                    nextTime = removePackage.DateFrom;
                    RemovePackage(removePackage);
                }

                // удалить значения из пачки, содержащие данное значение
                Package pack = LoadPackage(param.Idnum, timeFrom);
                if (pack != null)
                {
                    pack.Values.RemoveAll(v => v.Time >= timeFrom);
                    pack.Normailze();
                    SavePackage(pack);
                }
            }
        }
    }

    class Zipper
    {
        public byte[] ZipPackage(Package package)
        {
            byte[] buffer = null;

            using (MemoryStream memStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Compress, true))
                {
                    using (BinaryWriter writer = new BinaryWriter(zipStream))
                    {
                        foreach (ParamValueItem receiveItem in package.Values)
                        {
                            writer.Write(receiveItem.Time.ToBinary());
                            writer.Write((int)receiveItem.Quality);
                            writer.Write(receiveItem.Value);
                        }
                    }
                }
                buffer = memStream.ToArray();
            }
            return buffer;
        }

        public Package UnZipPackage(int id, byte[] buffer)
        {
            Package package = new Package();
            package.Id = id;
            package.Values = new List<ParamValueItem>();

            using (MemoryStream mem = new MemoryStream())
            {
                using (MemoryStream m = new MemoryStream(buffer))
                {
                    using (GZipStream g = new GZipStream(m, CompressionMode.Decompress))
                    {
                        byte[] buff = new byte[1 << 12];
                        int c;
                        while ((c = g.Read(buff, 0, buff.Length)) > 0) mem.Write(buff, 0, c);
                    }
                }
                mem.Position = 0;
                using (BinaryReader r = new BinaryReader(mem, System.Text.Encoding.ASCII))
                {

                    while (mem.Length > mem.Position)
                    {
                        ParamValueItemWithID param = new ParamValueItemWithID();

                        param.ParameterID = id;
                        param.Time = DateTime.FromBinary(r.ReadInt64());
                        param.Quality = (Quality)r.ReadInt32();
                        param.Value = r.ReadDouble();

                        package.Values.Add(param);
                    }
                }
            }
            return package;
        }
    }
}
