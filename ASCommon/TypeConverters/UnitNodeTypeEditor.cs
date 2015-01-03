using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Редактор для PropertyGrid запрашивающий UnitNode
    /// </summary>
    class UnitNodeTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            if (context != null && context.GetService(typeof(IUserInterfaceRequest)) != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var promt = context.GetService(typeof(IUserInterfaceRequest)) as IUserInterfaceRequest;

            if (promt != null)
            {
                return promt.PromtUnitNode(value as UnitNode);
            }
            return base.EditValue(context, provider, value);
        }
    }
}
