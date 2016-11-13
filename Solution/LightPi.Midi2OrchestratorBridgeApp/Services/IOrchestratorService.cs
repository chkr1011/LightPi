namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface IOrchestratorService
    {
        void SetOutputState(int id, bool state);
        void CommitChanges();
        void Initialize();
    }
}