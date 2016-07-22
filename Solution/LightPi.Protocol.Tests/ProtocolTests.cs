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
            byte[] package = LightPiProtocol.GeneratePackage(state);

            Assert.AreEqual(LightPiProtocol.PackageLength, package.Length);

            // Check the prefix
            for (int i = 0; i < LightPiProtocol.PackagePrefixLength; i++)
            {
                Assert.AreEqual(package[i], LightPiProtocol.PackagePrefix[i], "Prefix is invalid.");
            }

            // Check the state
            for (int i = 0; i < state.Length; i++)
            {
                Assert.AreEqual(state[i], package[i + LightPiProtocol.PackagePrefixLength], "State is invalid.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GeneratePackage_WithInvalidState()
        {
            var state = new byte[] { 1, 2, 4, 8, 16 };
            LightPiProtocol.GeneratePackage(state);
        }

        [TestMethod]
        public void TryGetState()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            byte[] package = LightPiProtocol.GeneratePackage(state);

            byte[] returnState;
            bool hasSucceeded = LightPiProtocol.TryGetState(package, out returnState);

            Assert.IsTrue(hasSucceeded);
            Assert.AreEqual(state.Length, returnState.Length);

            for (int i = 0; i < state.Length; i++)
            {
                Assert.AreEqual(state[i], returnState[i]);
            }
        }

        [TestMethod]
        public void TryGetState_WithInvalidPackagePrefix()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            byte[] package = LightPiProtocol.GeneratePackage(state);

            // Destroy the prefix of the package.
            package[0] = 32;

            byte[] returnState;
            bool hasSucceeded = LightPiProtocol.TryGetState(package, out returnState);

            Assert.IsFalse(hasSucceeded);
        }

        [TestMethod]
        public void TryGetState_WithInvalidPackageLength()
        {
            var state = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            byte[] package = LightPiProtocol.GeneratePackage(state);

            // Destroy the length of the package.
            Array.Resize(ref package, 9);

            byte[] returnState;
            bool hasSucceeded = LightPiProtocol.TryGetState(package, out returnState);

            Assert.IsFalse(hasSucceeded);
        }
    }
}
