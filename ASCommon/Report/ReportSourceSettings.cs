using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Базовый класс настроек источника данных отчёта
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(TimeReportSourceSetting))]
    [KnownType(typeof(StructureReportSourceSettings))]
    [KnownType(typeof(SimpleReportSourceSettings))]
    [KnownType(typeof(ParameterReportSourceSetting))]
    [KnownType(typeof(ExtensionDataReportSourceSettings))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class ReportSourceSettings : IManualSerializable, ICloneable
    {
        /// <summary>
        /// Информация о источнике данных
        /// </summary>
        [DataMember]
        [System.ComponentModel.Browsable(false)]
        public ReportSourceInfo Info { get; protected set; }

        public ReportSourceSettings(ReportSourceInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Источник данных используется для данного узла
        /// </summary>
        [DataMember]
        [System.ComponentModel.Browsable(false)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Получить ИД требуемых источников данных 
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual Guid[] References
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Получить требуемые параметры отчёта
        /// </summary>
        /// <returns></returns>
        public virtual ReportParameter[] GetReportParameters()
        {
            return null;
        }

        [NonSerialized]
        private Dictionary<Guid, ReportSourceSettings> referencesDictionary;

        private Dictionary<Guid, ReportSourceSettings> ReferencesDictionary
        {
            get
            {
                if (referencesDictionary == null)
                    referencesDictionary = new Dictionary<Guid, ReportSourceSettings>();
                return referencesDictionary;
            }
        }

        /// <summary>
        /// Удовлетворить зависимость настройкой требуемого источника данных
        /// </summary>
        /// <param name="reportSourceID">ИД требуемого источника данных</param>
        /// <param name="referenceSettings">Его настройки</param>
        public void SetReference(Guid reportSourceID, ReportSourceSettings referenceSettings)
        {
            ReferencesDictionary[reportSourceID] = referenceSettings;
        }

        /// <summary>
        /// Получить настройки требуемого источника данных
        /// </summary>
        /// <param name="reportSourceID">ИД требуемого источника данных</param>
        /// <returns></returns>
        public ReportSourceSettings GetReference(Guid reportSourceID)
        {
            ReportSourceSettings settings;

            ReferencesDictionary.TryGetValue(reportSourceID, out settings);

            return settings;
        }

        #region IManualSerializable Members

        public abstract void FromBytes(byte[] bytes);

        public abstract byte[] ToBytes();

        #endregion

        #region ICloneable Members

        public abstract object Clone();

        #endregion
    }
}
