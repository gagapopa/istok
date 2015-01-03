using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    class NormFuncNodeTypeConverter : UnitNodeTypeConverter
    {
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection result = base.GetProperties(context, value, attributes);
            NormFuncNode node = (NormFuncNode)value;
            MultiDimensionalTable mdt;

            IUnitProviderSupplier unitProviderSupplier = null;
            IUnitNodeProvider unitProvider = null;

            if (context!=null)
            {
                unitProviderSupplier = context.GetService(typeof(IUnitProviderSupplier)) as IUnitProviderSupplier;

                if (unitProviderSupplier != null)
                    unitProvider = unitProviderSupplier.GetProvider(node); 
            }

            if (unitProvider != null)
            {
                RevisionInfo revision = unitProvider.GetRealRevision(unitProvider.CurrentRevision);

                mdt = node.GetMDTable(revision);

                if (mdt != null)
                {
                    DimensionTypeConverter dtc = new DimensionTypeConverter();
                    DimensionPropertyCollection props = new DimensionPropertyCollection();

                    for (int i = mdt.DimensionInfo.Length - 1; i >= 0; i--)
                        props.AddDimensionInfo(mdt.DimensionInfo[i]);

                    foreach (PropertyDescriptor item in dtc.GetProperties(props))
                        result.Add(item);
                }
            }

            return result;
        }
    }
}
