using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace COTES.ISTOK.Client.Extension
{
    class ClientExtensionManager : MarshalByRefObject
    {
        List<IClientExtension> extensions;

        ClientState state;

        public ClientExtensionManager()
        {
            state = new ClientState(Program.MainForm);
            extensions = new List<IClientExtension>();
            Load();
        }

        public void Load()
        {
            IClientExtension extension;

            const String defaultExtensionPath = "Extension";

            // в отдельном домене перебрать все файлы в указанной папке и составить список доступных расширений
            String extensionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultExtensionPath);

            List<Type> retList = new List<Type>();

            DirectoryInfo info = new DirectoryInfo(extensionPath);

            // получить список типов найденных расширений
            if (info.Exists)
            {
                List<FileInfo> files = new List<FileInfo>(info.GetFiles("*.dll"));
                files.AddRange(info.GetFiles("*.exe"));
                String interfaceName = typeof(IClientExtension).FullName;

                foreach (var item in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(item.FullName);
                        Type[] types = assembly.GetTypes();

                        List<Type> typesList = new List<Type>(types);
                        List<Type> extensionTypes = typesList.FindAll(t => t.GetInterface(interfaceName) != null);

                        foreach (var extensionType in extensionTypes)
                        {
                            if (extensionType.IsPublic)
                                retList.Add(extensionType);
                        }
                    }
                    catch (ReflectionTypeLoadException) { }
                    catch (BadImageFormatException) { }
                }
            }

            // загрузить найденные расширения
            foreach (var extensionType in retList)
            {
                try
                {
                    ConstructorInfo constructor = extensionType.GetConstructor(new Type[0]);

                    if (constructor != null)
                    {
                        extension = constructor.Invoke(null) as IClientExtension;

                        if (extension != null)
                        {
                            extension.State = state;
                            extensions.Add(extension);
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Сбой в работе расширения", exc);
                }
            }
        }

        public String StatusString { get; protected set; }

        public event EventHandler StatusStringChanged;

        void extension_StatusStringChanged(object sender, EventArgs e)
        {
            StringBuilder statusBuilder = new StringBuilder();
           
            foreach (var item in extensions)
                try
                {
                    statusBuilder.Append(item.StatusString);
                }
                catch (Exception exc)
                {
                    throw new Exception("Сбой в работе расширения", exc);
                }

            StatusString = statusBuilder.ToString();

            if (StatusStringChanged != null)
                StatusStringChanged(this, e);
        }

        internal IISTOKMenuItem[] GetMainMenuExt()
        {
            IISTOKMenuItem[] extensionItems;
            List<IISTOKMenuItem> istokItems = new List<IISTOKMenuItem>();
         
            foreach (var extension in extensions)
            {
                try
                {
                        extensionItems = extension.MainMenuExt();
                        if (extensionItems != null)
                            istokItems.AddRange(extensionItems);
                }
                catch (Exception exc)
                {
                    throw new Exception("Сбой в работе расширения", exc);
                }
            }
            return istokItems.ToArray();
        }
    }
}
