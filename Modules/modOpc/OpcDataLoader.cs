using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Opc;

namespace COTES.ISTOK.Modules.modOpc
{
    public abstract class OpcDataLoader
    {
        protected Logger log;
        protected String channelLogPrefix = String.Empty;

        protected Opc.Server[] opcServers = null;

        protected String m_TagCatalog;
        protected String m_ConnectionType;

        protected List<ParameterItem> parameters;

        public bool KeepConnected { get; set; }

        protected abstract void Connect();

        protected abstract void Disconnect();

        public ParameterItem[] GetParameters()
        {
            log.Trace("{0}GetParameters()", channelLogPrefix);

            List<ParameterItem> parmeterList = new List<ParameterItem>();

            TreeNode node;
            string[] arrNames;

            int i;

            try
            {
                Connect();

                node = GetTagNames(new TreeNode(m_TagCatalog));
                arrNames = TreeToArray(node);

                for (i = 0; i < arrNames.Length; i++)
                {

                    ParameterItem parameter = new ParameterItem()
                    {
                        Name = arrNames[i]
                    };
                    parameter[CommonProperty.ParameterCodeProperty] = arrNames[i];

                    parmeterList.Add(parameter);
                }
            }
            finally
            {
                if (!KeepConnected)
                    Disconnect();
            }
            return parmeterList.ToArray();
        }

        protected abstract TreeNode GetTagNames(TreeNode treeNode);

        private string[] TreeToArray(TreeNode nNode)
        {
            log.Trace("{0}TreeToArray({1})", channelLogPrefix, nNode);

            List<string> lstItems = new List<string>();
            string[] strItems = null;
            int i, j;

            if (nNode == null) throw new ArgumentNullException("nNode");

            for (i = 0; i < nNode.Nodes.Count; i++)
            {
                if (nNode.Nodes[i].Nodes.Count > 0)
                {
                    strItems = TreeToArray(nNode.Nodes[i]);

                    for (j = 0; j < strItems.Length; j++)
                    {
                        if (!String.IsNullOrEmpty(strItems[j])) lstItems.Add(strItems[j]);
                    }
                }
                else
                {
                    lstItems.Add((string)nNode.Nodes[i].Tag);
                }
            }

            strItems = new string[lstItems.Count];

            for (i = 0; i < lstItems.Count; i++)
            {
                strItems[i] = lstItems[i];
            }

            return strItems;
        }

        protected Opc.Server FindServer(string strHost, string strServer)
        {
            log.Trace("{0}FindServer({0}, {1})", channelLogPrefix, strHost, strServer);

            int i;

            if (string.IsNullOrEmpty(strServer))
                throw new ArgumentNullException("null:strServer");

            try
            {
                opcServers = GetAvailableServers(strHost);
                if (opcServers == null) throw new ServerNotFoundException();
                for (i = 0; i < opcServers.Length; i++)
                {
                    if (opcServers[i].Name.ToLower() == strServer.ToLower())
                    {
                        return (Opc.Server)opcServers[i];
                    }
                }
            }
            catch (Exception exc)
            {
                log.ErrorException("Ошибка при подключении к серверу OPC", exc);
                throw new ServerNotFoundException(strServer);
            }

            return null;
        }
        /// <summary>
        /// GetAvailableServers
        /// </summary>
        /// <exception cref="ServerIsAbsentException"></exception>
        private Opc.Server[] GetAvailableServers(string strHost)
        {
            log.Trace("{0}GetAvailableServers({1})", channelLogPrefix, strHost);

            OpcCom.ServerEnumerator ServEnum = new OpcCom.ServerEnumerator();
            ConnectData cnData = new ConnectData(null, null);
            Opc.Server[] arrServers = null;
            //string[] strCleanServers = null;
            //int i, j = 0;

            //opcServers = null;     
            arrServers = ServEnum.GetAvailableServers(GetSpecification(), strHost, cnData);

            if (arrServers == null) throw new ServerIsAbsentException("OpcServers");

            /*strServers = new string[opcServers.Length];

            for (i = 0; i < opcServers.Length; i++)
            {
                if (opcServers[i] == null)
                {
                    j++;
                    continue;
                }

                strServers[i - j] = opcServers[i].Name;
            }

            if (j > 0)
            {
                strCleanServers = new string[opcServers.Length - j];

                for (i = 0; i < opcServers.Length - j; i++)
                {
                    strCleanServers[i] = strServers[i];
                }
            }
            else
                strCleanServers = strServers;

            return strCleanServers;*/
            return arrServers;
        }

        /// <summary>
        /// GetSpecification
        /// </summary>
        /// <returns></returns>
        private Opc.Specification GetSpecification()
        {
            log.Trace("{0}GetSpecification()", channelLogPrefix);

            Opc.Specification DASpecification;

            switch (m_ConnectionType.ToUpper())
            {
                case OpcDataLoaderFactory.ConnectionTypeOPCDA_10:
                    DASpecification = Opc.Specification.COM_DA_10;
                    break;
                case OpcDataLoaderFactory.ConnectionTypeOPCDA_20:
                    DASpecification = Opc.Specification.COM_DA_20;
                    break;
                case OpcDataLoaderFactory.ConnectionTypeOPCDA_30:
                    DASpecification = Opc.Specification.COM_DA_30;
                    break;
                case OpcDataLoaderFactory.ConnectionTypeOPCHDA:
                    DASpecification = Opc.Specification.COM_HDA_10;
                    break;
                default:
                    throw new WrongSpecificationException(m_ConnectionType);
            }

            return DASpecification;
        }
    }
}
