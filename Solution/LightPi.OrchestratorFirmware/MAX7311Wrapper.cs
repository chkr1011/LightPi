using System;
using System.Diagnostics;
using Windows.Devices.I2c;

namespace LightPi.OrchestratorFirmware
{
    internal sealed class MAX7311Wrapper
    {
        private const int OutputRegister1 = 2;
        private const int ConfigurationRegister1 = 6;

        private readonly byte[] _stateBuffer = { OutputRegister1, 255, 255 };

        private readonly string _i2CBusId;
        private readonly int _deviceAddress;

        private I2cDevice _i2cDevice;

        public MAX7311Wrapper(string i2cBusId, int deviceAddress)
        {
            _i2CBusId = i2cBusId;
            _deviceAddress = deviceAddress;
        }

        public void Initialize()
        {
            var settings = new I2cConnectionSettings(_deviceAddress)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Exclusive
            };

            _i2cDevice = I2cDevice.FromIdAsync(_i2CBusId, settings).AsTask().Result;

            // Write to the output registers and set all GPIOs to HIGH which means "Off" (inverted!)
            // Write to the configuration registers and set the operation mode to "output".
            _i2cDevice.Write(_stateBuffer);

            byte[] setConfigurationToOutput = { ConfigurationRegister1, 0, 0 };
            _i2cDevice.Write(setConfigurationToOutput);

            Debug.WriteLine($"Initialized MAX7311 with address {_deviceAddress}");
        }

        public void WriteState(byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            
            // Invert the states here because '1' means 'On' in terms of light is on.
            _stateBuffer[1] = (byte)~buffer[offset];
            _stateBuffer[2] = (byte)~buffer[offset + 1];

            _i2cDevice.Write(_stateBuffer);
        }
    }
}
