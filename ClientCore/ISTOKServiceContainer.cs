using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.TypeConverters;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.ClientCore
{
    /// <summary>
    /// Контэйнер интерфейсов
    /// </summary>
    //TODO: переделать контейнер на асинхронную работу с тасками
    public class ISTOKServiceContainer : ISite,
        IUnitTypeRetrieval,
        IOwnerRetrieval,
        IBlockUIDRetrieval,
        IModuleLibNameRetrieval,
        IChannelRetrieval,
        IScheduleRetrivial,
        IAttributeRetrieval,
        IExternalsSupplier,
        IStructureRetrieval,
        IIntervalSupplier
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        RemoteDataService rds;
        //Guid userGuid;
        UTypeNode[] arrUnitTypes = null;

        internal ISTOKServiceContainer(RemoteDataService rds)
        {
            this.rds = rds;
            //this.userGuid = userGuid;
            arrUnitTypes = rds.NodeDataService.GetUnitTypes();
        }

        public event Action<Exception> ErrorOcuired;

        private void OnError(Exception exc)
        {
            if (ErrorOcuired != null)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() => ErrorOcuired(exc));
            }
        }

        #region ISite Members

        public IComponent Component
        {
            get { return null; }
        }

        public IContainer Container
        {
            get { return null; }
        }

        public bool DesignMode
        {
            get { return false; }
        }

        public string Name
        {
            get;
            set;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType.IsInstanceOfType(this)) return this;
            return null;
        }

        #endregion

        #region IUnitTypeRetrieval Members

        int[] IUnitTypeRetrieval.GetUnitTypes(int parentNodeID)
        {
            try
            {
                List<int> retList = new List<int>();
                UTypeNode[] uTypeNodes = arrUnitTypes;//rds.NodeDataService.GetUnitTypes();
                //UTypeNode[] uTypeNodes = rds.Types;
                UnitNode unitNode = rds.NodeDataService.GetUnitNode(parentNodeID);
                int unitType;
                UTypeNode parentType;

                if (unitNode != null)
                    unitType = unitNode.Typ;
                else unitType = (int)UnitTypeId.Unknown;
                parentType = new List<UTypeNode>(uTypeNodes).Find(x => x.Idnum == (int)unitType);

                UserNode user = rds.UserDataService.GetUser();
                if (user != null)
                {
                    foreach (UTypeNode unitTypeNode in uTypeNodes)
                    {
                        if ((user.IsAdmin
                            || user.CheckPrivileges(unitTypeNode.Idnum, Privileges.Write))
                            && (parentType == null || parentType.ChildFilterAll || parentType.ChildFilter.Contains(unitTypeNode.Idnum)))
                            retList.Add(unitTypeNode.Idnum);
                    }
                }
                return retList.ToArray();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса типов элемнтов структуры.", exc);
                OnError(exc);
                return null;
            }
        }

        Dictionary<int, String> IUnitTypeRetrieval.GetUnitTypeLocalization(System.Globalization.CultureInfo culture)
        {
            Dictionary<int, String> retDictionary = new Dictionary<int, String>();
            UTypeNode[] uTypeNodes = arrUnitTypes;//rds.NodeDataService.GetUnitTypes();
            
            retDictionary[(int)UnitTypeId.Unknown] = "<Нет типа>";
            foreach (UTypeNode unitTypeNode in uTypeNodes)
                retDictionary[unitTypeNode.Idnum] = unitTypeNode.Text;
            
            return retDictionary;
        }

        UTypeNode IUnitTypeRetrieval.GetUnitTypeNode(int unitType)
        {
            UTypeNode[] uTypeNodes = arrUnitTypes;//rds.NodeDataService.GetUnitTypes();
            
            if (uTypeNodes != null)
                return uTypeNodes.FirstOrDefault(t => t.Idnum == (int)unitType);

            return null;
        }


        #endregion

        #region IOwnerRetrieval Members

        int IOwnerRetrieval.GetCurrentUser()
        {
            var user = rds.UserDataService.GetConnectedUser();
            return -user.Idnum;
        }

        IEnumerable<int> IOwnerRetrieval.GetGroups()
        {
            try
            {
                var user = rds.UserDataService.GetConnectedUser();

                GroupNode[] groups = rds.UserDataService.GetGroupNodes();
                List<int> groupIds = new List<int>();
                foreach (GroupNode groupNode in groups)
                {
                    if (user.IsAdmin
                        || user.CheckGroupPrivilegies(groupNode.Idnum, Privileges.Write))
                        groupIds.Add(groupNode.Idnum);
                }

                return groupIds.ToArray();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса групп пользователей.", exc);
                OnError(exc);
                return null;
            }
        }

        IEnumerable<int> IOwnerRetrieval.GetUsers()
        {
            throw new NotImplementedException();
        }

        string IOwnerRetrieval.GetOwnerLocalization(int ownerID, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (ownerID == 0) return "общий";
                else if (ownerID == -rds.UserDataService.GetConnectedUser().Idnum) return "личный";
                else if (ownerID > 0)
                {
                    List<GroupNode> groups = new List<GroupNode>(rds.UserDataService.GetGroupNodes());
                    GroupNode group = groups.Find(x => x.Idnum.Equals(ownerID));
                    if (group != null) return group.Text;
                }
                else
                {
                    String login = rds.UserDataService.GetUserLogin(ownerID);
                    if (!String.IsNullOrEmpty(login)) return "личный:" + login;
                }
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса данных пользователей.", exc);
                OnError(exc);
            }

            return ownerID.ToString();
        }

        #endregion

        #region IBlockUIDRetrieval Members

        string[] IBlockUIDRetrieval.GetBlockUIDs()
        {
            try
            {
                return rds.BlockDataService.GetBlockUIDs();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса доступных серверов сбора.", exc);
                OnError(exc);
                return new String[] { };
            }
        }

        #endregion

        #region IModuleLibNameRetrieval Members

        public ModuleInfo[] GetModuleLibNames(BlockNode blockNode)
        {
            try
            {
                return rds.BlockDataService.GetModuleLibNames(blockNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса доступных модулей сбора.", exc);
                OnError(exc);
                return null;
            }
        }

        BlockNode IModuleLibNameRetrieval.GetBlockNode(ChannelNode channel)
        {
            return GetBlockNode(channel);
        }

        #endregion

        #region IChannelRetrieval Members

        public IEnumerable<ItemProperty> GetProperties(BlockNode blockNode, string libName)
        {
            try
            {
                return rds.BlockDataService.GetModuleLibraryProperties(blockNode.Idnum, libName);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента.", exc);
                OnError(exc);
                return null;
            }
        }

        BlockNode IChannelRetrieval.GetBlockNode(ChannelNode channel)
        {
            return GetBlockNode(channel);
        }

        #endregion

        #region IScheduleRetrivial Members

        Schedule[] IScheduleRetrivial.GetSchedules()
        {
            //ManualResetEvent ev = new ManualResetEvent(false);
            //AsyncOperationWatcher watcher = rds.QueryParamsSchedules();
            //Schedule[] ret = null;

            //watcher.AddValueRecivedHandler(x => ret = x as Schedule[]);
            //watcher.AddFinishHandler(() => ev.Set());
            //watcher.Run();

            //ev.WaitOne();
            try
            {
                return rds.ScheduleDataService.GetParamsSchedules();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса расписаний.", exc);
                OnError(exc);
                return null;
            }
        }

        Schedule IScheduleRetrivial.GetSchedule(int id)
        {
            //ManualResetEvent ev = new ManualResetEvent(false);
            //AsyncOperationWatcher watcher = rds.QueryParamsSchedule(id);
            //Schedule ret = null;

            //watcher.AddValueRecivedHandler(x => ret = x as Schedule);
            //watcher.AddFinishHandler(() => ev.Set());
            //watcher.Run();

            //ev.WaitOne();
            try
            {
                return rds.ScheduleDataService.GetParamsSchedule(id);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса расписаний.", exc);
                OnError(exc);
                return null;
            }
        }

        Schedule IScheduleRetrivial.GetSchedule(string name)
        {
            //ManualResetEvent ev = new ManualResetEvent(false);
            //AsyncOperationWatcher watcher = rds.QueryParamsSchedule(name);
            //Schedule ret = null;

            //watcher.AddValueRecivedHandler(x => ret = x as Schedule);
            //watcher.AddFinishHandler(() => ev.Set());
            //watcher.Run();

            //ev.WaitOne();
            try
            {
                return rds.ScheduleDataService.GetParamsSchedule(name);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса расписаний.", exc);
                OnError(exc);
                return null;
            }
        }

        #endregion

        #region IAttributeRetrieval Members

        public ItemProperty[] GetProperties(UnitNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            foreach (var type in arrUnitTypes)
                if (node.Typ == type.Idnum)
                    return type.GetProperties();

            return null;
        }

        #endregion

        private BlockNode GetBlockNode(ChannelNode channelNode)
        {
            UnitNode unitNode = channelNode;
            BlockNode ret = null;

            try
            {
                while (unitNode != null && (ret = unitNode as BlockNode) == null)
                    if (unitNode.ParentId != 0)
                        unitNode = rds.NodeDataService.GetUnitNode(unitNode.ParentId);
                    else
                        break;
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса элементов структуры.", exc);
                OnError(exc);
            }

            return ret;
        }

        #region IExternalsSupplier Members

        public bool ExternalCodeSupported(ExtensionUnitNode unitNode)
        {
            try
            {
                return rds.ExtensionDataService.ExternalCodeSupported(unitNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента.", exc);
                OnError(exc);
                return false;
            }
        }

        public bool ExternalIDSupported(ExtensionUnitNode unitNode)
        {
            try
            {
                return rds.ExtensionDataService.ExternalIDSupported(unitNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента.", exc);
                OnError(exc);
                return false;
            }
        }

        public bool ExternalIDCanAdd(ExtensionUnitNode unitNode)
        {
            try
            {
                return rds.ExtensionDataService.ExternalIDCanAdd(unitNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента.", exc);
                OnError(exc);
                return false;
            }
        }

        public EntityStruct[] GetExternalIDList(ExtensionUnitNode unitNode)
        {
            try
            {
                return rds.ExtensionDataService.ExternalIDList(unitNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента.", exc);
                OnError(exc);
                return null;
            }
        }

        public ItemProperty[] GetExternalProperties(ExtensionUnitNode unitNode)
        {
            try
            {
                return rds.ExtensionDataService.GetExternalProperties(unitNode.Idnum);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса свойств элемента элемента.", exc);
                OnError(exc);
                return null;
            }
        }

        #endregion

        #region IStructureRetrieval Members

        public UnitNode GetUnitNode(int unitNodeID)
        {
            try
            {
                return rds.NodeDataService.GetUnitNode(unitNodeID);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса элементов структуры.", exc);
                OnError(exc);
                return null;
            }
        }

        public UnitNode[] GetChildNodes(int parentId, int[] typeFilter, int minLevel, int maxLevel)
        {
            try
            {
                return rds.NodeDataService.GetAllUnitNodes(parentId, typeFilter, minLevel, maxLevel);
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса элементов структуры.", exc);
                OnError(exc);
                return null;
            }
        }

        #endregion

        #region IIntervalSupplier Members

        IEnumerable<IntervalDescription> intervals;

        public IEnumerable<Interval> GetStandardIntervals()
        {
            try
            {
            if (intervals == null)
            {
                intervals = rds.IntervalDataService.GetStandardsIntervals();
            }

            return from d in intervals where d.IsStandard orderby d.interval select d.interval;
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса стандартных интервалов.", exc);
                OnError(exc);
                return new Interval[] { };
            }

            //return from t in namedIntervals select Interval.FromString(t.Item1);
        }

        public string GetIntervalHeader(Interval interval)
        {
            String header = null;
            String intervalString = interval.ToString();

            try
            {
            if (intervals == null)
            {
                intervals = rds.IntervalDataService.GetStandardsIntervals();
            }
            //String header = (from t in namedIntervals
            //                 where t.Item1 == intervalString
            //                 select t.Item2).FirstOrDefault();

                header = (from d in intervals
                             where d.interval.ToString() == intervalString
                             select d.Header).FirstOrDefault();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса стандартных интервалов.", exc);
                OnError(exc);
            }

            return header ?? intervalString;
        }

        public Interval GetIntervalByHeader(string intervalHeader)
        {
            Interval interval = null;

            try
            {
                if (intervals == null)
                {
                    intervals = rds.IntervalDataService.GetStandardsIntervals();
                }

                interval = (from d in intervals
                            where d.Header == intervalHeader
                            select d.interval).FirstOrDefault();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса стандартных интервалов.", exc);
                OnError(exc);
            }

            return interval ?? Interval.FromString(intervalHeader);
        }

        #endregion
    }
}
