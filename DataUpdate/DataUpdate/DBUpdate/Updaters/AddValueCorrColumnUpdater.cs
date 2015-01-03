using COTES.ISTOK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate.DBUpdate.Updaters
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class AddValueCorrColumnUpdater : IDBUpdater
    {
        public Guid UpdaterID
        {
            get { throw new NotImplementedException(); }
        }

        public int DBVersionFrom
        {
            get { return 0; }
        }

        public int DBVersionTo
        {
            get { return 830; }
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
                    const String addColumnQuery = "ALTER TABLE [dbo].[value_mparam] ADD [value_corr] [decimal](25, 7) NULL";
                    const String fillColumnValueQuery = @"UPDATE [dbo].[value_mparam]
   SET [value] = [value_begin]
      ,[value_corr] = [value]
 WHERE not value_begin is null AND value_begin <> value";
                    const String removeColumnQuery = "ALTER TABLE [value_mparam] DROP COLUMN [value_begin] ";

                    transactionID = dbwork.StartTransaction();

                    // добавляем колонку value_corr
                    dbwork.ExecSQL(transactionID, addColumnQuery, null);

                    // выставляем корректировки
                    dbwork.ExecSQL(transactionID, fillColumnValueQuery, null);

                    // удаляем столбец value_begin
                    dbwork.ExecSQL(transactionID, removeColumnQuery, null);

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
    }
}
