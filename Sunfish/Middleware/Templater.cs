using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sunfish.Middleware
{
    public class Templater
    {
        enum TempaltePartMode
        {
            Literal, Value, Start, End, Call
        }

        class TemplatePart
        {
            public TempaltePartMode Mode;
            public string Key;

            public TemplatePart(TempaltePartMode mode, string key)
            {
                Mode = mode;
                Key = key;
            }
            public override string ToString()
            {
                return "Part <" + Mode + " " + Key + ">";
            }
        }

        private List<TemplatePart> parts = new List<TemplatePart>();

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
                    string p = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(p))
                        parts.Add(new TemplatePart(TempaltePartMode.Literal, p));
                    sb.Clear();
                }
                else if (key && c == '}')
                {
                    key = false;
                    string p = sb.ToString().Trim();
                    if (p.Length > 0)
                        if (p.StartsWith("@"))
                            parts.Add(new TemplatePart(TempaltePartMode.Start, p.Substring(1)));
                        else if (p.StartsWith("%"))
                            parts.Add(new TemplatePart(TempaltePartMode.Call, p.Substring(1)));
                        else if (p == "/")
                            parts.Add(new TemplatePart(TempaltePartMode.End, null));
                        else if (p == "{")
                            parts.Add(new TemplatePart(TempaltePartMode.Literal, p));
                        else parts.Add(new TemplatePart(TempaltePartMode.Value, p));
                    sb.Clear();
                }
                else
                    sb.Append(c);
            }
            if (sb.Length > 0)
                if (key)
                    throw new Exception("Template placeholder not closed");
                else
                    parts.Add(new TemplatePart(TempaltePartMode.Literal, sb.ToString().Trim()));
        }

        private object GetValue(string key, object[] data)
        {
            if (key == "AppName")
                return "Sunfish";
            if (key == "AppVersion")
                return Program.VERSION;
            if (data != null)
                foreach (object o in data)
                {
                    if (o == null)
                        continue;
                    if (o is Dictionary<string, object>)
                    {
                        Dictionary<string, object> d = (Dictionary<string, object>)o;
                        object v;
                        if (d.TryGetValue(key, out v))
                            return v;
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
                                return v;
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
            int level = 0;
            int ignore = int.MaxValue;
            foreach (TemplatePart s in parts)
            {
                switch (s.Mode)
                {
                    case TempaltePartMode.Value:
                        if (level < ignore)
                        {
                            string v = GetValue(s.Key, data)?.ToString();
                            if (!string.IsNullOrEmpty(v))
                                sb.Append(v);
                        }
                        break;
                    case TempaltePartMode.Literal:
                        if (level < ignore)
                            sb.Append(s.Key);
                        break;
                    case TempaltePartMode.Start:
                        level++;
                        if (level < ignore)
                        {
                            object v = GetValue(s.Key, data);
                            if (v == null || (v is string && ((string)v).Length == 0))
                                ignore = level;
                        }
                        break;
                    case TempaltePartMode.End:
                        level--;
                        if (level < ignore)
                            ignore = int.MaxValue;
                        break;
                    case TempaltePartMode.Call:
                        if (level < ignore)
                        {
                            string[] cmd = s.Key.Split(':');
                            if (cmd.Length != 2)
                                throw new Exception("Template call placeholder format is <value>:<template>");
                            object v;
                            if (cmd[0].Length > 0)
                            {
                                v = GetValue(cmd[0], data);
                                if (v == null)
                                    break;
                            }
                            else v = data;
                            Templater templ = WebUI.Templs[cmd[1]];
                            if (templ == null)
                                break;
                            if (v is Array)
                            {
                                foreach (object o in (Array)v)
                                    if (o != null)
                                        sb.Append(templ.Process(o));
                            }
                            else if (v is IList)
                            {
                                foreach (object o in (IList)v)
                                    if (o != null)
                                        sb.Append(templ.Process(o));
                            }
                            else
                                sb.Append(templ.Process(v));
                        }
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
