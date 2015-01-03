using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using NLog;

namespace COTES.ISTOK.Modules.modAtec
{
    public class AtecDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        private ChannelInfo channel;
        private List<ParameterItem> parameters;

        private ServerType m_host_type;
        private string m_host;
        private string m_database;
        private string m_username;
        private string m_password;

        private DbConnection m_connection = null;

        public bool KeepConnected { get; set; }

        private void Connect()
        {
            DbConnectionStringBuilder builder;

            log.Trace(channelLogPrefix + "Подключение к базе данных {0} {1}/{2}.", m_host, m_database);

            switch (m_host_type)
            {
                case ServerType.MSJet:
                    builder = new OleDbConnectionStringBuilder();

                    ((OleDbConnectionStringBuilder)builder).DataSource = m_database;//"C:\\Temp\\Atec.mdb";
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
            log.Trace(channelLogPrefix + "Отключение от базы данных {0} {1}/{2}.", m_host, m_database);
         
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
                m_host_type = (ServerType)hostTypeConverter.ConvertFromString(channelInfo[AtecDataLoaderFactory.HostTypeProperty]);
            }
            catch
            {
                m_host_type = ServerType.MSSQL;
            }

            m_host = channelInfo[AtecDataLoaderFactory.HostProperty];
            m_database = channelInfo[AtecDataLoaderFactory.DatabaseProperty];
            m_username = channelInfo[AtecDataLoaderFactory.UsernameProperty];
            m_password = channelInfo[AtecDataLoaderFactory.PasswordProperty];

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

                query = "SELECT distinct Rasshivka.Nam_kan as Name";
                query += ", Metrans.nom_kan as Channel";
                query += ", TTyp.Typ_name as Type";
                query += ", TEI.NEI as Measure";
                query += ", Tobozn.Nobz as Label";
                query += ", Rasshivka.id as Id";
                query += ", Metrans.adr as Address";
                query += ", Max_val, Min_val, Max_lim, Min_lim";


                query += " FROM Metrans, Rasshivka, TTyp, TEI, Tobozn";

                query += " WHERE Rasshivka.id=Metrans.id_kan";
                query += " AND Rasshivka.Typ=TTyp.Typ";
                query += " AND TEI.KEI=Rasshivka.KEI";
                query += " AND Tobozn.Kobz=Rasshivka.Kobz";

                query += " ORDER BY Rasshivka.Nam_kan";

                DbCommand command = m_connection.CreateCommand();// new OleDbCommand(query, m_connection);
                command.CommandText = query;

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        String code;
                        if (!reader.IsDBNull(0))
                            code = String.Format("{0}:{1}", reader[6], reader[1]);
                        else code = String.Empty;
                        ParameterItem param = new ParameterItem();
                        param.Name = reader[0].ToString();
                        param[CommonProperty.ParameterCodeProperty] = code;
                        param.SetPropertyValue("ParameterType", reader[2].ToString());
                        param.SetPropertyValue(Consts.ParameterUnit, reader[3].ToString());
                        param.SetPropertyValue("ParameterLabel", reader[4].ToString());
                        param.SetPropertyValue(Consts.ParameterMaxValue, reader[7].ToString());
                        param.SetPropertyValue(Consts.ParameterMinValue, reader[8].ToString());
                        param.SetPropertyValue(Consts.ParameterMaxWarningLimit, reader[9].ToString());
                        param.SetPropertyValue(Consts.ParameterMinWarningLimit, reader[10].ToString());

                        parameterList.Add(param);
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
            DbDataReader dataReader = null;
            DbCommand m_command;
            String[] code;
            int valuesCount = 0;

            log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);

            Connect();

            string query;
            switch (m_host_type)
            {
                case ServerType.MSJet:
                    query = "SELECT * FROM Real_Data WHERE Format(Dtm, \"yyyy-mm-dd HH:MM:ss\")>=Format(#{0}#, \"yyyy-mm-dd HH:MM:ss\") and Format(Dtm, \"yyyy-mm-dd HH:MM:ss\")<=Format(#{1}#, \"yyyy-mm-dd HH:MM:ss\") and ({2}) ORDER BY Dtm";
                    break;
                case ServerType.MSSQLOLE:
                    query = "SELECT * FROM Real_Data WHERE Dtm>=? and Dtm<=? and ({2}) ORDER BY Dtm";
                    break;
                case ServerType.MSSQL:
                    query = "SELECT * FROM Real_Data WHERE Dtm>=@time1 and Dtm<=@time2 and ({2}) ORDER BY Dtm";
                    break;
                default:
                    throw new NotImplementedException();
            }
            string queryWhere = String.Empty, whereFormat = "Adr={0}";
            List<String> addressList = new List<String>();

            for (int i = 0; i < parameters.Count; i++)
            {
                String parameterCode = parameters[i][CommonProperty.ParameterCodeProperty];
                if (!String.IsNullOrEmpty(parameterCode)
                    && (code = parameterCode.Split(':')).Length > 1)
                {
                    if (!addressList.Contains(code[0]))
                    {
                        if (queryWhere.Length > 0) queryWhere += " or ";
                        queryWhere += String.Format(whereFormat, code[0]);
                        addressList.Add(code[0]);
                    }
                }
            }

            Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
            List<ParamValueItem> valuesList;

            try
            {
                m_command = m_connection.CreateCommand();
                m_command.CommandText = String.Format(System.Globalization.CultureInfo.InvariantCulture, query, startTime, endTime, queryWhere);
                m_command.CommandTimeout = 60000;
                switch (m_host_type)
                {
                    case ServerType.MSSQLOLE:
                        (m_command as OleDbCommand).Parameters.Add("@time1", OleDbType.DBTimeStamp).Value = startTime;
                        (m_command as OleDbCommand).Parameters.Add("@time2", OleDbType.DBTimeStamp).Value = endTime;
                        break;
                    case ServerType.MSSQL:
                        (m_command as SqlCommand).Parameters.Add("time1", SqlDbType.DateTime).Value = startTime;
                        (m_command as SqlCommand).Parameters.Add("time2", SqlDbType.DateTime).Value = endTime;
                        break;
                }

                dataReader = m_command.ExecuteReader();

                while (dataReader.Read())
                {
                    if (dataReader.IsDBNull(0) ||
                        dataReader.IsDBNull(1) ||
                        dataReader.IsDBNull(2)) continue;

                    String addr = dataReader[1].ToString(), columnName;
                    DateTime time = (DateTime)dataReader[0];

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        String parameterCode = parameters[i][CommonProperty.ParameterCodeProperty];
                        if (!String.IsNullOrEmpty(parameterCode) &&
                            (code = parameterCode.Split(':'))[0].Equals(addr) && code.Length > 1)
                        {
                            if (!valuesDictionary.TryGetValue(parameters[i], out valuesList))
                            {
                                valuesDictionary[parameters[i]] = valuesList = new List<ParamValueItem>();
                            }

                            columnName = "P" + code[1];
                            ParamValueItem value = new ParamValueItem();

                            value.Value = Convert.ToDouble(dataReader[columnName]);
                            value.Time = time;
                            value.Quality = Quality.Good;

                            valuesList.Add(value);
                        }
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
    }
}
