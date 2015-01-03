using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.DiagnosticsInfo
{
    [Serializable]
    [DataContract]
    public class GlobalDiag
    {
        [DataMember]
        private string host = "";
        [DataMember]
        private uint port = 0;
        [DataMember]
        private string dbhost = "";
        [DataMember]
        private string dbname = "";
        [DataMember]
        private string dbuser = "";

        [DisplayName("Компьютер")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        [DisplayName("Порт")]
        [ReadOnly(true)]
        [Browsable(false)]
        public uint Port
        {
            get { return port; }
            set { port = value; }
        }
        [DisplayName("Сервер БД")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string DbHost
        {
            get { return dbhost; }
            set { dbhost = value; }
        }
        [DisplayName("Имя БД")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string DbName
        {
            get { return dbname; }
            set { dbname = value; }
        }
        [DisplayName("Имя пользователя")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string DbUser
        {
            get { return dbuser; }
            set { dbuser = value; }
        }

        [DisplayName("Название")]
        public string Text { get; set; }
    }
}
