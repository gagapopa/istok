using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class UnitTypeTypeConverter : EnumConverter
    {
        Dictionary<int, String> data;
        public UnitTypeTypeConverter()
            : base(typeof(UnitTypeId))
        { }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                IUnitTypeRetrieval unitTypeRetrieval = context.GetService(typeof(IUnitTypeRetrieval)) as IUnitTypeRetrieval;
                if (unitTypeRetrieval != null)
                {
                    int parentID = 0;
                    UnitNode node = context.Instance as UnitNode;

                    if (node != null) parentID = node.ParentId;
                    data = RetrieveData(context, System.Globalization.CultureInfo.CurrentCulture);

                    var unitTypes = unitTypeRetrieval.GetUnitTypes(parentID);

                    if (unitTypes != null)
                    {
                        var types = (from t in unitTypes where t != (int)UnitTypeId.Unknown select t).ToArray();
                        return new StandardValuesCollection(types);
                    }
                }
            }
            return base.GetStandardValues(context);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
            {
                try
                {
                    if (value==null)
                    {
                        return String.Empty;
                    }
                    //Dictionary<UnitTypeId, String> data;
                    int unitType = (int)value;
                    if (data == null)
                        data = RetrieveData(context, culture);
                    if (data.ContainsKey(unitType)) return data[unitType];
                    else return data[(int)UnitTypeId.Unknown];
                }
                catch (InvalidCastException) { }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType().Equals(typeof(String)))
            {
                try
                {
                    //Dictionary<UnitTypeId, String> data;
                    String name = value.ToString();

                    if (data == null) 
                    data = RetrieveData(context, culture);
                    foreach (int nodeType in data.Keys)
                        if (data[nodeType].Equals(name)) return nodeType;
                }
                catch (InvalidCastException) { }
            }
            return base.ConvertFrom(context, culture, value);
        }

        private Dictionary<int, string> RetrieveData(ITypeDescriptorContext context, System.Globalization.CultureInfo culture)
        {
            Dictionary<int, String> ret = null;
            IUnitTypeRetrieval unitTypeRetrieval = context.GetService(typeof(IUnitTypeRetrieval)) as IUnitTypeRetrieval;

            if (unitTypeRetrieval != null)
                ret = unitTypeRetrieval.GetUnitTypeLocalization(culture);
            if (ret == null) ret = new Dictionary<int, string>();
            return ret;
        }
    }

    public class UnitTypeEditor : System.Drawing.Design.UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
        {
            IUnitTypeRetrieval unitTypeRetrieval = e.Context.GetService(typeof(IUnitTypeRetrieval)) as IUnitTypeRetrieval;

            if (unitTypeRetrieval != null)
            {
                int unitType = (int)e.Value;
                UTypeNode typeNode = unitTypeRetrieval.GetUnitTypeNode(unitType);
                if (typeNode != null)
                {
                    if (typeNode.Icon != null)
                        using (MemoryStream ms = new MemoryStream(typeNode.Icon))
                        {
                            Image typeImage = Image.FromStream(ms);
                            Rectangle destRect = e.Bounds;
                            //сделать картинку квадратной
                            Rectangle newRectangle;
                            int r = Math.Min(destRect.Width, destRect.Height);
                            newRectangle = new Rectangle(destRect.X + (destRect.Width - r) / 2, destRect.Y + (destRect.Height - r) / 2, r, r);
                            //destRect.Width = r;
                            //destRect.Height = r;
                            //typeImage.MakeTransparent();
                            // и отрисовываем
                            e.Graphics.DrawImage(typeImage, newRectangle);
                            return;
                        }
                }
            }
            base.PaintValue(e);
        }
    }
}
