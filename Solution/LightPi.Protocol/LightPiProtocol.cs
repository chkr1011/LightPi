using System;
using System.Text;

namespace LightPi.Protocol
{
    public static class LightPiProtocol
    {
        public static readonly int Port = 12345;
        public static readonly int StateLength = 6;

        public static readonly byte[] PackagePrefix = Encoding.ASCII.GetBytes("LIGHT_PI");
        public static readonly int PackageLength = PackagePrefix.Length + StateLength;
        
        public static bool TryGetState(byte[] package, out byte[] state)
        {
            state = null;

            if (package.Length != PackageLength)
            {
                return false;
            }

            if (!PrefixIsMatching(package))
            {
                return false;
            }
            
            state = new byte[StateLength];
            Array.Copy(package, PackagePrefix.Length, state, 0, state.Length);

            return true;
        }

        public static byte[] GeneratePackage(byte[] state)
        {
            var buffer = new byte[PackageLength];
            Array.Copy(PackagePrefix, 0, buffer, 0, PackagePrefix.Length);
            Array.Copy(state, 0, buffer, PackagePrefix.Length, state.Length);

            return buffer;
        }

        private static bool PrefixIsMatching(byte[] package)
        {
            for (int i = 0; i < PackagePrefix.Length; i++)
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
