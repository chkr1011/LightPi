using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using LightPi.OrchestratorFirmware.Devices;
using LightPi.Protocol;

namespace LightPi.OrchestratorFirmware.Core
{
    internal class Engine
    {
        private readonly AutoResetEvent _workerThreadEvent = new AutoResetEvent(false);
        private readonly List<IOutputDevice> _outputDevices = new List<IOutputDevice>();

        private readonly byte[] _firstFrameBuffer = new byte[LightPiProtocol.StateLength];
        private readonly byte[] _secondFrameBuffer = new byte[LightPiProtocol.StateLength];

        private readonly byte[] _state = new byte[LightPiProtocol.StateLength];
        
        public byte[] GetState()
        {
            lock (_state)
            {
                return (byte[])_state.Clone();
            }
        }

        public void EnqueueState(byte[] state)
        {
            lock (_firstFrameBuffer)
            {
                Array.Copy(state, 0, _firstFrameBuffer, 0, _firstFrameBuffer.Length);
            }

            _workerThreadEvent.Set();
        }

        public void Start()
        {
            Debug.WriteLine("Initializing I2C bus");

            var i2CBusId = GetI2CBusId();

            // The initial state will set all devices to OFF (respecting the inversions)
            _outputDevices.Add(new MAX7311(i2CBusId, 16, 0, new byte[] { 255, 255 }));
            _outputDevices.Add(new PCF8574(i2CBusId, 56, 2, 255));
            _outputDevices.Add(new MAX7311(i2CBusId, 20, 3, new byte[] { 255, 255 }));
            _outputDevices.Add(new MAX7311(i2CBusId, 25, 5, new byte[] { 0, 60 }));

            foreach (var outputDevice in _outputDevices)
            {
                outputDevice.Initialize();
            }

            // Apply the current frame which is set to 0x0000000000000... by default.
            ApplyFrame();

            Debug.WriteLine("I2C devices initialized");
        }

        public void Run()
        {
            while (true)
            {
                _workerThreadEvent.WaitOne();
                ApplyFrame();
            }
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

            lock (_state)
            {
                _secondFrameBuffer.CopyTo(_state, 0);
            }

            var buffer = new StateConverter(_secondFrameBuffer).Convert();
            foreach (var outputDevice in _outputDevices)
            {
                outputDevice.WriteState(buffer);
            }
        }

        private static string GetI2CBusId()
        {
            var deviceSelector = I2cDevice.GetDeviceSelector();

            var deviceInformation = DeviceInformation.FindAllAsync(deviceSelector).AsTask().Result;
            if (deviceInformation.Count == 0)
            {
                throw new InvalidOperationException("I2C bus not found.");
            }

            return deviceInformation.First().Id;
        }
    }
}
