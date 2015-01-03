using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using COTES.ISTOK.Modules;
using COTES.ISTOK.Block.Redundancy;

namespace COTES.ISTOK.Block
{
    // общие данные
    // временно паблик
    public static class NSI
    {
        public static ParameterRegistrator parRegistrator = null;
        internal static ValueReceiver valReceiver = null;
        internal static ChannelManager chanManager = null;

        public /*internal*/ static RedundancyServer redundancyServer = null;
        internal static DALManager dalManager = null;
        internal static ConnectionInspector conInspector = null;
        //internal static UnloadScheduler scheduler = null;

        /// <summary>
        /// настроечные параметры
        /// путь к загружаемым библиотекам
        /// </summary>
        public static string LoadersPath;

        /// <summary>
        /// период в минутах, между промежуточными сохранениями накопленных данных
        /// </summary>
#if DEBUG
        public const int DEFAULTSAVEMEMPARAM = 1;
#else
        public const int DEFAULTSAVEMEMPARAM = 3; 
#endif

        /// <summary>
        /// период в миллисекундах опроса канала на новые значения
        /// </summary>
        public const int DEFAULTASKVALUES = 100;

        /// <summary>
        /// автонастройка периода опроса канала на новые значения
        /// </summary>
        public const bool DEFAULTAUTOINTERVAL = false;
        /// <summary>
        /// автонастройка периода опроса канала на новые значения
        /// </summary>
        public static bool autointerval = DEFAULTAUTOINTERVAL;

        public const double DEFAULTMODULELIFETIME = 120.0;
        public static TimeSpan moduleLifeTime = TimeSpan.FromMinutes(DEFAULTMODULELIFETIME);

        /// <summary>
        /// Номер запуска сервера, применяется для разделения идентификаторов асинхронных операций при разных запусках
        /// </summary>
        public static byte StartNum { get; set; }

        /// <summary>
        /// Максимальный размер пакета
        /// </summary>
        public const int DEFAULTPACKAGESIZE = 720;//240;

        public static int DBPackageSize = DEFAULTPACKAGESIZE;
        public static int RetreivePackageSize = DEFAULTPACKAGESIZE;

    }

    ///// <summary>
    ///// элемент доппараметров каналов и параметров
    ///// </summary>
    //[Serializable]
    //class PropertyItem : IComparable<PropertyItem>
    //{
    //    public string tablename = "";
    //    public int parentid = 0;
    //    public string name = "";
    //    public string value = "";

    //    /// <summary>
    //    /// Создать новый экземпляр дополнительного параметра
    //    /// </summary>
    //    public PropertyItem() { }

    //    /// <summary>
    //    /// Создать новый экземпляр дополнительного параметра
    //    /// </summary>
    //    /// <param name="TableName">Имя таблицы (Не используется)</param>
    //    /// <param name="ParetID">ИД параметра или канала</param>
    //    /// <param name="Name">Имя свойства</param>
    //    /// <param name="Value">Значение свойства</param>
    //    public PropertyItem(string TableName, int ParetID, string Name, string Value)
    //        : this()
    //    {
    //        tablename = TableName;
    //        parentid = ParetID;
    //        name = Name;
    //        value = Value;
    //    }

    //    public override string ToString()
    //    {
    //        return "tablename=" + tablename + " parenid=" + parentid.ToString() + " name=" + name + " value=" + value;
    //    }

    //    #region IComparable<PropertyItem> Members

    //    public int CompareTo(PropertyItem other)
    //    {
    //        int r = 0;
    //        if (parentid < other.parentid) return -1;
    //        else if (parentid > other.parentid) return +1;
    //        r = String.Compare(name, other.name, true);
    //        if (r != 0) return r;
    //        return 0;
    //    }

    //    #endregion
    //}
    ///// <summary>
    ///// элемент списка параметров
    ///// </summary>
    //[Serializable]
    //class ParamItem : IEquatable<ParamItem>, IComparable<ParamItem>
    //{
    //    /// <summary>
    //    /// Таблица свойств параметра
    //    /// </summary>
    //    private Hashtable dicProperties = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

    //    private int m_idnum = 0;
    //    private int m_channel = 0;

    //    private ParamItem()
    //    {
    //        Name = "";
    //    }
    //    internal ParamItem(int Idnum)
    //        : this()
    //    {
    //        m_idnum = Idnum;
    //    }
    //    public ParamItem(int Idnum, int Channel)
    //        : this()
    //    {
    //        m_idnum = Idnum;
    //        m_channel = Channel;
    //    }

