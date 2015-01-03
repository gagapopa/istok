// Type: EMA.ASTDK.Data.ConnectionDB
// Assembly: MonitoringLife, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\kody\projects\Таганрог\MonitoringLife.dll

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System;

namespace EMA.ASTDK.Data
{
    internal class ConnectionDB
    {
        internal static DataTable ExecuteSQLCmd(SqlCommand sqlCmd)
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            String connectionString = appConfig.ConnectionStrings.ConnectionStrings["MonitoringLife.Properties.Settings.connectionString"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
                sqlCmd.Connection = sqlConnection;
                DataTable dataTable = new DataTable();
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                dataTable.Load((IDataReader)sqlDataReader);
                sqlDataReader.Close();
                sqlConnection.Close();
                return dataTable;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
