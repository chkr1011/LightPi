using System;
using System.Runtime.CompilerServices;

namespace LightPi.Protocol
{
    public static class ByteExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(this byte @byte, int index)
        {
            if (index > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (@byte & (0x1 << index)) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(this byte[] bytes, int index)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            
            var byteOffset = index / 8;
            var bitOffset = index % 8;

            return GetBit(bytes[byteOffset], bitOffset);
        }
    }
}