    //    /// <summary>
    //    /// Имя параметра
    //    /// </summary>
    //    public string Name { get; set; }
    //    /// <summary>
    //    /// Свойства параметра
    //    /// </summary>
    //    public IDictionary Properties
    //    {
    //        get { return dicProperties; }
    //    }
    //    /// <summary>
    //    /// Номер параметра
    //    /// </summary>
    //    public int Idnum { get { return m_idnum; } }
    //    /// <summary>
    //    /// Номер канала-родителя параметра
    //    /// </summary>
    //    public int Channel { get { return m_channel; } }

    //    double defaultApperture = 0.0d;
    //    /// <summary>
    //    /// Апертура параметра
    //    /// </summary>
    //    public double Aperture
    //    {
    //        get
    //        {
    //            try
    //            {
    //                if (dicProperties.Contains("aperture"))
    //                    return Double.Parse(dicProperties["aperture"].ToString(), NumberFormatInfo.InvariantInfo);
    //                return defaultApperture;
    //            }
    //            catch { return defaultApperture; } //апертура по умолчанию
    //        }
    //    }
    //    /// <summary>
    //    /// Количество дней хранения параметра
    //    /// </summary>
    //    public int Store_days
    //    {
    //        get
    //        {
    //            try
    //            {
    //                int res = 0;

    //                if (dicProperties.Contains("store_days"))
    //                    res = Int32.Parse(dicProperties["store_days"].ToString());

    //                return res;
    //            }
    //            catch { return 0; }
    //        }
    //    }
    //    /// <summary>
    //    /// Время последнего значения
    //    /// </summary>
    //    public DateTime LastTime { get; set; }

    //    /// <summary>
    //    /// Добавить свойство параметру
    //    /// </summary>
    //    /// <param name="name">Имя свойства</param>
    //    /// <param name="value">Значение свойства</param>
    //    public void AddProperty(string name, string value)
    //    {
    //        dicProperties.Add(name, value);
    //    }

    //    public override string ToString()
    //    {
    //        return "idnum=" + Idnum.ToString() + " channel=" + Channel.ToString();
    //    }

    //    #region IEquatable<ParamItem> Members

    //    public bool Equals(ParamItem other)
    //    {
    //        return Idnum == other.Idnum;
    //    }

    //    #endregion

    //    #region IComparable<ParamItem> Members

    //    public int CompareTo(ParamItem other)
    //    {
    //        if (Idnum < other.Idnum) return -1;
    //        else if (Idnum > other.Idnum) return +1;
    //        return 0;
    //    }

    //    #endregion
    //}
    ///// <summary>
    ///// элемент списка каналов
    ///// </summary>
    //[Serializable]
    //class ChannelItem : MarshalByRefObject, IEquatable<ChannelItem>, IComparable<ChannelItem>
    //{
    //    /// <summary>
    //    /// Таблица свойств канала
    //    /// </summary>
    //    private Hashtable dicProperties = new Hashtable();
    //    /// <summary>
    //    /// Список параметров канала
    //    /// </summary>
    //    private List<ParamItem> lstParameters = new List<ParamItem>();

    //    //List<Message> messagesList=new List<Message>();
    //    FalseLog falseLog = new FalseLog();

    //    private int idnum = 0;
    //    private Thread thread = null;
    //    private DateTime lastActivity;
    //    private BaseModule lmod = null;
    //    private bool fopenchannel = false;
    //    private volatile bool fstop = false;
    //    private volatile bool fbusy = true;

    //    public string Name { get; set; }
    //    public string Libname { get; set; }

    //    // время начала сбора данных
    //    public DateTime? start_time = null;
    //    // активность канала
    //    public bool Active { get; set; }
    //    // писать значения в базу
    //    public bool Storable { get; set; }

    //    /// <summary>
    //    /// Свойства канала
    //    /// </summary>
    //    public IDictionary Properties
    //    {
    //        get { return dicProperties; }
    //    }
    //    /// <summary>
    //    /// Параметры канала
    //    /// </summary>
    //    public ParamItem[] Parameters
    //    {
    //        get { return lstParameters.ToArray(); }
    //    }
    //    /// <summary>
    //    /// Буфер значений (надо бы в конструктор засунуть)
    //    /// </summary>
    //    public ValueBuffer Buffer { get; set; }
    //    /// <summary>
    //    /// Интервал ожидания останова канала (мс)
    //    /// </summary>
    //    public int JoinInterval { get; set; }
    //    /// <summary>
    //    /// Последнее появившееся исключение
    //    /// </summary>
    //    //public Exception LastException { get; set; }

