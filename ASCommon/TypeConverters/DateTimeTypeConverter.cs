using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
//using System.Drawing.Design;

namespace COTES.ISTOK.ASC.TypeConverters
{
    ////Для вывода только месяца и года
    //public class DateTimeTypeConverter : DateTimeEditor
    //{

    //    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        DateTimeEditor datEdit = new DateTimeEditor();
    //        DateTime date1 = new DateTime();

    //        try
    //        {
    //            //преобразуем строку
    //            date1 = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
    //        }
    //        catch
    //        {
    //            date1 = DateTime.Now;
    //        }


    //        DateTime date2 = (DateTime)datEdit.EditValue(context, provider, date1);
    //        //возвращаем в окно
    //        return date2.Month.ToString("00.") + "." + date2.Year.ToString();
    //    }

    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    //    {
    //        return UITypeEditorEditStyle.DropDown;
    //    }


    //}
}
