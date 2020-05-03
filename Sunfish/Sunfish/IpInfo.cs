﻿using System.Net.NetworkInformation;

namespace DolphinWebXplorer2
{
    class IpInfo
    {
        private NetworkInterface iface;
        private string ip;

        public IpInfo(NetworkInterface iface, string ip)
        {
            this.iface = iface;
            this.ip = ip;
        }

        public string InterfaceName { get { return iface.Name; } }
        public NetworkInterfaceType InterfaceType { get { return iface.NetworkInterfaceType; } }
        public string Address { get { return ip; } }
    }
}
