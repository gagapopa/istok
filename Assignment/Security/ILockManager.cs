using System;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    interface ILockManager
    {
        /// <summary>
        /// Взять узел на редактирование.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <returns>
        /// Если блокировка взята только что, возвращает true.
        /// Если блокировка взята ранее - false.
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        bool LockNode(OperationState state, UnitNode unitNode);

        /// <summary>
        /// Освободить редактируемый узел
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Редактируемы узел</param>
        void ReleaseNode(OperationState state, UnitNode unitNode);

        /// <summary>
        /// Взять узел на редактирования значений
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <param name="startTime">Начальное время редактируемых значений</param>
        /// <param name="endTime">Конечное время редактируемых значений</param>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        /// <exception cref="COTES.ISTOK.ASC.MultioLockException">Узел редактируется другим пользователем.</exception>
        void LockValues(OperationState state, UnitNode unitNode, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Освободить значения редактируемого узла за указанный интервал.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <param name="startTime">Начальное время редактируемых значений</param>
        /// <param name="endTime">Конечное время редактируемых значений</param>
        void ReleaseValues(OperationState state, UnitNode unitNode, DateTime startTime, DateTime endTime);
        
        /// <summary>
        /// Освободить указанный узел от редактирования и редактирования его значений.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Редактируемы узел</param>
        void ReleaseAll(OperationState state, UnitNode unitNode);
    }
}
