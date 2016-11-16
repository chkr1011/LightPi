using System;
using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface IMidiService
    {
        event EventHandler<NoteEventReceivedEventArgs> NoteEventReceived;

        void Initialize();
        IList<MidiPort> GetMidiPorts();
    }
}