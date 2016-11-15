using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    [DataContract]
    public class Mapping
    {
        [DataMember]
        public int Channel { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public int Octave { get; set; }

        [DataMember]
        public List<int> Outputs { get; set; } = new List<int>();

        [DataMember]
        public string Comment { get; set; }
    }
}
