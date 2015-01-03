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
    public class BlockDiag
    {
        [DisplayName("Компьютер")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string Host { get; set; }
        [DisplayName("Порт")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string Port { get; set; }

        [DisplayName("Идентификатор"), Description("Идентификатор сервера сбора данных.")]
        [Category("Сервер сбора")]
        public string BlockUID { get; set; }
    }
}