    //    public bool IsLoaded { get { return lmod != null; } }
    //    public bool IsStopped { get { return thread == null; } }
    //    public int Idnum { get { return idnum; } }

    //    public bool HasError { get; protected set; }

    //    public DateTime LastActivityTime { get { return lastActivity; } }

    //    public ChannelItem(int Idnum)
    //    {
    //        Active = true;
    //        Storable = true;
    //        Name = "";
    //        Libname = "";
    //        JoinInterval = 10000;
    //        idnum = Idnum;

    //        System.Runtime.Remoting.Channels.IChannel remChannel = System.Runtime.Remoting.Channels.ChannelServices.GetChannel("inner");
    //        if (remChannel == null)
    //        {
    //            System.Runtime.Remoting.Channels.BinaryServerFormatterSinkProvider serverProv = new System.Runtime.Remoting.Channels.BinaryServerFormatterSinkProvider();
    //            System.Runtime.Remoting.Channels.BinaryClientFormatterSinkProvider clientProv = new System.Runtime.Remoting.Channels.BinaryClientFormatterSinkProvider();
    //            IDictionary props = new Hashtable();

    //            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

    //            props["name"] = "inner";
    //            props["port"] = 0;//int.Parse(Properties["Settings/Remoting_port"].ToString());
    //            props.Add("typeFilterLevel", System.Runtime.Serialization.Formatters.TypeFilterLevel.Full);

    //            System.Runtime.Remoting.Channels.Tcp.TcpChannel tcpChannel = new System.Runtime.Remoting.Channels.Tcp.TcpChannel(props, clientProv, serverProv);
    //            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(tcpChannel, false);
    //        }

    //    }
    //    public ChannelItem(int Idnum, string LibName)
    //        : this(Idnum)
    //    {
    //        Libname = LibName;
    //    }

    //    public void AddParameter(ParamItem param)
    //    {
    //        lstParameters.Add(param);
    //    }

    //    public void ClearParameters()
    //    {
    //        lstParameters.Clear();
    //    }

    //    public void AddProperty(string name, string value)
    //    {
    //        dicProperties.Add(name, value);
    //    }

    //    public String[] GetLoadModules(String loaderPath)
    //    {
    //        List<String> modules = new List<string>();
    //        try
    //        {
    //            Type baseModuleType = typeof(BaseModule);
    //            DirectoryInfo dir = new DirectoryInfo(loaderPath);
    //            foreach (FileInfo file in dir.GetFiles("*.dll"))
    //            {
    //                try
    //                {
    //                    Assembly l = Assembly.LoadFrom(file.FullName);
    //                    Type[] t = l.GetTypes();

    //                    lmod = null;
    //                    for (int j = 0; j < t.Length; j++)
    //                        if (t[j].IsSubclassOf(baseModuleType))
    //                        {
    //                            try
    //                            {
    //                                lmod = (BaseModule)Activator.CreateInstance(t[j]);
    //                                break;
    //                            }
    //                            catch { }
    //                        }
    //                    if (lmod != null) modules.Add(file.Name);
    //                }
    //                catch { }
    //            }
    //        }
    //        catch (Exception) { }
    //        return modules.ToArray();
    //    }

    //    public bool LoadModule(String loadersPath)
    //    {
    //        //CommonData.Trace(4, "ChannelItem : начали загрузку модуля управления каналом");
    //        if (lmod != null) return true;
    //        try
    //        {
    //            string fn = Path.Combine(loadersPath, Libname);
    //            Assembly l = Assembly.LoadFrom(fn);
    //            Type[] t = l.GetTypes();
    //            Type baseModuleType = typeof(BaseModule);
    //            for (int j = 0; j < t.Length; j++)
    //                if (t[j].IsSubclassOf(baseModuleType))
    //                {
    //                    try
    //                    {
    //                        lmod = (BaseModule)Activator.CreateInstance(t[j]);
    //                        //lmod.SetFalseLog(falseLog);
    //                        break;
    //                    }
    //                    catch { }
    //                }
    //        }
    //        catch (Exception ex)
    //        {
    //            AddMessage(MessageCategory.Error, "ChannelItem : ошибка загрузки модуля управления каналом\r\n" + ex.Message);
    //            lmod = null;
    //            return false;
    //        }
    //        //CommonData.Trace(4, "ChannelItem : закончили загрузку модуля управления каналом");
    //        return true;
    //    }

