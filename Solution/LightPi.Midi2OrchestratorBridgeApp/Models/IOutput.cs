using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    public interface IOutput
    {
        string Name { get; }

        void SetState(OutputState state, IOrchestratorService orchestratorService);
    }
}
