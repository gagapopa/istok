using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.Client.Extension
{
    class ClientState : MarshalByRefObject, IClientState
    {
        public String ExtensionPath { get; set; }

        public List<String[]> ExtensionList { get; protected set; }

        public void FindExtension()
        {
            List<String[]> retList = new List<String[]>();

            DirectoryInfo info = new DirectoryInfo(ExtensionPath);

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
                                retList.Add(new String[] { assembly.CodeBase, extensionType.FullName });
                        }
                    }
                    catch (BadImageFormatException) { }
                }
            }
            ExtensionList = retList;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        Form mainForm;

        //public RemoteDataService RDS { get { return RemoteDataService.Instance; } }
        StructureProvider strucProvider;

        List<EventHandler> stupidActiveUniFormChanged = new List<EventHandler>();

        public event EventHandler ActiveUniFormChanged
        {
            add
            {
                lock (stupidActiveUniFormChanged)
                {
                    stupidActiveUniFormChanged.Add(value);
                }
            }
            remove
            {
                lock (stupidActiveUniFormChanged)
                {
                    if (stupidActiveUniFormChanged.Contains(value))
                        stupidActiveUniFormChanged.Remove(value);
                }
            }
        }

        protected void OnActiveUniFormChanged()
        {
            lock (stupidActiveUniFormChanged)
            {
                EventArgs e = EventArgs.Empty;
                foreach (var handler in stupidActiveUniFormChanged)
                {
                    try
                    {
                        handler(this, e);
                    }
                    catch { }
                }
            }
        }

        public ClientState(Form mainForm)
        {
            this.mainForm = mainForm;
            this.mainForm.MdiChildActivate += new EventHandler(mainForm_MdiChildActivate);
        }

        void mainForm_MdiChildActivate(object sender, EventArgs e)
        {
            OnActiveUniFormChanged();
        }

        #region IClientState Members

        public IUniForm ActiveUniForm { get { return mainForm.ActiveMdiChild as IUniForm; } }

        public void ShowMdi(Form form)
        {
            form.MdiParent = mainForm;
            form.Show();
        }

        public UTypeNode GetExtensionType(Guid typeGUID)
        {
            var ret = (from t in strucProvider.Session.Types where t.ExtensionGUID.Equals(typeGUID) select t).ToArray();

            if (ret.Length > 0)
                return ret[0];
            return null;

            //return RDS.Types.First(t => t.ExtensionGUID.Equals(typeGUID));
        }

        public UnitNode GetParent(UnitNode unitNode, int unitTypeId)
        {
            return strucProvider.GetParent(unitNode, unitTypeId);
        }

        public ExtensionDataInfo[] GetExtensionExtendedTableInfo(ExtensionUnitNode extensionUnitNode)
        {
            return strucProvider.GetExtensionTableInfo(extensionUnitNode.Idnum);
        }

        public ExtensionDataInfo[] GetExtensionExtendedTableInfo(string extensionName)
        {
            return strucProvider.GetExtensionTableInfo(extensionName);
        }

        public ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, string tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            return strucProvider.GetExtensionExtendedTable(unitNode.Idnum, tabKeyword, dateFrom, dateTo);
        }

        public ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo)
        {
            return strucProvider.GetExtensionExtendedTable(unitNode.Idnum, tabInfo.Name, dateFrom, dateTo);
        }

        public ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, string tabKeyword)
        {
            return strucProvider.GetExtensionExtendedTable(unitNode.Idnum, tabKeyword);
        }

        public ExtensionData GetExtensionExtendedTable(string extensionName, ExtensionDataInfo tabInfo)
        {
            return strucProvider.GetExtensionExtendedTable(extensionName, tabInfo.Name);
        }

        #endregion
    }
}
