using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK
{
    [global::System.Serializable]
    public class ISTOKException : Exception
    {
        //public Exception Exception { get; set; }

        public ISTOKException() { }
        public ISTOKException(string message) : base(message) { }
        public ISTOKException(string message, Exception inner) : base(message, inner) { }
        protected ISTOKException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
