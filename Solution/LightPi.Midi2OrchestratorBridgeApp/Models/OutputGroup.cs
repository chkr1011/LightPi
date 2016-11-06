using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    public class OutputGroup : IOutput
    {
        public string Name { get; set; }
        public List<IOutput> Outputs { get; set; } = new List<IOutput>();

        public void SetState(OutputState state, IOrchestratorService orchestratorService)
        {
            foreach (var output in Outputs)
            {
                output.SetState(state, orchestratorService);
            }
        }
    }
}
