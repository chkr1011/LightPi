using System;
using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface IMidiService
    {
        event EventHandler<MidiMessageReceivedEventArgs> MidiMessageReceived;

        void Initialize();
        IList<MidiPort> GetMidiPorts();
    }
}