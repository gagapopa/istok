using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace WebClient
{
    public class ParamsPageBuilder
    {
        const string prefix = @"?";
        const string format = @"{0}={1}&";
        private Dictionary<string, string> parameters = 
            new Dictionary<string, string>();

        public ParamsPageBuilder()
        { }

        public void Add(string key, string value)
        { parameters[key] = value; }

        public bool Exist(string key)
        { return parameters.ContainsKey(key); }

        public void Remove(string key)
        { parameters.Remove(key); }

        public void Clear()
        { parameters.Clear(); }

        public override string ToString()
        {
            if (parameters.Count == 0) return "";

            string result = prefix;
            

            foreach (var it in parameters)
                result += String.Format(format,
                                        HttpContext.Current.Server.UrlEncode(it.Key),
                                        HttpContext.Current.Server.UrlEncode(it.Value));

            return RemoveLastSeparator(result);
        }

        private string RemoveLastSeparator(string source)
        {
            return source.Remove(source.Length - 1);
        }
    }
}
