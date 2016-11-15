namespace LightPi.Orchestrator
{
    public class CommitChangesResult
    {
        public CommitChangesResult(bool stateChanged, byte[] state)
        {
            StateChanged = stateChanged;
            State = state;
        }

        public bool StateChanged { get; }

        public byte[] State { get; }
    }
}
