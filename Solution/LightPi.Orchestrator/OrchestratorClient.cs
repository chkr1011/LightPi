using System;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly UdpClient _udpClient = new UdpClient();
        private readonly string _hostName;

        public OrchestratorClient(string hostName)
        {
            if (hostName == null) throw new ArgumentNullException(nameof(hostName));

            _hostName = hostName;

            _udpClient.DontFragment = true;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);
        }

        public byte[] State { get; } = new byte[6];

        public void SetOutput(int id, bool state)
        {
            State.SetBit(id, state);
        }

        public void SendState()
        {
            byte[] package = LightPiProtocol.GeneratePackage(State);
            lock (_udpClient)
            {
                _udpClient.Send(package, package.Length, _hostName, LightPiProtocol.Port);
            }
        }

        public void SendState(byte[] state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            byte[] package = LightPiProtocol.GeneratePackage(state);
            lock (_udpClient)
            {
                _udpClient.Send(package, package.Length, _hostName, LightPiProtocol.Port);
            }
        }
    }
}
