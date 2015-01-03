using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using NLog;

namespace COTES.ISTOK.Modules.modFb
{
    class FbDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        private FbConnection m_connection = null;
        private FbCommand m_command = null;

        private string m_host;
        private string m_database;
        private string m_username;
        private string m_password;
        private string m_tagValuesTable;
        private string m_tagValuesFieldPName;
        private string m_tagValuesFieldTName;
        private string m_tagValuesFieldRName;
        private string m_tagValuesFieldXName;
        private string m_tagValuesFieldTimeName;
        private string m_tagValuesFieldID;
        private string m_tagNamesFieldId;
        private string m_tagNamesFieldName;
        private string m_tagNamesTable;

        private ChannelInfo channel;
        private List<ParameterItem> parameters;

        //        private string m_host;
        //        private string m_database;
        //        private string m_username;
        //        private string m_password;
        //        private string m_tagValuesTable;
        //        private string m_tagValuesFieldPName;
        //        private string m_tagValuesFieldTName;
        //        private string m_tagValuesFieldRName;
        //        private string m_tagValuesFieldXName;
        //        private string m_tagValuesFieldTimeName;
        //        private string m_tagValuesFieldID;
        //        private string m_tagNamesFieldId;
        //        private string m_tagNamesFieldName;
        //        private string m_tagNamesTable;

        //        private float captureIntervalNormal;
        //        private float captureIntervalLarge;
        //        private float captureDifference;
        //        private float maxWaitInterval;
        //        private float captureIntervalPrevious;

        Dictionary<int, bool> stateDictionary;
        Dictionary<int, Quality> qualityDictionary;

        public bool KeepConnected { get; set; }

        private void Connect()
        {
            string conString;

            log.Trace(channelLogPrefix + "Подключение к базе данных {0}/{1}.", m_host, m_database);

            if (m_connection != null &&
                m_connection.State != System.Data.ConnectionState.Closed &&
                m_connection.State != System.Data.ConnectionState.Broken) return;

            //m_database = FindPropertyByName("Database");
            //m_host = FindPropertyByName("Host");
            //m_username = FindPropertyByName("Username");
            //m_password = FindPropertyByName("Password");

            conString = "Data Source=" + m_host + ";";
            conString += "initial catalog=" + m_database + ";";
            conString += "User id=" + m_username + ";";
            conString += "Password=" + m_password + ";";

            m_connection = new FbConnection(conString);
            //FbConnection.ClearAllPools();
            m_command = m_connection.CreateCommand();
            m_connection.Open();

            var transactionOpt = new FbTransactionOptions();
            transactionOpt.TransactionBehavior =
                FbTransactionBehavior.Read
                | FbTransactionBehavior.NoRecVersion
                | FbTransactionBehavior.ReadCommitted;

            m_command.Transaction = m_connection.BeginTransaction(transactionOpt);//(System.Data.IsolationLevel.ReadCommitted);
            m_command.CommandTimeout = 15 * 60;
        }
        private void Disconnect()
        {
            log.Trace(channelLogPrefix + "Отключение от базы данных {0}/{1}.", m_host, m_database);

            if (m_connection != null)
            {
                if (m_connection.State != System.Data.ConnectionState.Closed)
                {
                    //m_command.Transaction.Rollback();
                    m_command.Transaction.Dispose();
                    m_command.Dispose();
                    m_connection.Dispose();
                    //m_connection.Close();
                }
                m_connection = null;
            }
        }

        #region IDataLoader Members

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала.");
            
            m_host = channelInfo[FbDataLoaderFactory.HostProperty];
            m_database = channelInfo[FbDataLoaderFactory.DatabaseProperty];
            m_username = channelInfo[FbDataLoaderFactory.UsernameProperty];
            m_password = channelInfo[FbDataLoaderFactory.PasswordProperty];

