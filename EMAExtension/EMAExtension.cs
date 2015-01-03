using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using EMA.ASTDK.Data;

namespace COTES.ISTOK.EMA
{
    /// <summary>
    /// Расширение ДКСМ-Клиента для реализации библиотеки обмена
    /// </summary>
    public class EMAExtension : IExtension
    {
        /// <summary>
        /// 
        /// </summary>
        IMonitoringData monitoringData;

        /// <summary>
        /// 
        /// </summary>
        public IExtensionSupplier Supplier { get; set; }

        public EMAExtension()
        {
            this.monitoringData = new MonitoringData();
        }

        /// <summary>
        /// 
        /// </summary>
        public String Caption { get { return "АСТДК"; } }

        /// <summary>
        /// GUID типа станции, предоставляемый данным расширением
        /// </summary>
        static readonly Guid PlantTypeGUID = new Guid("{AA582860-5F10-4e72-935F-1E200F2787A3}");

        /// <summary>
        /// GUID типа котла, предоставляемый данным расширением
        /// </summary>
        static readonly Guid BoilerTypeGUID = new Guid("{60A7DA28-684D-435c-9911-625310B2E045}");

        /// <summary>
        /// GUID типа барабан, предоставляемый данным расширением
        /// </summary>
        static readonly Guid DrumTypeGUID = new Guid("{E181EA80-E676-4017-9A1B-775BB309CA3D}");

        /// <summary>
        /// GUID типа DrumNozzle (штуцер), предоставляемый данным расширением
        /// </summary>
        static readonly Guid DrumNozzleGUID = new Guid("{FFD07A09-EDBC-43e7-907F-584E545979E4}");

        /// <summary>
        /// GUID типа StraightPipe (прямая труба), предоставляемый данным расширением
        /// </summary>
        static readonly Guid StraightPipeGUID = new Guid("{D4795181-1636-4467-899F-ABAE276A2984}");

        /// <summary>
        /// GUID типа Bend (гиб), предоставляемый данным расширением
        /// </summary>
        static readonly Guid BendGUID = new Guid("{1743DE5D-44E5-4620-A54F-78B077DC9AE7}");

        /// <summary>
        /// 
        /// </summary>
        public UTypeNode[] ProvidedTypes
        {
            get
            {
                UTypeNode plantType = new UTypeNode();

                plantType.Text = "Станция";
                plantType.ExtensionGUID = PlantTypeGUID;
                plantType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.PlantIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    plantType.Icon = ms.ToArray();
                }

                UTypeNode boilerType = new UTypeNode();
                boilerType.Text = "Котёл";
                boilerType.ExtensionGUID = BoilerTypeGUID;
                boilerType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.BoilerIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    boilerType.Icon = ms.ToArray();
                }

                UTypeNode drumType = new UTypeNode();
                drumType.Text = "Барабан";
                drumType.ExtensionGUID = DrumTypeGUID;
                drumType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.DrumIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    drumType.Icon = ms.ToArray();
                }

