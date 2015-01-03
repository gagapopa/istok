using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using System.Drawing;
using ZedGraph;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Класс для хранения настроек вкладки графика
    /// </summary>
    public class GraphUnitProviderState
    {
        #region GraphFrom
        /// <summary>
        /// Начальное время графика
        /// </summary>
        DateTime graphFrom;

        /// <summary>
        /// Получить, установить начальное время графика
        /// </summary>
        public DateTime GraphFrom
        {
            get { return graphFrom; }
            set
            {
                graphFrom = value;
                OnGraphFromChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении GraphFrom
        /// </summary>
        public event EventHandler GraphFromChanged;

        /// <summary>
        /// Вызвать событие GraphFromChanged
        /// </summary>
        protected virtual void OnGraphFromChanged()
        {
            if (GraphFromChanged != null)
                GraphFromChanged(this, EventArgs.Empty);
        }
        #endregion

        #region GraphTo
        /// <summary>
        /// Конечное время графика
        /// </summary>
        DateTime graphTo;

        /// <summary>
        /// Получить, установить конечное время графика
        /// </summary>
        public DateTime GraphTo
        {
            get { return graphTo; }
            set
            {
                graphTo = value;
                OnGraphToChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении GraphTo
        /// </summary>
        public event EventHandler GraphToChanged;

        /// <summary>
        /// Вызвать событие GraphToChanged
        /// </summary>
        protected virtual void OnGraphToChanged()
        {
            if (GraphToChanged != null)
                GraphToChanged(this, EventArgs.Empty);
        }
        #endregion

        #region GraphPeriod
        /// <summary>
        /// Запрашиваемый период
        /// </summary>
        GraphTimePeriod graphPeriod;

        /// <summary>
        /// Получить, установить запрашиваемый период
        /// </summary>
        public GraphTimePeriod GraphPeriod
        {
            get { return graphPeriod; }
            set
            {
                graphPeriod = value;
                OnGraphPeriodChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении GraphPeriod
        /// </summary>
        public event EventHandler GraphPeriodChanged;

        /// <summary>
        /// Вызвать событие GraphPeriodChanged
        /// </summary>
        protected virtual void OnGraphPeriodChanged()
        {
            if (GraphPeriodChanged != null)
                GraphPeriodChanged(this, EventArgs.Empty);
        }
        #endregion

        #region SortIndex
        /// <summary>
        /// 
        /// </summary>
        int sortIndex;

        /// <summary>
        /// Получить, установить 
        /// </summary>
        public int SortIndex
        {
            get { return sortIndex; }
            set
            {
                sortIndex = value;
                OnSortIndexChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении SortIndex
        /// </summary>
        public event EventHandler SortIndexChanged;

        /// <summary>
        /// Вызвать событие SortIndexChanged
        /// </summary>
        protected virtual void OnSortIndexChanged()
        {
            if (SortIndexChanged != null)
                SortIndexChanged(this, EventArgs.Empty);
        }
        #endregion

        #region RequestData
        /// <summary>
        /// Флаг запроса данных
        /// </summary>
        bool requestData;

        /// <summary>
        /// Получить, установить флаг запроса данных
        /// </summary>
        public bool RequestData
        {
            get { return requestData; }
            set
            {
                requestData = value;
                OnRequestDataChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении RequestData
        /// </summary>
        public event EventHandler RequestDataChanged;

        /// <summary>
        /// Вызвать событие RequestDataChanged
        /// </summary>
        protected virtual void OnRequestDataChanged()
        {
            if (RequestDataChanged != null)
                RequestDataChanged(this, EventArgs.Empty);
        }
        #endregion

        #region RemoteServer
        /// <summary>
        /// Флаг обращения к удаленному серверу
        /// </summary>
        bool remoteServer;

        /// <summary>
        /// Получить, установить флаг обращения к удаленному серверу
        /// </summary>
        public bool RemoteServer
        {
            get { return remoteServer; }
            set
            {
                remoteServer = value;
                OnRemoteServerChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении RemoteServer
        /// </summary>
        public event EventHandler RemoteServerChanged;

        /// <summary>
        /// Вызвать событие RemoteServerChanged
        /// </summary>
        protected virtual void OnRemoteServerChanged()
        {
            if (RemoteServerChanged != null)
                RemoteServerChanged(this, EventArgs.Empty);
        }
        #endregion

        #region ShowMarker
        /// <summary>
        /// Флаг отображения маркеров на графике
        /// </summary>
        bool showMarker;

        /// <summary>
        /// Получить, установить флаг отображения маркеров на графике
        /// </summary>
        public bool ShowMarker
        {
            get { return showMarker; }
            set
            {
                showMarker = value;
                OnShowMarkerChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении ShowMarker
        /// </summary>
        public event EventHandler ShowMarkerChanged;

        /// <summary>
        /// Вызвать событие ShowMarkerChanged
        /// </summary>
        protected virtual void OnShowMarkerChanged()
        {
            if (ShowMarkerChanged != null)
                ShowMarkerChanged(this, EventArgs.Empty);
        }
        #endregion

        #region SplitterDistance
        /// <summary>
        /// Относительная ширина таблицы с данными
        /// </summary>
        float splitterDistance;

        /// <summary>
        /// Получить, установить относительную ширину таблицы с данными
        /// </summary>
        public float SplitterDistance
        {
            get { return splitterDistance; }
            set
            {
                splitterDistance = value;
                OnSplitterDistanceChanged();
            }
        }

        /// <summary>
        /// Событие возникающие при изменении SplitterDistance
        /// </summary>
        public event EventHandler SplitterDistanceChanged;

        /// <summary>
        /// Вызвать событие SplitterDistanceChanged
        /// </summary>
        protected virtual void OnSplitterDistanceChanged()
        {
            if (SplitterDistanceChanged != null)
                SplitterDistanceChanged(this, EventArgs.Empty);
        }
        #endregion

        #region OnPage
        int onPage = 5;
        /// <summary>
        /// Количество отображаемых параметров
        /// </summary>
        public int OnPage 
        {
            get { return onPage; }
            set { if (value > 0 && value < 11) onPage = value; }
        }
        #endregion

        /// <summary>
        /// Инициировать значениями по умолчанию
        /// </summary>
        public GraphUnitProviderState()
        {
            GraphPeriod = GraphTimePeriod.Minutes30;

            GraphTo = DateTime.Now;

            GraphFrom = GraphTo.AddSeconds(-GraphTimePeriodFormatter.FormatInterval(GraphPeriod));

            RequestData = false;

            RemoteServer = false;

            ShowMarker = false;

            SplitterDistance = 0;
        }

        /// <summary>
        /// Инициировать как копию заданного
        /// </summary>
        /// <param name="y"></param>
        public GraphUnitProviderState(GraphUnitProviderState y)
        {
            GraphFrom = y.GraphFrom;

            GraphTo = y.GraphTo;

            GraphPeriod = y.GraphPeriod;

            RequestData = y.RequestData;

            RemoteServer = y.RemoteServer;

            ShowMarker = y.ShowMarker;

            SplitterDistance = y.SplitterDistance;
        }
    }

    /// <summary>
    /// Базовый провайдер для отображения многовкладочных графиков
    /// </summary>
    public abstract class CommonGraphUnitProvider : UnitProvider
    {
        public CommonGraphUnitProvider(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {

        }

        #region Настройки
        /// <summary>
        /// Настройки графика по умолчанию. 
        /// Настройки обновляются по последней активной вкладке
        /// </summary>
        static GraphUnitProviderState defaultState = new GraphUnitProviderState();

        /// <summary>
        /// Хранилище настроек текущих вкладок
        /// </summary>
        Dictionary<String, GraphUnitProviderState> stateDictionary = new Dictionary<String, GraphUnitProviderState>();

        /// <summary>
        /// Получить настройки для указанной вкладки.
        /// Если настройки нет, создаст настройки по умолчанию.
        /// </summary>
        /// <param name="tableInfo">Информация о вкладке</param>
        /// <returns></returns>
        public GraphUnitProviderState GetState(ExtensionDataInfo tableInfo)
        {
            GraphUnitProviderState state;

            String tableName = tableInfo == null ? String.Empty : tableInfo.Name;

            if (!stateDictionary.TryGetValue(tableName, out state))
                stateDictionary[tableName] = state = InitState();

            return state;
        }

        /// <summary>
        /// Создаёт новый объект настройки и заполняет его свойства значениями по умолчанию.
        /// Так же подписывается на события нового объекта и при изменении настроей, обновляются значения по умолчанию.
        /// </summary>
        /// <returns></returns>
        private GraphUnitProviderState InitState()
        {
            GraphUnitProviderState state = new GraphUnitProviderState(defaultState);

            //state.GraphFromChanged += new EventHandler((s, e) => defaultState.GraphFrom = (s as GraphUnitProviderState).GraphFrom);
            //state.GraphToChanged += new EventHandler((s, e) => defaultState.GraphTo = (s as GraphUnitProviderState).GraphTo);
            //state.GraphPeriodChanged += new EventHandler((s, e) => defaultState.GraphPeriod = (s as GraphUnitProviderState).GraphPeriod);
            //state.SortIndexChanged += new EventHandler((s, e) => defaultState.SortIndex = (s as GraphUnitProviderState).SortIndex);
            //state.RequestDataChanged += new EventHandler((s, e) => defaultState.RequestData = (s as GraphUnitProviderState).RequestData);
            //state.RemoteServerChanged += new EventHandler((s, e) => defaultState.RemoteServer = (s as GraphUnitProviderState).RemoteServer);
            //state.ShowMarkerChanged += new EventHandler((s, e) => defaultState.ShowMarker = (s as GraphUnitProviderState).ShowMarker);
            //state.SplitterDistanceChanged += new EventHandler((s, e) => defaultState.SplitterDistance = (s as GraphUnitProviderState).SplitterDistance);

            return state;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract bool InProcess(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract int ParameterCount(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract System.Data.DataTable DataTable(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract CurveList Curves(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<GraphUnitProviderEventArgs> LockControl;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<GraphUnitProviderEventArgs> UnlockControl;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<GraphUnitProviderEventArgs> ValuesChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<GraphUnitProviderEventArgs> ParameterListChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLockControl(GraphUnitProviderEventArgs e)
        {
            if (LockControl != null)
                LockControl(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUnlockControl(GraphUnitProviderEventArgs e)
        {
            if (UnlockControl != null)
                UnlockControl(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValuesChanged(GraphUnitProviderEventArgs e)
        {
            if (ValuesChanged != null)
                ValuesChanged(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParameterListChanged(GraphUnitProviderEventArgs e)
        {
            if (ParameterListChanged != null)
                ParameterListChanged(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        public abstract void ClearValues(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="userRemoteServer"></param>
        public abstract void QueryHistogramData(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="userRemoteServer"></param>
        public abstract void QueryGraphData(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract System.Drawing.Color MaxColor(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract System.Drawing.Color MinColor(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="citem"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public abstract bool GetScale(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo, CurveItem citem, out double min, out double max);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="citem"></param>
        /// <returns></returns>
        public abstract string GetFullName(COTES.ISTOK.Extension.ExtensionDataInfo tableInfo, CurveItem citem);
    }

    public enum GraphTimePeriod
    {
        Minutes30,
        Hours1,
        Hours4,
        Days1,
        Days30,
        User
    }
    public static class GraphTimePeriodFormatter
    {
        public static GraphTimePeriod Format(string period)
        {
            switch (period.ToLower())
            {
                case "30 минут": return GraphTimePeriod.Minutes30;
                case "1 час": return GraphTimePeriod.Hours1;
                case "4 часа": return GraphTimePeriod.Hours4;
                case "сутки": return GraphTimePeriod.Days1;
                case "30 суток": return GraphTimePeriod.Days30;
                case "выбор": return GraphTimePeriod.User;
                default:
                    throw new NotSupportedException(period);
            }
        }
        /// <summary>
        /// Преобразует интервал в секунды
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double FormatInterval(GraphTimePeriod period)
        {
            switch (period)
            {
                case GraphTimePeriod.Minutes30: return 1800f;
                case GraphTimePeriod.Hours1: return 3600f;
                case GraphTimePeriod.Hours4: return 14400f;
                case GraphTimePeriod.Days1: return 86400f;
                case GraphTimePeriod.Days30: return 2592000f;
                case GraphTimePeriod.User: return 0f;
                default:
                    throw new NotSupportedException(period.ToString());
            }
        }
        public static string Format(GraphTimePeriod period)
        {
            switch (period)
            {
                case GraphTimePeriod.Minutes30: return "30 минут";
                case GraphTimePeriod.Hours1: return "1 час";
                case GraphTimePeriod.Hours4: return "4 часа";
                case GraphTimePeriod.Days1: return "Сутки";
                case GraphTimePeriod.Days30: return "30 суток";
                case GraphTimePeriod.User: return "Выбор";
                default:
                    throw new NotSupportedException(period.ToString());
            }
        }
    }
}
