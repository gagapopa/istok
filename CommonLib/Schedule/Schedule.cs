using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace COTES.ISTOK
{
    [Serializable]
    public class Schedule
    {
        public ScheduleReg Rule { get; set; }
        public int Id { get; set; }
        public DateTime LastCall { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        [NonSerialized]
        public List<AsyncDelegate> ActionList;

        public Schedule()
        {
            Name = "";
            LastCall = DateTime.MinValue;
            ActionList = new List<AsyncDelegate>();
        }
        public Schedule(DbDataReader reader)
            : this()
        {
            Id = (int)reader["id"];
            Name = (string)reader["name"];
            DataTable table = reader.GetSchemaTable();
            foreach (DataRow row in table.Rows)
            {
                string col = row["ColumnName"].ToString();
                switch (col.ToLower())
                {
                    //возможно, в колонке type стоит хранить тип расписания,
                    //но сейчас пока все хранимые расписания отвечают за передачу
                    //значений параметров с блочного
                    case "type":
                        Type = (int)reader[col];
                        break;
                    case "rule":
                        Rule = new ScheduleReg((string)reader[col]);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
        public static Schedule FromString(string schedule)
        {
            Schedule res = new Schedule();
            //
            return res;
        }
    }
}
