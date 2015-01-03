using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace COTES.ISTOK
{
    [Serializable]
    public class ServerNotAccessibleException : ISTOKException, ISerializable
    {
        public ServerNotAccessibleException() : base() { }
        public ServerNotAccessibleException(String message) : base(message) { }
        //public ServerNotAccessibleException(String message, Exception innerException) : base(message, innerException) { }

        protected ServerNotAccessibleException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region ISerializable Members
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
