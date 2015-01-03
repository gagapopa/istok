using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [DataContract]
    public class GlobalNode : UnitNode
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

        public GlobalNode()
        {
            //
        }
        //#region Description for logging
        //public override string DescriptionLog
        //{
        //    get { return "Сервер"; }
        //}
        //#endregion
    }
}
