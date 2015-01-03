using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    public interface ILogger
    {
        void Message(MessageLevel level, object message);
        void Message(MessageLevel level, params object[] pars);
    }

    public class Logger : ILogger
    {
        public void Message(MessageLevel level, object message)
        {
            Exception ex;
            string txt;
            if (message != null)
            {
                ex = message as Exception;
                if (ex != null) txt = ex.Message;
                else
                    txt = message.ToString();
            }
            else
                txt = "null";
            Console.WriteLine(string.Format("({0}): {1}", level.ToString(), txt));
        }

        public void Message(MessageLevel level, params object[] pars)
        {
            StringBuilder sb = new StringBuilder();
            if (pars != null)
            {
                if (pars.Length > 0)
                {
                    sb.Append("0: ");
                    if (pars[0] is Exception) sb.Append(((Exception)pars[0]).Message);
                    else
                        if (pars[0] is string) sb.Append(pars[0].ToString());
                }
                if (pars.Length > 1)
                {
                    sb.Append(" 1: ");
                    if (pars[1] is Exception) sb.Append(((Exception)pars[0]).Message);
                    else
                        if (pars[1] is string) sb.Append(pars[0].ToString());
                }
            }
            else
                sb.Append("params");
            Console.WriteLine(string.Format("({0}): {1}", level.ToString(), sb.ToString()));
        }
    }

    public class LoggerContainer : Logger
    {
        //
    }

    public enum MessageLevel
    {
        Error,
        Warning,
        Debug,
        Info,
        Critical
    }
}
