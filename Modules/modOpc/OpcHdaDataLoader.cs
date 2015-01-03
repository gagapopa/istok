using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using Opc;
using Opc.Hda;

namespace COTES.ISTOK.Modules.modOpc
{
    public class OpcHdaDataLoader : OpcDataLoader, IDataLoader
    {
        Opc.Hda.Server opcHdaServer = null;
        ItemIdentifier[] items;

        private string m_Host;
        private string m_Server;
        private string m_TrendName;
        private string collectionType;

        ChannelInfo channel;

        public OpcHdaDataLoader()
        {
            log = LogManager.GetCurrentClassLogger();
            KeepConnected = true;
        }

        protected override void Connect()
        {
            Opc.Hda.Trend nTrend;

            if (opcHdaServer != null && opcHdaServer.IsConnected) return;

            log.Debug("{0}Подключение к OPC HDA Серверу \\\\{1}\\{2}", channelLogPrefix, m_Host, m_Server);
            opcHdaServer = (Opc.Hda.Server)FindServer(m_Host, m_Server);

            if (opcHdaServer != null)
            {
                opcHdaServer.Connect();
                nTrend = CreateTrend(m_TrendName);
                CreateItems(nTrend, items);
            }
        }

        protected override void Disconnect()
        {
            Opc.Hda.Trend trend;
            int i;

            log.Debug("{0}Отключение от OPC HDA сервера \\\\{1}\\{2}", channelLogPrefix, m_Host, m_Server);
            if (opcHdaServer != null && opcHdaServer.IsConnected)
            {
                if (!string.IsNullOrEmpty(m_TrendName))
                {
                    for (i = 0; i < opcHdaServer.Trends.Count; i++)
                    {
                        trend = opcHdaServer.Trends[i];
                        if (trend.Name == m_TrendName)
                        {
                            if (trend.SubscriptionActive) trend.SubscribeCancel();
                            opcHdaServer.Trends.Remove(trend);
                            break;
                        }
                    }
                }
                opcHdaServer.Disconnect();
            }
        }

        /// <summary>
        /// CreateTrend
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TrendIsAlreadyExistException"></exception>
        public Opc.Hda.Trend CreateTrend(string strName)
        {
            log.Trace("{0}CreateTrend({1})", channelLogPrefix, strName);

            Opc.Hda.Trend nTrend = null;

            if (opcHdaServer == null) throw new ServerIsAbsentException("opcHdaServer");
            if (strName == null) throw new ArgumentNullException("strName");

            foreach (Trend trend in opcHdaServer.Trends)
            {
                if (trend.Name == strName)
                {
                    opcHdaServer.Trends.Remove(trend);
                }
            }

            nTrend = new Opc.Hda.Trend(opcHdaServer);
            nTrend.Name = strName;

            return nTrend;
        }

        /// <summary>
        /// CreateItems
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void CreateItems(Opc.Hda.Trend nTrend, ItemIdentifier[] items)
        {
            log.Trace("{0}CreateItems({1}, {2})", channelLogPrefix, nTrend, items);

            IdentifiedResult[] temp = null;
            ItemIdentifier nItem;
            int i;

            if (opcHdaServer == null) throw new ServerIsAbsentException("opcHdaServer");
            if (nTrend == null) throw new ArgumentNullException("null:nTrend");
            if (items == null)
            {
                items = new ItemIdentifier[1];
                items[0] = new ItemIdentifier();
                //throw new ArgumentNullException("items");
            }

            try
            {
                temp = opcHdaServer.CreateItems(items);

                if (temp == null) throw new Exception("Cannot create items(HDA)");

                for (i = 0; i < items.Length; i++)
                {
                    nItem = new ItemIdentifier(items[i]);
                    nItem.ServerHandle = temp[i].ServerHandle;
                    nTrend.Items.Add(new Item(nItem));
                }

                opcHdaServer.Trends.Add(nTrend);
            }
            catch (ResultIDException ex)
            {
                throw ex;
            }
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

            parameters = new List<ParameterItem>(channelInfo.Parameters);
            channel = channelInfo;

            m_TrendName = Guid.NewGuid().ToString("N");
            items = new ItemIdentifier[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                items[i] = new ItemIdentifier(parameters[i][CommonProperty.ParameterCodeProperty]);
            }

            log.Debug("{0}Канал инициирован. Зарегистрировано {1} параметров.", channelLogPrefix, parameters.Count);
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

            BrowseElement[] children = null;
            BrowseElement parent = new BrowseElement();
            TreeNode tmpnode = null;//, tmpparent = null;
            TreeNode nNode = null;
            IBrowser browser = null;
            ResultID[] results = null;

            int i;

            if (opcHdaServer == null) throw new ServerIsAbsentException("OpcHdaServer");
            if (opcServers == null) throw new ServerIsAbsentException("OpcServers");
            if (nParent == null) throw new ArgumentNullException("nParent");

            if (browser == null) browser = opcHdaServer.CreateBrowser(null, out results);

            nNode = new TreeNode(null);

            try
            {
                if (nParent.Tag == null && nParent.Text == "")
                {
                    parent.ItemName = "";
                }
                else
                {
                    if (nParent.Tag != null) parent.ItemName = nParent.Tag.ToString();
                    else parent.ItemName = nParent.Text;
                }

                children = browser.Browse(parent);

                for (i = 0; i < children.Length; i++)
                {
                    tmpnode = new TreeNode(null);

                    tmpnode.Text = children[i].Name;
                    tmpnode.Tag = children[i].ItemName;

                    if (children[i].HasChildren)
                    {
                        tmpnode = GetTagNames(tmpnode);
                    }

                    nNode.Nodes.Add(tmpnode);
                }
            }
            catch (Exception ex)
            {
                throw new CannotReadTagsException(parent.ItemName + "(" + ex.Message + ")");
            }

            return nNode;
        }

