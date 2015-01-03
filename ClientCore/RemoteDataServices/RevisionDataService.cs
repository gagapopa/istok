using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class RevisionDataService : AsyncGlobalWorker
    {
        internal RevisionDataService(Session session)
            : base(session)
        {
            //
        }

        public ulong BeginGetRevisions()
        {
            //return qManager.BeginGetRevisions(session.Uid);
            throw new NotImplementedException();
        }
        public RevisionInfo[] GetRevisions()
        {
            string opid = "GetRevisions" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetRevisions(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<RevisionInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        //public void RemoveRevisions(int[] revisionIds)
        //{
        //    try
        //    {
        //        var res = AllocQManager().RemoveRevisions(session.Uid, revisionIds);
        //        session.CommitDataChanges(res.Changes);
        //    }
        //    catch (FaultException ex)
        //    {
        //        ExceptionMethod<Object>(ex);
        //    }
        //    finally
        //    {
        //        FreeQManager();
        //    }
        //}

        //public void UpdateRevisions(RevisionInfo[] revisions)
        //{
        //    try
        //    {
        //        var res = AllocQManager().UpdateRevisions(session.Uid, revisions);
        //        session.CommitDataChanges(res.Changes);
        //    }
        //    catch (FaultException ex)
        //    {
        //        ExceptionMethod<Object>(ex);
        //    }
        //    finally
        //    {
        //        FreeQManager();
        //    }
        //}

        public void UpdateRevisions(int[] removeArray, RevisionInfo[] revisions)
        {
            string opid = "UpdateRevisions" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateRevisions(session.Uid, removeArray, revisions);
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
