using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;
using COTES.ISTOK.Calc;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Шаблон руного ввода и расчета
    /// </summary>
    [Serializable]
    public class ParameterGateNode : UnitNode
    {
        const String intervalAttributeName = "interval";
        const String startTimeAttributeName = "start_time";


        /// <summary>
        /// Параметры шаблона
        /// </summary>
        [Browsable(false)]
        [DataMember]
        public List<ParameterNode> ManualParameters { get; set; }

        /// <summary>
        /// Получить входные аргументы для условных параметров
        /// </summary>
        /// <param name="getNodeFunction">Функция, возвращающая UnitNode по его идентификатору</param>
        /// <returns></returns>
        public virtual KeyValuePair<int, CalcArgumentInfo[]>[] GetArguments(RevisionInfo revision, Func<int, UnitNode> getNodeFunction)
        {
            UnitNode node;
            ParameterGateNode gateNode;

            node = getNodeFunction(ParentId);
            while (node != null)
            {
                if ((gateNode = node as ParameterGateNode) != null)
                    return gateNode.GetArguments(revision, getNodeFunction);
                if (node.ParentId > 0)
                {
                    node = getNodeFunction(node.ParentId);
                }
                else node = null;
            }

            return null;
        }

        /// <summary>
        /// получить интервал (с), с которым нужно заполнять параметры ручного ввода для данного шаблона
        /// </summary>
        [DisplayName("Интервал")]
        [CategoryOrder(CategoryGroup.Values)]
        [TypeConverter(typeof(IntervalTypeConverter))]
        public Interval Interval
        {
            get
            {
                Interval inter;
                try
                {
                    inter = Interval.FromString(GetAttribute(intervalAttributeName));
                    //inter = new Interval(Convert.ToDouble(GetAttribute(intervalAttributeName), System.Globalization.NumberFormatInfo.InvariantInfo));
                }
                catch { inter = Interval.Zero; }
                return inter;
            }
            set
            {
                //SetAttribute(intervalAttributeName, doubleconv.ConvertToInvariantString(value.ToDouble()));
                SetAttribute(intervalAttributeName, value.ToString());
            }
        }

        ///// <summary>
        ///// Время, используемое для сдвига периодов.
        ///// Например, что бы сутки были от 4 до 4 часов, время в  StartTime выставляется в 4 часа.
        ///// </summary>       
        //[DisplayName("Время запуска")]
        //[Category("Расчет")]
        //[Description("Дата и время начала расчета, так же применяется для смещения времени")]
        //public DateTime StartTime
        //{
        //    get
        //    {
        //        DateTime res;

        //        if (DateTime.TryParse(GetAttribute(startTimeAttributeName), out res))
        //            return res;

        //        return DateTime.MinValue;
        //    }
        //    set
        //    {
        //        SetAttribute(startTimeAttributeName, value.ToString());
        //    }
        //}

        /// <summary>
        /// возврашает false, если ручной ввод ведется по последнему значению
        /// true, если вводиться с определенным интервалом
        /// </summary>
        [Browsable(false)]
        public bool IsReg { get { return Interval != Interval.Zero; } }

        public ParameterGateNode() : base() { }
        public ParameterGateNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.ManualGate
                && Typ != (int)UnitTypeId.TEPTemplate
                && Typ != (int)UnitTypeId.OptimizeCalc)
                throw new Exception("Неверный тип узла");

            //modified = false;
        }

        public override bool Equals(object obj)
        {
            ParameterGateNode gateNode = obj as ParameterGateNode;
            if (gateNode == null) return false;
            if (!base.Equals(obj)) return false;

            bool eq = gateNode.nodesIds.Count.Equals(nodesIds.Count);
            for (int i = 0; eq && i < nodesIds.Count; i++)
                eq = gateNode.nodesIds[i].Equals(nodesIds[i]);
            return eq;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
