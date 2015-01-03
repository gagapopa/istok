using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using COTES.ISTOK.ClientCore;
using System.ServiceModel;

namespace COTES.ISTOK.ClientCore
{
    [KnownType(typeof(UTypeNode))]
    public abstract class AsyncWorker : IAsyncOperationManager
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #region AsyncOperations
        public virtual void WaitAsyncOperation(ulong operationId)
        {
            throw new NotImplementedException();
        }
        public virtual UAsyncResult GetOperationResult(ulong operationId)
        {
            throw new NotImplementedException();
        }
        public virtual UAsyncResult GetOperationMessages(ulong operationId)
        {
            throw new NotImplementedException();
        }
        public virtual OperationInfo GetOperationState(ulong operationId)
        {
            throw new NotImplementedException();
        }
        public virtual void EndAsyncOperation(ulong operationId)
        {
            throw new NotImplementedException();
        }
        public virtual bool AbortAsyncOperation(ulong operationId)
        {
            throw new NotImplementedException();
        }

        protected virtual T GetOperationResult<T>(ulong operationId)
        {
            WaitAsyncOperation(operationId);
            T res = default(T);
            UAsyncResult tmp;
            throw new NotImplementedException();
            do
            {
                tmp = GetOperationResult(operationId);
                if (tmp != null)
                {

                    break;
                }
            } while (tmp != null);
            EndAsyncOperation(operationId);
            return res;
        }
        /// <summary>
        /// Возвращает результат асинхронной операции
        /// </summary>
        /// <typeparam name="T">Тип, перечисление которого требуется получить</typeparam>
        /// <param name="userGuid">Идентификатор сессии</param>
        /// <param name="operationId">Идентификатор асинхронной операции</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetOperationMultipleResult<T>(ulong operationId)
        {
            WaitAsyncOperation(operationId);
            List<T> lstRes = new List<T>();
            UAsyncResult res;
            do
            {
                res = GetOperationResult(operationId);
                if (res != null)
                {
                    object tmp = res.Packages;
                    if (typeof(T) == typeof(Package) ||
                        typeof(T) == typeof(Package[]))
                        lstRes.AddRange((T[])tmp);
                }
            } while (res != null);
            EndAsyncOperation(operationId);

            return lstRes.ToArray();
        }
        #endregion

        protected virtual T ExceptionMethod<T>(FaultException ex)
        {
            ExceptionMethod(ex);
            return default(T);
        }

        protected virtual void ExceptionMethod(FaultException ex)
        {
            if (ex != null)
            {
                if (ex is FaultException<UserNotConnectedException>)
                    ThrowException(((FaultException<UserNotConnectedException>)ex).Detail);
                else if (ex is FaultException<NoOneUserException>)
                    ThrowException(((FaultException<NoOneUserException>)ex).Detail);
                else if (ex is FaultException<LockExceptionFault>)
                    ThrowException(new LockException(((FaultException<LockExceptionFault>)ex).Detail));
                else
                    ThrowException(new Exception(ex.Message));
                //    throw ex;
            }
        }

        protected virtual void ThrowException(Exception ex)
        {
            log.DebugException("Получено исключение от сервера.", ex);
            throw ex;
        }

        #region IAsyncOperationManager members
        //protected virtual OperationState GetOperationRealState(Guid userGuid, ulong operationId)
        //{
        //    return GetOperationState(userGuid, operationId);
        //}

        public bool IsComplete(ulong id)
        {
            return GetOperationState(id).IsCompleted;
        }

        public bool IsInterrupted(ulong id)
        {
            return GetOperationState(id).IsInterrupted;
        }

        public UAsyncResult GetResult(ulong id)
        {
            return GetOperationResult(id);
        }

        public UAsyncResult GetMessages(ulong id)
        {
            return GetOperationMessages(id);
        }

        public double GetProgress(ulong id)
        {
            return GetOperationState(id).Progress;
        }

        public string GetStatus(ulong id)
        {
            return GetOperationState(id).StateString;
        }

        public void End(ulong id)
        {
            EndAsyncOperation(id);
        }

        public bool Abort(ulong id)
        {
            return AbortAsyncOperation(id);
        }
        #endregion
    }
}
