using System;
using System.Linq;
using System.Text;

namespace LightPi.Protocol
{
    public static class LightPiProtocol
    {
        public static readonly int FrameLength = 6;
        public static readonly byte[] PackagePrefix = Encoding.ASCII.GetBytes("LIGHT_PI");
        public static readonly int PackageLength = PackagePrefix.Length + FrameLength;
        
        public static bool TryGetFrameFromPackage(byte[] package, out byte[] frame)
        {
            frame = null;

            if (package.Length != PackageLength)
            {
                return false;
            }

            byte[] prefix = new byte[PackagePrefix.Length];
            Array.Copy(package, 0, prefix, 0, PackagePrefix.Length);

            bool prefixIsMatching = prefix.SequenceEqual(PackagePrefix);
            if (!prefixIsMatching)
            {
                return false;
            }
            
            frame = new byte[FrameLength];
            Array.Copy(package, PackagePrefix.Length, frame, 0, frame.Length);

            return true;
        }

        public static byte[] GeneratePackage(byte[] frame)
        {
            byte[] buffer = new byte[PackageLength];
            Array.Copy(PackagePrefix, 0, buffer, 0, PackagePrefix.Length);
            Array.Copy(frame, 0, buffer, PackagePrefix.Length, frame.Length);

            return buffer;
        }
    }
}
