using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightPi.Orchestrator.Tests
{
    [TestClass]
    public class OrchestratorClientTests
    {
        [TestMethod]
        public void SendEmptyState()
        {
            var expectedState = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var client = new OrchestratorClient(IPAddress.Loopback.ToString());
            var sendStateResult = client.CommitChanges();

            bool statesMatching = sendStateResult.State.SequenceEqual(expectedState);

            Assert.IsTrue(statesMatching);
        }

        [TestMethod]
        public void SendCorrectState_WithSingleOutput()
        {
            var expectedState = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 };
            var client = new OrchestratorClient(IPAddress.Loopback.ToString());
            client.SetOutput(0, true);
            var sendStateResult = client.CommitChanges();

            bool statesMatching = sendStateResult.State.SequenceEqual(expectedState);

            Assert.IsTrue(statesMatching);
        }

        [TestMethod]
        public void SendCorrectState_WithMultipleOutputs()
        {
            var expectedState = new byte[] { 3, 0, 0, 0, 0, 128, 0, 0 };
            var client = new OrchestratorClient(IPAddress.Loopback.ToString());
            client.SetOutput(0, true);
            client.SetOutput(1, true);
            client.SetOutput(47, true);
            var sendStateResult = client.CommitChanges();

            bool statesMatching = sendStateResult.State.SequenceEqual(expectedState);

            Assert.IsTrue(statesMatching);
        }

        [TestMethod]
        public void SendCorrectState_WithSingleOutputMultipleTimes()
        {
            var expectedState = new byte[] { 3, 0, 0, 0, 0, 128, 0, 0 };
            var client = new OrchestratorClient(IPAddress.Loopback.ToString());
            client.SetOutput(0, true);

            client.SetOutput(1, true);
            client.SetOutput(1, true);
            client.SetOutput(1, false);

            client.SetOutput(47, true);
            var sendStateResult = client.CommitChanges();

            bool statesMatching = sendStateResult.State.SequenceEqual(expectedState);

            Assert.IsTrue(statesMatching);
        }
    }
}
