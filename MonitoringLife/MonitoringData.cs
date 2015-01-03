// Type: EMA.ASTDK.Data.MonitoringData
// Assembly: MonitoringLife, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\kody\projects\Таганрок\MonitoringLife.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace EMA.ASTDK.Data
{
  public class MonitoringData : IMonitoringData
  {
    public DataTable GetDrum(int boiler_id, string code)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_DrumSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetDrumNozzle(int boiler_id, string code)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_DrumNozzleSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetStraightPipe(int boiler_id, string code)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_StraightPipeSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetBend(int boiler_id, string code)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_BendSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetEquivalentStress(int boiler_id, string code, DateTime? start , DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_EquivalentStressSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Start", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) start;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Finish", SqlDbType.DateTime);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) finish;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetLifeCreep(int boiler_id, string code, DateTime? start , DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeCreepSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Start", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) start;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Finish", SqlDbType.DateTime);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) finish;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void GetLifeCreepCurrent(int boiler_id, string code, out DateTime timeCreep, out double lifeCreep)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeCreepCurrentSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        timeCreep = Convert.ToDateTime(dataTable.Rows[0]["TimeCreep"].ToString());
        lifeCreep = Convert.ToDouble(dataTable.Rows[0]["Value"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetLifeHFC(int boiler_id, string code, DateTime? start , DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeHFCSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Start", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) start;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Finish", SqlDbType.DateTime);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) finish;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void GetLifeHFCCurrent(int boiler_id, string code, out DateTime timeHFC, out double lifeHFC)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeHFCCurrentSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        timeHFC = Convert.ToDateTime(dataTable.Rows[0]["TimeHFC"].ToString());
        lifeHFC = Convert.ToDouble(dataTable.Rows[0]["Value"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetLifeLFC(int boiler_id, string code, DateTime? start , DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeLFCSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Start", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) start;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Finish", SqlDbType.DateTime);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) finish;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void GetLifeLFCCurrent(int boiler_id, string code, out DateTime timeLFC, out double lifeLFC)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeLFCCurrentSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        timeLFC = Convert.ToDateTime(dataTable.Rows[0]["TimeLFC"].ToString());
        lifeLFC = Convert.ToDouble(dataTable.Rows[0]["Value"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetLifeStartStop(int boiler_id, string code, DateTime? start , DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeStartStopSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Start", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) start;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Finish", SqlDbType.DateTime);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) finish;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        dataTable.Columns.Add("LifeTotal");
        foreach (DataRow dataRow in (InternalDataCollectionBase) dataTable.Rows)
          dataRow["LifeTotal"] = (object) (Convert.ToDouble(dataRow["LifeBasicCycle"].ToString()) + Convert.ToDouble(dataRow["LifeLFC"].ToString()) + Convert.ToDouble(dataRow["LifeHFC_or_Creep"].ToString()));
        return dataTable;
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void GetLifeStartStopCurrent(int boiler_id, string code, out DateTime start, out DateTime stop, out double lifeTotal, out double lifeBasicCycle, out double lifeLFC, out double lifeHFC_or_Creep)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_LifeStartStopCurrentSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) boiler_id;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) code;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        start = Convert.ToDateTime(dataTable.Rows[0]["TimeStart"].ToString());
        stop = Convert.ToDateTime(dataTable.Rows[0]["TimeStop"].ToString());
        lifeTotal = Convert.ToDouble(dataTable.Rows[0]["LifeBasicCycle"].ToString()) + Convert.ToDouble(dataTable.Rows[0]["LifeLFC"].ToString()) + Convert.ToDouble(dataTable.Rows[0]["LifeHFC_or_Creep"].ToString());
        lifeBasicCycle = Convert.ToDouble(dataTable.Rows[0]["LifeBasicCycle"].ToString());
        lifeLFC = Convert.ToDouble(dataTable.Rows[0]["LifeLFC"].ToString());
        lifeHFC_or_Creep = Convert.ToDouble(dataTable.Rows[0]["LifeHFC_or_Creep"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void GetStartCount(int boiler_id, out int totalFact, out int coldStateFact, out int notCoolStateFact, out int hotStateFact, out int hydrotesting, out int coldStatePermit, out int notCoolHotStatePermit)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_StartCountSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter = sqlCmd.Parameters.Add("@Boiler_Id", SqlDbType.TinyInt);
      sqlParameter.Direction = ParameterDirection.Input;
      sqlParameter.Value = (object) boiler_id;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        totalFact = (int) Convert.ToInt16(dataTable.Rows[0]["TotalFact"].ToString());
        coldStateFact = (int) Convert.ToInt16(dataTable.Rows[0]["ColdStateFact"].ToString());
        notCoolStateFact = (int) Convert.ToInt16(dataTable.Rows[0]["NotCoolStateFact"].ToString());
        hotStateFact = (int) Convert.ToInt16(dataTable.Rows[0]["HotStateFact"].ToString());
        hydrotesting = (int) Convert.ToInt16(dataTable.Rows[0]["Hydrotesting"].ToString());
        coldStatePermit = (int) Convert.ToInt16(dataTable.Rows[0]["ColdStatePermit"].ToString());
        notCoolHotStatePermit = (int) Convert.ToInt16(dataTable.Rows[0]["NotCoolHotStatePermit"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetMessageLog(string type, DateTime? start, DateTime? finish )
    {
      SqlCommand sqlCmd = new SqlCommand("prc_MessageLogSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Type", SqlDbType.VarChar);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) type;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@start", SqlDbType.DateTime);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) start;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@finish", SqlDbType.DateTime);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) finish;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetModulusElasticity()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefModulusElasticitySelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetLinearExpansion()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefLinearExpansionSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetThermalConduct()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefThermalConductSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetTemperatureConduct()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefTemperatureConductSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      return ConnectionDB.ExecuteSQLCmd(sqlCmd);
    }

    public DataTable GetPermissibleStress()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefPermissibleStressSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetCreepTemperature()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefCreepTemperatureSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetExponent()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefExponentSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetReducing()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefReducingSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetConstant()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_CoefConstantSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetListPlant()
    {
      SqlCommand sqlCmd = new SqlCommand("prc_PlantListSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void AddInfoPlant(int plantKotesId, string plantName, ref int plantEMAllianceId)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_PlantAddInfo");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@KotesId", SqlDbType.Int);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) plantKotesId;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@Name", SqlDbType.VarChar);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) plantName;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Id", SqlDbType.TinyInt);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) plantEMAllianceId;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        plantEMAllianceId = (int) Convert.ToInt16(dataTable.Rows[0]["Id"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public DataTable GetListBoiler(int plantEMAllianceId)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_BoilerListSelect");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter = sqlCmd.Parameters.Add("@Plant_Id", SqlDbType.VarChar);
      sqlParameter.Direction = ParameterDirection.Input;
      sqlParameter.Value = (object) plantEMAllianceId;
      try
      {
        return ConnectionDB.ExecuteSQLCmd(sqlCmd);
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }

    public void AddInfoBoiler(int plantEMAllianceId, int boilerKotesId, string boilerName, ref int boilerEMAllianceId)
    {
      SqlCommand sqlCmd = new SqlCommand("prc_BoilerAddInfo");
      sqlCmd.CommandType = CommandType.StoredProcedure;
      SqlParameter sqlParameter1 = sqlCmd.Parameters.Add("@Plant_Id", SqlDbType.TinyInt);
      sqlParameter1.Direction = ParameterDirection.Input;
      sqlParameter1.Value = (object) plantEMAllianceId;
      SqlParameter sqlParameter2 = sqlCmd.Parameters.Add("@KotesId", SqlDbType.Int);
      sqlParameter2.Direction = ParameterDirection.Input;
      sqlParameter2.Value = (object) boilerKotesId;
      SqlParameter sqlParameter3 = sqlCmd.Parameters.Add("@Name", SqlDbType.VarChar);
      sqlParameter3.Direction = ParameterDirection.Input;
      sqlParameter3.Value = (object) boilerName;
      SqlParameter sqlParameter4 = sqlCmd.Parameters.Add("@Id", SqlDbType.TinyInt);
      sqlParameter4.Direction = ParameterDirection.Input;
      sqlParameter4.Value = (object) boilerEMAllianceId;
      try
      {
        DataTable dataTable = ConnectionDB.ExecuteSQLCmd(sqlCmd);
        boilerEMAllianceId = (int) Convert.ToInt16(dataTable.Rows[0]["Id"].ToString());
      }
      catch (SqlException ex)
      {
        throw ex;
      }
    }
  }
}
