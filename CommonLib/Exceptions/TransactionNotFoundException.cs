using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;

namespace COTES.ISTOK
{
    [global::System.Serializable]
    public class TransactionNotFoundException : ISTOKException
    {
        public TransactionNotFoundException() { }
        public TransactionNotFoundException(string message) : base(message) { }
        //public TransactionNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TransactionNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
