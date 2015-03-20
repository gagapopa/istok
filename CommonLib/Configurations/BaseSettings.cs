using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Windows.Forms;

namespace COTES.ISTOK
{
    //[XmlElement(ElementName = "Settings")]
    [Serializable]
    public struct InnerSettingItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

    public class BaseSettings
    {
        private List<Type> extra_types = new List<Type>();

        #region Singleton

        protected static Dictionary<string, object> settings = new Dictionary<string, object>();

        static BaseSettings() 
        {
            // empty
            DefaultConfigPath = CreateDefaultConfigPath(
                Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData));
        }

        protected static BaseSettings instance;

        public static BaseSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new BaseSettings();

                return instance;
            }
        }

        protected BaseSettings() 
        { }

        #endregion

        public static string DefaultConfigPath
        {
            get;
            protected set;
        }

        public static string CreateDefaultConfigPath(string base_folder)
        {
            return base_folder +
                   Path.DirectorySeparatorChar + Application.CompanyName +
                   Path.DirectorySeparatorChar + Application.ProductName +
                   Path.DirectorySeparatorChar;
        }

        public virtual string DefaultConfigFile
        {
            get
            {
                return "settings.conf";
            }
        }

        public String ConfigPath { get; set; }

        public String ConfigFilePath
        {
            get
            {
                return Path.Combine(ConfigPath, DefaultConfigFile);
            }
        }


        public void RegisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type can't be null");

            extra_types.Add(type);
        }

        public IEnumerable<Type> ExtraTypes
        {
            get { return extra_types; }
        }

        public void UnregisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type can't be null");

            extra_types.Remove(type);
        }

        public void Add(string key, object value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be a value");
            if (settings.ContainsKey(key))
                throw new InvalidOperationException("Value with same key added before");
            if (value == null)
                throw new ArgumentNullException("Value can't be null");

            settings.Add(key, value);
        }

        public object Get(string key)
        {
            object result;
            if (!this.TryGet(key, out result))
                throw new InvalidOperationException("Same key not found: " + key);

            adminreturn result;
        }

        public bool TryGet(string key, out object value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be a value");

            return settings.TryGetValue(key, out value);
        }

        public bool Contain(string key)
        {
            return settings.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be a value");

            return settings.Remove(key);
        }

        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentException("Key must be a value");
                if (value == null)
                    throw new ArgumentNullException("Value can't be null");

                settings[key] = value;
            }
        }

        public void Save()
        {
            Save(ConfigPath);
        }

        public virtual void Save(string configPath)
        {
            if (String.IsNullOrEmpty(configPath))
                throw new ArgumentException("File name must be a value");

            var pathInfo = new DirectoryInfo(configPath);

            if (!pathInfo.Exists)
                pathInfo.Create();

            ConfigPath = pathInfo.FullName;

            var file_info = new FileInfo(ConfigFilePath);

            using (var stream = file_info.Create())
            {
                var writer = new XmlSerializer(typeof(InnerSettingItem[]), extra_types.ToArray());
                writer.Serialize(stream, GetOutData());
            }
        }

        public virtual void Load(string configPath)
        {
            if (String.IsNullOrEmpty(configPath))
                throw new ArgumentException("File name must be a value");

            ConfigPath = configPath;
            
            if (!File.Exists(ConfigFilePath))
                throw new InvalidOperationException("File is not found");

            using (var stream = File.OpenRead(ConfigFilePath))
            {
                var reader = new XmlSerializer(typeof(InnerSettingItem[]), extra_types.ToArray());
                FillSettings(reader.Deserialize(stream) as InnerSettingItem[]);
            }
        }

        private void FillSettings(InnerSettingItem[] in_data)
        {
            foreach (var it in in_data)
                settings[it.Key] = it.Value;
        }

        private InnerSettingItem[] GetOutData()
        {
            int i = 0;
            InnerSettingItem[] out_data = new InnerSettingItem[settings.Count];
            foreach (var it in settings)
                out_data[i++] = new InnerSettingItem() { Key = it.Key, Value = it.Value };

            return out_data;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return settings.Keys;
            }
        }

        public void Clear()
        {
            settings.Clear();
        }
    }
}