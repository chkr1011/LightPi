using System;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    public class NoteEventReceivedEventArgs : EventArgs
    {
        public NoteEventReceivedEventArgs(int channel, string note, int octave, int velocity, MidiCommandCode command)
        {
            Channel = channel;
            Note = note;
            Octave = octave;
            Velocity = velocity;
            Command = command;
        }

        public int Channel { get; }

        public string Note { get; }

        public int Octave { get; }

        public int Velocity { get; }

        public MidiCommandCode Command { get; }
    }
}
