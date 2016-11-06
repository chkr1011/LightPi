using System;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class MidiMessageReceivedEventArgs : EventArgs
    {
        public MidiMessageReceivedEventArgs(NoteEvent note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            Note = note;
        }

        public NoteEvent Note { get; }
    }
}
