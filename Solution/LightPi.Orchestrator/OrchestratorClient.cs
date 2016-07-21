using System;
using System.Net;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly object _syncRoot = new object();
        private readonly OrchestratorOutput[] _outputs = new OrchestratorOutput[6*8];

        private readonly UdpClient _udpClient = new UdpClient();
        private readonly IPEndPoint _ipEndPoint;

        public OrchestratorClient(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            _ipEndPoint = new IPEndPoint(ipAddress, LightPiProtocol.Port);

            _udpClient.DontFragment = true;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);

            InitializeOutputs();
        }

        public byte[] LastSentState { get; private set; }

        public void SetOutput(int id, bool state)
        {
            lock (_syncRoot)
            {
                if (state)
                {
                    _outputs[id].Increment();
                }
                else
                {
                    _outputs[id].Decrement();
                }
            }
        }

        public void SendState()
        {
            lock (_syncRoot)
            {
                var state = new byte[6];
                for (int i = 0; i < _outputs.Length; i++)
                {
                    if (_outputs[i].IsActive())
                    {
                        state.SetBit(i, true);
                    }
                }

                var package = LightPiProtocol.GeneratePackage(state);
                _udpClient.Send(package, package.Length, _ipEndPoint);

                LastSentState = state;
            }
        }

        private void InitializeOutputs()
        {
            for (int i = 0; i < 48; i++)
            {
                _outputs[i] = new OrchestratorOutput();
            }
        }
    }
}
