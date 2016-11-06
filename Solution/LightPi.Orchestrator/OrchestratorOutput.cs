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

        public void Activate()
        {
            _onStateCounter = 1;
        }

        public void Deactivate()
        {
            _onStateCounter = 0;
        }

        public bool IsActive()
        {
            return _onStateCounter > 0;
        }
    }
}
