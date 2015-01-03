using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    [Flags]
    public enum NodeBackState
    {
        Unknown = 0, Failed = 1, Success = 2, Blocked = 4, Middle = Failed | Success
    }

    /// <summary>
    /// Флаг достоверности
    /// </summary>
    [Flags]
    public enum NodeReliabilityState
    {
        Unknown = 0, NoValue = 1, UnReliability = 2, Reliability = 4
    }

    /// <summary>
    /// Информация о ходе расчета параметра и его сотояние
    /// </summary>
    [Serializable]
    public class NodeBack
    {
        private NodeBackState state;
        private NodeReliabilityState relState;
        private TimeValueStructure timeValue;

        public NodeBack(int parameterID, NodeBackState state)
        {
            ParameterID = parameterID;
            this.state = state;
        }

        public int ParameterID { get; protected set; }

        /// <summary>
        /// Состояние расчета
        /// </summary>
        public NodeBackState State
        {
            get { return state; }
        }

        public NodeReliabilityState ReliabilityState
        {
            get { return relState; }
            set { relState = value; }
        }

        /// <summary>
        /// Время значения
        /// </summary>
        public TimeValueStructure TimeValue
        {
            get { return timeValue; }
            set { timeValue = value; }
        }
    }

    /// <summary>
    /// Структура, для хранения времени значения.
    /// </summary>
    [Serializable]
    public struct TimeValueStructure
    {
        private DateTime valueTime;
        private double valueValue;

        public TimeValueStructure(DateTime time, double value)
        {
            valueTime = time;
            valueValue = value;
        }

        /// <summary>
        /// Время
        /// </summary>
        public DateTime Time
        {
            get { return valueTime; }
            set { valueTime = value; }
        }

        /// <summary>
        /// Значения
        /// </summary>
        public double Value
        {
            get { return valueValue; }
            set { valueValue = value; }
        }
    }
}
