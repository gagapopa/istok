using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace COTES.ISTOK
{
    /// <summary>
    /// Сохранить/получить значения
    /// </summary>
    public abstract class BaseValueReceiver
    {
        Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Сгенеровать апертуренные данные
        /// </summary>
        /// <param name="valuesList">Исходные данные</param>
        /// <param name="beginTime">Начальное время</param>
        /// <param name="interval">Интервал апертуры</param>
        /// <param name="maxvalues">Максимальное количество возвращаемых значений</param>
        /// <returns>Проапертуренные данные</returns>
        /// <exception cref="Exception">Сгенерированно слишком много значений</exception>
        protected virtual List<ParamValueItem> generateApperure(List<ParamValueItem> valuesList, DateTime beginTime, Interval interval, int maxvalues)
        {
            try
            {
                ParamValueItem prevItem = null;
                DateTime prevTime = DateTime.MinValue;
                List<ParamValueItem> anotherList = new List<ParamValueItem>();

                for (int i = 0; i < valuesList.Count && anotherList.Count < maxvalues; i++)
                {
                    while (prevTime > DateTime.MinValue && Interval.GetInterval(prevItem.Time, valuesList[i].Time) > interval
                        && anotherList.Count < maxvalues)
                    {
                        prevTime = interval.GetNextTime(prevTime);
                        ParamValueItem item = (ParamValueItem)prevItem.Clone();
                        item.Time = prevTime;
                        anotherList.Add(item);
                    }
                    anotherList.Add(valuesList[i]);
                    prevTime = valuesList[i].Time;
                    if (prevTime < beginTime)
                    {
                        prevTime = interval.NearestEarlierTime(/*prevTime,*/ beginTime);
                        if (prevTime.Equals(beginTime)) prevTime = interval.GetPrevTime(prevTime);
                    }
                    prevItem = valuesList[i];
                }

                if (anotherList.Count >= maxvalues)
                {
                    throw new Exception(
                        String.Format("Расчетное количество значений за интервал ['{0:yyyy-MM-dd HH:mm:ss}','{1:yyyy-MM-dd HH:mm:ss}') больше настроечного",
                                      valuesList[0].Time,
                                      valuesList[valuesList.Count - 1].Time));
                }

                anotherList.Sort();
                return anotherList;
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка генерации значений по апертуре", ex);
                throw;
            }
        }

        /// <summary>
        /// Сформировать на основе полученных данных пачки и добавить в результат операции
        /// </summary>
        /// <param name="paramID">Идентификатор параметра, чьи значения обрабатываются</param>
        /// <param name="endTime">Конечное время, выставляется у последней пачки для оченки продолжительности последнего значения</param>
        /// <param name="valuesList">Исходные данные</param>
        /// <param name="packageSize">Максимальный размер пачки</param>
        /// <returns>Сформрованные пачки данных</returns>
        protected virtual Package[] AddParametersToAsyncResult(int paramID,
                                                               DateTime endTime,
                                                               List<ParamValueItem> valuesList,
                                                               int packageSize)
        {
            List<Package> retPackageList = new List<Package>();
            for (int index = 0; index < valuesList.Count; index += packageSize)
            {
                int size = Math.Min(valuesList.Count - index, packageSize);
                ParamValueItem[] values = new ParamValueItem[size];
                Package package = new Package();
                package.Id = paramID;
                package.DateFrom = valuesList[index].Time;
                if (index + packageSize < valuesList.Count) package.DateTo = valuesList[index + packageSize].Time;
                else package.DateTo = endTime;
                valuesList.CopyTo(index, values, 0, size);
                package.Values = new List<ParamValueItem>(values);
                package.Normailze();
                retPackageList.Add(package);
            }
            return retPackageList.ToArray();
        }
    }
}
