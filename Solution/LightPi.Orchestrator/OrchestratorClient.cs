using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly object _syncRoot = new object();
        private readonly OrchestratorOutput[] _outputs = new OrchestratorOutput[LightPiProtocol.StateLength*8];

        private readonly UdpClient _udpClient = new UdpClient();
        private readonly IPEndPoint _ipEndPoint;

        private byte[] _previousSentState; 

        public OrchestratorClient(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            _ipEndPoint = new IPEndPoint(ipAddress, LightPiProtocol.Port);

            _udpClient.DontFragment = true;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);

            InitializeOutputs();
        }
        
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

        public CommitChangesResult CommitChanges()
        {
            lock (_syncRoot)
            {
                var state = GenerateState();
                if (_previousSentState != null)
                {
                    if (_previousSentState.SequenceEqual(state))
                    {
                        return new CommitChangesResult(false, null, TimeSpan.Zero);
                    }
                }

                var stopwatch = Stopwatch.StartNew();

                var package = LightPiProtocol.GeneratePackage(state);
                _udpClient.Send(package, package.Length, _ipEndPoint);

                _previousSentState = state;

                return new CommitChangesResult(true, state, stopwatch.Elapsed);
            }
        }

        private void InitializeOutputs()
        {
            for (int i = 0; i < _outputs.Length; i++)
            {
                _outputs[i] = new OrchestratorOutput();
            }
        }

        private byte[] GenerateState()
        {
            ulong buffer = 0;
            for (int i = 0; i < _outputs.Length; i++)
            {
                if (_outputs[i].IsActive())
                {
                    buffer |= (ulong)0x1 << i;
                }
            }

            return BitConverter.GetBytes(buffer);
        }
    }
}
