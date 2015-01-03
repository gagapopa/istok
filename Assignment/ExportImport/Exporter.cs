using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using COTES.ISTOK;
using System.Xml;
using System.IO;
using COTES.ISTOK.ASC;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Базовый класс для экспорта/импорта
    /// </summary>
    abstract class Exporter
    {
        ///// <summary>
        ///// Создать объект экспорта/импорта по формату файла
        ///// </summary>
        ///// <param name="unitManager"></param>
        ///// <param name="format"></param>
        ///// <returns></returns>
        //public static Exporter GetExporter(UnitManager unitManager, SecurityManager securityManager, ExportFormat format)
        //{
        //    switch (format)
        //    {
        //        case ExportFormat.XML:
        //            return new XmlExporter(unitManager, securityManager, false);
        //        case ExportFormat.ZippedXML:
        //            return new XmlExporter(unitManager, securityManager, true);
        //        case ExportFormat.Excel:
        //            return new ExcelExporter();
        //        case ExportFormat.WordX:
        //            return new WordExporter(unitManager);
        //        default:
        //            throw new InvalidOperationException();
        //    }
        //}

        /// <summary>
        /// Сереализовать узлы и значения
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="nodes">Сереализуемые узлы</param>
        /// <param name="nodeCount">Количетсво узлов</param>
        /// <param name="values">Сереализуемые значения</param>
        /// <returns>Содержимое файла</returns>
        public abstract byte[] Serialize(OperationState state, TreeWrapp<UnitNode>[] nodes, int nodeCount, ParameterNode[] parameters, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Десереализовать ущлы и значения
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="importBuffer">Содержимое фала</param>
        /// <param name="values">Десереализованные значения</param>
        /// <returns>Узлы</returns>
        public abstract ImportDataContainer Deserialize(OperationState state, byte[] importBuffer);       
    }
}