        public IDataListener DataListener { get; set; }

        public DataLoadMethod LoadMethod
        {
            get { return DataLoadMethod.Archive; }
        }

        public void RegisterSubscribe()
        {
            throw new NotSupportedException();
        }

        public void UnregisterSubscribe()
        {
            throw new NotSupportedException();
        }

        public void GetCurrent()
        {
            throw new NotSupportedException();
        }

        private void ProccessValues(ItemValueCollection[] prop_res)
        {
            log.Trace("{0}ProccessValues({1})", channelLogPrefix, prop_res);

            Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
            List<ParamValueItem> valuesList = new List<ParamValueItem>();
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
                    for (int j = 0; j < prop_res[i].Count; j++)
                    {
                        if (!valuesDictionary.TryGetValue(parameter, out valuesList))
                        {
                            valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                        }

                        valuesList.Add(new ParamValueItem(
                            prop_res[i][j].Timestamp,
                            prop_res[i][j].Quality.GetCode() > 0 ? COTES.ISTOK.Quality.Good : COTES.ISTOK.Quality.Bad,
                            System.Convert.ToDouble(prop_res[i][j].Value))); 
                    }
                }
            }
            foreach (var parameter in valuesDictionary.Keys)
            {
                DataListener.NotifyValues(null, parameter, valuesDictionary[parameter]);
            }
        }

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            log.Trace("{0}GetArchive({1}, {2})", channelLogPrefix, startTime, endTime);

            ItemValueCollection[] results = null;

            try
            {
                Connect();

                results = GetTagValues(m_TrendName, startTime, endTime);

                if (log.IsDebugEnabled)
                {
                    int count = 0;
                    for (int i = 0; i < results.Length; i++)
                    {
                        count += results[i].Count;
                    }
                    log.Debug("{0}Получено архивных данных за период [{1}; {2}]: {3}", channelLogPrefix, startTime, endTime, count);
                }
                ProccessValues(results);

            }
            catch (Exception ex)
            {
                opcHdaServer = null;
                throw ex;
            }
            finally
            {
                if (!KeepConnected) Disconnect();
            }
        }

        /// <summary>
        /// GetTagValues
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TrendNotFoundException"></exception>
        public ItemValueCollection[] GetTagValues(string strTrendName, DateTime nTIStart, DateTime nTIEnd)
        {
            log.Trace("{0}GetTagValues({1}, {2}, {3})", channelLogPrefix, strTrendName, nTIStart, nTIEnd);

            Trend ptrTrend = null;
            ItemValueCollection[] results = null;

            if (opcHdaServer == null) throw new ServerIsAbsentException("OpcHdaServer");
            if (strTrendName == null) throw new ArgumentNullException("strTrendName");
            if (strTrendName == "") throw new ArgumentException("strTrendName");

            foreach (Trend trend in opcHdaServer.Trends)
            {
                if (trend.Name == strTrendName)
                {
                    ptrTrend = trend;
                    break;
                }
            }

            if (ptrTrend == null)
                throw new ArgumentException("TrendNotFound: " + strTrendName);

            ptrTrend.StartTime = new Time();
            ptrTrend.EndTime = new Time();
            ptrTrend.StartTime.AbsoluteTime = nTIStart;
            ptrTrend.EndTime.AbsoluteTime = nTIEnd;

            results = ptrTrend.ReadRaw();

            return results;
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
