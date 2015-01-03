using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace COTES.ISTOK.Block
{
    public delegate void UpdateValueDelegate(ParameterItem param, ParamValueItem value);

    public class ValueBuffer
    {
        Dictionary<int, ParameterInfo> dicParameters = new Dictionary<int, ParameterInfo>();
        Dictionary<int, Tuple<Package, Package, Package>> dicPackages = new Dictionary<int, Tuple<Package, Package, Package>>();

        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public event UpdateValueDelegate UpdateValue = null;

        public ValueBuffer()
        {
            //
        }

        public void RegisterParameters(ParameterItem[] parameters, IDictionary channelProperties)
        {
            ParameterInfo paramInfo;

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            foreach (var item in parameters)
            {
                paramInfo = ParseParameter(item);//item.Idnum, item.Properties);

                if (channelProperties.Contains(CommonData.StoreDBProperty))
                {
                    try
                    {
                        int store = int.Parse(channelProperties[CommonData.StoreDBProperty].ToString());
                        paramInfo.SaveValues = store != 0;
                    }
                    catch (FormatException) { paramInfo.SaveValues = true; }
                }
                else
                    paramInfo.SaveValues = true;

                if (dicParameters.ContainsKey(item.Idnum))
                    dicParameters[item.Idnum] = paramInfo;
                else
                    dicParameters.Add(item.Idnum, paramInfo);
            }
        }
        public void UnregisterParameters(ParameterItem[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            foreach (var item in parameters)
            {
                dicParameters.Remove(item.Idnum);
                dicPackages.Remove(item.Idnum);
            }
        }

        public void AddValue(ParameterItem parameter, IEnumerable<ParamValueItem> values)
        {
            var packageTuple = GetPackageTuple(parameter.Idnum);
            
            ParamValueItem updateValue = null;

            foreach (var valueItem in values)
            {
                // choose value to notify
                if (updateValue == null
                    || valueItem.Time > updateValue.Time)
                {
                    updateValue = valueItem;
                }

                // get current package
                packageTuple = RenewPackageTuple(parameter, packageTuple, valueItem);

                Package package = packageTuple.Item2;

                var par = package.Values.FindLast(v => v.Time <= valueItem.Time);

                if (par != null && par.Time == valueItem.Time)
                {
                    par.Value = valueItem.Value;
                    par.Quality = valueItem.Quality;
                    par.Arguments = valueItem.Arguments;
                    par.ChangeTime = valueItem.ChangeTime;
                }
                else
                {
                    bool appertures = par != null && CheckAperture(parameter, par, valueItem);

                    // if values count excess
                    if (!appertures && package.Count >= NSI.DBPackageSize)
                    {
                        // if next package is empty, create new package
                        if (packageTuple.Item3 == null)
                        {
                            packageTuple = ShiftOnNewPackage(parameter, packageTuple.Item2, valueItem.Time);
                            //package = newPackage;
                            package = packageTuple.Item2;
                        }
                        // move last value to the next package
                        else if (packageTuple.Item3.Count < NSI.DBPackageSize)
                        {
                            if (valueItem.Time > package.DateTo)
                            {
                                FlushPackage(package);

                                package = packageTuple.Item3;
                            }
                            else
                            {
                                // sort values in current package
                                package.Normailze();
                                var lastValue = package.Values.Last();

                                package.Values.Remove(lastValue);
                                packageTuple.Item3.Add(lastValue);
                                package.DateTo = packageTuple.Item3.DateFrom;

                                FlushPackage(packageTuple.Item3);
                            }
                        }
                        // split packages
                        else
                        {
                            packageTuple = SplitPackages(parameter, packageTuple.Item1, packageTuple.Item2, packageTuple.Item3, valueItem.Time);

                            package = packageTuple.Item2;
                        }
                    }

                    if (appertures)
                    {
                        package.UpdateDates(valueItem.Time);
                    }
                    else
                    {
                        package.Add(valueItem);
                    }
                }
            }

            if (UpdateValue != null && updateValue != null)
            {
                UpdateValue(parameter, updateValue);
            }
        }

        /// <summary>
        /// Тотальное удаление значений параметра из буфера
        /// </summary>
        /// <param name="param"></param>
        public void RemoveValue(ParameterItem param)
        {
            dicParameters.Remove(param.Idnum);
            dicPackages.Remove(param.Idnum);
        }

        private Tuple<Package, Package, Package> SplitPackages(ParameterItem parameter,Package prevPackage, Package curPackage, Package nextPackage, DateTime valueTime)
        {
            Tuple<Package, Package, Package> packageTuple;

            Package newPackage = new Package()
            {
                Id = parameter.Idnum
            };

            // move values from current package
            newPackage.Values.AddRange(curPackage.Values.Skip(NSI.DBPackageSize * 2 / 3).Take(NSI.DBPackageSize / 3));
            curPackage.Values.RemoveRange(NSI.DBPackageSize * 2 / 3, NSI.DBPackageSize / 3);

            // move values from next package
            newPackage.Values.AddRange(nextPackage.Values.Take(NSI.DBPackageSize / 3));
            nextPackage.Values.RemoveRange(0, NSI.DBPackageSize / 3);

            // renew package times
            curPackage.Normailze();
            newPackage.Normailze();
            nextPackage.Normailze();
            curPackage.DateTo = newPackage.DateFrom;
            newPackage.DateTo = nextPackage.DateFrom;

            // save all packages
            FlushPackage(curPackage);
            FlushPackage(newPackage);
            FlushPackage(nextPackage);

            // correct buffer tuple and package variable
            if (valueTime < curPackage.DateTo)
            {
                packageTuple = Tuple.Create(prevPackage, curPackage, newPackage);
            }
            else
            {
                packageTuple = Tuple.Create(curPackage, newPackage, nextPackage);
                curPackage = newPackage;
            }
            dicPackages[parameter.Idnum] = packageTuple;
            return packageTuple;
        }

        private Tuple<Package, Package, Package> ShiftOnNewPackage(ParameterItem parameter, Package package, DateTime valueTime)
        {
            Package newPackage = new Package()
            {
                Id = parameter.Idnum,
                DateFrom = valueTime,
                DateTo = valueTime.AddSeconds(1),
            };
            package.DateTo = newPackage.DateFrom;

            FlushPackage(package);

            var packageTuple = Tuple.Create(package, newPackage, (Package)null);
            dicPackages[parameter.Idnum] = packageTuple;
            return packageTuple;
        }
        
        private Tuple<Package, Package, Package> RenewPackageTuple(ParameterItem parameter, Tuple<Package, Package, Package> packageTuple, ParamValueItem valueItem)
        {
            if (packageTuple == null
                || (packageTuple.Item1 != null && packageTuple.Item1.DateTo > valueItem.Time)
                || (packageTuple.Item3 != null && packageTuple.Item3.DateFrom <= valueItem.Time))
            {
                Package currentPackage = NSI.valReceiver.LoadPackage(parameter.Idnum, valueItem.Time);
                Package prevPackage = NSI.valReceiver.LoadPrevPackage(parameter.Idnum, valueItem.Time);
                Package nextPackage = NSI.valReceiver.LoadNextPackage(parameter.Idnum, valueItem.Time);

                if (currentPackage == null)
                {
                    if (prevPackage == null)
                    {
                        if (nextPackage == null)
                        {
                            currentPackage = new Package()
                            {
                                Id = parameter.Idnum,
                                DateFrom = valueItem.Time,
                                DateTo = valueItem.Time.AddSeconds(1),
                            };
                        }
                        else
                        {
                            currentPackage = nextPackage;
                            nextPackage = NSI.valReceiver.LoadNextPackage(parameter.Idnum, currentPackage.DateTo);
                        }
                    }
                    else
                    {
                        currentPackage = prevPackage;
                        prevPackage = NSI.valReceiver.LoadPrevPackage(parameter.Idnum, currentPackage.DateFrom);
                    }
                }

                // flush current package
                if (packageTuple != null)
                {
                    FlushPackage(packageTuple.Item2);
                }

                packageTuple = Tuple.Create(prevPackage, currentPackage, nextPackage);
                dicPackages[parameter.Idnum] = packageTuple;
            }
            return packageTuple;
        }

        public void FlushAll()
        {
            lock (dicPackages)
            {
                var paramIDs = dicPackages.Keys.ToArray();

                foreach (var item in paramIDs)
                {
                    ParameterInfo paramInfo = null;
                    Tuple<Package, Package, Package> tuple;

                    if (dicParameters.TryGetValue(item, out paramInfo)
                        && (paramInfo == null || paramInfo.SaveValues)
                        && dicPackages.TryGetValue(item, out tuple))
                    {
                        NSI.valReceiver.SavePackage(tuple.Item2);
                    }
                }
            }
        }

        private Tuple<Package, Package, Package> GetPackageTuple(int param_id)
        {
            if (dicPackages.ContainsKey(param_id))
                //return dicPackages[param_id] as Package;
                return dicPackages[param_id];
            else
                return null;
        }

        public Package GetPackage(int param_id)
        {
            var tuple = GetPackageTuple(param_id);
            if (tuple != null)
            {
                return tuple.Item2;
            }
            return null;
        }

        #region Private Methods
        private void FlushPackage(Package package)
        {
            ParameterInfo paramInfo = null;

            if (package == null)
                throw new ArgumentNullException("package");

            if (dicParameters.ContainsKey(package.Id))
                paramInfo = dicParameters[package.Id];

            if (paramInfo == null || paramInfo.SaveValues)
                NSI.valReceiver.SavePackage(package);
            //lock (dicPackages)
            //    dicPackages.Remove(package.Id);
        }
        /// <summary>
        /// Проверка на совпадение по апертуре
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="oldparam">Параметр-образец</param>
        /// <param name="newparam">Сравниваемый параметр</param>
        /// <returns>True, если параметр считается равным по апертуре, иначе - False</returns>
        private bool CheckAperture(ParameterItem parameter, ParamValueItem oldparam, ParamValueItem newparam)
        {
            ParameterInfo paramInfo = null; 
            
            if (!dicParameters.ContainsKey(parameter.Idnum)) return false;
            paramInfo = dicParameters[parameter.Idnum];

            if (!paramInfo.UseAperture) return false;

            if (paramInfo.UsePercents)
            {
                double val, limit;

                if (paramInfo.UseLimits)
                    val = Math.Abs(paramInfo.MaxValue - paramInfo.MinValue);
                else
                    val = oldparam.Value;

                limit = paramInfo.ApertureValue * val / 100;

                if (Math.Abs(oldparam.Value - newparam.Value) <= limit)
                    return true;
            }
            else
            {
                if (Math.Abs(oldparam.Value - newparam.Value) <= paramInfo.ApertureValue)
                    return true;
            }

            return false;
        }

        private ParameterInfo ParseParameter(ParameterItem parameterItem)
        {
            ParameterInfo res = new ParameterInfo();
            string str_aperture;
            string str_maxvalue = "";
            string str_minvalue = "";

            res.UseAperture =parameterItem.Contains(CommonData.ApertureProperty);

            if (res.UseAperture)
            {
                str_aperture = parameterItem.GetPropertyValue(CommonData.ApertureProperty);
                if (!string.IsNullOrEmpty(str_aperture))
                {
                    str_aperture = str_aperture.Trim();

                    res.UsePercents = str_aperture.EndsWith("%");
                    try
                    {
                        if (res.UsePercents)
                            str_aperture = str_aperture.Substring(0, str_aperture.Length - 1);
                        res.ApertureValue = double.Parse(str_aperture, NumberFormatInfo.InvariantInfo);
                    }
                    catch (FormatException ex)
                    {
                        if (log != null)
                            log.WarnException(String.Format("Ошибка проверки апертуры параметра {0}. Апертура игнорируется", parameterItem.Idnum), ex);
                        res.UseAperture = false;
                    }
                }
                else
                    res.UseAperture = false;
            }

            if (parameterItem.Contains(CommonData.MaxValueProperty))
                str_maxvalue = parameterItem.GetPropertyValue(CommonData.MaxValueProperty);
            if (parameterItem.Contains(CommonData.MinValueProperty))
                str_minvalue = parameterItem.GetPropertyValue(CommonData.MinValueProperty);

            try
            {
                if (string.IsNullOrEmpty(str_maxvalue) || string.IsNullOrEmpty(str_minvalue))
                {
                    res.UseLimits = false;
                }
                else
                {
                    res.MaxValue = double.Parse(str_maxvalue, NumberFormatInfo.InvariantInfo);
                    res.MinValue = double.Parse(str_minvalue, NumberFormatInfo.InvariantInfo);
                    res.UseLimits = true;
                }
            }
            catch (FormatException ex)
            {
                if (log != null)
                    log.WarnException(String.Format("Ошибка проверки границ значений параметра {0}. Апертура будет производиться по предыдущему значению", parameterItem.Idnum), ex);
                res.UseLimits = false;
            }

            return res;
        }
        #endregion
    }

    class ParameterInfo
    {
        public bool UseAperture { get; set; }
        public bool UsePercents { get; set; }
        public bool UseLimits { get; set; }
        public bool SaveValues { get; set; }
        public double ApertureValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
