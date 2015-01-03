using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Интерфейс для запроса стандартных (с точки зрения программы) 
    /// окон редактирования.
    /// </summary>
    public interface IUserInterfaceRequest
    {
        /// <summary>
        /// Запросить узел
        /// </summary>
        /// <param name="unitNode">Начальное значение</param>
        /// <returns></returns>
        UnitNode PromtUnitNode(UnitNode unitNode);

        /// <summary>
        /// Вывести окно выбора парамеров.
        /// </summary>
        /// <param name="caption">Загаловок окна</param>
        /// <param name="beginNodes">Начальный список параметров для редактирования</param>
        /// <returns></returns>
        IEnumerable<UnitNode> SelectNodes(String caption, IEnumerable<UnitNode> beginNodes);

        /// <summary>
        /// Вызвать выпадающий список для выбора нескольких типов узлов.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="beginTypes">Изначально выбранные узлы</param>
        /// <returns>Выбранные узлы</returns>
        int[] PromtUnitTypesDropDown(IServiceProvider provider, int[] beginTypes);
    }
}
