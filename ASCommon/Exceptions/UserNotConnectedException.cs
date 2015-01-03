using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Исключение возникает если пользователь не подключен к системе
    /// </summary>
    [global::System.Serializable]
    public class UserNotConnectedException : ISTOKException
    {
        public UserNotConnectedException() : base("UserNotConnected") { }
        public UserNotConnectedException(string message) : base("UserNotConnected " + message) { }
        public UserNotConnectedException(string message, Exception inner) : base("UserNotConnected " + message, inner) { }
        protected UserNotConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        //public UserNotConnectedException(UserNotConnectedFault fault)
        //    :base()
        //{
        //    //
        //}
    }

    //[DataContract]
    //public class UserNotConnectedFault
    //{
    //    public UserNotConnectedFault()
    //    {
    //        //
    //    }
    //    public UserNotConnectedFault(UserNotConnectedException ex)
    //    {
    //        //
    //    }
    //}
}
