using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace Assignment
{
    [Serializable]
    public class MultiDimensionalTable
    {
        Dictionary<double, object> data;
        DimensionInfo[] dimentionInfo;

        public MultiDimensionalTable()
        {
            Init();
            AddDimension("X", "", 0f);
            SetValue(0f, 0f);
        }
        public MultiDimensionalTable(DataSet dataset)
        {
            double valZ = 0.0;
            bool useZ = false;

            Init();

            if (dataset.Tables.Count > 1)
            {
                AddDimension("Y", "", 0.0);
                AddDimension("Z", "", 0.0);
                useZ = true;
            }

            foreach (DataTable table in dataset.Tables)
            {
                if (useZ) valZ++;
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    for (int j = 1; j < row.ItemArray.Length; j++)
                    {
                        double val;

                        try
                        {
                            val = (double)table.Rows[i][j];
                        }
                        catch { val = double.NaN; }
                        if (!double.IsNaN(val))
                        {
                            if (useZ)
                                SetValue(val, (double)table.Rows[0][j], (double)table.Rows[i][0], valZ);
                            else
                            {
                                if (DimensionInfo.Length < 1)
                                    AddDimension("X", "", (double)table.Rows[0][j]);
                                if (table.Rows.Count > 2)
                                {
                                    if (DimensionInfo.Length < 2) AddDimension("Y", "", (double)table.Rows[i][0]);
                                    SetValue(val, (double)table.Rows[0][j], (double)table.Rows[i][0]);
                                }
                                else
                                    SetValue(val, (double)table.Rows[0][j]);
                            }
                        }
                    }
                }
            }
        }

        private void Init()
        {
            Dictionary<double, object> dat = new Dictionary<double, object>();
            data = new Dictionary<double, object>();

            dimentionInfo = new DimensionInfo[] { };
        }

        /// <summary>
        /// Массив данных об измерениях
        /// </summary>
        public DimensionInfo[] DimensionInfo
        {
            get { return dimentionInfo; }
        }

        #region Работа с измерениями и данными
        /// <summary>
        /// Добавляет новое измерение в конец существующих.
        /// </summary>
        /// <param name="name">Имя измерения.</param>
        /// <param name="measure">Единица измерения.</param>
        /// <param name="value">Значение, которое присвоится текущей ветке измерений.</param>
        public void AddDimension(string name, string measure, double value)
        {
            Dictionary<double, object> new_data;
            List<DimensionInfo> lstDimensionInfo = new List<DimensionInfo>();

            new_data = new Dictionary<double, object>();
            new_data.Add(value, data);

            lstDimensionInfo.AddRange(dimentionInfo);
            lstDimensionInfo.Add(new DimensionInfo(name, measure));
            
            dimentionInfo = lstDimensionInfo.ToArray();
            data = new_data;
        }
        /// <summary>
        /// Удаляет измерение (In construction).
        /// Пока удаляется только последнее измерение.
        /// </summary>
        /// <param name="dInfo">Удаляемое измерение.</param>
        public void RemoveDimension(DimensionInfo dInfo)
        {
            DimensionInfo[] dimensions;
            int i;

            //throw new NotImplementedException("Не реализовано, пока.");

            if (dimentionInfo.Length == 1)
                throw new Exception("Нельзя удалять единственное измерение.");

            for (i = 0; i < dimentionInfo.Length; i++)
            {
                if (dimentionInfo[i] == dInfo)
                    break;
            }

            if (i != dimentionInfo.Length - 1)
                throw new Exception("Удалять можно только последнее измерение.");
            if (i == dimentionInfo.Length)
                throw new Exception("Измерение не найдено.");

            dimensions = new DimensionInfo[dimentionInfo.Length - 1];
            Array.Copy(dimentionInfo, dimensions, dimentionInfo.Length - 1);

            Dictionary<double, object>.ValueCollection.Enumerator ptr = data.Values.GetEnumerator();
            if (!ptr.MoveNext()) throw new Exception("Ошибка данных.");

            try
            {
                data = (Dictionary<double, object>)ptr.Current;
                dimentionInfo = dimensions;
            }
            catch
            {
                throw new Exception("Ошибка данных.");
            }
        }
        public void SwapDimensions(string name1, string name2)
        {
            throw new NotImplementedException("Не реализовано, пока.");
        }

        /// <summary>
        /// Очистка всех значений измерений, кроме указанных, в заданной координатной ветке.
        /// </summary>
        /// <param name="values">Набор фильтруемых значений.</param>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        public void FilterDimension(double[] values, params double[] coordinates)
        {
            double[] coords = new double[coordinates.Length + 1];
            double[] vals = GetDimension(coordinates);
            bool found;

            Array.Copy(coordinates, coords, coordinates.Length);
            
            for (int i = 0; i < vals.Length; i++)
            {
                found = false;
                for (int j = 0; j < values.Length; j++)
                {
                    if (vals[i] == values[j])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    //if (i == vals.Length - 1)
                    //{
                    //    SetValue(0f);
                    //}
                    coords[coordinates.Length] = vals[i];
                    RemoveValue(coords);
                }
            }
        }

        public void ChangeDimensionValue(double value, params double[] coordinates)
        {
            Dictionary<double, object>[] arrptr;
            double[] coords = new double[coordinates.Length - 1];
            double key;
            
            if (double.IsNaN(value)) throw new Exception("Значение измерения не может быть NaN.");

            if (coordinates.Length > 1)
                Array.Copy(coordinates, coords, coordinates.Length - 1);
            else
                coords = null;
            arrptr = GetAllData(false, data, coordinates);

            if (arrptr == null || arrptr.Length == 0) throw new Exception("Измерение не найдено.");

            key = coordinates[coordinates.Length - 1];
            foreach (var item in arrptr)
            {
                object tmp;

                if (item.ContainsKey(key))
                {
                    if (item.ContainsKey(value) && value != key)
                        throw new Exception("Такое значение уже существует.");

                    tmp = item[key];
                    item.Remove(key);
                    item.Add(value, tmp);
                    SortKeys(item);
                }
                else
                    throw new Exception("Измерение не найдено.");
            }
        }

        /// <summary>
        /// Выдает значение по полному набору координат.
        /// </summary>
        /// <param name="coordinates">Полный набор координат в прямом порядке.</param>
        /// <returns>Значение.</returns>
        public double GetValue(params double[] coordinates)
        {
            double[] coords;
            Dictionary<double, object> ptr = data;
            Dictionary<double, object> tmp;
            object obj;
            double left, right;

            coordinates = SetDefaultCoordinates(coordinates);

            if (coordinates.Length != dimentionInfo.Length)
                throw new ArgumentException("Неверное количество аргументов");

            coords = new double[coordinates.Length];
            
            Array.Copy(coordinates, coords, coordinates.Length);
            Array.Reverse(coords);

            return GetValue(data, coords);
        }

        /// <summary>
        /// Выдает массив координат измерения.
        /// </summary>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        /// <returns>Массив координат.</returns>
        public double[] GetDimension(params double[] coordinates)
        {
            return GetDimension(Approx.None, coordinates);
        }
        /// <summary>
        /// Выдает массив координат измерения.
        /// </summary>
        /// <param name="approx">Тип аппроксимации.</param>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        /// <returns>Массив координат.</returns>
        public double[] GetDimension(Approx approx, params double[] coordinates)
        {
            Dictionary<double, object> ptr;
            List<double> lstDimensions = new List<double>();

            if (coordinates.Length >= dimentionInfo.Length)
                throw new ArgumentException("Слишком большое количество аргументов");

            switch (approx)
            {
                case Approx.None:
                    ptr = GetData(false, coordinates);
                    if (ptr != null)
                        foreach (var item in ptr.Keys)
                            lstDimensions.Add(item);
                    break;
                case Approx.Linear:
                    if (coordinates == null || coordinates.Length == 0) 
                        goto case Approx.None;

                    double lastcoord = coordinates[coordinates.Length - 1];
                    double left, right;

                    GetNeighbors(out left, out right, coordinates);
                    if (lastcoord == left || lastcoord == right)
                        goto case Approx.None;
                    if (double.IsNaN(left) || double.IsNaN(right))
                        throw new Exception("Измерение не найдено.");

                    bool exists;
                    
                    double[] coords = new double[coordinates.Length];
                    Array.Copy(coordinates, coords, coordinates.Length);
                    coords[coords.Length - 1] = left;
                    ptr = GetData(false, coords);
                    foreach (var item in ptr.Keys)
                        lstDimensions.Add(item);
                    
                    coords[coords.Length - 1] = right;
                    ptr = GetData(false, coords);
                    
                    foreach (var item in ptr.Keys)
                    {
                        exists = false;
                        foreach (var item2 in lstDimensions)
                        {
                            if (item2 == item)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                            lstDimensions.Add(item);
                    }
                    break;
                case Approx.Quadratic:
                    break;
            }

            lstDimensions.Sort();
            return lstDimensions.ToArray();
        }

        /// <summary>
        /// Устанавливает новое значение. Если такой координатной ветки нет, то она будет создана.
        /// </summary>
        /// <param name="value">Добавляемое значение.</param>
        /// <param name="coordinates">Полный набор координат в прямом порядке.</param>
        public void SetValue(double value, params double[] coordinates)
        {
            double[] coords = new double[coordinates.Length - 1];
            object obj;
            
            if (coordinates.Length != dimentionInfo.Length)
                throw new ArgumentException("Неверное количество аргументов");

            for (int i = 1; i < coordinates.Length; i++) coords[i - 1] = coordinates[i];

            Array.Reverse(coords);
            obj = GetData(true, coords);

            if (obj is Dictionary<double, object>)
            {
                Dictionary<double, object> tmp = obj as Dictionary<double, object>;

                tmp[coordinates[0]] = value;
            }
        }
        /// <summary>
        /// Удаляет ветку значений.
        /// </summary>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        public void RemoveValue(params double[] coordinates)
        {
            Dictionary<double, object>[] arrptr;
            double[] coords;
            double key;

            if (coordinates.Length == 0) throw new Exception("Измерение не найдено.");

            coords = new double[coordinates.Length - 1];
            arrptr = GetAllData(false, data, coordinates);

            if (arrptr == null || arrptr.Length == 0)
                return;
                //throw new Exception("Измерение не найдено.");

            key = coordinates[coordinates.Length - 1];
            foreach (var ptr in arrptr)
            {
                if (ptr.ContainsKey(key))
                {
                    if (arrptr.Length == 1 && ptr.Count == 1) 
                        throw new Exception("Нельзя удалять единственную ветку.");
                    ptr.Remove(key);
                }
            }
        }
        /// <summary>
        /// Удаляет ветку значений.
        /// </summary>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        //public void RemoveValue(Dictionary<double, object> data, int nulls, params double[] coordinates)
        //{
        //    if (nulls > 0)
        //    {
        //        double[] coords = new double[coordinates.Length - 1];

        //        for (int i = 1; i < coordinates.Length; i++)
        //            coords[i - 1] = coordinates[i];

        //        foreach (var item in data.Values)
        //        {
        //            RemoveValue((Dictionary<double, object>)item, nulls - 1, coords);
        //        }
        //    }
        //    else
        //    {
        //        double key = coordinates[coordinates.Length - 1];

        //        if (data.ContainsKey(key))
        //            data.Remove(key);
        //        else
        //            throw new Exception("Измерение не найдено.");
        //    }
        //}

        public DataTable GetTable(double[] coordinates)
        {
            DataTable table = new DataTable();
            double[] coords = new double[coordinates.Length + 1];
            bool dblView;

            coordinates.CopyTo(coords, 0);

            int res = DimensionInfo.Length - coordinates.Length;
            if (res > 2) return table;

            dblView = res == 2;

            if (dblView)
            {
                DataColumn col = new DataColumn();
                col.Unique = true;
                col.ColumnName = "Y\\X";
                table.Columns.Add(col);
            }

            foreach (var item in GetDimension(coordinates))
            {
                Dictionary<int, double> dicValues = new Dictionary<int, double>();
                double[] xs = new double[DimensionInfo.Length];
                double[] x_coords;

                coords[coordinates.Length] = item;
                coords.CopyTo(xs, 0);
                Array.Reverse(xs);

                if (dblView)
                    x_coords = GetDimension(coords);
                else
                    x_coords = new double[] { item };
                foreach (var x in x_coords)
                {
                    DataColumn column;

                    if (!table.Columns.Contains(x.ToString()))
                    {
                        int index;

                        column = table.Columns.Add(x.ToString(), typeof(double));
                        index = column.Ordinal;
                        for (int i = index - 1; i >= 0; i--)
                        {
                            double val;

                            try
                            {
                                val = double.Parse(table.Columns[i].ColumnName);
                            }
                            catch (FormatException) { val = double.NaN; }

                            if (val > x) index = i;
                        }
                        column.SetOrdinal(index);
                        if (table.Rows.Count == 0) table.Rows.Add();
                        table.Rows[0][column] = x;
                    }
                    else
                        column = table.Columns[x.ToString()];
                    xs[0] = x;
                    if (!dicValues.ContainsKey(column.Ordinal))
                        dicValues.Add(column.Ordinal, GetValue(xs));
                }

                DataRow row;

                if (dblView)
                {
                    row = table.NewRow();
                    row[0] = item;
                }
                else
                {
                    if (table.Rows.Count < 2)
                        row = table.Rows.Add();
                    row = table.Rows[1];
                }

                foreach (var key in dicValues.Keys)
                {
                    if (double.IsNaN(dicValues[key]))
                        row[key] = "";
                    else
                        row[key] = dicValues[key];
                }
                if (dblView) table.Rows.Add(row);
            }

            return table;
        }
        #endregion

        #region Вторичные штуки
        /// <summary>
        /// Возвращает минимально необходимое количество указанных координат.
        /// </summary>
        /// <returns>Минимальное количество координат.</returns>
        public int GetMinArgs()
        {
            Dictionary<double, object> ptr = data;
            object obj;
            int count = dimentionInfo.Length;
            
            while (ptr != null)
            {
                if (ptr.Keys.Count != 1) return count;

                Dictionary<double, object>.ValueCollection.Enumerator tmp = ptr.Values.GetEnumerator();
                obj = tmp.Current;
                if (obj is Dictionary<double, object>)
                    ptr = (Dictionary<double, object>)obj;
                else
                    ptr = null;

                count--;
            }

            return count;
        }
        /// <summary>
        /// Возвращает максимально возможное количество координат.
        /// </summary>
        /// <returns>Максимальное количество координат.</returns>
        public int GetMaxArgs()
        {
            return dimentionInfo.Length;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Выставление недостающих координат значениями по умолчанию.
        /// </summary>
        /// <param name="coordinates">Набор координат в прямом порядке.</param>
        /// <returns>Полный набор координат в прямом порядке.</returns>
        private double[] SetDefaultCoordinates(double[] coordinates)
        {
            Dictionary<double, object> ptr = data;
            double[] res;
            int dif;

            if (coordinates.Length >= dimentionInfo.Length) return coordinates;

            res = new double[dimentionInfo.Length];
            dif = dimentionInfo.Length - coordinates.Length;

            while (ptr != null && dif > 0)
            {
                if (ptr.Keys.Count != 1) return coordinates;

                Dictionary<double, object>.KeyCollection.Enumerator tmp = ptr.Keys.GetEnumerator();
                tmp.MoveNext();
                res[coordinates.Length + dif - 1] = tmp.Current;
                dif--;
            }

            for (int i = 0; i < coordinates.Length; i++)
                res[i] = coordinates[i];

            return res;
        }
        private void ChangeDimensionValue(double value, Dictionary<double, object> data, int nulls,
            params double[] coordinates)
        {
            if (nulls > 0)
            {
                double[] coords = new double[coordinates.Length - 1];

                for (int i = 1; i < coordinates.Length; i++)
                    coords[i - 1] = coordinates[i];

                foreach (var item in data.Values)
                {
                    ChangeDimensionValue(value, (Dictionary<double, object>)item, nulls - 1, coords);
                }
            }
            else
            {
                object tmp;
                double key = coordinates[coordinates.Length - 1];

                if (data.ContainsKey(key))
                {
                    if (data.ContainsKey(value) && value != key)
                        throw new Exception("Такое значение уже существует.");

                    tmp = data[key];
                    data.Remove(key);
                    data.Add(value, tmp);
                }
                else
                    throw new Exception("Измерение не найдено.");
            }
        }

        //Обратный порядок
        private double GetValue(object data, params double[] coordinates)
        {
            IDictionary idata = (IDictionary)data;
            double[] coords;
            double left, right;
            double val_l = 0, val_r = 0;
            double k;

            coords = new double[coordinates.Length - 1];
            for (int i = 1; i < coordinates.Length; i++)
                coords[i - 1] = coordinates[i];

            GetNeighbors(out left, out right, (IDictionary)data, coordinates[0]);

            if (coordinates.Length == 1)
            {
                if (!double.IsNaN(left) && left == coordinates[0]) return (double)idata[left];
                if (!double.IsNaN(right) && right == coordinates[0]) return (double)idata[right];
            }
            else
            {
                if (!double.IsNaN(left) && left == coordinates[0]) return GetValue(idata[left], coords);
                if (!double.IsNaN(right) && right == coordinates[0]) return GetValue(idata[right], coords);
            }

            if (double.IsNaN(left) && double.IsNaN(right)) return double.NaN;
            if (double.IsNaN(left))
            {
                left = right;
                GetNextCoord(out right, (IDictionary)data, right);
            }
            if (double.IsNaN(right))
            {
                right = left;
                GetPrevCoord(out left, (IDictionary)data, left);
            }
            
            if (coordinates.Length == 1)
            {
                val_l = (double)idata[left];
                val_r = (double)idata[right];
            }
            else
            {
                val_l = GetValue(idata[left], coords);
                val_r = GetValue(idata[right], coords);
            }
            
            if (right != left)
                k = (coordinates[0] - left) / (right - left);
            else
                throw new Exception("Left==Right");

            //if (coordinates.Length == 1)
            //{
            //    val_l = (double)idata[left];
            //    val_r = (double)idata[right];
            //}
            //else
            //{
            //    val_l = GetValue(idata[left], coords);
            //    val_r = GetValue(idata[right], coords);
            //}

            return val_l + (val_r - val_l) * k;
        }

        private void GetPrevCoord(out double prev, IDictionary data, double coordinate)
        {
            double lastcoord = coordinate;
            prev = double.NaN;

            if (data == null) throw new ArgumentNullException("data");

            foreach (double item in data.Keys)
                if (item < lastcoord && (double.IsNaN(prev) || item > prev)) prev = item;
        }
        private void GetNextCoord(out double next, IDictionary data, double coordinate)
        {
            double lastcoord = coordinate;
            next = double.NaN;

            if (data == null) throw new ArgumentNullException("data");

            foreach (double item in data.Keys)
                if (lastcoord < item && (double.IsNaN(next) || item < next)) next = item;
        }
        private void GetNeighbors(out double left, out double right, IDictionary data, double coordinate)
        {
            double lastcoord = coordinate;
            left = right = double.NaN;

            if (data == null) throw new ArgumentNullException("data");

            foreach (double item in data.Keys)
            {
                if (item <= lastcoord && (double.IsNaN(left) || item > left)) left = item;
                if (lastcoord <= item && (double.IsNaN(right) || item < right)) right = item;
            }
        }
        private void GetNeighbors(out double left, out double right, params double[] coordinates)
        {
            left = right = double.NaN;

            if (coordinates == null || coordinates.Length == 0) return;

            double[] coords = new double[coordinates.Length - 1];
            double[] arrPrevCoords;
            double lastcoord = coordinates[coordinates.Length - 1];

            Array.Copy(coordinates, coords, coordinates.Length - 1);
            arrPrevCoords = GetDimension(coords);

            foreach (var item in arrPrevCoords)
            {
                if (item <= lastcoord && (double.IsNaN(left) || lastcoord - item < lastcoord - left)) left = item;
                if (lastcoord <= item && (double.IsNaN(right) || item - lastcoord < right - lastcoord)) right = item;
            }
        }
        /// <summary>
        /// Возвращает словарь измерений.
        /// </summary>
        /// <param name="create">Флаг создания недостаюших измерений.</param>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        /// <returns>Словарь измерений.</returns>
        private Dictionary<double, object> GetData(bool create, params double[] coordinates)
        {
            Dictionary<double, object> res;
            int nulls;

            res = GetData(create, out nulls, coordinates);
            if (nulls > 0) throw new Exception("Присутствуют пустые измерения.");

            return res;
        }
        /// <summary>
        /// Возвращает словарь измерений.
        /// (Возможно заполнение последний измерений double.NaN для последующего рекурсивного обхода)
        /// </summary>
        /// <param name="create">Флаг создания недостаюших измерений.</param>
        /// <param name="nulls">Возвращаемое количество пустых измерений.</param>
        /// <param name="coordinates">Набор координат в обратном порядке.</param>
        /// <returns>Словарь измерений.</returns>
        private Dictionary<double, object> GetData(bool create, out int nulls, params double[] coordinates)
        {
            Dictionary<double, object> ptr = data;
            object obj = null;

            nulls = 0;
            if (coordinates != null)
            {
                for (int i = 0; i < coordinates.Length; i++)
                {
                    if (!double.IsNaN(coordinates[i]))
                    {
                        if (nulls > 0) throw new Exception("Пустые измерения не последовательны.");
                        if (!ptr.TryGetValue(coordinates[i], out obj))
                        {
                            if (create)
                            {
                                obj = new Dictionary<double, object>();
                                ptr.Add(coordinates[i], obj);
                                SortKeys(ptr);
                            }
                            else
                                return null;
                        }

                        ptr = (Dictionary<double, object>)obj;
                    }
                    else
                    {
                        nulls++;
                    }
                }
            }

            return ptr;
        }

        private void SortKeys(Dictionary<double, object> data)
        {
            Dictionary<double, object> tmp = new Dictionary<double, object>();
            List<double> lstKeys = new List<double>();

            if (data == null) throw new ArgumentNullException("data");

            foreach (var item in data.Keys)
            {
                lstKeys.Add(item);
                tmp.Add(item, data[item]);
            }
            lstKeys.Sort();
            data.Clear();
            foreach (var item in lstKeys)
                data.Add(item, tmp[item]);
        }

        private Dictionary<double, object>[] GetAllData(bool create, Dictionary<double, object> data, params double[] coordinates)
        {
            List<Dictionary<double, object>> lstResult = new List<Dictionary<double, object>>();
            Dictionary<double, object> ptr = data;
            double[] coords;
            double key = coordinates[0];

            if (coordinates == null || coordinates.Length == 0) return null;

            coords = new double[coordinates.Length - 1];
            for (int i = 1; i < coordinates.Length; i++)
                coords[i - 1] = coordinates[i];

            if (double.IsNaN(key))
            {
                foreach (Dictionary<double, object> item in data.Values)
                {
                    if (coordinates.Length > 1)
                        lstResult.AddRange(GetAllData(create, item, coords));
                    else
                        lstResult.Add(item);
                }
            }else
            {
                if (data.ContainsKey(key))
                {
                    if (coordinates.Length == 1)
                        lstResult.Add(data);
                    else
                        lstResult.AddRange(GetAllData(create, (Dictionary<double, object>)data[key], coords));
                }
                //else
                //    throw new Exception("Измерение не найдено.");
            }

            return lstResult.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Класс информации об измерении
    /// </summary>
    [Serializable]
    public class DimensionInfo : ICloneable
    {
        public string Name { get; set; }
        public string Measure { get; set; }

        public DimensionInfo()
        {
            Name = "";
            Measure = "";
        }
        public DimensionInfo(string name, string measure)
        {
            Name = name;
            Measure = measure;
        }

        public override bool Equals(object obj)
        {
            DimensionInfo di;
            bool res;

            if (obj == null) return false;
            //if (this.GetType() != obj.GetType()) return false;

            di = obj as DimensionInfo;

            if (di == null) return false;
            res = Name.Equals(di.Name);
            res = res && Measure.Equals(di.Measure);
            
            return res;
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            DimensionInfo res = new DimensionInfo((string)Name.Clone(), (string)Measure.Clone());

            return res;
        }

        #endregion
    }
    [Serializable]
    public class DimensionValue : DimensionInfo
    {
        public double Value { get; set; }

        public DimensionValue()
            : base()
        {
            Value = 0.0;
        }
        public DimensionValue(string name, string measure)
            : base(name, measure)
        {
            Value = 0.0;
        }
        public DimensionValue(string name, string measure, double value)
            : base(name, measure)
        {
            Value = value;
        }

        public override object Clone()
        {
            DimensionValue res = new DimensionValue();
            DimensionInfo bres = (DimensionInfo)base.Clone();

            res.Name = bres.Name;
            res.Measure = bres.Measure;
            res.Value = Value;

            return res;
        }
        public override bool Equals(object obj)
        {
            DimensionValue dv;
            bool res = base.Equals(obj);

            if (res)
            {
                dv = obj as DimensionValue;
                res = dv != null && Value == dv.Value;
            }

            return res;
        }
    }

    public class DimensionTypeConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            DimensionPropertyCollection obj = (DimensionPropertyCollection)value;
            PropertyDescriptorCollection result = new PropertyDescriptorCollection(null);
            string category;

            for (int i = obj.DimensionInfo.Length - 1; i >= 0; i--)
            {
                category = "Измерение";
                if (obj.DimensionInfo.Length > 1)
                    category += " " + (obj.DimensionInfo.Length - i).ToString();
                result.Add(new DimensionNamePropertyDescriptor((DimensionInfo)obj.DimensionInfo[i], category));
                result.Add(new DimensionMeasurePropertyDescriptor((DimensionInfo)obj.DimensionInfo[i], category));
                if (obj.DimensionInfo[i] is DimensionValue)
                    result.Add(new DimensionValuePropertyDescriptor((DimensionValue)obj.DimensionInfo[i], category));
            }

            return result;
        }

        protected class DimensionNamePropertyDescriptor : SimplePropertyDescriptor
        {
            DimensionInfo dimensionInfo;
            string category;

            public DimensionNamePropertyDescriptor(DimensionInfo dimensionInfo, string category)
                : base(typeof(DimensionPropertyCollection), "name", typeof(string))
            {
                this.dimensionInfo = dimensionInfo;
                this.category = category;
            }

            public override string Category
            {
                get { return category; }
            }
            public override string DisplayName
            {
                get { return "Название измерения"; }
            }
            public override string Description
            {
                get { return "Описание"; }
            }
            public override object GetValue(object component)
            {
                return dimensionInfo.Name;
            }
            public override void SetValue(object component, object value)
            {
                dimensionInfo.Name = (string)value;
            }
        }
        protected class DimensionMeasurePropertyDescriptor : SimplePropertyDescriptor
        {
            DimensionInfo dimensionInfo;
            string category;

            public DimensionMeasurePropertyDescriptor(DimensionInfo dimensionInfo, string category)
                : base(typeof(DimensionPropertyCollection), "measure", typeof(string))
            {
                this.dimensionInfo = dimensionInfo;
                this.category = category;
            }

            public override string Category
            {
                get { return category; }
            }
            public override string DisplayName
            {
                get { return "Единица измерения"; }
            }
            public override string Description
            {
                get { return "Описание"; }
            }
            public override object GetValue(object component)
            {
                return dimensionInfo.Measure;
            }
            public override void SetValue(object component, object value)
            {
                dimensionInfo.Measure = (string)value;
            }
        }
        protected class DimensionValuePropertyDescriptor : SimplePropertyDescriptor
        {
            DimensionValue dimensionInfo;
            string category;

            public DimensionValuePropertyDescriptor(DimensionValue dimensionInfo, string category)
                : base(typeof(DimensionPropertyCollection), "value", typeof(double))
            {
                this.dimensionInfo = dimensionInfo;
                this.category = category;
            }

            public override string Category
            {
                get { return category; }
            }
            public override string DisplayName
            {
                get { return "Значение"; }
            }
            public override string Description
            {
                get { return "Описание"; }
            }
            public override object GetValue(object component)
            {
                return dimensionInfo.Value;
            }
            public override void SetValue(object component, object value)
            {
                dimensionInfo.Value = (double)value;
            }
        }
    }
    [TypeConverter(typeof(DimensionTypeConverter))]
    public class DimensionPropertyCollection : ICloneable
    {
        List<DimensionInfo> lstDimensions;

        [Browsable(false)]
        public DataTable Table { get; set; }
        public DimensionInfo[] DimensionInfo
        {
            get { return lstDimensions.ToArray(); }
        }

        public DimensionPropertyCollection()
        {
            lstDimensions = new List<DimensionInfo>();
        }

        public void AddDimensionInfo(DimensionInfo dimensionInfo)
        {
            bool found = false;

            foreach(var item in lstDimensions)
                if (item.Name == dimensionInfo.Name)
                {
                    found = true;
                    break;
                }
            if (!found) lstDimensions.Add(dimensionInfo);
            //dicDimensions.Add(dimensionInfo, value);
        }
        public void AddDimensionInfo(DimensionInfo[] dimensionsInfo)
        {
            lstDimensions.AddRange(dimensionsInfo);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;

            DimensionPropertyCollection dpc = (DimensionPropertyCollection)obj;
            int len = DimensionInfo.Length;

            if (!CompareTables(Table, dpc.Table)) return false;
            if (len != dpc.DimensionInfo.Length) return false;
            
            for (int i = 0; i < len; i++)
                if (!DimensionInfo[i].Equals(dpc.DimensionInfo[i])) return false;

            return true;
        }

        private bool CompareTables(DataTable table1, DataTable table2)
        {
            DataTable table;

            if (table1 == table2) return true;
            if (table1 == null || table2 == null) return false;

            if (table1.Columns.Count != table2.Columns.Count) return false;
            if (table1.Rows.Count != table2.Rows.Count) return false;

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                for (int j = 0; j < table1.Columns.Count; j++)
                {
                    if (!table1.Rows[i][j].Equals(table2.Rows[i][j])) return false;
                }
            }
            
            //table = table1.Copy();
            ////if (table.Columns.Count > 0)
            //    //table.PrimaryKey = new DataColumn[] { table.Columns[0] };
            //table.Constraints.Clear();
            //foreach (DataColumn item in table.Columns)
            //    item.Unique = false;
            //table.Merge(table2, false);

            //return table.GetChanges() == null;
            return true;
        }

        #region ICloneable Members

        public object Clone()
        {
            DimensionPropertyCollection res = new DimensionPropertyCollection();

            foreach (var item in DimensionInfo)
                res.AddDimensionInfo((DimensionInfo)item.Clone());

            if (Table != null) res.Table = (DataTable)Table.Copy();

            return res;
        }

        #endregion
    }
}
