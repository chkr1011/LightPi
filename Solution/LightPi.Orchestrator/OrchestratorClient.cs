using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly object _syncRoot = new object();
        private readonly OrchestratorOutput[] _outputs = new OrchestratorOutput[LightPiProtocol.OutputsCount];
        private readonly UdpClient _udpClient = new UdpClient();
        private readonly IPEndPoint _ipEndPoint;
        
        private byte[] _previousSentState = new byte[0];

        public OrchestratorClient(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            _ipEndPoint = new IPEndPoint(ipAddress, LightPiProtocol.Port);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);
            
            InitializeOutputs();
        }
        
        public bool ForceCommits { get; set; }

        public void SetOutput(int id, bool state, SetOutputMode mode = SetOutputMode.IncrementDecrement)
        {
            if (id < 0 || id >= LightPiProtocol.OutputsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            lock (_syncRoot)
            {
                if (mode == SetOutputMode.IncrementDecrement)
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
                else if (mode == SetOutputMode.Set)
                {
                    if (state)
                    {
                        _outputs[id].Activate();
                    }
                    else
                    {
                        _outputs[id].Deactivate();
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public CommitChangesResult CommitChanges()
        {
            lock (_syncRoot)
            {
                var state = GenerateState();
                if (!ForceCommits && _previousSentState.SequenceEqual(state))
                {
                    return new CommitChangesResult(false, null);
                }

                _previousSentState = state;
                
                var package = LightPiProtocol.GeneratePackage(state);
                _udpClient.Send(package, package.Length, _ipEndPoint);
                
                return new CommitChangesResult(true, state);
            }
        }

        public CommitChangesResult Reset()
        {
            lock (_syncRoot)
            {
                foreach (var output in _outputs)
                {
                    output.Deactivate();
                }

                return CommitChanges();
            }
        }

        private void InitializeOutputs()
        {
            for (var i = 0; i < _outputs.Length; i++)
            {
                _outputs[i] = new OrchestratorOutput();
            }
        }

        private byte[] GenerateState()
        {
            ulong buffer = 0;
            for (var i = 0; i < _outputs.Length; i++)
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
