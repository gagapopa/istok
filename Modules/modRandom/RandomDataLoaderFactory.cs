using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modRandom
{
    class RandomDataLoaderFactory : IDataLoaderFactory
    {
        const String category = "Генератор случайных чисел";

        readonly ModuleInfo info;

        public static readonly ItemProperty MinValueProperty;
        public static readonly ItemProperty MaxValueProperty;
        public static readonly ItemProperty GenPeriodProperty;
        public static readonly ItemProperty GenDelayProperty;
        public static readonly ItemProperty DieProbProperty;
        public static readonly ItemProperty DieIntervalProperty;

        static RandomDataLoaderFactory()
        {            
            MinValueProperty = new ItemProperty()
            {
                Name = "MinValue",
                DisplayName = "Минимум",
                Description = "",
                Category = category,
                ValueType = typeof(int),
                DefaultValue = 0.ToString()
            };
            MaxValueProperty = new ItemProperty()
            {
                Name = "MaxValue",
                DisplayName = "Максимум",
                Description = "",
                Category = category,
                ValueType = typeof(int),
                DefaultValue = 100.ToString()
            };
            GenPeriodProperty = new ItemProperty()
            {
                Name = "GenPeriod",
                DisplayName = "Период",
                Description = "Задержка между значениями в мс",
                Category = category,
                ValueType = typeof(int),
                DefaultValue = 1000.ToString()
            };
            GenDelayProperty = new ItemProperty()
            {
                Name = "GenDelay",
                DisplayName = "Задержка",
                Description = "В мс",
                Category = category,
                ValueType = typeof(int),
                DefaultValue = 1000.ToString()
            };
            DieProbProperty = new ItemProperty()
            {
                Name = "DieProb",
                DisplayName = "Вероятность падения модуля",
                Description = "0..1",
                Category = category,
                ValueType = typeof(float),
                DefaultValue = 0.0.ToString()
            };
            DieIntervalProperty = new ItemProperty()
            {
                Name = "DieInterval",
                DisplayName = "Задержка перед падением",
                Description = "Задержка перед вожможным падением, мс",
                Category = category,
                ValueType = typeof(int),
                DefaultValue = 600000.ToString()
            };
        }

        public RandomDataLoaderFactory()
        {
            // подменяем категорию для общих свойств
            CommonProperty.ChannelLoadMethodProperty.Category = category;

            info = new ModuleInfo(
                "modRandom",
                category,
                new ItemProperty[] 
                {
                    CommonProperty.ChannelLoadMethodProperty,
                    MinValueProperty,
                    MaxValueProperty,
                    GenPeriodProperty,
                    GenDelayProperty,
                    DieProbProperty,
                    DieIntervalProperty,
                },
                new ItemProperty[] 
                { 
                    CommonProperty.ParameterCodeProperty
                });
        }

        #region IDataLoaderFactory Members

        public ModuleInfo Info
        {
            get { return info; }
        }

        public IEnumerable<DataLoadMethod> GetSupportedLoadMethods()
        {
            yield return DataLoadMethod.Current;
            yield return DataLoadMethod.Subscribe;
            yield return DataLoadMethod.Archive;
            yield return DataLoadMethod.ArchiveByParameter;
        }

        public IDataLoader CreateLoader(ChannelInfo channelInfo)
        {
            return new RandomDataLoader();
        }

        #endregion
    }
}
