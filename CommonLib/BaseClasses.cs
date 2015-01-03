using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;

namespace COTES.ISTOK
{
    ///// <summary>
    ///// базовый класс для элементов, использующих список свойств
    ///// </summary>
    //[Serializable]
    //public class BaseWithPropertyItem
    //{
    //    public Dictionary<String, String> propertylist = new Dictionary<String, String>();

    //    /// <summary>
    //    /// Проверить существует ли свойство с таким именем
    //    /// </summary>
    //    /// <param name="Property">Имя свойства</param>
    //    /// <returns>true, если свойство с таким именем существует и false в противном случае</returns>
    //    public bool ExistsProperty(string Property)
    //    {
    //        return propertylist.ContainsKey(Property);
    //    }

    //    /// <summary>
    //    /// Получить значение свойства
    //    /// </summary>
    //    /// <param name="Property">Имя свойства</param>
    //    /// <returns>Значение свойства</returns>
    //    public String FindProperty(String Property)
    //    {
    //        if (ExistsProperty(Property))
    //        {
    //            return propertylist[Property];
    //        }
    //        else return null;
    //    }

    //    /// <summary>
    //    /// Удалить все имеющиеся свойства
    //    /// </summary>
    //    public void ClearProperty()
    //    {
    //        propertylist.Clear();
    //    }
    //}

    ///// <summary>
    ///// передаваемый параметр
    ///// </summary>
    //[Serializable]
    //public class ParamSendItem : BaseWithPropertyItem, IComparable<ParamSendItem>
    //{
    //    public int channel = 0; // код канала
    //    public int param = 0; // код параметра
    //    public string name = ""; // наименование параметра
    //    public DateTime lasttime = DateTime.Now; // время получения последнего значения

    //    /// <summary>
    //    /// Создать новый экземпляр параметра
    //    /// </summary>
    //    public ParamSendItem() { }

    //    /// <summary>
    //    /// Создать новый экземпляр параметра
    //    /// </summary>
    //    /// <param name="Channel">ИД канала</param>
    //    /// <param name="Param">ИД параметра</param>
    //    public ParamSendItem(int Channel, int Param)
    //    {
    //        channel = Channel;
    //        param = Param;
    //        name = "";
    //    }

    //    /// <summary>
    //    /// Создать новый экземпляр параметра
    //    /// </summary>
    //    /// <param name="Channel">ИД канала</param>
    //    /// <param name="Param">ИД параметра</param>
    //    /// <param name="LastTime">Время последнего полученного значения</param>
    //    public ParamSendItem(int Channel, int Param, DateTime LastTime)
    //    {
    //        channel = Channel;
    //        param = Param;
    //        name = "";
    //        lasttime = LastTime;
    //    }

    //    #region IComparable<ParamSendItem> Members

    //    public int CompareTo(ParamSendItem other)
    //    {
    //        if (channel < other.channel) return -1;
    //        else if (channel > other.channel) return +1;
    //        else if (param < other.param) return -1;
    //        else if (param > other.param) return +1;
    //        else return 0;
    //    }

    //    #endregion
    //}

    public enum Quality : byte
    {
        Bad = 0x00,
        //HasSkipp = 0x10,
        Good = 0x80
    }

    /// <summary>
    /// Упакованная пачка
    /// </summary>
    [Serializable]
    public class PackedPackage : ICloneable
    {
        public int Id { get; protected set; }
        public DateTime DateFrom { get; protected set; }
        public DateTime DateTo { get; protected set; }
        public Interval Interval { get; protected set; }
        public byte[] Buffer { get; set; }

        public PackedPackage(int id, DateTime dateFrom, DateTime dateTo, Interval interval)
        {
            Id = id;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Interval = interval;
        }

        public PackedPackage(Package pack, Interval interval) : this(pack.Id, pack.DateFrom, pack.DateTo, interval) { }

        /// <summary>
        /// Создать пустую пачку и выстовить ей свойства такие же как и в упакованной пачке
        /// </summary>
        /// <returns></returns>
        public Package GetPackage()
        {
            Package pack = new Package();
            pack.Id = Id;
            pack.DateFrom = DateFrom;
            pack.DateTo = DateTo;
            return pack;
        }

        #region ICloneable Members

