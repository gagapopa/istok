using System;
using System.Collections.Generic;
using System.IO;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.Version1_5
{
    /// <summary>
    /// Сохранить/получить значения
    /// </summary>
    class ValueReceiver : BaseValueReceiver
    {
        public ValueReceiver()
        {
        }

        #region Work with arguments
        public class ArgumentedParamValueItem
        {
            public ValueArguments Arguments { get; set; }

            public ParamValueItem ValueItem { get; set; }

            public ArgumentedParamValueItem(ValueArguments args, ParamValueItem value)
            {
                this.Arguments = args;
                this.ValueItem = value;
            }
        }

        enum ArgumentedValuesBynaryKeyword : byte
        {
            Argument,
            Value,
            Correct,
            LevelDown,
            LevelUp
        }

        public byte[] ArgumentedValuesToBinary(List<ArgumentedParamValueItem> values)
        {
            ValueArguments baseArgument = null;

            // чем отсортированней(по базовым аргументам) аргументы, тем меньше результирующая пачка
            values.Sort((a, b) => a.Arguments.CompareTo(b.Arguments));

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(ms))
                {
                    foreach (var item in values)
                    {
                        //ищменить текущий аргумент
                        ValueArguments args = item.Arguments;
                        while (!args.Include(baseArgument))
                        {
                            w.Write((byte)ArgumentedValuesBynaryKeyword.LevelUp);
                            baseArgument = baseArgument.GetBaseArgument();
                        }
                        for (int i = baseArgument == null ? 0 : baseArgument.Length; i < args.Length; i++)
                        {
                            w.Write((byte)ArgumentedValuesBynaryKeyword.Argument);
                            w.Write(args[i].OptimizationNodeID);
                            w.Write((byte)args[i].Length);
                            for (int j = 0; j < args[i].Length; j++)
                            {
                                w.Write(args[i][j].Key);
                                w.Write((double)args[i][j].Value);
                            }
                            w.Write((byte)ArgumentedValuesBynaryKeyword.LevelDown);
                        }
                        baseArgument = args;
                        // записать значение
                        w.Write((byte)ArgumentedValuesBynaryKeyword.Value);
                        w.Write(item.ValueItem.Value);
                    }
                }
                return ms.ToArray();
            }
        }

        public List<ArgumentedParamValueItem> ArgumentedValuesFromBinary(DateTime time, byte[] pack)
        {
            List<ArgumentedParamValueItem> retList = new List<ArgumentedParamValueItem>();

            using (MemoryStream ms = new MemoryStream(pack))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    ValueArguments args = null;
                    ValueArguments baseArgs = null;

                    while (ms.Length > ms.Position)
                    {
                        ArgumentedValuesBynaryKeyword keyword = (ArgumentedValuesBynaryKeyword)r.ReadByte();
                        switch (keyword)
                        {
                            case ArgumentedValuesBynaryKeyword.Argument:
                                int optimizationID = r.ReadInt32();
                                int count = r.ReadByte();
                                String name;
                                double value;
                                args = new ValueArguments(baseArgs);
                                List<KeyValuePair<String, Object>> argumentValueList = new List<KeyValuePair<String, Object>>();
                                for (int i = 0; i < count; i++)
                                {
                                    name = r.ReadString();
                                    value = r.ReadDouble();
                                    argumentValueList.Add(new KeyValuePair<String, Object>(name, value));
                                }
                                args.Add(optimizationID, argumentValueList.ToArray());
                                break;
                            case ArgumentedValuesBynaryKeyword.LevelDown:
                                if (args != null)
                                    baseArgs = new ValueArguments(args);
                                break;
                            case ArgumentedValuesBynaryKeyword.LevelUp:
                                if (baseArgs != null)
                                    baseArgs = baseArgs.GetBaseArgument();
                                break;
                            case ArgumentedValuesBynaryKeyword.Value:
                                double val = r.ReadDouble();
                                retList.Add(new ArgumentedParamValueItem(args, new ParamValueItem(time, Quality.Good, val)));
                                break;
                            case ArgumentedValuesBynaryKeyword.Correct:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return retList;
        }
        #endregion
    }
}
