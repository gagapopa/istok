using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Редактор массива UnitNode'ов
    /// </summary>
    public class ParameterNodeUIEditor : System.Drawing.Design.UITypeEditor
    {
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return base.GetPaintValueSupported(context);
        }

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var uiRequest = context.GetService(typeof(IUserInterfaceRequest)) as IUserInterfaceRequest;
            //var prov = context.GetService(typeof(IStructureRetrieval)) as IStructureRetrieval;

            if (uiRequest != null)
            {
                //UnitNode[] editNodes;

                //if ((editNodes = value as UnitNode[]) != null)
                //{
                //    editNodes = (from u in editNodes select prov.GetUnitNode(u.Idnum)).ToArray();
                //}

                var nodes = uiRequest.SelectNodes("Параметры для отчета", value as UnitNode[]);

                if (nodes != null)
                    return nodes.ToArray();

                return null;
            }
            return base.EditValue(context, provider, value);
        }
    }
}
