using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class ExcelReportUnitProvider : UnitProvider
    {
        public ExcelReportUnitProvider(StructureProvider strucProvider, ExcelReportNode excelNode)
            : base(strucProvider, excelNode)
        {
        }

        //protected override BaseUnitControl GetUnitControl(bool multitab)
        //{
        //    return new ExcelReportUnitControl(this);
        //}

        public byte[] GenerateReport(bool saveInSystem)
        {
            return RDS.ReportDataService.GenerateReport(UnitNode.Idnum, DatFrom, DatTo, saveInSystem);
        }

        #region Настройки компонента
        List<ParamValueItemWithID> lstParamValues = new List<ParamValueItemWithID>();

        public DateTime DatFrom { get; set; }
        public DateTime DatTo { get; set; }
        #endregion

        public bool DataReady { get; set; }
    }
}
