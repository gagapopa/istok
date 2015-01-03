using COTES.ISTOK.Block.Mirroring;
using System;
using System.Data;
using System.Data.SqlClient;

namespace COTES.ISTOK.Block.MirroringManager
{
    /// <summary>
    /// Класс для управления и создания зеркалирования.
    /// </summary>
    public class Mirroring
    {
        /// <summary>
        ///     Функция, создает соединение к серверу баз данных, 
        ///     к базе данных мастер.
        /// </summary>
        /// <param name="server">
        ///     Имя сервера.
        ///     <example>
        ///         Serv1\SqlServer1
        ///     </example>
        /// </param>
        /// <param name="password">
        ///     Пароль.
        /// </param>
        /// <param name="user">
        ///     Имя пользователя.
        ///     Пользователь должен обладать административными правами.
        /// </param>
        /// <returns>
        ///     Новое соедниение к указанному серверу, к базе данных
        ///     @"master".
        /// </returns>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static SqlConnection CreateConnection(string server,
                                                     string password,
                                                     string user)
        {
            try
            {
                const string master_data_base = @"master";

                SqlConnectionStringBuilder builder =
                    new SqlConnectionStringBuilder();

                builder.Clear();

                builder.DataSource = server;
                builder.InitialCatalog = master_data_base;
                builder.Password = password;
                builder.UserID = user;

                SqlConnection result = 
                    new SqlConnection(builder.ToString());
                result.Open();

                return result;
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /// <summary>
        ///     Функция, проверяет установлено ли для
        ///     данной базы данных зеркалирование.
        /// </summary>
        /// <param name="server">
        ///     Соединение с сервером баз данных.
        ///     Соединение долно быть осуществлено к
        ///     системной базе данных мастер от
        ///     пользователя с правами администратора.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных.
        /// </param>
        /// <returns>
        ///     В случае если зеркалирование установлено
        ///     возвращает истину, иначе лож.
        /// </returns>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static bool MirroringInstalled(SqlConnection server,
                                              string data_base_name)
        { return GetMirroringState(server, data_base_name) != null; }

        /// <summary>
        /// pokopk
        /// </summary>
        /// <param name="server"></param>
        /// <param name="data_base_name"></param>
        /// <returns></returns>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static bool IsPrincipal(SqlConnection server,
                                       string data_base_name)
        {
            const string strPrincipal = "PRINCIPAL";
            const int role_index = 1;

            string[] state = GetMirroringState(server, data_base_name);

            return state != null && state[role_index] == strPrincipal;
        }

