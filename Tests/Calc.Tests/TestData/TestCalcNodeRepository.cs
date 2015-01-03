using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestCalcNodeRepository
    {
        List<IParameterInfo> parametersList = new List<IParameterInfo>();

        List<IOptimizationInfo> optimizationList = new List<IOptimizationInfo>();

        public IEnumerable<IParameterInfo> Parameters
        {
            get { return parametersList.AsReadOnly(); }
        }

        public IEnumerable<IOptimizationInfo> Optimizations
        {
            get { return optimizationList.AsReadOnly(); }
        }

        public void Add(IParameterInfo parameter)
        {
            parametersList.Add(parameter);
        }

        public void Add(IOptimizationInfo optimization)
        {
            optimizationList.Add(optimization);
        }
    }
}
