using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightPi.Protocol.Tests
{
    [TestClass]
    public class ProtocolTests
    {
        [TestMethod]
        public void GeneratePackage()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var package = LightPiProtocol.CreatePackage(state);

            Assert.AreEqual(LightPiProtocol.PackageLength, package.Length);

            // Check the prefix
            for (var i = 0; i < LightPiProtocol.PackagePrefixLength; i++)
            {
                Assert.AreEqual(package[i], LightPiProtocol.PackagePrefix[i], "Prefix is invalid.");
            }

            // Check the state
            for (var i = 0; i < state.Length; i++)
            {
                Assert.AreEqual(state[i], package[i + LightPiProtocol.PackagePrefixLength], "State is invalid.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GeneratePackage_WithInvalidState()
        {
            var state = new byte[] { 1, 2, 4, 8, 16 };
            LightPiProtocol.CreatePackage(state);
        }

        [TestMethod]
        public void TryGetState()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var package = LightPiProtocol.CreatePackage(state);

            byte[] returnState;
            var hasSucceeded = LightPiProtocol.TryParsePackage(package, out returnState);

            Assert.IsTrue(hasSucceeded);
            Assert.AreEqual(state.Length, returnState.Length);

            for (var i = 0; i < state.Length; i++)
            {
                Assert.AreEqual(state[i], returnState[i]);
            }
        }

        [TestMethod]
        public void TryGetState_WithInvalidPackagePrefix()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var package = LightPiProtocol.CreatePackage(state);

            // Destroy the prefix of the package.
            package[0] = 32;

            byte[] returnState;
            var hasSucceeded = LightPiProtocol.TryParsePackage(package, out returnState);

            Assert.IsFalse(hasSucceeded);
        }

        [TestMethod]
        public void TryGetState_WithInvalidPackageLength()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var package = LightPiProtocol.CreatePackage(state);

            // Destroy the length of the package.
            Array.Resize(ref package, 9);

            byte[] returnState;
            var hasSucceeded = LightPiProtocol.TryParsePackage(package, out returnState);

            Assert.IsFalse(hasSucceeded);
        }
    }
}
