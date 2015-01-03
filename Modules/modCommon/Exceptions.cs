using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.Modules
{
    [Serializable]
    public class ItemNotFoundException : Exception, ISerializable
    {
        public ItemNotFoundException()
        {

        }

        public ItemNotFoundException(string message)
            : base("ItemNotFoundException(" + message + ")")
        {

        }

        public ItemNotFoundException(string message, Exception e)
            : base(message, e)
        {

        }
        protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //
        }

        #region ISerializable Members
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }
    [Serializable]
    public class ItemIsAlreadyExistException : Exception
    {
        public ItemIsAlreadyExistException()
        {

        }

        public ItemIsAlreadyExistException(string message)
            : base(message)
        {

        }

        public ItemIsAlreadyExistException(string message, Exception e)
            : base(message, e)
        {

        }

    }
    [Serializable]
    public class ServerNotFoundException : Exception
    {
        public ServerNotFoundException()
        {

        }

        public ServerNotFoundException(string message)
            : base("ServerNotFound(" + message + ")")
        {
        }

        public ServerNotFoundException(string message, Exception e)
            : base(message, e)
        {

        }
        protected ServerNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //
        }
        //#region ISerializable Members
        //[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        //void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //}
        //#endregion
    }
    [Serializable]
    public class ServerIsAbsentException : Exception
    {
        public ServerIsAbsentException()
        {
        }
        public ServerIsAbsentException(string message)
            : base("ServerIsAbsent(" + message + ")")
        {
        }
        public ServerIsAbsentException(string message, Exception e)
            : base("ServerIsAbsent(" + message + ")", e)
        {
        }
    }
    [Serializable]
    public class CannotReadTagsException : Exception
    {
        public CannotReadTagsException()
        {

        }

        public CannotReadTagsException(string message)
            : base("CannotReadTags(" + message + ")")
        {
        }

        public CannotReadTagsException(string message, Exception e)
            : base(message, e)
        {

        }

    }
}
