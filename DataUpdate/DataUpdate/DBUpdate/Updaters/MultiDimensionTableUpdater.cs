using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Assignment;

namespace COTES.ISTOKDataUpdate.DBUpdater
{
    [ISTOKDBUpdater(CurrentService.ServiceType.Global)]
    class MultiDimensionTableUpdater : IDBUpdater
    {
        // {DCF7D3A9-8C61-4951-8848-0561BD41FFC1}
        static readonly Guid updaterID = new Guid(0xdcf7d3a9, 0x8c61, 0x4951, 0x88, 0x48, 0x5, 0x61, 0xbd, 0x41, 0xff, 0xc1);
  
        #region IDBUpdater Members

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
            get { return 500; }
        }

        public IEnumerable<Guid> RequiredBefore
        {
            get { return null; }
        }

        public bool Update(CurrentService service)
        {
            var dicBuffer = new Dictionary<int, byte[]>();

            Assignment.MultiDimensionalTable mdtable;
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream stream;

            byte[] ctableBuffer = null;

            using (COTES.ISTOK.MyDBdata dbwork = new COTES.ISTOK.MyDBdata(COTES.ISTOK.ServerType.MSSQL))
            {
                dbwork.SetParam(
                        service.Settings.DataBase.Host,
                        service.Settings.DataBase.Name,
                        service.Settings.DataBase.User,
                        COTES.ISTOK.CommonData.SecureStringToString(
                            COTES.ISTOK.CommonData.DecryptText(
                                COTES.ISTOK.CommonData.Base64ToString(
                                    service.Settings.DataBase.Password))));

                if (dbwork == null)
                    throw new Exception("Соединение не установлено.");
                int transactionID = 0;
                try
                {

                    COTES.ISTOK.DB_Parameters Param = new COTES.ISTOK.DB_Parameters();
                    Param.Add("tablename", DbType.String, "unit");
                    Param.Add("name", DbType.String, "ctable");
                    string query = "select value,idnum,parentid from lobs where tablename=@tablename and name=@name";//and parentid=@parentid 

                    transactionID = dbwork.StartTransaction();

                    using (DbDataReader reader = dbwork.ExecSQL_toReader(transactionID, query, Param))
                    {
                        int id = -1;
                        int parentid = -1;
                        object obj;

                        while (reader.Read())
                        {
                            if (reader.IsDBNull(0)) continue;

                            id = -1;
                            ctableBuffer = (byte[])reader.GetValue(0);
                            id = (int)reader.GetValue(1);
                            parentid = (int)reader.GetValue(2);
                            if (ctableBuffer != null)
                            {
                                stream = new MemoryStream(ctableBuffer);

                                obj = bin.Deserialize(stream);
                                stream.Close();

                                if (obj is DataSet)
                                    mdtable = new Assignment.MultiDimensionalTable((DataSet)obj);
                                else 
                                    mdtable = obj as Assignment.MultiDimensionalTable; 

                                if (mdtable != null)
                                    dicBuffer.Add(id, mdtable.ToNewTable().ToBytes());
                            }
                        }
                    }

                    COTES.ISTOK.DB_Parameters a = new COTES.ISTOK.DB_Parameters();
                    COTES.ISTOK.DB_Parameter idnumParameter = new COTES.ISTOK.DB_Parameter("idnum", System.Data.DbType.Int32);
                    COTES.ISTOK.DB_Parameter valueParameter = new COTES.ISTOK.DB_Parameter("value", System.Data.DbType.Binary);
                    a.AddRange(new COTES.ISTOK.DB_Parameter[] { idnumParameter, valueParameter });

                    int count = 0;
                    foreach (var item in dicBuffer.Keys)
                    {
                        idnumParameter.ParamValue = item;
                        valueParameter.ParamValue = dicBuffer[item];
                        count += dbwork.ExecSQL(transactionID, "UPDATE [lobs] SET [name]='normfunc', [value] = @value WHERE [idnum] = @idnum", a);
                    }

                    dbwork.Commit(transactionID);
                }
                catch
                {
                    dbwork.Rollback(transactionID);
                    //throw;
                    return false;

                }
            }
            return true;
        }

        #endregion
    }

    static class MultiDimensionTableConverterExtension
    {
        public static COTES.ISTOK.ASC.MultiDimensionalTable ToNewTable(this Assignment.MultiDimensionalTable oldTable)
        {
            COTES.ISTOK.ASC.MultiDimensionalTable newTable = new COTES.ISTOK.ASC.MultiDimensionalTable();

            Assignment.DimensionInfo xDimension = oldTable.DimensionInfo[0];//oldTable.DimensionInfo.Length - 1];

            newTable.DimensionInfo[0].Name = xDimension.Name;
            newTable.DimensionInfo[0].Measure = xDimension.Measure;

            //for (int i = 1; i < oldTable.DimensionInfo.Length; i++)
            //{
            //    //if (newTable.DimensionInfo.Length - 1 < coords.Length)
            //    Assignment.DimensionInfo dimensionInfo = oldTable.DimensionInfo[i];
            //    newTable.AddDimension(dimensionInfo.Name, dimensionInfo.Measure, coords[coordinates.Length]);

            //}

            CopyTableValues(oldTable, newTable);

            return newTable;
        }

        private static void CopyTableValues(
            Assignment.MultiDimensionalTable oldTable,
            COTES.ISTOK.ASC.MultiDimensionalTable newTable,
            params double[] coordinates)
        {
            if (coordinates.Length < oldTable.DimensionInfo.Length)
            {
                double[] coords = new double[coordinates.Length + 1];
                coordinates.CopyTo(coords,  1);

                double[] dimensionCoords = oldTable.GetDimension(coordinates.Reverse().ToArray());//coordinates);
                for (int i = 0; i < dimensionCoords.Length; i++)
                {
                    coords[0] = dimensionCoords[i];
                    //coords[coordinates.Length] = dimensionCoords[i];
                    if (coords.Length == oldTable.DimensionInfo.Length)
                    {
                        bool removeZero = i == 0 && dimensionCoords[i] != 0f
                            && newTable.GetDimension(coordinates.Reverse().ToArray()).Length == 1;

                        newTable.SetValue(oldTable.GetValue(coords), coords);

                        if (removeZero)
                        {
                            coords[0] = 0f;
                            //coords[coordinates.Length] = 0f;
                            newTable.RemoveValue(coords.Reverse().ToArray());
                        }
                    }
                    else
                    {
                        double[] tmpCoords=null;
                        if (newTable.DimensionInfo.Length - 1 < coords.Length)
                        {
                            //if (newTable.DimensionInfo.Length > 1)
                            //    ;

                            //tmpCoords = new double[coordinates.Length+1];
                            //Array.Copy(coordinates, tmpCoords, coordinates.Length);
                            //tmpCoords[coordinates.Length] = double.NaN;

                            DimensionInfo dimensionInfo = oldTable.DimensionInfo[coordinates.Length + 1];
                            newTable.AddDimension(dimensionInfo.Name, dimensionInfo.Measure, double.NaN); //coords[coordinates.Length]);
                        }
                        CopyTableValues(oldTable, newTable, coords);
                        
                        //if (tmpCoords != null)
                        //    newTable.RemoveValue(tmpCoords);
                    }
                }
            }
        }
    }
}
