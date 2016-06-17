using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NLog;

namespace COTES.ISTOK.Modules.Registrator
{
    /// <summary>
    /// Модуль сбора из SQL
    /// </summary>
    public class Sql2DataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        ChannelInfo channel;
        List<ParameterItem> parameters;

        private string m_host;
        private string m_database;
        private string m_username;
        private string m_password;
        
        private string queryTagNames;
        
        private DbTable tagNames;
        private DbValuesTable tagValues;

        private SqlConnection m_connection = null;
        private SqlCommand m_command = null;

        public bool KeepConnected { get; set; }

        private void Connect()
        {
            string conString;

            log.Trace(channelLogPrefix + "Подключение к базе данных {0}/{1}.", m_host, m_database);

            if (m_connection != null &&
                m_connection.State == System.Data.ConnectionState.Open) return;

            conString = "Initial Catalog=" + m_database + ";";
            conString += "Data Source=" + m_host + ";";
            conString += "User id=" + m_username + ";";
            conString += "Password=" + m_password + ";";

            if (m_connection != null) Disconnect();

            m_connection = new SqlConnection(conString);
            m_command = m_connection.CreateCommand();
            m_connection.Open();
        }

        private void Disconnect()
        {
            log.Trace(channelLogPrefix + "Отключение от базы данных {0}/{1}.", m_host, m_database);

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

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала.");

            m_host = channelInfo[SqlDataLoaderFactory.HostProperty];
            m_database = channelInfo[SqlDataLoaderFactory.DatabaseProperty];
            m_username = channelInfo[SqlDataLoaderFactory.UsernameProperty];
            m_password = channelInfo[SqlDataLoaderFactory.PasswordProperty];

            queryTagNames = channelInfo[SqlDataLoaderFactory.QueryTagNamesProperty];

            parameters = new List<ParameterItem>(channelInfo.Parameters);
            channel = channelInfo;

            log.Debug(channelLogPrefix + "Канал инициирован. Зарегистрировано {0} параметров.", parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> parameterList = new List<ParameterItem>();
            SqlDataReader reader = null;
            SqlCommand command;

            log.Trace(channelLogPrefix + "Запрос параметров.");

            Connect();

            try
            {
                command = m_connection.CreateCommand();
                command.CommandText = queryTagNames;

                reader = command.ExecuteReader();

                parameterList = new List<ParameterItem>();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        ParameterItem p = new ParameterItem();
                        p.Name = reader[0].ToString();
                        if (!reader.IsDBNull(1))
                        {
                            p[CommonProperty.ParameterCodeProperty] = reader[1].ToString();
                        }
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

        public DataLoadMethod LoadMethod { get { return DataLoadMethod.Archive; } }

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
            SqlDataReader dataReader = null;
            string tmp;
            int valuesCount = 0;

            log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);

            Connect();
            m_command.CommandText = "SELECT ID_MEASUREMENT_VALUE, VALUE ,[DATETIME] from NTEC4_INTEGRA ('" + startTime.ToString("yyyyMMdd") + "') ";
            //m_command.CommandText = "SELECT " + tagValues.FieldTag;
            //m_command.CommandText += "," + tagValues.Field;
            //m_command.CommandText += "," + tagValues.FieldTime;
            //tmp = tagValues.FieldQuality;
            //if (!string.IsNullOrEmpty(tmp)) m_command.CommandText += "," + tmp;

            //m_command.CommandText += " FROM " + tagValues.Name;
            //m_command.CommandText += " WHERE (" + FormatTrend(tagValues.FieldTagName,
            //    tagValues) + ")";
            ////m_command.CommandText += " AND " + tagValues.FieldTime + ">='" + startTime.ToString(dateFormat) + "'";
            ////m_command.CommandText += " AND " + tagValues.FieldTime + "<='" + endTime.ToString(dateFormat) + "'";
            //String isoDateFormat = "yyyy-MM-ddTHH:mm:ss";
            //m_command.CommandText += " AND " + tagValues.FieldTime + ">='" + startTime.ToString(isoDateFormat) + "'";
            //m_command.CommandText += " AND " + tagValues.FieldTime + "<='" + endTime.ToString(isoDateFormat) + "'";

            //if (!string.IsNullOrEmpty(queryWhere))
            //{
            //    m_command.CommandText += " AND " + queryWhere;
            //}
            m_command.CommandTimeout = 60000;

            try
            {
                Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
                List<ParamValueItem> valuesList;

                log.Debug(channelLogPrefix + "Выполнение запроса \"{0}\", StartTime={{{1}}}, EndTime={{{2}}}", m_command.CommandText, startTime, endTime);
                dataReader = m_command.ExecuteReader();

                while (dataReader.Read())
                {
                    if (dataReader.IsDBNull(0) ||
                        dataReader.IsDBNull(1) ||
                        dataReader.IsDBNull(2)) continue;

                    String code = dataReader[0].ToString();

                    ParameterItem parameter = parameters.FirstOrDefault(t => t[CommonProperty.ParameterCodeProperty] == code);

                    if (parameter != null)
                    {
                        if (!valuesDictionary.TryGetValue(parameter, out valuesList))
                        {
                            valuesDictionary[parameter] = valuesList = new List<ParamValueItem>();
                        }

                        ParamValueItem value = new ParamValueItem
                        {
                            Value = Convert.ToDouble(dataReader[1]),
                            Time = (DateTime) dataReader[2],
                            Quality = Quality.Good
                        };



                        if (dataReader.FieldCount > 3)
                        {
                            if (dataReader.IsDBNull(3))
                                value.Quality = (int)dataReader[3] > 0 ? Quality.Good : Quality.Bad;
                        }
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
            catch (SqlException exc)
            {
                String message = channelLogPrefix + String.Format("Ошибка запроса значений за период [{0}; {1}]. Запрос: \"{2}\"", startTime, endTime, m_command.CommandText);
                log.Error(message, exc);
                //throw;
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

        private string FormatTrend(string strTableField)
        {
            return FormatTrend(strTableField, null);
        }
        private string FormatTrend(string tableField, DbTable table)
        {
            string[] arrSeparator = null;
            string[] arrFields = null;
            string[] arrValues = null;
            string res = "";
            bool useTable = false;
            bool useSplit = false;
            int i, j;

            if (tableField == null) throw new ArgumentNullException("null:strTableField");

            if (table == null) useTable = true;

            arrSeparator = new string[] { "\\" };

            arrFields = tableField.Split(arrSeparator, StringSplitOptions.None);
            if (arrFields.Length > 1) useSplit = true;

            for (i = 0; i < parameters.Count; i++)
            {
                if (i > 0) res += " OR ";
                if (useSplit)
                {
                    arrValues = parameters[i][CommonProperty.ParameterCodeProperty].Split(arrSeparator, StringSplitOptions.None);
                    if (arrValues.Length != arrFields.Length) throw new ArgumentException("Wrong params(fields count)!");
                }
                else
                {
                    arrValues = new string[1];
                    arrValues[0] = parameters[i][CommonProperty.ParameterCodeProperty];
                }

                res += " (";
                for (j = 0; j < arrFields.Length; j++)
                {
                    if (j > 0) res += " AND ";
                    if (useTable)
                    {
                        if (!arrFields[j].StartsWith(table.Name + table.TablePoint))
                        {
                            arrFields[j] = table.Name + table.TablePoint + arrFields[j];
                        }
                    }
                    try
                    {
                        int test;
                        test = int.Parse(arrValues[j]);
                    }
                    catch
                    {
                        arrValues[j] = "\"" + arrValues[j] + "\"";
                    }
                    finally
                    {
                        res += arrFields[j] + "=" + arrValues[j];
                    }
                }
                res += ") ";
            }

            return res;
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
