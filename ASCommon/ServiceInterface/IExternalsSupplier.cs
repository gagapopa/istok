using System;
using System.Collections.Generic;
using System.ComponentModel;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Extension
{
    public interface IExternalsSupplier
    {
        bool ExternalCodeSupported(ExtensionUnitNode unitNode);
        bool ExternalIDSupported(ExtensionUnitNode unitNode);
        bool ExternalIDCanAdd(ExtensionUnitNode unitNode);

        EntityStruct[] GetExternalIDList(ExtensionUnitNode unitNode);

        ItemProperty[] GetExternalProperties(ExtensionUnitNode unitNode);
    }
}