    //    #region Messages
    //    private void AddMessage(MessageCategory messageCategory, string messageFormat, params Object[] pars)
    //    {
    //        //lock (messagesList)
    //        //{
    //        //    messagesList.Add(new Message(messageCategory, messageFormat, pars));
    //        //}
    //        if (falseLog != null)
    //            falseLog.AddMessage(messageCategory, messageFormat, pars);
    //    }

    //    public Message[] GetMessages()
    //    {
    //        Message[] retMessages = null;

    //        if (falseLog != null)
    //        {
    //            if (lmod != null) falseLog.AddRangeMessage(lmod.GetMessages());
    //            retMessages = falseLog.GetMessages();
    //        }
    //        if (retMessages != null) foreach (Message mes in retMessages)
    //            {
    //                mes.AppendText(String.Format("При работе канала {0}: ", Idnum));
    //            }
    //        return retMessages;
    //    }
    //    #endregion

    //    /// <summary>
    //    /// установить параметры канала
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool InitModule()
    //    {
    //        DateTime lastTime;
    //        string str_prop;
    //        string str_value;

    //        //CommonData.Trace(4, "ChannelItem : начали загрузку параметров канала в модуль");
    //        if (lmod == null)
    //        {
    //            //CommonData.Trace(1, "ChannelItem : не загружен модуль");
    //            return false;
    //        }
    //        //CommonData.Trace(5, "ChannelItem : удаляем все списки из модуля");
    //        lmod.Clear();
    //        //CommonData.Trace(5, "ChannelItem : добавляем канал");
    //        lmod.AddChannel(idnum, Properties);
    //        //CommonData.Trace(5, "ChannelItem : добавляем свойства канала");
    //        start_time = null;
    //        Active = true;
    //        Storable = true;

    //        foreach (var prop in dicProperties.Keys)
    //        {
    //            str_prop = prop.ToString();
    //            str_value = dicProperties[prop].ToString();

    //            switch (str_prop.ToLower())
    //            {
    //                case "start_time":
    //                    start_time = DateTime.Parse(str_value, DateTimeFormatInfo.InvariantInfo);// (DateTime)dateconv.ConvertFromInvariantString(str_value);
    //                    break;
    //                case "active":
    //                    Active = Int32.Parse(str_value) != 0;
    //                    break;
    //                case "store_db":
    //                    Storable = Int32.Parse(str_value) != 0;
    //                    break;
    //            }
    //        }

    //        //CommonData.Trace(5, "ChannelItem : добавляем параметры");

    //        foreach (var item in lstParameters)
    //        {
    //            lastTime = DateTime.Now;

    //            if (start_time != null)
    //            {
    //                if (item.LastTime == DateTime.MinValue)
    //                    lastTime = ((DateTime)start_time).AddSeconds(-1);
    //                else
    //                {
    //                    if (start_time > item.LastTime)
    //                        lastTime = ((DateTime)start_time).AddSeconds(-1);
    //                    else
    //                        lastTime = item.LastTime;
    //                }
    //            }
    //            else
    //            {
    //                if (item.LastTime == DateTime.MinValue)
    //                    lastTime = DateTime.Now;
    //                else
    //                    lastTime = item.LastTime;
    //            }
    //            lmod.AddParameter(idnum, item.Idnum, item.Properties, lastTime);
    //        }

    //        //CommonData.Trace(4, "ChannelItem : закончили загрузку параметров канала в модуль");
    //        return true;
    //    }

    //    /// <summary>
    //    /// запустить канал в работу
    //    /// </summary>
    //    public void Exec()
    //    {
    //        bool exit = false;

    //        fstop = false;
    //        while (!fstop && !exit)
    //        {
    //            try
    //            {
    //                Open();
    //                WorkData();
    //                lastActivity = DateTime.Now;
    //                if (fstop) break;

    //                //Thread.Sleep(100);

