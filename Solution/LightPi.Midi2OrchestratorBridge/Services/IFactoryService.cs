namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface IFactoryService
    {
        TType GetInstance<TType>() where TType : class;
    }
}
