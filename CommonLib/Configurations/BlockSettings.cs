using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    public class BlockSettings : ServerSettings
    {
        #region Singleton

        protected new static BlockSettings instance;

        public new static BlockSettings Instance // this is not threadsafe and barbariam method
        {
            get
            {
                if (instance == null)
                    instance = new BlockSettings();

                return instance as BlockSettings;
            }
        }

        protected BlockSettings()
        {
            this.RegisterType(typeof(MaintenanceSettings));
        }

        #endregion

        public override string DefaultConfigFile
        {
            get
            {
                return "dksm_server_settings.conf";
            }
        }

        public const string DEFAULT_SERVER_NAME = @"Server1";

        public string ServerName
        {
            get { return (string)GetOrDefault("Settings/UniqueServerName", DEFAULT_SERVER_NAME); }
            set { this["Settings/UniqueServerName"] = value; }
        }

        public const string DEFAULT_MODULE_PATH = @"Modules";

        public string ModulesPath
        {
            get { return (string)GetOrDefault("Settings/LoadersPath", DEFAULT_MODULE_PATH); }
            set { this["Settings/LoadersPath"] = value; }
        }

        public const uint DEFAULT_PORT = 8082;

        public uint Port
        {
            get { return (uint)GetOrDefault("Settings/Remoting_port", DEFAULT_PORT); }
            set { this["Settings/Remoting_port"] = value; }
        }

        public const uint DEFAULT_MAX_VALUES_COUNT = 1000000;

        public uint MaxValuesCount
        {
            get { return (uint)GetOrDefault("Settings/Remoting_maxvalues", DEFAULT_MAX_VALUES_COUNT); }
            set { this["Settings/Remoting_maxvalues"] = value; }
        }

        public const string DEFAULT_INTERFACE = @"localhost";

        public string Interface
        {
            get { return (string)GetOrDefault("Settings/Remoting_interface", DEFAULT_INTERFACE); }
            set { this["Settings/Remoting_interface"] = value; }
        }

        public const string DEFAULT_GLOBAL_SERVER_HOST = @"localhost";

        public string GlobalServerHost
        {
            get { return (string)GetOrDefault("Settings/GlobalHost", DEFAULT_GLOBAL_SERVER_HOST); }
            set { this["Settings/GlobalHost"] = value; }
        }

        public uint DEFAULT_GLOBAL_PORT = 8081;

        public uint GlobalServerPort
        {
            get { return (uint)GetOrDefault("Settings/GlobalPort", DEFAULT_GLOBAL_PORT); }
            set { this["Settings/GlobalPort"] = value; }
        }

        public string TelnetHost
        {
            get { return (string)GetOrDefault("Diagnostics/Telnet", String.Empty); }
            set { this["Diagnostics/Telnet"] = value; }
        }

        public string RouterIp
        {
            get { return (string)GetOrDefault("Settings/RouterIp", String.Empty); }
            set { this["Settings/RouterIp"] = value; }
        }

        public MaintenanceSettings Maintenance
        {
            get { return GetOrCreate<MaintenanceSettings>("Settings/Maintenance"); }
            set { this["Settings/Maintenance"] = value; }
        }

        public VPNSettings Vpn
        {
            get { return GetOrCreate<VPNSettings>("Settings/VPN"); }
            set { this["Settings/VPN"] = value; }
        }

        public string MirroringBackupPath
        {
            get { return (string)GetOrDefault("Settings/MirroringBackupPath", String.Empty); }
            set { this["Settings/MirroringBackupPath"] = value; }
        }

        public const uint DEFAULT_MIRRORING_PORT = 7070;

        public uint MirroringPort
        {
            get { return (uint)GetOrDefault("Settings/MirroringPort", DEFAULT_MIRRORING_PORT); }
            set { this["Settings/MirroringPort"] = value; }
        }

        public string ReplicationUser
        {
            get { return (string)GetOrDefault("Settings/ReplicationUser", String.Empty); }
            set { this["Settings/ReplicationUser"] = value; }
        }

        public string ReplicationPassword
        {
            get { return (string)GetOrDefault("Settings/ReplicationPassword", String.Empty); }
            set { this["Settings/ReplicationPassword"] = value; }
        }

        public const byte DEFAULT_SERVER_PRIORITY = 5;

        public byte ServerPriority
        {
            get { return (byte)GetOrDefault("Settings/ServerPriority", DEFAULT_SERVER_PRIORITY); }
            set { this["Settings/ServerPriority"] = value; }
        }

        protected override string LogConfigName
        {
            get { return @"dksm_server_log.conf"; }
        }
    }
}
