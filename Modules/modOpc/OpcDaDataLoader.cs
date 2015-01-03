using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using Opc;
using Opc.Da;

namespace COTES.ISTOK.Modules.modOpc
{
    public class WrongSpecificationException : Exception
    {
        public WrongSpecificationException()
        {

        }

        public WrongSpecificationException(string message)
            : base(message)
        {

        }

        public WrongSpecificationException(string message, Exception e)
            : base(message, e)
        {

        }

    }

    public class OpcDaDataLoader : OpcDataLoader, IDataLoader
    {
        private Opc.Da.Server opcDaServer = null;
        //private Opc.Server[] opcServers = null;
        private BrowseFilters filter = null;
        private Item[] arrParameters = null;

        //List<ParameterItem> parameters;
        ChannelInfo channel;

        private string m_Host;
        private string m_Server;
        //private string m_ConnectionType;
        private string collectionType;
        private bool subscribed;
        int subscribeRate;

        private Subscription sub;

        public OpcDaDataLoader()
        {
            log = LogManager.GetCurrentClassLogger();
            KeepConnected = true;
        }

        protected override void Connect()
        {
            if (opcDaServer != null && opcDaServer.IsConnected) return;

            log.Debug("{0}Подключение к OPC DA серверу \\\\{1}\\{2}.", channelLogPrefix, m_Host, m_Server);
            opcDaServer = (Opc.Da.Server)FindServer(m_Host, m_Server);

            if (opcDaServer != null)
            {
                opcDaServer.Connect();
                opcDaServer.ServerShutdown +=
                    new ServerShutdownEventHandler(opcDaServer_ServerShutdown);
            }
        }

        protected override void Disconnect()
        {
            log.Debug("{0}Отключение от OPC DA сервера \\\\{1}\\{2}.", channelLogPrefix, m_Host, m_Server);
            if (opcDaServer != null && opcDaServer.IsConnected) opcDaServer.Disconnect();
        }

        void opcDaServer_ServerShutdown(string reason)
        {
            subscribed = false;
            opcDaServer = null;
        }

        #region IDataLoader Members

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace("{0}Init({1})", channelLogPrefix, channelInfo);

            m_Host = channelInfo[OpcDataLoaderFactory.HostProperty];
            m_Server = channelInfo[OpcDataLoaderFactory.ServerProperty];
            m_ConnectionType = channelInfo[OpcDataLoaderFactory.ConnectionTypeProperty];
            m_TagCatalog = channelInfo[OpcDataLoaderFactory.TagCatalogProperty];

            collectionType = channelInfo[OpcDataLoaderFactory.CollectionTypeProperty];

            if (collectionType == "sub")
                LoadMethod = DataLoadMethod.Subscribe;
            else
                LoadMethod = DataLoadMethod.Current;

            if (LoadMethod == DataLoadMethod.Subscribe)
            {
                String pause = channelInfo.GetPropertyValue(CommonProperty.PauseProperty) ?? CommonProperty.PauseProperty.DefaultValue;
                double pauseInSeconds = double.Parse(pause, System.Globalization.NumberFormatInfo.InvariantInfo);
                subscribeRate = (int)(1000 * pauseInSeconds);
            }

            parameters = new List<ParameterItem>(channelInfo.Parameters);
            channel = channelInfo;

            arrParameters = new Item[parameters.Count];
            for (int i = 0; i < arrParameters.Length; i++)
            {
                arrParameters[i] = new Item(new ItemIdentifier(parameters[i][CommonProperty.ParameterCodeProperty]));
            }

            log.Debug("{0}Канал инициирован в режиме '{1}'. Зарегистрировано {2} параметров.", channelLogPrefix, LoadMethod, parameters.Count);
        }

