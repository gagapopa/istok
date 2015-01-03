using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Windows.Forms;
using COTES.ISTOK.Extension;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    static class UnitProviders
    {
        static Dictionary<Type, Func<UnitProvider, BaseUnitControl[]>> dicUnitControls = new Dictionary<Type, Func<UnitProvider, BaseUnitControl[]>>();

        static UnitProviders()
        {
            Init();
        }

        private static void Init()
        {
            dicUnitControls[typeof(ParameterGateUnitProvider)] = ((x) => new BaseUnitControl[] { new ParameterGateControl((ParameterGateUnitProvider)x) });
            dicUnitControls[typeof(GraphUnitProvider)] = ((x) => new BaseUnitControl[] { new GraphUnitControl((GraphUnitProvider)x) });
            dicUnitControls[typeof(SchemaUnitProvider)] = ((x) => new BaseUnitControl[] { new SchemaUnitControl((SchemaUnitProvider)x) });
            dicUnitControls[typeof(ExcelReportUnitProvider)] = ((x) => new BaseUnitControl[] { new ExcelReportUnitControl((ExcelReportUnitProvider)x) });
            dicUnitControls[typeof(NormFuncUnitProvider)] = ((x) => new BaseUnitControl[] { new NormFuncUnitControl((NormFuncUnitProvider)x) });
            dicUnitControls[typeof(MonitorTableUnitProvider)] = ((x) => new BaseUnitControl[] { new MonitorTableUnitControl((MonitorTableUnitProvider)x) });
            dicUnitControls[typeof(FormulaUnitProvider)] = ((x) => new BaseUnitControl[] { new FormulaEditControl((FormulaUnitProvider)x) });
            dicUnitControls[typeof(OptimizationUnitProvider)] = ((x) => new BaseUnitControl[] { new OptimizationUnitControl((OptimizationUnitProvider)x) });
            dicUnitControls[typeof(ReportUnitProvider)] = ((x) => new BaseUnitControl[] { new ReportUnitControl((ReportUnitProvider)x) });
            //dicUnitControls[typeof(NormFuncUnitProvider)] = ((x) => new NormFuncUnitControl((NormFuncUnitProvider)x));
            dicUnitControls[typeof(MulticontrolUnitProvider)] = ((x) =>
            {
                var lstControls = new List<BaseUnitControl>();
                var nodes = new List<UnitNode>();
                UnitProvider prov;
                if (x.UnitNode != null)
                {
                    //TODO: мож тож заменить на асинхрон
                    var nds = x.StructureProvider.GetUnitNodes(x.UnitNode.NodesIds);
                    nodes.AddRange(nds);
                    nodes.Sort((a, b) =>
                    {
                        if (a == null && b == null) return 0;
                        if (b == null) return 1;
                        if (a == null) return -1;
                        int ret = a.Index.CompareTo(b.Index);
                        if (ret == 0) ret = a.Idnum.CompareTo(b.Idnum);
                        return ret;
                    });
                    foreach (var item in nodes)
                    {
                        if (x.StructureProvider.FilteredTypes.Length > 0 && !x.StructureProvider.FilteredTypes.Contains(item.Typ))
                            continue;
                        prov = x.StructureProvider.GetUnitProvider(item);
                        if (prov != null && prov.GetType() != typeof(MulticontrolUnitProvider))
                        {
                            ((MulticontrolUnitProvider)x).AddSubProvider(prov);
                            var cntrls = CreateControls(prov, null);
                            if (cntrls != null)
                            {
                                lstControls.AddRange(cntrls);
                            }
                        }
                    }
                }

                return lstControls.ToArray();
            });
        }

        public static BaseUnitControl[] CreateControls(UnitProvider unitProvider, BaseAsyncWorkForm uniForm)
        {
            if (unitProvider == null) throw new ArgumentNullException("unitProvider");
            if (dicUnitControls.ContainsKey(unitProvider.GetType()))
            {
                var c = dicUnitControls[unitProvider.GetType()](unitProvider);
                if (c != null)
                    foreach (var item in c) item.UniForm = uniForm;
                return c;
            }
            return null;
        }
        //public static UnitProvider Create(UnitNode unitNode, BaseAsyncWorkForm uniForm, RemoteDataService rds)
        //{
        //    switch (unitNode.Typ)
        //    {
        //        case UnitTypeId.Unknown:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.Station:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.Block:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.Channel:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.Parameter:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.TEP:
        //            return new FormulaUnitProvider(unitNode as CalcParameterNode, uniForm, rds);
        //        case UnitTypeId.Graph:
        //            return new GraphUnitProvider(unitNode as GraphNode, uniForm, rds);
        //        case UnitTypeId.Histogram:
        //            return new GraphUnitProvider(unitNode as HistogramNode, uniForm, rds);
        //        case UnitTypeId.Schema:
        //            return new SchemaUnitProvider(unitNode as SchemaNode, uniForm, rds);
        //        //case UnitTypeId.MonitorTable:
        //        //    return new MonitorTableUnitProvider(unitNode as MonitorTableNode, uniForm, rds);
        //        //case UnitTypeId.ManualGate:
        //        //    return new ParameterGateUnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.ManualParameter:
        //            return new UnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.Report:
        //            return new ReportUnitProvider(unitNode as ReportNode, uniForm, rds);
        //        case UnitTypeId.Folder:
        //            return new MulticontrolUnitProvider(unitNode, uniForm, rds);
        //        case UnitTypeId.NormFunc:
        //            return new NormFuncUnitProvider(unitNode as NormFuncNode, uniForm, rds);
        //        case UnitTypeId.ExcelReport:
        //            return new ExcelReportUnitProvider(unitNode as ExcelReportNode, uniForm, rds);
        //        case UnitTypeId.ManualGate:
        //        case UnitTypeId.TEPTemplate:
        //            UnitNode tempNode = unitNode;
        //            OptimizationGateNode optimizationNode = null;
        //            while (tempNode != null && (optimizationNode = tempNode as OptimizationGateNode) == null)
        //                tempNode = rds.GetUnitNode(tempNode.ParentId);
        //            if (optimizationNode != null)
        //                return new CParameterGateProvider(unitNode as ParameterGateNode, uniForm, rds);
        //            return new ParameterGateUnitProvider(unitNode, uniForm, rds);
        //        //case UnitTypeId.Boiler:
        //        //    return new MonitorTableUnitProvider(unitNode as MonitorTableNode, uniForm, rds);
        //        case UnitTypeId.OptimizeCalc:
        //            return new OptimizationUnitProvider(unitNode as OptimizationGateNode, uniForm, rds);
        //        default:
        //            if (unitNode is ExtensionUnitNode)
        //                return new ExtensionUnitProvider(unitNode as ExtensionUnitNode, uniForm, rds);

        //            return new UnitProvider(unitNode, uniForm, rds);
        //    }

        //    throw new NotSupportedException();
        //}

        public static UnitEditForm CreateEditForm(StructureProvider strucProvider, UnitNode unitNode)
        {
            switch (unitNode.Typ)
            {
                //case UnitTypeId.Root:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.Station:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.Block:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.Channel:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.Parameter:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.TEP:
                //    return new UnitEditForm(unitNode);
                case (int)UnitTypeId.Graph:
                    return new GraphEditForm(strucProvider, unitNode);
                case (int)UnitTypeId.Histogram:
                    return new GraphEditForm(strucProvider, unitNode);
                case (int)UnitTypeId.Schema:
                    return new SchemaEditForm(strucProvider, unitNode);
                //case UnitTypeId.Boiler:
                case (int)UnitTypeId.MonitorTable:
                    return new MonitorTableEditForm(strucProvider, unitNode);
                //case UnitTypeId.ManualGate:
                //    return new UnitEditForm(unitNode);
                //case UnitTypeId.ManualParameter:
                //    return new UnitEditForm(unitNode);
                case (int)UnitTypeId.Report:
                    return new ReportEditForm(strucProvider, unitNode);
                //case UnitTypeId.Folder:
                //    return new UnitEditForm(unitNode);
                case (int)UnitTypeId.NormFunc:
                    return new NormFuncEditForm(strucProvider, unitNode);
                case (int)UnitTypeId.ExcelReport:
                    return new ExcelEditForm(strucProvider, unitNode);
                //case UnitTypeId.TEPTemplate:
                //    return new UnitEditForm(unitNode);
                default:
                    return null; // new UnitEditForm(unitNode);
            }

            throw new NotSupportedException();
        }

        public static bool CanCreateEditForm(UnitNode curNode)
        {
            switch (curNode.Typ)
            {
                case (int)UnitTypeId.Graph:
                case (int)UnitTypeId.Histogram:
                case (int)UnitTypeId.MonitorTable:
                //case UnitTypeId.Boiler:
                case (int)UnitTypeId.Schema:
                case (int)UnitTypeId.ExcelReport:
                case (int)UnitTypeId.NormFunc:
                case (int)UnitTypeId.Report:
                    return true;
                default:
                    return false;
            }
        }
    }
}
