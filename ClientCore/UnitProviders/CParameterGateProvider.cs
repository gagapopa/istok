using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore.Utils;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class CParameterGateProvider : UnitProvider
    {
        public CParameterGateProvider(StructureProvider strucProvider, ParameterGateNode parameterGateNode)
            : base(strucProvider, parameterGateNode)
        {
            try { UseMimeTex = (bool)BaseSettings.Instance["UseMimeTex"]; }
            catch { UseMimeTex = ParameterGateUnitProvider.DefaultUseMimeTex; }

            originalValuesDictionary = new Dictionary<int, Dictionary<ArgumentsValues, ParamValueItem>>();
            valuesDictionary = new Dictionary<int, Dictionary<ArgumentsValues, ParamValueItem>>();
            manualArgumentValues = new Dictionary<IOptimizationArgument, Dictionary<ArgumentsValues, double>>();
        }

        bool providerStarted;

        bool isValuesOutOfDate;

        /// <summary>
        /// Начальная инициализация провайдера
        /// </summary>
        public void StartProvider()
        {
            if (CurrentTime == DateTime.MinValue)
            {
                CurrentTime = Interval.NearestEarlierTime(strucProvider.LastDateTime);
            }

            if (!providerStarted)
            {
                RetrieveParameterList();
                providerStarted = true;
            }
            else if (isValuesOutOfDate)
            {
                RetrievValues();
                isValuesOutOfDate = false;
            }

            OnParameterListChanged();
            OnCurrentTimeChanged();
            OnValuesChanged();
            OnUseMimeTexChanged();
            OnArgumentsChanged();
            //OnValuesEditModeChanged();
            if (LockValueAlways)
                LockValues();
            else OnValuesEditModeChanged();
        }

        private void OnUseMimeTexChanged()
        {
            if (UseMimeTexChanged != null)
                UseMimeTexChanged(this, EventArgs.Empty);
        }

        #region Работа с параметрами
        List<ParameterNode> parameters;

        /// <summary>
        /// Обновить список параметров
        /// </summary>
        private void RetrieveParameterList()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(s =>
            {
                if (parameters == null)
                    parameters = new List<ParameterNode>();
                parameters.Clear();

                int[] filterType = null;
                if (UnitNode.Typ == (int)UnitTypeId.ManualGate)
                    filterType = new int[] { (int)UnitTypeId.ManualParameter };
                else if (UnitNode.Typ == (int)UnitTypeId.TEPTemplate)
                    filterType = new int[] { (int)UnitTypeId.TEP };

                throw new NotImplementedException("RetrieveParameterList");
                //AsyncOperationWatcher<Object> watcher = rds.QueryUnitNodes(UnitNode.Idnum, filterType);
                //watcher.AddValueRecivedHandler(ParamReceive);
                //watcher.Run();
                //watcher.AddSuccessFinishHandler(() =>
                //{
                //    RetrievValues(CurrentTime);
                //    OnParameterListChanged();
                //});
                //watcher.Wait();
            });
        }

        private void ParamReceive(Object x)
        {
            UnitNode[] unitNodes = x as UnitNode[];
            ParameterNode paramNode;

            if (unitNodes != null)
            {
                foreach (UnitNode node in unitNodes)
                {
                    if ((paramNode = node as ParameterNode) != null)
                    {
                        if (!parameters.Contains(paramNode))
                            parameters.Add(paramNode);
                    }
                }
            }
        }

        private void OnParameterListChanged()
        {
            StartMimeTexGenerator();
            if (ParameterListChanged != null)
                ParameterListChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Работа со значениями
        /// <summary>
        /// Скорректированные значения параметров
        /// </summary>
        private Dictionary<int, Dictionary<ArgumentsValues, ParamValueItem>> valuesDictionary;

        /// <summary>
        /// Исходные значения параметров
        /// </summary>
        private Dictionary<int, Dictionary<ArgumentsValues, ParamValueItem>> originalValuesDictionary;

        /// <summary>
        /// Запросить значения
        /// </summary>
        private void RetrievValues()
        {
            if (!providerStarted)
                return;
            strucProvider.LastDateTime = CurrentTime;
            DateTime beginTime, endTime;

            //очистить valueParamDictionary
            Dictionary<ArgumentsValues, ParamValueItem> argsDictionary;
            List<ArgumentsValues> removeList = new List<ArgumentsValues>();

            ClearValues();
            foreach (int parameterId in valuesDictionary.Keys)
            {
                argsDictionary = valuesDictionary[parameterId];
                removeList.Clear();

                foreach (ArgumentsValues args in argsDictionary.Keys)
                    if (!Modified(parameterId, args))
                        removeList.Add(args);

                foreach (ArgumentsValues args in removeList)
                    argsDictionary.Remove(args);
            }
            lock (originalValuesDictionary)
                originalValuesDictionary.Clear();

            if (Interval != Interval.Zero)
            {
                beginTime = CurrentTime;
                endTime = Interval.GetNextTime(CurrentTime);
            }
            else endTime = beginTime = DateTime.Now;

            if (parameters != null)
            {
                throw new NotImplementedException("RetrievValues");
                //AsyncOperationWatcher<Object> watcher = rds.BeginGetArgumentedValues(parameters, beginTime, endTime);

                //watcher.AddValueRecivedHandler(ValueReceive);
                //watcher.AddFinishHandler(() =>
                //{
                //    try
                //    {
                //        RetrieveSortedArguments();
                //    }
                //    catch { }
                //    OnValuesChanged();
                //});
                //uniForm.RunWatcher(watcher);
            }
        }

        private void ValueReceive(Object x)
        {
            Package package = x as Package;
            Dictionary<ArgumentsValues, ParamValueItem> argsDictionary;

            if (package != null)
            {
                String[] optArgs = GetOptimizationArguments();
                lock (originalValuesDictionary)
                {
                    if (!originalValuesDictionary.TryGetValue(package.Id, out argsDictionary))
                        originalValuesDictionary[package.Id] = argsDictionary = new Dictionary<ArgumentsValues, ParamValueItem>();

                    //String[] optArgs = GetOptimizationArguments();
                    foreach (ParamValueItem valueItem in package.Values)
                    {
                        if (valueItem.Arguments != null
                               && valueItem.Arguments.CorrespondTo(optArgs)
                               && DateTime.Equals(CurrentTime, valueItem.Time))
                        {
                            argsDictionary[valueItem.Arguments] = valueItem;
                            break;
                        }
                    }
                }
            }
        }

        private void OnValuesChanged()
        {
            if (ValuesChanged != null)
                ValuesChanged(this, EventArgs.Empty);
        }

        private void OnArgumentsChanged()
        {
            lock (originalValuesDictionary)
            {
                arguments = null;
                filteredArguments = null;
                InitFilterArgument();
            }
            if (ArgumentsChanged != null)
                ArgumentsChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Работа с аргументами
        private List<ArgumentsValues> manualArguments;
        /// <summary>
        /// Полный список аргументов
        /// </summary>
        private List<ArgumentsValues> arguments;

        /// <summary>
        /// Отфильтрованный список аргументов
        /// </summary>
        private List<ArgumentsValues> filteredArguments;

        /// <summary>
        /// Отсортированные аргументы в порядке убывания оптимальности
        /// </summary>
        private ArgumentsValues[] sortedArgsList;

        /// <summary>
        /// Оптимальные аргументы для каждого базового аргумента
        /// </summary>
        private Dictionary<ArgumentsValues, ArgumentsValues> optimalArguments;

        private Dictionary<IOptimizationArgument, Dictionary<ArgumentsValues, double>> manualArgumentValues;

        /// <summary>
        /// Запросить отсортированные аргументы
        /// </summary>
        private void RetrieveSortedArguments()
        {
            throw new NotImplementedException("RetrieveSortedArguments");
            //AsyncOperationWatcher<Object> watcher = rds.BeginGetSortedArgs(UnitNode, CurrentTime);

            //watcher.AddValueRecivedHandler(SortedArgumentsReceive);
            //watcher.AddFinishHandler(OnArgumentsSortChanged);
            //uniForm.RunWatcher(watcher);
        }

        private void SortedArgumentsReceive(Object x)
        {
            ArgumentsValues[] args = x as ArgumentsValues[];
            sortedArgsList = new ArgumentsValues[args.Length];

            for (int i = 0; i < sortedArgsList.Length; i++)
            {
                sortedArgsList[i] = args[i];
            }
        }

        private void OnArgumentsSortChanged()
        {
            optimalArguments = null;
            OnArgumentsChanged();
        }
        #endregion

        #region Работа с изображениями кодов параметров
        /// <summary>
        /// Отображать изображения кодов параметров
        /// </summary>
        private bool useMimeTex;

        /// <summary>
        /// Изоюражения кодов параметров
        /// </summary>
        private Dictionary<ParameterNode, System.Drawing.Image> parameterCodeImageDictionary = new Dictionary<ParameterNode, System.Drawing.Image>();

        /// <summary>
        /// Получить изображение кода параметра
        /// </summary>
        /// <param name="parameterNode">Параметр</param>
        /// <returns></returns>
        public System.Drawing.Image GetParameterCodeImage(ParameterNode parameterNode)
        {
            System.Drawing.Image codeImage;

            if (!parameterCodeImageDictionary.TryGetValue(parameterNode, out codeImage))
                throw new NotImplementedException("GetParameterCodeImage");
            //codeImage = Program.MainForm.Icons.Images[((int)parameterNode.Typ).ToString()];

            return codeImage;
        }

        public System.Drawing.Image GetParameterCodeImage(IOptimizationArgument optimizationArgument)
        {
            return Properties.Resources.edit_add;
        }

        /// <summary>
        /// Каллбэк принимающий изображение кода параметра из генератора
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="codeImage">Новое изображение</param>
        private void CodeImageChanged(ParameterNode parameter, System.Drawing.Image codeImage)
        {
            parameterCodeImageDictionary[parameter] = codeImage;
            if (codeImage.Height > RowHeight) RowHeight = codeImage.Height;
            if (codeImage.Width > CodeColumnWidth) CodeColumnWidth = codeImage.Width;
            OnParameterCodeImageChanged();
        }

        private void OnParameterCodeImageChanged()
        {
            if (ParameterCodeImageChanged != null)
                ParameterCodeImageChanged(this, EventArgs.Empty);
        }

        private void StartMimeTexGenerator()
        {
            if (UseMimeTex && ParameterList != null)
                foreach (ParameterNode paramNode in ParameterList)
                    MimeTexGenerator.Singlton.GetImage(paramNode, CodeImageChanged);
        }
        #endregion

        /// <summary>
        /// Отображать изображения кодов параметров
        /// </summary>
        public bool UseMimeTex
        {
            get { return useMimeTex; }
            set
            {
                if (value != useMimeTex)
                {
                    useMimeTex = value;
                    StartMimeTexGenerator();
                    OnUseMimeTexChanged();
                }
            }
        }

        /// <summary>
        /// Максимальная высота среди всех изображений кодов параметров
        /// </summary>
        public int RowHeight { get; protected set; }

        /// <summary>
        /// Максимальная ширина среди всех изображений кодов параметров
        /// </summary>
        public int CodeColumnWidth { get; protected set; }

        /// <summary>
        /// Интервал отображаемого шаблона
        /// </summary>
        public Interval Interval { get { return ((ParameterGateNode)UnitNode).Interval; } }

        /// <summary>
        /// Верхний узел оптимизации, передаваемый в окно оптимизации
        /// </summary>
        public OptimizationGateNode TopOptimizationNode
        {
            get
            {
                UnitNode tempUnitNode = unitNode;
                OptimizationGateNode optimNode = null;

                while (tempUnitNode != null)
                {
                    if (tempUnitNode.Typ == (int)UnitTypeId.OptimizeCalc)
                        optimNode = tempUnitNode as OptimizationGateNode;

                    tempUnitNode = RDS.NodeDataService.GetUnitNode(tempUnitNode.ParentId);
                }
                return optimNode;
            }
        }

        public OptimizationGateNode OptimizationNode
        {
            get
            {
                UnitNode tempUnitNode = unitNode;
                OptimizationGateNode optimNode = null;

                while (tempUnitNode != null
                    && (optimNode = tempUnitNode as OptimizationGateNode) == null)
                    tempUnitNode = RDS.NodeDataService.GetUnitNode(tempUnitNode.ParentId);

                return optimNode;
            }
        }

        private String[] optimizationArguments;
        public String[] OptimizationArguments
        {
            get
            {
                //if (optimizationArguments==null)
                //{
                //    optimizationArguments = GetAllArguments(OptimizationNode, CurrentTime);
                //}
                return optimizationArguments;
            }
        }

        public string[] GetOptimizationArguments()
        {
            if (optimizationArguments == null)
            {
                optimizationArguments = GetAllArguments(OptimizationNode, CurrentTime);
            }
            return optimizationArguments;
        }

        private string[] GetAllArguments(OptimizationGateNode optimizationNode, DateTime CurrentTime)
        {
            List<String> arguments = new List<string>();

            if (optimizationNode.ParentId != null)
            {
                var parentNode = RDS.NodeDataService.GetUnitNode(optimizationNode.ParentId);
                UnitNode un = RDS.NodeDataService.GetParent(parentNode, (int)UnitTypeId.OptimizeCalc);
                OptimizationGateNode baseOptimization = un as OptimizationGateNode;

                if (baseOptimization != null)
                    arguments.AddRange(GetAllArguments(baseOptimization, CurrentTime));
            }
            arguments.AddRange(from a in optimizationNode.ArgsValues select a.Name);

            return arguments.ToArray();
        }

        /// <summary>
        /// Список параметров
        /// </summary>
        public ParameterNode[] ParameterList
        {
            get
            {
                if (parameters != null)
                    return parameters.ToArray();

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime currentTime;

        /// <summary>
        /// Текущие время
        /// </summary>
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set
            {
                RetrievValues(value);
                OnCurrentTimeChanged();
            }
        }

        private void RetrievValues(DateTime value)
        {
            bool locked = ValuesEditMode;
            ReleaseValues();
            currentTime = Interval.NearestEarlierTime(/*StartTime,*/ value);
            RetrievValues();
            if (locked || LockValueAlways)
                LockValues();
        }

        public void NextTime()
        {
            CurrentTime = Interval.GetNextTime(CurrentTime);
        }

        public void PrevTime()
        {
            CurrentTime = Interval.GetPrevTime(CurrentTime);
        }

        private void OnCurrentTimeChanged()
        {
            if (CurrentTimeChanged != null)
                CurrentTimeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Все аргументы
        /// </summary>
        public ArgumentsValues[] Arguments
        {
            get
            {
                lock (originalValuesDictionary)
                {
                    if (arguments == null)
                        arguments = GetArgumentsList(false);

                    return arguments.ToArray();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        private List<ArgumentsValues> GetArgumentsList(bool all)
        {
            List<ArgumentsValues> argumentsList = new List<ArgumentsValues>();

            foreach (int parameterID in originalValuesDictionary.Keys)
            {
                List<ArgumentsValues> argList = new List<ArgumentsValues>(originalValuesDictionary[parameterID].Keys);
                foreach (ArgumentsValues args in argList)
                    if (originalValuesDictionary[parameterID][args].Quality != Quality.Bad
                        && (all || !deleteArgumentList.Contains(args)) && !argumentsList.Contains(args))
                        argumentsList.Add(args);
            }
            if (manualArguments != null)
                argumentsList.AddRange(manualArguments/*.FindAll(a => a.Current != null)*/);
            return argumentsList;
        }

        /// <summary>
        /// Отфильтрованные аргументы
        /// </summary>
        public ArgumentsValues[] FilteredArguments
        {
            get
            {
                lock (originalValuesDictionary)
                {
                    if (filteredArguments == null)
                    {
                        filteredArguments = new List<ArgumentsValues>();

                        foreach (ArgumentsValues args in Arguments)
                            if (FilterArgument(args))
                                filteredArguments.Add(args);
                    }
                    return filteredArguments.ToArray();
                }
            }
        }

        /// <summary>
        /// Отсортированные аргументы
        /// </summary>
        public ArgumentsValues[] SortedArguments
        {
            get
            {
                if (sortedArgsList != null)
                    return sortedArgsList.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Оптимальные аргументы
        /// </summary>
        public Dictionary<ArgumentsValues, ArgumentsValues> OptimalArguments
        {
            get
            {
                if (optimalArguments == null && SortedArguments != null)
                {
                    ArgumentsValues baseArgs;
                    List<ArgumentsValues> baseArgsList = new List<ArgumentsValues>();

                    var nodeArgs = from a in OptimizationNode.ArgsValues select a.Name;


                    foreach (ArgumentsValues args in SortedArguments)
                    {
                        if (args != null)
                        {
                            baseArgs = args.Exclude(nodeArgs);

                            if (baseArgs == null)
                                baseArgs = new ArgumentsValues();

                            if (!baseArgsList.Contains(baseArgs))
                                baseArgsList.Add(baseArgs);
                        }
                    }

                    optimalArguments = new Dictionary<ArgumentsValues, ArgumentsValues>();
                    foreach (ArgumentsValues args in baseArgsList)
                        for (int i = 0; i < SortedArguments.Length; i++)
                            if (SortedArguments[i].Include(args))
                            {
                                optimalArguments[args] = SortedArguments[i];
                                break;
                            }
                }
                return optimalArguments;
            }
        }

        /// <summary>
        /// Фильтр аргументов
        /// </summary>
        private Dictionary<ArgumentsValues, bool> filterArguments;

        /// <summary>
        /// Изменить фильтр
        /// </summary>
        /// <param name="baseArgs">Аргумент</param>
        /// <param name="check">Значения</param>
        public void FilterArgument(ArgumentsValues baseArgs, bool check)
        {
            // инициализация аргументов фильтрации
            if (filterArguments == null)
                InitFilterArgument();

            foreach (var args in filterArguments.Keys.ToArray())
            {
                if (args.Include(baseArgs))
                    filterArguments[args] = check;
            }

            OnFilterChanged();
        }

        private void InitFilterArgument()
        {
            if (filterArguments == null)
            {
                filterArguments = new Dictionary<ArgumentsValues, bool>();
            }
            else
                filterArguments.Clear();

            string[] args = (from a in OptimizationNode.ArgsValues select a.Name).ToArray();

            foreach (var valueArgs in Arguments)
            {
                var levelArgs = valueArgs.Exclude(args);
                filterArguments[levelArgs] = true;
            }
        }

        /// <summary>
        /// Проверить фильтр
        /// </summary>
        /// <param name="args">Аргумент</param>
        /// <returns></returns>
        public bool FilterArgument(ArgumentsValues args)
        {
            if (filterArguments == null)
                return true;

            foreach (var levelArgs in filterArguments.Keys)
            {
                if (args.Include(levelArgs))
                    return filterArguments[levelArgs];
            }

            return true;
        }
        public bool FilterLevelArgument(ArgumentsValues args)
        {
            if (filterArguments == null)
                return true;

            bool ret = false;

            foreach (var levelArgs in filterArguments.Keys)
            {
                if (levelArgs.Include(args))
                {
                    ret |= filterArguments[levelArgs];
                    if (ret)
                        break;
                }
            }

            return ret;
        }

        private void OnFilterChanged()
        {
            lock (originalValuesDictionary)
            {
                filteredArguments = null;
                if (FilterChanged != null)
                    FilterChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Получить значение параметра
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="args">Значения аргументов</param>
        /// <returns>Значение параметра</returns>
        public ParamValueItem GetValue(ParameterNode parameter, ArgumentsValues args)
        {
            Dictionary<ArgumentsValues, ParamValueItem> argsDictionary;
            ParamValueItem retItem;

            if ((valuesDictionary.TryGetValue(parameter.Idnum, out argsDictionary)
                && argsDictionary.TryGetValue(args, out retItem)))
                return retItem;
            if ((originalValuesDictionary.TryGetValue(parameter.Idnum, out argsDictionary)
                && argsDictionary.TryGetValue(args, out retItem)))
                return retItem.Clone() as ParamValueItem;
            return null;
        }

        /// <summary>
        /// Изменить значения параметра
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="args">Значения аргументов</param>
        /// <param name="value">Значение парамтера</param>
        public void SetValue(ParameterNode parameter, ArgumentsValues args, ParamValueItem value)
        {
            Dictionary<ArgumentsValues, ParamValueItem> argsDictionary;

            if (!valuesDictionary.TryGetValue(parameter.Idnum, out argsDictionary))
                valuesDictionary[parameter.Idnum] = argsDictionary = new Dictionary<ArgumentsValues, ParamValueItem>();
            argsDictionary[args] = value;
        }

        public double GetArgumentValue(IOptimizationArgument optimizationArgument, ArgumentsValues pair)
        {
            double value;
            Dictionary<ArgumentsValues, double> argumentDictionary;

            if (!manualArgumentValues.TryGetValue(optimizationArgument, out argumentDictionary)
                || !argumentDictionary.TryGetValue(pair, out value))
            {
                value = pair[optimizationArgument.Name];
            }
            return value;
        }

        public void SetArgumentValue(IOptimizationArgument optimizationArgument, ArgumentsValues pair, double value)
        {
            Dictionary<ArgumentsValues, double> argumentDictionary;

            if (!manualArgumentValues.TryGetValue(optimizationArgument, out argumentDictionary))
            {
                manualArgumentValues[optimizationArgument] = argumentDictionary = new Dictionary<ArgumentsValues, double>();
            }

            argumentDictionary[pair] = value;
        }

        /// <summary>
        /// Изменилось ли значение параметра
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="args">Значения аргументов</param>
        /// <returns></returns>
        public bool Modified(ParameterNode parameter, ArgumentsValues args)
        {
            return Modified(parameter.Idnum, args);
        }

        /// <summary>
        /// Изменилось ли значение параметра
        /// </summary>
        /// <param name="parameterID">ИД параметра</param>
        /// <param name="args">Значения аргументов</param>
        /// <returns></returns>
        private bool Modified(int parameterID, ArgumentsValues args)
        {
            Dictionary<ArgumentsValues, ParamValueItem> argsDictionary;
            ParamValueItem valueItem = null, originalValueItem = null;

            bool original, current;
            original = originalValuesDictionary.TryGetValue(parameterID, out argsDictionary)
                && argsDictionary.TryGetValue(args, out originalValueItem);

            current = valuesDictionary.TryGetValue(parameterID, out argsDictionary)
                    && argsDictionary.TryGetValue(args, out valueItem);

            if (original && current)
                return !originalValueItem.Equals(valueItem);
            if (current && !original)
                return true;
            return false;
        }

        public bool Modified(IOptimizationArgument optimizationArgument, ArgumentsValues pair)
        {
            if (pair != null)
            {
                double value = GetArgumentValue(optimizationArgument, pair);

                return !double.Equals(value, pair[optimizationArgument.Name]);
            }

            return false;
        }

        #region События
        /// <summary>
        /// Изменился список параметров
        /// </summary>
        public event EventHandler ParameterListChanged;

        /// <summary>
        /// Изменилось текущие время
        /// </summary>
        public event EventHandler CurrentTimeChanged;

        /// <summary>
        /// Изменился список аргументов
        /// </summary>
        public event EventHandler ArgumentsChanged;

        /// <summary>
        /// Изменился фильтр аргументов
        /// </summary>
        public event EventHandler FilterChanged;

        /// <summary>
        /// Изменились значения параметров
        /// </summary>
        public event EventHandler ValuesChanged;

        /// <summary>
        /// Изменилось свойство UseMimeTex
        /// </summary>
        public event EventHandler UseMimeTexChanged;

        /// <summary>
        /// Изменились измображения кодов параметров
        /// </summary>
        public event EventHandler ParameterCodeImageChanged;
        #endregion

        public override bool HasChanges
        {
            get
            {
                foreach (var item in GetArgumentsList(true))
                    foreach (var arg in OptimizationNode.ArgsValues)
                        if (Modified(arg, item))
                            return true;

                if (ParameterList != null)
                    foreach (ParameterNode parameterNode in ParameterList)
                        foreach (var args in Arguments)
                            if (Modified(parameterNode, args))
                                return true;

                return false;
            }
        }

        List<ArgumentsValues> deleteArgumentList = new List<ArgumentsValues>();

        public void SaveValues()
        {
            Dictionary<ArgumentsValues, ArgumentsValues> changeArguments = new Dictionary<ArgumentsValues, ArgumentsValues>();

            // изменение аргументов
            foreach (var optimizationArg in manualArgumentValues.Keys)
            {
                foreach (var originalArg in manualArgumentValues[optimizationArg].Keys)
                {
                    ArgumentsValues args;
                    if (!changeArguments.TryGetValue(originalArg, out args))
                        changeArguments[originalArg] = args = new ArgumentsValues(originalArg);

                    args[optimizationArg.Name] = manualArgumentValues[optimizationArg][originalArg];
                }
            }

            AsyncOperationWatcher watcher = null;
            if (changeArguments.Count > 0)
            {
                //watcher = rds.BeginChangeArguments(TopOptimizationNode, CurrentTime, changeArguments.ToArray());
                throw new NotImplementedException();
            }
            // удаление значений
            if (deleteArgumentList.Count > 0)
            {
                //watcher = rds.BeginDeleteValues(TopOptimizationNode, deleteArgumentList.ToArray(), CurrentTime);
                throw new NotImplementedException();
                RDS.ValuesDataService.DeleteValuesOptimization(TopOptimizationNode.Idnum, deleteArgumentList.ToArray(), CurrentTime);
            }

            // сохранение значений
            List<Package> packageList = new List<Package>();
            Package package;
            foreach (int parameterID in valuesDictionary.Keys)
            {
                package = new Package();
                package.Id = parameterID;

                foreach (ArgumentsValues args in valuesDictionary[parameterID].Keys)
                {
                    ParamValueItem valueItem = valuesDictionary[parameterID][args];

                    // корректируем значение аргументов, если требуется
                    if (changeArguments.ContainsKey(args))
                    {
                        CorrectedParamValueItem correctedValue;

                        if ((correctedValue = valueItem as CorrectedParamValueItem) == null)
                            valueItem = correctedValue = new CorrectedParamValueItem(valueItem);

                        correctedValue.Arguments = changeArguments[args];
                    }

                    package.Add(valueItem);
                }
                packageList.Add(package);
            }
            if (watcher != null)
            {
                watcher.AddFinishHandler(() =>
                {
                    throw new NotImplementedException();
                    //watcher = rds.BeginSaveValues(packageList.ToArray());
                    //watcher.AddFinishHandler(() => RetrievValues());
                    //uniForm.RunWatcher(watcher);
                });
            }
            else
            {
                throw new NotImplementedException();
                //watcher = rds.BeginSaveValues(packageList.ToArray());
                //watcher.AddFinishHandler(() => RetrievValues());
            }

            //uniForm.RunWatcher(watcher);
        }

        private bool valuesEditMode;
        public bool ValuesEditMode
        {
            get { return valuesEditMode; }
            protected set
            {
                valuesEditMode = value;
                OnValuesEditModeChanged();
            }
        }

        public event EventHandler ValuesEditModeChanged;

        protected void OnValuesEditModeChanged()
        {
            if (ValuesEditModeChanged != null)
                ValuesEditModeChanged(this, EventArgs.Empty);
        }

        public bool LockValueAlways { get { return true && UnitNode.Typ == (int)UnitTypeId.ManualGate; } }

        public void LockValues()
        {
            try
            {
                DateTime startTime, endTime;
                if (Interval == Interval.Zero)
                {
                    startTime = DateTime.MinValue;
                    endTime = DateTime.MaxValue;
                }
                else
                {
                    startTime = CurrentTime;
                    endTime = Interval.GetNextTime(startTime);
                }
                RDS.ValuesDataService.LockValues(UnitNode, startTime, endTime);
                lockedParameters = null;
                ValuesEditMode = true;
            }
            catch (LockException exc)
            {
                lockedParameters = (from c in exc.Causes select c.Node).ToArray();
                throw;
            }
        }

        public virtual void ClearUnsavedValues()
        {
            ClearValues();
            OnValuesEditModeChanged();
        }

        private void ReleaseValues()
        {
            DateTime startTime, endTime;
            if (Interval == Interval.Zero)
            {
                startTime = DateTime.MinValue;
                endTime = DateTime.MaxValue;
            }
            else
            {
                startTime = CurrentTime;
                endTime = Interval.GetNextTime(startTime);
            }
            RDS.ValuesDataService.ReleaseValues(UnitNode, startTime, endTime);
            ValuesEditMode = false;
        }

        private void ClearValues()
        {
            valuesDictionary.Clear();
            if (manualArguments != null)
                manualArguments.Clear();

            manualArgumentValues.Clear();
            deleteArgumentList.Clear();

            //hack костыльный костыль
            if (UnitNode.Typ == (int)UnitTypeId.ManualGate)
                OnArgumentsChanged();
        }

        private UnitNode[] lockedParameters;

        public bool IsLocked(ParameterNode parameter)
        {
            return lockedParameters != null && lockedParameters.Contains(parameter/*.Idnum*/);
        }

        public override IEnumerable<ActionArgs> Save()
        {
            SaveValues();
            return base.Save();
        }

        public override void ClearUnsavedData()
        {
            ClearUnsavedValues();
            ReleaseValues();
            base.ClearUnsavedData();
        }

        public void CalcForm_CalcFinished(Object sender, EventArgs e)
        {
            //throw new NotImplementedException("CalcForm_CalcFinished");
            // сбрасываем кэш внутри оптимизации
            //UniForm uniForm = UniForm as UniForm;

            var nodes = RDS.NodeDataService.GetAllUnitNodes(TopOptimizationNode.Idnum, new int[] { (int)UnitTypeId.TEPTemplate });
            if (nodes != null)
                foreach (var item in nodes)
                {
                    CParameterGateProvider provider = StructureProvider.GetUnitProvider(item) as CParameterGateProvider;
                    if (provider != null)
                        provider.isValuesOutOfDate = true;
                }

            //Обновляем значения на текущей вкладке
            RetrievValues();
        }

        public void AddColumn(ArgumentsValues valueArguments)
        {
            if (manualArguments == null)
                manualArguments = new List<ArgumentsValues>();

            ArgumentsValues args = new ArgumentsValues(valueArguments);

            double columnNum = double.NaN;
            double argVal;

            foreach (var item in OptimizationNode.ArgsValues)
            {
                if (item.Mode == OptimizationArgumentMode.ColumnNum)
                {
                    if (double.IsNaN(columnNum))
                    {
                        columnNum = 1;
                        List<double> objectList = GetArgumentsList(true).ConvertAll<double>(
                            p => p[item.Name]);
                        while (objectList.Contains(columnNum))
                            ++columnNum;
                    }
                    argVal = columnNum;
                }
                else
                    argVal = double.NaN;

                args[item.Name] = argVal;
            }

            manualArguments.Add(args);

            OnArgumentsChanged();
        }

        public void DeleteColumn(ArgumentsValues pair)
        {
            if (pair != null)
            {
                if (manualArguments != null
                    && manualArguments.Contains(pair))
                    manualArguments.Remove(pair);
                else
                    deleteArgumentList.Add(pair);

                foreach (var item in manualArgumentValues.Keys)
                {
                    if (manualArgumentValues[item].ContainsKey(pair))
                        manualArgumentValues[item].Remove(pair);
                }

                OnArgumentsChanged();
            }
        }
    }
}
