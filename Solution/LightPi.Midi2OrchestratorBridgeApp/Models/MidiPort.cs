using System;

namespace LightPi.Midi2OrchestratorBridgeApp.Models
{
    public class MidiPort
    {
        public MidiPort(int id, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }
    }
}
