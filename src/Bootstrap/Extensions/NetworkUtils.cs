using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Extensions
{
    public static class NetworkUtils
    {
        public static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public static int GetFreeTcpPortFrom(int startPort)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            var usingPorts = tcpConnInfoArray.Select(s => s.LocalEndPoint.Port).ToHashSet();
            for (var i = startPort; i < 65536; i++)
            {
                if (!usingPorts.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}