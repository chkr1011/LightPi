using System;
using System.Net;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly UdpClient _udpClient = new UdpClient();
        private readonly IPEndPoint _ipEndPoint;

        public OrchestratorClient(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            _ipEndPoint = new IPEndPoint(ipAddress, LightPiProtocol.Port);
            
            _udpClient.DontFragment = true;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);
        }

        public void SendFrame(byte[] frame)
        {
            byte[] package = LightPiProtocol.GeneratePackage(frame);
            lock (_udpClient)
            {
                _udpClient.Send(package, package.Length, _ipEndPoint);
            }
        }
    }
}
