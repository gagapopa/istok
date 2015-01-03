using System;
using System.Data;
using System.Drawing;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using ZedGraph;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Провайдер для GraphUnitControl
    /// </summary>
    interface IGraphUnitProvider
    {
        #region Настройки
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        DateTime GraphFrom(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void GraphFrom(ExtensionDataInfo tableInfo, DateTime value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        DateTime GraphTo(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void GraphTo(ExtensionDataInfo tableInfo, DateTime value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        GraphTimePeriod GraphPeriod(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void GraphPeriod(ExtensionDataInfo tableInfo, GraphTimePeriod value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        int SortIndex(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void SortIndex(ExtensionDataInfo tableInfo, int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        bool RequestData(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void RequestData(ExtensionDataInfo tableInfo, bool value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        bool RemoteServer(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void RemoteServer(ExtensionDataInfo tableInfo, bool value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        bool ShowMarkers(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void ShowMarkers(ExtensionDataInfo tableInfo, bool value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        float SplitterDistance(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="value"></param>
        void SplitterDistance(ExtensionDataInfo tableInfo, float value);
        #endregion

        #region Доступ к значениям
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="tableInfo"></param>
        ///// <returns></returns>
        //bool DataReady(MegaTabInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        bool InProcess(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        int ParameterCount(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        DataTable DataTable(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        CurveList Curves(ExtensionDataInfo tableInfo);
        #endregion

        #region События
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<GraphUnitProviderEventArgs> LockControl;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<GraphUnitProviderEventArgs> UnlockControl;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<GraphUnitProviderEventArgs> ValuesChanged;

        event EventHandler<GraphUnitProviderEventArgs> ParameterListChanged;
        #endregion

        #region Управление значениями
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        void ClearValues(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="userRemoteServer"></param>
        void QueryHistogramData(ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="userRemoteServer"></param>
        void QueryGraphData(ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        Color MaxColor(ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        Color MinColor(ExtensionDataInfo tableInfo);

        /// <summary>
        /// Получить масштаб для заданной линнии
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="citem"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        bool GetScale(ExtensionDataInfo tableInfo, CurveItem citem, out double min, out double max);

        /// <summary>
        /// Получить полное наименование линии
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="citem"></param>
        /// <returns></returns>
        String GetFullName(ExtensionDataInfo tableInfo, CurveItem citem);
    }

    /// <summary>
    /// Аргументы для событий порождаемые IGraphUnitProvider 
    /// и хранящие в себе информацию о текущей вкладе
    /// </summary>
    public class GraphUnitProviderEventArgs : EventArgs
    {
        /// <summary>
        /// Вкладка, с которой связано событие
        /// </summary>
        public ExtensionDataInfo TableInfo { get; protected set; }

        public GraphUnitProviderEventArgs(ExtensionDataInfo tableInfo)
        {
            this.TableInfo = tableInfo;
        }
    }
}