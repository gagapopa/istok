using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Исключение выдается при запросе не существующего узла
    /// </summary>
    [global::System.Serializable]
    public class UnitNodeNotExistException : ISTOKException
    {
        public UnitNodeNotExistException() { }
        public UnitNodeNotExistException(string message) : base(message) { }
        //public UnitNodeNotExistException(string message, Exception inner) : base(message, inner) { }
        protected UnitNodeNotExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
