using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
//using SimpleLogger;

namespace COTES.ISTOK
{
    /// <summary>
    /// работа с базой данных
    /// </summary>
    public class MyDBdata : IDisposable
    {
        private DbConnectionStringBuilder builder = null;

        private DbProviderFactory DbFactory;
        /// <summary>
        /// Тип базы данных
        /// </summary>
        private ServerType db_type;

        //ILogger logger;
        //private string db_host;
        //private string db_port;
        //private string db_name;
        //private string db_user;
        //private string db_password;
        /// <summary>
        /// Самодельный пул подключений для распаралеливания запросов
        /// </summary>
        //private Queue<DbConnection> connectionQueue;

        /// <summary>
        /// Размре пула
        /// </summary>
        public int PoolLength { get; protected set; }

        /// <summary>
        /// Размер пула по умолчанию
        /// </summary>
        public const int DefaultPoolLength = 1;

        /// <summary>
        /// Семафор для синхронизации работы с пулом
        /// </summary>
        //private System.Threading.Semaphore semaphore;

        private string selectIdentity = "";

        public MyDBdata(ServerType type)
            : this(type, DefaultPoolLength)
        { }

        /// <summary>
        /// Ид транзакция для выполнения запроса без внешней транзакции
        /// </summary>
        public const int WithoutTransactionID = 0;

        public MyDBdata(ServerType type, int poolCount)
        {
            //connectionQueue = new Queue<DbConnection>();
            this.PoolLength = poolCount;
            //this.logger = logger;
            db_type = type;
            switch (type)
            {
                case ServerType.MSSQL:
                    SqlConnection.ClearAllPools();
                    builder = new SqlConnectionStringBuilder();
                    (builder as SqlConnectionStringBuilder).MinPoolSize = PoolLength;
                    selectIdentity = "select @@identity;";
                    DbFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    break;
                case ServerType.Oracle:
                    builder = new OleDbConnectionStringBuilder();
                    (builder as OleDbConnectionStringBuilder).Provider = "OraOLEDB.Oracle";
                    DbFactory = DbProviderFactories.GetFactory("System.Data.OracleClient");
                    break;
                default:
                    builder = new OleDbConnectionStringBuilder();
                    DbFactory = DbProviderFactories.GetFactory("System.Data.OleDb");
                    break;
            }

            //for (int i = 0; i < PoolLength; i++)
            //{
            //    connectionQueue.Enqueue(DbFactory.CreateConnection());
            //}

            transactinDictionary = new Dictionary<int, TransactionKeep>();
            transactinDictionary[WithoutTransactionID] = null;

            //semaphore = new System.Threading.Semaphore(poolCount, poolCount);
        }

        /// <summary>
        /// Возвращает sql-запрос на получение значения последнего инкремента
        /// </summary>
        public string SelectIdentity
        {
            get { return selectIdentity; }
        }

