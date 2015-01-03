using COTES.ISTOK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate.DBUpdate.Updaters
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class CreateValueAparamTableUpdater:IDBUpdater
    {
        // {99D5CD4B-0FCE-4204-891D-3657F34D65FC}
        static readonly Guid updaterGUID = new Guid(0x99d5cd4b, 0xfce, 0x4204, 0x89, 0x1d, 0x36, 0x57, 0xf3, 0x4d, 0x65, 0xfc);

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
            get { return 500; }
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

                    AddValueAparamTable(dbwork, transactionID);

                    //AddRevisionColumns(dbwork, transactionID);

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

        private void AddValueAparamTable(MyDBdata dbwork, int transactionID)
        {
            const String revisionTableCreateTable = @"
CREATE TABLE [dbo].[value_aparam](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[idparam] [int] NULL,
	[time] [datetime] NULL,
	[pack] [varbinary](max) NULL,
 CONSTRAINT [PK_value_aparam] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            dbwork.ExecSQL(transactionID, revisionTableCreateTable, null);
        }
    }
}