        public object Clone()
        {
            PackedPackage ret = new PackedPackage(Id, DateFrom, DateTo, Interval);
            ret.Buffer = new byte[Buffer.Length];
            Buffer.CopyTo(ret.Buffer, 0);
            return ret;
        }

        #endregion
    }

    public class UltimateZipper
    {
        public bool UseGZip { get; set; }

        public UltimateZipper()
        {
            UseGZip = true;
        }

        public PackedPackage Pack(Package package)
        {
            PackedPackage retPackage = null;
            List<int> valuesList, gcdList = new List<int>();
            DateTime prevTime = DateTime.MinValue;
            foreach (ParamValueItem item in package.Values)
            {
                if (prevTime > DateTime.MinValue)
                {
                    //int period = (int)Interval.GetInterval(prevTime, item.Time).ToDouble();
                    int period = (int)(Interval.GetInterval(prevTime, item.Time) as SecondsInterval).Seconds;
                    if (!gcdList.Contains(period)) gcdList.Add(period);
                }
                prevTime = item.Time;
            }
            while (gcdList.Count > 1)
            {
                valuesList = gcdList;
                gcdList = new List<int>();
                for (int i = 0; i + 1 < valuesList.Count; i += 2)
                {
                    int gcdValue = gcd(valuesList[i], valuesList[i + 1]);
                    if (!gcdList.Contains(gcdValue)) gcdList.Add(gcdValue);
                }
                if ((valuesList.Count & 1) == 1)
                {
                    if (!gcdList.Contains(valuesList[valuesList.Count - 1])) gcdList.Add(valuesList[valuesList.Count - 1]);
                }
            }
            double intervalQuantum;
            if (gcdList.Count > 0)
                intervalQuantum = gcdList[0];
            else
                intervalQuantum = 0;
            retPackage = new PackedPackage(package, new SecondsInterval(intervalQuantum));

            byte[] buffer = null;

            using (MemoryStream memStream = new MemoryStream())
            {
                if (UseGZip)    // использовать сжатие
                {
                    using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Compress, true))
                    {
                        using (BinaryWriter writer = new BinaryWriter(zipStream))
                        {
                            prevTime = WriteValues(writer, package, intervalQuantum);
                        }
                    }
                }
                else            // не исмользовать сжатие
                {
                    using (BinaryWriter writer = new BinaryWriter(memStream))
                    {
                        prevTime = WriteValues(writer, package, intervalQuantum);
                    }
                }
                buffer = memStream.ToArray();
            }
            retPackage.Buffer = buffer;

