using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class OnlineUnitProvider : UnitProvider
    {
        public OnlineUnitProvider(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {

        }

        /// <summary>
        /// Номер транзакции обновления параметров
        /// </summary>
        private int taID = 0;

        private System.Threading.Timer timer = null;

        #region Настройки компонента
        protected ParamValueItemWithID[] paramValues = null;

        public ParamValueItemWithID[] ParamValues
        {
            get
            {
                return paramValues;
            }
            set
            {
                if (paramValues != null)
                {
                    lock (paramValues)
                    {
                        paramValues = value;
                    }
                }
                paramValues = value;
            }
        }

        public int UpdateInterval { get; set; }
        #endregion

        #region Регистрация обновления параметров
        public void RegisterParameters()
        {
            List<ParamValueItemWithID> lstParams = new List<ParamValueItemWithID>();
            ParamValueItemWithID ptr;

            if (taID == 0)
            {
                foreach (ChildParamNode item in unitNode.Parameters)
                {
                    ptr = new ParamValueItemWithID();
                    ptr.ParameterID = item.ParameterId;
                    lstParams.Add(ptr);
                }
                if (lstParams.Count > 0)
                {
                    // регистрация параметров на сервере
                    taID = RDS.OnlineDataService.RegisterClient(lstParams.ToArray());
                    timer = new System.Threading.Timer(new TimerCallback(TimerCallback));

                    timer.Change(0, UpdateInterval);
                }
            }
        }

        public void UnregisterParameters()
        {
            if (taID != 0)
                RDS.OnlineDataService.UnRegisterClient(taID);

            taID = 0;
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        private void TimerCallback(object state)
        {
            ParamValueItemWithID[] values;

            if (taID != 0)
            {
                try
                {
                    values = RDS.OnlineDataService.GetValuesFromBank(taID);

                    if (ParamValues != null)
                    {
                        lock (ParamValues)
                        {
                            ParamValues = values;
                        }
                    }
                    else
                        ParamValues = values;
                }
                catch (TransactionNotFoundException) { taID = 0; }
                catch (Exception)
                {
                    //TODO: записать в лог
                }
            }
        }
        #endregion

        protected override void DisposeProvider()
        {
            try
            {
                UnregisterParameters();
            }
            finally
            {
                //base.DisposeProvider();
            }
        }
    }
}
