using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LightPi.OrchestratorEmulator.Settings
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public string OrchestratorAddress { get; set; } = "LightPi";

        [DataMember]
        public List<OutputDefinition> OutputDefinitions { get; set; } = new List<OutputDefinition>();
    }
}
