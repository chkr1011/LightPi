using System;
using System.Diagnostics;
using Windows.Devices.I2c;

namespace LightPi.OrchestratorFirmware.Devices
{
    internal sealed class PCF8574 : IOutputDevice
    {
        private readonly byte[] _stateBuffer = { 0 };

        private readonly string _i2CBusId;
        private readonly int _deviceAddress;
        private readonly int _stateOffset;

        private I2cDevice _i2CDevice;

        public PCF8574(string i2CBusId, int deviceAddress, int stateOffset, byte initialState)
        {
            if (i2CBusId == null) throw new ArgumentNullException(nameof(i2CBusId));

            _stateBuffer[0] = initialState;

            _i2CBusId = i2CBusId;
            _deviceAddress = deviceAddress;
            _stateOffset = stateOffset;
        }

        public void Initialize()
        {
            var settings = new I2cConnectionSettings(_deviceAddress)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Exclusive
            };

            _i2CDevice = I2cDevice.FromIdAsync(_i2CBusId, settings).AsTask().Result;
            _i2CDevice.Write(_stateBuffer);

            Debug.WriteLine($"Initialized PCF8574 with address {_deviceAddress}");
        }

        public void WriteState(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            _stateBuffer[0] = buffer[_stateOffset];

            _i2CDevice.Write(_stateBuffer);
        }
    }
}
