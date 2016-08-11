using System;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    public class Output : IOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public void SetState(OutputState state, IOrchestratorService orchestratorService)
        {
            orchestratorService.SetOutputState(Id, state == OutputState.On);
        }
    }
}
