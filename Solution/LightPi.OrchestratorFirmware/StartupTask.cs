using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using LightPi.Protocol;

namespace LightPi.OrchestratorFirmware
{
    public sealed class StartupTask : IBackgroundTask
    {
        private readonly AutoResetEvent _workerThreadEvent = new AutoResetEvent(false);
        private readonly byte[] _firstFrameBuffer = new byte[6];
        private readonly byte[] _secondFrameBuffer = new byte[6];

        private BackgroundTaskDeferral _deferral;

        private UdpEndpoint _udpEndpoint;
        private MAX7311Wrapper _max7311_1;
        private MAX7311Wrapper _max7311_2;
        private PCF8574Wrapper _pcf8574_1;
        private PCF8574Wrapper _pcf8574_2;
        
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var task = Task.Factory.StartNew(Run, CancellationToken.None, TaskCreationOptions.LongRunning,TaskScheduler.Default);
            task.ConfigureAwait(false);
        }

        private void Run()
        {
            try
            {
                Debug.WriteLine("Starting");

                InitializeI2CBus();
                InitializeUdpEndpoint();

                while (true)
                {
                    _workerThreadEvent.WaitOne();
                    ApplyFrame();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Unable to start. " + exception);
                _deferral.Complete();
            }
        }

        private void InitializeUdpEndpoint()
        {
            _udpEndpoint = new UdpEndpoint(EnqueueFrame);
            _udpEndpoint.Open();
        }
        
        private void EnqueueFrame(byte[] package)
        {
            byte[] frame;
            if (!LightPiProtocol.TryGetFrameFromPackage(package, out frame))
            {
                return;
            }

            // On sender side bit 0 means the first SSR from the first case. Bit 1 means the next one and so on.
            // Here all output bits are being reordered to match the physical output 0 to bit 0 state etc.
            byte[] reorderedPayload = new byte[LightPiProtocol.FrameLength];

            ByteExtensions.MoveBit(frame, 0, reorderedPayload, 15);
            ByteExtensions.MoveBit(frame, 1, reorderedPayload, 14);
            ByteExtensions.MoveBit(frame, 2, reorderedPayload, 13);
            ByteExtensions.MoveBit(frame, 3, reorderedPayload, 12);
            ByteExtensions.MoveBit(frame, 4, reorderedPayload, 11);
            ByteExtensions.MoveBit(frame, 5, reorderedPayload, 10);
            ByteExtensions.MoveBit(frame, 6, reorderedPayload, 9);
            ByteExtensions.MoveBit(frame, 7, reorderedPayload, 8);
            ByteExtensions.MoveBit(frame, 8, reorderedPayload, 0);
            ByteExtensions.MoveBit(frame, 9, reorderedPayload, 1);
            ByteExtensions.MoveBit(frame, 10, reorderedPayload, 2);
            ByteExtensions.MoveBit(frame, 11, reorderedPayload, 3);
            ByteExtensions.MoveBit(frame, 12, reorderedPayload, 4);
            ByteExtensions.MoveBit(frame, 13, reorderedPayload, 5);
            ByteExtensions.MoveBit(frame, 14, reorderedPayload, 6);
            ByteExtensions.MoveBit(frame, 15, reorderedPayload, 7);

            lock (_firstFrameBuffer)
            {
                Array.Copy(reorderedPayload, 0, _firstFrameBuffer, 0, _firstFrameBuffer.Length);
            }

            _workerThreadEvent.Set();
        }
        
        private void ApplyFrame()
        {
            // Move the frame from the first buffer to a second buffer to ensure that the endpoint can update the first buffer
            // while the frame is being written to the devices. Some frames maybe skipped but that is no problem because the
            // frames are written as fast as possible using the I2C bus. Frames which are received while writing are not enqueued and
            // written later because it will add a delay to the animation.
            lock (_firstFrameBuffer)
            {
                Array.Copy(_firstFrameBuffer, 0, _secondFrameBuffer, 0, _secondFrameBuffer.Length);
            }

            _max7311_1.WriteState(_secondFrameBuffer, 0);
            //_max7311_2.WriteState(_secondFrameBuffer, 2);

            _pcf8574_1.WriteState(_secondFrameBuffer, 4);
            //_pcf8574_2.WriteState(_secondFrameBuffer, 5);
        }

        private void InitializeI2CBus()
        {
            Debug.WriteLine("Initializing I2C bus");

            var i2cBusId = GetI2cBusId();
            
            _max7311_1 = new MAX7311Wrapper(i2cBusId, 0x10);
            _max7311_1.Initialize();

            _max7311_2 = new MAX7311Wrapper(i2cBusId, 0x11);
            //_max7311_2.Initialize();

            _pcf8574_1 = new PCF8574Wrapper(i2cBusId, 0x38);
            _pcf8574_1.Initialize();

            _pcf8574_2 = new PCF8574Wrapper(i2cBusId, 0x39);
            //_pcf8574_2.Initialize();

            Debug.WriteLine("I2C devices initialized");
        }

        private string GetI2cBusId()
        {
            var deviceSelector = I2cDevice.GetDeviceSelector();

            var deviceInformation = DeviceInformation.FindAllAsync(deviceSelector).AsTask().Result;
            if (deviceInformation.Count == 0)
            {
                throw new InvalidOperationException("I2C bus not found.");
            }

            var i2cBusId = deviceInformation.First().Id;
            Debug.WriteLine($"Found I2C bus $'{i2cBusId}'");

            return i2cBusId;
        }
    }
}
