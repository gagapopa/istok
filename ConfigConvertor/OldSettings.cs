using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Net;
using COTES.ISTOK;
using System.Reflection;

namespace ConfigConvertor
{
    /// <summary>
    /// Класс настроек клиента
    /// </summary>
    [XmlInclude(typeof(BackUpSettings))]
    //[XmlInclude(typeof(DiagServerInfo))]
    public class ClientSettings
    {
        private static XmlAttributeOverrides overrides;
        private static object lockFlag = new object();
        private static ClientSettings instance;
        private static ClientSettings dummy = new ClientSettings();
        private static string filename = "";

        private string clientInterface = "";
        private string serverHost = "localhost";
        private int serverPort = 8001;

        private string user = "";

        private NamedObject[] arrObjects = null;
        private FormSettings[] arrForms = null;

        private Dictionary<string, string> dicKeys = null;

        #region Instance
        /// <summary>
        /// Получить экземпляр настроек
        /// </summary>
        [XmlIgnore]
        public static ClientSettings Instance
        {
            get
            {
                lock (lockFlag)
                {
                    if (instance == null || instance == dummy)
                    {
                        try
                        {
                            Init();
                            AddIgnores();
                            using (FileStream fs =
                                new FileStream(filename, FileMode.Open))
                            {
                                System.Xml.Serialization.XmlSerializer xs =
                                    new System.Xml.Serialization.XmlSerializer(typeof(ClientSettings),
                                    overrides);
                                instance = (ClientSettings)xs.Deserialize(fs);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            instance = new ClientSettings();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            instance = new ClientSettings();
                        }
                        catch (IOException)
                        {
                            instance = dummy;
                        }
                        catch (Exception)
                        {
                            instance = new ClientSettings();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// Путь к файлу настроек
        /// </summary>
        [XmlIgnore]
        public static string FileName
        {
            get { Init(); return filename; }
            set
            {
                filename = value;
                Clear();
            }
        }
        /// <summary>
        /// Очистить настройки
        /// </summary>
        public static void Clear()
        {
            instance = null;
        }
        /// <summary>
        /// Сохранение настроек
        /// </summary>
        public void Save()
        {
            if (instance != dummy)
            {
                FileInfo file = new FileInfo(filename);
                if (!file.Directory.Exists)
                    file.Directory.Create();

                using (FileStream fs = file.Create())
                {
                    System.Xml.Serialization.XmlSerializer xs =
                        new System.Xml.Serialization.XmlSerializer(typeof(ClientSettings),
                        overrides);
                    xs.Serialize(fs, instance);
                }
            }
        }
        /// <summary>
        /// Назначить настройки
        /// </summary>
        /// <param name="settings"></param>
        public static void NewSettings(ClientSettings settings)
        {
            instance = settings;
        }
        #endregion

        #region BrowsableProperies
        /// <summary>
        /// Имя сервера
        /// </summary>
        [Browsable(true)]
        [Category("Подключение")]
        [DisplayName("Имя сервера")]
        [DefaultValue("localhost")]
        public string ServerHost
        {
            get { return serverHost; }
            set { serverHost = value; }
        }
        /// <summary>
        /// Порт сервера
        /// </summary>
        [Browsable(true)]
        [Category("Подключение")]
        [DisplayName("Порт сервера")]
        [DefaultValue(8001)]
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
        /// <summary>
        /// Интерфейс
        /// </summary>
        [Browsable(true)]
        [Category("Подключение")]
        [DisplayName("Интерфейс")]
        //[TypeConverter(typeof(InterfaceTypeConverter))]
        public string ClientInterface
        {
            get
            {
                if (!string.IsNullOrEmpty(clientInterface)) return clientInterface;

                IPHostEntry local = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ipaddress in local.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        clientInterface = ipaddress.ToString();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(clientInterface)) clientInterface = "127.0.0.1";
                return clientInterface;
            }
            set { clientInterface = value; }
        }
        #endregion
        #region Properties
        /// <summary>
        /// Пустышка (нет реального файла настроек)
        /// </summary>
        [Browsable(false)]
        public bool IsDummy
        {
            get { return instance == dummy; }
        }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Browsable(false)]
        public string User
        {
            get { return user; }
            set { user = value; }
        }
        /// <summary>
        /// Массив настроек форм
        /// </summary>
        [Browsable(false)]
        public FormSettings[] Forms
        {
            get { return arrForms; }
            set { arrForms = value; }
        }
        /// <summary>
        /// Массив объектов настроек
        /// </summary>
        [Browsable(false)]
        public NamedObject[] Objects
        {
            get { return arrObjects; }
            set { arrObjects = value; }
        }
        /// <summary>
        /// Таблица ключевых значений
        /// </summary>
        [Browsable(false)]
        public DataTable Keys
        {
            get
            {
                DataTable table = new DataTable("KeyTable");
                string val;

                table.Columns.Add("Key");
                table.Columns.Add("Value");
                if (dicKeys == null) return table;
                foreach (string item in dicKeys.Keys)
                {
                    if (dicKeys.TryGetValue(item, out val))
                    {
                        table.Rows.Add(item, val);
                    }
                }
                return table;
            }
            set
            {
                DataTable table = value;
                string key, val;

                dicKeys = new Dictionary<string, string>();

                if (table == null) return;
                foreach (DataRow item in table.Rows)
                {
                    key = item["Key"].ToString();
                    val = item["Value"].ToString();
                    if (!string.IsNullOrEmpty(key) && val != null)
                    {
                        dicKeys.Add(key, val);
                    }
                }
            }
        }
        /// <summary>
        /// Словарь ключевых значений
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public IDictionary Properties
        {
            get
            {
                var res = new Dictionary<string, string>();

                if (dicKeys != null) foreach (var item in dicKeys.Keys)
                    {
                        res.Add(item, dicKeys[item]);
                    }

                return res;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает копию объекта
        /// </summary>
        /// <returns></returns>
        public ClientSettings Clone()
        {
            return (ClientSettings)instance.MemberwiseClone();
        }
        /// <summary>
        /// Сохранить настройки формы
        /// </summary>
        /// <param name="form">Форма, настройки которой нужно сохранить</param>
        public void SaveFormState(Form form)
        {
            FormSettings formSettings;

            if (form == null) return;

            formSettings = FindForm(form.Name);

            if (formSettings == null)
            {
                formSettings = new FormSettings(form);
                AddForm(formSettings);
            }
            else
            {
                formSettings.SetForm(form);
            }
        }
        /// <summary>
        /// Загрузить настройки формы
        /// </summary>
        /// <param name="form">Форма, настройки которой нужно загрузить</param>
        public void LoadFormState(Form form)
        {
            if (arrForms == null || form == null) return;

            foreach (FormSettings item in arrForms)
            {
                if (item.Name == form.Name)
                {
                    item.SetupForm(form);
                    break;
                }
            }
        }
        /// <summary>
        /// Добавление параметров в настройки
        /// </summary>
        /// <param name="key">Имя параметра</param>
        /// <param name="value">Значение</param>
        public void SaveKey(string key, string value)
        {
            string tmp;

            if (dicKeys == null)
            {
                dicKeys = new Dictionary<string, string>();
                dicKeys.Add(key, value);
                return;
            }

            if (!dicKeys.TryGetValue(key, out tmp))
                dicKeys.Add(key, value);
            else
                dicKeys[key] = value;
        }
        /// <summary>
        /// Получение значения параметра по имени параметра
        /// </summary>
        /// <param name="key">Имя параметра</param>
        /// <returns>Значение</returns>
        public string LoadKey(string key)
        {
            string val = "";

            if (dicKeys == null) return "";

            dicKeys.TryGetValue(key, out val);

            if (val == null) val = "";
            return val;
        }
        /// <summary>
        /// Сохраняет объект под указанным именем
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void SaveObject(string name, object obj)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return;

            AddObject(name, obj);
        }
        /// <summary>
        /// Загружает объект
        /// </summary>
        /// <param name="name">Имя, под которым сохранен объект</param>
        /// <returns></returns>
        public object LoadObject(string name)
        {
            return FindObject(name);
        }
        #endregion
        #region PrivateMethods
        /// <summary>
        /// Добавление объекта в настройки
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="form">Объект настроек</param>
        private void AddObject(string name, object obj)
        {
            int i;

            if (arrObjects == null)
                arrObjects = new NamedObject[] { };

            foreach (NamedObject item in arrObjects)
            {
                if (item.Name == name)
                {
                    item.Object = obj;
                    return;
                }
            }

            NamedObject[] n_arrObjects = new NamedObject[arrObjects.Length + 1];
            for (i = 0; i < arrObjects.Length; i++)
                n_arrObjects[i] = arrObjects[i];
            n_arrObjects[i] = new NamedObject();
            n_arrObjects[i].Name = name;
            n_arrObjects[i].Object = obj;
            arrObjects = n_arrObjects;
        }
        /// <summary>
        /// Поиск объекта настроек
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <returns></returns>
        private object FindObject(string name)
        {
            if (arrObjects == null) return null;

            foreach (NamedObject item in arrObjects)
            {
                if (item.Name == name)
                {
                    return item.Object;
                }
            }

            return null;
        }

        /// <summary>
        /// Добавление настроек формы
        /// </summary>
        /// <param name="form">Настройки формы</param>
        private void AddForm(FormSettings form)
        {
            int i;

            if (arrForms == null)
                arrForms = new FormSettings[] { };

            FormSettings[] n_arrForms = new FormSettings[arrForms.Length + 1];
            for (i = 0; i < arrForms.Length; i++)
                n_arrForms[i] = arrForms[i];
            n_arrForms[i] = form;
            arrForms = n_arrForms;
        }
        /// <summary>
        /// Поиск настроек формы
        /// </summary>
        /// <param name="name">Имя формы</param>
        /// <returns></returns>
        private FormSettings FindForm(string name)
        {
            if (arrForms == null) return null;

            foreach (FormSettings item in arrForms)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }
        /// <summary>
        /// Заполнение настроек сериализации
        /// </summary>
        private static void AddIgnores()
        {
            XmlAttributes attr;
            XmlAttributeAttribute xmlAttribute;

            overrides = new XmlAttributeOverrides();
            Type type = typeof(TreeNode);

            foreach (PropertyInfo item in type.GetProperties())
            {
                attr = new XmlAttributes();

                if (item.Name != "Text" &&
                    item.Name != "Nodes")
                {
                    attr.XmlIgnore = true;
                }
                else
                {
                    if (item.Name == "Text")
                    {
                        xmlAttribute = new XmlAttributeAttribute(item.Name);
                        attr.XmlAttribute = xmlAttribute;
                    }
                }
                overrides.Add(type, item.Name, attr);
            }
        }
        private static void Init()
        {
            if (string.IsNullOrEmpty(filename))
                filename = Application.StartupPath +
                                    "\\config.xml";
        }
        #endregion
    }

    /// <summary>
    /// Класс для сопоставления объектов с их именованием
    /// </summary>
    public class NamedObject
    {
        private string name;
        private object obj;

        /// <summary>
        /// Имя объекта
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// Объект
        /// </summary>
        public object Object
        {
            get { return obj; }
            set { obj = value; }
        }
    }

    /// <summary>
    /// Класс настроек для форм
    /// </summary>
    public class FormSettings
    {
        private string name;
        private int pos_x, pos_y;
        private int size_w, size_h;
        private FormWindowState state;
        private TreeState[] nodes;

        public string Name { get { return name; } set { name = value; } }
        [DefaultValue(0)]
        public int X { get { return pos_x; } set { pos_x = value; } }
        [DefaultValue(0)]
        public int Y { get { return pos_y; } set { pos_y = value; } }
        [DefaultValue(0)]
        public int Width { get { return size_w; } set { size_w = value; } }
        [DefaultValue(0)]
        public int Height { get { return size_h; } set { size_h = value; } }
        public FormWindowState WindowState { get { return state; } set { state = value; } }
        public TreeState[] TreeStates { get { return nodes; } set { nodes = value; } }

        public FormSettings()
        {
            name = "";
            pos_x = pos_y = 0;
            size_h = size_w = 0;
            state = FormWindowState.Normal;
            nodes = null;
        }
        public FormSettings(Form form)
            : this()
        {
            SetForm(form);
        }

        /// <summary>
        /// Получить настройки из указанной формы
        /// </summary>
        /// <param name="form">Форма, из которой берутся настройки</param>
        public void SetForm(Form form)
        {
            if (form == null) return;

            name = form.Name;
            state = form.WindowState;
            if (state == FormWindowState.Normal)
            {
                pos_x = form.Left;
                pos_y = form.Top;
                size_w = form.Width;
                size_h = form.Height;
            }
        }
        /// <summary>
        /// Настроить форму
        /// </summary>
        /// <param name="form">Форма, которую нужно настроить</param>
        public void SetupForm(Form form)
        {
            if (form == null) return;

            if (size_w != 0) form.Width = size_w;
            if (size_h != 0) form.Height = size_h;
            if (form.MdiParent == null || form.MdiParent.ActiveMdiChild == null)
                form.WindowState = state;
        }
        public void AddState(TreeState state)
        {
            TreeState[] n_nodes;
            int i;

            if (nodes == null)
            {
                nodes = new TreeState[] { state };
            }
            else
            {
                n_nodes = new TreeState[nodes.Length + 1];
                for (i = 0; i < nodes.Length; i++)
                {
                    n_nodes[i] = nodes[i];
                }
                n_nodes[i] = state;
                nodes = n_nodes;
            }
        }
    }

    /// <summary>
    /// Класс с настройками дерева
    /// </summary>
    public class TreeState
    {
        private TreeNode node = null;
        private string selected = "";

        public TreeNode Node { get { return node; } set { node = value; } }
        public string SelectedNode { get { return selected; } set { selected = value; } }

        public TreeState(string text)
        {
            node = new TreeNode(text);
        }
        public TreeState()
            : this("")
        {
        }
    }
}
