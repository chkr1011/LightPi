using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public Target Target { get; set; }

        [DataMember]
        public string OrchestratorAddress { get; set; }

        [DataMember]
        public string MidiIn { get; set; }

        [DataMember]
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();

        [DataMember]
        public Dictionary<int, string> Outputs { get; set; } = new Dictionary<int, string>();
    }
}
