using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public string OrchestratorAddress { get; set; }

        [DataMember]
        public string MidiIn { get; set; }

        [DataMember]
        public Dictionary<int, Output> Outputs { get; set; } = new Dictionary<int, Output>();
    }
}
