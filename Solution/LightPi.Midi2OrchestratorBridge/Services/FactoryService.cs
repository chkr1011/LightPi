using System;
using SimpleInjector;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public class FactoryService : IFactoryService
    {
        private readonly Container _container;

        public FactoryService(Container container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public TType GetInstance<TType>() where TType : class
        {
            return _container.GetInstance<TType>();
        }
    }
}
