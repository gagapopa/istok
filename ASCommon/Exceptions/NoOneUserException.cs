using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Исключение возникающие когда в системе не зарегистрированно ни одного пользователя
    /// </summary>
    [Serializable]
    public class NoOneUserException : ISTOKException
    {
        public NoOneUserException() : this("The system hasn't any user") { }
        public NoOneUserException(string message) : base(message) { }
        //public NoOneUserException(string message, Exception inner) : base(message, inner) { }
        protected NoOneUserException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