            m_tagValuesTable = channelInfo[FbDataLoaderFactory.TagValuesTableProperty];
            m_tagValuesFieldPName = channelInfo[FbDataLoaderFactory.TagValuesFieldPNameProperty];
            m_tagValuesFieldTName = channelInfo[FbDataLoaderFactory.TagValuesFieldTNameProperty];
            m_tagValuesFieldRName = channelInfo[FbDataLoaderFactory.TagValuesFieldRNameProperty];
            m_tagValuesFieldXName = channelInfo[FbDataLoaderFactory.TagValuesFieldXNameProperty];
            m_tagValuesFieldTimeName = channelInfo[FbDataLoaderFactory.TagValuesFieldTimeNameProperty];
            m_tagValuesFieldID = channelInfo[FbDataLoaderFactory.TagValuesFieldIDProperty];

            m_tagNamesFieldId = channelInfo[FbDataLoaderFactory.TagNamesFieldIdProperty];
            m_tagNamesFieldName = channelInfo[FbDataLoaderFactory.TagNamesFieldNameProperty];
            m_tagNamesTable = channelInfo[FbDataLoaderFactory.TagNamesTableProperty];

            channel = channelInfo;
            parameters = new List<ParameterItem>(channelInfo.Parameters);

            stateDictionary = new Dictionary<int, bool>();
            qualityDictionary = new Dictionary<int, Quality>();
            foreach (var sendItem in parameters)
            {
                stateDictionary.Add(sendItem.Idnum, true);
                qualityDictionary.Add(sendItem.Idnum, Quality.Good);
            }

            log.Debug(channelLogPrefix + "Канал инициирован. Зарегистрировано {0} параметров.", parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> parameterList = new List<ParameterItem>();
            FbDataReader reader = null;
            FbConnection getParamsConnection;
            FbCommand command;

            log.Trace(channelLogPrefix + "Запрос параметров.");

            String format = "select distinct a.{1}, cast(a.{2} as varchar(128)) || '/{0}' FROM {3} a, {4} w WHERE w.{5}=a.{2} AND not (w.{6} is null);";

            if (m_connection == null)
            {
                Connect();
            }
            getParamsConnection = (FbConnection)((ICloneable)m_connection).Clone();
            getParamsConnection.Open();

            command = getParamsConnection.CreateCommand();
            parameterList = new List<ParameterItem>();

            try
            {
                ParameterItem send;
                command.CommandText = //FindPropertyByName("QueryTagPNames");
                    String.Format(format, "P", m_tagNamesFieldName, m_tagNamesFieldId, m_tagNamesTable, m_tagValuesTable, m_tagValuesFieldID, m_tagValuesFieldPName);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    send = new ParameterItem();
                    send.Name = reader[0].ToString() + " (P)";
                    send[CommonProperty.ParameterCodeProperty] = reader[1].ToString();
                    parameterList.Add(send);
                    //table.AddParameter(reader[1].ToString(),
                    //            "", "", "", reader[0].ToString() + " (P)", "");
                }
                command.CommandText = //FindPropertyByName("QueryTagTNames");
                String.Format(format, "T", m_tagNamesFieldName, m_tagNamesFieldId, m_tagNamesTable, m_tagValuesTable, m_tagValuesFieldID, m_tagValuesFieldTName);

                reader.Close();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    send = new ParameterItem();
                    send.Name = reader[0].ToString() + " (T)";
                    send[CommonProperty.ParameterCodeProperty] = reader[1].ToString();
                    parameterList.Add(send);

                    //table.AddParameter(reader[1].ToString(),
                    //            "", "", "", reader[0].ToString() + " (T)", "");
                }
                command.CommandText = //FindPropertyByName("QueryTagRNames");
                    String.Format(format, "R", m_tagNamesFieldName, m_tagNamesFieldId, m_tagNamesTable, m_tagValuesTable, m_tagValuesFieldID, m_tagValuesFieldRName);

                reader.Close();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    send = new ParameterItem();
                    send.Name = reader[0].ToString() + " (R)";
                    send[CommonProperty.ParameterCodeProperty] = reader[1].ToString();
                    parameterList.Add(send);