            return retPackage;
        }

        private static DateTime WriteValues(BinaryWriter writer, Package package, double intervalQuantum)
        {
            double interval;
            int times;
            DateTime prevTime = package.DateFrom;
            foreach (ParamValueItem receiveItem in package.Values)
            {
                //interval = Interval.GetInterval(prevTime, receiveItem.Time).ToDouble();
                interval = (Interval.GetInterval(prevTime, receiveItem.Time) as SecondsInterval).Seconds;
                if (intervalQuantum != 0)
                    times = (int)(interval / intervalQuantum);
                else
                    times = 0;
                if (byte.MinValue > times || times >= byte.MaxValue)
                {
                    writer.Write(byte.MaxValue);
                    writer.Write(times);
                }
                else writer.Write((byte)times);
                //writer.Write(receiveItem.tim.ToBinary());
                writer.Write((byte)receiveItem.Quality);
                writer.Write(receiveItem.Value);
                prevTime = receiveItem.Time;
            }
            return prevTime;
        }

        private int gcd(int m, int n)
        {
            bool n_odd, m_odd;
            if (n == 0) return m;
            if (m == 0) return n;
            if (n == 1 || m == 1) return 1;
            n_odd = (n & 1) == 0;
            m_odd = (m & 1) == 0;
            if (n_odd && m_odd) return gcd(m >> 1, n >> 1) << 1;
            if (m_odd && !n_odd) return gcd(m >> 1, n);
            if (!m_odd && n_odd) return gcd(m, n >> 1);
            return gcd(n, Math.Abs(m - n));
        }

        public Package Unpack(PackedPackage package)
        {
            Package retPackage = package.GetPackage();

            using (MemoryStream mem = new MemoryStream())
            {
                if (UseGZip)
                {
                    using (MemoryStream m = new MemoryStream(package.Buffer))
                    {
                        using (GZipStream g = new GZipStream(m, CompressionMode.Decompress))
                        {
                            byte[] buff = new byte[1 << 12];
                            int c;
                            while ((c = g.Read(buff, 0, buff.Length)) > 0) mem.Write(buff, 0, c);
                        }
                    }

                }
                else mem.Write(package.Buffer, 0, package.Buffer.Length);
                mem.Position = 0;
                using (BinaryReader r = new BinaryReader(mem, System.Text.Encoding.ASCII))
                {
                    DateTime prevTime = package.DateFrom;
                    int times;
                    while (mem.Length > mem.Position)
                    {
                        ParamValueItem param = new ParamValueItem();

                        //param.par = package.Id;
                        //param.channel = channel_id;

                        times = r.ReadByte();
                        if (times == byte.MaxValue)
                        {
                            times = r.ReadInt32();
                        }
                        prevTime = param.Time = package.Interval.GetTime(prevTime, times);
                        param.Quality = (Quality)r.ReadByte();
                        param.Value = r.ReadDouble();

                        retPackage.Values.Add(param);
                    }
                }
            }
            return retPackage;
        }
    }

    #region Проверка соединения
    /// <summary>
    /// Интерфейс для кривого таймаута
    /// </summary>
    [ServiceContract]
    public interface ITestConnection<T>
    {
        [OperationContract]
        bool Test(T arg);
    }
    /// <summary>
    /// Кривой способ реализации таймаута для ремоутинга
    /// </summary>
    public static class TestConnection<T>
    {
        internal class TestTargetArgument
        {
            public ITestConnection<T> target;
            public bool result;
            public T Argument { get; set; }

            public TestTargetArgument()
            {
                target = null;
                result = false;
            }
        }

        static private int defaultTimeout = 5000;

        /// <summary>
        /// Проверка соединения
        /// </summary>
        /// <param name="target">Проверяемый объект</param>
        /// <returns>true - если есть соединение, иначе - false</returns>
        public static bool Test(ITestConnection<T> target, T argument)
        {
            return Test(target, argument, false);
        }
        /// <summary>
        /// Проверка соединения
        /// </summary>
        /// <param name="target">Проверяемый объект</param>
        /// <param name="generateException">Генерировать исключение при отсутствии соединения</param>
        /// <returns>true - если есть соединение, иначе - false</returns>
        public static bool Test(ITestConnection<T> target, T argument, bool generateException)
        {
            return Test(target, argument, defaultTimeout, generateException);
        }
        /// <summary>
        /// Проверка соединения
        /// </summary>
        /// <param name="target">Проверяемый объект</param>
        /// <param name="timeout">Таймаут проверки (мс)</param>
        /// <returns>true - если есть соединение, иначе - false</returns>
        public static bool Test(ITestConnection<T> target, T argument, int timeout)
        {
            return Test(target, argument, timeout, false);
        }
        /// <summary>
        /// Проверка соединения
        /// </summary>
        /// <param name="target">Проверяемый объект</param>
        /// <param name="timeout">Таймаут проверки (мс)</param>
        /// <param name="generateException">Генерировать исключение при отсутствии соединения</param>
        /// <returns>true - если есть соединение, иначе - false</returns>
        public static bool Test(ITestConnection<T> target, T argument, int timeout, bool generateException)
        {
            Thread thr = new Thread(new ParameterizedThreadStart(Method));
            TestTargetArgument arg = new TestTargetArgument();
            bool res = false;

            if (target == null)
            {
                //throw new ArgumentNullException("target");
                if (generateException)
                    throw new ArgumentNullException("target");
                else
                    return false;
            }

            try
            {
                arg.target = target;
                arg.Argument = argument;

                thr.Start(arg);
                res = thr.Join(timeout);
                if (!res) thr.Abort();
                res = arg.result;
            }
            catch (UriFormatException) { res = false; }
            catch (SocketException) { res = false; }
            catch (RemotingException) { res = false; }

            if (generateException && !res)
                throw new Exception("Connection.Test: timeout exception");

            return res;
        }

        private static void Method(object target)
        {
            TestTargetArgument arg = null;

            try
            {
                arg = target as TestTargetArgument;
                if (arg != null)
                {
                    arg.result = arg.target.Test(arg.Argument);
                    //arg.result = true;
                }
            }
            catch (Exception)
            {
                if (arg != null)
                    arg.result = false;
            }
        }
    }
    #endregion
}
