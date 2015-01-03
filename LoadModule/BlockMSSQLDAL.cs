using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using COTES.ISTOK.Block.MirroringManager;
using COTES.ISTOK.Block.Redundancy;
using Microsoft.SqlServer;
//using SimpleLogger;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Replication;
using Microsoft.SqlServer.Server;
using NLog;

namespace COTES.ISTOK.Block
{
    public class MSSqlDAL : DALManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private const string MetaDataTableName = "MetaData";
        private SqlConnection connection = null;

        private Object connectionLock = new Object();

        public MSSqlDAL()
        {
            ConnectionProperties = new Dictionary<String, String>();
        }
        //public MSSqlDAL()
        //    : this()
        //{
        //    //this.messageLog = messageLog;
        //}

        #region Connection Methods
        public override void Connect()
        {
            SqlConnectionStringBuilder builder;

            try
            {
            if (connection != null)
                Disconnect();
                builder = new SqlConnectionStringBuilder();
                builder.DataSource = ConnectionProperties["DB_host"];
                builder.InitialCatalog = ConnectionProperties["DB_name"];
                builder.UserID = ConnectionProperties["DB_user"];
                builder.Password = ConnectionProperties["DB_pass"];
                connection = new SqlConnection(builder.ConnectionString);

                connection.Open();
                Connected = true;
            }
            catch (Exception)
            {
                Connected = false;
                throw;
            }
        }
        public override void Disconnect()
        {
            try
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            finally
            {
                connection = null;
                Connected = false;
            }
        }
        /// <summary>
        /// Получить строку подключения
        /// </summary>
        /// <returns>Строка подключения</returns>
        internal override SecureString GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            SecureString ss = new SecureString();

            builder.DataSource = ConnectionProperties["DB_host"];
            builder.InitialCatalog = ConnectionProperties["DB_name"];
            builder.UserID = ConnectionProperties["DB_user"];
            builder.Password = ConnectionProperties["DB_pass"];
            foreach (var item in builder.ConnectionString)
                ss.AppendChar(item);
            ss.MakeReadOnly();

