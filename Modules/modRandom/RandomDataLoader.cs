using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace COTES.ISTOK.Modules.modRandom
{
    /// <summary>
    /// Модуль сбора случайных чисел
    /// </summary>
    class RandomDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        Random rand = new Random();

        Timer timer;

        float minValue;
        float maxValue;
        Interval genPeriod;
        int genDelay;
        //double dieProb;
        //int dieInterval;

        ChannelInfo channel;

        DataLoadMethod loadMethod;

        private List<ParamValueItem> GetTagValues(DateTime timeStart, DateTime timeEnd)
        {
            List<ParamValueItem> table = new List<ParamValueItem>();
            ParamValueItem param;
            DateTime curTime;
            //TimeSpan span;
            //Random rand = new Random();
            double value;
            //int i;
            //long tmp;
            int min, max;

            //span = new TimeSpan(timeStart.Ticks);
            //tmp = (long)span.TotalMilliseconds / genPeriod;
            //tmp *= genPeriod;
            //span = TimeSpan.FromMilliseconds((double)tmp);
            //curTime = new DateTime(span.Ticks);

            //Interval interval = new Interval(genPeriod / 1000);

            curTime = genPeriod.NearestEarlierTime(timeStart);

            min = (int)minValue;
            max = (int)maxValue;

            while (timeEnd.Subtract(curTime).TotalSeconds >= 0)
            {
                //for (i = 0; i < m_parameters.Count; i++)
                //{
                value = (double)rand.Next(min, max);
                param = new ParamValueItem(curTime, Quality.Good, value);
                //param = (Parameter)m_parameters[i].Clone();
                //param.Time = curTime.ToString();
                //param.Value = value.ToString();
                //param.Quality = "100";
                //table.AddParameter(param);
                table.Add(param);
                //}
                //curTime = curTime.AddMilliseconds((double)genPeriod);
                curTime = genPeriod.GetTime(curTime, 1); //.GetNextTime(curTime);
            }

            //if (dieProb > (rand.Next(0, 100) / 100.0)) System.Threading.Thread.Sleep(dieInterval);
            return table;
        }

        public void VerefyLoadMode(DataLoadMethod loadMode)
        {
            if (loadMode != loadMethod)
            {
                String message = String.Format(channelLogPrefix + "Канал сбора находится в режиме {0}, но к нему обращаются как {1}.", LoadMethod, loadMode);
                log.Error(message);
                throw new InvalidOperationException(message);
            }
        }

        #region IDataLoader Members

        List<ParameterItem> parameters;

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала.");

            minValue = float.Parse(channelInfo[RandomDataLoaderFactory.MinValueProperty]);
            maxValue = float.Parse(channelInfo[RandomDataLoaderFactory.MaxValueProperty]);
            int period = int.Parse(channelInfo[RandomDataLoaderFactory.GenPeriodProperty]);
            genPeriod = new SecondsInterval(period / 1000.0);
            genDelay = int.Parse(channelInfo[RandomDataLoaderFactory.GenDelayProperty]);

            loadMethod = (DataLoadMethod)int.Parse(channelInfo[CommonProperty.ChannelLoadMethodProperty]);

            parameters = new List<ParameterItem>(channelInfo.Parameters);
            channel = channelInfo;

            log.Debug(channelLogPrefix + "Канал инициирован в режиме '{0}'. Зарегистрировано {1} параметров.", loadMethod, parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> parametersList = new List<ParameterItem>();
            string param;
            int count = 100;
            int i;

            log.Trace(channelLogPrefix + "Запрос параметров.");

            for (i = 0; i < count; i++)
            {
                param = "Param" + i.ToString();

                ParameterItem paramSendItem = new ParameterItem()
                {
                    Name = param
                };
                paramSendItem[CommonProperty.ParameterCodeProperty] = param;
                parametersList.Add(paramSendItem);
            }

            log.Debug(channelLogPrefix + "Получен список из {0} параметров.", parametersList.Count);
            return parametersList.ToArray();
        }

        public IDataListener DataListener { get; set; }

        public DataLoadMethod LoadMethod
        {
            get { return loadMethod; }
        }

        public void RegisterSubscribe()
        {
            log.Trace(channelLogPrefix + "Запрос на регистрацию подписки.");

            VerefyLoadMode(DataLoadMethod.Subscribe);

            timer = new Timer(GetSubscribeValue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(genDelay));
            log.Debug(channelLogPrefix + "Зарегистрирована подписка.");
        }

        private void GetSubscribeValue(object state)
        {
            log.Trace(channelLogPrefix + "Получены данные по подписке.");

            VerefyLoadMode(DataLoadMethod.Subscribe);

            GenerateCurrentValues();
        }

        public void UnregisterSubscribe()
        {
            log.Trace(channelLogPrefix + "Запрос на удаление подписки.");

            VerefyLoadMode(DataLoadMethod.Subscribe);

            timer.Change(-1, -1);
            timer.Dispose();
            timer = null;
            log.Debug(channelLogPrefix + "Подписка удалена.");
        }

        public void GetCurrent()
        {
            log.Trace(channelLogPrefix + "Запрос текущих данных.");

            VerefyLoadMode(DataLoadMethod.Current);

            GenerateCurrentValues();
        }

        private void GenerateCurrentValues()
        {
            int valueCount = 0;

            DateTime curTime = genPeriod.NearestEarlierTime(DateTime.Now);

            foreach (var parameter in parameters)
            {
                double value = (double)rand.Next((int)minValue, (int)maxValue);
                ParamValueItem valueItem = new ParamValueItem(curTime, Quality.Good, value);

                DataListener.NotifyValues(null, parameter, new ParamValueItem[] { valueItem });
               
                if (log.IsDebugEnabled)
                {
                    ++valueCount;
                }
            }

            log.Debug(channelLogPrefix + "Получено текущих данных: {0}.", valueCount);
        }

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);

            VerefyLoadMode(DataLoadMethod.Archive);

            int valuesCount = 0;

            List<ParamValueItem> table = new List<ParamValueItem>();

            foreach (var parameter in parameters)
            {
                table = GetTagValues(startTime, endTime);

                DataListener.NotifyValues(null, parameter, table);

                if (log.IsDebugEnabled)
                {
                    valuesCount += table.Count;
                }
            }
            log.Debug(channelLogPrefix + "Получено архивных данных за период [{0}; {1}]: {2}", startTime, endTime, valuesCount);
        }

        ArchiveRequestTransaction currentTransaction = new ArchiveRequestTransaction();

        public void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            log.Trace(channelLogPrefix + "Настройка запроса архивных данных по параметрам {0} [{1}; {2}].", parameter, startTime, endTime);

            VerefyLoadMode(DataLoadMethod.ArchiveByParameter);

            currentTransaction.SetInterval(parameter, startTime, endTime);
        }

        public void GetArchive()
        {
            VerefyLoadMode(DataLoadMethod.ArchiveByParameter);

            if (log.IsTraceEnabled)
            {
                log.Trace(channelLogPrefix + "Запрос архивных данных по {0} параметрам", currentTransaction.GetParameters().Count());
            }

            int valuesCount = 0;
            ArchiveRequestTransaction transaction = currentTransaction;
            currentTransaction = new ArchiveRequestTransaction();

            List<ParamValueItem> table = new List<ParamValueItem>();

            foreach (var parameter in transaction.GetParameters())
            {
                DateTime startTime = transaction.GetStartTime(parameter);
                DateTime endTime = transaction.GetEndTime(parameter);

                table = GetTagValues(startTime, endTime);

                DataListener.NotifyValues(null, parameter, table);

                if (log.IsDebugEnabled)
                {
                    valuesCount += table.Count;
                }
            }

            log.Debug(channelLogPrefix + "Получено архивных данных: {0}", valuesCount);
        }

        #endregion
    }
}
