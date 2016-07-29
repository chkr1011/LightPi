using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public Target Target { get; set; }

        [DataMember]
        public IPAddress OrchestratorAddress { get; set; }

        [DataMember]
        public string MidiIn { get; set; }

        [DataMember]
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();
    }
}
