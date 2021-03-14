using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetFwTypeLib;
using System.Reflection;
using System.Security.Cryptography;

namespace Sunfish.Middleware
{
    public class WindowsFirewall
    {
        private static INetFwPolicy2 FirewallPolicy()
        {
            return (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        }

        public static WindowsFirewallRule Allow(int port)
        {
            string path=Assembly.GetExecutingAssembly().Location;
            INetFwRule firewallRule = CreateRule();
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "Used to allow SunFish on "+path;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "SunFish ("+port+") ["+idid(path)+"]";
            firewallRule.ApplicationName = Assembly.GetExecutingAssembly().Location;
            firewallRule.LocalPorts = port.ToString();
            FirewallPolicy().Rules.Add(firewallRule);
            return new WindowsFirewallRule(firewallRule);
        }

        public static void Remove(WindowsFirewallRule rule)
        {
            if (rule == null)
                return;
            RemovePolicy(rule.Name);
        }

        private static INetFwRule CreateRule()
        {
            return (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
        }

        private static void RemovePolicy(string id)
        {
            FirewallPolicy().Rules.Remove(id);
        }

        private static string idid(string id)
        {
            SHA1 sha1 = SHA1.Create();
            byte [] hash=sha1.ComputeHash(Encoding.UTF8.GetBytes(id));
            return Convert.ToBase64String(hash);
        }
    }

    public class WindowsFirewallRule
    {
        private INetFwRule rule;
        private string name;

        internal WindowsFirewallRule(INetFwRule rule)
        {
            this.rule = rule;
            name = rule.Name;
        }

        public string Name { get { return name; } }
    }
}
