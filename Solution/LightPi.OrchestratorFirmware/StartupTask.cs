using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using LightPi.OrchestratorFirmware.Core;
using LightPi.OrchestratorFirmware.Rest;
using LightPi.OrchestratorFirmware.Udp;
using Restup.Webserver.File;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;

namespace LightPi.OrchestratorFirmware
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        private readonly Engine _engine = new Engine();
        private readonly UdpServer _udpServer;

        public StartupTask()
        {
            _udpServer = new UdpServer(_engine);
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var task = Task.Factory.StartNew(Run, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            task.ConfigureAwait(false);
        }

        private void Run()
        {
            try
            {
                Debug.WriteLine("Starting");

                _engine.Start();
                _udpServer.Start();
                StartHttpServer();

                _engine.Run();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Unable to start. " + exception);
                _deferral.Complete();
            }
        }

        private void StartHttpServer()
        {
            var appPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "App");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<StateController>(_engine);

            var configuration = new HttpServerConfiguration()
              .RegisterRoute("api", restRouteHandler)
              .RegisterRoute(new StaticFileRouteHandler(appPath))
              .EnableCors();

            var httpServer = new HttpServer(configuration);
            httpServer.StartServerAsync().Wait();
        }
    }
}
