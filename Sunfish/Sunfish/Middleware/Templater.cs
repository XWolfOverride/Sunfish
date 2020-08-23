using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DolphinWebXplorer2.Middleware
{
    public class Templater
    {
        private List<string> parts = new List<string>();

        public Templater(string template)
        {
            Tokenize(template);
        }

        private void Tokenize(string templ) //Can be optimized with indexOf??
        {
            parts.Clear();
            StringBuilder sb = new StringBuilder();
            bool key = false;
            for (int i = 0; i < templ.Length; i++)
            {
                char c = templ[i];
                if (!key && c == '{')
                {
                    key = true;
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
                else if (key && c == '}')
                {
                    key = false;
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
                else
                    sb.Append(c);
            }
            if (sb.Length > 0)
                parts.Add(sb.ToString());
        }

        private string GetValue(string key, object[] data)
        {
            foreach (object o in data)
            {
                if (o == null)
                    continue;
                if (o is Dictionary<string, object>)
                {
                    Dictionary<string, object> d = (Dictionary<string, object>)o;
                    object v;
                    if (d.TryGetValue(key, out v))
                        if (v == null)
                            return null;
                        else
                            return v.ToString();
                }
                else
                {
                    Type t = o.GetType();
                    try
                    {
                        PropertyInfo p = t.GetProperty(key);
                        if (p != null)
                        {
                            object v = p.GetValue(o);
                            return v?.ToString();
                        }
                    }
                    catch { };
                }
            }
            return null;
        }

        public string Process(params object[] data)
        {
            StringBuilder sb = new StringBuilder();
            bool key = false;
            foreach (string s in parts)
            {
                if (key)
                {
                    key = false;
                    string v = GetValue(s, data);
                    if (!string.IsNullOrEmpty(v))
                        sb.Append(v);
                }
                else
                {
                    key = true;
                    sb.Append(s);
                }
            }
            return sb.ToString();
        }

    }
}
