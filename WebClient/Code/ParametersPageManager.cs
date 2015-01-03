using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace WebClient
{
    public class ParametersPageManager
    {
        const string prefix = @"?";
        const string format = @"{0}={1}&";
        private Dictionary<string, string> parameters = 
            new Dictionary<string, string>();

        public ParametersPageManager()
        { }

        public static ParametersPageManager FromQueryStrings(NameValueCollection query_strings)
        {
            ParametersPageManager result = new ParametersPageManager();

            foreach (var it in query_strings.AllKeys)
                result.Add(HttpContext.Current.Server.UrlDecode(it),
                           HttpContext.Current.Server.UrlDecode(query_strings[it]));

            return result;
        }

        public void Add(string key, string value)
        { parameters[key] = value; }

        public bool Exist(string key)
        { return parameters.ContainsKey(key); }

        public void Remove(string key)
        {
            try
            {
                parameters.Remove(key);
            }
            catch (KeyNotFoundException exp)
            {
                throw new PageParameterException(exp);
            }
        }

        public void Clear()
        { parameters.Clear(); }

        public string this[string key]
        {
            get
            {
                try
                {
                    return parameters[key];
                }
                catch (KeyNotFoundException exp)
                {
                    throw new PageParameterException(exp);
                }
            }
            set
            {
                try
                {
                    parameters[key] = key;
                }
                catch (KeyNotFoundException exp)
                {
                    throw new PageParameterException(exp);
                }
            }
        }

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
