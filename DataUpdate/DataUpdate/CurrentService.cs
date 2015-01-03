using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate
{
    class CurrentService
    {
        [Flags]
        public enum ServiceType
        {
            Client = 1,
            Block = 2,
            Global = 4
        }

        public ServiceType CurrentServiceType { get; set; }

        public ServerSettings Settings { get; set; }

        public delegate void MessageWriter(MessageCategory category, String text);

        MessageWriter messageWriter;

        public void RegisterMessageWriter(MessageWriter writer)
        {
            if (messageWriter == null)
                messageWriter = writer;
            else
                messageWriter = (MessageWriter)Delegate.Combine(messageWriter, writer);
        }

        public void Message(MessageCategory category, String text)
        {
            if (messageWriter != null)
                messageWriter(category, text);
        }
    }
}
