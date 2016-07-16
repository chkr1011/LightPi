using System;
using System.Diagnostics;
using Windows.Networking.Sockets;
using LightPi.Protocol;

namespace LightPi.OrchestratorFirmware
{
    internal sealed class UdpEndpoint
    {
        private readonly DatagramSocket _datagramSocket = new DatagramSocket();
        private readonly Action<byte[]> _callback;
        
        public UdpEndpoint(Action<byte[]> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _callback = callback;
        }

        public void Open()
        {
            // Ensure that the sockets works as fast as any possible to ensure that the animations are running fine.
            _datagramSocket.Control.DontFragment = true;
            _datagramSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;

            _datagramSocket.MessageReceived += ProcessIncomingData;
            _datagramSocket.BindServiceNameAsync(LightPiProtocol.Port.ToString()).AsTask().Wait();

            Debug.WriteLine($"UDP endpoint opened on port {LightPiProtocol.Port}");
        }

        private void ProcessIncomingData(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs message)
        {
            var package = ReadPackageFromMessage(message);
            if (package == null)
            {
                return;
            }

            _callback(package);
        }

        private byte[] ReadPackageFromMessage(DatagramSocketMessageReceivedEventArgs message)
        {
            using (var reader = message.GetDataReader())
            {
                if (reader.UnconsumedBufferLength != LightPiProtocol.PackageLength)
                {
                    // Ignore all invalid packages.
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
