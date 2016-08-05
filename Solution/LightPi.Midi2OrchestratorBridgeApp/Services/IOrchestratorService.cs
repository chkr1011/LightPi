using System.Net;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface IOrchestratorService
    {
        void AttachOrchestrator(IPAddress ipAddress);
        void SetOutputState(int id, bool state);
        void CommitChanges();
    }
}