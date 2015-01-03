using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class ModuleLibNameTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.GetService(typeof(IModuleLibNameRetrieval)) != null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IModuleLibNameRetrieval libNameRetrieval = context.GetService(typeof(IModuleLibNameRetrieval)) as IModuleLibNameRetrieval;
            ChannelNode channel = context.Instance as ChannelNode;
            
            if (libNameRetrieval != null && channel != null)
            {
                BlockNode blockNode = libNameRetrieval.GetBlockNode(channel);

                if (blockNode != null)
                {
                    IEnumerable<ModuleInfo> libs = libNameRetrieval.GetModuleLibNames(blockNode);
                    if (libs != null)
                        return new StandardValuesCollection(libs.ToArray());
                }
            }

            return base.GetStandardValues(context);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return value;
        }
    }
}
