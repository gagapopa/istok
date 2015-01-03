using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Функция для проведения интерполяции во время расчета
    /// </summary>
    class InterpolateFunctionInfo : IExternalFunctionInfo
    {
        String name;
        String groupName;
        int measure;

        public InterpolateFunctionInfo(String name, int measure)
        {
            this.name = name;
            this.measure = measure;
            this.groupName = "Табличные функции";
        }

        #region IExternalFunctionInfo Members

        public string Name
        {
            get { return name; }
        }

        public RevisionInfo Revision
        {
            get { return RevisionInfo.Default; }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public string GroupName
        {
            get { return groupName; }
        }

        public CalcArgumentInfo[] Arguments
        {
            get
            {
                List<CalcArgumentInfo> arguments = new List<CalcArgumentInfo>();
                arguments.Add(new CalcArgumentInfo("Arr", "", ParameterAccessor.In));
                for (int i = 0; i < measure; i++)
                {
                    arguments.Add(new CalcArgumentInfo(String.Format("val{0}", (char)('a' + i)), "", ParameterAccessor.In));
                }

                return arguments.ToArray();
            }
        }

        public object Call(object[] args)
        {
            MultiDimensionalTable mdTable = new MultiDimensionalTable();

            if (args != null && args.Length > measure)
            {
                Object[] vals = args[0] as Object[];
                if (vals == null)
                    throw new ArgumentException("Неверный формат интерполируемых данных");
                for (int i = 0; i < vals.Length; i++)
                {
                    Object[] point = vals[i] as Object[];
                    if (point!=null)
                    {
                        if (point == null || point.Length < measure + 1)
                            throw new ArgumentException("Неверный формат интерполируемых данных");
                        double[] pointCoords = new double[measure];
                        for (int j = 0; j < measure; j++)
                            pointCoords[j] = Convert.ToDouble(point[j]);

                        mdTable.SetValue(Convert.ToDouble(point[measure]), pointCoords); 
                    }
                }
                double[] coords = new double[measure];
                for (int i = 0; i < measure; i++)
                    coords[i] = Convert.ToDouble(args[i + 1]);

                return mdTable.GetValue(coords);
            }
            return double.NaN;
        }

        #endregion
    }
}
