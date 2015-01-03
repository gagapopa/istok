using System;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace WNetConnection
{
    public class NetworkConnection : IDisposable
    {
        string _networkName;

        public NetworkConnection(string networkName,
        NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                credentials.UserName,
                0); //CONNECT_TEMPORARY | CONNECT_COMMANDLINE);

            if (result != 0)
            {
                throw new IOException(String.Format("Error connecting to remote share {0}", result),
                result);
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }
        //[DllImport("mpr.dll", EntryPoint = "WNetAddConnection2W",
        //    CharSet=System.Runtime.InteropServices.CharSet.Unicode)]
        //private static extern int WNetAddConnection2(
        //    ref NetResource lpNetResource, string lpPassword, string lpUsername, Int32 dwFlags);

        private const UInt32 RESOURCETYPE_ANY = 0x0;
        private const UInt32 CONNECT_TEMPORARY = 0x4;
        private const UInt32 CONNECT_INTERACTIVE = 0x00000008;
        private const UInt32 CONNECT_PROMPT = 0x00000010;
        private const UInt32 CONNECT_COMMANDLINE = 0x00000800;

        [DllImport("mpr.dll")]
        private static extern Int32 WNetAddConnection2(NetResource netResource,
        string password, string username, UInt32 flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, Int32 flags,
        bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {

        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public Int32 Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {

        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {

        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {

        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }
}