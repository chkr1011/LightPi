using System;
using System.Diagnostics;
using Windows.Devices.I2c;

namespace LightPi.OrchestratorFirmware.Devices
{
    internal sealed class MAX7311 : IOutputDevice
    {
        private const int OutputRegister1 = 2;
        private const int ConfigurationRegister1 = 6;

        private readonly byte[] _stateBuffer = { OutputRegister1, 255, 255 };

        private readonly string _i2CBusId;
        private readonly int _deviceAddress;
        private readonly int _stateOffset;

        private I2cDevice _i2CDevice;

        public MAX7311(string i2CBusId, int deviceAddress, int stateOffset, byte[] initialState)
        {
            if (i2CBusId == null) throw new ArgumentNullException(nameof(i2CBusId));
            if (initialState == null) throw new ArgumentNullException(nameof(initialState));

            _stateBuffer[1] = initialState[0];
            _stateBuffer[2] = initialState[1];

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

            // Write the initial state to the output register. Then set the configuration to "output".
            _i2CDevice.Write(_stateBuffer);

            byte[] setConfigurationToOutput = { ConfigurationRegister1, 0, 0 };
            _i2CDevice.Write(setConfigurationToOutput);

            Debug.WriteLine($"Initialized MAX7311 with address {_deviceAddress}");
        }

        public void WriteState(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            _stateBuffer[1] = buffer[_stateOffset];
            _stateBuffer[2] = buffer[_stateOffset + 1];

            _i2CDevice.Write(_stateBuffer);
        }
    }
}
