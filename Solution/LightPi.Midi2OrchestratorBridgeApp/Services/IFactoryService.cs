namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface IFactoryService
    {
        TType GetInstance<TType>() where TType : class;
    }
}
