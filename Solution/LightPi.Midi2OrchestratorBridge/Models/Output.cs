using System.Runtime.Serialization;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    [DataContract]
    public class Output
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Watts { get; set; }
    }
}