        public void SetParam(string host,string dbname, string user, string password)
        {
            DB_Host = host; //db_host = host;
            DB_Name = dbname; //db_name = dbname;
            DB_User = user; //db_user = user;
            DB_Password = password; //db_password = password;
        }
        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return builder; }
        }

        /// <summary>
        /// Проверить соединение с базе данных
        /// </summary>
        public void Test()
        {
            DbConnection db_conn = LockConnection();
            try
            {
                Connect(db_conn);
                Disconnect(db_conn);
            }
            finally
            {
                ReleaseConnection(db_conn);
            }
        }

        /// <summary>
        /// Подключиться к базе
        /// </summary>
        /// <param name="db_conn"></param>
        private void Connect(DbConnection db_conn)
        {
            Disconnect(db_conn);
            switch (db_type)
            {
                case ServerType.MSSQL:
                    // коннект к mssql
                    //(builder as SqlConnectionStringBuilder).Pooling = false;
                    //(builder as SqlConnectionStringBuilder).Enlist = false;
                    //(builder as SqlConnectionStringBuilder).MinPoolSize = PoolLength;
                    (db_conn as SqlConnection).ConnectionString = builder.ConnectionString;
                    (db_conn as SqlConnection).Open();
                    break;
                case ServerType.Oracle:
                    // коннект к oracle
                    (db_conn as OracleConnection).ConnectionString = builder.ConnectionString;
                    db_conn.Open();
                    break;
                default:
                    throw new Exception("Задан неверный тип сервера БД");
            }
        }

        /// <summary>
        /// Отключиться от базы
        /// </summary>
        /// <param name="db_conn"></param>
        /// <returns></returns>
        private bool Disconnect(DbConnection db_conn)
        {
            try
            {
                if (db_conn != null && db_conn.State == ConnectionState.Open)
                    db_conn.Close();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Проверить состояние текущего подключения
        /// </summary>
        /// <param name="db_conn"></param>
        /// <returns></returns>
        private bool IsConnect(DbConnection db_conn)
        {
            if (db_conn != null && db_conn.State == ConnectionState.Open)
                return true;
            return false;
        }

        public void CreateDatabase(DbDatabaseSettings settings)
        {
            string query = "";

            try
            {
                switch (DB_Type)
                {
                    case ServerType.MSSQL:
                        SqlDatabaseSettings sql_settings = (SqlDatabaseSettings)settings;

                        query = "CREATE DATABASE " + sql_settings.Name +
                            " ON PRIMARY (NAME=" + sql_settings.Name + "_Data" +
                            ", FILENAME='" + sql_settings.DataFile + "')" +
                            " LOG ON (NAME=" + sql_settings.Name + "_Log" +
                            ", FILENAME='" + sql_settings.LogFile + "')";

                        CreateDir(sql_settings.DataFile);
                        CreateDir(sql_settings.LogFile);
                        break;
                    case ServerType.MySQL:
                        MySqlDatabaseSettings mysql_settings = (MySqlDatabaseSettings)settings;

                        query = "CREATE DATABASE " + mysql_settings.Name +
                            " CHARACTER SET utf8 " +
                            " COLLATE utf8_general_ci ";
                        break;
                    case ServerType.Oracle:
                        OracleDatabaseSettings oracle_settings = (OracleDatabaseSettings)settings;
                        break;
                }

                ExecSQL(query, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        Random generator = new Random();

        /// <summary>
        /// Класс для хранения пары транзакции и соединения
        /// </summary>
        class TransactionKeep
        {
            public DbConnection Connection { get; set; }
            public DbTransaction Transaction { get; set; }

            public TransactionKeep(DbConnection conn, DbTransaction tran)
            {
                this.Connection = conn;
                this.Transaction = tran;
            }
        }

        /// <summary>
        /// Словарь открытых транзакций
        /// </summary>
        Dictionary<int, TransactionKeep> transactinDictionary;

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        /// <returns>Ид транзакции</returns>
        public int StartTransaction()
        {
            int transactionID;
            lock (transactinDictionary)
            {
                DbConnection db_conn = LockConnection();

                if (!IsConnect(db_conn))
                    Connect(db_conn);

                do transactionID = generator.Next();
                while (transactinDictionary.ContainsKey(transactionID));
                transactinDictionary[transactionID] = new TransactionKeep(db_conn, db_conn.BeginTransaction());
            }

            return transactionID;
        }

        /// <summary>
        /// Закрыть транзакцию и освободить подключения
        /// </summary>
        /// <param name="transactionID">Ид транзакции</param>
        public void CloseTransaction(int transactionID)
        {
            TransactionKeep keep;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out keep))
            {
                transactinDictionary.Remove(transactionID);
                keep.Transaction.Dispose();
                ReleaseConnection(keep.Connection);
            }
        }

        /// <summary>
        /// Принять изменения внесенные транзакцией
        /// </summary>
        /// <param name="transactionID">ИД транзакции</param>
        public void Commit(int transactionID)
        {
            TransactionKeep keep;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out keep))
                keep.Transaction.Commit();
        }

        /// <summary>
        /// Отменить изменения внесенные транзакцией
        /// </summary>
        /// <param name="transactionID">ИД транзакции</param>
        public void Rollback(int transactionID)
        {
            TransactionKeep keep;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out keep))
                keep.Transaction.Rollback();
        }

        public int ExecSQL(string query, DB_Parameters Param)
        {
            return ExecSQL(WithoutTransactionID, query, Param, 30);//30 is default SqlCommand timeout
        }

        public int ExecSQL(int transactionID, string query, DB_Parameters Param)
        { return ExecSQL(transactionID, query, Param, 30); }

        public int ExecSQL(string query, DB_Parameters Param, int cmd_timeout)
        { return ExecSQL(WithoutTransactionID, query, Param, cmd_timeout); }

        public int ExecSQL(int transactionID, string query, DB_Parameters Param, int cmd_timeout)
        {
            int res = 0;
            TransactionKeep db_transact = null;
            DbConnection db_conn = null;
            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {

                if (!IsConnect(db_conn))
                    Connect(db_conn);

                using (DbCommand cmd = db_conn.CreateCommand())
                {
                    cmd.CommandTimeout = cmd_timeout;
                    cmd.Transaction = db_transact == null ? null : db_transact.Transaction;
                    DbParameter[] parameters = null;

                    switch (DB_Type)
                    {
                        case ServerType.MSSQL:
                            if (Param != null) parameters = Param.ToSqlParameters();
                            break;
                        case ServerType.Oracle:
                            if (Param != null) parameters = Param.ToOracleParameters();
                            break;
                    }
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.CommandText = query;
                    res = cmd.ExecuteNonQuery();
                    for (int i = 0; i < cmd.Parameters.Count; i++)
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                            Param[i].ParamValue = cmd.Parameters[i].Value;
                }
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
            return res;
        }

        /// <summary>
        /// Взять DbConnection из пула
        /// </summary>
        /// <returns></returns>
        private DbConnection LockConnection()
        {
            //DbConnection conn;
            //semaphore.WaitOne();
            //lock (connectionQueue)
            //{
            //    if (connectionQueue.Count == 0) { conn = DbFactory.CreateConnection(); }
            //    else conn = connectionQueue.Dequeue(); 
            //}
            //return conn;
            return DbFactory.CreateConnection();
        }

        /// <summary>
        /// Освободить DbConnection
        /// </summary>
        /// <param name="db_conn"></param>
        private void ReleaseConnection(DbConnection db_conn)
        {
            if (db_conn != null)
            {
                //if (IsConnect(db_conn))
                //    lock (connectionQueue)
                //    {
                //        connectionQueue.Enqueue(db_conn);
                //    }
                ////else connectionQueue.Enqueue(DbFactory.CreateConnection());
                //else
                db_conn.Dispose();
            }
            //semaphore.Release();
        }

        public DataTable ExecSQL_toTable(string Query, int Idnum)
        {
            DB_Parameters Param = new DB_Parameters();
            Param.Add("p_idnum", DbType.Int32, Idnum, ParameterDirection.Input);
            return ExecSQL_toTable(WithoutTransactionID, Query, Param);
        }

        public DataTable ExecSQL_toTable(string query, DB_Parameters Param)
        { return ExecSQL_toTable(WithoutTransactionID, query, Param); }

        public DataTable ExecSQL_toTable(int transactionID, string query, DB_Parameters Param)
        {
            DataTable res = null;
            DbConnection db_conn = null;
            TransactionKeep db_transact = null;
            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {
                if (!IsConnect(db_conn))
                    Connect(db_conn);

                using (DbCommand cmd = db_conn.CreateCommand())
                {
                    DbDataAdapter da = null;

                    cmd.Transaction = db_transact == null ? null : db_transact.Transaction;
                    cmd.CommandText = query;
                    DbParameter[] parameters = null;

                    switch (DB_Type)
                    {
                        case ServerType.MSSQL:
                            if (Param != null) parameters = Param.ToSqlParameters();
                            da = new SqlDataAdapter();
                            break;
                        case ServerType.Oracle:
                            if (Param != null) parameters = Param.ToOracleParameters();
                            da = new OracleDataAdapter();
                            break;
                    }
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    da.SelectCommand = cmd;
                    res = new DataTable();
                    /*rowsaff =*/ da.Fill(res);

                    for (int i = 0; i < cmd.Parameters.Count; i++)
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                            Param[i].ParamValue = cmd.Parameters[i].Value;
                }
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
            return res;
        }

        public void ExecSQL_toXml(int transactionID, string query, DB_Parameters Param, XmlWriter writer)
        {
            DbConnection db_conn = null;
            TransactionKeep db_transact = null;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {
                if (!IsConnect(db_conn))
                    Connect(db_conn);
                using (DbCommand cmd = db_conn.CreateCommand())
                {
                    DataTable dtSchema = null;
                    DataTable dtData = new DataTable("Table1");

                    cmd.Transaction = db_transact == null ? null : db_transact.Transaction;
                    DbParameter[] parameters = null;

                    switch (DB_Type)
                    {
                        case ServerType.MSSQL:
                            if (Param != null) parameters = Param.ToSqlParameters();
                            break;
                        case ServerType.Oracle:
                            if (Param != null) parameters = Param.ToOracleParameters();
                            break;
                    }
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    cmd.CommandText = query;
                    using (DbDataReader r = cmd.ExecuteReader())
                    {
                        dtSchema = r.GetSchemaTable();
                        dtData.Load(r);
                        SqlConnection.ClearPool(db_conn as SqlConnection);
                    }
                    for (int i = 0; i < cmd.Parameters.Count; i++)
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                            Param[i].ParamValue = cmd.Parameters[i].Value;
                    if (dtSchema != null)
                    {
                        writer.WriteStartElement("Schema");
                        dtSchema.WriteXml(writer, XmlWriteMode.WriteSchema);
                        writer.WriteEndElement();
                        writer.WriteStartElement("Data");
                        dtData.WriteXml(writer, XmlWriteMode.WriteSchema);
                        writer.WriteEndElement();
                    }
                }
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
        }

        public DbDataReader ExecSQL_toReader(int transactionID, string Query, int Idnum)
        {
            DB_Parameters Param = new DB_Parameters();
            Param.Add("p_idnum", DbType.Int32, Idnum, ParameterDirection.Input);
            return ExecSQL_toReader(transactionID, Query, Param);
        }

        public DbDataReader ExecSQL_toReader(string query, DB_Parameters Param)
        { return ExecSQL_toReader(WithoutTransactionID, query, Param); }

        public DbDataReader ExecSQL_toReader(int tranactionID, string query, DB_Parameters Param)
        {
            DbConnection db_conn = null;
            TransactionKeep db_transact = null;

            if (tranactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(tranactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {
                if (!IsConnect(db_conn))
                    Connect(db_conn);

                DbDataReader res = null;
                using (DbCommand cmd = db_conn.CreateCommand())
                {
                    cmd.Transaction = db_transact == null ? null : db_transact.Transaction;
                    DbParameter[] parameters = null;

                    switch (DB_Type)
                    {
                        case ServerType.MSSQL:
                            if (Param != null) parameters = Param.ToSqlParameters();
                            break;
                        case ServerType.Oracle:
                            if (Param != null) parameters = Param.ToOracleParameters();
                            break;
                    }
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.CommandText = query;
                    res = cmd.ExecuteReader();
                    for (int i = 0; i < cmd.Parameters.Count; i++)
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                            Param[i].ParamValue = cmd.Parameters[i].Value;
                }
                return res;
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
        }

        public void Exec_Function(int transactionID, string Query, DB_Parameters Param)
        {
            TransactionKeep db_transact = null;
            DbConnection db_conn = null;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {
                if (!IsConnect(db_conn))
                    Connect(db_conn);

                if (db_conn is SqlConnection)
                {
                    using (SqlCommand cmd = (db_conn as SqlConnection).CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = db_transact == null ? null : db_transact.Transaction as SqlTransaction;
                        if (Param != null) cmd.Parameters.AddRange(Param.ToSqlParameters());
                        cmd.CommandText = Query;
                        /*  rowsaff =*/
                        cmd.ExecuteNonQuery();
                        for (int i = 0; i < cmd.Parameters.Count; i++)
                            if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                                Param[i].ParamValue = cmd.Parameters[i].Value;
                    }
                }
                else if (db_conn is OleDbConnection)
                {
                    using (OleDbCommand cmd = (db_conn as OleDbConnection).CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = db_transact == null ? null : db_transact.Transaction as OleDbTransaction;
                        if (Param != null) cmd.Parameters.AddRange(Param.ToOleDbParameters());
                        cmd.CommandText = Query;
                        /*rowsaff =*/ cmd.ExecuteNonQuery();
                        for (int i = 0; i < cmd.Parameters.Count; i++)
                            if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                                Param[i].ParamValue = cmd.Parameters[i].Value;
                    }
                }
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
        }

        public DataSet ExecSQL_toDataset(string query, DB_Parameters Param)
        { return ExecSQL_toDataset(WithoutTransactionID, query, Param); }

        public DataSet ExecSQL_toDataset(int transactionID, string query, DB_Parameters Param)
        {
            DbConnection db_conn = null;
            TransactionKeep db_transact = null;

            if (transactionID != WithoutTransactionID
                && transactinDictionary.TryGetValue(transactionID, out db_transact))
                db_conn = db_transact.Connection;
            else db_conn = LockConnection();

            try
            {
                if (!IsConnect(db_conn))
                    Connect(db_conn);

                DataSet res = null;
                using (DbCommand cmd = db_conn.CreateCommand())
                {
                    DbDataAdapter da = null;

                    cmd.Transaction = db_transact == null ? null : db_transact.Transaction;
                    DbParameter[] parameters = null;

                    switch (DB_Type)
                    {
                        case ServerType.MSSQL:
                            if (Param != null) parameters = Param.ToSqlParameters();
                            da = new SqlDataAdapter();
                            break;
                        case ServerType.Oracle:
                            if (Param != null) parameters = Param.ToOracleParameters();
                            da = new OracleDataAdapter();
                            break;
                    }
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.CommandText = query;
                    res = new DataSet();
                    da.SelectCommand = cmd;
                    /*rowsaff = */da.Fill(res);

                    for (int i = 0; i < cmd.Parameters.Count; i++)
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                            Param[i].ParamValue = cmd.Parameters[i].Value;
                }
                return res;
            }
            finally
            {
                if (db_transact == null)
                    ReleaseConnection(db_conn);
            }
        }

        public string DB_Host
        {
            get
            {
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        return (builder as SqlConnectionStringBuilder).DataSource;
                    case ServerType.Oracle:
                        return (builder as OracleConnectionStringBuilder).DataSource;
                    default:
                        return (builder as OleDbConnectionStringBuilder).DataSource;
                }
            }
            set
            {
                Disconnect();
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        (builder as SqlConnectionStringBuilder).DataSource = value;
                        break;
                    case ServerType.Oracle:
                        (builder as OracleConnectionStringBuilder).DataSource = value;
                        break;
                    default:
                        (builder as OleDbConnectionStringBuilder).DataSource = value;
                        break;
                }
            }
        }

        private void Disconnect()
        {
            //foreach (DbConnection connection in connectionQueue.ToArray())
            //    Disconnect(connection);
        }

        public string DB_Name
        {
            get
            {
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        return (builder as SqlConnectionStringBuilder).InitialCatalog;
                    case ServerType.Oracle:
                        return (builder as OracleConnectionStringBuilder).DataSource;
                    default:
                        return (builder as OleDbConnectionStringBuilder).DataSource;
                }
            }
            set
            {
                Disconnect();
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        (builder as SqlConnectionStringBuilder).InitialCatalog = value;
                        break;
                    case ServerType.Oracle:
                        (builder as OracleConnectionStringBuilder).DataSource = value;
                        break;
                    default:
                        (builder as OleDbConnectionStringBuilder).DataSource = value;
                        break;
                }
            }
        }
        public string DB_User
        {
            get
            {
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        return (builder as SqlConnectionStringBuilder).UserID;
                    case ServerType.Oracle:
                        return (builder as OracleConnectionStringBuilder).UserID;
                    default:
                        return "";
                }
            }
            set
            {
                Disconnect();
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        (builder as SqlConnectionStringBuilder).UserID = value;
                        break;
                    case ServerType.Oracle:
                        (builder as OracleConnectionStringBuilder).UserID = value;
                        break;
                    default:
                        break;
                }
            }
        }
        public string DB_Password
        {
            get
            {
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        return (builder as SqlConnectionStringBuilder).Password;
                    case ServerType.Oracle:
                        return (builder as OracleConnectionStringBuilder).Password;
                    default:
                        return "";
                }
            }
            set
            {
                Disconnect();
                switch (db_type)
                {
                    case ServerType.MSSQL:
                        (builder as SqlConnectionStringBuilder).Password = value;
                        break;
                    case ServerType.Oracle:
                        (builder as OracleConnectionStringBuilder).Password = value;
                        break;
                    default:
                        break;
                }
            }
        }

        public ServerType DB_Type
        {
            get { return db_type; }
        }
        public MyDBdata Clone()
        {
            MyDBdata r = new MyDBdata(this.DB_Type, PoolLength);
            r.SetParam(this.DB_Host,  this.DB_Name, this.DB_User, this.DB_Password);
            return r;
        }

        private void CreateDir(string filename)
        {
            string path;
            int pos;

            pos = filename.LastIndexOf("\\");
            if (pos != -1)
                path = filename.Substring(0, pos);
            else
                path = filename;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();
        }

        #endregion

        public DataTable GetSchema(string collectionName)
        {
            DbConnection db_conn = LockConnection();
            try
            {
                Connect(db_conn);
                DataTable table = db_conn.GetSchema(collectionName);
                Disconnect(db_conn);
                return table;
            }
            finally
            {
                ReleaseConnection(db_conn);
            }
        }
    }

    public class DbDatabaseSettings
    {
        protected string db_name = "";

        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string Name
        {
            get { return db_name; }
            set { db_name = value; }
        }
    }
    public class SqlDatabaseSettings : DbDatabaseSettings
    {
        protected string db_datafile = "";
        protected string db_logfile = "";

        /// <summary>
        /// Путь и имя файла данных
        /// </summary>
        public string DataFile
        {
            get { return db_datafile; }
            set { db_datafile = value; }
        }
        /// <summary>
        /// Путь и имя файла лога
        /// </summary>
        public string LogFile
        {
            get { return db_logfile; }
            set { db_logfile = value; }
        }
    }
    public class MySqlDatabaseSettings : DbDatabaseSettings
    {
        //
    }
    public class OracleDatabaseSettings : DbDatabaseSettings
    {
        //
    }

    public class DB_Parameter
    {
        public string ParamName;
        public DbType ParamType;
        public object ParamValue;
        public ParameterDirection ParamDir;
        public DB_Parameter(string paramName, DbType paramType)
            : this(paramName, paramType, null, ParameterDirection.Input)
        { }
        public DB_Parameter(string paramName, DbType paramType, ParameterDirection paramDir)
            : this(paramName, paramType, null, paramDir)
        { }
        public DB_Parameter(string paramName, DbType paramType, object paramValue)
            : this(paramName, paramType, paramValue, ParameterDirection.Input)
        { }
        public DB_Parameter(string paramName, DbType paramType, object paramValue, ParameterDirection paramDir)
        {
            ParamName = paramName;
            ParamType = paramType;
            ParamValue = paramValue;
            ParamDir = paramDir;
        }
    }

    public class DB_Parameters : List<DB_Parameter>
    {
        public void Add(string paramName, DbType paramType)
        {
            base.Add(new DB_Parameter(paramName, paramType));
        }
        public void Add(string paramName, DbType paramType, ParameterDirection paramDir)
        {
            base.Add(new DB_Parameter(paramName, paramType, paramDir));
        }
        public void Add(string paramName, DbType paramType, object paramValue)
        {
            base.Add(new DB_Parameter(paramName, paramType, paramValue));
        }
        public void Add(string paramName, DbType paramType, object paramValue, ParameterDirection paramDir)
        {
            base.Add(new DB_Parameter(paramName, paramType, paramValue, paramDir));
        }
        public SqlParameter[] ToSqlParameters()
        {
            SqlParameter[] res = new SqlParameter[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                res[i] = new SqlParameter();
                res[i].ParameterName = this[i].ParamName;
                if (this[i].ParamType == DbType.Binary)
                    res[i].SqlDbType = SqlDbType.Image;
                else
                    res[i].DbType = this[i].ParamType;
                res[i].Direction = this[i].ParamDir;
                if (this[i].ParamValue == null)
                    res[i].Value = DBNull.Value;
                else
                    res[i].Value = this[i].ParamValue;
            }
            return res;
        }
        public OleDbParameter[] ToOleDbParameters()
        {
            OleDbParameter[] res = new OleDbParameter[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                res[i] = new OleDbParameter();
                res[i].ParameterName = this[i].ParamName;
                res[i].DbType = this[i].ParamType;
                res[i].Direction = this[i].ParamDir;
                if (this[i].ParamValue == null)
                    res[i].Value = DBNull.Value;
                else
                    res[i].Value = this[i].ParamValue;
            }
            return res;
        }

        public OracleParameter[] ToOracleParameters()
        {
            OracleParameter[] res = new OracleParameter[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                res[i] = new OracleParameter();
                res[i].ParameterName = this[i].ParamName;
                if (this[i].ParamType == DbType.Binary)
                    res[i].OracleType = OracleType.Blob;
                else
                    res[i].DbType = this[i].ParamType;
                res[i].Direction = this[i].ParamDir;
                if (this[i].ParamValue == null)
                    res[i].Value = DBNull.Value;
                else
                    res[i].Value = this[i].ParamValue;
            }
            return res;
        }
    }
}
