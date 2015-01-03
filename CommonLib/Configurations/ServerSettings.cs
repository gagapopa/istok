using System.IO;

namespace COTES.ISTOK
{
    public abstract class ServerSettings : BaseSettings
    {
        //protected ConfigurationProfile log_profile;

        protected ServerSettings() 
        {
            this.RegisterType(typeof(DataBaseSettings));
            this.RegisterType(typeof(VPNSettings));
        }

        public DataBaseSettings DataBase
        {
            get 
            {
                if (!this.Contain("Settings/DB"))
                    this["Settings/DB"] = new DataBaseSettings();

                return (DataBaseSettings)this["Settings/DB"];
            }
            set { this["Settings/DB"] = value; }
        }

        protected abstract string LogConfigName
        {
            get;
        }

        //public ConfigurationProfile Logs
        //{
        //    get
        //    {
        //        if (log_profile == null)
        //            log_profile = new ConfigurationProfile();

        //        return log_profile;
        //    }
        //    set
        //    {
        //        log_profile = value;
        //    }
        //}

        public override void Save(string configPath)
        {
            base.Save(configPath);
            //if (Logs != null)
            //    LoggerManager.WriteConfig(Logs,
            //                             Path.Combine(configPath, LogConfigName));
        }

        public override void Load(string configPath)
        {
            base.Load(configPath);

            string log_config = Path.Combine(configPath, LogConfigName);

            //if (File.Exists(log_config))
            //    Logs = LoggerManager.ReadConfig(log_config);
            //else
            //    Logs = new ConfigurationProfile();
        }

        protected T GetOrDefault<T>(string key, T default_value)
        {
            T result;

            try
            {
                result = (T)this.Get(key);
            }
            catch
            {
                this[key] = default_value;
                result = default_value;
            }

            return result;
        }

        protected T GetOrCreate<T>(string key)
            where T: class, new()
        {
            T result;

            try
            {
                result = (T)this.Get(key);
            }
            catch
            {
                result = new T();
                this[key] = result;
            }

            return result;
        }
    }
}