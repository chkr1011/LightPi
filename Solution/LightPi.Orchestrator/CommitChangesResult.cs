using System;

namespace LightPi.Orchestrator
{
    public class CommitChangesResult
    {
        public CommitChangesResult(bool stateChanged, byte[] state, TimeSpan duration)
        {
            StateChanged = stateChanged;
            State = state;
            Duration = duration;
        }

        public bool StateChanged { get; }

        public byte[] State { get; }

        public TimeSpan Duration { get; }
    }
}
