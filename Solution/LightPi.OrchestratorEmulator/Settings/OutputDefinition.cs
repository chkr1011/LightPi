using System.Runtime.Serialization;

namespace LightPi.OrchestratorEmulator.Settings
{
    [DataContract]
    public class OutputDefinition
    {
        public OutputDefinition(int id, double watts)
        {
            Id = id;
            Watts = watts;
        }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public double Watts { get; private set; }
    }
}