    //                HasError = lmod.cmd_GetStateChannel(Idnum) == StateChannel.Error;
    //                fbusy = false;
    //                //try
    //                //{
    //                if (NSI.autointerval && minInterval > 0) Thread.Sleep((int)minInterval);
    //                else Thread.Sleep(NSI.DEFAULTASKVALUES);
    //                //}
    //                //catch (ThreadInterruptedException) { throw; }
    //                //catch (ThreadAbortException) { throw; }
    //                //catch { break; }

    //                //while (fbusy) try { Thread.Sleep(10); }
    //                //    catch { break; }
    //                while (!fstop && fbusy) Thread.Sleep(10);

    //                fbusy = true;
    //            }
    //            catch (ThreadInterruptedException) { }
    //            catch (ThreadAbortException) { }
    //            //catch (ThreadAbortException ex)
    //            //{
    //            //    AddMessage(MessageCategory.Error, "Сбой при работе канала {0}: {1}", Name, ex.Message);
    //            //    exit = true;
    //            //}
    //            catch (Exception ex)
    //            {
    //                AddMessage(MessageCategory.Error, "Сбой при работе канала {0}: {1}", Name, ex.Message);
    //                exit = true;
    //            }
    //        }

    //        if (!exit)
    //        {
    //            try
    //            {
    //                // закрыть канал обмена
    //                Close();
    //                // прочитать все данные из очереди
    //                WorkData();
    //                // сохранить все буферы
    //                Buffer.FlushAll();
    //                // прекратить работу
    //            }
    //            catch (ThreadInterruptedException) { }
    //            catch (ThreadAbortException) { }
    //            catch (Exception ex)
    //            {
    //                AddMessage(MessageCategory.Error, "Сбой при работе канала {0}: {1}", Name, ex.Message);
    //            }
    //        }
    //        fbusy = false;
    //        thread = null;
    //    }

    //    /// <summary>
    //    /// открыть канал
    //    /// </summary>
    //    public void Open()
    //    {
    //        if (fopenchannel) return;
    //        if (lmod != null)
    //        {
    //            if (!lmod.cmd_Open()) return;
    //            fopenchannel = true;
    //        }
    //    }

    //    /// <summary>
    //    /// закрыть канал
    //    /// </summary>
    //    public void Close()
    //    {
    //        if (!fopenchannel) return;
    //        if (lmod != null)
    //        {
    //            if (!lmod.cmd_Close())
    //            {
    //                //NSI.Trace("Закрыт канал: " + idnum.ToString());
    //                return;
    //            }
    //            fopenchannel = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Сохранение параметра (если надо) в банк для мнемосхем
    //    /// </summary>
    //    /// <param name="param"></param>
    //    private void SaveParamValue(ParamValueItemWithID param)
    //    {
    //        NSI.parRegistrator.UpdateValue(param);
    //    }
    //    double minInterval = 0;
    //    private Dictionary<int, DateTime> lastTimes = new Dictionary<int, DateTime>();

    //    /// <summary>
    //    /// обработать данные
    //    /// </summary>
    //    public void WorkData()
    //    {
    //        if (lmod == null) return;

    //        List<ParamValueItemWithID> pr = lmod.cmd_Get();
    //        if (pr == null) return;
    //        if (NSI.autointerval) minInterval = GetInterval(pr, lastTimes);
    //        //for (int i = 0; i < pr.Length; i++)
    //        //{
    //        //    ParamReceiveItem e = (ParamReceiveItem)pr[i];
    //        foreach (ParamValueItemWithID e in pr)
    //        {
    //            e.ChangeTime = DateTime.Now;
    //            if (Buffer != null)
    //                Buffer.AddValue(e);
    //        }
    //        //pr.Dispose();
    //        pr.Clear();
    //    }

    //    public double GetInterval(List<ParamValueItemWithID> pr, Dictionary<int, DateTime> lastTimes)
    //    {
    //        int i;
    //        TimeSpan timeSpan, interval = TimeSpan.Zero;
    //        Dictionary<int, List<DateTime>> dateTimes = new Dictionary<int, List<DateTime>>();

