using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace COTES.ISTOK
{
    #region Регистрация параметров для мнемосхем
    /// <summary>
    /// Делегат для получения последнего значения регистратором параметров
    /// </summary>
    /// <param name="id">Код параметра</param>
    /// <returns>Объект параметра со значением</returns>
    public delegate ParamValueItemWithID GetLastValueDelegate(int id);
    
    /// <summary>
    /// Класс регистрации обновления параметров
    /// </summary>
    public class ParameterRegistrator : COTES.ISTOK.DiagnosticsInfo.ISummaryInfo
    {
        private int ta_counter;
        private Dictionary<int, ParameterCounter> dicParameters;
        private Dictionary<int, ParametersTransaction> dicTransactions;

        /// <summary>
        /// Максимальное время жизни транзакции, а которой перестали обращаться (секунды)
        /// </summary>
        const int maxKeepTime = 86400;
        
        /// <summary>
        /// Событие, через которое регистратор может получить последнее значение параметра
        /// </summary>
        public event GetLastValueDelegate GetLastValue = null;
        
        public ParameterRegistrator()
        {
            ta_counter = 0;
            dicParameters = new Dictionary<int, ParameterCounter>();
            dicTransactions = new Dictionary<int, ParametersTransaction>();
        }

        #region Public Methods
        /// <summary>
        /// Регистрация массива параметров
        /// </summary>
        /// <param name="parameters">Массив параметров</param>
        /// <returns>Номер транзакции</returns>
        public int Register(ParamValueItemWithID[] parameters)
        {
            List<int> lstParams = new List<int>();

            foreach (ParamValueItemWithID item in parameters)
            {
                lstParams.Add(item.ParameterID);
            }

            return Register(lstParams.ToArray());
        }
        /// <summary>
        /// Регистрация массива параметров
        /// </summary>
        /// <param name="parameters">Коды параметров</param>
        /// <returns>Номер транзакции</returns>
        public int Register(int[] parameters)
        {
            List<ParamValueItemWithID> lstNParameters = new List<ParamValueItemWithID>();
            ParamValueItemWithID parameter;
            ParameterCounter counter;

            KillZombies();
            foreach (int item in parameters)
            {
                parameter = null;
                if (!dicParameters.TryGetValue(item, out counter))
                {
                    parameter = new ParamValueItemWithID();
                    parameter.ParameterID = item;

                    if (GetLastValue != null)
                    {
                        try
                        {
                            ParamValueItemWithID tmpparam;
                            tmpparam = GetLastValue(item);
                            if (tmpparam != null)
                            {
                                //parameter.begval = tmpparam.begval;
                                //parameter.channel = tmpparam.channel;
                                parameter.ChangeTime = tmpparam.ChangeTime;
                                parameter.Time = tmpparam.Time;
                                parameter.Quality = tmpparam.Quality;
                                parameter.Value = tmpparam.Value;
                            }
                        }
                        catch { }
                    }

                    counter = new ParameterCounter(parameter);
                    dicParameters.Add(item, counter);
                }
                else
                {
                    parameter = counter.Parameter;
                    counter.Counter++;
                }

                lstNParameters.Add(parameter);
            }

            IncTaID();
            ParametersTransaction ta = new ParametersTransaction(ta_counter, lstNParameters.ToArray());
            dicTransactions.Add(ta_counter, ta);

            return ta_counter;
        }
        /// <summary>
        /// Разрегистрация массива параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        public void Unregister(int taID)
        {
            ParametersTransaction ta;
            ParameterCounter counter;

            if (dicTransactions.TryGetValue(taID, out ta))
            {
                foreach (ParamValueItemWithID item in ta.Parameters)
                {
                    if (dicParameters.TryGetValue(item.ParameterID, out counter))
                    {
                        counter.Counter--;
                        if (counter.Counter <= 0)
                            dicParameters.Remove(item.ParameterID);
                    }
                }

                dicTransactions.Remove(taID);
            }
            else
                throw new Exception("Транзакция не найдена");
        }
        public int[] GetTransactionsID()
        {
            List<int> idList = new List<int>(dicTransactions.Keys);
            return idList.ToArray();
        }
        /// <summary>
        /// Получение объекта класса транзакции по ее ИД
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        /// <returns>Транзакция</returns>
        public ParametersTransaction GetTransaction(int taID)
        {
            ParametersTransaction ta;

            if (dicTransactions.TryGetValue(taID, out ta))
            {
                return ta;
            }
            else
                throw new TransactionNotFoundException();
        }
        /// <summary>
        /// Получение массива значений параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        /// <returns>Массив параметров со значениями</returns>
        public ParamValueItemWithID[] GetValues(int taID)
        {
            ParametersTransaction ta;

            ta = GetTransaction(taID);
            return ta.Parameters;
        }
        /// <summary>
        /// Обновление значений параметра
        /// </summary>
        /// <param name="param">Параметр со значением, качеством и временем</param>
        public void UpdateValue(ParamValueItemWithID param)
        {
            ParamValueItemWithID par;
            ParameterCounter counter;

            if (dicParameters.TryGetValue(param.ParameterID, out counter))
            {
                par = counter.Parameter;
                //закомментил временно. пусть клиент будет в курсе,
                //что хоть что-то передается
                if (param.Time >= par.Time)
                {
                    //par.begval = param.begval;
                    //par.channel = param.channel;
                    par.ChangeTime = param.ChangeTime;
                    par.Quality = param.Quality;
                    par.Time = param.Time;
                    par.Value = param.Value;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Увеличивает номер транзакции
        /// </summary>
        private void IncTaID()
        {
            ta_counter++;
        }
        /// <summary>
        /// Очистка списка транзакций от устаревших (к которым долго не обращались)
        /// </summary>
        private void KillZombies()
        {
            List<int> lstTaIDs = new List<int>();
            
            try
            {
                foreach (int key in dicTransactions.Keys)
                {
                    ParametersTransaction item = dicTransactions[key];

                    if (DateTime.Now.Subtract(item.LastCallTime).TotalSeconds >= maxKeepTime)
                        lstTaIDs.Add(key);
                }

                foreach (int taID in lstTaIDs)
                {
                    Unregister(taID);
                }
            }
            catch { }
        }
        #endregion

        #region ISummaryInfo Members
        const string caption = "Обновляемые параметры";

        public System.Data.DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            ParametersTransaction transaction = null;
            ParamValueItemWithID[] parameters;
            int[] ids = GetTransactionsID();
            DataTable table = new DataTable(caption);
            table.Columns.Add("Номер", typeof(int));
            table.Columns.Add("Количество", typeof(int));
            table.Columns.Add("Последняя активность", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;

            if (ids != null && ids.Length > 0)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    try
                    {
                        transaction = GetTransaction(ids[i]);
                        if (transaction != null)
                        {
                            table.Rows.Add(transaction.ID,
                                ((parameters = transaction.GetParameters()) != null ? parameters.Length : 0),
                                transaction.LastCallTime);
                        }
                    }
                    catch { }
                }
            }
            ds.Tables.Add(table);
            return ds;
        }

        public string GetSummaryCaption()
        {
            return caption;
        }

        #endregion
    }

    class ParameterCounter
    {
        private int counter;
        private ParamValueItemWithID param;

        public ParameterCounter(ParamValueItemWithID parameter)
        {
            counter = 1;
            param = parameter;
        }

        public int Counter
        {
            get { return counter; }
            set { counter = value; }
        }
        public ParamValueItemWithID Parameter
        {
            get { return param; }
        }
    }
    /// <summary>
    /// Класс набора параметров (для мнемосхем)
    /// </summary>
    public class ParametersTransaction
    {
        private int taID;
        private DateTime lastCallTime;
        private ParamValueItemWithID[] arrParams;

        public ParametersTransaction(int id, ParamValueItemWithID[] parameters)
        {
            taID = id;
            lastCallTime = DateTime.Now; 
            arrParams = parameters;
        }

        public int ID
        {
            get { return taID; }
        }
        public ParamValueItemWithID[] Parameters
        {
            get
            {
                lastCallTime = DateTime.Now;
                return arrParams;
            }
        }
        public ParamValueItemWithID[] GetParameters()
        {
            return arrParams;
        }
        public DateTime LastCallTime
        {
            get { return lastCallTime; }
        }
    }
    #endregion
}
