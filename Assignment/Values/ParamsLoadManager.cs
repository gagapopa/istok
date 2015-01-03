//using System;
//using System.Collections.Generic;
//using System.Linq;

//using System.Threading;

//using COTES.ISTOK;

//namespace COTES.ISTOK.Assignment
//{
//    /// <summary>
//    /// Менеджер загрузки параметров с блочного
//    /// сырой пока что,
//    /// в потоке опрашиваются айдишки и сохраняются. вот.
//    /// </summary>
//    class ParamsLoadManager
//    {
//        private object sync_obj = new object();

//        private bool work;
//        private Thread work_thread;

//        private LinkedList<Package> recived_packages = 
//            new LinkedList<Package>();
//        private List<ulong> operation_code;
//        private AsyncOperation operation_manager;
//        private Action<OperationState, Package[]> save_recived_value;
//        //private Log log;

//        public ParamsLoadManager(AsyncOperation manager,
//                                 Action<OperationState, Package[]> reciver)
//        {
//            if (manager == null ||
//                reciver == null)
//                throw new ArgumentNullException();

//            operation_manager = manager;
//            //this.log = log;
//            this.save_recived_value = reciver;
//            work_thread = new Thread(this.QueryCycle);
//        }

//        public void Start(IEnumerable<ulong> operations)
//        {
//            if (operations == null)
//                throw new ArgumentNullException();

//            if (!work)
//            {
//                operation_code = operations.ToList();
//                work = true;
//                work_thread.Priority = ThreadPriority.Lowest;
//                work_thread.Start();
//            }
//            else
//                lock (sync_obj)
//                    operation_code.AddRange(operations);
//        }

//        public void End()
//        {
//            work = false;
//        }

//        private void QueryCycle()
//        {
//            try
//            {
//                while (work && operation_code.Count != 0)
//                {
//                    lock (sync_obj)
//                    {
//                        for (int i = operation_code.Count - 1; i <= 0; --i)
//                        {
//                            GetData(operation_code[i]);

//                            var state = 
//                                operation_manager.GetAsyncOperationState(operation_code[i]);
//                            if (state.IsCompleted)
//                            {
//                                while (GetData(operation_code[i])) { }

//                                try
//                                { operation_manager.EndAsyncOperation(operation_code[i]); }
//                                catch (Exception exp)
//                                { WriteToLog(exp); }

//                                operation_code.RemoveAt(i);
//                            }

//                            Thread.Sleep(0);
//                        }
//                    }
//                }
//            }
//            catch (Exception exp)
//            {
//                WriteToLog(exp);
//            }
//            finally
//            {
//                save_recived_value(new OperationState(), recived_packages.ToArray());
//                work = false;
//            }
//        }

//        private void WriteToLog(Exception exp)
//        {
//            //if (log == null || exp == null)
//            //    return;

//            //log.Write(MessageCategory.Error, exp.Message);
//        }

//        private bool GetData(ulong operation)
//        {
//            var res = operation_manager.GetAsyncOperationData(operation, true);
//            if (res != null && res is Package)
//            {
//                recived_packages.AddFirst(res as Package);
//                return true;
//            }
//            else 
//                return false;
//        }
//    }
//}
