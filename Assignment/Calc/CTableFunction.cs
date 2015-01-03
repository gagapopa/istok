using System;
using System.Collections.Generic;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Класс для передачи нормативных функций в модуль расчета
    /// </summary>
    class CTableFunctionInfo : IExternalFunctionInfo
    {
        private MultiDimensionalTable funcBody = null;
        private NormFuncNode funcNode;

        public CTableFunctionInfo(NormFuncNode node, RevisionInfo revision, String categoryName)
        {
            name = node.Code;
            description = node.Text;
            this.revision = revision;
            groupName = "Табличные функции";
            if (!String.IsNullOrEmpty(categoryName)) groupName += ";" + categoryName;

            int minArgs, j;
            List<String> names = new List<String>();
            String parameterName;
            //funcBody = node.MDTable;
            funcBody = node.GetMDTable(this.revision);
            funcNode = node;
            Parameters = new CalcArgumentInfo[funcBody.DimensionInfo.Length];
            minArgs = funcBody.GetMinArgs();
            for (int i = 0; i < Parameters.Length; i++)
            {
                for (j = 0, parameterName = funcBody.DimensionInfo[i].Name; names.Contains(parameterName); j++) parameterName = funcBody.DimensionInfo[i].Name + j;
                String defValue = null;
                if (i >= minArgs) defValue = "0";
                Parameters[i] = new CalcArgumentInfo(parameterName, defValue, ParameterAccessor.In);
            }
        }

        String name, description, groupName;
        private RevisionInfo revision;
        CalcArgumentInfo[] Parameters;

        #region IExtermalFunctionInfo Members

        public string Name
        {
            get { return name; }
        }

        public RevisionInfo Revision
        {
            get { return revision; }
        }

        public string Description
        {
            get { return description; }
        }

        public string GroupName
        {
            get { return groupName; }
        }

        public CalcArgumentInfo[] Arguments
        {
            get { return Parameters; }
        }

        public object Call(Object[] args)
        {
            double[] coords = new double[args.Length];

            if (args.Length < funcBody.GetMinArgs() || args.Length > funcBody.GetMaxArgs())
                throw new Exception("Wrong number of arguments (" + Name + ")");

            for (int i = 0; i < args.Length; i++)
                coords[i] = (double)args[i];

            try { return funcBody.GetValue(coords); }
            catch (ArgumentNullException) { return null; }
        }

        #endregion
    }
}
