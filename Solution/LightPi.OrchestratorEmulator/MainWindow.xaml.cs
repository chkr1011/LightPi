using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using LightPi.Orchestrator;
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
                var udpEndpoint = new OrchestratorServer(UpdateStates);
                udpEndpoint.Start();

                var settings = new SettingsReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LightPi.OrchestratorEmulatorSettings.xml"));
                settings.Load();

                _orchestratorClient = new OrchestratorClient(settings.GetOrchestratorAddress());

                Surface.RegisterBackgroundSprite(settings.GetBackgroundSpriteFilename());
                foreach (var output in settings.GetOutputs())
                {
                    int id = int.Parse(output.Attribute("ID").Value);
                    string spriteFilename = output.Attribute("Sprite").Value;
                    double watts = double.Parse(output.Attribute("Watts").Value);

                    if (!File.Exists(spriteFilename))
                    {
                        continue;
                    }

                    Surface.RegisterOutput(id, watts, spriteFilename);
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
                _orchestratorClient.SetOutput(output.ID, output.IsActive, OrchestratorClient.SetOutputMode.Set);
            }

            _orchestratorClient.CommitChanges();
        }

        private void SetupFramesPerSecondMonitor()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, __) =>
            {
                int actualFrameCount = _frameCount;
                _frameCount = 0;

                FramesPerSecondTextBlock.Text = actualFrameCount.ToString("00") + " FPS";
            };

            timer.Start();
        }

        private void CalculateStatistics(object sender, EventArgs e)
        {
            double watts = Surface.Outputs.Where(o => o.IsActive).Sum(o => o.Watts);

            // TODO: Dispatcher Timer für Summe
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
                        output.IsActive = states.GetBit(output.ID);
                    }
                }

                Surface.Update();
            }));
        }
    }
}
