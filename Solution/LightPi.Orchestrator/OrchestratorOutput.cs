using System.Security.Cryptography;

namespace LightPi.Orchestrator
{
    public class OrchestratorOutput
    {
        private int _onStateCounter;

        public void Increment()
        {
            _onStateCounter++;
        }

        public void Decrement()
        {
            _onStateCounter--;
        }

        public bool IsActive()
        {
            return _onStateCounter > 0;
        }
    }
}
