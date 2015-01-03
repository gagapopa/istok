using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class SchemaUnitProvider : OnlineUnitProvider
    {
        public SchemaUnitProvider(StructureProvider strucProvider, SchemaNode schemaNode)
            : base(strucProvider, schemaNode)
        {
            //
            UpdateInterval = schemaNode.UpdateInterval * 1000;
        }

        //public override BaseUnitControl[] CreateControls(UnitTypeId[] types)
        //{
        //    if (Controls != null) DisposeControls();
        //    BaseUnitControl control;

        //    if (types != null && types.Length > 0 && !types.Contains(UnitNode.Typ)) return null;
        //    control = new SchemaUnitControl();
        //    //control.UnitProvider = this;
        //    control.Text = UnitNode.Text;
        //    control.Dock = DockStyle.Fill;
        //    Controls = new BaseUnitControl[] { control };
            
        //    return Controls;
        //}

        #region Настройки компонента
        //protected ParamReceiveItem[] schemaValues = null;
        //public ParamReceiveItem[] SchemaValues
        //{
        //    get
        //    {
        //        return schemaValues;
        //    }
        //    set
        //    {
        //        if (schemaValues != null)
        //        {
        //            lock (schemaValues)
        //            {
        //                schemaValues = value;
        //            }
        //        }
        //        schemaValues = value;
        //    }
        //}
        #endregion
    }
}
