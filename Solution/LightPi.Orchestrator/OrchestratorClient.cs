using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public class OrchestratorClient
    {
        private readonly object _syncRoot = new object();
        private readonly OrchestratorOutput[] _outputs = new OrchestratorOutput[LightPiProtocol.OutputsCount];
        private readonly UdpClient _udpClient = new UdpClient();
        private readonly string _address;

        private byte[] _previousSentState = new byte[0];

        public OrchestratorClient(string address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));

            _address = address;

            _udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);

            InitializeOutputs();
        }
        
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
                if (_previousSentState.SequenceEqual(state))
                {
                    return new CommitChangesResult(false, null, TimeSpan.Zero);
                }

                _previousSentState = state;

                var stopwatch = Stopwatch.StartNew();

                var package = LightPiProtocol.GeneratePackage(state);
                _udpClient.Send(package, package.Length, _address, LightPiProtocol.Port);
                
                return new CommitChangesResult(true, state, stopwatch.Elapsed);
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
