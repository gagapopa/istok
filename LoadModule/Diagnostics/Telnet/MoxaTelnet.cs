using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Block.Telnet;
using System.Text.RegularExpressions;
using COTES.ISTOK.DiagnosticsInfo;
using System.Data;

namespace COTES.ISTOK.Block.Telnet
{
    class MoxaTelnet : ISummaryInfo
    {
        const string caption = "Telnet";
        string host;

        public MoxaTelnet(string host)
        {
            this.host = host;
        }

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(caption);
            string[] logs = GetLogs(host, false);
            
            table.Columns.Add("Сообщение");
            foreach (var item in logs) table.Rows.Add(item);
            ds.Tables.Add(table);
            
            return ds;
        }
        public string GetSummaryCaption()
        {
            return caption;
        }

        public string[] GetLogs(bool clearlog)
        {
            return GetLogs(host, clearlog);
        }
        public string[] GetLogs(string host, bool clearlog)
        {
            TelnetConnection tc = new TelnetConnection(host, 23);
            string s = "$$";

            string prompt = s.TrimEnd();
            prompt = s.Substring(prompt.Length - 1, 1);
            if (prompt != "$" && prompt != ">")
                throw new Exception("Connection failed");

            tc.Read(); tc.WriteLine("");
            tc.Read(); tc.WriteLine("m");
            tc.Read(); tc.WriteLine("");
            tc.Read(); tc.WriteLine("y");
            Regex regexp = new Regex(@"\d\d\d\d/\d\d/\d\d \d\d:\d\d:\d\d \[.*\] .*");
            List<string> lst = new List<string>();
            prompt = tc.Read();
            if (!prompt.Contains("No system log record"))
            {
                while (!string.IsNullOrEmpty(prompt))
                {
                    foreach (Match item in regexp.Matches(prompt))
                    {
                        lst.Add(item.ToString());
                        //Console.WriteLine(item.ToString());
                    }
                    tc.WriteLine(" ");
                    prompt = tc.Read();
                }
                if (clearlog) tc.WriteLine(Convert.ToChar(4).ToString()); //Ctrl+D (clear log)
            }
            return lst.ToArray();
        }
    }
}
