using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using NLog;

namespace COTES.ISTOK.Modules.modVzlet
{
    public class VzletDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        ChannelInfo channel;
        List<ParameterItem> parameters;
        
        private ServerType m_host_type;
        private string m_host;
        private string m_database;
        private string m_username;
        private string m_password;
        private QueryType m_querytype;
        private string m_queryfunction;

        private DbConnection m_connection = null;

        public bool KeepConnected { get; set; }

        private void Connect()
        {
            DbConnectionStringBuilder builder;

            log.Trace(channelLogPrefix + "Подключение к базе данных {0} {1}/{2}.", m_host_type, m_host, m_database);

            switch (m_host_type)
            {
                case ServerType.MSJet:
                    builder = new OleDbConnectionStringBuilder();

                    ((OleDbConnectionStringBuilder)builder).DataSource = m_database;
                    ((OleDbConnectionStringBuilder)builder).Provider = "Microsoft.Jet.OLEDB.4.0";

                    m_connection = new OleDbConnection(builder.ConnectionString);
                    break;
                case ServerType.MSSQLOLE:
                    builder = new OleDbConnectionStringBuilder();

                    ((OleDbConnectionStringBuilder)builder).DataSource = m_host;
                    ((OleDbConnectionStringBuilder)builder).Provider = "sqloledb";
                    ((OleDbConnectionStringBuilder)builder).PersistSecurityInfo = false;
                    builder["Initial Catalog"] = m_database;
                    builder["User id"] = m_username;
                    builder["Password"] = m_password;

                    m_connection = new OleDbConnection(builder.ConnectionString);
                    break;
                case ServerType.MSSQL:
                    SqlConnectionStringBuilder sqlBuilder;

                    sqlBuilder = new SqlConnectionStringBuilder();
                    sqlBuilder.DataSource = m_host;
                    sqlBuilder.InitialCatalog = m_database;
                    sqlBuilder.UserID = m_username;
                    sqlBuilder.Password = m_password;

                    m_connection = new SqlConnection(sqlBuilder.ConnectionString);
                    break;
                default:
                    throw new NotImplementedException("Данный тип сервера (" + m_host_type + ") не поддерживается");
            }
            m_connection.Open();
        }

        private void Disconnect()
        {
            log.Trace(channelLogPrefix + "Отключение от базы данных {0} {1}/{2}.", m_host_type, m_host, m_database);

            if (m_connection != null && m_connection.State != ConnectionState.Closed)
                m_connection.Close();
        }

        #region IDataLoader Members

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала.");

