using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Контейнер для передачи информации о недавних изменених на сервере
    /// </summary>
    [DataContract]
    public class ReturnContainer
    {
        [DataMember]
        public ServiceDataChange[] Changes { get; private set; }

        public ReturnContainer(IEnumerable<ServiceDataChange> changes)
        {
            this.Changes = changes.ToArray();
        }
    }

    /// <summary>
    /// Контейнер для передачи информации о недавних изменених на сервере,
    /// в дополнении к результату операции.
    /// </summary>
    /// <typeparam name="T">Ожидаемый тип результата операции</typeparam>
    [DataContract]
    public class ReturnContainer<T> : ReturnContainer
    {
        /// <summary>
        /// Результат операции
        /// </summary>
        [DataMember]
        public T Result { get; private set; }

        public ReturnContainer(T result, IEnumerable<ServiceDataChange> changes)
            : base(changes)
        {
            this.Result = result;
        }
    }
}
