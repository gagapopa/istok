using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client.Extension
{
    /// <summary>
    /// Состояние униформы для расширения
    /// </summary>
    public interface IUniForm
    {
        /// <summary>
        /// Получить узел по элементу из дерева StructureTree
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        UnitNode GetUnitNode(TreeNode treeNode);

        /// <summary>
        /// Дерево структуры
        /// </summary>
        TreeView StructureTree { get; }

        /// <summary>
        /// Панель, на котором расположенно дерево
        /// </summary>
        Panel StructurePanel { get; }
    }

    /// <summary>
    /// Интерфейс для пункта меню, которое может быть добавлено из расширения
    /// </summary>
    public interface IISTOKMenuItem
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Всплывающая подсказка
        /// </summary>
        String ToolTip { get; }

        /// <summary>
        /// Имя родительского меню, если данный пункт меню должен находиться в подменю
        /// </summary>
        String UberMenu { get; }

        /// <summary>
        /// Является ли данный пункт меню активным
        /// </summary>
        Task<bool> GetEnabled();

        /// <summary>
        /// Установлен ли элемент
        /// </summary>
        bool Checked { get; }

        /// <summary>
        /// Действия по нажатию на пункт меню
        /// </summary>
        void Click();
    }

    /// <summary>
    /// Интерфейс для внешних расширений
    /// </summary>
    public interface IClientExtension
    {
        /// <summary>
        /// Состояние клиента
        /// </summary>
        IClientState State { get; set; }

        /// <summary>
        /// Строка состояния из расширения
        /// </summary>
        String StatusString { get; }

        /// <summary>
        /// Событие, возникающие, при изменении строки состояния из расширения
        /// </summary>
        event EventHandler StatusStringChanged;

        /// <summary>
        /// Получить дополнительные пункты меню в главное меню
        /// </summary>
        /// <returns></returns>
        IISTOKMenuItem[] MainMenuExt();

        ///// <summary>
        ///// Получить дополнительные пункты меню в контекстное меню
        ///// </summary>
        ///// <param name="uniForm"></param>
        ///// <returns></returns>
        //IISTOKMenuItem[] ContextMenuExt(UniForm uniForm);
    }
}
