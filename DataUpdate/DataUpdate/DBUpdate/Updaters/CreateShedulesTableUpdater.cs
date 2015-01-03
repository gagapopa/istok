using COTES.ISTOK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate.DBUpdate.Updaters
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Block | CurrentService.ServiceType.Global)]
    class CreateShedulesTableUpdater:IDBUpdater
    {
        // {074C52AA-234A-4CFA-9CC2-FEF991DDCA85}
        static readonly Guid updaterID = new Guid(0x74c52aa, 0x234a, 0x4cfa, 0x9c, 0xc2, 0xfe, 0xf9, 0x91, 0xdd, 0xca, 0x85);

        public Guid UpdaterID
        {
            get { return updaterID; }
        }

        public int DBVersionFrom
        {
            get { return 0; }
        }

        public int DBVersionTo
        {
            get { return 400; }
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
                    transactionID = dbwork.StartTransaction();

                    AddSchedulesTable(dbwork, transactionID);

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

        private void AddSchedulesTable(MyDBdata dbwork, int transactionID)
        {
            const String revisionTableCreateTable = @"
if not exists (select * from sysobjects where name='schedules' and xtype='U')
CREATE TABLE [dbo].[schedules](
	[id] [int] NULL,
	[name] [varchar](256) NULL,
	[period] [bigint] NULL
) ON [PRIMARY]
";

            dbwork.ExecSQL(transactionID, revisionTableCreateTable, null);
        }
    }
}
