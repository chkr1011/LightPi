using System;
using System.Diagnostics;
using Windows.Devices.I2c;

namespace LightPi.OrchestratorFirmware
{
    internal sealed class PCF8574Wrapper
    {
        private readonly byte[] _stateBuffer = { 0 };

        private readonly string _i2CBusId;
        private readonly int _deviceAddress;

        private I2cDevice _i2cDevice;

        public PCF8574Wrapper(string i2CBusId, int deviceAddress)
        {
            _i2CBusId = i2CBusId;
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

            // Write the actual state to the device. The GPIOs for the relays are not inverted due to an existing hardware inverter.
            // The not used 3 GPIOs are not used at this project. We ignore them.
            byte[] setAllGPIOsToHigh= { 255 };
            _i2cDevice.Write(setAllGPIOsToHigh);

            Debug.WriteLine($"Initialized PCF8574 with address {_deviceAddress}");
        }

        public void WriteState(byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            // For GPIO 0-4 inversion is required because the HSRel5 from CCTools has its own hardware inverter.
            _stateBuffer[0] = (byte)~buffer[offset];

            _i2cDevice.Write(_stateBuffer);
        }
    }
}
