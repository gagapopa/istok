using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace COTES.ISTOK.DiagnosticsInfo
{
    public class SystemDiagnostician : ISummaryInfo
    {
        const string caption = "Система";

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(caption);
            System.Diagnostics.PerformanceCounter cpu = new System.Diagnostics.PerformanceCounter();
            System.Diagnostics.PerformanceCounter mem = new System.Diagnostics.PerformanceCounter();
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();

            cpu.CategoryName = "Processor";
            cpu.CounterName = "% Processor Time";
            cpu.InstanceName = "_Total";

            mem.CategoryName = "Memory";
            mem.CounterName = "Available MBytes";

            cpu.NextValue();
            mem.NextValue();
            System.Threading.Thread.Sleep(1000);
            table.Columns.Add("Загрузка процессора, %");
            table.Columns.Add("Доступно памяти, Мб");
            table.Columns.Add("Выделено памяти процессом, Мб", typeof(long));
            table.Rows.Add(string.Format("{0}", cpu.NextValue().ToString("F0")), string.Format("{0}", mem.NextValue()),
                proc.PrivateMemorySize64 / 1024 / 1024);
            ds.Tables.Add(table);

            return ds;
        }

        public string GetSummaryCaption()
        {
            return caption;
        }
    }
}