            try
            {
                System.ComponentModel.TypeConverter hostTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(ServerType));
                m_host_type = (ServerType)hostTypeConverter.ConvertFromString(channelInfo[VzletDataLoaderFactory.HostTypeProperty]);
            }
            catch
            {
                m_host_type = ServerType.MSSQL;
            }

            m_host = channelInfo[VzletDataLoaderFactory.HostProperty] ?? "";
            m_database = channelInfo[VzletDataLoaderFactory.DatabaseProperty] ?? "";
            m_username = channelInfo[VzletDataLoaderFactory.UsernameProperty] ?? "";
            m_password = channelInfo[VzletDataLoaderFactory.PasswordProperty] ?? "";
            m_queryfunction = channelInfo[VzletDataLoaderFactory.QueryFunctionProperty] ?? "";
            
            var tmp = channelInfo[VzletDataLoaderFactory.QueryTypeProperty];
            if (tmp != null && tmp.Trim().ToLower() == "функция") m_querytype = QueryType.Function;
            else m_querytype = QueryType.Query;

            channel = channelInfo;
            parameters = new List<ParameterItem>(channelInfo.Parameters);

            log.Debug(channelLogPrefix + "Канал инициирован. Зарегистрировано {0} параметров.", parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> parameterList = new List<ParameterItem>();
            DbDataReader reader = null;

            log.Trace(channelLogPrefix + "Запрос параметров.");

            Connect();

            try
            {
                string query;

                query = "SELECT obj.[name] + '/' + col.[name] " +
                    "\nFROM [sys].[all_objects] obj, [sys].[all_columns] col " +
                    "\nWHERE obj.[name] like 'ТЭЦ%' and col.[object_id] = obj.[object_id] and col.[name] <> 'ДатаВремя'";

                DbCommand command = m_connection.CreateCommand();// new OleDbCommand(query, m_connection);
                command.CommandText = query;

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        String code = reader[0].ToString();
                        ParameterItem param = new ParameterItem();
                        param[CommonProperty.ParameterCodeProperty] = code;
                        param.Name = code;

                        parameterList.Add(param);
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (!KeepConnected)
                    Disconnect();
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
            DbDataReader dataReader = null;
            DbCommand m_command;
            String[] code;
            int valuesCount = 0;
            int rowCount = 0;

            log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);

            Connect();

            Dictionary<String, List<String>> columnDictionary = new Dictionary<String, List<String>>();
            Dictionary<String, ParameterItem> parameterByCodeMap = new Dictionary<String, ParameterItem>();

            for (int i = 0; i < parameters.Count; i++)
            {
                String parameterCode = parameters[i][CommonProperty.ParameterCodeProperty];

                if (!String.IsNullOrEmpty(parameterCode)
                    && (code = parameterCode.Split('/')).Length > 1)
                {
                    if (!parameterByCodeMap.ContainsKey(parameterCode))
                    {
                        parameterByCodeMap.Add(parameterCode, parameters[i]);
                        List<String> columnList;
                        if (!columnDictionary.TryGetValue(code[0], out columnList))
                            columnDictionary.Add(code[0], columnList = new List<String>());
                        if (!columnList.Contains(code[1])) columnList.Add(code[1]);
                    }
                }
            }
            Dictionary<String, String> queryDictionary = new Dictionary<String, String>();
            StringBuilder queryBuilder = new StringBuilder();

            log.Debug(channelLogPrefix + "Формирование ключей для запроса");

            string strfrom, strwhere;
            string tablename = null;
            ICollection<string> querycollection;
            if (m_querytype == QueryType.Function)
            {
                var lst = new List<string>();
                var date = startTime;
                while (date <= endTime)
                {
                    lst.Add(string.Format("'{0}'", date.ToString("yyyyMMdd")));
                    date = date.AddDays(1);
                }
                querycollection = lst;
            }
            else
                querycollection = columnDictionary.Keys;
            log.Debug(channelLogPrefix + "Формирование запроса");
            foreach (String item in querycollection)
            {
                if (m_querytype == QueryType.Function)
                {
                    strfrom = string.Format(" \nFROM {0} ", m_queryfunction);
                    strwhere = string.Format(" ({0}) ORDER BY [ДатаВремя] ", item);
                    //в columnDictionary.Keys должен быть только один ключ (общая таблица параметров)
                    tablename = columnDictionary.Keys.First();
                }
                else
                {
                    strfrom = string.Format(" \nFROM {0} ", item);
                    strwhere = " \nWHERE [ДатаВремя]>=@time1 and [ДатаВремя]<=@time2 ";
                    tablename = item;
                }   

                queryBuilder.Length = 0;
                queryBuilder.Append("SELECT [ДатаВремя]");
                foreach (String columnName in columnDictionary[tablename])
                    queryBuilder.AppendFormat(", {0}", columnName);

                queryBuilder.Append(strfrom);
                queryBuilder.Append(strwhere);
                var key = tablename;
                if (queryDictionary.ContainsKey(key)) key += item;
                queryDictionary.Add(key, queryBuilder.ToString());
            }
            try
            {
                m_command = m_connection.CreateCommand();
                m_command.CommandTimeout = 60000;

                Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
                List<ParamValueItem> valuesList = new List<ParamValueItem>();

                foreach (String key in queryDictionary.Keys)
                {
                    if (m_querytype == QueryType.Function) tablename = queryDictionary.Keys.First();
                    else tablename = key;
                    m_command.CommandText = queryDictionary[key]; 
                    switch (m_host_type)
                    {
                        case ServerType.MSSQLOLE:
                            (m_command as OleDbCommand).Parameters.Clear();
                            (m_command as OleDbCommand).Parameters.Add("@time1", OleDbType.DBTimeStamp).Value = startTime;
                            (m_command as OleDbCommand).Parameters.Add("@time2", OleDbType.DBTimeStamp).Value = endTime;
                            break;
                        case ServerType.MSSQL:
                            (m_command as SqlCommand).Parameters.Clear();
                            (m_command as SqlCommand).Parameters.Add("time1", SqlDbType.DateTime).Value = startTime;
                            (m_command as SqlCommand).Parameters.Add("time2", SqlDbType.DateTime).Value = endTime;
                            break;
                    }
                    log.Debug(channelLogPrefix + "Выполнение запроса для '{0}': \"{1}\". StartTime={{{2}}}; EndTime={{{3}}};", key, queryDictionary[key], startTime, endTime);
                    dataReader = m_command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        if (dataReader.IsDBNull(0) /*||
                            dataReader.IsDBNull(1) ||
                            dataReader.IsDBNull(2)*/) continue;

                        if (log.IsDebugEnabled)
                        {
                            ++rowCount;
                        }

                        String columnName;
                        DateTime time;
                        if (dataReader[0] is DateTime) time = (DateTime)dataReader[0];
                        else time = DateTime.ParseExact(dataReader[0].ToString(), "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        bool allNULL = true;
                        for (int i = 1; allNULL && i < dataReader.FieldCount; i++)
                        {
                            allNULL = dataReader.IsDBNull(i);
                        }

                        if (log.IsDebugEnabled && allNULL)
                        {
                            log.Debug(channelLogPrefix + "All bad row for time '{0}'", time);
                        }

                        for (int i = 1; !allNULL && i < dataReader.FieldCount; i++)
                        {
                            columnName = dataReader.GetName(i);
                            ParameterItem parameter;
                            String parameterCode = String.Format("{0}/{1}", tablename, columnName);

                            if (parameterByCodeMap.TryGetValue(parameterCode, out parameter))
                            {
                                if (!valuesDictionary.TryGetValue(parameter, out valuesList))
                                {
                                    valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                                }

                                ParamValueItem value = new ParamValueItem();

                                if (dataReader[columnName] is double
                                    || dataReader[columnName] is float)
                                {
                                    value.Value = (double)dataReader[columnName];
                                    value.Quality = Quality.Good;//?
                                }
                                else if (dataReader[columnName] is String)
                                {
                                    double val;
                                    if (double.TryParse(dataReader[columnName].ToString(),
                                                        System.Globalization.NumberStyles.Float,
                                                        System.Globalization.NumberFormatInfo.InvariantInfo,
                                                        out val)
                                        || double.TryParse(dataReader[columnName].ToString(), out val))
                                    {
                                        value.Value = val;
                                        value.Quality = Quality.Good;//?
                                    }
                                    else
                                    {
                                        log.Debug(channelLogPrefix + "Unparsable string received '{0}'", dataReader[columnName]);
                                        value.Quality = Quality.Bad;
                                    }
                                }
                                else
                                {
                                    value.Quality = Quality.Bad;
                                    if (!dataReader.IsDBNull(i))
                                    {
                                        try
                                        {
                                            value.Value = Convert.ToDouble(dataReader[columnName]);
                                            value.Quality = Quality.Good;
                                        }
                                        catch (FormatException) { }
                                        catch (InvalidCastException) { }
                                    }
                                    if (value.Quality == Quality.Bad)
                                    {
                                        log.Debug(channelLogPrefix + "Bad value received '{0}' ({1})", dataReader[columnName], dataReader[columnName].GetType());

                                    }
                                }
                                value.Time = time;

                                valuesList.Add(value);
                            }
                        }
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                }

                foreach (var parameter in valuesDictionary.Keys)
                {
                    DataListener.NotifyValues(null, parameter, valuesDictionary[parameter]);

                    if (log.IsDebugEnabled)
                    {
                        valuesCount += valuesDictionary[parameter].Count;
                    }
                }

                log.Debug(channelLogPrefix + "Обработано строк за запрос: {0}", rowCount);
                log.Debug(channelLogPrefix + "Получено архивных данных за период [{0}; {1}]: {2}", startTime, endTime, valuesCount);
            }
            finally
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
                if (!KeepConnected)
                {
                    Disconnect();
                }
            }
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

        enum QueryType
        {
            Query,
            Function
        }
    }
}
