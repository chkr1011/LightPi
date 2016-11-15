using System;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    public class ChangesCommittedEventArgs : EventArgs
    {
        public ChangesCommittedEventArgs(byte[] state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            State = state;
        }

        public byte[] State { get; }
    }
}
