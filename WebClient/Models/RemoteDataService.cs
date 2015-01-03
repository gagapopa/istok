using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COTES.ISTOK.WebClient.Models
{
    public class RemoteDataService : IRemoteDataService
    {
        //IGlobalQueryManager qManager;
        SessionKeeper sKeeper;

        UTypeNode[] types = null;

        public RemoteDataService(SessionKeeper sKeeper)
        {
            //this.qManager = qManager;
            this.sKeeper = sKeeper;
        }

        public COTES.ISTOK.ASC.UserNode GetUser()
        {
            return sKeeper.Session.User;
        }

        public COTES.ISTOK.ASC.UTypeNode[] GetUnitTypes(bool renew = true)
        {
            if (types == null || renew)
            {
                types = sKeeper.Session.GetUnitTypes();
            }
            return types;
        }

        public COTES.ISTOK.ASC.UnitNode GetUnitNode(int parentNode)
        {
            return sKeeper.GetStrucProvider(0).GetUnitNode(parentNode);
        }

        public COTES.ISTOK.ASC.GroupNode[] GetGroupNodes()
        {
            //ulong op = sKeeper.Client.BeginGetGroupNodes(sKeeper.Uid);
            //sKeeper.Client.WaitAsyncOperation(sKeeper.Uid, op);
            throw new NotImplementedException();
        }

        public string GetUserLogin(int userId)
        {
            throw new NotImplementedException();
        }

        public string[] GetBlockUIDs()
        {
            throw new NotImplementedException();
        }

        public ModuleInfo[] GetModuleLibNames(int blockId)
        {
            throw new NotImplementedException();
        }

        public COTES.ISTOK.ItemProperty[] GetModuleLibraryProperties(int blockId, string moduleName)
        {
            return sKeeper.Session.GetModuleLibraryProperties(blockId, moduleName);
        }

        public COTES.ISTOK.Schedule[] GetParamsSchedules()
        {
            return sKeeper.Session.GetSchedules();
        }

        public COTES.ISTOK.Schedule GetParamsSchedule(int id)
        {
            return sKeeper.Session.GetSchedule(id);
        }

        public COTES.ISTOK.Schedule GetParamsSchedule(string name)
        {
            return sKeeper.Session.GetSchedule(name);
        }

        public bool ExternalCodeSupported(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
        {
            throw new NotImplementedException();
        }

        public bool ExternalIDSupported(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
        {
            throw new NotImplementedException();
        }

        public bool ExternalIDCanAdd(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
        {
            throw new NotImplementedException();
        }

        public COTES.ISTOK.ASC.EntityStruct[] ExternalIDList(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
        {
            throw new NotImplementedException();
        }

        public COTES.ISTOK.ItemProperty[] GetExternalProperties(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
        {
            throw new NotImplementedException();
        }

        public COTES.ISTOK.ASC.UnitNode[] GetAllUnitNodes(COTES.ISTOK.ASC.UnitNode parentNode, COTES.ISTOK.ASC.UnitTypeId[] typeFilter, int minLevel, int maxLevel)
        {
            throw new NotImplementedException();
        }
    }
}