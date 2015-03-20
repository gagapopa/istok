using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using COTES.ISTOK;
using System.Collections;
using COTES.ISTOK.Block.Redundancy;
using COTES.ISTOK.Block.Telnet;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс для диагностинки и мониторинга каналов на блочном сервере
    /// </summary>
    
    [System.ServiceModel.ServiceBehavior(InstanceContextMode = System.ServiceModel.InstanceContextMode.Single, 
                                         IncludeExceptionDetailInFaults = true)]
    public class BlockDiagnostics : Diagnostics
    {
        private ValueBuffer valueBuffer;
        private ChannelManager chanManager = null;
        private RedundancyServer redunServer = null;

        internal BlockDiagnostics(ChannelManager manager, ValueBuffer valueBuffer, String blockName)
        {
            chanManager = manager;
            this.valueBuffer = valueBuffer;
            Text = blockName;
        }
        internal BlockDiagnostics(ChannelManager manager, ValueBuffer valueBuffer, String blockName, RedundancyServer rServer)
            : this(manager, valueBuffer, blockName)
        {
            redunServer = rServer;
        }
        
        public override bool CanManageChannels()
        {
            return true;
        }
        /// <summary>
        /// Получить список ИД каналов
        /// </summary>
        /// <returns></returns>
        public override ChannelInfo[] GetChannels()
        {
            return chanManager.GetLoadedChannels();
        }

        //internal IDictionary Properties { get; set; }

        #region Общая диагностическая инфа
        /// <summary>
        /// Настройка собирателей информации
        /// </summary>
        protected override List<ISummaryInfo> SetupInfoGetters()
        {
            List<ISummaryInfo> lstInfoGetters = new List<ISummaryInfo>();
            //if (lstInfoGetters == null)
            //    lstInfoGetters = new List<ISummaryInfo>();
            //else
            //    lstInfoGetters.Clear();
            lstInfoGetters.Add(chanManager);
            lstInfoGetters.Add(NSI.parRegistrator);
            lstInfoGetters.Add(redunServer);
            //if (Properties != null)
            //{
            //    string prop = "Diagnostics/Telnet";
            //    if (Properties.Contains(prop))
            if (!String.IsNullOrEmpty(BlockSettings.Instance.TelnetHost))
                lstInfoGetters.Add(new MoxaTelnet(BlockSettings.Instance.TelnetHost));////Properties[prop].ToString()));
            //}
            lstInfoGetters.Add(new BatteryDiagnostician());
            lstInfoGetters.Add(new SystemDiagnostician());

            return lstInfoGetters;
        }
        #endregion

        #region Дублирование
        /// <summary>
        /// Возвращает поддерживает ли данный объект дублирование
        /// </summary>
        /// <returns></returns>
        public override bool IsRedundancySupported()
        {
            return redunServer != null;
        }
        /// <summary>
        /// Возвращает информацию о состоянии дублирования
        /// </summary>
        /// <returns></returns>
        public override DataTable GetRedundancyInfo()
        {
            DataTable table;
            DataRow row;

            if (!IsRedundancySupported()) throw new NotSupportedException();

            table = new DataTable("RedundancyInfo");
            table.Columns.Add("Url");
            table.Columns.Add("State");
            table.Columns.Add("UID");
            table.Columns.Add("IsMaster");
            table.Columns.Add("Priority");
            table.Columns.Add("Version");

            foreach (var item in redunServer.GetServers())
            {
                row = table.NewRow();
                row["Url"] = item.URL;
                row["State"] = item.State.ToString();
                row["UID"] = item.UID;
                row["IsMaster"] = item.IsMaster.ToString();
                row["Priority"] = item.Priority.ToString();
                row["Version"] = item.Version;
                table.Rows.Add(row);
            }

            return table;
        }
        #endregion

        #region Управление каналами
        /// <summary>
        /// Запустить канал
        /// </summary>
        /// <param name="channel_id">ИД канала</param>
        public override void StartChannel(ChannelInfo channel_id)
        {
            chanManager.StartChannel(channel_id, true);
        }

        /// <summary>
        /// Остановить канал
        /// </summary>
        /// <param name="channel_id">ИД канала</param>
        public override void StopChannel(ChannelInfo channel_id)
        {
            chanManager.StopChannel(channel_id);
        }

        ///// <summary>
        ///// Загрузить канал
        ///// </summary>
        ///// <param name="channel_id">ИД канала</param>
        //public override void LoadChannel(ChannelInfo channel_id)
        //{
        //    chanManager.LoadChannel(channel_id);
        //}

        ///// <summary>
        ///// Выгрузить канал
        ///// </summary>
        ///// <param name="channel_id">ИД канала</param>
        //public override void UnloadChannel(ChannelInfo channel_id)
        //{
        //    chanManager.UnloadChannel(channel_id);
        //}
        #endregion

        #region Получение информации про каналы
        /// <summary>
        /// Получить информацию о всех каналах
        /// </summary>
        /// <returns></returns>
        public override DataTable GetChannelInfo()
        {
            return chanManager.GetSummaryInfo().Tables[0];
        }

        ///// <summary>
        ///// Получить информацию о канале
        ///// </summary>
        ///// <param name="channelInfo">ИД канала</param>
        ///// <returns></returns>
        //public override DataTable GetChannelInfo(ChannelInfo channelInfo)
        //{
        //    return null;
        //}

        ///// <summary>
        ///// Получить свойства канала
        ///// </summary>
        ///// <param name="id">ИД канала</param>
        ///// <returns></returns>
        //public override IDictionary GetItemProperty(int id)
        //{
        //    return chanManager.GetItemProperty(id);
        //}

        /// <summary>
        /// Получить состояние канала
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        /// <returns></returns>
        public override ChannelStatus GetChannelState(ChannelInfo channelInfo)
        {
            return chanManager.GetChannelState(channelInfo);
        }
        #endregion

        #region Работа с буфером
        public override bool CanManageBuffer()
        {
            return true;
        }
        /// <summary>
        /// Получить значения из буфера
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        /// <returns></returns>
        public override List<ParamValueItemWithID> GetBufferValues(ChannelInfo channelInfo)
        {
            List<ParamValueItemWithID> values = new List<ParamValueItemWithID>();
            ParamValueItemWithID withIdValue;

            //var parameters = chanManager.GetParameters(channelInfo);

            foreach (var parameter in channelInfo.Parameters)
            {
                Package pack = valueBuffer.GetPackage(parameter.Idnum);
                if (pack != null)
                    foreach (ParamValueItem valueItem in pack.Values)
                        if ((withIdValue = valueItem as ParamValueItemWithID) != null)
                            values.Add(withIdValue);
                        else
                            values.Add(new ParamValueItemWithID(valueItem, pack.Id));
            }
            return values;
        }

        /// <summary>
        /// Сбосить буфер в БД
        /// </summary>
        public override void FlushBuffer()
        {
            valueBuffer.FlushAll();
            //chanManager.FlushBuffer();
        }
        #endregion

        #region Parameter Transaction Manage
        public override bool CanManageParameterTransaction()
        {
            return true;
        }

        public override int[] GetParametersTransactionsID()
        {
            return NSI.parRegistrator.GetTransactionsID();
        }

        public override DataTable GetParameterTransactionInfo()
        {
            DataTable table = null;
            ParametersTransaction transaction = null;
            int[] ids = null;

            ids = NSI.parRegistrator.GetTransactionsID();

            if (ids != null && ids.Length > 0)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    try
                    {
                        transaction = NSI.parRegistrator.GetTransaction(ids[i]);
                    }
                    catch { }

                    table = FillParameterTransactionTable(table, transaction);
                }
            }
            return table;
        }

        public override DataTable GetParameterTransactionInfo(int transaction_id)
        {
            ParametersTransaction transaction = null;

            try
            {
                transaction = NSI.parRegistrator.GetTransaction(transaction_id);
            }
            catch { }

            return FillParameterTransactionTable(null, transaction);
        }

        private DataTable FillParameterTransactionTable(DataTable dest, ParametersTransaction transaction)
        {
            ParamValueItemWithID[] parameters;
            DataTable table;

            if (dest == null)
                table = new DataTable("Обновляемые параметры");
            else
                table = dest;

            if (transaction != null)
            {
                if (!table.Columns.Contains("Номер"))
                    table.Columns.Add("Номер", typeof(int));
                if (!table.Columns.Contains("Количество"))
                    table.Columns.Add("Количество", typeof(int));
                if (!table.Columns.Contains("Последняя активность"))
                    table.Columns.Add("Последняя активность", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;

                DataRow row = table.NewRow();
                row["Номер"] = transaction.ID;
                row["Количество"] = ((parameters = transaction.GetParameters()) != null ? parameters.Length : 0);
                row["Последняя активность"] = transaction.LastCallTime;
                table.Rows.Add(row);
            }

            return table;
        }

        public override ParamValueItemWithID[] GetParameterTransactionValues(int transaction_id)
        {
            ParametersTransaction transaction = null;
            ParamValueItemWithID[] parameters;

            try
            {
                transaction = NSI.parRegistrator.GetTransaction(transaction_id);
            }
            catch { }

            parameters = transaction.GetParameters();
            return parameters;
        }
        #endregion
    }
}