            return ss;
        }

        private void KeepConnection()
        {
            if (Connected && (connection.State == ConnectionState.Broken || connection.State == ConnectionState.Closed))
            {
                log.Warn("connection.State='{0}' while Connected='{1}'", connection.State, Connected);
                //messageLog.Message(MessageLevel.Warning, "connection.State='{0}' while Connected='{1}'", connection.State, Connected);
                
                Connected = false;
            }
            if (!Connected)
            {
                Connect();
            }
        }
        #endregion

        #region LoadInfo Methods
        public override IEnumerable<ChannelInfo> GetChannels(IEnumerable<ModuleInfo> modules)
        {
            System.ComponentModel.DateTimeConverter dateconv = new System.ComponentModel.DateTimeConverter();
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            List<ChannelInfo> channelList = new List<ChannelInfo>();

            string query = "SELECT idnum,name FROM canals";

            DataTable channelsTable = QueryToTable(query);

            foreach (DataRow channelRow in channelsTable.Rows)
            {
                DataTable propertiesTable = GetProperties(Convert.ToInt32(channelRow["idnum"]));

                var rows = propertiesTable.Select("name = 'libname'");
                String libname;

                if (rows != null && rows.Length > 0)
                {
                    libname = rows[0]["value"].ToString();

                    var module = (from m in modules where m.Name == libname select m).FirstOrDefault();

                    if (module != null)
                    {
                        ChannelInfo channel = new ChannelInfo()
                        {
                            Id = int.Parse(channelRow["idnum"].ToString()),
                            Module = module,
                            Name = channelRow["name"].ToString(),
                        };

                        foreach (DataRow propertyRow in propertiesTable.Rows)
                        {
                            String propertyName = propertyRow["name"].ToString();

                            ItemProperty prop = (from p in module.ChannelProperties where p.Name == propertyName select p).FirstOrDefault();

                            if (prop != null)
                            {
                                channel.SetPropertyValue(prop, propertyRow["value"].ToString());
                            }

                            else
                            {
                                const String startTimeAttributeName = "start_time";
                                const String activeAttributeName = "active";
                                const String storeValuesAttributeName = "store_db";

                                //bool res;
                                switch (propertyName)
                                {
                                    case startTimeAttributeName:
                                        channel.StartTime = (DateTime)dateconv.ConvertFromInvariantString(propertyRow["value"].ToString());
                                        break;
                                    case activeAttributeName:
                                        //if (bool.TryParse(propertyRow["value"].ToString(), out res))

                                        channel.Active = "1".Equals(propertyRow["value"]);
                                        //else
                                        //    channel.Active = false;
                                        break;
                                    case storeValuesAttributeName:
                                        //if (bool.TryParse(propertyRow["value"].ToString(), out res))
                                        channel.Storable = "1".Equals(propertyRow["value"]);
                                        //else
                                        //    channel.Storable = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        DataTable parametersTable = GetParameters(channel.Id);

                        List<ParameterItem> parameters = new List<ParameterItem>();

                        foreach (DataRow parameterRow in parametersTable.Rows)
                        {
                            ParameterItem param = new ParameterItem()
                            {
                                Idnum = Convert.ToInt32(parameterRow["idnum"]),
                                Name = parameterRow["name"].ToString()
                            };

                            DataTable parameterProperties = GetProperties(param.Idnum);

                            foreach (DataRow propertyRow in parameterProperties.Rows)
                            {
                                String propertyName = propertyRow["name"].ToString();
                                ItemProperty prop = (from p in module.ParameterProperties where p.Name == propertyName select p).FirstOrDefault();
                                if (prop != null)
                                {
                                    param.SetPropertyValue(prop, propertyRow["value"].ToString());
                                }
                            }
                            parameters.Add(param);
                        }
                        channel.Parameters = parameters.ToArray();
                        channelList.Add(channel);
                    }
                }
            }
            return channelList;
        }
        //public override DataTable GetChannel(int id)
        //{
        //    //if (!Connected)
        //    //    throw new Exception("Not connected");
        //    KeepConnection();

        //    SqlParameter param = new SqlParameter("@id", id);
        //    string query = "SELECT idnum,name FROM canals WHERE idnum=@id";

        //    return QueryToTable(query, new SqlParameter[] { param });
        //}
        public override DataTable GetParameters(int id)
        {
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter param = new SqlParameter("@id", id);
            string query = "SELECT idnum,name,parent FROM parameters WHERE parent=@id";

            return QueryToTable(query, new SqlParameter[] { param });
        }
        public override DataTable GetProperties(int id)
        {
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter param = new SqlParameter("@id", id);
            string query = "SELECT recid,name,value FROM properties WHERE recid=@id";

            return QueryToTable(query, new SqlParameter[] { param });
        }
        public override DataTable GetSchedules()
        {
            KeepConnection();

            string query = @"SELECT * FROM schedules;";
            return QueryToTable(query);
        }
        public override DataTable GetSchedule(int id)
        {
            KeepConnection();
            string query = @"SELECT * FROM schedules WHERE id = @id;";
            SqlParameter param = new SqlParameter(@"@id", id);
            return QueryToTable(query, new SqlParameter[] { param });
        }
        #endregion

        #region UploadInfo
        /// <summary>
        /// Загружает справочную информацию в БД
        /// </summary>
        /// <param name="input">Сериализованная информация</param>
        public override void UploadInfo(byte[] input, bool cleardata)
        {
            //dbwork.StartTransaction();
            try
            {
                using (MemoryStream inputStream = new MemoryStream(input))
                {
                    inputStream.Position = 0;
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        DataSet dataset = (DataSet)bf.Deserialize(zipStream);
                        DataTable table;

                        table = new DataTable(MetaDataTableName);
                        table.Columns.Add("TableName", typeof(string));
                        table.Columns.Add("ColumnName", typeof(string));
                        table.Columns.Add("DataTypeName", typeof(string));
                        table.Columns.Add("ColumnSize", typeof(int));
                        table.Columns.Add("NumericPrecision", typeof(int));
                        table.Columns.Add("NumericScale", typeof(int));
                        table.Columns.Add("IsPrimaryKey", typeof(bool));
                        table.Columns.Add("IsIdentity", typeof(bool));

                        table.Rows.Add(CommonData.CanalTableName, "idnum", "int", null, null, null, true);
                        table.Rows.Add(CommonData.CanalTableName, "name", "varchar", 100);

                        table.Rows.Add(CommonData.ParameterTableName, "idnum", "int", null, null, null, true);
                        table.Rows.Add(CommonData.ParameterTableName, "name", "varchar", 100);
                        table.Rows.Add(CommonData.ParameterTableName, "parent", "int");

                        table.Rows.Add(CommonData.PropertyTableName, "id", "int", null, null, null, true, true);
                        //table.Rows.Add(CommonData.PropertyTableName, "tablename", "varchar", 50);
                        table.Rows.Add(CommonData.PropertyTableName, "recid", "int");
                        table.Rows.Add(CommonData.PropertyTableName, "name", "varchar", 100);
                        table.Rows.Add(CommonData.PropertyTableName, "value", "nvarchar", 4000);

                        table.Rows.Add(@"schedules", @"id", @"int", null, null, null, true);
                        table.Rows.Add(@"schedules", @"name", @"varchar", 256);
                        table.Rows.Add(@"schedules", @"period", @"bigint");

                        dataset.Tables.Add(table);
                        UploadInfo(dataset, cleardata);
                    }
                }
                TouchValueTable();
                //dbwork.Commit();
            }
            catch (Exception exc)
            {
                log.ErrorException("Ошибка обновление справочников", exc);
                //dbwork.Rollback();
                Connected = false;
                throw;
            }
        }

        private void TouchValueTable()
        {
            String touchQuery = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[value_param]') AND type in (N'U')) " +
                "\nBEGIN\nCREATE TABLE [value_param]( " +
                "\n[idnum] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "\n[idparam] [int] NULL, " +
                "\n[tim1] [datetime] NULL, " +
                "\n[tim2] [datetime] NULL, " +
                "\n[data] [image] NULL " +
                "\n) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] " +
                "\n\nCREATE NONCLUSTERED INDEX [value_param_index1] ON [dbo].[value_param] " +
                "\n(\n	[idparam] ASC, " +
                "\n[tim1] ASC, " +
                "\n[tim2] ASC\n) " +
                "\nINCLUDE ( [idnum]) WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY] " +
                "\nEND";

            QueryToTable(touchQuery);
        }

        #endregion

        #region Packages
        public override void SavePackage(DataTable packages)
        {
            String updateQuery = "UPDATE value_param SET [tim1] = @tim1, [tim2] = @tim2, " +
            "[data] = @data WHERE [idparam] = @idparam AND [tim1] = @tim1";
            String insertQuery = "INSERT into value_param(idparam,tim1,tim2,data) values(@idparam,@tim1,@tim2,@data)";
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            foreach (DataRow row in packages.Rows)
            {
                lock(connectionLock)
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(updateQuery, connection);

                        command.Parameters.Add(new SqlParameter("@idparam", row["id"]));
                        command.Parameters.Add(new SqlParameter("@tim1", row["tim1"]));
                        command.Parameters.Add(new SqlParameter("@tim2", row["tim2"]));
                        command.Parameters.Add(new SqlParameter("@data", row["data"]));

                        if (command.ExecuteNonQuery() == 0)
                        {
                            command.CommandText = insertQuery;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.ErrorException("SavePackage", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Получает информацию о пакете данных параметра за указанное время
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <param name="param_time">Время параметра</param>
        /// <returns>Таблица с информацией</returns>
        public override DataTable LoadPackage(int id, DateTime param_time)
        {
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter par_id = new SqlParameter("@id", id);
            SqlParameter par_time = new SqlParameter("@time", param_time);
            string query = "SELECT TOP 1 idparam,tim1,tim2,data FROM value_param WHERE idparam=@id AND tim1<=@time AND @time<=tim2 ORDER BY tim1 DESC";

            return QueryToTable(query, new SqlParameter[] { par_id, par_time });
        }

        public override DataTable LoadPrevPackage(int param_id, DateTime time)
        {
            String query = "SELECT TOP 1 idparam,tim1,tim2,data FROM value_param WHERE idparam=@id AND tim2<@time ORDER by tim1 DESC";
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter par_id = new SqlParameter("@id", param_id);
            SqlParameter par_time = new SqlParameter("@time", time);

            return QueryToTable(query, new SqlParameter[] { par_id, par_time });
        }

        public override DataTable LoadNextPackage(int param_id, DateTime time)
        {
            String query = "SELECT TOP 1 idparam,tim1,tim2,data FROM value_param WHERE idparam=@id AND tim1>@time ORDER by tim1 ASC";
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter par_id = new SqlParameter("@id", param_id);
            SqlParameter par_time = new SqlParameter("@time", time);

            return QueryToTable(query, new SqlParameter[] { par_id, par_time });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param_begin_time"></param>
        /// <param name="param_end_time"></param>
        /// <returns></returns>
        public override DataTable LoadPackage(int id, DateTime param_begin_time, DateTime param_end_time, int maxvalues)
        {
            DateTime minDate = new DateTime(1753, 1, 1);
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            if (param_begin_time < minDate)
                param_begin_time = minDate;
            if (param_end_time < minDate)
                param_end_time = minDate;

            SqlParameter par_id = new SqlParameter("@id", id);
            SqlParameter par_time1 = new SqlParameter("@time1", param_begin_time);
            SqlParameter par_time2 = new SqlParameter("@time2", param_end_time);
            string query = "SELECT TOP " + (maxvalues + 1) + " idparam,tim1,tim2,data FROM value_param WHERE idparam=@id " +
                "AND ((@time1<=tim1 AND tim1<=@time2) OR (@time1<=tim2 AND tim2<=@time2) OR (@time1>tim1 AND tim2>=@time2)) ORDER by tim1 ASC";

            return QueryToTable(query, new SqlParameter[] { par_id, par_time1, par_time2 });
        }

        public override void RemovePackage(int id, DateTime param_time)
        {
            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter par_id = new SqlParameter("@id", id);
            SqlParameter par_time = new SqlParameter("@time", param_time);
            string query = "DELETE FROM value_param WHERE idparam=@id AND tim1<=@time AND @time<tim2";

            lock(connectionLock)
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(par_id);
                command.Parameters.Add(par_time);

                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Обслуживание базы
        TimeSpan longCommandTimeout = TimeSpan.FromMinutes(2);
        public override int CleanExcessValues(int count)
        {
            String selectQuery = "SELECT TOP {0} [idnum]  FROM [value_param] " +
                "WHERE [idparam] not in (SELECT [idnum]  FROM [parameters])";
            int proccessed;

            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            DataTable table = QueryToTable(String.Format(selectQuery, count), longCommandTimeout);

            proccessed = DeleteValuesByRowID(table);
            return proccessed;
        }

        public override int CleanOldValues(int count, int param_id, int store_days)
        {
            DateTime lastTime = DateTime.Today.AddDays(-store_days);
            String selectQuery = "SELECT  TOP {0} [idnum]  FROM [value_param] " +
                "WHERE [tim2]<@time and [idparam]=@param_id ";
            int proccessed;

            //if (!Connected)
            //    throw new Exception("Not connected");
            KeepConnection();

            SqlParameter par_id = new SqlParameter("@param_id", param_id);
            SqlParameter par_time = new SqlParameter("@time", lastTime);

            DataTable table = QueryToTable(String.Format(selectQuery, count), new SqlParameter[] { par_time, par_id });

            proccessed = DeleteValuesByRowID(table);
            return proccessed;
        }

        public override int CleanBadPackages(int count, int param_id)
        {
            const String selectQuery = @"SELECT TOP {0} [idnum], [idparam], [tim1], [tim2] FROM [loader-tec4].[dbo].[value_param] WHERE [idparam] = @param_id  ORDER BY idnum";

            KeepConnection();

            SqlParameter par_id = new SqlParameter("@param_id", param_id);

            DataTable table = QueryToTable(String.Format(selectQuery, count), new SqlParameter[] { par_id });

            DataTable removeTable = table.Clone();

            List<Tuple<DateTime, DateTime>> timesList = new List<Tuple<DateTime, DateTime>>();

            foreach (DataRow row in table.Rows)
            {
                DateTime timeFrom = Convert.ToDateTime(row["tim1"]);
                DateTime timeTo = Convert.ToDateTime(row["tim2"]);

                var fullCover = from t in timesList where t.Item1 < timeFrom && timeTo < t.Item2 select t;

                if (fullCover.Count() > 0)
                {
                    var r = removeTable.NewRow();
                    foreach (DataColumn column in removeTable.Columns)
                    {
                        r[column.ColumnName] = row[column.ColumnName];
                    }
                    removeTable.Rows.Add(r);
                }
                else
                {
                    var list = (from t in timesList
                                where (t.Item1 < timeFrom && timeFrom <= t.Item2) || (t.Item1 <= timeTo && timeTo < t.Item2)
                                select t).ToArray();

                    foreach (var oldPair in list)
                    {
                        timesList.Remove(oldPair);
                        timeFrom = oldPair.Item1 < timeFrom ? oldPair.Item1 : timeFrom;
                        timeTo = oldPair.Item2 > timeTo ? oldPair.Item2 : timeTo;
                    }

                    timesList.Add(Tuple.Create(timeFrom, timeTo));
                }
            }
            return DeleteValuesByRowID(removeTable);
        }

        private int DeleteValuesByRowID(DataTable table)
        {
            int proccessed = 0, i, index = 0, count, maxCount = 300;
            String deleteQuery = "DELETE FROM [value_param] WHERE [idnum] in ({0})";
            String whereQuery;//, wherePatern = " [idnum] in ({0}) ";

            while (index < table.Rows.Count)
            {
                count = index + maxCount;
                if (count > table.Rows.Count) count = table.Rows.Count;
                whereQuery = null;
                for (i = index; i < count; i++)
                //foreach (DataRow row in table.Rows)
                {
                    if (whereQuery != null) whereQuery += ", ";
                    //whereQuery += String.Format(wherePatern, table.Rows[i][0]);
                    whereQuery += table.Rows[i][0];
                }

                if (!String.IsNullOrEmpty(whereQuery))
                    lock (connectionLock)
                    {
                        using (SqlCommand command = new SqlCommand(String.Format(deleteQuery, whereQuery), connection))
                        {
                            proccessed += command.ExecuteNonQuery();
                        }
                    }
                index = count;
            }
            if (proccessed == 0) proccessed = table.Rows.Count;
            return proccessed;
        }
        #endregion

        #region Работа с репликацией
        public override void CreateMirroring(string connStrPrincipal)
        {
            //SqlConnectionStringBuilder sbuilder;
            //SqlConnection connPrincipal = null;
            //SqlConnection connPartner = null;

            //try
            //{
            //    SecureString connStringPrincipal = CommonData.DecryptText(
            //        CommonData.Base64ToString(connStrPrincipal));
                
            //    if (connStringPrincipal == null) throw new ArgumentNullException("connStringPrincipal");

            //    string cstr = CommonData.SecureStringToString(connStringPrincipal);

            //    sbuilder = new SqlConnectionStringBuilder();
            //    sbuilder.DataSource = ConnectionProperties["DB_host"];
            //    sbuilder.UserID = ConnectionProperties["DB_user"];
            //    sbuilder.Password = ConnectionProperties["DB_pass"];
            //    connPartner = new SqlConnection(sbuilder.ConnectionString);

            //    sbuilder = new SqlConnectionStringBuilder(cstr);
            //    sbuilder.InitialCatalog = "";
            //    connPrincipal = new SqlConnection(sbuilder.ConnectionString);

            //    try
            //    {
            //        connPrincipal.Open();
            //        if (connPartner.State != ConnectionState.Open) connPartner.Open();
            //    }
            //    catch (SqlException)
            //    {
            //        // ignored
            //    }

            //    string dbname = ConnectionProperties["DB_name"];

            //    if (Mirroring.MirroringInstalled(connPartner, dbname))
            //    {
            //        if (!Mirroring.MirrorringIsActive(connPartner, dbname))
            //        {
            //            Mirroring.Resume(connPrincipal, dbname);
            //            Mirroring.WaitNormalState(connPartner, dbname);
            //        }
            //    }
            //    else
            //    {
            //        string host = sbuilder.DataSource;
            //        string host2 = ConnectionProperties["DB_host"];
            //        string backup = BlockSettings.Instance.MirroringBackupPath;//OldClientSettings.Instance.LoadKey(Strings.Default.cfgMirroringBackupPath);

            //        host = host.Split("\\".ToCharArray())[0];
            //        host2 = host2.Split("\\".ToCharArray())[0];

            //        uint iport = 7000;
            //        //uint.TryParse(OldClientSettings.Instance.LoadKey(Strings.Default.cfgMirroringPort), out iport);

            //        try
            //        {
            //            iport = BlockSettings.Instance.MirroringPort;
            //        }
            //        catch { }

            //        Mirroring.Install(connPrincipal, connPartner, dbname, backup, iport, host, host2);
            //    }
            //}
            //finally
            //{
            //    if (connPrincipal != null) connPrincipal.Close();
            //    if (connPartner != null) connPartner.Close();
            //}
        }
        public override void SwitchMirroring(string connStrPrincipal)
        {
            //SqlConnectionStringBuilder sbuilder;
            //SqlConnection connPrincipal = null;
            //SqlConnection connPartner = null;
            //bool switched = false;

            //sbuilder = new SqlConnectionStringBuilder();
            //sbuilder.DataSource = ConnectionProperties["DB_host"];
            //sbuilder.UserID = ConnectionProperties["DB_user"];
            //sbuilder.Password = ConnectionProperties["DB_pass"];
            //connPartner = new SqlConnection(sbuilder.ConnectionString);

            //try
            //{
            //    string cstr = null;

            //    if (connStrPrincipal != null)
            //    {
            //        SecureString connStringPrincipal = CommonData.DecryptText(
            //            CommonData.Base64ToString(connStrPrincipal));

            //        cstr = CommonData.SecureStringToString(connStringPrincipal);
            //    }

            //    if (cstr != null)
            //    {
            //        sbuilder = new SqlConnectionStringBuilder(cstr);
            //        sbuilder.InitialCatalog = "";
            //        connPrincipal = new SqlConnection(sbuilder.ConnectionString);

            //        try
            //        {
            //            connPrincipal.Open();
            //        }
            //        catch (SqlException) 
            //        { 
            //            connPrincipal = null; 
            //        }
            //    }

            //    connPartner.Open();

            //    string dbname = ConnectionProperties["DB_name"];

            //    if (Mirroring.MirroringInstalled(connPartner, dbname))
            //    {
            //        if (!Mirroring.IsPrincipal(connPartner, dbname))
            //        {
            //            if (connPrincipal != null)
            //            {
            //                try
            //                {
            //                    if (!Mirroring.MirrorringIsActive(connPartner, dbname))
            //                    {
            //                        Mirroring.Resume(connPrincipal, dbname);
            //                        Mirroring.WaitNormalState(connPartner, dbname);
            //                    }

            //                    Mirroring.Switch(connPrincipal, dbname);
            //                    Mirroring.WaitNormalState(connPartner, dbname);
            //                    switched = true;
            //                }
            //                catch (TimeoutException)
            //                {
            //                    //
            //                }
            //                catch (MirroringException)
            //                {
            //                    //
            //                }
            //            }

            //            if (!switched)
            //            {
            //                Mirroring.ForceSwitch(connPartner, dbname);
            //                Mirroring.WaitMirroring(connPartner, dbname);
            //            }
            //        }
            //    }
            //}
            ////catch (MirroringException ex)
            ////{
            ////    //
            ////}
            ////catch (SqlException ex)
            ////{
            ////    //
            ////}
            //finally
            //{
            //    if (connPrincipal != null) connPrincipal.Close();
            //    if (connPartner != null) connPartner.Close();
            //}
        }
        public override void ResetIdentities()
        {
            SqlCommand command;
            const string val_table = "value_param";
            object cnt;

            KeepConnection();
            command = new SqlCommand(string.Format("select * from INFORMATION_SCHEMA.TABLES where TABLE_TYPE='BASE TABLE' and TABLE_NAME='{0}'",
                val_table), connection);
            cnt = command.ExecuteScalar();
            if (cnt != null)
            {
                command = new SqlCommand(string.Format("dbcc checkident ('{0}', RESEED)", val_table), connection);
                command.ExecuteNonQuery();
            }
        }
        public override COTES.ISTOK.Block.Redundancy.PublicationInfo CreateReplicationPublication(string publicationName)
        {
            COTES.ISTOK.Block.Redundancy.PublicationInfo repl = new COTES.ISTOK.Block.Redundancy.PublicationInfo();
            ServerConnection conn = null;

            try
            {
                ReplicationDatabase publicationDb;
                TransPublication publication;

                //TransArticle art;

                KeepConnection();
                repl.PublicationDbName = ConnectionProperties["DB_name"];
                //bug! DB_host может быть пустым для локалхоста!
                repl.PublisherName = ConnectionProperties["DB_host"];//System.Net.Dns.GetHostName();
                repl.PublicationName = publicationName;//"repl__" + repl.PublicationDbName + "_" + repl.PublisherName;
                
                conn = new ServerConnection(connection);

                ReplicationServer replServer = new ReplicationServer(conn);
                DistributionDatabase distrDb;
                //todo: "distribution" to config
                //todo: "ReplData" to config
                if (!replServer.IsDistributor)
                {
                    Console.WriteLine("Installing Distributor");
                    distrDb = new DistributionDatabase("distribution", conn);
                    replServer.InstallDistributor((string)null, distrDb);
                }
                else if (replServer.DistributionDatabases.Count > 0)
                {
                    distrDb = replServer.DistributionDatabases[0];
                }
                else
                    throw new ApplicationException("Distribution database is absent.");
                Console.WriteLine("Distributor is OK");
                if (!replServer.IsPublisher)
                {
                    //if (replServer.DistributionDatabases.Count > 0)
                    //{
                    Console.WriteLine("Installing Publisher");
                    DistributionPublisher distrPub = new DistributionPublisher(repl.PublisherName, conn);
                    distrPub.DistributionDatabase = distrDb.Name;
                    distrPub.WorkingDirectory = string.Format(@"\\{0}\{1}", repl.PublisherName, "ReplData");
                    distrPub.PublisherSecurity.SqlStandardLogin =
                        ConnectionProperties["ReplicationUser"];
                    distrPub.PublisherSecurity.SecureSqlStandardPassword =
                        CommonData.DecryptText(
                                CommonData.Base64ToString(ConnectionProperties["ReplicationPassword"]));
                    distrPub.Create();
                    //}
                    //else
                    //    throw new ApplicationException("Distribution database is absent.");
                }
                Console.WriteLine("Publisher is OK");

                publicationDb = new ReplicationDatabase(repl.PublicationDbName, conn);

                if (publicationDb.LoadProperties())
                {
                    if (!publicationDb.EnabledTransPublishing)
                        publicationDb.EnabledTransPublishing = true;

                    if (!publicationDb.LogReaderAgentExists)
                    {
                        publicationDb.LogReaderAgentProcessSecurity.Login =
                            ConnectionProperties["ReplicationUser"];
                        publicationDb.LogReaderAgentProcessSecurity.SecurePassword =
                            CommonData.DecryptText(
                                CommonData.Base64ToString(ConnectionProperties["ReplicationPassword"]));
                    }
                }
                else
                {
                    throw new ApplicationException(String.Format(
                        "The {0} database does not exist at {1}.",
                        publicationDb, repl.PublisherName));
                }

                publication = new TransPublication();
                publication.ConnectionContext = conn;
                publication.Name = repl.PublicationName;
                publication.DatabaseName = repl.PublicationDbName;
                publication.Type = PublicationType.Transactional;
                publication.Status = State.Active;
                publication.Attributes |= PublicationAttributes.AllowPush;
                publication.Attributes |= PublicationAttributes.AllowPull;
                publication.Attributes |= PublicationAttributes.IndependentAgent;
                publication.SnapshotGenerationAgentProcessSecurity.Login =
                    ConnectionProperties["ReplicationUser"];
                publication.SnapshotGenerationAgentProcessSecurity.SecurePassword =
                    CommonData.DecryptText(
                        CommonData.Base64ToString(ConnectionProperties["ReplicationPassword"]));
                //publication.SnapshotGenerationAgentPublisherSecurity.SqlStandardLogin =
                //    publication.SnapshotGenerationAgentProcessSecurity.Login;
                //publication.SnapshotGenerationAgentPublisherSecurity.SecureSqlStandardPassword =
                //    publication.SnapshotGenerationAgentProcessSecurity.SecurePassword;
                publication.SnapshotGenerationAgentPublisherSecurity.WindowsAuthentication = true;

                if (!publication.IsExistingObject)
                {
                    publication.Create();
                    //publication.CreateSnapshotAgent();
                }
                Console.WriteLine("Publication is created");
                //if (publication.SnapshotAgentExists)
                //    publication.StartSnapshotGenerationAgentJob();

                //этот набор надо автоматизировать
                string[] arrArticles = new string[] { "canals", "parameters", "properties",
                                                      /*"schedules",*/ "value_param" };
                foreach (var item in arrArticles)
                    try
                    {
                        AddArticleToReplication(item, repl, conn);
                    }
                    catch (Exception ex)
                    {
                        //string msg = string.Format("Ошибка добавления статьи {0} в публикацию: {1}.", item, ex.Message);
                        //messageLog.Message(MessageLevel.Error, msg);
                        log.ErrorException(String.Format("Ошибка добавления статьи {0} в публикацию.", item), ex);
                        //Console.WriteLine(msg);
                    }

                if (!publication.SnapshotAgentExists)
                    publication.CreateSnapshotAgent();
                publication.StartSnapshotGenerationAgentJob();

                return repl;
            }
            //catch (Exception ex)
            //{
            //    throw;
            //}
            finally
            {
                if (conn != null) conn.Disconnect();
            }

            //return null;
        }
        public override void DeleteReplicationPublication(string publicationName)
        {
            ServerConnection conn = null;

            //if (repl == null) throw new ArgumentNullException("repl");

            try
            {
                KeepConnection();
                conn = new ServerConnection(connection);
                TransPublication publication = new TransPublication(publicationName, ConnectionProperties["DB_name"], conn);
                if (publication.IsExistingObject)
                {
                    foreach (TransSubscription subs in publication.EnumSubscriptions())
                        subs.Remove();
                    publication.Remove();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null) conn.Disconnect();
            }
        }

        public override COTES.ISTOK.Block.Redundancy.SubscriptionInfo CreateReplicationSubscription(COTES.ISTOK.Block.Redundancy.PublicationInfo repl,
            SecureString publisherConnString)
        {
            COTES.ISTOK.Block.Redundancy.SubscriptionInfo subInfo = null;
            ServerConnection conn = null;

            if (repl == null) throw new ArgumentNullException("repl");

            try
            {
                TransPublication publication;
                TransPullSubscription subscription;
                ServerConnection publConn = new ServerConnection(new SqlConnection(CommonData.SecureStringToString(publisherConnString)));
                //string subName = "sub__" + ConnectionProperties["DB_name"] + "_" + repl.PublisherName;
                
                KeepConnection();
                conn = new ServerConnection(connection);
                
                publConn.Connect();
                publication = new TransPublication(repl.PublicationName, repl.PublicationDbName, publConn);
                
                if (publication.IsExistingObject)
                {
#if DEBUG
                    Console.WriteLine("publ is exists");
#endif
                    if ((publication.Attributes & PublicationAttributes.AllowPull) == 0)
                        publication.Attributes |= PublicationAttributes.AllowPull;

                    subInfo = new COTES.ISTOK.Block.Redundancy.SubscriptionInfo();
                    subInfo.PublicationName = repl.PublicationName;
                    subInfo.PublicationDbName = repl.PublicationDbName;
                    subInfo.PublisherName = repl.PublisherName;
                    subInfo.SubscriberName = ConnectionProperties["DB_host"];
                    subInfo.SubscriptionDbName = ConnectionProperties["DB_name"];
                    //subscription = new TransSubscription();
                    ////subscription.SubscriberTy
                    //subscription.SubscriberName = ConnectionProperties["DB_host"];
                    //subscription.SubscriptionDBName = ConnectionProperties["DB_name"];
                    //subscription.ConnectionContext = conn;
                    //if (subscription.IsExistingObject)
                    //{
        //                subscription.Remove();
                    //}
#if DEBUG
                    Console.WriteLine("subInfo created");
#endif
                    subscription = new TransPullSubscription(subInfo.SubscriptionDbName,
                        subInfo.PublisherName,
                        subInfo.PublicationDbName,
                        subInfo.PublicationName,
                        conn);
                    if (subscription.IsExistingObject)
                    {
                        //subscription.Remove();
                        //subscription = new TransSubscription(subInfo.PublicationName,
        //                    subInfo.PublicationDbName,
        //                    subInfo.SubscriberName,
        //                    subInfo.SubscriptionDbName,
        //                    conn);
                    }
                    subscription.SynchronizationAgentProcessSecurity.Login =
                        ConnectionProperties["ReplicationUser"];
                    subscription.SynchronizationAgentProcessSecurity.SecurePassword =
                        CommonData.DecryptText(
                            CommonData.Base64ToString(ConnectionProperties["ReplicationPassword"]));
                    subscription.CreateSyncAgentByDefault = true;
                    subscription.AgentSchedule.FrequencyType = ScheduleFrequencyType.Continuously;

                    subscription.Create();
#if DEBUG
                    Console.WriteLine("subs created");
#endif

                    bool registered = false;
                    foreach (TransSubscription existing in publication.EnumSubscriptions())
                        if (existing.SubscriberName.ToLower() == subInfo.SubscriberName.ToLower()
                            && existing.SubscriptionDBName.ToLower() == subInfo.SubscriptionDbName.ToLower())
                        {
                            registered = true;
                            break;
                        }
                    if (!registered)
                    {
                        publication.MakePullSubscriptionWellKnown(subInfo.SubscriberName,
                            subInfo.SubscriptionDbName,
                            SubscriptionSyncType.Automatic,
                            TransSubscriberType.ReadOnly);
                    }
                    
                    if (subscription.LoadProperties() && subscription.AgentJobId != null)
                    {
                        subscription.Reinitialize();
                        //publication.StartSnapshotGenerationAgentJob();
                        //while (!publication.SnapshotAvailable) Thread.Sleep(100);
                        //subscription.StopSynchronizationJob();
                        //
#if DEBUG
                        Console.WriteLine("subs reinitializing");
#endif
                        //subscription.Reinitialize();
                        //subscription.SynchronizeWithJob();
                    }
                    //subscription.Reinitialize();
                    //subscription.Reinitialize(true);
                    //publication.StartSnapshotGenerationAgentJob();
                    //subscription.SynchronizeWithJob();
                }
                else
                {
                    throw new ApplicationException(String.Format(
                        "The publication '{0}' does not exist on {1}.",
                        repl.PublicationName, repl.PublisherName));
                }

                return subInfo;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null) conn.Disconnect();
            }

            //return null;
        }
        public override void DeleteReplicationSubscription(PublicationInfo repl)
        {
            ServerConnection conn = null;

            //if (repl == null) throw new ArgumentNullException("repl");

            try
            {
                KeepConnection();
#if DEBUG
                if (connection == null) Console.WriteLine("Connection lost!");
#endif
                conn = new ServerConnection(connection);
                ReplicationDatabase rd = new ReplicationDatabase(ConnectionProperties["DB_name"], conn);
                foreach (TransPullSubscription item in rd.EnumTransPullSubscriptions())
                {
                    item.Remove();
                }
                //TransSubscription sub = new TransSubscription();
                //sub.PublicationName = repl.PublicationName;
                //sub.DatabaseName = repl.PublicationDbName;
                //sub.SubscriberName = ConnectionProperties["DB_host"];
                //sub.SubscriptionDBName = ConnectionProperties["DB_name"];
                //sub.ConnectionContext = conn;
                ////TransSubscription sub = new TransSubscription(repl.PublicationName,
                ////    repl.PublicationDbName,
                ////    repl.SubscriberName,
                ////    repl.SubscriptionDbName,
                ////    conn);
                //if (sub.IsExistingObject)
        //            sub.Remove();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null) conn.Disconnect();
            }
        }

        private void AddArticleToReplication(string article, COTES.ISTOK.Block.Redundancy.PublicationInfo repl, ServerConnection conn)
        {
            TransArticle art = new TransArticle(article,
                repl.PublicationName,
                repl.PublicationDbName,
                conn);
            art.SourceObjectName = article;
            art.SchemaOption |= CreationScriptOptions.DriUniqueKeys;
            art.SchemaOption |= CreationScriptOptions.DriPrimaryKey;
            art.SchemaOption |= CreationScriptOptions.DriForeignKeys;

            if (!art.IsExistingObject) art.Create();
        }
        #endregion

        #region Private Methods
        private DataTable QueryToTable(string query, SqlParameter[] parameters)
        {
            return QueryToTable(query, parameters, TimeSpan.MinValue);
        }
        private DataTable QueryToTable(string query, SqlParameter[] parameters, TimeSpan timeout)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            SqlDataReader reader = null;
            int i;

            lock(connectionLock)
            {
                try
                {

                    SqlCommand command = new SqlCommand(query, connection);
                    if (timeout != TimeSpan.MinValue) command.CommandTimeout = (int)timeout.TotalSeconds;
                    if (parameters != null && parameters.Length > 0)
                        command.Parameters.AddRange(parameters);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (table.Columns.Count == 0)
                        {
                            for (i = 0; i < reader.FieldCount; i++)
                            {
                                column = new DataColumn(reader.GetName(i));
                                column.DataType = reader.GetFieldType(i);
                                table.Columns.Add(column);
                            }
                        }
                        row = table.NewRow();

                        for (i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }

                        table.Rows.Add(row);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Connected = false;
                    throw ex;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                }
            }
            return table;
        }
        private DataTable QueryToTable(string query)
        {
            return QueryToTable(query, new SqlParameter[] { });
        }
        private DataTable QueryToTable(string query, TimeSpan timeout)
        {
            return QueryToTable(query, new SqlParameter[] { }, timeout);
        }
        #endregion

        public override void RemoveChannel(int channelId)
        {
            //DataTable cleanTable = new DataTable();

            //cleanTable.Columns.Add("dest", typeof(string));
            //cleanTable.Columns.Add("destId", typeof(string));
            //cleanTable.Columns.Add("source", typeof(string));
            //cleanTable.Columns.Add("sourceId", typeof(string));

            //cleanTable.Rows.Add("properties", "recid", "parameters", "idnum");
            //cleanTable.Rows.Add("properties", "recid", "", channelId.ToString());
            //cleanTable.Rows.Add("parameters", "parent", "", channelId.ToString());
            //cleanTable.Rows.Add("canals", "idnum", "", channelId.ToString());

            //CleanTable(cleanTable);
            SqlParameter par_channel;
            DataTable parameters;
            string query;

            query = "SELECT idnum FROM parameters WHERE parent=@channelId";
            par_channel = new SqlParameter("channelId", channelId);
            parameters = QueryToTable(query, new SqlParameter[] { par_channel });

            foreach (DataRow row in parameters.Rows)
            {
                query = "DELETE FROM properties WHERE recid=" + row["idnum"].ToString();
                QueryToTable(query);
            }
            query = "DELETE FROM parameters WHERE parent=@channelId";
            par_channel = new SqlParameter("channelId", channelId);
            QueryToTable(query, new SqlParameter[] { par_channel });

            query = "DELETE FROM properties WHERE recid=" + channelId.ToString(); ;
            QueryToTable(query);

            query = "DELETE FROM canals WHERE idnum=" + channelId.ToString(); ;
            QueryToTable(query);
        }

        private void UploadInfo(DataSet dataset, bool cleardata)
        {
            lock(connectionLock)
            {
                log.Debug("Скидывание справочников на блочных");

                foreach (DataTable item in dataset.Tables)
                {
                    if (item.TableName != MetaDataTableName)
                    {
                        log.Debug("Обновление таблицы {0}", item.TableName);
                        KeepConnection();
                        if (!TableExists(item.TableName) || cleardata)
                        {
                            log.Debug("Создание таблицы {0}", item.TableName);
                            KeepConnection();
                            CreateTable(item.TableName, dataset.Tables[MetaDataTableName]);
                        }
                        log.Debug("Заполнение таблицы {0}", item.TableName);
                        KeepConnection();
                        FillTable(item, dataset.Tables[MetaDataTableName]);
                    }
                } 

                log.Debug("Скидывание справочников на блочных завершено успешно");
            }

        }

        private bool TableExists(string tableName)
        {
            DataTable tables;
            string query = "SELECT name FROM dbo.sysobjects WHERE xtype = 'U'";

            tables = QueryToTableWithOutLock(query);
            foreach (DataRow row in tables.Rows)
            {
                if (row["name"].ToString() == tableName)
                    return true;
            }

            return false;
        }

        private DataTable QueryToTableWithOutLock(string query)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            SqlDataReader reader = null;
            int i;

            try
            {

                SqlCommand command = new SqlCommand(query, connection);
                //if (parameters != null && parameters.Length > 0)
                //    command.Parameters.AddRange(parameters);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (table.Columns.Count == 0)
                    {
                        for (i = 0; i < reader.FieldCount; i++)
                        {
                            column = new DataColumn(reader.GetName(i));
                            column.DataType = reader.GetFieldType(i);
                            table.Columns.Add(column);
                        }
                    }
                    row = table.NewRow();

                    for (i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }

                    table.Rows.Add(row);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Connected = false;
                throw ex;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return table;
        }
        private void CreateTable(string tableName, DataTable metaTable)
        {
            try
            {
                string query, fields = "";

                try
                {
                    QueryToTable("drop table " + tableName);
                    //using (SqlCommand command = new SqlCommand("delete from " + tableName, connection))
                    //{
                    //    command.ExecuteNonQuery();
                    //}
                }
                catch {  }

                StringBuilder sb;
                foreach (DataRow item in metaTable.Rows)
                {
                    if (item["TableName"].ToString() == tableName)
                    {
                        if (fields != "") fields += ",";
                        sb = new StringBuilder();
                        sb.Append(string.Format(" {0} {1} ", item["ColumnName"].ToString(), GetColumnType(item)));
                        if (item["IsIdentity"] is bool && (bool)item["IsIdentity"])
                            sb.Append(" IDENTITY(1,1) ");
                        if (item["IsPrimaryKey"] is bool && (bool)item["IsPrimaryKey"])
                            sb.Append(" PRIMARY KEY ");
                        //fields += item["ColumnName"].ToString() + " " + GetColumnType(item);
                        fields += sb.ToString();
                    }
                }
                query = "create table " + tableName + " (" + fields + ")";
                //QueryToTable(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка при скидывании справочников", ex);
            }
        }

        private void FillTable(DataTable table, DataTable metaTable)
        {
            try
            {
                List<SqlParameter> lstParams = new List<SqlParameter>();
                DB_Parameters param = new DB_Parameters();
                string query = "";
                string columns = "";

                foreach (DataColumn item in table.Columns)
                {
                    DbType type = DbType.Object;
                    foreach (DataRow row in metaTable.Rows)
                    {
                        if (row["TableName"].ToString() == table.TableName &&
                            row["ColumnName"].ToString() == item.ColumnName)
                        {
                            type = GetDbParameterType(row);
                            break;
                        }
                    }
                    if (query != "") query += ",";
                    query += "@" + item.ColumnName;
                    if (columns != "") columns += ",";
                    columns += item.ColumnName;
                    lstParams.Add(new SqlParameter(item.ColumnName, type));
                    //param.Add(item.ColumnName, type);
                }
                query = string.Format("insert into {0} ({1}) values ({2})", table.TableName, columns, query);

                foreach (DataRow item in table.Rows)
                {
                    SqlParameter[] arrParams = new SqlParameter[lstParams.Count];

                    for (int i = 0; i < lstParams.Count; i++)
                    {
                        arrParams[i] = new SqlParameter(lstParams[i].ParameterName, lstParams[i].DbType);
                        arrParams[i].Value = item[i];
                    }
                    //param[i].ParamValue = item[i];
                    //QueryToTable(query, arrParams);
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (arrParams != null && arrParams.Length > 0)
                            command.Parameters.AddRange(arrParams);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                Connected = false;
                throw;
            }
        }

        private string GetColumnType(DataRow column)
        {
            if ("varchar".Equals(column["DataTypeName"])) return String.Format("varchar({0})", column["ColumnSize"]);
            else if ("nvarchar".Equals(column["DataTypeName"])) return String.Format("nvarchar({0})", column["ColumnSize"]);
            else if ("char".Equals(column["DataTypeName"])) return String.Format("char({0})", column["ColumnSize"]);
            else if ("nchar".Equals(column["DataTypeName"])) return String.Format("nchar({0})", column["ColumnSize"]);
            else if ("decimal".Equals(column["DataTypeName"])) return String.Format("decimal({0},{1})", column["NumericPrecision"], column["NumericScale"]);
            else if ("numeric".Equals(column["DataTypeName"])) return String.Format("decimal({0},{1})", column["NumericPrecision"], column["NumericScale"]);
            else return column["DataTypeName"].ToString();

            //return "";
        }
        private DbType GetDbParameterType(DataRow column)
        {
            if ("varchar".Equals(column["DataTypeName"])) return DbType.String;
            else if ("nvarchar".Equals(column["DataTypeName"])) return DbType.String;
            else if ("char".Equals(column["DataTypeName"])) return DbType.String;
            else if ("nchar".Equals(column["DataTypeName"])) return DbType.String;
            else if ("text".Equals(column["DataTypeName"])) return DbType.String;
            else if ("ntext".Equals(column["DataTypeName"])) return DbType.String;
            else if ("sysname".Equals(column["DataTypeName"])) return DbType.String;
            else if ("numeric".Equals(column["DataTypeName"])) return DbType.Decimal;
            else if ("decimal".Equals(column["DataTypeName"])) return DbType.Decimal;
            else if ("smallmoney".Equals(column["DataTypeName"])) return DbType.Currency;
            else if ("money".Equals(column["DataTypeName"])) return DbType.Currency;
            else if ("datetime".Equals(column["DataTypeName"])) return DbType.DateTime;
            else if ("smallint".Equals(column["DataTypeName"])) return DbType.Int16;
            else if ("int".Equals(column["DataTypeName"])) return DbType.Int32;
            else if ("bigint".Equals(column["DataTypeName"])) return DbType.Int64;

            return DbType.Object;
        }

        public override DataTable GetDataSources()
        {
            return SqlDataSourceEnumerator.Instance.GetDataSources();
        }

        public override String[] GetBases()//String login, String password, String host)
        {
            List<String> bases = new List<String>();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = ConnectionProperties["DB_host"];
            builder.UserID = ConnectionProperties["DB_user"];
            builder.Password = ConnectionProperties["DB_pass"];

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                DataTable table = connection.GetSchema("Databases");

                foreach (DataRow row in table.Rows)
                {
                    bases.Add(row["database_name"].ToString());
                }
            }
            return bases.ToArray();
        }

        public SqlConnection GetConnection() { return connection; }

        public override void CheckConnection()
        {
            const int attempts = 4;
            int attemptsLeft = attempts;
            TimeSpan atempDelay = TimeSpan.FromSeconds(1);
            bool loop = true;

            while (loop)
            {
                try
                {
                    Connect();
                    Disconnect();
                    loop = false;
                }
                catch
                {
                    if (--attemptsLeft < 0)
                        throw;
                    System.Threading.Thread.Sleep(atempDelay);
                }
            }
        }
    }
}
