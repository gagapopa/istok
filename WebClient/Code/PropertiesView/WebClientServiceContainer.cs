using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.TypeConverters;
using System.ComponentModel;
using COTES.ISTOK;
using System.Collections.Generic;

namespace WebClient
{
    /// <summary>
    /// Сервис контайнер для конвертирования значений свойств объекта.
    /// </summary>
    public class WebClientServiceContainer : ITypeDescriptorContext, ISite, IUnitTypeRetrieval, IOwnerRetrieval,
        IBlockUIDRetrieval, IModuleLibNameRetrieval, IChannelRetrieval, IScheduleRetrivial, IStructureRetrieval
    {
        private WebRemoteDataService data_service;

        public WebClientServiceContainer(WebRemoteDataService data_service)
        {
            this.data_service = data_service;
        }

        #region ITypeDescriptorContext Members

        void ITypeDescriptorContext.OnComponentChanged()
        {
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            return true;
        }

        Object ITypeDescriptorContext.Instance
        {
            get { return null; }
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get { return null; }
        }

        #endregion

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

        // retrivial

        #region IUnitTypeRetrieval Members

        UnitTypeId[] IUnitTypeRetrieval.GetUnitTypes(int parentNodeID)
        {
            List<UnitTypeId> retList = new List<UnitTypeId>();
            UTypeNode[] uTypeNodes = data_service.GetUnitTypes();
            UTypeNode parentType;
            UserNode user = data_service.GetUser();
            UnitNode unitNode = data_service.GetUnitNode(parentNodeID); 
            UnitTypeId unitType;

            if (unitNode != null)
                unitType = unitNode.Typ;
            else unitType = UnitTypeId.Unknown;
            parentType = new List<UTypeNode>(uTypeNodes).Find(x => x.Idnum == (int)unitType);
            
            foreach (UTypeNode unitTypeNode in uTypeNodes)
            {
                if ((user.IsAdmin
                    || user.CheckPrivileges((UnitTypeId)unitTypeNode.Idnum, Privileges.Write))
                    && (parentType == null || parentType.ChildFilterAll || parentType.ChildFilter.Contains(unitTypeNode.Idnum)))
                    retList.Add((UnitTypeId)unitTypeNode.Idnum);
            }
            return retList.ToArray();
        }

        Dictionary<UnitTypeId, String> IUnitTypeRetrieval.GetUnitTypeLocalization(System.Globalization.CultureInfo culture)
        {
            Dictionary<UnitTypeId, String> retDictionary = new Dictionary<UnitTypeId, String>();
            UTypeNode[] uTypeNodes = data_service.GetUnitTypes();

            foreach (UTypeNode unitTypeNode in uTypeNodes)
                retDictionary[(UnitTypeId)unitTypeNode.Idnum] = unitTypeNode.Text;

            return retDictionary;
        }

        UTypeNode IUnitTypeRetrieval.GetUnitTypeNode(UnitTypeId unitType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOwnerRetrieval Members

        int IOwnerRetrieval.GetCurrentUser()
        {
            return -data_service.GetUser().Idnum;
        }

        IEnumerable<int> IOwnerRetrieval.GetGroups()
        {
            GroupNode[] groups = data_service.GetGroupNodes();
            List<int> groupIds = new List<int>();
            foreach (GroupNode groupNode in groups)
                groupIds.Add(groupNode.Idnum);

            return groupIds.ToArray();
        }

        IEnumerable<int> IOwnerRetrieval.GetUsers()
        {
            throw new NotImplementedException();
        }

        string IOwnerRetrieval.GetOwnerLocalization(int ownerID, System.Globalization.CultureInfo culture)
        {
            if (ownerID == 0) return "общий";
            else if (ownerID == -data_service.GetUser().Idnum) return "личный";
            else if (ownerID > 0)
            {
                List<GroupNode> groups = new List<GroupNode>(data_service.GetGroupNodes());
                GroupNode group = groups.Find(x => x.Idnum.Equals(ownerID));
                if (group != null) return group.Text;
            }
            else
            {
                String login = data_service.GetUserLogin(ownerID);
                if (!String.IsNullOrEmpty(login)) return "личный:" + login;
            }

            return ownerID.ToString();
        }

        #endregion

        #region IBlockUIDRetrieval Members

        string[] IBlockUIDRetrieval.GetBlockUIDs()
        {
            return data_service.GetBlockUIDs();
        }

        #endregion

        #region IModuleLibNameRetrieval Members

        string[] IModuleLibNameRetrieval.GetModuleLibNames(BlockNode blockNode)
        {
            return data_service.GetModuleLibNames(blockNode.Idnum);
        }

        #endregion

        #region IChannelRetrieval Members

        ChannelProperty[] IChannelRetrieval.GetProperties(BlockNode blockNode, String libName)
        {
            return data_service.GetModuleLibraryProperties(blockNode.Idnum, 
                                                           libName);
        }

        #endregion

        #region IScheduleRetrivial Members

        Schedule[] IScheduleRetrivial.GetSchedules()
        {
            return data_service.GetParmetrUnloadSchedules();
        }

        Schedule IScheduleRetrivial.GetSchedule(int id)
        {
            return data_service.GetParametrUnloadSchedule(id);
        }

        Schedule IScheduleRetrivial.GetSchedule(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IModuleLibNameRetrieval Members


        public BlockNode GetBlockNode(ChannelNode channel)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IChannelRetrieval Members


        BlockNode IChannelRetrieval.GetBlockNode(ChannelNode channel)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStructureRetrieval Members

        UnitNode IStructureRetrieval.GetUnitNode(int unitNodeID)
        {
            return data_service.GetUnitNode(unitNodeID);
        }

        UnitNode[] IStructureRetrieval.GetChildNodes(UnitNode parentNode, UnitTypeId[] typeFilter, int minLevel, int maxLevel)
        {
            return data_service.GetAllUnitNodes(parentNode, typeFilter, minLevel, maxLevel);
        }

        #endregion
    }
}
