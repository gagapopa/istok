using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using NLog;
using NLog.LogReceiverService;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Log service server object that logs messages.
    /// </summary>
    public class LogReceiverServer : ILogReceiverServer
    {
        public void ProcessLogMessages(NLogEvents nevents)
        {
            var events = nevents.ToEventInfo();

            foreach (var ev in events)
            {
                var logger = LogManager.GetLogger(ev.LoggerName);
                logger.Log(ev);
            }
        }
    }
}
