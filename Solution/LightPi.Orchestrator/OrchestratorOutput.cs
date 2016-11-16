using System.Threading;

namespace LightPi.Orchestrator
{
    public class OrchestratorOutput
    {
        private int _counter;

        public void Increment()
        {
            Interlocked.Increment(ref _counter);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _counter);
        }

        public void Activate()
        {
            Interlocked.Exchange(ref _counter, 1);
        }

        public void Deactivate()
        {
            Interlocked.Exchange(ref _counter, 0);
        }

        public bool IsActive()
        {
            return _counter > 0;
        }
    }
}
