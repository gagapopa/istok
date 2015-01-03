using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using NLog.LogReceiverService;
using NLog.Targets;

namespace COTES.ISTOK.ClientCore
{
    [Target("StationLogReceiverService")]
    public class StationLogReceiverWebServiceTarget : LogReceiverWebServiceTarget
    {
        protected override WcfLogReceiverClient CreateWcfLogReceiverClient()
        {
            String url = this.EndpointAddress;

            url = url.Replace("${host}", ClientSettings.Instance.Host)
                .Replace("${port}", ClientSettings.Instance.Port.ToString());

            EndpointAddress address = new System.ServiceModel.EndpointAddress(url);
            //Binding binding = new NetTcpBinding("NetTcpBinding_ILogReceiverServer");
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            return new WcfLogReceiverClient(binding, address);
        }
    }
}
