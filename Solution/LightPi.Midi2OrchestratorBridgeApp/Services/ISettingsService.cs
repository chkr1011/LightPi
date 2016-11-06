using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface ISettingsService
    {
        Settings Settings { get; }
        
        void Save();
    }
}