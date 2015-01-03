using System;
using System.Drawing.Design;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Редактор PropertyGrid. Выбрать несколько типов.
    /// </summary>
    class UnitTypeArrayEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            if (context!=null&&context.GetService(typeof(IUserInterfaceRequest))!=null)
            {
                return UITypeEditorEditStyle.DropDown; 
            }
            return base.GetEditStyle(context);
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var userInterface = context.GetService(typeof(IUserInterfaceRequest)) as IUserInterfaceRequest;

            if (userInterface!=null)
            {
                return userInterface.PromtUnitTypesDropDown(provider, value as int[]); 
            }

            return base.EditValue(context, provider, value);
        }
    }
}
