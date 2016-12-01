using System;
using System.Diagnostics;
using Windows.Networking.Sockets;
using LightPi.OrchestratorFirmware.Core;
using LightPi.Protocol;

namespace LightPi.OrchestratorFirmware.Udp
{
    internal sealed class UdpServer
    {
        private readonly DatagramSocket _datagramSocket = new DatagramSocket();
        private readonly Engine _engine;
        
        public UdpServer(Engine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            _engine = engine;
        }

        public void Start()
        {
            // Ensure that the sockets works as fast as any possible to ensure that the animations are running fine.
            _datagramSocket.Control.DontFragment = true;
            _datagramSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;

            _datagramSocket.MessageReceived += ProcessMessage;
            _datagramSocket.BindServiceNameAsync(LightPiProtocol.Port.ToString()).AsTask().Wait();

            Debug.WriteLine($"UDP endpoint opened on port {LightPiProtocol.Port}");
        }

        private void ProcessMessage(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs message)
        {
            var package = ReadPackageFromMessage(message);
            if (package == null)
            {
                return;
            }

            byte[] state;
            if (!LightPiProtocol.TryParsePackage(package, out state))
            {
                return;
            }

            _engine.EnqueueState(state);
        }

        private static byte[] ReadPackageFromMessage(DatagramSocketMessageReceivedEventArgs message)
        {
            using (var reader = message.GetDataReader())
            {
                if (reader.UnconsumedBufferLength != LightPiProtocol.PackageLength)
                {
                    // Ignore all messages with a wrong package size.
                    Debug.WriteLine($"Received invalid message with a length of {reader.UnconsumedBufferLength} bytes");
                    return null;
                }
                
                var buffer = new byte[LightPiProtocol.PackageLength];
                reader.ReadBytes(buffer);

                return buffer;
            }
        }
    }
}
