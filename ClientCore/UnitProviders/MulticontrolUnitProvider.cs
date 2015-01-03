using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class MulticontrolUnitProvider : UnitProvider
    {
        List<UnitProvider> lstUnitProviders = new List<UnitProvider>();

        public UnitProvider UnitProvider { get; protected set; }

        public MulticontrolUnitProvider(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            UnitProvider = new UnitProvider(strucProvider, unitNode);
            //UnitProvider.OnSave += new SaveItemDelegate(UnitProvider_OnSave);
            UnitProvider.NodeSaved += UnitProvider_NodeSaved;
        }

        void UnitProvider_NodeSaved(object sender, EventArgs e)
        {
            OnNodeSaved();
        }

        //void UnitProvider_OnSave(UnitNode node)
        //{
        //    // Передача команды на сохранение не самым прямым способом
        //    newUnitNode = node;
        //    base.Save();
        //}

        //public override RevisionInfo CurrentRevision
        //{
        //    get
        //    {
        //        return strucProvider.CurrentRevision;
        //        //if (UnitProvider != null)
        //        //    return UnitProvider.CurrentRevision;
        //        //return base.CurrentRevision;
        //    }
        //    set
        //    {
        //        strucProvider.CurrentRevision = value;
        //        //if (UnitProvider != null)
        //        //    UnitProvider.CurrentRevision = value;
        //    }
        //}

        public override bool HasChanges
        {
            get
            {
                if (UnitProvider.HasChanges)
                    return true;
                foreach (UnitProvider provider in lstUnitProviders)
                    if (provider.HasChanges)
                        return true;

                return false;
            }
        }

        public override UnitNode NewUnitNode
        {
            get
            {
                return UnitProvider.NewUnitNode;
            }
            set
            {
                UnitProvider.NewUnitNode = value;
            }
        }

        public override void CommitSaving()
        {
            UnitProvider.CommitSaving();
        }

        public void CalcForm_CalcFinished(Object sender, EventArgs e)
        {
            ParameterGateUnitProvider parameterProvider;
            foreach (var item in lstUnitProviders)
            {
                if ((parameterProvider = item as ParameterGateUnitProvider) != null)
                    parameterProvider.CalcForm_CalcFinished(sender, e);
            }
        }

        public override IEnumerable<ActionArgs> Save()
        {
            UnitProvider.Save();
            foreach (UnitProvider provider in lstUnitProviders)
                provider.Save();
            OnNodeSaved();
            return null;
        }

        public override void ClearUnsavedData()
        {
            UnitProvider.ClearUnsavedData();
            foreach (UnitProvider provider in lstUnitProviders)
                provider.ClearUnsavedData();
        }
        public override bool EditMode
        {
            get
            {
                if (UnitProvider.EditMode)
                    return true;
                foreach (UnitProvider provider in lstUnitProviders)
                    if (provider.EditMode)
                        return true;

                return false;
            }
            set
            {
                //base.EditMode = value;
            }
        }

        //#region Настройки компонента
        //List<ParamValueItemWithID> lstParamValues = new List<ParamValueItemWithID>();

        //public DateTime DatFrom { get; set; }
        //public DateTime DatTo { get; set; }
        //#endregion

        public void AddSubProvider(UnitProvider prov)
        {
            if (!lstUnitProviders.Contains(prov)) lstUnitProviders.Add(prov);
        }
    }
}
