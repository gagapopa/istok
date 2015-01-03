using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class AddValueAparamChangeTimeUpdater : IDBUpdater
    {
         // {77A0A598-A7F6-4fc6-8646-D18366BD84A9}
        static readonly Guid updaterGUID = new Guid(0x77a0a598, 0xa7f6, 0x4fc6, 0x86, 0x46, 0xd1, 0x83, 0x66, 0xbd, 0x84, 0xa9);

        #region IDBUpdater Members

        public Guid UpdaterID
        {
            get { return updaterGUID; }
        }

        public int DBVersionFrom
        {
            get { return 0; }
        }

        public int DBVersionTo
        {
            get { return 842; }
        }

        public IEnumerable<Guid> RequiredBefore
        {
            get { return null; }
        }

        public bool Update(CurrentService service)
        {
            using (MyDBdata dbwork = new MyDBdata(ServerType.MSSQL))
            {
                dbwork.DB_Host = service.Settings.DataBase.Host;
                dbwork.DB_Name = service.Settings.DataBase.Name;
                dbwork.DB_User = service.Settings.DataBase.User;
                dbwork.DB_Password = CommonData.SecureStringToString(
                                 CommonData.DecryptText(
                                     CommonData.Base64ToString(
                                         service.Settings.DataBase.Password)));

                if (dbwork == null)
                    throw new Exception("Соединение не установлено.");

                DB_Parameters Param = new DB_Parameters();

                int transactionID = 0;
                try
                {
                    const String addColumnQuery = "ALTER TABLE [value_aparam] ADD [ch_time] [datetime] NULL";
                    const String fillColumnValueQuery = "UPDATE [value_aparam] SET [ch_time] = @ch_time";
                    const String alterColumnQuery = "ALTER TABLE [value_aparam] ALTER COLUMN [ch_time] [datetime] NOT NULL";

                    transactionID = dbwork.StartTransaction();

                    // добавляем колонку
                    dbwork.ExecSQL(transactionID, addColumnQuery, null);

                    // выставляем время изменения на текущие время
                    Param.Add("ch_time", System.Data.DbType.DateTime, DateTime.Now);
                    dbwork.ExecSQL(transactionID, fillColumnValueQuery, Param);

                    // выставляем ограничение на колонку
                    dbwork.ExecSQL(transactionID, alterColumnQuery, null);

                    dbwork.Commit(transactionID);
                }
                catch (Exception exc)
                {
                    dbwork.Rollback(transactionID);

                    service.Message(MessageCategory.Error, exc.Message);
                    //throw;
                    return false;

                }
            }
            return true;
        }

        #endregion
    }
}
