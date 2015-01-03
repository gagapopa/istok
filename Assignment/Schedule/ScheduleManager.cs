using System;
using System.Collections.Generic;
using System.Data;
using NLog;

namespace COTES.ISTOK.Assignment
{
    class ScheduleManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private MyDBdata dbwork;
        public BlockProxy BlockProxy { get; set; }

        public ScheduleManager(MyDBdata dbwork)
        {
            this.dbwork = dbwork.Clone();
        }
        List<Schedule> lstSchedules;

        #region Загрузка структуры

        #region Работа с расписаниями в бд

        /// <summary>
        /// Загружает из бд список расписаний
        /// </summary>
        /// <returns>
        ///     истина если загрузка удалась,
        ///     ложь если провалилась
        /// </returns>
        public void LoadSchedules(OperationState state)
        {
            lstSchedules = new List<Schedule>();

            if (dbwork == null)
                throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                const string query = @"SELECT * FROM schedules;";
                using (var reader = dbwork.ExecSQL_toReader(transaction, query, null))
                {
                    while (reader.Read())
                    {
                        lstSchedules.Add(new Schedule(reader));
                    }
                }
                BlockProxy.FillParameterSchedules(lstSchedules);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
        }

        /// <summary>
        /// добавляет новое расписание в бд
        /// </summary>
        /// <param name="schedule">
        ///     расписание для добавления
        /// </param>
        /// <returns>
        ///     возвращает ид вновь созданного расписания
        /// </returns>
        private int AddScheduleToDb(Schedule schedule)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            if (schedule == null) throw new ArgumentNullException("schedule");
            if (schedule.Rule == null) throw new ArgumentException("Schedule rule is not set.");

            int transaction = 0;

            try
            {
                int id;
                const string query = @"INSERT INTO schedules OUTPUT INSERTED.id VALUES(@name, @rule);";

                DB_Parameters parametrs = new DB_Parameters();
                parametrs.Add(new DB_Parameter(@"@name",
                                               DbType.StringFixedLength,
                                               schedule.Name));
                parametrs.Add(new DB_Parameter(@"@rule",
                                               DbType.StringFixedLength,
                                               schedule.Rule.ToString()));
                transaction = dbwork.StartTransaction();
                using (var reader = dbwork.ExecSQL_toReader(transaction, query, parametrs))
                {
                    reader.Read();
                    id = reader.GetInt32(0);
                }
                dbwork.Commit(transaction);
                return id;
            }
            catch
            {
                dbwork.Rollback(transaction);
                throw;
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
        }

        /// <summary>
        /// удаляет расписание из бд
        /// </summary>
        /// <param name="id">
        ///     ид удаляемого расписания
        /// </param>
        private void DeleteScheduleFromDb(int id)
        {
            if (dbwork == null)
                throw new Exception("Отсутствует подключение к серверу БД");

            const string query = @"DELETE FROM schedules WHERE id = @id;";
            DB_Parameters parametrs = new DB_Parameters();
            parametrs.Add(new DB_Parameter(@"@id",
                                           DbType.Int32,
                                           id));

            dbwork.ExecSQL(query, parametrs);
        }

        /// <summary>
        /// обновляет расписание в бд
        /// </summary>
        /// <param name="schedule">
        ///     обновленное раписание
        /// </param>
        private void UpdateScheduleInDb(Schedule schedule)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            if (schedule == null) throw new ArgumentNullException("schedule");
            if (schedule.Rule == null) throw new ArgumentException("Schedule rule is not set.");

            const string query = @"UPDATE schedules SET name = @name, [rule] = @rule WHERE id = @id";

            DB_Parameters parametrs = new DB_Parameters();
            parametrs.Add(new DB_Parameter(@"@name",
                                           DbType.StringFixedLength,
                                           schedule.Name));
            parametrs.Add(new DB_Parameter(@"@rule",
                                           DbType.StringFixedLength,
                                           schedule.Rule.ToString()));
            parametrs.Add(new DB_Parameter(@"@id",
                                           DbType.Int32,
                                           schedule.Id));

