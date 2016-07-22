using System;

namespace LightPi.Orchestrator
{
    public class SendStateResult
    {
        public SendStateResult(byte[] state, TimeSpan duration)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            State = state;
            Duration = duration;
        }

        public byte[] State { get; }

        public TimeSpan Duration { get; }
    }
}
