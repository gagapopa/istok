using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Информация об участнике дублирования
    /// </summary>
    [Serializable]
    public class ServerInfo
    {
        private string url;

        [XmlIgnore]
        public string UID { get; private set; }
        [XmlIgnore]
        public ServerState State { get; set; }
        public string URL
        {
            get { return url; }
            set { url = value.ToLower(); }
        }
        public byte Priority { get; set; }
        public string Version { get; set; }
        public string ConnectionString { get; set; }
        public string MirrorConnectionString { get; set; }
        public string Database { get; set; }
        public string[] NetworkInterfaces { get; set; }
        public uint Port { get; set; }
        [XmlIgnore]
        public int DBVersion { get; set; }
        [XmlIgnore]
        public bool IsMaster
        {
            //get { return Priority == byte.MaxValue; }
            get;
            internal set;
        }

        public ServerInfo()
        {
            UID = "";
            URL = "";
            Priority = 0;
            DBVersion = 0;
            IsMaster = false;
            State = ServerState.Offline;
            Version = VersionInfo.BuildVersion.VersionString/*CommonData.Version*/;
        }
        public ServerInfo(string uid)
            : this()
        {
            UID = uid;
        }
        public ServerInfo(string uid, string url)
            : this(uid)
        {
            URL = url;
        }
        public ServerInfo(string uid, string url, byte priority)
            : this(uid, url)
        {
            Priority = priority;
        }
        public ServerInfo(string uid, string[] networkInterfaces, uint port, byte priority)
            : this(uid)
        {
            NetworkInterfaces = networkInterfaces;
            Port = port;
            Priority = priority;
        }
    }

    /// <summary>
    /// Состояние участника дублирования
    /// </summary>
    public enum ServerState
    {
        /// <summary>
        /// Подключен
        /// </summary>
        Online,
        /// <summary>
        /// Отключен
        /// </summary>
        Offline,
        /// <summary>
        /// Синхронизируется
        /// </summary>
        Synchronizing,
        /// <summary>
        /// Занят
        /// </summary>
        Busy,
        /// <summary>
        /// Ожидает
        /// </summary>
        Waiting
    }
}
