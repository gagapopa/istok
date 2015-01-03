using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class ReportDataService : AsyncGlobalWorker
    {
        internal ReportDataService(Session session)
            : base(session)
        {
            //
        }

        //public ulong BeginGenerateReport(int parameterId, DateTime dateFrom, DateTime dateTo, bool saveInSystem)
        //{
        //    //return qManager.BeginGenerateExcelReport(session.Uid, parameterId, dateFrom, dateTo, saveInSystem);
        //    throw new NotImplementedException();
        //}
        public byte[] GenerateReport(int parameterId, DateTime dateFrom, DateTime dateTo, bool saveInSystem)
        {
            string opid = "GenerateReport" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GenerateExcelReport(session.Uid, parameterId, dateFrom, dateTo, saveInSystem);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<byte[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public byte[] GenerateFastReport(ReportNode reportNode, bool SaveInSystem, ReportParameter[] reportParameter)
        {
            string opid = "GenerateFastReport" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GenerateReport(session.Uid, reportNode.Idnum, SaveInSystem, reportParameter);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<byte[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ReportSourceInfo[] GetReportSources()
        {
            string opid = "GetReportSources" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetReportSources(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ReportSourceInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ReportSourceSettings GetReportSourceSettings(Guid reportSourceID)
        {
            string opid = "GetReportSourceSettings" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetReportSourceSettings(session.Uid, reportSourceID);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ReportSourceSettings>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public FastReportWrap GenerateReportData(ReportSourceSettings[] reportSourceSettings, ReportParameter[] reportParameter)
        {
            string opid = "GenerateReportData" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GenerateReportData(session.Uid, reportSourceSettings, reportParameter);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<FastReportWrap>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public FastReportWrap GenerateEmptyReportData(ReportSourceSettings[] reportSourceSettings)
        {
            string opid = "GenerateEmptyReportData" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GenerateEmptyReportData(session.Uid, reportSourceSettings);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<FastReportWrap>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public PreferedReportInfo[] GetPreferedReports(DateTime dateTime1, DateTime dateTime2)
        {
            string opid = "GetPreferedReports" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetPreferedReports(session.Uid, dateTime1, dateTime2);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<PreferedReportInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public byte[] GetPreferedReportBody(PreferedReportInfo reportInfo)
        {
            string opid = "GetPreferedReportBody" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetPreferedReportBody(session.Uid, reportInfo);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<byte[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void DeletePreferedReport(PreferedReportInfo reportInfo)
        {
            string opid = "DeletePreferedReport" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).DeletePreferedReport(session.Uid, reportInfo);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

    }
}
