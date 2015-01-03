namespace COTES.ISTOK
{
    public class GlobalSettings : ServerSettings
    {
        #region Singleton

        protected new static GlobalSettings instance;

        public new static GlobalSettings Instance // this is barbarians method. singleton is sucks.
        {
            get
            {
                if (instance == null)
                    instance = new GlobalSettings();

                return instance as GlobalSettings;
            }
        }

        protected GlobalSettings()
        {
            this.RegisterType(typeof(BackUpSettings));;
        }

        #endregion

        public override string DefaultConfigFile
        {
            get
            {
                return "dksm_client_settings.conf";
            }
        }

        public const uint DEFAULT_PORT = 8001;

        public uint Port
        {
            get { return (uint)GetOrDefault("Settings/Remoting_port", DEFAULT_PORT); }
            set { this["Settings/Remoting_port"] = value; }
        }

        public const string DEFAULT_HOST = @"localhost";

        public string Host
        {
            get { return (string)GetOrDefault("Settings/Remoting_interface", DEFAULT_HOST); }
            set { this["Settings/Remoting_interface"] = value; }
        }

        public const uint DEFAULT_VALUES_MAX_COUNT = 10000;

        public uint ValuesMaxCount
        {
            get { return (uint)GetOrDefault("Settings/Remoting_maxvalues", DEFAULT_VALUES_MAX_COUNT); }
            set { this["Settings/Remoting_maxvalues"] = value; }
        }

        public BackUpSettings BackUpFull
        {
            get { return GetOrCreate<BackUpSettings>("Settings/BUFull"); }
            set { this["Settings/BUFull"] = value; }
        }

        public BackUpSettings BackUpDiff
        {
            get { return GetOrCreate<BackUpSettings>("Settings/BUDiff"); }
            set { this["Settings/BUDiff"] = value; }
        }

        public const uint DEFAULT_CALCULACTION_INTERVAL = 3600;

        public uint CalculationInterval
        {
            get { return (uint)GetOrDefault("Settings/LoadCalcInterval", DEFAULT_CALCULACTION_INTERVAL); }
            set { this["Settings/LoadCalcInterval"] = value; }
        }

        public const uint DEFAULT_PARAMETERS_CONSTRACTION = 500;

        public uint ParametersConstraction
        {
            get { return (uint)GetOrDefault("Settings/LoadMaxCalcCount", DEFAULT_PARAMETERS_CONSTRACTION); }
            set { this["Settings/LoadMaxCalcCount"] = value; }
        }

        public const uint DEFAULT_LOOP_CONSTRACTION = 1000;

        public uint LoopConstraction
        {
            get { return (uint)GetOrDefault("Settings/LoadMaxLoopCount", DEFAULT_LOOP_CONSTRACTION); }
            set { this["Settings/LoadMaxLoopCount"] = value; }
        }

        public const bool DEFAULT_ALLOW_ROUND_ROBIN_AUTOSTART = true;

        public bool AllowRoundRobinAutostart
        {
            get { return (bool)GetOrDefault("Settings/AllowRoundRobinAutostart", DEFAULT_ALLOW_ROUND_ROBIN_AUTOSTART); }
            set { this["Settings/AllowRoundRobinAutostart"] = value; }
        }

        protected override string LogConfigName
        {
            get { return @"dksm_client_log.conf"; }
        }
    }
}