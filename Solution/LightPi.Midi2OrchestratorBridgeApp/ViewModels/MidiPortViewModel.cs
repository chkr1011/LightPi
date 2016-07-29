using System;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class MidiPortViewModel
    {
        public MidiPortViewModel(MidiPort midiPort, bool isSelected)
        {
            if (midiPort == null) throw new ArgumentNullException(nameof(midiPort));

            MidiPort = midiPort;
            IsSelected = isSelected;
        }

        public MidiPort MidiPort { get; }

        public bool IsSelected { get; set; }
    }
}
