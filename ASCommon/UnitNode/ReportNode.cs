using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Узел FastReport-отчёта
    /// </summary>
    [Serializable]
    public class ReportNode : UnitNode
    {
        const String reportBodyAttributeName = "report";

        protected byte[] bodyBuffer = null;
        [Browsable(false)]
        public byte[] ReportBody
        {
            get
            {
                bodyBuffer = GetBinaries(reportBodyAttributeName);
                return bodyBuffer;
            }
            set
            {
                bodyBuffer = value;
                SetBinaries(reportBodyAttributeName, bodyBuffer);
            }
        }
        [Browsable(false)]
        public string FileName
        {
            get
            {
                return GetAttribute("report_cod");
            }
        }

        public ReportNode() : base() { }
        public ReportNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.Report) throw new Exception("Неверный тип узла");
        }

        const String reportSourceSettingPrefix = "rptsrc";

        /// <summary>
        /// Получить ИД источников данных, настройки для которых храняться в атрибутах узла
        /// </summary>
        /// <returns></returns>
        public Guid[] GetReportSourcesGuids()
        {
                int gostLength = reportSourceSettingPrefix.Length + System.Guid.Empty.ToString().Length;
                List<Guid> guidList = new List<Guid>();
                foreach (var item in Binaries.Keys)
                {
                    if (item.Length == gostLength
                        && reportSourceSettingPrefix.Equals(item.Substring(0, reportSourceSettingPrefix.Length)))
                        guidList.Add(new Guid(item.Substring(reportSourceSettingPrefix.Length)));
                }
                return guidList.ToArray();                
        }

        public override void AcceptChanges()
        {
            foreach (var guid in settingsDictionary.Keys)
            {
                //binaries[String.Format("{0}{1}", reportSourceSettingPrefix, guid)] = settingsDictionary[guid].ToBytes();
                SetBinaries(String.Format("{0}{1}", reportSourceSettingPrefix, guid), settingsDictionary[guid].ToBytes());
            }
        }

        /// <summary>
        /// Получить настройки источника данных отчёта в бинарном виде
        /// </summary>
        /// <param name="guid">ИД источника данных</param>
        /// <returns></returns>
        public byte[] GetReportSettingBinary(Guid guid)
        {
            //byte[] bytes;
            //binaries.TryGetValue(String.Format("{0}{1}", reportSourceSettingPrefix, guid), out bytes);

            //return bytes;
            return GetBinaries(String.Format("{0}{1}", reportSourceSettingPrefix, guid));
        }

        /// <summary>
        /// Словарь для хранения распакованных настроек источников данных
        /// </summary>
        Dictionary<Guid, ReportSourceSettings> settingsDictionary = new Dictionary<Guid, ReportSourceSettings>();

        /// <summary>
        /// Получить настройки источника данных отчёта
        /// </summary>
        /// <param name="guid">ИД источника данных</param>
        /// <returns></returns>
        public ReportSourceSettings GetReportSetting(Guid guid)
        {
            ReportSourceSettings ret;

            settingsDictionary.TryGetValue(guid, out ret);

            return ret;
        }

        /// <summary>
        /// Установить узлу настройки источника данных отчёта
        /// </summary>
        /// <param name="guid">ИД источника данных</param>
        /// <param name="setting"></param>
        public void SetReportSetting(Guid guid, ReportSourceSettings setting)
        {
            settingsDictionary[guid] = setting;
            //binaries[String.Format("{0}{1}", reportSourceSettingPrefix, guid)] = setting == null ? null : setting.ToBytes();
            SetBinaries(String.Format("{0}{1}", reportSourceSettingPrefix, guid), setting == null ? null : setting.ToBytes());
        }

        public override object Clone()
        {
            Object clonedObject = base.Clone();
            ReportNode reportNode = clonedObject as ReportNode;

            if (reportNode != null)
                reportNode.settingsDictionary = new Dictionary<Guid, ReportSourceSettings>(settingsDictionary);

            return clonedObject;
        }

        [NonSerialized]
        ReportParametersContainer parameterContainer;

        /// <summary>
        /// Контейнер для параметров отчета.
        /// Используется при редактировании отчета
        /// </summary>
        [Browsable(false)]
        public ReportParametersContainer ParameterContainer
        {
            get
            {
                if (parameterContainer == null)
                {
                    parameterContainer = new ReportParametersContainer();
                }
                return parameterContainer;
            }
        }
    }
}
