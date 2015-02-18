using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;

namespace COTES.ISTOK.DiagnosticsInfo
{
    /// <summary>
    /// Состояние канала
    /// </summary>
    [Flags]
    public enum ChannelStatus
    {
        /// <summary>
        /// Канал не загружен
        /// </summary>
        Unloaded = 0x0,
        /// <summary>
        /// Канал запущен
        /// </summary>
        Started = 0x1,
        /// <summary>
        /// Канал заюлокирован
        /// </summary>
        Blocked = 0x2,
        /// <summary>
        /// Канал записывает данные в БД
        /// </summary>
        Storable = 0x4,
        /// <summary>
        /// В канале произошла ошибка
        /// </summary>
        HasErrors = 0x8
    }

    public interface ISummaryInfo
    {
        System.Data.DataSet GetSummaryInfo();
        string GetSummaryCaption();
    }

    [DataContract]
    public class GDiagnostics : Diagnostics
    {
        //
    }

    [DataContract]
    [KnownType(typeof(DiagnosticsProxy))]
    [KnownType(typeof(GlobalDiag))] 
    public class Diagnostics : MarshalByRefObject ,ITestConnection<Object>, COTES.ISTOK.DiagnosticsInfo.IDiagnostics
    {
        /// <summary>
        /// Список интерфейсов собирателей диагностической информации
        /// </summary>
        [DataMember]
        protected List<ISummaryInfo> lstInfoGetters = new List<ISummaryInfo>();

        /// <summary>
        /// Получить/установить название диагностируемого узла
        /// </summary>
        [DataMember]
        public virtual String Text { get; set; }        
        
		
        #region InfoGetters
        public void AddInfoGetter(ISummaryInfo infoGetter)
        {
            if (infoGetter == null) throw new ArgumentNullException("infoGetter");

            lstInfoGetters.Add(infoGetter);
        }
        /// <summary>
        /// Настройка собирателей информации
        /// </summary>
        protected virtual List<ISummaryInfo> SetupInfoGetters()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Получить свойства диагностируемого узла
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelfProperties()
        {
            return null;
        }

        /// <summary>
        /// Перезагрузить службу
        /// </summary>
        /// <param name="reason">Причина перезагрузки</param>
        /// <param name="delay"></param>
        public virtual void Restart(String reason, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вернуть сводную диагностическую информацию
        /// </summary>
        /// <returns></returns>
        public virtual DataSet GetAllInfo()
        {
            DataSet ds = new DataSet();
            string cap = "";

            List<ISummaryInfo> lstInfoGetters = SetupInfoGetters();
            foreach (var item in lstInfoGetters)
            {
                if (item != null)
                {
                    try
                    {
                        cap = item.GetSummaryCaption();
                        AddFromDataSet(ds, item.GetSummaryInfo());
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrEmpty(cap)) AddStubTable(ds, cap, ex.Message);
                    }
                }
            }
            return ds;
        }

        private void AddStubTable(DataSet dest, string tablename, string text)
        {
            try
            {
                DataTable table = new DataTable(tablename);
                table.Columns.Add();
                table.Rows.Add(text);
                dest.Tables.Add(table);
            }
            catch { }
        }
        private void AddFromDataSet(DataSet dest, DataSet source)
        {
            DataTable table;
            while (source.Tables.Count > 0)
            {
                table = source.Tables[0];
                source.Tables.Remove(table);
                table.TableName = string.Format(@"{0}", table.TableName);
                dest.Tables.Add(table);
            }
        }

        #region Redundancy
        /// <summary>
        /// Возвращает поддерживает ли данный объект дублирование
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRedundancySupported()
        {
            return false;
        }
        /// <summary>
        /// Возвращает информацию о состоянии дублирования
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetRedundancyInfo()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Block Manage
        /// <summary>
        /// Возвращает может ли данный объект управлять блочными службами
        /// </summary>
        /// <returns></returns>
        public virtual bool CanManageBlocks()
        {
            return false;
        }

