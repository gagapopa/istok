using System;
using COTES.ISTOK.Modules;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Интерфейс для загрузки модулей сбора и информации о доступных модулях.
    /// Интерфейс нужен для инъекций в процесс загрузки модулей.
    /// </summary>
    public interface IModuleLoader
    {
        /// <summary>
        /// Получить информацию о всех доступных системе модулях сбора
        /// </summary>
        /// <returns></returns>
        ModuleInfo[] LoadInfo();

        /// <summary>
        /// Загрузитьт указанный модуль и вернуть объект IDataLoaderFactory для дальнейшей работы с модулем сбора.
        /// </summary>
        /// <param name="module">Информация о требуемом модуле</param>
        /// <returns>Объект IDataLoaderFactory загруженный из модуля</returns>
        IDataLoaderFactory LoadModule(ModuleInfo module);
    }
}