    //        //for (i = 0; i < pr.Count; i++)
    //        //{
    //        //    ParamReceiveItem item = p[i] as ParamReceiveItem;
    //        foreach (ParamValueItemWithID item in pr)
    //        {
    //            if (item != null)
    //            {
    //                if (!dateTimes.ContainsKey(item.ParameterID)) dateTimes.Add(item.ParameterID, new List<DateTime>());
    //                dateTimes[item.ParameterID].Add(item.Time);
    //            }
    //        }
    //        foreach (int key in dateTimes.Keys)
    //        {
    //            DateTime lastTime;
    //            if (dateTimes[key].Count > 0)
    //            {
    //                if (lastTimes.ContainsKey(key)) dateTimes[key].Add(lastTimes[key]);
    //                dateTimes[key].Sort();
    //                for (i = 1; i < dateTimes[key].Count; i++)
    //                {
    //                    timeSpan = dateTimes[key][i] - dateTimes[key][i - 1];
    //                    if (timeSpan != TimeSpan.Zero && (timeSpan < interval || interval == TimeSpan.Zero))
    //                        interval = timeSpan;
    //                }
    //                lastTime = dateTimes[key][dateTimes[key].Count - 1];
    //                if (!lastTimes.ContainsKey(key)) lastTimes.Add(key, lastTime);
    //                else lastTimes[key] = lastTime;
    //            }
    //        }
    //        return interval.TotalMilliseconds;
    //    }

    //    #region BaseLoader
    //    /// <summary>
    //    /// запросить и выдать список параметров
    //    /// </summary>
    //    /// <param name="Channel"></param>
    //    /// <returns></returns>
    //    public ParamSendItem[] GetParamList(int Channel)
    //    {
    //        if (lmod == null) return null;
    //        if (idnum == Channel)
    //        {
    //            ParamSendItem[] res = lmod.cmd_GetParamList(Channel);
    //            if (res != null)
    //                foreach (var item in res)
    //                    if (string.IsNullOrEmpty(item.name)) item.name = item.FindProperty(Consts.ParameterCode);
    //            return res;
    //        }
    //        else return null;
    //    }

    //    /// <summary>
    //    /// Получение данных о состоянии канала
    //    /// </summary>
    //    /// <returns></returns>
    //    public DataTable GetState()
    //    {
    //        const String chanalNameColumnName = "ChannelName";
    //        const String channelNameColumnCaption = "Канал";
    //        DataTable table;

    //        if (lmod != null)
    //            table = lmod.ModuleState();
    //        else
    //            table = new DataTable();

    //        table.TableName = "Состояние каналов";
    //        if (!table.Columns.Contains(chanalNameColumnName))
    //        {
    //            DataColumn column = table.Columns.Add(chanalNameColumnName);
    //            column.Caption = channelNameColumnCaption;
    //            foreach (DataRow row in table.Rows) row[chanalNameColumnName] = idnum;
    //        }
    //        if (!table.Columns.Contains("Название"))
    //        {
    //            table.Columns.Add("Название");
    //            foreach (DataRow row in table.Rows) row["Название"] = Name;
    //        }

    //        return table;
    //    }
    //    #endregion

    //    public void Start()
    //    {
    //        Thread t = new Thread(new ThreadStart(Exec));
    //        t.Name = Name;
    //        thread = t;
    //        t.Start();
    //        lastActivity = DateTime.Now;
    //    }
    //    internal void Stop()
    //    {
    //        if (thread != null)
    //        {
    //            fstop = true;
    //            thread.Join(JoinInterval);
    //            if (thread != null && thread.ThreadState != ThreadState.Stopped)
    //                thread.Abort();
    //            //thread.Interrupt();
    //            thread = null;
    //        }
    //    }

    //    public override object InitializeLifetimeService()
    //    {
    //        System.Runtime.Remoting.Lifetime.ILease lease =
    //            base.InitializeLifetimeService() as System.Runtime.Remoting.Lifetime.ILease;
    //        if (lease != null) lease.InitialLeaseTime = TimeSpan.Zero;
    //        return lease;
    //    }


    //    #region IComparable<ChannelItem> Members

    //    public int CompareTo(ChannelItem other)
    //    {
    //        if (idnum < ((ChannelItem)other).Idnum) return -1;
    //        else if (idnum > ((ChannelItem)other).Idnum) return +1;
    //        return 0;
    //    }

    //    #endregion

    //    #region IEquatable<ChannelItem> Members

    //    public bool Equals(ChannelItem other)
    //    {
    //        return idnum == ((ChannelItem)other).Idnum;
    //    }

    //    #endregion

    //    public AppDomain GetDomain()
    //    {
    //        return AppDomain.CurrentDomain;
    //    }

    //    public ItemProperty[] GetPropertiesList()
    //    {
    //        return lmod.GetPropertiesList();
    //    }
    //}
}