        /// <summary>
        ///     Функция, проверяет является ли
        ///     текущий сервер для текущей базы данных
        ///     принципалом(основным сервером).
        /// </summary>
        /// <param name="server">
        ///     Соединение с сервером баз данных.
        ///     Соединение долно быть осуществлено к
        ///     системной базе данных мастер от
        ///     пользователя с правами администратора.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных.
        /// </param>
        /// <returns>
        ///     В случае если текущий сервер принципал(основной)
        ///     возвращает истину, иначе лож.
        /// </returns>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static bool ServerIsPrincipal(SqlConnection server,
                                             string data_base_name)
        {
            const int role_index = 1;
            const string principal_role = @"PRINCIPAL";

            string[] states = GetMirroringState(server, data_base_name); 

            return states != null && states[role_index] == principal_role;
        }

        /// <summary>
        ///     Функция, проверяет активно ли зеркалирование.
        /// </summary>
        /// <param name="server">
        ///     Соединение с сервером баз данных.
        ///     Соединение долно быть осуществлено к
        ///     системной базе данных мастер от
        ///     пользователя с правами администратора.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных.
        /// </param>
        /// <returns>
        ///     В случае если зеркалирование активно
        ///     возвращает истину, иначе ложь.
        /// </returns>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static bool MirrorringIsActive(SqlConnection server,
                                              string data_base_name)
        {
            const int state_index = 0;
            const string suspend_state = @"SUSPENDED";

            string[] states = GetMirroringState(server, data_base_name);

            return states != null && states[state_index] != suspend_state;
        }

        /// <summary>
        ///     Функция, приостанавливает зеркалирование.
        /// </summary>
        /// <param name="principal">
        ///     Соединение с основным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных для которой 
        ///     приостанавливается зеркалирование.
        /// </param>
        /// <remarks>
        ///     В случае, если зеркалирование уже
        ///     приостановлено, не активно, то
        ///     вызов будет проигнорирован.
        /// </remarks>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static void Suspend(SqlConnection principal,
                                   string data_base_name)
        {
            SetState(false, principal, data_base_name);
        }

        /// <summary>
        ///     Функция, возобновляет зеркалирование.
        /// </summary>
        /// <param name="principal">
        ///     Соединение с основным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных для которой
        ///     возобновляется зеркалирование.
        /// </param>
        /// <remarks>
        ///     В случае, если зеркалирование работает,
        ///     активно, то вызов игнорируется.
        /// </remarks>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static void Resume(SqlConnection principal,
                                  string data_base_name)
        {
            SetState(true, principal, data_base_name);
        }

        /// <summary>
        ///     Функция, переключает роли в
        ///     обычном, рабочем режиме.
        /// </summary>
        /// <param name="principal">
        ///     Соединение с основным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных для которой
        ///     происходит переключение ролей 
        ///     зеркалирования.
        /// </param>
        /// <remarks>
        ///     Данный тип переключения работает только
        ///     в случае, если оба сервера видят друг друга,
        ///     то есть находятся в рабочем состоянии.
        ///     Переключение происходит спустя некоторое время,
        ///     в течении которого происходит ожидание
        ///     завершения всех текуших транзакций.
        /// </remarks>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static void Switch(SqlConnection principal, 
                                  string data_base_name)
        {
            Switch(false, principal, data_base_name);
        }

        /// <summary>
        ///     Статическая функция, переключает роли 
        ///     зеркалирования в форсированном режиме.
        /// </summary>
        /// <param name="mirror">
        ///     Соединение с зеркальным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных для которой 
        ///     происходит перключение ролей зеркалирования.
        /// </param>
        /// <remarks>
        ///     Данный тип переключения работает только в случае,
        ///     если основной и зеркальный сервера не видят 
        ///     друг друга. При этом переключение происходит сразу
        ///     и возможны потери данных.
        ///     После восстановления связи, процесс зеркалирования 
        ///     будет приостановлен. Его следует возобновить.
        ///     После возобновления зеркальный станет основным 
        ///     и наоборот.
        /// </remarks>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static void ForceSwitch(SqlConnection mirror,
                                       string data_base_name)
        {
            Switch(true, mirror, data_base_name);
        }

        /// <summary>
        ///     Функция, устанавливает зеркалирование.
        ///     При установке используются созданные или
        ///     переданные в конструкторе соединения к 
        ///     серверам баз данных.
        /// </summary>
        /// <param name="principal">
        ///     Соединение с основным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="mirror">
        ///     Соединение с зеркальным сервером. Соединение
        ///     должно быть произведено от пользователя с
        ///     правами администратора к базе данных мастер.
        ///     Рекомендуется использоватье функцию CreateConnection.
        /// </param>
        /// <param name="data_base_name">
        ///     Имя базы данных для которой устанавливается
        ///     зеркалирование.
        /// </param>
        /// <param name="backup_path">
        ///     Путь для создания бэкапа основного сервера 
        ///     и восстановления его на зеркале. Предполагается
        ///     использовать некоторую сетевую папку или фтп сервер
        ///     доступный обоим серверам.
        ///     <example>
        ///         \\\Serv1\BackupFolder\
        ///     </example>
        /// </param>
        /// <param name="tcp_listened_port">
        ///     Потр для просулшивания. Через данный порт
        ///     будет происходит коммуникация между серверами по
        ///     протоколу тисипи. Порт должен быть открыт и не занят
        ///     другим приложением.
        /// </param>
        /// <param name="principal_address">
        ///     Адрес основного сервера. Допускается ипользование
        ///     доменного имени или ип адреса. Предпочтительней 
        ///     использовать ип адрес.
        ///     <example>
        ///         192.168.201.145
        ///         или
        ///         serv1.cotes.ru
        ///     </example>
        /// </param>
        /// <param name="mirror_address">
        ///     Адрес зеркального сервера. Допускается ипользование
        ///     доменного имени или ип адреса. Предпочтительней 
        ///     использовать ип адрес.
        ///     <example>
        ///         192.168.201.146
        ///         или
        ///         serv2.cotes.ru
        ///     </example>
        /// </param>
        /// <exception cref="MirroringManager.MirroringException">
        /// </exception>
        public static void Install(SqlConnection principal,
                                   SqlConnection mirror,
                                   string data_base_name,
                                   string backup_path,
                                   uint tcp_listened_port,
                                   string principal_address,
                                   string mirror_address)
        {
            string backup_file =
                BackupDataBaseFromPrincipal(principal,
                                            data_base_name,
                                            backup_path);

            RestoreDataBaseToMirror(mirror,
                                    data_base_name,
                                    backup_file);

            backup_file = 
                BackupTransactionLogFromPrincipal(principal,
                                                  data_base_name,
                                                  backup_path);

            RestoreTransactionLogToMirror(mirror,
                                          data_base_name,
                                          backup_file);

            CreateEndpoints(principal, 
                            mirror, 
                            tcp_listened_port);

            SetPartners(principal, 
                        mirror,
                        data_base_name, 
                        tcp_listened_port, 
                        principal_address, 
                        mirror_address);
        }

        /// <summary>
        /// Ожидание состояния зеркалируемой базы отличного от "DISCONNECTED"
        /// </summary>
        /// <param name="connection">Соединение с сервером</param>
        /// <param name="data_base_name">Название базы данных</param>
        public static void WaitNormalState(SqlConnection connection,
                                            string data_base_name)
        {
            WaitNormalState(connection, data_base_name, 30000);
        }
        /// <summary>
        /// Ожидание состояния зеркалируемой базы отличного от "DISCONNECTED"
        /// </summary>
        /// <param name="connection">Соединение с сервером</param>
        /// <param name="data_base_name">Название базы данных</param>
        /// <param name="timeout">Таймаут в мс</param>
        public static void WaitNormalState(SqlConnection connection,
                                           string data_base_name,
                                           int timeout)
        {
            string[] state;
            bool ret = false;
            int sleeptime = 500, maxtime = timeout;

            while (!ret)
            {
                if (maxtime < sleeptime) sleeptime = maxtime;
                state = GetMirroringState(connection, data_base_name);

                if (state != null && (state[0] == "DISCONNECTED" || state[0] == "SUSPENDED"))
                {
                    System.Threading.Thread.Sleep(sleeptime);
                    maxtime -= sleeptime;
                    if (maxtime <= 0) throw new MirroringException("Mirroring state timeout");
                }
                else
                    ret = true;
            }
        }

        public static void WaitMirroring(SqlConnection connection,
                                         string data_base_name)
        {
            WaitMirroring(connection, data_base_name, 30000);
        }
        public static void WaitMirroring(SqlConnection connection,
                                         string data_base_name,
                                         int timeout)
        {
            string[] state;
            bool ret = false;
            int sleeptime = 500, maxtime = timeout;

            while (!ret)
            {
                if (maxtime < sleeptime) sleeptime = maxtime;
                state = GetMirroringState(connection, data_base_name);

                if (state == null)
                {
                    System.Threading.Thread.Sleep(sleeptime);
                    maxtime -= sleeptime;
                    if (maxtime <= 0) throw new MirroringException("Mirroring state timeout");
                }
                else
                    ret = true;
            }
        }

        #region Реализация

        /*
         * Функция, возвращает массив строк, идентифицирующих
         * состояние зеркалирования для текущей базы данных.
         *      идекс 0 - состояние(синхронное, асинхронное, остановленное)
         *      идекс 1 - роль(принципал, миррор, витнесс)
         * В случае, если для текущей базы не установлено зеркалирование,
         * возвращает нул.
         */
        private static string[] GetMirroringState(SqlConnection server,
                                                  string data_base_name)
        {
            try
            {
                const int required_filed_count = 2;

                SqlCommand command = server.CreateCommand();

                command.CommandText = String.Format(Query.Default.StateQuery,
                                                    data_base_name);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read() && reader.FieldCount == required_filed_count)
                    {
                        string[] result = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                            result[i] = reader[i].ToString();
                        return result;
                    }
                    else
                        return null;
                }
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, изменяет состояние зеркалирования. 
         */
        private static void SetState(bool work,
                                     SqlConnection principal,
                                     string dataBaseName)
        {
            try
            {
                SqlCommand command = principal.CreateCommand();

                command.CommandText =
                    String.Format(work ? 
                                         Query.Default.Resume : 
                                         Query.Default.Suspend,
                                  dataBaseName);

                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, переключает роли зеркалирования.
         */
        private static void Switch(bool force,
                                   SqlConnection connection,
                                   string data_base_name)
        {
            try
            {
                SqlCommand command = connection.CreateCommand();

                command.CommandText =
                    String.Format(force ?
                                          Query.Default.ForceSwitch :
                                          Query.Default.Switch,
                                  data_base_name);

                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, создает бэкап базы данных на основном сервере.
         */
        private static string BackupDataBaseFromPrincipal(SqlConnection principal,
                                                          string data_base_name,
                                                          string backup_path)
        {
            const string backup_extension = @".backup";

            try
            {
                string backup_file = backup_path + 
                                     data_base_name +
                                     backup_extension;

                SqlCommand command = principal.CreateCommand();

                command.CommandText = String.Format(Query.Default.BackupDataBase,
                                                    data_base_name,
                                                    backup_file);

                command.ExecuteNonQuery();

                return backup_file;
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, восстанавливает базу данных на зеркальном сервере.
         */
        private static void RestoreDataBaseToMirror(SqlConnection mirror,
                                                    string data_base_name,
                                                    string backup_file)
        {
            try
            {
                SqlCommand command = mirror.CreateCommand();

                command.CommandText = String.Format(Query.Default.RestoreDataBase,
                                                    data_base_name,
                                                    backup_file);

                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, создает бэкап логов транзакций на основном сервере.
         */
        private static string BackupTransactionLogFromPrincipal(SqlConnection principal,
                                                                string data_base_name, 
                                                                string backup_path)
        {
            const string backup_log_extension = @"_log.backup";

            try
            {
                string backup_file = backup_path +
                                     data_base_name + 
                                     backup_log_extension;

                SqlCommand command = principal.CreateCommand();

                command.CommandText = String.Format(Query.Default.BackupLog,
                                                    data_base_name,
                                                    backup_file);

                command.ExecuteNonQuery();

                return backup_file;
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, восстанавливает бэкап логов транхакций на зеркальном сервере.
         */
        private static void RestoreTransactionLogToMirror(SqlConnection mirror,
                                                          string data_base_name,
                                                          string backup_file)
        {
            try
            {
                SqlCommand command = mirror.CreateCommand();

                command.CommandText =
                    String.Format(Query.Default.RestoreLog,
                                  data_base_name,
                                  backup_file);

                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, создает оконечные точки на серверах.
         */
        private static void CreateEndpoints(SqlConnection principal,
                                            SqlConnection mirror,
                                            uint tcp_listened_port)
        {
            try
            {
                SqlCommand command =
                    new SqlCommand(String.Format(Query.Default.CreateEndpoint,
                                                 tcp_listened_port));

                command.Connection = principal;
                command.ExecuteNonQuery();

                command.Connection = mirror;
                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }

        /*
         * Функция, устанавливает партнеров зеркалирования.
         */
        private static void SetPartners(SqlConnection principal,
                                        SqlConnection mirror, 
                                        string data_base_name,
                                        uint port,
                                        string principal_address,
                                        string mirror_address)
        {
            SetPartner(mirror, principal_address, data_base_name, port);
            SetPartner(principal, mirror_address, data_base_name, port);
        }

        /*
         * Функция, устанавливает партнера для конкретного сервера.
         */
        private static void SetPartner(SqlConnection current,
                                       string partner,
                                       string data_base_name,
                                       uint port)
        {
            try
            {
                SqlCommand command = current.CreateCommand();
                
                command.CommandText = 
                    String.Format(Query.Default.SetPartner,
                                  data_base_name,
                                  partner,
                                  port.ToString());
                 
                command.ExecuteNonQuery();
            }
            catch (SqlException exp)
            {
                throw new MirroringException(exp);
            }
        }
        #endregion
    }
}
