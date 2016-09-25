using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using LightPi.Orchestrator;
using LightPi.OrchestratorEmulator.Settings;
using LightPi.Protocol;

namespace LightPi.OrchestratorEmulator
{
    public partial class MainWindow
    {
        private readonly OrchestratorClient _orchestratorClient;
        private int _frameCount;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var udpEndpoint = new OrchestratorServer(UpdateStates);
                udpEndpoint.Start();

                var settingsService = new SettingsService(Path.Combine(baseDirectory, "LightPi.OrchestratorEmulatorSettings.xml"));
                settingsService.Load();

                _orchestratorClient = new OrchestratorClient(settingsService.Settings.OrchestratorAddress);

                Surface.RegisterBackgroundSprite(Path.Combine(baseDirectory, @"Sprites\Background.png"));

                for (var i = 0; i < LightPiProtocol.OutputsCount; i++)
                {
                    var outputDefinition = settingsService.Settings.OutputDefinitions.FirstOrDefault(o => o.Id.Equals(i));
                    Surface.RegisterOutput(i, outputDefinition?.Watts ?? 0, Path.Combine(baseDirectory, $@"Sprites\{i}.png"));
                }
                
                Surface.Updated += CalculateStatistics;
                Surface.Updated += ForwardStateToOrchestrator;
                Surface.Update();
           
                SetupFramesPerSecondMonitor();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForwardStateToOrchestrator(object sender, EventArgs e)
        {
            if (ForwardCheckBox.IsChecked != true)
            {
                return;
            }

            foreach (var output in Surface.Outputs)
            {
                _orchestratorClient.SetOutput(output.Id, output.IsActive, SetOutputMode.Set);
            }

            _orchestratorClient.CommitChanges();
        }

        private void SetupFramesPerSecondMonitor()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, __) =>
            {
                var actualFrameCount = _frameCount;
                _frameCount = 0;

                FramesPerSecondTextBlock.Text = actualFrameCount.ToString("00") + " FPS";
            };

            timer.Start();
        }

        private void CalculateStatistics(object sender, EventArgs e)
        {
            var watts = Surface.Outputs.Where(o => o.IsActive).Sum(o => o.Watts);
            ActualWattsTextBlock.Text = watts.ToString("0000") + " W";
        }

        private void UpdateStates(byte[] states)
        {
            _frameCount++;

            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (IsEnabledCheckBox.IsChecked == true)
                {
                    foreach (var output in Surface.Outputs)
                    {
                        output.IsActive = states.GetBit(output.Id);
                    }
                }

                Surface.Update();
            }));
        }
    }
}
