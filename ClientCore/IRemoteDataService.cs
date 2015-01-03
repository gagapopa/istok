using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public interface IRemoteDataService
    {
        UserNode GetUser();

        UTypeNode[] GetUnitTypes(bool renew = true);
        UnitNode GetUnitNode(int parentNode);
        GroupNode[] GetGroupNodes();
        string GetUserLogin(int userId);
        string[] GetBlockUIDs();
        ModuleInfo[] GetModuleLibNames(int blockId);
        ItemProperty[] GetModuleLibraryProperties(int blockId, string moduleName);
        Schedule[] GetParamsSchedules();
        Schedule GetParamsSchedule(int id);
        Schedule GetParamsSchedule(string name);
        
        bool ExternalCodeSupported(ExtensionUnitNode unitNode);
        bool ExternalIDSupported(ExtensionUnitNode unitNode);
        bool ExternalIDCanAdd(ExtensionUnitNode unitNode);
        EntityStruct[] ExternalIDList(ExtensionUnitNode unitNode);
        ItemProperty[] GetExternalProperties(ExtensionUnitNode unitNode);
        
        UnitNode[] GetAllUnitNodes(UnitNode parentNode, UnitTypeId[] typeFilter, int minLevel, int maxLevel);
    }
}
