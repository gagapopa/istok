using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Runtime.Remoting;
using System.IO;

namespace COTES.ISTOK.ParameterReceiverExtension
{
    /// <summary>
    /// Приемник данных из ИСТОК для передачи внешним системам
    /// </summary>
    public class ISTOKExtension : IParameterReceiver
    {
        public const String IstokDataReadMutexName = "Global\\ISTOK-A7AAB2A7-CCCA-4ca1-A2FF-B13A4D24A0D4";
     
        /// <summary>
        /// Объект синхронизации
        /// </summary>
        private Mutex istokDataReadMutex;

        /// <summary>
        /// Время ожидания после ошибки
        /// </summary>
        TimeSpan onErrorSleepTimeSpan = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Задержка при вызове события
        /// </summary>
        TimeSpan eventDelayTimeSpan = TimeSpan.FromSeconds(2);

        ///// <summary>
        ///// Адрес подключения к серверу приложения
        ///// </summary>
        //const String Url = "tcp://localhost:8001/" + "GlobalQueryManager.rem";

        public ISTOKExtension()
        {
            ThreadPool.QueueUserWorkItem(eventRaiseCallBack);
        }

        /// <summary>
        /// Прием события из ИСТОК и передача его далее
        /// </summary>
        /// <param name="state"></param>
        private void eventRaiseCallBack(Object state)
        {
            while (true)
            {
                try
                {
                    using (istokDataReadMutex = Mutex.OpenExisting(IstokDataReadMutexName))
                    {
                        Thread.Sleep(eventDelayTimeSpan);
                        istokDataReadMutex.WaitOne();
                        istokDataReadMutex.ReleaseMutex();
                        ThreadPool.QueueUserWorkItem(OnDataReady);
                    }
                }
                catch { Thread.Sleep(onErrorSleepTimeSpan); }
            }
        }

        /// <summary>
        /// Вызов события
        /// </summary>
        /// <param name="state"></param>
        private void OnDataReady(Object state)
        {
            if (DataReady != null)
                DataReady(this, EventArgs.Empty);
        }

        #region IParameterSource Members

        public event EventHandler DataReady;

        public Parameter[] GetParameters(DateTime start, DateTime finish)
        {
            List<Parameter> parameterList = new List<Parameter>();
            ITransmitterExtension extension = Connect();

            if (extension != null)
            {
                return extension.GetExtensionParameters(start, finish);
                //    Dictionary<int, String> codeDictionary = new Dictionary<int, string>();
                //    Parameter parameter;

                //    Package[] packs = extension.GetExtensionParameters(start, finish);
                //    foreach (Package package in packs)
                //    {
                //        String code;
                //        if (!codeDictionary.TryGetValue(package.Id, out code))
                //            codeDictionary[package.Id] = code = extension.GetParameterCode(package.Id);

                //        foreach (ParamValueItem item in package.Values)
                //        {
                //            parameter = new Parameter();
                //            parameter.Id = package.Id;
                //            parameter.BoilerID = 0;
                //            parameter.Code = code;
                //            parameter.Value = item.Value;
                //            parameter.Time = item.Time;
                //            parameter.Quality = item.Quality == Quality.Good ? 100 : 0;
                //            parameterList.Add(parameter);
                //        }
                //    }
            }
            //if (parameterList.Count == 0)
            return null;
            //return parameterList.ToArray();
        }

        /// <summary>
        /// Подключение к серверу приложения
        /// </summary>
        /// <returns></returns>
        private static ITransmitterExtension Connect()
        {
            ITransmitterExtension extension = null; ;

            //try
            //{
            //OldClientSettings.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISTOKReceiverExtension.xml");
            //BaseSettings.Instance.
            BaseSettings.Instance.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISTOKReceiverExtension.xml"));
            String host = BaseSettings.Instance["ISTOKExtension/host"].ToString();//OldClientSettings.Instance.LoadKey("ISTOKExtension/host");
            String port = BaseSettings.Instance["ISTOKExtension/port"].ToString();//OldClientSettings.Instance.LoadKey("ISTOKExtension/port");

            String url = String.Format("tcp://{0}:{1}/{2}", host, port, "GlobalQueryManager.rem");

            TcpChannel tcpChannel = null;
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            if (tcpChannel == null)
            {
                IDictionary ht = new Hashtable(StringComparer.OrdinalIgnoreCase);
                ht.Add("name", string.Empty);
                ht.Add("port", 0);
                ht.Add("typeFilterLevel", System.Runtime.Serialization.Formatters.TypeFilterLevel.Full);
                //ht["machineName"] = BaseSettings.Instance["ISTOKExtension/client_interface"].ToString();//OldClientSettings.Instance.ClientInterface;
                tcpChannel = new TcpChannel(ht, clientProvider, serverProvider);
                ChannelServices.RegisterChannel(tcpChannel, false);
            }

            // Регистрация типа клиента
            bool res = false;
            WellKnownClientTypeEntry[] types = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
            foreach (WellKnownClientTypeEntry typ in types)
            {
                res = typ.ObjectType.Equals(typeof(ITransmitterExtension));
                if (res) break;
            }
            if (!res)
            {
                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(typeof(ITransmitterExtension), url);
            }

            if (extension == null)
            {
                extension = (ITransmitterExtension)Activator.GetObject(typeof(ITransmitterExtension), url);
            }
            //}
            //catch
            //{
            //    extension = null;
            //}
            return extension;
        }

        #endregion
    }
}
