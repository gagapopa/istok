using COTES.ISTOK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate.DBUpdate.Updaters
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class AddUnitTypesChildFilterNExtGuidColumnsUpdater : IDBUpdater
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
            get { return 404; }
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

                string query = "select [idnum], [idparam], [time], [pack] FROM [value_aparam]";

                Dictionary<int, byte[]> packages = new Dictionary<int, byte[]>();

                int transactionID = 0;
                try
                {
                    const String addChildFilterColumnQuery = "ALTER TABLE [unit_type] ADD [child_filter] [varchar](1024) NULL";
                    const String addExtGuidColumnQuery = "ALTER TABLE [unit_type] ADD [ext_guid] [binary](16) NULL";

                    transactionID = dbwork.StartTransaction();

                    dbwork.ExecSQL(transactionID, addChildFilterColumnQuery, null);

                    dbwork.ExecSQL(transactionID, addExtGuidColumnQuery, null);

                    dbwork.Commit(transactionID);
                }
                catch (Exception)
                {
                    dbwork.Rollback(transactionID);
                    //throw;
                    return false;

                }
            }
            return true;
        }
    }
}
