using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using COTES.ISTOK;
using System.Runtime.Remoting.Lifetime;
using System.Security;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Описание
    /// </summary>
    public abstract class DALManager
    {
        //protected ILogger messageLog = null;

        public DALManager()
        {
            Connected = false;
        }
        //public DALManager()
        //    : this()
        //{
        //    this.messageLog = messageLog;
        //}

        #region Public Properties
        /// <summary>
        /// Показывает, есть ли соединение
        /// </summary>
        public bool Connected { get; protected set; }
        /// <summary>
        /// Свойства, необходимые для поключения
        /// </summary>
        public Dictionary<string, string> ConnectionProperties { get; set; }
        #endregion

        #region Connection Methods
        /// <summary>
        /// Установить соединение
        /// </summary>
        public virtual void Connect()
        {
            Connected = true;
        }
        /// <summary>
        /// Разорвать соединение
        /// </summary>
        public virtual void Disconnect()
        {
            Connected = false;
        }
        /// <summary>
        /// Получить строку подключения
        /// </summary>
        /// <returns>Строка подключения</returns>
        internal virtual SecureString GetConnectionString()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region LoadInfo Methods
        /// <summary>
        /// Получает информацию об имеющихся в базе каналах
        /// </summary>
        /// <returns>Таблица с информацией</returns>
        public virtual IEnumerable<ChannelInfo> GetChannels(IEnumerable<ModuleInfo> modules)
        {
            throw new NotImplementedException();
        }
        ///// <summary>
        ///// Получает информацию об указанном канале
        ///// </summary>
        ///// <param name="id">Номер канала</param>
        ///// <returns>Таблица с информацией</returns>
        //public virtual DataTable GetChannel(int id)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Получает информацию о свойствах элемента
        /// </summary>
        /// <param name="id">Номер элемента</param>
        /// <returns>Таблица с информацией</returns>
        public virtual DataTable GetProperties(int id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получает информацию о параметрах канала
        /// </summary>
        /// <param name="id">Номер канала</param>
        /// <returns>Таблица с информацией</returns>
        public virtual DataTable GetParameters(int id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получает список расписаний.
        /// </summary>
        /// <returns>Таблица с информацией</returns>
        public virtual DataTable GetSchedules()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получает информацию о расписании.
        /// </summary>
        /// <param name="id">Номер расписания</param>
        /// <returns>Талица с информацией</returns>
        public virtual DataTable GetSchedule(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region UploadInfo
        /// <summary>
        /// Загружает справочную информацию в БД
        /// </summary>
        /// <param name="input">Сериализованная информация</param>
        public virtual void UploadInfo(byte[] input, bool cleardata)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Packages
        /// <summary>
        /// Получает информацию о пакете данных параметра за указанное время
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <param name="param_time">Время параметра</param>
        /// <returns>Таблица с информацией</returns>
        /// <remarks>Структура DataTable: <br />
        /// id - ид параметра;<br />
        /// time1 - начальное время пачки;<br />
        /// time2 - конечное время пачки;<br />
        /// data - содержимое пачки.
        /// </remarks>
        public virtual DataTable LoadPackage(int param_id, DateTime param_time)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает информацию о пакете данных параметра, предыдущим от указанного времени
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <param name="time">Время параметра</param>
        /// <returns></returns>
        public virtual DataTable LoadPrevPackage(int param_id, DateTime time)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает информацию о пакете данных параметра, следующим за указанным временем
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <param name="time">Время параметра</param>
        /// <returns></returns>
        public virtual DataTable LoadNextPackage(int param_id, DateTime time)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает информацию о пакете данных параметра за указанное период времени
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <param name="param_begin_time">Начальное время запрашиваемого периода</param>
        /// <param name="param_end_time">Конечное время запрашиваемого периода</param>
        /// <param name="maxvalues">Максимальное количество запрашиваемых пачек</param>
        /// <returns></returns>
        public virtual DataTable LoadPackage(int param_id, DateTime param_begin_time, DateTime param_end_time, int maxvalues)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Сохранить пакеты
        /// </summary>
        /// <param name="packages"></param>
        public virtual void SavePackage(DataTable packages)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Удалить пакет из БД
        /// </summary>
        /// <param name="id">Номер параметра</param>
        /// <param name="param_time">Время параметра</param>
        public virtual void RemovePackage(int id, DateTime param_time)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создать DataTable со структурой для раблты с пакетами
        /// </summary>
        /// <returns></returns>
        public virtual DataTable CreatePackageDataTable()
        {
            DataTable packageDataTable = new DataTable();

            packageDataTable.Columns.Add("id", typeof(int));
            packageDataTable.Columns.Add("tim1", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;
            packageDataTable.Columns.Add("tim2", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;
            packageDataTable.Columns.Add("data", typeof(byte[]));

            return packageDataTable;
        }
        #endregion

        public virtual void RemoveChannel(int channelId)
        {
            throw new NotImplementedException();
        }

        #region Обслуживание базы
        /// <summary>
        /// Удалить значения, параметры которых отсутсвуют в базе
        /// </summary>
        /// <param name="count">Максимальное количество удаляемых пачек</param>
        /// <returns>Количество удаленных пачек</returns>
        public virtual int CleanExcessValues(int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Удалить устаревшие значения
        /// </summary>
        /// <param name="count">Максимальное количество удаляемых пачек</param>
        /// <param name="param_id">ИД параметра</param>
        /// <param name="store_days">Количество дней, по прошествию которых, значения считаются устаревшими</param>
        /// <returns>Количество удаленных пачек</returns>
        public virtual int CleanOldValues(int count, int param_id, int store_days)
        {
            throw new NotImplementedException();
        }

        public virtual int CleanBadPackages(int count, int param_id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Работа с репликацией
        public virtual void ResetIdentities()
        {
        }
        public virtual COTES.ISTOK.Block.Redundancy.PublicationInfo CreateReplicationPublication(string publicationName)
        {
            throw new NotImplementedException();
        }
        public virtual void DeleteReplicationPublication(string publicationName)
        {
            throw new NotImplementedException();
        }
        public virtual COTES.ISTOK.Block.Redundancy.SubscriptionInfo CreateReplicationSubscription(COTES.ISTOK.Block.Redundancy.PublicationInfo repl,
            SecureString publisherConnString)
        {
            throw new NotImplementedException();
        }
        //public virtual void DeleteReplicationSubscription(COTES.ISTOK.Block.Redundancy.SubscriptionInfo repl)
        public virtual void DeleteReplicationSubscription(COTES.ISTOK.Block.Redundancy.PublicationInfo repl)
        {
            throw new NotImplementedException();
        }

        public virtual void CreateMirroring(string connStrPrincipal)
        {
            throw new NotImplementedException();
        }
        public virtual void SwitchMirroring(string connStrPrincipal)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Получить список допустимых серверов баз данных
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetDataSources()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить список допустимых Баз Данных на указанном сервере
        /// </summary>
        /// <returns></returns>
        public virtual String[] GetBases()//String login, String password, String host)
        {
            throw new NotImplementedException();
        }

        public virtual void CheckConnection()
        {
            throw new NotImplementedException();
        }
    }
}
