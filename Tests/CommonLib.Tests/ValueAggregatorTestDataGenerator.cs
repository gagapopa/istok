using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Tests
{
    class ValueAggregatorTestDataGenerator
    {
        const int valuesCount = 2;

        const int topSeed = 42;
        
        const bool useZip = true;

        Tuple<Interval, DateTime, DateTime, DateTime, String>[] array = new Tuple<Interval, DateTime, DateTime, DateTime, String>[]
        {
            Tuple.Create(Interval.Zero, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), "FromRaw"),
            Tuple.Create(Interval.Second, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), "FromSecond"),
            Tuple.Create(Interval.Minute, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), "FromMinute"),
            Tuple.Create(Interval.Hour, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 07, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2013, 01, 01, 00, 00, 00, DateTimeKind.Utc), "FromHour"),
            Tuple.Create(Interval.Day, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 07, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2013, 01, 01, 00, 00, 00, DateTimeKind.Utc), "FromDay"),
        };

        Tuple<Interval, String>[] destArray = new Tuple<Interval, String>[]
        {
            Tuple.Create(Interval.Minute, "Minute"),
            Tuple.Create(Interval.Hour, "Hour"),
            Tuple.Create(Interval.Day, "Day"),
            Tuple.Create(Interval.Month, "Month"),
        };

        CalcAggregation[] aggregationArray = new CalcAggregation[]
        {
            CalcAggregation.First,
            CalcAggregation.Last,
            CalcAggregation.Maximum,
            CalcAggregation.Minimum,
            CalcAggregation.Sum,
            CalcAggregation.Average,
            CalcAggregation.Count,
            CalcAggregation.Exist,
            CalcAggregation.Weighted
        };

        Tuple<String, float, float, float, float>[] valuesType = new Tuple<String, float, float, float, float>[]
        {
            Tuple.Create("", 0f,0f, 0f,0f),
            Tuple.Create("WithSkips", 0.01f, 0f, 0.3f, 0f),
            Tuple.Create("WithBads", 0f, 0.01f, 0f, 0.3f)
        };

        public void GenerateSourceData(String filesDirectory)
        {
            Random seedGenerator = new Random(topSeed);

            DirectoryInfo dirInfo = new DirectoryInfo(filesDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            foreach (var tuple in array)
            {
                Interval interval = tuple.Item1;
                DateTime startTime = tuple.Item2;
                DateTime midleTime = tuple.Item3;
                DateTime endTime = tuple.Item4;
                String groupName = tuple.Item5;

                foreach (var valueTuple in valuesType)
                {
                    String valueName = valueTuple.Item1;
                    float skipProb1 = valueTuple.Item2;
                    float badProb1 = valueTuple.Item3;
                    float skipProb2 = valueTuple.Item4;
                    float badProb2 = valueTuple.Item5;

                    Dictionary<DateTime, ParamValueItem[]> sourceDictionary = new Dictionary<DateTime, ParamValueItem[]>();
                    ParamValueItem[] paramArray;

                    // generate values for all parameters
                    for (int i = 0; i < valuesCount; i++)
                    {
                        var values = GenerateSourceValues(seedGenerator.Next(), interval, startTime, midleTime, skipProb1, badProb1)
                            .Concat(GenerateSourceValues(seedGenerator.Next(), interval, midleTime, endTime, skipProb2, badProb2));

                        foreach (var item in values)
                        {
                            if (!sourceDictionary.TryGetValue(item.Time, out paramArray))
                            {
                                sourceDictionary[item.Time] = paramArray = new ParamValueItem[valuesCount];
                            }

                            paramArray[i] = item;
                        }
                    }

                    if (interval != Interval.Zero)
                    {
                        DateTime time = interval.NearestEarlierTime(startTime);

                        while (time < endTime)
                        {
                            if (!sourceDictionary.ContainsKey(time))
                            {
                                sourceDictionary[time] = new ParamValueItem[valuesCount];
                            }

                            time = interval.GetNextTime(time);
                        }
                    }

                    String fileName = Path.Combine(filesDirectory, String.Format("ValueAggregatorTests_{0}{1}.txt", groupName, valueName));
                    SaveFile(sourceDictionary, fileName);

                    foreach (var destTuple in destArray)
                    {
                        Interval destInterval = destTuple.Item1;
                        String destName = destTuple.Item2;

                        if (destInterval > interval)
                        {
                            Dictionary<DateTime, ParamValueItem[]> destDictionary = new Dictionary<DateTime, ParamValueItem[]>();
                            int count = aggregationArray.Length;

                            DateTime time = destInterval.NearestEarlierTime(startTime);

                            while (time < endTime)
                            {
                                destDictionary[time] = new ParamValueItem[count];
                                for (int i = 0; i < count; i++)
                                {
                                    destDictionary[time][i] = new ParamValueItem(time, Quality.Good, double.NaN);
                                }
                                time = destInterval.GetNextTime(time);
                            }

                            fileName = Path.Combine(filesDirectory, String.Format("ValueAggregatorTests_{0}{1}_{2}.txt", groupName, valueName, destName));
                            SaveFile(destDictionary, fileName);
                        }
                    }
                }
            }
        }

        private static void SaveFile(Dictionary<DateTime, ParamValueItem[]> sourceDictionary, String fileName)
        {
            if (useZip)
            {
                using (FileStream stream = new FileStream(fileName + ".gz", FileMode.Create))
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                    {
                        using (StreamWriter writer = new StreamWriter(zip))
                        {
                            SaveFile(sourceDictionary, writer);
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    SaveFile(sourceDictionary, writer);
                }
            }
        }

        private static void SaveFile(Dictionary<DateTime, ParamValueItem[]> sourceDictionary, StreamWriter writer)
        {
            StringBuilder builderString = new StringBuilder();

            var keys = from k in sourceDictionary.Keys orderby k select k;

            foreach (var time in keys)
            {
                builderString.Clear();
                builderString.AppendFormat("{0:o}", time);

                for (int i = 0; i < sourceDictionary[time].Length; i++)
                {
                    var paramItem = sourceDictionary[time][i];
                    builderString.AppendFormat("\t{0}",
                        paramItem == null ?
                            "" :
                        paramItem.Quality == Quality.Bad ?
                            double.NaN.ToString() :
                            paramItem.Value.ToString(NumberFormatInfo.InvariantInfo));
                }

                writer.WriteLine(builderString);
            }
        }

        private IEnumerable<ParamValueItem> GenerateSourceValues(int seed, Interval interval, DateTime startTime, DateTime endTime, float skipProb, float badProb)
        {
            const int rawStep = 80;

            Random rnd = new Random(seed);

            DateTime time = startTime;

            while (time <= endTime)
            {
                int dice = rnd.Next() % 100;
                double value = rnd.NextDouble();
                bool isSkip = dice < skipProb * 100;
                bool isBad = !isSkip && dice < (skipProb + badProb) * 100;

                if (isBad)
                {
                    yield return new ParamValueItem(time, Quality.Bad, double.NaN);
                }
                else if (!isSkip)
                {
                    yield return new ParamValueItem(time, Quality.Good, value);
                }

                if (interval == Interval.Zero)
                {
                    time = time.AddMilliseconds(25 * (rnd.Next() % rawStep));
                }
                else
                {
                    time = interval.GetNextTime(time);
                }
            }
        }

        public IEnumerable<ParamValueItem> GetSourceData(int paramNum, Interval sourceInterval, String valueName)
        {
            String groupName = (from t in array where t.Item1 == sourceInterval select t.Item5).First();

            String resourceName = String.Format("ValueAggregatorTests_{0}{1}_txt", groupName, valueName);

            var valuesDictionary = LoadValues(resourceName);

            return (from k in valuesDictionary.Keys
                    where valuesDictionary[k].Length > paramNum && valuesDictionary[k][paramNum] != null
                    select valuesDictionary[k][paramNum]).ToArray();
        }

        public IEnumerable<ParamValueItem> GetAggregateData(Interval sourceInterval, string valueName, Interval destInterval, CalcAggregation aggregation)
        {
            String groupName = (from t in array where t.Item1 == sourceInterval select t.Item5).First();

            String dest = (from t in destArray where t.Item1 == destInterval select t.Item2).First();

            int aggregationIndex = aggregationArray.ToList().IndexOf(aggregation);

            String resourceName = String.Format("ValueAggregatorTests_{0}{1}_{2}_txt", groupName, valueName, dest);

            var valuesDictionary = LoadValues(resourceName);

            return (from k in valuesDictionary.Keys
                    where valuesDictionary[k].Length > aggregationIndex && valuesDictionary[k][aggregationIndex] != null
                    select valuesDictionary[k][aggregationIndex]).ToArray();
        }

        private Dictionary<DateTime, ParamValueItem[]> LoadValues(String resourceName)
        {
            Dictionary<DateTime, ParamValueItem[]> valuesDictionary;
            byte[] bytes = (byte[])Properties.Resources.ResourceManager.GetObject(resourceName);

            using (var stream = new MemoryStream(bytes))
            {
                if (useZip)
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(zip))
                        {
                            valuesDictionary = LoadValues(reader);
                        }
                    }
                }
                else
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        valuesDictionary = LoadValues(reader);
                    }
                }
            }

            return valuesDictionary;
        }

        private Dictionary<DateTime, ParamValueItem[]> LoadValues(StreamReader reader)
        {
            Dictionary<DateTime, ParamValueItem[]> valuesDictionary = new Dictionary<DateTime, ParamValueItem[]>();

            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    var columns = line.Split('\t');
                    DateTime time = Convert.ToDateTime(columns[0]);//.ToUniversalTime();

                    ParamValueItem[] valueItems = new ParamValueItem[columns.Length - 1];

                    for (int i = 1; i < columns.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(columns[i]))
                        {
                            double value = Convert.ToDouble(columns[i], NumberFormatInfo.InvariantInfo);

                            if (double.IsNaN(value))
                            {
                                valueItems[i - 1] = new ParamValueItem(time, Quality.Bad, value);
                            }
                            else
                            {
                                valueItems[i - 1] = new ParamValueItem(time, Quality.Good, value);
                            }
                        }
                    }
                    valuesDictionary[time] = valueItems;
                }
            }
            return valuesDictionary;
        }
    }
}
