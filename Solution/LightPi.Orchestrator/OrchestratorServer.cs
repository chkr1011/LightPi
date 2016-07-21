using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LightPi.Protocol;

namespace LightPi.Orchestrator
{
    public sealed class OrchestratorServer
    {
        private readonly UdpClient _datagramSocket = new UdpClient(new IPEndPoint(IPAddress.Any, LightPiProtocol.Port));
        private readonly Action<byte[]> _callback;
        
        public OrchestratorServer(Action<byte[]> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _callback = callback;
        }

        public void Start()
        {
            _datagramSocket.DontFragment = true;
            
            Task.Factory.StartNew(ProcessIncomingData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            
            Debug.WriteLine($"UDP endpoint opened on port {LightPiProtocol.Port}");
        }

        private void ProcessIncomingData()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remoteIPEndpoint = null;
                    byte[] package = _datagramSocket.Receive(ref remoteIPEndpoint);

                    byte[] state;
                    if (!LightPiProtocol.TryGetState(package, out state))
                    {
                        continue;
                    }

                    _callback(state);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.ToString());
                }
            }
        }
    }
}
