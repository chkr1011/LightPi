using System;

namespace LightPi.OrchestratorFirmware.Core
{
    internal sealed class StateConverter
    {
        private readonly byte[] _state;

        private ulong _source;
        private ulong _target;

        public StateConverter(byte[] state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            _state = state;
        }

        public byte[] Convert()
        {
            // On sender side bit 0 means the first relay (SSR) from the first case. Bit 1 means the next one and so on.
            // Here all output bits are being reordered to match the physical output 0 to bit 0 state etc.
            _source = BitConverter.ToUInt64(_state, 0);

            // MAX7311 - 1 (HSPE16)
            // The SSRs at the HSPE16 are controlled via GND. They are inverted (HIGH = OFF, LOW = ON).
            var offset = 0;
            MoveBit(0, offset + 15, MoveBitMode.Invert);
            MoveBit(1, offset + 14, MoveBitMode.Invert);
            MoveBit(2, offset + 13, MoveBitMode.Invert);
            MoveBit(3, offset + 12, MoveBitMode.Invert);
            MoveBit(4, offset + 11, MoveBitMode.Invert);
            MoveBit(5, offset + 10, MoveBitMode.Invert);
            MoveBit(6, offset + 9, MoveBitMode.Invert);
            MoveBit(7, offset + 8, MoveBitMode.Invert);
            MoveBit(8, offset + 0, MoveBitMode.Invert);
            MoveBit(9, offset + 1, MoveBitMode.Invert);
            MoveBit(10, offset + 2, MoveBitMode.Invert);
            MoveBit(11, offset + 3, MoveBitMode.Invert);
            MoveBit(12, offset + 4, MoveBitMode.Invert);
            MoveBit(13, offset + 5, MoveBitMode.Invert);
            MoveBit(14, offset + 6, MoveBitMode.Invert);
            MoveBit(15, offset + 7, MoveBitMode.Invert);

            // PCF8574 - 1 (HSRel5+3)
            // For GPIO 0-4 inversion is required because the HSRel5 from CCTools has its own hardware inverter.
            // The 3 GPIOs are not used. So inversion for them causes no problems.
            offset = 2 * 8;
            MoveBit(16, offset + 0, MoveBitMode.Invert);
            MoveBit(17, offset + 1, MoveBitMode.Invert);
            MoveBit(18, offset + 2, MoveBitMode.Invert);
            MoveBit(19, offset + 3, MoveBitMode.Invert);
            MoveBit(20, offset + 4, MoveBitMode.Invert);

            // MAX7311 - 2 (HSPE16)
            // The SSRs at the HSPE16 are controlled via GND. They are inverted (HIGH = OFF, LOW = ON).
            offset = 3 * 8;
            MoveBit(21, offset + 15, MoveBitMode.Invert);
            MoveBit(22, offset + 14, MoveBitMode.Invert);
            MoveBit(23, offset + 13, MoveBitMode.Invert);
            MoveBit(24, offset + 12, MoveBitMode.Invert);
            MoveBit(25, offset + 11, MoveBitMode.Invert);
            MoveBit(26, offset + 10, MoveBitMode.Invert);
            MoveBit(27, offset + 9, MoveBitMode.Invert);
            MoveBit(28, offset + 8, MoveBitMode.Invert);
            MoveBit(29, offset + 0, MoveBitMode.Invert);
            MoveBit(30, offset + 1, MoveBitMode.Invert);
            MoveBit(31, offset + 2, MoveBitMode.Invert);
            MoveBit(32, offset + 3, MoveBitMode.Invert);
            MoveBit(33, offset + 4, MoveBitMode.Invert);
            MoveBit(34, offset + 5, MoveBitMode.Invert);
            MoveBit(35, offset + 6, MoveBitMode.Invert);
            MoveBit(36, offset + 7, MoveBitMode.Invert);

            // MAX7311 - 3 (HSRel8+8)
            offset = 5 * 8;

            // The 4 additional SSRs
            MoveBit(37, offset + 10, MoveBitMode.Invert);
            MoveBit(38, offset + 11, MoveBitMode.Invert);
            MoveBit(39, offset + 12, MoveBitMode.Invert);
            MoveBit(40, offset + 13, MoveBitMode.Invert);

            // The 8 regular relays
            MoveBit(41, offset + 0, MoveBitMode.Default);
            MoveBit(42, offset + 1, MoveBitMode.Default);
            MoveBit(43, offset + 2, MoveBitMode.Default);
            MoveBit(44, offset + 3, MoveBitMode.Default);
            MoveBit(45, offset + 4, MoveBitMode.Default);
            MoveBit(46, offset + 5, MoveBitMode.Default);
            MoveBit(47, offset + 6, MoveBitMode.Default);
            MoveBit(48, offset + 7, MoveBitMode.Default);

            // The 2 additional relays
            MoveBit(49, offset + 14, MoveBitMode.Default);
            MoveBit(50, offset + 15, MoveBitMode.Default);

            return BitConverter.GetBytes(_target);
        }

        private void MoveBit(int sourceIndex, int targetIndex, MoveBitMode mode)
        {
            var isActive = (_source & (ulong)0x1 << sourceIndex) > 0;

            if (mode == MoveBitMode.Invert)
            {
                isActive = !isActive;
            }

            if (isActive)
            {
                _target |= (ulong)0x1 << targetIndex;
            }
        }
    }
}
