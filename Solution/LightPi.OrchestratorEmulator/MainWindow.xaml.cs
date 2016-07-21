using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using LightPi.Orchestrator;
using LightPi.Protocol;

namespace LightPi.OrchestratorEmulator
{
    public partial class MainWindow
    {
        private int _frameCount;

        public MainWindow()
        {
            InitializeComponent();
            
            var udpEndpoint = new OrchestratorServer(UpdateStates);
            udpEndpoint.Start();

            Surface.Updated += CalculateStatistics;

            Surface.RegisterBackgroundSprite(@".\Sprites\Background.jpg");

            for (int i = 0; i < 48; i++)
            {
                string spriteFile = $@".\Sprites\{i}.png";
                if (!File.Exists(spriteFile))
                {
                    continue;
                }

                Surface.RegisterOutput(i, 192, spriteFile);
            }

            Surface.Update();

            SetupFramesPerSecondMonitor();
        }

        private void SetupFramesPerSecondMonitor()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
                if (IsEnabledCheckBox.IsChecked != true)
                {
                    return;
                }

                foreach (var output in Surface.Outputs)
                {
                    output.IsActive = states.GetBit(output.ID);
                }

                Surface.Update();
            }));
        }
    }
}