            dbwork.ExecSQL(query, parametrs);
        }

        private void DeleteScheduleBindingsInDB(int id)
        {
            if (dbwork == null)
                throw new Exception("Отсутствует подключение к серверу БД");

            const string query = @"DELETE FROM props WHERE value = @id AND name = @name;";

            DB_Parameters parametrs = new DB_Parameters();
            parametrs.Add(new DB_Parameter(@"@name",
                                           DbType.StringFixedLength,
                                           @"schedule"));
            parametrs.Add(new DB_Parameter(@"@id",
                                           DbType.StringFixedLength,
                                           id.ToString()));

            dbwork.ExecSQL(query, parametrs);
        }

        #endregion

        /// <summary>
        /// очистить структуру
        /// </summary>
        private void Clear()
        {
            //if (!Monitor.TryEnter(types, 10000))
            //{
            //    log.Error("GNSI.Clear: Серверный список типов оборудования занят.");
            //    return;
            //}
            //try { types.Clear(); }
            //finally { Monitor.Exit(types); }

            //if (!Monitor.TryEnter(units, 10000))
            //{
            //    log.Error("GNSI.Clear: Серверный список оборудования занят.");
            //    return;
            //}
            //try
            //{
            //    units.Clear();
            //}
            //finally { Monitor.Exit(units); }
        }
        #endregion

        #region Рaбота с расписаниями

        /// <summary>
        /// Получает список расписаний.
        /// </summary>
        /// <returns>
        ///     Список расписаний.
        /// </returns>
        public Schedule[] GetParameterSchedules(OperationState state)
        {
            if (lstSchedules == null) return new Schedule[0];
            return lstSchedules.ToArray();
        }

        /// <summary>
        /// Возвращает расписание по иду
        /// </summary>
        /// <param name="id">
        ///     Ид расписания.
        /// </param>
        /// <returns>
        ///     расписание
        /// </returns>
        public Schedule GetUnloadParamsSchedule(OperationState state, int id)
        {
            //return schedules.Find((UnloadParamsSchedule it) =>
            //            it.ID == id);
            return lstSchedules.Find((Schedule it) => it.Id == id);
        }

        /// <summary>
        /// Возвращает расписание по названию
        /// </summary>
        /// <param name="name">Название расписания</param>
        /// <returns>Расписание</returns>
        public Schedule GetUnloadParamsSchedule(OperationState state, string name)
        {
            return lstSchedules.Find((Schedule it) => it.Name == name);
        }

        /// <summary>
        /// Добавляет расписание в список и в бд
        /// </summary>
        /// <param name="added">
        ///     Добавляемое расписание.
        /// </param>
        public void AddUnloadParamsSchedule(OperationState state, Schedule added)
        {
            //added.ID = AddScheduleToDb(added);

            //schedules.Add(added);
            foreach (var item in lstSchedules)
                if (item.Name.ToLower() == added.Name.ToLower())
                    throw new ISTOKException("Расписание с таким названием уже существует.");
            added.Id = AddScheduleToDb(added);
            lstSchedules.Add(added);
            BlockProxy.FillParameterSchedules(lstSchedules);
        }

        /// <summary>
        /// Удалет расписание из списка и бд
        /// </summary>
        /// <param name="id">
        ///     ид удаляемого расписания
        /// </param>
        public void RemoveUnloadParamsSchedule(OperationState state, int id)
        {
            RemoveScheduleBinding(state, id);

            DeleteScheduleFromDb(id);

            //schedules.RemoveAll((UnloadParamsSchedule it) =>
            //                        it.ID == id);
            lstSchedules.RemoveAll((Schedule item) => item.Id == id);
        }

        /// <summary>
        /// Обновляет расписание как в списке, так и в бд
        /// </summary>
        /// <param name="updated">
        ///     обновленное расписание
        /// </param>
        public void UpdateUnloadParamsSchedule(OperationState state, Schedule updated)
        {
            foreach (var item in lstSchedules)
                if (item.Name.ToLower() == updated.Name.ToLower() && item.Id != updated.Id)
                    throw new ISTOKException("Расписание с таким названием уже существует.");
            UpdateScheduleInDb(updated);

            //schedules.RemoveAll((UnloadParamsSchedule it) =>
            //                        it.ID == updated.ID);
            lstSchedules.RemoveAll((Schedule item) => item.Id == updated.Id);
            //schedules.Add(updated);
            lstSchedules.Add(updated);
            BlockProxy.FillParameterSchedules(lstSchedules);
        }

        /// <summary>
        /// удаляет привязку параметров к расписанию
        /// выполнять перед удалением расписания
        /// </summary>
        /// <param name="id">
        ///     ид расписания
        /// </param>
        public void RemoveScheduleBinding(OperationState state, int id)
        {
            DeleteScheduleBindingsInDB(id);

            //lock (dicUnits)
            //{
            //    foreach (var it in dicUnits.Values)
            //    {
            //        String stringValue = it.GetAttribute(@"schedule");
            //        if (!String.IsNullOrEmpty(stringValue) && Convert.ToInt32(stringValue) == id)
            //            it.Attributes.Remove(@"schedule");
            //        //var key_remove =
            //        //    it.Attributes.FirstOrDefault((KeyValuePair<string, string> current) =>
            //        //            current.Key == @"schedule" &&
            //        //            Convert.ToInt32(current.Value) == id
            //        //        );
            //        //if (key_remove.Key == @"schedule" && it.Attributes.ContainsKey(key_remove.Key))
            //        //    it.Attributes.Remove(key_remove.Key);
            //    }
            //}
        }

        #endregion
    }
}
