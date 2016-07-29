using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    [DataContract]
    public class Mapping
    {
        [DataMember]
        public MidiChannel Channel{ get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public int Output { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
