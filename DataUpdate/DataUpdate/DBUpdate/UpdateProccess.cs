using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace COTES.ISTOKDataUpdate
{
    class UpdateProccess
    {
        CurrentService service;

        public UpdateProccess(CurrentService service)
        {
            this.service = service;
        }

        private IEnumerable<IDBUpdater> CollectUpdaters()
        {
            List<IDBUpdater> updaterList = new List<IDBUpdater>();

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var interfaceType=typeof(IDBUpdater);

            foreach (var type in assembly.GetTypes())
            {

                if (type.GetInterface(interfaceType.FullName) == interfaceType)
                {
                    var attributes = type.GetCustomAttributes(typeof(ISTOKDBUpdaterAttribute), false);

                    if (attributes != null && attributes.Length > 0)
                    {
                        var attribute = attributes[0] as ISTOKDBUpdaterAttribute;

                        if (attribute != null
                            && (attribute.ServerType & service.CurrentServiceType) != 0)
                        {
                            try
                            {
                                var updater = type.GetConstructor(new Type[] { }).Invoke(new Object[] { }) as IDBUpdater;

                                if (updater != null)
                                    updaterList.Add(updater);
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                    }
                }
            }

            return updaterList;
        }

        public void Proccess()
        {
            var updaters = CollectUpdaters();

            updaters = from u in updaters orderby u.DBVersionTo select u;

            int dbVersion = GetDBVersion();
            int lastUpdaterVersion = 0;

            List<IDBUpdater> successUpdaters=new List<IDBUpdater>();
            List<IDBUpdater> failedUpdaters=new List<IDBUpdater>();

            try
            {
                foreach (var updater in updaters)
                {
                    if (dbVersion > updater.DBVersionTo)
                    {
                        successUpdaters.Add(updater);
                    }
                    else
                    {
                        service.Message(COTES.ISTOK.MessageCategory.Message, String.Format("Обновление {0}", updater));
                        if (updater.Update(service))
                        {
                            service.Message(COTES.ISTOK.MessageCategory.Message, String.Format("Обновление {0} произведенно удачно", updater));
                            successUpdaters.Add(updater);
                            lastUpdaterVersion = updater.DBVersionTo;
                        }
                        else
                        {
                            service.Message(COTES.ISTOK.MessageCategory.Message, String.Format("Обновление {0} произведенно с ошибками", updater));
                            failedUpdaters.Add(updater);
                        }
                    }
                }
            }
            finally
            {
                if (failedUpdaters.Count > 0)
                    service.Message(COTES.ISTOK.MessageCategory.Message, String.Format("{0} обновлений не применено", failedUpdaters.Count));
                else
                    service.Message(COTES.ISTOK.MessageCategory.Message, "Все обновления применены");

                SaveDBVersion(lastUpdaterVersion);
            }
        }

        private int GetDBVersion()
        {
            int dbVersion = 0;

            //SimpleLogger.ILogger logger = new SimpleLogger.LoggerContainer();

            using (COTES.ISTOK.MyDBdata dbwork = new COTES.ISTOK.MyDBdata(COTES.ISTOK.ServerType.MSSQL))
            {
                dbwork.DB_Host = service.Settings.DataBase.Host;
                dbwork.DB_Name = service.Settings.DataBase.Name;
                dbwork.DB_User = service.Settings.DataBase.User;
                dbwork.DB_Password = COTES.ISTOK.CommonData.SecureStringToString(
                                 COTES.ISTOK.CommonData.DecryptText(
                                     COTES.ISTOK.CommonData.Base64ToString(
                                         service.Settings.DataBase.Password)));

                try
                {
                    const String query = "SELECT TOP 1 [db_version] FROM [db_info]";

                    using (System.Data.Common.DbDataReader reader = dbwork.ExecSQL_toReader(query, null))
                    {
                        while (reader.Read())
                        {
                            String versionString = reader.GetString(0);
                            dbVersion = int.Parse(versionString);
                        }
                    }
                }
                catch
                {
                    dbVersion = 0;
                } 
            }
            return dbVersion;
        }

        private void SaveDBVersion(int dbVersion)
        {
            using (COTES.ISTOK.MyDBdata dbwork = new COTES.ISTOK.MyDBdata(COTES.ISTOK.ServerType.MSSQL))
            {
                dbwork.DB_Host = service.Settings.DataBase.Host;
                dbwork.DB_Name = service.Settings.DataBase.Name;
                dbwork.DB_User = service.Settings.DataBase.User;
                dbwork.DB_Password = COTES.ISTOK.CommonData.SecureStringToString(
                                 COTES.ISTOK.CommonData.DecryptText(
                                     COTES.ISTOK.CommonData.Base64ToString(
                                         service.Settings.DataBase.Password)));

                int transactionID = 0;
                try
                {
                    transactionID = dbwork.StartTransaction();

                    const String createQuery = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[db_info]') AND type in (N'U'))
CREATE TABLE [dbo].[db_info](
	[db_version] [nchar](10) NULL
) ON [PRIMARY]";
                    const String updateVersionQuery = "UPDATE [db_info] SET [db_version] = @db_version";
                    const String insertVersionQuery = "INSERT INTO [db_info] ([db_version]) VALUES (@db_version)";

                    dbwork.ExecSQL(createQuery, null);

                    COTES.ISTOK.DB_Parameters Param = new COTES.ISTOK.DB_Parameters();
                    Param.Add("db_version", DbType.String, dbVersion.ToString());

                    int rows = dbwork.ExecSQL(updateVersionQuery, Param);

                    if (rows == 0)
                        dbwork.ExecSQL(insertVersionQuery, Param);

                    dbwork.Commit(transactionID);
                }
                catch
                {
                    if (transactionID != 0)
                        dbwork.Rollback(transactionID);
                    throw;
                }
            }
        }
    }
}
