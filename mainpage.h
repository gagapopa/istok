/*
 * Файл для генератора документации.
 * Содержит описание пространств имён
 * и главную страницу с общими положениями проекта.
 */

/* Главная страница и её подстраницы */
/**
 @mainpage ПК «ИСТОК-СБК»
 @section intro Введение
 Это вообще мегасистема.
 @section sec_descr Описание
 Подробней о том, что здесь твориться:
 @li @subpage pg_distrib.
 @li @subpage pg_architecture.
 @li @subpage pg_structure.
 @li @subpage pg_sysextension.
 @li @subpage pg_calc.
 
 @page pg_distrib Получение, Компиляция, Установка
 
 @section sec_get_build Получение и компиляция
 @par
 Исходники (на момент написания данной страницы) доступны через SVN по адресу http://192.168.201.220/svn/istok/trunk/ .
 @par
 Программа написана на C# .NET 3.5. 
 @par
 Для компиляции потребуются следующие утилиты:
 @li TortoiseSVN, утилита SubWCRev -- добавляет номер ревизии в номер версии проекта.
 Если не работает, можно, вручную, создать файл Version.cs из Version.cs.template в проекте CommonLib, заменив $WCREV$ на какое ни будь число и удалить build events у проекта CommonLib;
 @li Windows Installer XML (WiX) -- пакет для создания установщика. Возможно без него солюшен не откроется;
 @li Dotfuscator -- обфускация
 @li NUnit -- фреймворк для тестов.
 @par
 Следующие библиотеки включены в репозиторий:
 @li ZedGraph -- библиотека для рисования всяких графиков;
 @li FastReport -- генератор отчета;
 @li DocumentFormat.OpenXml -- библиотека для импорта из MS Office 2007+;
 @li Moq -- изоляционный фреймворк (для тестов).
 @par Компиляция
 Для компиляции программы, просто откройте файл солюшена VTGRES.sln и скомпилируйте солюшен.
 Для запуска тестов смотриете @ref sec_tests "следующий раздел".
 Для подготовки установщика смотрите @ref sec_installer "в соответствующем разделе".
 
 @section sec_tests Тестирование
 @par Запуск тестов
 Для запуска тестов, необходимо воспользоваться графическим интерфейсом nunit.exe и открыть файл ISTOK.nunit - в котором перечисленны сборки с тестами.
 @par Запуск тестов в режиме отладки
 Запустить тесты в режиме отладки можно следующим образом:
 @li открыть свойства одного из проектов с тестами;
 @li открыть вкладку "Debug";
 @li в "Start Action" выбрать "Start external program";
 @li выбрать путь к nunit.exe, например, "C:\Program Files\NUnit 2.6.1\bin\nunit.exe";
 @li "Working directory" выбрать корневую папку с солюшеном;
 @li "Command line arguments" ввести "ISTOK.nunit /run";
 @li запустить проект для отладки.
 
 @section sec_installer Подготовка установщика
 @par
 Для создания установщика в конфигурации Debug просто скомпилируйте проект ISTOKWixInstaller.
 @par
 Для создания установщика в конфигурации Release или DKSMRelease:
 @li скомпилируйте весть солющен;
 @li запустите Dotfuscator и откройте настройки в корне солюшена: dotfuscate.xml - для Release и dksmuscate.xml - DKSMRelease;
 @li запустите процесс обфускации;
 @li по завершении обфускации скомпилировать проект ISTOKWixInstaller.
 @par
 Установщик будет находиться по следующему пути:
 $(SolutionDir)IstokWixInstaller\\bin\\x86\\$(Configuration)\\ru-RU\\
 
 @page pg_architecture Архитектура системы
 @tableofcontents
 Система состоит из нескольких уровней: @ref sec_loader "блочного", @ref sec_station "глобала" и @ref sec_client "клиента"
 @section sec_loader Сервер сбора (блочный)
 Занимается сбором данных с оборудования и дальнейшей передачи её на глобал
 @subsection sec_tunnel Туннель модулей сбора
 Очень хитрая штука для обхода ограничений по работе в службе или если доступ к данным производиться только на локальном компьютере
 @section sec_station Станционный сервер (глобал)
 Работает со структурой системы, производит расчёт и прочие мелкие ништяки
 @section sec_client Интерфейсный модуль (Клиент)
 Отображает всю эту чушь
 @section sec_Webclient Тонкий клиент
 то же

 @page pg_structure Структура данных
 @tableofcontents
 Состоит из UnitNode'ов
 
 @page pg_sysextension Расширение системы
 @tableofcontents
 @section sec_modules Модули сбора
 @par
 COTES.ISTOK.Modules.BaseModule
 @section sec_extension Дополнительные модули
 @par
 Базовым для расширения функционала является интерфейс COTES.ISTOK.Extension.IExtension.
 @section sec_report Источники данных отчёта
 @par
 Для добавления своего источника данных необходимо:
 @li Реализовать интерфейс COTES.ISTOK.Assignment.IReportSource, который будет подготавливать для отчёта данные;
 @li Пронаследоваться от COTES.ISTOK.ASC.ReportSourceSettings для хранения настроек источника данных, 
 или воспользоваться COTES.ISTOK.ASC.SimpleReportSourceSettings, если никакие дополнительные настройки не требуются;
 @li Пронаследоваться от COTES.ISTOK.Client.ReportSourceEditControl и зарегистрировать класс в COTES.ISTOK.Client.ReportSourceManager.
 Для настройки COTES.ISTOK.ASC.SimpleReportSourceSettings текущего источника данных;
 @li COTES.ISTOK.ASC.SimpleReportSourceSettings.GetReportParameters() должен возвращать COTES.ISTOK.ASC.ReportParameter -- 
 параметры, указываемые при формировании отчёта.
 
 @page pg_calc Подсистема расчёта
 @tableofcontents
 На столько мудренная, что сам чёрт ногу сломит
**/

/* Описание пространств имен */
/**
 @namespace COTES.ISTOK
 @brief Главное пространство имен для ПК. Так же тут сложены общие классы
 @namespace COTES.ISTOK.ASC
 @brief Связка глобла и клиента, Описание всей структуры
 @namespace COTES.ISTOK.Assignment
 @brief Логика глобала
 @namespace COTES.ISTOK.Block
 @brief Логика блочного
 @namespace COTES.ISTOK.Client
 @brief Клиент
 @namespace COTES.ISTOK.Modules
 @brief Общие пространство имен для модулей сбора и типы данных используемые модулями
 @namespace COTES.ISTOK.Modules.Tunnel
 @brief Туннель для сбора
 @namespace COTES.ISTOK.Calc
 @brief Подсистема расчёта распологается здесь
 @namespace COTES.ISTOK.Test
 @brief Пространство имён для юнит тестов
 @namespace COTES.ISTOK.Tests.ASC
 @brief Юнит-тесты сборки ASC
 @namespace COTES.ISTOK.Tests.ASC.Report
 @brief 
 @namespace COTES.ISTOK.Tests.Assignment
 @brief Юнит тести сборки Assignment
 @namespace COTES.ISTOK.Tests.Assignment.Reports
 @brief 
 @namespace COTES.ISTOK.Tests.Calc
 @brief Юнит тексты расчёта
**/