                    //table.AddParameter(reader[1].ToString(),
                    //            "", "", "", reader[0].ToString() + " (R)", "");
                }

                command.CommandText = //FindPropertyByName("QueryTagXNames");
                    String.Format(format, "X", m_tagNamesFieldName, m_tagNamesFieldId, m_tagNamesTable, m_tagValuesTable, m_tagValuesFieldID, m_tagValuesFieldXName);

                reader.Close();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    send = new ParameterItem();
                    send.Name = reader[0].ToString() + " (X)";
                    send[CommonProperty.ParameterCodeProperty] = reader[1].ToString();
                    parameterList.Add(send);
                    //table.AddParameter(reader[1].ToString(),
                    //            "", "", "", reader[0].ToString() + " (X)", "");
                }
            }
            finally
            {
                reader.Close();
                if (!KeepConnected) Disconnect();
                getParamsConnection.Dispose();
            }

            log.Debug(channelLogPrefix + "Получен список из {0} параметров.", parameterList.Count);
            
            return parameterList.ToArray();
        }

        public IDataListener DataListener { get; set; }

        public DataLoadMethod LoadMethod
        {
            get { return DataLoadMethod.ArchiveByParameter; }
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
            throw new NotSupportedException();
        }

        ArchiveRequestTransaction currentTransaction = new ArchiveRequestTransaction();

        public void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            log.Trace(channelLogPrefix + "Настройка запроса архивных данных по параметрам {0} [{1}; {2}].", parameter, startTime, endTime);

            currentTransaction.SetInterval(parameter, startTime, endTime);
        }

        public void GetArchive()
        {
            //List<ParamValueItemWithID> table = new List<ParamValueItemWithID>();
            //ParamValueItemWithID parameter;
            FbDataReader dataReader = null;
            //DateTime parameterLastTime, parameterEndTime, now;
            String whereFormat = "({0}={1} AND {2} >= '{3:yyyy-MM-dd}' AND {2} < '{4:yyyy-MM-dd}') ";
            int valuesCount = 0;

            if (log.IsTraceEnabled)
            {
                log.Trace(channelLogPrefix + "Запрос архивных данных по {0} параметрам", currentTransaction.GetParameters().Count());
            }

            ArchiveRequestTransaction transaction = currentTransaction;
            currentTransaction = new ArchiveRequestTransaction();

            Connect();

            //minTime = DateTime.MaxValue;
            //maxTime = DateTime.MinValue;
            //now = DateTime.Now;

            List<String> idLisr = new List<String>();
            String[] whereString = new String[4];
            //int i;
            //for (i = 0; i < parameters.Count; i++)
            foreach (var parameter in transaction.GetParameters())
            {
                int index;//, slashIndex = m_parameters[i].propertylist[Consts.ParameterCode].IndexOf("/");

                String parameterCode = parameter[CommonProperty.ParameterCodeProperty];
                
                //String valID = m_parameters[i].propertylist[Consts.ParameterCode].Substring(0, slashIndex);
                String valID = parameterCode.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
                String par = parameterCode.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                //try { parameterLastTime = m_parameters[i].lasttime; }
                //catch (FormatException) { parameterLastTime = now.AddSeconds(-(captureIntervalLarge/* + captureIntervalNormal*/)); }
                switch (par[0])//m_parameters[i].propertylist[Consts.ParameterCode][slashIndex + 1])
                {
                    case 'P':
                    case 'p':
                        index = 0;
                        break;
                    case 'T':
                    case 't':
                        index = 1;
                        break;
                    case 'R':
                    case 'r':
                        index = 2;
                        break;
                    case 'X':
                    case 'x':
                        index = 3;
                        break;
                    default:
                        continue;
                }
                //parameterLastTime = parameterLastTime.AddSeconds(captureIntervalNormal);
                //parameterEndTime = now;
                //if (now.Subtract(parameterLastTime) > TimeSpan.FromHours(maxWaitInterval))
                //{
                //    if (stateDictionary[parameters[i].Idnum])//m_parameters[i].Value == "1")
                //    {
                //        stateDictionary[parameters[i].Idnum] = false;
                //        //m_parameters[i].Value = "0";
                //    }
                //    else
                //    {
                //        if (qualityDictionary[parameters[i].Idnum] != Quality.Bad)
                //        {
                //            qualityDictionary[parameters[i].Idnum] = Quality.Bad;
                //            stateDictionary[parameters[i].Idnum] = true;
                //            parameter = new ParamValueItemWithID(); //(Parameter)parameters[i].Clone();
                //            parameter.ParameterID = parameters[i].Idnum;
                //            parameter.Quality = Quality.Bad;
                //            parameter.Time = parameterLastTime;
                //            //parameter.Time = parameterLastTime.ToString("yyyy-MM-dd 00:00:00");
                //            //table.AddParameter(parameter);
                //            table.Add(parameter);
                //        }
                //        if (now - parameterLastTime > TimeSpan.FromHours(2 * maxWaitInterval))
                //            parameterLastTime = parameterLastTime.AddHours(maxWaitInterval);
                //        else parameterLastTime = parameterLastTime.AddSeconds(captureIntervalLarge);
                //        parameters[i].lasttime = parameterLastTime;
                //    }
                //    parameterEndTime = parameterLastTime.AddHours(maxWaitInterval);
                //    //if (parameterEndTime > now) parameterEndTime = now;
                //}
                //else if (now.Subtract(parameterLastTime) > TimeSpan.FromSeconds(captureDifference)) // установка конца периода запроса
                //    parameterEndTime = parameterLastTime.AddSeconds(captureIntervalLarge);
                //else parameterEndTime = parameterLastTime.AddSeconds(captureIntervalNormal);


                //parameterLastTime = parameterLastTime.AddSeconds(-captureIntervalPrevious); // установка начала периода запроса
                //if (parameterEndTime > now) parameterEndTime = now;

                //if (parameterLastTime < minTime) minTime = parameterLastTime;
                //if (parameterEndTime > maxTime) maxTime = parameterEndTime;

                if (whereString[index] != null
                    && whereString[index].Length != 0)
                {
                    whereString[index] += " OR ";
                }

                whereString[index] += String.Format(whereFormat,
                    m_tagValuesFieldID,
                    valID,
                    m_tagValuesFieldTimeName,
                    transaction.GetStartTime(parameter),
                    transaction.GetEndTime(parameter));
                // parameterLastTime, parameterEndTime);
            }

            for (int i = 0; i < 4; i++)
                if (whereString[i] != null)
                {
                    if (whereString[i].Length > 0)
                        whereString[i] = " AND (" + whereString[i] + ")";
                }
                else
                {
                    whereString[i] = String.Format("AND {0} = 0", m_tagValuesFieldID);
                }

            //int counter;
            String format = "SELECT {0} || '/{1}', {2}, {3} \n FROM {4} \n WHERE not ({2} is null) ";// + whereString[0],
            String[] selectQuery ={
                        String.Format(format, m_tagValuesFieldID, "P", m_tagValuesFieldPName, m_tagValuesFieldTimeName, m_tagValuesTable) + whereString[0],
                        String.Format(format, m_tagValuesFieldID, "T", m_tagValuesFieldTName, m_tagValuesFieldTimeName, m_tagValuesTable) + whereString[1],
                        String.Format(format, m_tagValuesFieldID, "R", m_tagValuesFieldRName, m_tagValuesFieldTimeName, m_tagValuesTable) + whereString[2],
                        String.Format(format, m_tagValuesFieldID, "X", m_tagValuesFieldXName, m_tagValuesFieldTimeName, m_tagValuesTable) + whereString[3]
                   };


            //counter = 0;
            //m_command.CommandText = selectQuery[counter];

            //m_command.CommandTimeout = 60000;

            //dataReader = m_command.ExecuteReader();

            try
            {
                Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
                List<ParamValueItem> valuesList = new List<ParamValueItem>();

                for (int counter = 0; counter < selectQuery.Length; counter++)
                {
                    m_command.CommandText = selectQuery[counter];

                    m_command.CommandTimeout = 60000;

                    try
                    {
                        dataReader = m_command.ExecuteReader();

                        while (dataReader.Read())
                        {
                            //if (!dataReader.Read())
                            //    if (counter > 2) break;
                            //    else
                            //    {
                            //        ++counter;
                            //        m_command.CommandText = selectQuery[counter];
                            //        dataReader.Close();
                            //        dataReader = m_command.ExecuteReader();
                            //        if (!dataReader.Read()) continue;
                            //    }

                            if (dataReader.IsDBNull(0) ||
                                    dataReader.IsDBNull(1) ||
                                    dataReader.IsDBNull(2)) continue;

                            //ParamValueItem value = new ParamValueItem();

                            String code = dataReader[0].ToString();
                            //value.Value = Convert.ToDouble(dataReader[1]);
                            ////String.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "{0}", dataReader[1]);
                            //value.Time = (DateTime)dataReader[2];

                            ////parameter.Quality = "192";//?
                            //value.Quality = Quality.Good;

                            var parameter = (from p in parameters where p[CommonProperty.ParameterCodeProperty].Equals(code) select p).FirstOrDefault();

                            if (parameter != null)
                            {
                                if (!valuesDictionary.TryGetValue(parameter, out valuesList))
                                {
                                    valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                                }

                                ParamValueItem value = new ParamValueItem();

                                //String code = dataReader[0].ToString();
                                value.Value = Convert.ToDouble(dataReader[1]);
                                //String.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "{0}", dataReader[1]);
                                value.Time = (DateTime)dataReader[2];

                                //parameter.Quality = "192";//?
                                value.Quality = Quality.Good;
                                //for (int i = 0; i < parameters.Count; i++)
                                //    if (parameters[i][CommonProperty.ParameterCodeProperty].Equals(code))
                                //    {
                                //        parameter.ParameterID = parameters[i].param;
                                //        try
                                //        {
                                //            DateTime date = parameters[i].lasttime;
                                //            if ((DateTime)dataReader[2] > date)
                                //            {
                                //                if (qualityDictionary[parameters[i].param] != Quality.Bad
                                //                    && ((DateTime)dataReader[2] - date) > TimeSpan.FromSeconds(captureIntervalNormal))
                                //                {
                                //                    ParamValueItemWithID parm1 = new ParamValueItemWithID();
                                //                    //parm1 = (Parameter)parameter.Clone();
                                //                    parm1.ParameterID = parameter.ParameterID;
                                //                    parm1.Value = 0;
                                //                    parm1.Quality = Quality.Bad;
                                //                    parm1.Time = date.AddSeconds(captureIntervalNormal);
                                //                    table.Add(parm1);
                                //                }
                                //                stateDictionary[parameters[i].param] = true;
                                //                qualityDictionary[parameters[i].param] = parameter.Quality;
                                //                //parameters[i].Value = "1";
                                //                //parameters[i].Quality = parameter.Quality;
                                //                parameters[i].lasttime = parameter.Time;
                                //            }
                                //        }
                                //        catch (FormatException) { parameters[i].lasttime = parameter.Time; }
                                //        break;
                                //    }
                                valuesList.Add(value);
                                //table.Add(parameter); 
                            }
                        }
                    }
                    finally
                    {
                        dataReader.Close();
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

                log.Debug(channelLogPrefix + "Получено архивных данных: {0}", valuesCount);
            }
            finally
            {
                if (dataReader != null) dataReader.Close();
                if (!KeepConnected) Disconnect();
            }
            //if (table != null) parametersCount = table.Count;
            //else parametersCount = 0;
            //return table;
        }

        #endregion
    }
}
