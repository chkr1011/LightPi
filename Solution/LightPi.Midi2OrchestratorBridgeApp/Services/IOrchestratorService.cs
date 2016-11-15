using System;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface IOrchestratorService
    {
        event EventHandler<ChangesCommittedEventArgs> ChangesCommitted;
         
        void SetOutputState(int id, bool state);
        void CommitChanges();
        void Initialize();
    }
}