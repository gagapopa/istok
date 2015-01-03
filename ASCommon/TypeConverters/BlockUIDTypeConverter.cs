using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class BlockUIDTypeConverter : StringConverter
    {
        string[] arr = new string[] { };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.GetService(typeof(IBlockUIDRetrieval))!=null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                //if (arr.Length == 0)
                {
                    IBlockUIDRetrieval bidRetrieval = context.GetService(typeof(IBlockUIDRetrieval)) as IBlockUIDRetrieval;
                    if (bidRetrieval != null)
                    {
                        string[] tag = bidRetrieval.GetBlockUIDs();
                        if (tag != null) arr = tag;
                    }
                    //if (context.Instance is ExtPropertyCollection)
                    //    tag = ((ExtPropertyCollection)context.Instance).Tag as string[];
                    //if (tag != null && tag.Length > 0)
                }
            }
            catch
            {
                //
            }
            return new StandardValuesCollection(arr);
        }
    }
}
