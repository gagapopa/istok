using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class AddRevisionsUpdater : IDBUpdater
    {
        // {F57F3672-8478-444f-909D-DE2B483606B0}
        static readonly Guid updaterGUID = new Guid(0xf57f3672, 0x8478, 0x444f, 0x90, 0x9d, 0xde, 0x2b, 0x48, 0x36, 0x6, 0xb0);

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
            get { return 843; }
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

                    AddRevisionTable(dbwork, transactionID);

                    AddRevisionColumns(dbwork, transactionID);

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

        private void AddRevisionTable(MyDBdata dbwork, int transactionID)
        {
            const String revisionTableCreateTable = @"
CREATE TABLE [dbo].[revision](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[time_from] [datetime] NOT NULL,
	[brief] [nvarchar](128) NOT NULL,
	[comment] [nvarchar](1024) NULL,
 CONSTRAINT [PK_revision] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]";

            dbwork.ExecSQL(transactionID, revisionTableCreateTable, null);
        }

        private void AddRevisionColumns(MyDBdata dbwork, int transactionID)
        {
            const String addPropsRevisionColumnQuery = "ALTER TABLE [dbo].[props] ADD [revision] [int] NULL";
            const String addLobsRevisionColumnQuery = "ALTER TABLE [dbo].[lobs] ADD [revision] [int] NULL";

            const String addPropsRevisionFKQuery = @"
ALTER TABLE [dbo].[props]  WITH CHECK ADD  CONSTRAINT [FK_props_revision] FOREIGN KEY([revision])
REFERENCES [dbo].[revision] ([id])

ALTER TABLE [dbo].[props] CHECK CONSTRAINT [FK_props_revision]";
            const String addLobsRevisionFKQuery = @"
ALTER TABLE [dbo].[lobs]  WITH CHECK ADD  CONSTRAINT [FK_lobs_revision] FOREIGN KEY([revision])
REFERENCES [dbo].[revision] ([id])

ALTER TABLE [dbo].[lobs] CHECK CONSTRAINT [FK_lobs_revision]";

            // добавляем колонки
            dbwork.ExecSQL(transactionID, addPropsRevisionColumnQuery, null);
            dbwork.ExecSQL(transactionID, addLobsRevisionColumnQuery, null);

            // добавляем внешние ключи
            dbwork.ExecSQL(transactionID, addPropsRevisionFKQuery, null);
            dbwork.ExecSQL(transactionID, addLobsRevisionFKQuery, null);
        }
    }
}
