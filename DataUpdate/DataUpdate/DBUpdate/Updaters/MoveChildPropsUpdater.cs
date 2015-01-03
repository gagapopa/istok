using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class MoveChildPropsUpdater : IDBUpdater
    {
        // {338D4C14-B77F-4c2d-97C7-0F859E5E79B1}
        static readonly Guid updaterGUID = new Guid(0x338d4c14, 0xb77f, 0x4c2d, 0x97, 0xc7, 0xf, 0x85, 0x9e, 0x5e, 0x79, 0xb1);

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
            get { return 841; }
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

                    CreateChildParamTable(dbwork, transactionID);

                    MoveChildParamProperties(dbwork, transactionID);

                    RemoveExcessProperties(dbwork, transactionID);

                    AlterPropsTableFK(dbwork, transactionID);

                    RemoveExcessColumn(dbwork, transactionID);

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

        private void CreateChildParamTable(MyDBdata dbwork, int transactionID)
        {
            const String createTableQuery = @"
CREATE TABLE [dbo].[child_props](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[parentid] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[value] [varchar](max) NULL,
 CONSTRAINT [PK_child_props] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[child_props]  WITH CHECK ADD  CONSTRAINT [FK_child_props_child_params] FOREIGN KEY([parentid])
REFERENCES [dbo].[child_params] ([idnum])

ALTER TABLE [dbo].[child_props] CHECK CONSTRAINT [FK_child_props_child_params]";

            dbwork.ExecSQL(transactionID, createTableQuery, null);
        }

        private void MoveChildParamProperties(MyDBdata dbwork, int transactionID)
        {
            const String selectPropertiesQuery = @"SELECT [idnum], [tablename], [parentid], [name], [value]  FROM [props] WHERE [tablename] = 'child_params'";
            const String insertPropertiesQuery = "INSERT INTO [child_props] ([parentid], [name], [value]) VALUES (@parentid, @name, @value)";
            const String removePropertiesQuery = "DELETE [props] WHERE [tablename] <> 'unit'";

            DB_Parameters Param = new DB_Parameters();
            DB_Parameter parentidParameter = new DB_Parameter("parentid", DbType.Int32);
            DB_Parameter nameParameter = new DB_Parameter("name", DbType.String);
            DB_Parameter valueParameter = new DB_Parameter("value", DbType.String);

            Param.AddRange(new DB_Parameter[] { parentidParameter, nameParameter, valueParameter });

            using (DataTable table = dbwork.ExecSQL_toTable(transactionID, selectPropertiesQuery, null))
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    try
                    {
                        parentidParameter.ParamValue = dataRow["parentid"];
                        nameParameter.ParamValue = dataRow["name"];
                        valueParameter.ParamValue = dataRow["value"];

                        dbwork.ExecSQL(transactionID, insertPropertiesQuery, Param);
                    }
                    catch (Exception)
                    {
                        // муссорные свойства
                    }
                }
            }

            // удаляем перенесенные свойства
            dbwork.ExecSQL(transactionID, removePropertiesQuery, null);
        }

        private void RemoveExcessProperties(MyDBdata dbwork, int transactionID)
        {
            const String removeExcessPropsQuery = "DELETE FROM [props] WHERE NOT EXISTS (SELECT * FROM [unit] u WHERE u.[idnum] = [parentid])";
            const String removeExcessLobsQuery = "DELETE FROM [lobs] WHERE NOT EXISTS (SELECT * FROM [unit] u WHERE u.[idnum] = [parentid])";

            // удаляем строковае мусорные свойства 
            dbwork.ExecSQL(transactionID, removeExcessPropsQuery, null);

            // удаляем бинарные мусорные свойства
            dbwork.ExecSQL(transactionID, removeExcessLobsQuery, null);
        }

        private void AlterPropsTableFK(MyDBdata dbwork, int transactionID)
        {
            const String alterPropsQuery = @"
ALTER TABLE [dbo].[props]  WITH CHECK ADD  CONSTRAINT [FK_props_unit] FOREIGN KEY([parentid])
REFERENCES [dbo].[unit] ([idnum])

ALTER TABLE [dbo].[props] CHECK CONSTRAINT [FK_props_unit]";
            const String alterLobsQuery = @"
ALTER TABLE [dbo].[lobs]  WITH CHECK ADD  CONSTRAINT [FK_lobs_unit] FOREIGN KEY([parentid])
REFERENCES [dbo].[unit] ([idnum])

ALTER TABLE [dbo].[lobs] CHECK CONSTRAINT [FK_lobs_unit]";

            // добавляем ключ обычным свойствам
            dbwork.ExecSQL(transactionID, alterPropsQuery, null);

            // добавляем ключ бинарным свойствам
            dbwork.ExecSQL(transactionID, alterLobsQuery, null);
        }

        private void RemoveExcessColumn(MyDBdata dbwork, int transactionID)
        {
            const String dropPropOldIndexQuery = @"
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[props]') AND name = N'idx1_props')
DROP INDEX [idx1_props] ON [dbo].[props] WITH ( ONLINE = OFF )

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[props]') AND name = N'props_idx1')
DROP INDEX [props_idx1] ON [dbo].[props] WITH ( ONLINE = OFF )";
            const String dropLobsOldIndexQuery = @"
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lobs]') AND name = N'lobs_idx12')
DROP INDEX [lobs_idx12] ON [dbo].[lobs] WITH ( ONLINE = OFF )
";

            const String createPropsIndex = @"
CREATE UNIQUE NONCLUSTERED INDEX [idx1_props] ON [dbo].[props] 
(
	[parentid] ASC,
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
            const String createLobsIndex = @"
CREATE UNIQUE NONCLUSTERED INDEX [lobs_idx12] ON [dbo].[lobs] 
(
	[parentid] ASC,
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
";

            const String removePropsTablenameQuery = "ALTER TABLE [dbo].[props] DROP COLUMN [tablename]";
            const String removelobsTablenameQuery = "ALTER TABLE [dbo].[lobs] DROP COLUMN [tablename]";

            // удаляем старые индексы
            dbwork.ExecSQL(transactionID, dropPropOldIndexQuery, null);

            dbwork.ExecSQL(transactionID, dropLobsOldIndexQuery, null);

            // создаём новые индексы
            dbwork.ExecSQL(transactionID, createPropsIndex, null);

            // удаляем колонку бинарным свойствам
            dbwork.ExecSQL(transactionID, createLobsIndex, null);

            // удаляем колонку обычным свойствам
            dbwork.ExecSQL(transactionID, removePropsTablenameQuery, null);

            // удаляем колонку бинарным свойствам
            dbwork.ExecSQL(transactionID, removelobsTablenameQuery, null);
        }
    }
}
