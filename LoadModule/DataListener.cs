using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using COTES.ISTOK.Modules;
using NLog;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс принимающий в главном домене блочного от каналов сбора полученные значения 
    /// и синхронно передающий эти значения в буфер.
    /// </summary>
    class DataListener : MarshalByRefObject, IDataListener
    {
        /// <summary>
        /// Максимальное количество элементов в очереди.
        /// При превышении очериди заданной длины, вызывается событие ChannelQueueOverflow
        /// </summary>
        const int queueCountLimit = 1024;

        Logger log = LogManager.GetCurrentClassLogger();

        public override object InitializeLifetimeService()
        {
            return null;
        }
        TaskFactory factory;

        Dictionary<ChannelInfo, ChannelDataListenerInfo> channelsDictionary = new Dictionary<ChannelInfo, ChannelDataListenerInfo>();

        private ChannelDataListenerInfo GetChannel(ChannelInfo channelInfo)
        {
            ChannelDataListenerInfo channel;

            lock (channelsDictionary)
            {
                if (!channelsDictionary.TryGetValue(channelInfo, out channel))
                {
                    channel = new ChannelDataListenerInfo();
                    channel.BufferNotifyTask = factory.StartNew(NotifyMethod, channelInfo);

                    channelsDictionary[channelInfo] = channel;
                } 
            }

            return channel;
        }

        /// <summary>
        /// Событие возникает, когда количество элементов в очереди 
        /// для конкретного канала превышает заданное максимальное количество.
        /// Событие может вызываться много раз, 
        /// пока длина очереди не будет меньше максимального.
        /// </summary>
        public event Action<ChannelInfo> ChannelQueueOverflow;

        /// <summary>
        /// Событие возникает при полной очистке очереди канала, 
        /// для которого ранее было вызвано событие ChannelQueueOverflow.
        /// </summary>
        public event Action<ChannelInfo> ChannelQueueEmpted;

        private void OnChannelQueueOverflow(ChannelInfo channelInfo)
        {
            if (ChannelQueueOverflow != null)
            {
                ChannelQueueOverflow(channelInfo);
            }
        }

        private void OnChannelQueueEmpted(ChannelInfo channelInfo)
        {
            if (ChannelQueueOverflow != null)
            {
                ChannelQueueEmpted(channelInfo);
            }
        }

        public int GetChannelBufferCount(ChannelInfo channelInfo)
        {
            ChannelDataListenerInfo channel;

            lock (channelsDictionary)
            {
                if (channelsDictionary.TryGetValue(channelInfo, out channel))
                {
                    return channel.ValuesQueue.Count;
                }
            }

            return 0;
        }

        ValueBuffer buffer;

        public DataListener(ValueBuffer buffer)
        {
            this.buffer = buffer;

            factory = new System.Threading.Tasks.TaskFactory();
        }

        /// <summary>
        /// Метод, осуществляющий связь с буфером.
        /// Читает значения из очереди valuesQueue
        /// и передаёт их в буфер для дальнейшей обработки
        /// </summary>
        public void NotifyMethod(Object state)
        {
            ChannelInfo channelInfo = state as ChannelInfo;
            Tuple<ParameterItem, IEnumerable<ParamValueItem>> tuple;
            bool isOverloaded = false;

            var channel = GetChannel(channelInfo);
            var hasValuesEvent = channel.HasValueEvent;
            var valuesQueue = channel.ValuesQueue;

            try
            {
                while (true)
                {
                    hasValuesEvent.WaitOne();
                    hasValuesEvent.Reset();

                    while (buffer != null && valuesQueue.Count > 0)
                    {
                        if (valuesQueue.Count > queueCountLimit)
                        {
                            OnChannelQueueOverflow(channelInfo);
                            isOverloaded = true;
                        }

                        lock (channel)
                        {
                            tuple = valuesQueue.Dequeue();
                        }

                        var parameter = tuple.Item1;
                        var values = tuple.Item2;

                        buffer.AddValue(parameter, values);
                    }

                    if (isOverloaded)
                    {
                        OnChannelQueueEmpted(channelInfo);
                        isOverloaded = false;
                    }
                }
            }
            catch (Exception exc)
            {
                log.FatalException("Ошибка передачи собранных значений", exc);
                throw;
            }
        }

        #region IDataListener Members

        public void NotifyValues(ChannelInfo channelInfo, ParameterItem parameter, IEnumerable<ParamValueItem> values)
        {
            ChannelDataListenerInfo channel = GetChannel(channelInfo);

            lock (channel)
            {
                channel.ValuesQueue.Enqueue(Tuple.Create(parameter, new List<ParamValueItem>(values) as IEnumerable<ParamValueItem>));
            }

            channel.HasValueEvent.Set();
        }

        public void NotifyError(string errorMessage)
        {

        }

        #endregion

        class ChannelDataListenerInfo
        {
            public ManualResetEvent HasValueEvent { get; private set; }

            public Queue<Tuple<ParameterItem, IEnumerable<ParamValueItem>>> ValuesQueue { get; private set; }

            public Task BufferNotifyTask { get; set; }

            public ChannelDataListenerInfo()
            {
                HasValueEvent = new ManualResetEvent(false);
                ValuesQueue = new Queue<Tuple<ParameterItem, IEnumerable<ParamValueItem>>>();
            }
        }
    }

 }
