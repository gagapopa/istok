using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using NLog;

namespace COTES.ISTOK.Modules.modTornado
{
    class TornadoDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;
        
        ChannelInfo channel;
        List<ParameterItem> parameters;

        private string m_host;
        private string m_databaseConf;
        private string m_databaseVal;
        private string m_username;
        private string m_password;
        TimeSpan offsetTime;

        private SqlConnection m_connection = null;
        private SqlCommand m_command = null;
        private string tagValuesTable;

        public bool KeepConnected { get; set; }

        private void Connect(String db)
        {
            string conString;

            log.Trace(channelLogPrefix + "Подключение к базе данных {0}/{1}.", m_host, db);

            Disconnect();

            conString = "Initial Catalog=" + db + ";";
            conString += "Data Source=" + m_host + ";";
            conString += "User id=" + m_username + ";";
            conString += "Password=" + m_password + ";";

            m_connection = new SqlConnection(conString);
            m_command = m_connection.CreateCommand();
            m_connection.Open();
        }

        private void Disconnect()
        {
            log.Trace(channelLogPrefix + "Отключение от базы данных.");

            if (m_connection != null)
            {
                if (m_connection.State != System.Data.ConnectionState.Closed)
                {
                    m_connection.Close();
                }
                m_connection = null;
            }
        }

        #region IDataLoader Members

        Dictionary<int, ParameterItem> parameterInnerId;
        Dictionary<ParameterItem, int> innerIdByParameter;

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала."); 
            
            m_host = channelInfo[TornadoDataLoaderFactory.HostProperty];
            m_databaseConf = channelInfo[TornadoDataLoaderFactory.DatabaseProperty];
            m_databaseVal = channelInfo[TornadoDataLoaderFactory.DatabaseValuesProperty];
            m_username = channelInfo[TornadoDataLoaderFactory.UsernameProperty];
            m_password = channelInfo[TornadoDataLoaderFactory.PasswordProperty];

            tagValuesTable = channelInfo[TornadoDataLoaderFactory.TagValuesTableProperty];

            offsetTime = TimeSpan.FromHours(double.Parse(channelInfo[TornadoDataLoaderFactory.AddTimeHourProperty]));

            parameters = new List<ParameterItem>(channelInfo.Parameters);
            channel = channelInfo;

            // получить ид параметров в исходной базе
            parameterInnerId = new Dictionary<int, ParameterItem>();
            innerIdByParameter = new Dictionary<ParameterItem, int>();
            Connect(m_databaseConf);
            foreach (var item in parameters)
            {
                m_command.CommandText = "select [id] from [AllObjects] where [Name]=@name";
                m_command.Parameters.Add(new SqlParameter("name", item.GetPropertyValue(CommonProperty.ParameterCodeProperty)));
                using (SqlDataReader reader = m_command.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        parameterInnerId[reader.GetInt32(0)] = item;
                        innerIdByParameter[item] = reader.GetInt32(0);
                    }
                }
            }

            log.Debug(channelLogPrefix + "Канал инициирован. Зарегистрировано {0} параметров.", parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> parameterList = new List<ParameterItem>();
            SqlDataReader reader = null;
            SqlCommand command;

            log.Trace(channelLogPrefix + "Запрос параметров.");

            Connect(m_databaseConf);
           
            try
            {

                command = m_connection.CreateCommand();

                command.CommandText = "select a.[name],a.[description],m.[short_name]" +
                                    " from [AllObjects] as a" +
                                    " left outer join [real_sig_cfg] as rs on rs.[id]=a.[id]" +
                                    " left outer join [measures] as m on m.[id]=rs.[mea_id]";

                reader = command.ExecuteReader();
                parameterList = new List<ParameterItem>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        ParameterItem p = new ParameterItem();
                        p[CommonProperty.ParameterCodeProperty] = reader[0].ToString();
                        if (!reader.IsDBNull(1))
                            p.Name = reader[1].ToString();
                        if (!reader.IsDBNull(2))
                            p.SetPropertyValue(Consts.ParameterUnit, reader[2].ToString());
                        parameterList.Add(p);
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (!KeepConnected) Disconnect();
            }

            log.Debug(channelLogPrefix + "Получен список из {0} параметров.", parameterList.Count);

            return parameterList.ToArray();
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

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            ParamValueItem value;
            SqlDataReader dataReader = null;
            int valuesCount = 0;

            log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);

            startTime = startTime.Add(-offsetTime);
            startTime = startTime.ToUniversalTime();
            endTime = endTime.Add(-offsetTime);
            endTime = endTime.ToUniversalTime();

            Connect(m_databaseVal);

            m_command.CommandText = "SELECT [id],[value],[last_changed_at] ";
            m_command.CommandText += " FROM " + tagValuesTable;
            m_command.CommandText += " WHERE (" + FormatTrend() + ")";
            m_command.CommandText += " AND [last_changed_at]>='" + startTime.ToString("MM.dd.yyyy HH:mm:ss") + "'";
            m_command.CommandText += " AND [last_changed_at]<='" + endTime.ToString("MM.dd.yyyy HH:mm:ss") + "'";

            m_command.CommandTimeout = 60000;

            try
            {
                dataReader = m_command.ExecuteReader();

                Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
                List<ParamValueItem> valuesList;

                while (dataReader.Read())
                {
                    if (dataReader.IsDBNull(0) ||
                        dataReader.IsDBNull(1) ||
                        dataReader.IsDBNull(2)) continue;

                    int id = dataReader.GetInt32(0);

                    ParameterItem parameter;
                    if (parameterInnerId.TryGetValue(id, out parameter))
                    {
                        if (!valuesDictionary.TryGetValue(parameter,out valuesList))
                        {
                            valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                        }

                        value = new ParamValueItem();

                        value.Value = (double)dataReader[1];

                        DateTime utcTime = (DateTime)dataReader[2];
                        utcTime = utcTime.Add(offsetTime);
                        value.Time = utcTime.ToLocalTime();

                        value.Quality = Quality.Good;

                        valuesList.Add(value);
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

                log.Debug(channelLogPrefix + "Получено архивных данных за период [{0}; {1}]: {2}", startTime, endTime, valuesCount);
            }           
            finally
            {
                if (dataReader != null) 
                    dataReader.Close();

                if (!KeepConnected) 
                    Disconnect();
            }
        }

        private string FormatTrend()
        {
            StringBuilder sb = new StringBuilder();
            int i;

            for (i = 0; i < parameters.Count; i++)
            {
                if (i > 0) sb.Append(" OR ");
                sb.Append(" [id]=" + innerIdByParameter[parameters[i]]);
            }

            return sb.ToString();
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