        /// <summary>
        /// Получить список ИД блочных серверов
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetBlocks()
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Получить свойства блока
        /// </summary>
        /// <param name="block_id"></param>
        /// <returns></returns>
        public virtual string GetBlockProperties(int block_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить объект для диагностики блочного
        /// </summary>
        /// <returns></returns>
        public virtual Diagnostics[] GetBlockDiagnostics()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить URL Объекта диагностики
        /// </summary>
        /// <param name="block_id">ИД блочного</param>
        /// <returns></returns>
        public virtual string GetBlockDiagnosticsURL(int block_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить объект для диагностики блочного
        /// </summary>
        /// <param name="block_id">ИД блочного</param>
        /// <returns></returns>
        public virtual Diagnostics GetBlockDiagnostics(int block_id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Channel Manage
        /// <summary>
        /// Возвращает может ли данный объект управлять каналами
        /// </summary>
        /// <returns></returns>
        public virtual bool CanManageChannels()
        {
            return false;
        }
        /// <summary>
        /// Получить информацию о всех каналах
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetChannelInfo()
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Получить информацию о канале
        ///// </summary>
        ///// <param name="channelInfo">ИД канала</param>
        ///// <returns></returns>
        //public virtual DataTable GetChannelInfo(ChannelInfo channelInfo)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Получить свойства канала
        ///// </summary>
        ///// <param name="id">ИД канала</param>
        ///// <returns></returns>
        //public virtual IDictionary GetItemProperty(int id)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Получить состояние канала
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        /// <returns></returns>
        public virtual ChannelStatus GetChannelState(ChannelInfo channelInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить список ИД каналов
        /// </summary>
        /// <returns></returns>
        public virtual ChannelInfo[] GetChannels()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Запустить канал
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        public virtual void StartChannel(ChannelInfo channelInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Остановить канал
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        public virtual void StopChannel(ChannelInfo channelInfo)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Загрузить канал
        ///// </summary>
        ///// <param name="channelInfo">ИД канала</param>
        //public virtual void LoadChannel(ChannelInfo channelInfo)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Выгрузить канал
        ///// </summary>
        ///// <param name="channelInfo">ИД канала</param>
        //public virtual void UnloadChannel(ChannelInfo channelInfo)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region Buffer Manage
        /// <summary>
        /// Возвращает, может ли данный объект управлять буфером сбора
        /// </summary>
        /// <returns></returns>
        public virtual bool CanManageBuffer()
        {
            return false;
        }
        /// <summary>
        /// Получить значения из буфера
        /// </summary>
        /// <param name="channelInfo">ИД канала</param>
        /// <returns></returns>
        public virtual List<ParamValueItemWithID> GetBufferValues(ChannelInfo channelInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Сбосить буфер в БД
        /// </summary>
        public virtual void FlushBuffer()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Parameter Transaction Manage
        /// <summary>
        /// Возвращает может ли данный объект управлять параметрами для мнемосхем
        /// </summary>
        /// <returns></returns>
        public virtual bool CanManageParameterTransaction()
        {
            return false;
        }

        /// <summary>
        /// Получить массив ИД транзакций параметров для мнемосхем
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetParametersTransactionsID()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить информацию о всех транзакциях
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetParameterTransactionInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить информацию о транзакции
        /// </summary>
        /// <param name="transaction_id">ИД транзакции</param>
        /// <returns></returns>
        public virtual DataTable GetParameterTransactionInfo(int transaction_id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получить текущие значения параметров
        /// </summary>
        /// <param name="transaction_id">ИД транзакции</param>
        /// <returns></returns>
        public virtual ParamValueItemWithID[] GetParameterTransactionValues(int transaction_id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Calc Transaction Manage
        /// <summary>
        /// Возвращает может ли данный объект управлять запущенными пользователем расчетами
        /// </summary>
        /// <returns></returns>
        public virtual bool CanManageCalcTransaction()
        {
            return false;
        }

        /// <summary>
        /// Получить массив ИД транзакций запущенных пользователем расчетов
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetCalcTransactions()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ITestConnection Members

        public bool Test(Object obj)
        {
            return true;
        }

        #endregion
    }

    [DataContract]
    public class DiagnosticsProxy : Diagnostics
    {
    	Diagnostics diag;
    	
        public DiagnosticsProxy(Diagnostics _diag)
        {
            diag = _diag;
        }

        public override bool CanManageBlocks()
        {
            return diag.CanManageBlocks();
        }
        public override bool CanManageBuffer()
        {
            return diag.CanManageBuffer();
        }
        public override bool CanManageCalcTransaction()
        {
            return diag.CanManageCalcTransaction();
        }
        public override bool CanManageChannels()
        {
            return diag.CanManageChannels();
        }
        public override bool CanManageParameterTransaction()
        {
            return diag.CanManageParameterTransaction();
        }
        public override void FlushBuffer()
        {
            diag.FlushBuffer();
        }
        public override DataSet GetAllInfo()
        {
            return diag.GetAllInfo();
        }
        public override Diagnostics[] GetBlockDiagnostics()
        {
            return diag.GetBlockDiagnostics();
        }
        public override Diagnostics GetBlockDiagnostics(int block_id)
        {
            return diag.GetBlockDiagnostics(block_id);
        }
        public override string GetBlockDiagnosticsURL(int block_id)
        {
            return diag.GetBlockDiagnosticsURL(block_id);
        }
        public override string GetBlockProperties(int block_id)
        {
            return diag.GetBlockProperties(block_id);
        }
        public override int[] GetBlocks()
        {
            return diag.GetBlocks();
        }
        public override List<ParamValueItemWithID> GetBufferValues(ChannelInfo channelInfo)
        {
            return diag.GetBufferValues(channelInfo);
        }
        public override int[] GetCalcTransactions()
        {
            return diag.GetCalcTransactions();
        }
        public override DataTable GetChannelInfo()
        {
            return diag.GetChannelInfo();
        }
        //public override DataTable GetChannelInfo(ChannelInfo channelInfo)
        //{
        //    return diag.GetChannelInfo(channelInfo);
        //}
        //public override IDictionary GetItemProperty(int id)
        //{
        //    return diag.GetItemProperty(id);
        //}
        public override ChannelInfo[] GetChannels()
        {
            return diag.GetChannels();
        }
        public override ChannelStatus GetChannelState(ChannelInfo channelInfo)
        {
            return diag.GetChannelState(channelInfo);
        }
        public override int[] GetParametersTransactionsID()
        {
            return diag.GetParametersTransactionsID();
        }
        public override DataTable GetParameterTransactionInfo()
        {
            return diag.GetParameterTransactionInfo();
        }
        public override DataTable GetParameterTransactionInfo(int transaction_id)
        {
            return diag.GetParameterTransactionInfo(transaction_id);
        }
        public override ParamValueItemWithID[] GetParameterTransactionValues(int transaction_id)
        {
            return diag.GetParameterTransactionValues(transaction_id);
        }
        public override DataTable GetRedundancyInfo()
        {
            return diag.GetRedundancyInfo();
        }
        public override string GetSelfProperties()
        {
            return diag.GetSelfProperties();
        }
        public override bool IsRedundancySupported()
        {
            return diag.IsRedundancySupported();
        }
        //public override void LoadChannel(ChannelInfo channelInfo)
        //{
        //    diag.LoadChannel(channelInfo);
        //}
        public override void Restart(string reason, TimeSpan delay)
        {
            diag.Restart(reason, delay);
        }
        public override void StartChannel(ChannelInfo channelInfo)
        {
            diag.StartChannel(channelInfo);
        }
        public override void StopChannel(ChannelInfo channelInfo)
        {
            diag.StopChannel(channelInfo);
        }       
        public override string Text
        {
            get
            {
            	return diag.Text;
            }
            set
            {
            	diag.Text = value;            	
            }
        }
        //public override void UnloadChannel(ChannelInfo channelInfo)
        //{
        //    diag.UnloadChannel(channelInfo);
        //}
    }
}