                UTypeNode drumNozzleType = new UTypeNode();
                drumNozzleType.Text = "Штуцер барабана";
                drumNozzleType.ExtensionGUID = DrumNozzleGUID;
                drumNozzleType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.DrumNozzleIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    drumNozzleType.Icon = ms.ToArray();
                }

                UTypeNode straightPipeType = new UTypeNode();
                straightPipeType.Text = "Прямая труба";
                straightPipeType.ExtensionGUID = StraightPipeGUID;
                straightPipeType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.StraightPipeIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    straightPipeType.Icon = ms.ToArray();
                }

                UTypeNode bendType = new UTypeNode();
                bendType.Text = "Гиб";
                bendType.ExtensionGUID = BendGUID;
                bendType.ChildFilterAll = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.BendIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bendType.Icon = ms.ToArray();
                }


                return new UTypeNode[] { plantType, boilerType, drumType, drumNozzleType, straightPipeType, bendType };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ExtensionUnitNode NewUnitNode(UTypeNode type)
        {
            ExtensionUnitNode unitNode = new ExtensionUnitNode();
            unitNode.Typ = type.Idnum;
            unitNode.ExternalID = -1;
            return unitNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public ExtensionUnitNode NewUnitNode(UTypeNode type, DataRow row)
        {
            return new ExtensionUnitNode(row);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        public bool ListSupported(UTypeNode typeNode)
        {
            return typeNode.ExtensionGUID == PlantTypeGUID
                || typeNode.ExtensionGUID == BoilerTypeGUID;
        }

        public bool CodeSupported(UTypeNode typeNode)
        {
            //return typeNode.ExtensionGUID == DrumTypeGUID
            //    || typeNode.ExtensionGUID == DrumNozzleGUID
            //    || typeNode.ExtensionGUID == StraightPipeGUID
            //    || typeNode.ExtensionGUID == BendGUID;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        public bool ListCanAdd(UTypeNode typeNode)
        {
            return typeNode.ExtensionGUID == PlantTypeGUID
                || typeNode.ExtensionGUID == BoilerTypeGUID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitNode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityStruct[] GetList(ExtensionUnitNode unitNode, UTypeNode type)
        {
            Guid typeGUID = type.ExtensionGUID;

            if (typeGUID == PlantTypeGUID)
            {
                const String plantIDString = "Id";
                const String plantNameString = "Name";
                List<EntityStruct> entityList = new List<EntityStruct>();
                DataTable plantListTable = monitoringData.GetListPlant();

                foreach (DataRow plantListRow in plantListTable.Rows)
                    entityList.Add(new EntityStruct(Convert.ToInt32(plantListRow[plantIDString]), plantListRow[plantNameString].ToString()));

                return entityList.ToArray();
            }
            else if (typeGUID == BoilerTypeGUID)
            {
                ExtensionUnitNode parent = Supplier.GetParent(unitNode, PlantTypeGUID);

                if (parent != null)
                {
                    const String boilerIDString = "Id";
                    const String boilerNameString = "Name";

                    List<EntityStruct> entityList = new List<EntityStruct>();
                    DataTable boilerListTable = monitoringData.GetListBoiler(parent.ExternalID);

                    foreach (DataRow boilerListRow in boilerListTable.Rows)
                        entityList.Add(new EntityStruct(Convert.ToInt32(boilerListRow[boilerIDString]), boilerListRow[boilerNameString].ToString()));

                    return entityList.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitNode"></param>
        public void NotifyChange(ExtensionUnitNode unitNode)
        {
            if (unitNode.ExternalID < 0)
                return;

            UTypeNode typeNode = Supplier.GetTypeNode(unitNode.Typ);

            if (typeNode.ExtensionGUID == PlantTypeGUID)
            {
                int plantID = unitNode.ExternalID;

                monitoringData.AddInfoPlant(unitNode.Idnum, unitNode.ExternalCode, ref plantID);
                unitNode.ExternalID = plantID;
            }
            else if (typeNode.ExtensionGUID == BoilerTypeGUID)
            {
                int boilerID = unitNode.ExternalID;
                ExtensionUnitNode plantUnitNode = Supplier.GetParent(unitNode, PlantTypeGUID);

                if (plantUnitNode != null && plantUnitNode.ExternalID > 0)
                {
                    monitoringData.AddInfoBoiler(plantUnitNode.ExternalID, unitNode.Idnum, unitNode.ExternalCode, ref boilerID);
                    unitNode.ExternalID = boilerID;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitNode"></param>
        /// <returns></returns>
        public ItemProperty[] GetProperties(ExtensionUnitNode unitNode)
        {
            ExtensionUnitNode boilerNode = Supplier.GetParent(unitNode, BoilerTypeGUID);
            DataTable propertiesTable = null;
            UTypeNode typeNode = Supplier.GetTypeNode(unitNode.Typ);

            if (typeNode.ExtensionGUID == DrumTypeGUID)
                propertiesTable = monitoringData.GetDrum(boilerNode.ExternalID, unitNode.ExternalCode);
            else if (typeNode.ExtensionGUID == DrumNozzleGUID)
                propertiesTable = monitoringData.GetDrumNozzle(boilerNode.ExternalID, unitNode.ExternalCode);
            else if (typeNode.ExtensionGUID == StraightPipeGUID)
                propertiesTable = monitoringData.GetStraightPipe(boilerNode.ExternalID, unitNode.ExternalCode);
            else if (typeNode.ExtensionGUID == BendGUID)
                propertiesTable = monitoringData.GetBend(boilerNode.ExternalID, unitNode.ExternalCode);

            if (propertiesTable != null)
            {
                const String categoryName = "Конструктивные данные";
                List<ItemProperty> propertiesList = new List<ItemProperty>();

                foreach (DataRow dataRow in propertiesTable.Rows)
                {
                    foreach (DataColumn dataColumn in propertiesTable.Columns)
                    {
                        propertiesList.Add(new ItemProperty()
                        {
                            Name = dataColumn.ColumnName,
                            DisplayName = GetDisplayName(dataColumn.ColumnName),
                            Description = GetDescription(dataColumn.ColumnName),
                            Category = categoryName,
                            DefaultValue = (dataRow[dataColumn] ?? String.Empty).ToString()
                        });
                    }
                }
                return propertiesList.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Перевести имя свойства из полученного в DataTable в отображаемый
        /// </summary>
        /// <param name="propertyName">Имя свойства из DataTable</param>
        /// <returns></returns>
        private string GetDisplayName(string propertyName)
        {
            switch (propertyName)
            {
                case "Code": return "Код";
                case "Description": return "Описание ";
                case "DiameterInside": return "Внутренний диаметра";
                case "DiameterOutMax": return "Максимальный диаметр в сечении";
                case "DiameterOutMin": return "Минимальный диаметр в сечении";
                case "DiameterOutside": return "Наружный диаметр";
                case "DrumCode": return "Код барабана";
                case "NozzleCodes": return "Коды водоопускных стояков (через \"; \")";
                case "NozzleCount": return "Количество водоопускных стояков";
                case "PID": return "Путь к PI диаграмме";
                case "PlanResource": return "Плановый ресурс";
                case "Radius": return "Радиус по нейтральной оси";
                case "SensorPressFirst": return "Код датчика давления";
                case "SensorPressSecond": return "Код датчика давления";
                case "SensorTemp": return "Код датчика температуры среды";
                case "SensorTempM": return "Код датчика температуры металла";
                case "SensorTempM_BtmCntr": return "Код датчика температуры металла внизу в центре";
                case "SensorTempM_BtmLft": return "Код датчика температуры металла внизу слева";
                case "SensorTempM_BtmRght": return "Код датчика температуры металла внизу справа";
                case "SensorTempM_TopCntr": return "Код датчика температуры металла вверху в центре";
                case "SensorTempM_TopLft": return "Код датчика температуры металла вверху слева";
                case "SensorTempM_TopRght": return "Код датчика температуры металла вверху справа";
                case "Steel": return "Марка стали";
                case "SteelWeldedPipe": return "марка стали привариваемой трубы";
                case "Thickness": return "Толщина стенки";
            }
            return propertyName;
        }

        /// <summary>
        /// Перевести имя свойства из полученного в DataTable в краткое описание
        /// </summary>
        /// <param name="propertyName">Имя свойства из DataTable</param>
        /// <returns></returns>
        private string GetDescription(string propertyName)
        {
            return String.Empty;
        }

        /// <summary>
        /// Получить все таблицы, которые могут быть полученны из расширения
        /// </summary>
        /// <returns></returns>
        public ExtensionDataInfo[] GetAllDataInfo()
        {
            return new ExtensionDataInfo[]{
                GetTabInfo(ModulusElasticityKeyword),
                GetTabInfo(LinearExpansionKeyword),
                GetTabInfo(ThermalConductKeyword),
                GetTabInfo(TemperatureConductKeyword),
                GetTabInfo(PermissibleStressKeyword),
                GetTabInfo(CreepTemperatureKeyword),
                GetTabInfo(ExponentKeyword),
                GetTabInfo(ReducingKeyword),
                GetTabInfo(ConstantKeyword),
                GetTabInfo(EquivalentStressKeyword),
                GetTabInfo(LifeCreepKeyword),
                //GetTabInfo(LifeCreepCurrentKeyword),
                GetTabInfo(LifeHFCKeyword),
                //GetTabInfo(LifeHFCCurrentKeyword),
                GetTabInfo(LifeLFCKeyword)//,
                //GetTabInfo(LifeLFCCurrentKeyword),
                //GetTabInfo(LifeStartStopKeyword),
                //GetTabInfo(LifeStartStopCurrentKeyword)
            };
        }

        /// <summary>
        /// Вкладочки не зависящие от ЮнитНода
        /// </summary>
        /// <returns></returns>
        public ExtensionDataInfo[] GetDataInfo()
        {
            return new ExtensionDataInfo[]{
                 GetTabInfo(ModulusElasticityKeyword),
                 GetTabInfo(LinearExpansionKeyword),
                 GetTabInfo(ThermalConductKeyword),
                 GetTabInfo(TemperatureConductKeyword),
                 GetTabInfo(PermissibleStressKeyword),
                 GetTabInfo(CreepTemperatureKeyword),
                 GetTabInfo(ExponentKeyword),
                 GetTabInfo(ReducingKeyword),
                 GetTabInfo(ConstantKeyword)
            };
        }

        /// <summary>
        /// Во вкладочки
        /// </summary>
        /// <param name="unitNode"></param>
        /// <returns></returns>
        public ExtensionDataInfo[] GetDataInfo(ExtensionUnitNode unitNode)
        {
            return new ExtensionDataInfo[]{
                GetTabInfo(EquivalentStressKeyword),
                GetTabInfo(LifeCreepKeyword),
                //GetTabInfo(LifeCreepCurrentKeyword),
                GetTabInfo(LifeHFCKeyword),
                //GetTabInfo(LifeHFCCurrentKeyword),
                GetTabInfo(LifeLFCKeyword)//,
                //GetTabInfo(LifeLFCCurrentKeyword),
                //GetTabInfo(LifeStartStopKeyword),
                //GetTabInfo(LifeStartStopCurrentKeyword)
            };
        }

        private const String EquivalentStressKeyword = "GetEquivalentStress";
        private ExtensionDataInfo equivalentStressInfo;
        private ExtensionDataInfo EquivalentStressInfo
        {
            get
            {// "Получение значений эквивалентных напряжений для объекта диагностирования"),
                if (equivalentStressInfo == null)
                    equivalentStressInfo = new ExtensionDataInfo(EquivalentStressKeyword, ExtensionDataType.Graph, "График напряжений");

                return equivalentStressInfo;
            }
        }

        private const String LifeCreepKeyword = "GetLifeCreep";
        private ExtensionDataInfo lifeCreepInfo;
        private ExtensionDataInfo LifeCreepInfo
        {
            get
            {
                // "Получение исчерпания ресурса в результате воздействия ползучести для объекта диагностирования"),
                if (lifeCreepInfo == null)
                    lifeCreepInfo = new ExtensionDataInfo(LifeCreepKeyword, ExtensionDataType.Graph, "График ползучести");

                return lifeCreepInfo;
            }
        }

        private const String LifeCreepCurrentKeyword = "GetLifeCreepCurrent";
        private ExtensionDataInfo lifeCreepCurrentInfo;
        private ExtensionDataInfo LifeCreepCurrentInfo
        {
            get
            {
                // "Получение суммарного (текущего) значения исчерпания ресурса в результате воздействия ползучести для объекта диагностирования"),
                if (lifeCreepCurrentInfo == null)
                    lifeCreepCurrentInfo = new ExtensionDataInfo(LifeCreepCurrentKeyword, ExtensionDataType.Histogram, "");

                return lifeCreepCurrentInfo;
            }
        }

        private const String LifeHFCKeyword = "GetLifeHFC";
        private ExtensionDataInfo lifeHFCInfo;
        private ExtensionDataInfo LifeHFCInfo
        {
            get
            {
                //"Получение исчерпания ресурса в результате воздействия высокочастотных циклов для объекта диагностирования \"Барабан\""),
                if (lifeHFCInfo == null)
                    lifeHFCInfo = new ExtensionDataInfo(LifeHFCKeyword, ExtensionDataType.Graph, "График ресурса (высокочастотный)");

                return lifeHFCInfo;
            }
        }

        private const String LifeHFCCurrentKeyword = "GetLifeHFCCurrent";
        private ExtensionDataInfo lifeHFCCurrentInfo;
        private ExtensionDataInfo LifeHFCCurrentInfo
        {
            get
            {// "Получение суммарного (текущего) значения исчерпания ресурса в результате воздействия высокочастотных циклов для объекта диагностирования \"Барабан\"" ),
                if (lifeHFCCurrentInfo == null)
                    lifeHFCCurrentInfo = new ExtensionDataInfo(LifeHFCCurrentKeyword, ExtensionDataType.Histogram, "");

                return lifeHFCCurrentInfo;
            }
        }

        private const String LifeLFCKeyword = "GetLifeLFC";
        private ExtensionDataInfo lifeLFCInfo;
        private ExtensionDataInfo LifeLFCInfo
        {
            get
            {            // "Получение исчерпания ресурса в результате воздействия малоцикловой усталости для объекта диагностирования"),
                if (lifeLFCInfo == null)
                    lifeLFCInfo = new ExtensionDataInfo(LifeLFCKeyword, ExtensionDataType.Graph, "График ресурса (микроциклы)");
                return lifeLFCInfo;
            }
        }

        private const String LifeLFCCurrentKeyword = "GetLifeLFCCurrent";
        private ExtensionDataInfo lifeLFCCurrentInfo;
        private ExtensionDataInfo LifeLFCCurrentInfo
        {
            get
            {// "Получение суммарного (текущего) значения исчерпания ресурса в результате воздействия малоцикловой усталости для объекта диагностирования"),
                if (lifeLFCCurrentInfo == null)
                    lifeLFCCurrentInfo = new ExtensionDataInfo(LifeLFCCurrentKeyword, ExtensionDataType.Histogram, "");
                return lifeLFCCurrentInfo;
            }
        }

        private const String LifeStartStopKeyword = "GetLifeStartStop";
        private ExtensionDataInfo lifeStartStopInfo;
        private ExtensionDataInfo LifeStartStopInfo
        {
            get
            {//"Получение исчерпания ресурса (по каждому виду ресурса) относительно циклов \"пуск-останов\" котла для объекта диагностирования "),
                if (lifeStartStopInfo == null)
                    lifeStartStopInfo = new ExtensionDataInfo(LifeStartStopKeyword, ExtensionDataType.Graph, "");
                return lifeStartStopInfo;
            }
        }

        private const String LifeStartStopCurrentKeyword = "GetLifeStartStopCurrent";
        private ExtensionDataInfo lifeStartStopCurrentInfo;
        private ExtensionDataInfo LifeStartStopCurrentInfo
        {
            get
            {//"Получение суммарного (текущего) исчерпания ресурса (по каждому виду ресурса) относительно циклов \"пуск-останов\" котла для объекта диагностирования ")
                if (lifeStartStopCurrentInfo == null)
                    lifeStartStopCurrentInfo = new ExtensionDataInfo(LifeStartStopCurrentKeyword, ExtensionDataType.Histogram, "");
                return lifeStartStopCurrentInfo;
            }
        }

        const String StartCountKeyword = "GetStartCount";
        private ExtensionDataInfo startCountInfo;
        private ExtensionDataInfo StartCountInfo
        {
            get
            {
                if (startCountInfo == null)
                    startCountInfo = new ExtensionDataInfo(StartCountKeyword, ExtensionDataType.Histogram, "Количество пусков");
                return startCountInfo;
            }
        }

        const String totalFactName = "totalFact";
        const String coldStateFactName = "coldStateFact";
        const String notCoolStateFactName = "notCoolStateFact";
        const String hotStateFactName = "hotStateFact";
        const String hydrotestingName = "hydrotesting";
        const String coldStatePermitName = "coldStatePermit";
        const String notCoolHotStatePermitName = "notCoolHotStatePermit";

        const String ModulusElasticityKeyword = "ModulusElasticity";
        private ExtensionDataInfo modulusElasticityInfo;
        private ExtensionDataInfo ModulusElasticityInfo
        {
            get
            {
                if (modulusElasticityInfo == null)
                {
                    modulusElasticityInfo = new ExtensionDataInfo(ModulusElasticityKeyword, ExtensionDataType.Table, "Модуль нормальной упругости");
                    modulusElasticityInfo.IsCommon = true;
                }
                return modulusElasticityInfo;
            }
        }

        const String LinearExpansionKeyword = "LinearExpansion";
        private ExtensionDataInfo linearExpansionInfo;
        private ExtensionDataInfo LinearExpansionInfo
        {
            get
            {
                if (linearExpansionInfo == null)
                {
                    linearExpansionInfo = new ExtensionDataInfo(LinearExpansionKeyword, ExtensionDataType.Table, "Линейное расширение");
                    linearExpansionInfo.IsCommon = true;
                }
                return linearExpansionInfo;
            }
        }

        const String ThermalConductKeyword = "ThermalConduct";
        private ExtensionDataInfo thermalConductInfo;
        private ExtensionDataInfo ThermalConductInfo
        {
            get
            {
                if (thermalConductInfo == null)
                {
                    thermalConductInfo = new ExtensionDataInfo(ThermalConductKeyword, ExtensionDataType.Table, "Теплопроводность");
                    thermalConductInfo.IsCommon = true;
                }
                return thermalConductInfo;
            }
        }

        const String TemperatureConductKeyword = "TemperatureConduct";
        private ExtensionDataInfo temperatureConductInfo;
        private ExtensionDataInfo TemperatureConductInfo
        {
            get
            {
                if (temperatureConductInfo == null)
                {
                    temperatureConductInfo = new ExtensionDataInfo(TemperatureConductKeyword, ExtensionDataType.Table, "Температуропроводность");
                    temperatureConductInfo.IsCommon = true; 
                }
                return temperatureConductInfo;
            }
        }

        const String PermissibleStressKeyword = "PermissibleStress";
        private ExtensionDataInfo permissibleStressInfo;
        private ExtensionDataInfo PermissibleStressInfo
        {
            get
            {
                if (permissibleStressInfo == null)
                {
                    permissibleStressInfo = new ExtensionDataInfo(PermissibleStressKeyword, ExtensionDataType.Table, "Допускаемые напряжения");
                    permissibleStressInfo.IsCommon = true;
                }
                return permissibleStressInfo;
            }
        }

        const String CreepTemperatureKeyword = "CreepTemperature";
        private ExtensionDataInfo creepTemperatureInfo;
        private ExtensionDataInfo CreepTemperatureInfo
        {
            get
            {
                if (creepTemperatureInfo == null)
                {
                    creepTemperatureInfo = new ExtensionDataInfo(CreepTemperatureKeyword, ExtensionDataType.Table, "Температура ползучести для каждой марки стали");
                    creepTemperatureInfo.IsCommon = true;
                }
                return creepTemperatureInfo;
            }
        }

        const String ExponentKeyword = "Exponent";
        private ExtensionDataInfo exponentInfo;
        private ExtensionDataInfo ExponentInfo
        {
            get
            {
                if (exponentInfo == null)
                {
                    exponentInfo = new ExtensionDataInfo(ExponentKeyword, ExtensionDataType.Table, "Показатель степени");
                    exponentInfo.IsCommon = true;
                }
                return exponentInfo;
            }
        }

        const String ReducingKeyword = "Reducing";
        private ExtensionDataInfo reducingInfo;
        private ExtensionDataInfo ReducingInfo
        {
            get
            {
                if (reducingInfo == null)
                {
                    reducingInfo = new ExtensionDataInfo(ReducingKeyword, ExtensionDataType.Table, "Снижение влияния овальности");
                    reducingInfo.IsCommon = true;
                }
                return reducingInfo;
            }
        }

        const String ConstantKeyword = "Constant";
        private ExtensionDataInfo constantInfo;
        private ExtensionDataInfo ConstantInfo
        {
            get
            {
                if (constantInfo == null)
                {
                    constantInfo = new ExtensionDataInfo(ConstantKeyword, ExtensionDataType.Table, "Постоянные расчетные коэффициенты");
                    constantInfo.IsCommon = true;
                }
                return constantInfo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabKeyword"></param>
        /// <returns></returns>
        private ExtensionDataInfo GetTabInfo(string tabKeyword)
        {
            switch (tabKeyword)
            {
                case EquivalentStressKeyword:
                    return EquivalentStressInfo;
                case LifeCreepKeyword:
                    return LifeCreepInfo;
                case LifeCreepCurrentKeyword:
                    return LifeCreepCurrentInfo;
                case LifeHFCKeyword:
                    return LifeHFCInfo;
                case LifeHFCCurrentKeyword:
                    return LifeHFCCurrentInfo;
                case LifeLFCKeyword:
                    return LifeLFCInfo;
                case LifeLFCCurrentKeyword:
                    return LifeLFCCurrentInfo;
                case LifeStartStopKeyword:
                    return LifeStartStopInfo;
                case LifeStartStopCurrentKeyword:
                    return LifeStartStopCurrentInfo;
                case StartCountKeyword:
                    return StartCountInfo;
                case ModulusElasticityKeyword:
                    return ModulusElasticityInfo;
                case LinearExpansionKeyword:
                    return LinearExpansionInfo;
                case ThermalConductKeyword:
                    return ThermalConductInfo;
                case TemperatureConductKeyword:
                    return TemperatureConductInfo;
                case PermissibleStressKeyword:
                    return PermissibleStressInfo;
                case CreepTemperatureKeyword:
                    return CreepTemperatureInfo;
                case ExponentKeyword:
                    return ExponentInfo;
                case ReducingKeyword:
                    return ReducingInfo;
                case ConstantKeyword:
                    return ConstantInfo;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitNode"></param>
        /// <param name="tabKeyword"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ExtensionData GetData(ExtensionUnitNode unitNode, ExtensionDataInfo extensionDataInfo, DateTime beginTime, DateTime endTime)
        {
            ExtensionUnitNode parent = Supplier.GetParent(unitNode, BoilerTypeGUID);
            ExtensionDataInfo info = GetTabInfo(extensionDataInfo.Name);

            switch (extensionDataInfo.Name)
            {
                case EquivalentStressKeyword:
                    if (parent != null)
                        return GetEquivalentStress(info, unitNode, parent, beginTime, endTime);
                    break;
                case LifeCreepKeyword:
                    if (parent != null)
                        return GetLifeCreep(info, unitNode, parent, beginTime, endTime);
                    break;
                case LifeCreepCurrentKeyword:
                    return null;
                case LifeHFCKeyword:
                    if (parent != null)
                        return GetLifeHFC(info, unitNode, parent, beginTime, endTime);
                    break;
                case LifeHFCCurrentKeyword:
                    return null;
                case LifeLFCKeyword:
                    if (parent != null)
                        return GetLifeLFC(info, unitNode, parent, beginTime, endTime);
                    break;
                case LifeLFCCurrentKeyword:
                case LifeStartStopKeyword:
                case LifeStartStopCurrentKeyword:
                    return null;
                case StartCountKeyword:
                    return GetStartCount(info, parent);
                case ModulusElasticityKeyword:
                    return TabledMegaTab(info, monitoringData.GetModulusElasticity());
                case LinearExpansionKeyword:
                    return TabledMegaTab(info, monitoringData.GetLinearExpansion());
                case ThermalConductKeyword:
                    return TabledMegaTab(info, monitoringData.GetThermalConduct());
                case TemperatureConductKeyword:
                    return TabledMegaTab(info, monitoringData.GetTemperatureConduct());
                case PermissibleStressKeyword:
                    return TabledMegaTab(info, monitoringData.GetPermissibleStress());
                case CreepTemperatureKeyword:
                    return TabledMegaTab(info, monitoringData.GetCreepTemperature());
                case ExponentKeyword:
                    return TabledMegaTab(info, monitoringData.GetExponent());
                case ReducingKeyword:
                    return TabledMegaTab(info, monitoringData.GetReducing());
                case ConstantKeyword:
                    return TabledMegaTab(info, monitoringData.GetConstant());
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private ExtensionData GetStartCount(ExtensionDataInfo info, ExtensionUnitNode parent)
        {
            int totalFact;
            int coldStateFact;
            int notCoolStateFact;
            int hotStateFact;
            int hydrotesting;
            int coldStatePermit;
            int notCoolHotStatePermit;

            monitoringData.GetStartCount(parent.ExternalID,
                                         out totalFact,
                                         out coldStateFact,
                                         out notCoolStateFact,
                                         out hotStateFact,
                                         out hydrotesting,
                                         out coldStatePermit,
                                         out notCoolHotStatePermit);

            ExtensionDataTrend columnInfo=new ExtensionDataTrend("StartStop", String.Empty, new ExtensionDataColumn[]{
                new ExtensionDataColumn(totalFactName,                typeof(int)),
                new ExtensionDataColumn(coldStateFactName,            typeof(int)),
                new ExtensionDataColumn(notCoolStateFactName,         typeof(int)),
                new ExtensionDataColumn(hotStateFactName,             typeof(int)),
                new ExtensionDataColumn(hydrotestingName,             typeof(int)),
                new ExtensionDataColumn(coldStatePermitName,          typeof(int)),
                new ExtensionDataColumn(notCoolHotStatePermitName,    typeof(int))
            });

            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            tabInfo.Trends.Add(columnInfo);

            ExtensionData tab = new ExtensionData(tabInfo);
            DataRow row = tab.Table.Rows.Add();
            row[totalFactName] = totalFact;
            row[coldStateFactName] = coldStateFact;
            row[notCoolStateFactName] = notCoolStateFact;
            row[hotStateFactName] = hotStateFact;
            row[hydrotestingName] = hydrotesting;
            row[coldStatePermitName] = coldStatePermit;
            row[notCoolHotStatePermitName] = notCoolHotStatePermit;

            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="unitNode"></param>
        /// <param name="parent"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private ExtensionData GetLifeLFC(ExtensionDataInfo info, ExtensionUnitNode unitNode, ExtensionUnitNode parent, DateTime beginTime, DateTime endTime)
        {
            const String CodeString = "Code";
            const String TimeLFCString = "TimeLFC";
            const String LifeLFCString = "Value";

            DataTable table = monitoringData.GetLifeLFC(parent.ExternalID, unitNode.ExternalCode, beginTime, endTime);

            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            ExtensionDataColumn[] columns = new ExtensionDataColumn[]{
                new ExtensionDataColumn(TimeLFCString, typeof(DateTime)),
                new ExtensionDataColumn(LifeLFCString,typeof(double))
            };


            String code;
            List<String> codes = new List<String>();
            String filterFormat = String.Format("{0} = '{{0}}'", CodeString);

            // для каждого кода из полученной таблицы, добавляем по две колонки
            foreach (DataRow item in table.Rows)
            {
                code = item[CodeString].ToString();
                if (!codes.Contains(code))
                {
                    codes.Add(code);
                    tabInfo.Trends.Add(new ExtensionDataTrend(code, String.Format(filterFormat, code), columns));
                }
            }

            ExtensionData tab = new ExtensionData(tabInfo, table);
            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="unitNode"></param>
        /// <param name="parent"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private ExtensionData GetLifeHFC(ExtensionDataInfo info, ExtensionUnitNode unitNode, ExtensionUnitNode parent, DateTime beginTime, DateTime endTime)
        {
            const String CodeString = "Code";
            const String TimeHFCString = "TimeHFC";
            const String LifeHFCString = "Value";

            DataTable table = monitoringData.GetLifeHFC(parent.ExternalID, unitNode.ExternalCode, beginTime, endTime);

            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            ExtensionDataColumn[] columns = new ExtensionDataColumn[]{
                new ExtensionDataColumn(TimeHFCString, typeof(DateTime)),
                new ExtensionDataColumn(LifeHFCString,typeof(double))
            };

            String code;
            List<String> codes = new List<String>();
            String filterFormat = String.Format("{0} = '{{0}}'", CodeString);

            // для каждого кода из полученной таблицы, добавляем по две колонки
            foreach (DataRow item in table.Rows)
            {
                code = item[CodeString].ToString();
                if (!codes.Contains(code))
                {
                    codes.Add(code);
                    tabInfo.Trends.Add(new ExtensionDataTrend(code, String.Format(filterFormat, code), columns));
                }
            }

            ExtensionData tab = new ExtensionData(tabInfo, table);

            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="unitNode"></param>
        /// <param name="parent"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private ExtensionData GetLifeCreep(ExtensionDataInfo info, ExtensionUnitNode unitNode, ExtensionUnitNode parent, DateTime beginTime, DateTime endTime)
        {
            const String TimeCreepString = "TimeCreep";
            const String LifeCreepString = "LifeCreep";
            const String TemperatureString = "Temperature";

            ExtensionDataColumn timeDescription = new ExtensionDataColumn(TimeCreepString, typeof(DateTime));
            ExtensionDataColumn lifeCreepStringDescription = new ExtensionDataColumn(LifeCreepString, typeof(double));
            ExtensionDataColumn temperatureStringDescription = new ExtensionDataColumn(TemperatureString, typeof(double));

            // заполняем информацию о линиях графика
            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            tabInfo.Trends.Add(new ExtensionDataTrend(LifeCreepString, String.Empty, new ExtensionDataColumn[] { timeDescription, lifeCreepStringDescription }));
            tabInfo.Trends.Add(new ExtensionDataTrend(TemperatureString, String.Empty, new ExtensionDataColumn[] { temperatureStringDescription, lifeCreepStringDescription }));

            DataTable table = monitoringData.GetLifeCreep(parent.ExternalID, unitNode.ExternalCode, beginTime, endTime);

            ExtensionData tab = new ExtensionData(tabInfo, table);

            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="unitNode"></param>
        /// <param name="parent"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private ExtensionData GetEquivalentStress(ExtensionDataInfo info, ExtensionUnitNode unitNode, ExtensionUnitNode parent, DateTime beginTime, DateTime endTime)
        {
            const String CodeString = "Code";
            const String TimeStressString = "TimeStress";
            const String HoopEquivalentStressString = "HoopEquivalentStress";
            const String AxialEquivalentStressString = "AxialEquivalentStress";

            DataTable table = monitoringData.GetEquivalentStress(parent.ExternalID, unitNode.ExternalCode, beginTime, endTime);

            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            ExtensionDataColumn codeDescription = new ExtensionDataColumn(CodeString, typeof(String));
            ExtensionDataColumn timeDescription = new ExtensionDataColumn(TimeStressString, typeof(DateTime));
            ExtensionDataColumn hoopEquivalentStressDescription = new ExtensionDataColumn(HoopEquivalentStressString, typeof(double));
            ExtensionDataColumn axialEquivalentStressDescription = new ExtensionDataColumn(AxialEquivalentStressString, typeof(double));

                        String code;
            List<String> codes = new List<String>();
            String filterFormat = String.Format("{0} = '{{0}}'", CodeString);

            // для каждого кода из полученной таблицы, добавляем по две колонки
            foreach (DataRow item in table.Rows)
            {
                code = item[CodeString].ToString();
                if (!codes.Contains(code))
                {
                    codes.Add(code);
                    tabInfo.Trends.Add(new ExtensionDataTrend(code, String.Format(filterFormat, code), new ExtensionDataColumn[] { timeDescription, hoopEquivalentStressDescription }));
                    tabInfo.Trends.Add(new ExtensionDataTrend(code, String.Format(filterFormat, code), new ExtensionDataColumn[] { timeDescription, axialEquivalentStressDescription }));
                }
            }
            ExtensionData tab = new ExtensionData(tabInfo, table);

            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        private ExtensionData TabledMegaTab(ExtensionDataInfo info, DataTable sourceTable)
        {
            ExtensionDataInfo tabInfo = new ExtensionDataInfo(info);

            List<ExtensionDataColumn> columnList = new List<ExtensionDataColumn>();
            ExtensionDataColumn column;

            foreach (DataColumn dataColumn in sourceTable.Columns)
            {
                column = new ExtensionDataColumn(dataColumn.ColumnName, dataColumn.DataType);
                columnList.Add(column);
                switch (column.Name)
                {
                    case "CalculatedLife":
                        column.Caption = "Расчетный ресурс";
                        break;
                    case "Description":
                        column.Caption = "Описание константы";
                        break;
                    case "Exponent":
                        column.Caption = "значение коэффициента";
                        break;
                    case "LinearExpansion":
                        column.Caption = "Значение коэффициента";
                        break;
                    case "ModulusElasticity":
                        column.Caption = "Значение коэффициента";
                        break;
                    case "Name":
                        column.Caption = "Наименование константы";
                        break;
                    case "Reducing":
                        column.Caption = "Значение коэффициента";
                        break;
                    case "Steel":
                        column.Caption = "Марка стали";
                        break;
                    case "Stress":
                        column.Caption = "Значение допускаемого напряжения";
                        break;
                    case "Symbol":
                        column.Caption = "Символьное обозначение константы";
                        break;
                    case "Temperature":
                        column.Caption = "Значение температуры";
                        break;
                    case "ThermalConduct":
                        column.Caption = "Значение коэффициента";
                        break;
                    case "Value":
                        column.Caption = "Значение константы";
                        break;
                }
                ExtensionDataTrend line = new ExtensionDataTrend(info.Name, String.Empty, columnList.ToArray());

                tabInfo.Trends.Add(line);
            }

            ExtensionData tab = new ExtensionData(tabInfo, sourceTable);

            return tab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable GetMessages(MessageCategory filter, DateTime startTime, DateTime endTime)
        {
            String[] messageTypes = new String[] { "Предупреждение", "Ошибка" };

            DataTable messageTable = new DataTable();

            foreach (var item in messageTypes)
            {
                DataTable table = monitoringData.GetMessageLog(item, startTime, endTime);
                if (table != null)
                {
                    messageTable.Merge(table);
                }
            }
            return messageTable;
        }
    }
}
