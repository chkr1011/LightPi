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

        private readonly byte[] _firstFrameBuffer = new byte[LightPiProtocol.StateLength];
        private readonly byte[] _secondFrameBuffer = new byte[LightPiProtocol.StateLength];

        private BackgroundTaskDeferral _deferral;

        private OrchestratorServer _server;
        private MAX7311Wrapper _max7311_1;
        private MAX7311Wrapper _max7311_2;
        private PCF8574Wrapper _pcf8574_1;
        private PCF8574Wrapper _pcf8574_2;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var task = Task.Factory.StartNew(Run, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
            _server = new OrchestratorServer(EnqueueFrame);
            _server.Start();
        }

        private void EnqueueFrame(byte[] package)
        {
            byte[] state;
            if (!LightPiProtocol.TryGetState(package, out state))
            {
                return;
            }

            // On sender side bit 0 means the first relay (SSR) from the first case. Bit 1 means the next one and so on.
            // Here all output bits are being reordered to match the physical output 0 to bit 0 state etc.
            ulong sourceState = BitConverter.ToUInt64(state, 0);
            ulong targetState = 0;

            // MAX7311 - 1
            int offset = 0;
            targetState = MoveBit(sourceState, 0, targetState, offset + 15);
            targetState = MoveBit(sourceState, 1, targetState, offset + 14);
            targetState = MoveBit(sourceState, 2, targetState, offset + 13);
            targetState = MoveBit(sourceState, 3, targetState, offset + 12);
            targetState = MoveBit(sourceState, 4, targetState, offset + 11);
            targetState = MoveBit(sourceState, 5, targetState, offset + 10);
            targetState = MoveBit(sourceState, 6, targetState, offset + 9);
            targetState = MoveBit(sourceState, 7, targetState, offset + 8);
            targetState = MoveBit(sourceState, 8, targetState, offset + 0);
            targetState = MoveBit(sourceState, 9, targetState, offset + 1);
            targetState = MoveBit(sourceState, 10, targetState, offset + 2);
            targetState = MoveBit(sourceState, 11, targetState, offset + 3);
            targetState = MoveBit(sourceState, 12, targetState, offset + 4);
            targetState = MoveBit(sourceState, 13, targetState, offset + 5);
            targetState = MoveBit(sourceState, 14, targetState, offset + 6);
            targetState = MoveBit(sourceState, 15, targetState, offset + 7);

            // PCF8574 - 1
            offset = 2 * 8;
            targetState = MoveBit(sourceState, 16, targetState, offset + 0);
            targetState = MoveBit(sourceState, 17, targetState, offset + 1);
            targetState = MoveBit(sourceState, 18, targetState, offset + 2);
            targetState = MoveBit(sourceState, 19, targetState, offset + 3);
            targetState = MoveBit(sourceState, 20, targetState, offset + 4);

            // MAX7311 - 2
            offset = 3 * 8;
            targetState = MoveBit(sourceState, 21, targetState, offset + 15);
            targetState = MoveBit(sourceState, 22, targetState, offset + 14);
            targetState = MoveBit(sourceState, 23, targetState, offset + 13);
            targetState = MoveBit(sourceState, 24, targetState, offset + 12);
            targetState = MoveBit(sourceState, 25, targetState, offset + 11);
            targetState = MoveBit(sourceState, 26, targetState, offset + 10);
            targetState = MoveBit(sourceState, 27, targetState, offset + 9);
            targetState = MoveBit(sourceState, 28, targetState, offset + 8);
            targetState = MoveBit(sourceState, 29, targetState, offset + 0);
            targetState = MoveBit(sourceState, 30, targetState, offset + 1);
            targetState = MoveBit(sourceState, 31, targetState, offset + 2);
            targetState = MoveBit(sourceState, 32, targetState, offset + 3);
            targetState = MoveBit(sourceState, 33, targetState, offset + 4);
            targetState = MoveBit(sourceState, 34, targetState, offset + 5);
            targetState = MoveBit(sourceState, 35, targetState, offset + 6);
            targetState = MoveBit(sourceState, 36, targetState, offset + 7);

            // PCF8574 - 2
            offset = 5 * 8;
            targetState = MoveBit(sourceState, 16, targetState, offset + 0);
            targetState = MoveBit(sourceState, 17, targetState, offset + 1);
            targetState = MoveBit(sourceState, 18, targetState, offset + 2);
            targetState = MoveBit(sourceState, 19, targetState, offset + 3);
            targetState = MoveBit(sourceState, 20, targetState, offset + 4);

            byte[] reorderedState = BitConverter.GetBytes(targetState);
            lock (_firstFrameBuffer)
            {
                Array.Copy(reorderedState, 0, _firstFrameBuffer, 0, _firstFrameBuffer.Length);
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
            _pcf8574_1.WriteState(_secondFrameBuffer, 2);
            //_max7311_2.WriteState(_secondFrameBuffer, 3);
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

        private ulong MoveBit(ulong source, int sourceIndex, ulong target, int targetIndex)
        {
            bool isActive = (source & (ulong)0x1 << sourceIndex) > 0;
            if (isActive)
            {
                target |= (ulong)0x1 << targetIndex;
            }

            return target;
        }
    }
}
