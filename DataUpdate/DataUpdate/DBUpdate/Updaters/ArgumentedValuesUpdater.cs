using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class ArgumentedValuesUpdater : IDBUpdater
    {
        #region IDBUpdater Members

        // {45D575B2-CA1F-4de1-8CC6-23799DD0F28C}
        static readonly Guid updaterID = new Guid(0x45d575b2, 0xca1f, 0x4de1, 0x8c, 0xc6, 0x23, 0x79, 0x9d, 0xd0, 0xf2, 0x8c);

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
            get { return 852; }
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
                    transactionID = dbwork.StartTransaction();

                    using (DbDataReader reader = dbwork.ExecSQL_toReader(transactionID, query, Param))
                    {
                        Version1_5.ValueReceiver receiver = new Version1_5.ValueReceiver();

                        while (reader.Read())
                        {
                            int paramID = reader.GetInt32(1);

                            var argumentedValues = receiver.ArgumentedValuesFromBinary(reader.GetDateTime(2), (byte[])reader.GetValue(3));

                            // удаляем безаргументные значения
                            argumentedValues.RemoveAll(a => a.Arguments == null || a.Arguments.Length == 0);

                            var parameterList = argumentedValues.ConvertAll(a =>
                                new COTES.ISTOK.ParamValueItem(ConvertArguments(a.Arguments), a.ValueItem.Time, a.ValueItem.Quality, a.ValueItem.Value));

                            packages[reader.GetInt32(0)] = argumentedValuesToBinary(parameterList);
                        }
                    }

                    DB_Parameter itemParameter;
                    DB_Parameter packParameter;
                    Param.Add(itemParameter = new DB_Parameter("idnum", DbType.Int32));
                    Param.Add(packParameter = new DB_Parameter("pack", DbType.Binary));

                    foreach (var item in packages.Keys)
                    {
                        itemParameter.ParamValue = item;
                        packParameter.ParamValue = packages[item];
                        dbwork.ExecSQL(transactionID, "UPDATE [value_aparam] SET [pack] = @pack WHERE [idnum]=@idnum", Param);
                    }
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

        #endregion

        ArgumentsValues ConvertArguments(Version1_5.ValueArguments oldArguments)
        {
            ArgumentsValues args = new ArgumentsValues();

            for (int i = 0; i < oldArguments.Length; i++)
            {
                for (int j = 0; j < oldArguments[i].Length; j++)
                {
                    args[oldArguments[i][j].Key] = System.Convert.ToDouble(oldArguments[i][j].Value);
                }
            }
            return args;
        }

        enum ArgumentedValuesBynaryKeyword : byte
        {
            Argument,
            Value,
            Correct,
            //LevelDown,
            //LevelUp
        }


        private byte[] argumentedValuesToBinary(IEnumerable<ParamValueItem> argumentedList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    CorrectedParamValueItem correctedValue;
                    foreach (var item in argumentedList)
                    {
                        foreach (var name in item.Arguments)
                        {
                            bw.Write((byte)ArgumentedValuesBynaryKeyword.Argument);
                            bw.Write(name);
                            bw.Write(item.Arguments[name]);
                        }
                        bw.Write((byte)ArgumentedValuesBynaryKeyword.Value);
                        if ((correctedValue = item as CorrectedParamValueItem) != null)
                        {
                            bw.Write(correctedValue.OriginalValueItem.Value);
                            bw.Write((byte)ArgumentedValuesBynaryKeyword.Correct);
                            bw.Write(correctedValue.CorrectedValueItem.Value);
                        }
                        else
                        {
                            bw.Write(item.Value);
                        }
                    }                   
                }
                return ms.ToArray();
            }
        }
    }
}
