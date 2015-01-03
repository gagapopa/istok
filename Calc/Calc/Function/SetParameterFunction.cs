using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    class SetParameterFunction : ParameterFunction
    {
        public const String ValueArgumentName = CalcContext.TempVariablePrefix + "value";

        public SetParameterFunction(ICalcSupplier calcSupplier, String name, String groupName, String comment)
            : base(calcSupplier, name, CalcAggregation.First, groupName, comment)
        {
            Parameters = new CalcArgumentInfo[]{
                new CalcArgumentInfo(ParameterDefaultArgumentName, "", ParameterAccessor.In),
                new CalcArgumentInfo(ValueArgumentName, "", ParameterAccessor.In)
            };
        }

        public override void Subroutine(ICalcContext context)
        {
            String[] node;
            String parameterCode;

            node = GetNode(context);
            if (node == null && node.Length != 1)
            {
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Не верное число аргументов для функции {0}", Name));
                return;
            }
            else parameterCode = node[0];

            RevisionInfo revision = calcSupplier.GetRevision(context.SymbolTable.CallContext.EndTime);

            IParameterInfo parameterInfo = calcSupplier.GetParameterNode(context, revision, parameterCode);
            ICalcNode revisionCalcnode = parameterInfo.CalcNode;
            IOptimizationInfo optimizationInfo = parameterInfo.Optimization;

            DateTime startTime, endTime;

            startTime = context.SymbolTable.CallContext.StartTime;
            endTime = context.SymbolTable.CallContext.EndTime;

            // проверить корректность данных
            if (Interval.GetInterval(startTime, endTime) != parameterInfo.Interval
                || parameterInfo.Interval.NearestEarlierTime(/*parameterInfo.StartTime,*/ startTime) != startTime)
            {
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Попыка сохранить значение параметра ${0}$ при неверной дискретности", parameterCode));
                return;
            }

            ArgumentsValues args = null;
            SymbolValue val = null;

            Variable par = context.SymbolTable.GetSymbol(ValueArgumentName) as Variable;
            val = par.Value;

            // Получение условных аргументов значения
            if (optimizationInfo != null)
            {
                bool fail;
                args = GetArgumentsValues(context, parameterCode, optimizationInfo, 0, out fail);
            }

            context.ValuesKeeper.AddCalculatedValue(parameterInfo.CalcNode, args, startTime, val);

            context.ValuesKeeper.ClearAggregation(parameterInfo.CalcNode, args, startTime);
        }
    }
}
