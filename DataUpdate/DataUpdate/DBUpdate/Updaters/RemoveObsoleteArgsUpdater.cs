using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class RemoveObsoleteArgsUpdater : IDBUpdater
    {
        // {D11200B0-1A0A-4a15-9237-E13EE30F300E}
        static readonly Guid updaterGUID = new Guid(0xd11200b0, 0x1a0a, 0x4a15, 0x92, 0x37, 0xe1, 0x3e, 0xe3, 0xf, 0x30, 0xe);

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
            get { return 840; }
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
                    const String dropQueryFormat = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
DROP TABLE [dbo].[{0}]";

                    transactionID = dbwork.StartTransaction();

                    String[] obsoleteTables = new String[] { "value_cparam", "arg_value", "arg_set" };

                    foreach (var tableName in obsoleteTables)
                    {
                        dbwork.ExecSQL(transactionID, String.Format(dropQueryFormat, tableName), null);
                    }

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