        /// <summary>
        /// GetTagNames
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CannotReadTagsException"></exception>
        protected override TreeNode GetTagNames(TreeNode nParent)
        {
            log.Trace("{0}GetTagNames({1})", channelLogPrefix, nParent);

            BrowseElement[] elem = null;
            BrowseFilters filter = new BrowseFilters();
            BrowsePosition position = null;
            TreeNode node = null;

            TreeNode nNode = new TreeNode(null);
            string strParent = "";
            int i;

            if (opcDaServer == null) throw new ServerIsAbsentException("OpcDaServer");
            if (opcServers == null) throw new ServerIsAbsentException("OpcServers");
            if (nParent == null) throw new ArgumentNullException("nParent");

            filter.ReturnAllProperties = false;
            filter.ReturnPropertyValues = false;
            filter.MaxElementsReturned = 100;

            try
            {
                if (nParent.Tag == null && nParent.Text == "")
                {
                    strParent = "";
                }
                else
                {
                    if (nParent.Tag != null) strParent = nParent.Tag.ToString();
                    else strParent = nParent.Text;
                }
                var itemid = new ItemIdentifier(strParent);

                elem = opcDaServer.Browse(itemid, filter, out position);

                if (elem == null) throw new CannotReadTagsException(strParent);

                for (i = 0; i < elem.Length; i++)
                {
                    node = new TreeNode(elem[i].Name);

                    node.Tag = elem[i].ItemName;

                    if (elem[i].HasChildren)
                    {
                        try
                        {
                            node = GetTagNames(node);
                        }
                        catch (CannotReadTagsException)
                        {
                            //
                        }
                    }

                    nNode.Nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                throw new CannotReadTagsException(strParent + "(" + ex.Message + ")");
            }

            return nNode;
        }

        public IDataListener DataListener { get; set; }

        public DataLoadMethod LoadMethod { get; private set; }

        public void RegisterSubscribe()
        {
            log.Trace("{0}RegisterSubscribe()", channelLogPrefix);

            SubscriptionState st;

            if (subscribed && sub != null)
            {
                if (!sub.Enabled) sub.SetEnabled(true);
                return;
            }

            Connect();
            if (opcDaServer == null)
            {
                throw new ServerIsAbsentException("OpcDaServer");
            }

            st = new SubscriptionState();
            st.Active = true;
            
            //channel.GetPropertyValue(
            st.UpdateRate =  750;
            sub = (Subscription)opcDaServer.CreateSubscription(st);

            if (sub == null)
                throw new ArgumentException("Subscripstion is not created");

            sub.AddItems(arrParameters);

            sub.DataChanged += new DataChangedEventHandler(sub_DataChanged);

            subscribed = true;

            log.Debug("{0}Зарегистрирована подписка.", channelLogPrefix);
        }

        public void UnregisterSubscribe()
        {
            log.Trace("{0}UnregisterSubscribe()", channelLogPrefix);
            
            Subscription item = null;
            int i, index = -1;

            if (opcDaServer == null) throw new ServerIsAbsentException("OpcDaServer");

            if (sub == null) return;

            for (i = 0; i < opcDaServer.Subscriptions.Count; i++)
            {
                item = opcDaServer.Subscriptions[i];
                if (item.Name == sub.Name)
                {
                    index = i;
                    break;
                }
            }

            if (item != null)
            {
                item.SetEnabled(false);
            }
            if (index != -1)
            {
                opcDaServer.Subscriptions.RemoveAt(index);
            }

            subscribed = false;
        
            log.Debug("{0}Подписка удалена.", channelLogPrefix);
        }

        private void sub_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            log.Trace("{0}sub_DataChanged({1}, {2}, {3})", channelLogPrefix, subscriptionHandle, requestHandle, values);

            if (values.Length > 0)
            {
                ProccessValues(values);
            }
        }

        public void GetCurrent()
        {
            Opc.Da.ItemValueResult[] prop_res = null;

            log.Trace("{0}GetCurrent()", channelLogPrefix);

            try
            {
                Connect();

                prop_res = GetTagValues();

                ProccessValues(prop_res);
            }
            catch (Exception ex)
            {
                opcDaServer = null;
                throw ex;
            }
            finally
            {
                if (!KeepConnected) Disconnect();
            }
        }

        private void ProccessValues(Opc.Da.ItemValueResult[] prop_res)
        {
            log.Trace("{0}ProccessValues({1})", channelLogPrefix, prop_res);

            Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
            List<ParamValueItem> valuesList = new List<ParamValueItem>();
            int valuesCount = 0;
            
            for (int i = 0; i < prop_res.Length; i++)
            {
                if (prop_res[i].ResultID != ResultID.S_OK)
                {
                    //res[i] = null;
                    continue;
                }

                ParameterItem parameter = null;
                for (int j = 0; j < parameters.Count; j++)
                {
                    if (parameters[j][CommonProperty.ParameterCodeProperty] == prop_res[i].ItemName)
                    {
                        parameter = parameters[j];
                        break;
                    }
                }

                if (parameter != null)
                {
                    if (!valuesDictionary.TryGetValue(parameter, out valuesList))
                    {
                        valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                    }

                    valuesList.Add(new ParamValueItem(
                        prop_res[i].Timestamp,
                        prop_res[i].Quality.GetCode() > 0 ? COTES.ISTOK.Quality.Good : COTES.ISTOK.Quality.Bad,
                        System.Convert.ToDouble(prop_res[i].Value)));
                }
            }
            foreach (var parameter in valuesDictionary.Keys)
            {
                DataListener.NotifyValues(null, parameter, valuesDictionary[parameter]);

                if (log.IsDebugEnabled)
                {
                    valuesCount += valuesDictionary[parameter].Count;
                }
            }

            log.Debug("{0}Получено данных: {1}. Принято: {2}", channelLogPrefix, prop_res.Length, valuesCount);
        }
        
        /// <summary>
        /// GetTagValues
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TrendNotFoundException"></exception>
        public ItemValueResult[] GetTagValues()//DateTime nTIStart, DateTime nTIEnd)
        {
            log.Trace("{0}GetTagValues()", channelLogPrefix);

            ItemValueResult[] results;

            if (opcDaServer == null) throw new ServerIsAbsentException("OpcDaServer");

            filter = new BrowseFilters();

            filter.ReturnAllProperties = true;
            filter.ReturnPropertyValues = true;

            results = opcDaServer.Read(arrParameters);

            return results;
        }

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            throw new NotSupportedException();
        }

        public void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            throw new NotSupportedException();
        }

        public void GetArchive()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
