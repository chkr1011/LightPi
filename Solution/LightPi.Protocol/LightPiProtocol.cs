using System;
using System.Text;

namespace LightPi.Protocol
{
    public static class LightPiProtocol
    {
        public static readonly int Port = 12345;

        public static readonly int StateLength = 8;

        public static readonly int OutputsCount = 51;

        public static readonly byte[] PackagePrefix = Encoding.ASCII.GetBytes("LIGHT_PI");
        public static readonly int PackagePrefixLength = PackagePrefix.Length;
        public static readonly int PackageLength = PackagePrefixLength + StateLength;

        public static bool TryGetState(byte[] package, out byte[] state)
        {
            state = null;

            if (package?.Length != PackageLength)
            {
                return false;
            }

            if (!PrefixIsMatching(package))
            {
                return false;
            }
            
            state = new byte[StateLength];
            Array.Copy(package, PackagePrefixLength, state, 0, StateLength);
            
            return true;
        }

        public static byte[] GeneratePackage(byte[] state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (state.Length != StateLength)
            {
                throw new ArgumentException("State length is invalid");   
            }

            var buffer = new byte[PackageLength];

            Array.Copy(PackagePrefix, 0, buffer, 0, PackagePrefixLength);
            Array.Copy(state, 0, buffer, PackagePrefixLength, StateLength);

            return buffer;
        }

        private static bool PrefixIsMatching(byte[] package)
        {
            for (int i = 0; i < PackagePrefixLength; i++)
            {
                if (!package[i].Equals(PackagePrefix[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
