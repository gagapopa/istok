using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class UserDataService : AsyncGlobalWorker
    {
        internal UserDataService(Session session)
            : base(session)
        {
            //
        }


        public UserNode GetConnectedUser()
        {
            return session.User;
        }

        #region Users
        public UserNode GetUser()
        {
            string opid = "GetUser" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUser(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UserNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public string GetUser(int userId)
        {
            string opid = "GetUser" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUserLogin(session.Uid, userId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<string>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public UserNode[] GetUserNodes()
        {
            string opid = "GetUserNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUserNodes(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UserNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public UserNode[] GetUserNodes(int[] userNodeIds)
        {
            string opid = "GetUserNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUserNodesByIds(session.Uid, userNodeIds);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UserNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public string GetUserLogin(int ownerID)
        {
            string opid = "GetUserLogin" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUserLogin(session.Uid, ownerID);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<string>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public UserNode AddUserNode(UserNode userNode)
        {
            string opid = "AddUserNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddUserNode(session.Uid, userNode);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UserNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public UserNode UpdateUserNode(UserNode userNode)
        {
            string opid = "UpdateUserNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateUserNode(session.Uid, userNode);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UserNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void RemoveUserNode(UserNode userNode)
        {
            string opid = "RemoveUserNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveUserNode(session.Uid, new int[] { userNode.Idnum });
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

        public void NewAdmin(UserNode userNode)
        {
            string opid = "NewAdmin" + Guid.NewGuid().ToString();
            try
            {
                AllocQManager(opid).NewAdmin(userNode);
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
        #endregion

        #region Groups
        public GroupNode[] GetGroupNodes()
        {
            string opid = "GetGroupNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetGroupNodes(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<GroupNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public GroupNode AddGroupNode(GroupNode groupNode)
        {
            string opid = "AddGroupNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddGroupNode(session.Uid, groupNode);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<GroupNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public GroupNode UpdateGroupNode(GroupNode groupNode)
        {
            string opid = "UpdateGroupNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateGroupNode(session.Uid, groupNode);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<GroupNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void RemoveGroupNode(int[] groupNodeIds)
        {
            string opid = "RemoveGroupNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveGroupNode(session.Uid, groupNodeIds);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        #endregion
    }
}
