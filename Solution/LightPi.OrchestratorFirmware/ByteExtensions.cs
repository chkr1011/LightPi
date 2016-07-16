using System;

namespace LightPi.Protocol
{
    internal static class ByteExtensions
    {
        public static bool GetBit(this byte @byte, int index)
        {
            if (index > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (@byte & (0x1 << index)) > 0;
        }

        public static bool GetBit(this byte[] bytes, int index)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            
            var byteOffset = index / 8;
            var bitOffset = index % 8;

            return GetBit(bytes[byteOffset], bitOffset);
        }

        public static byte SetBit(this byte @byte, int index, bool state)
        {
            if (index > 7) throw new ArgumentOutOfRangeException(nameof(index));

            if (state)
            {
                // Byte: 01010101
                // Mask: 00000010 (00000001 is moved left)
                // Combined with "|" (or) this will result in:
                //       01010111
                return (byte)((0x1 << index) | @byte);
            }

            // Byte: 01010101
            // Mask: 11111011 (00000001 is moved left and then negated using "~")
            // Combined with "&" (and) this will result in:
            //       01010001
            return (byte)(~(0x1 << index) & @byte);
        }

        public static void SetBit(this byte[] bytes, int index, bool state)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            
            var byteOffset = index / 8;
            var bitOffset = index % 8;

            bytes[byteOffset] = SetBit(bytes[byteOffset], bitOffset, state);
        }
    }
}
