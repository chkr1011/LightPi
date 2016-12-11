using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using LightPi.Protocol;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class EmulatorViewModel : BaseViewModel
    {
        private readonly IOrchestratorService _orchestratorService;

        private int _frameCount;
        private int _framesPerSecond;
        private int _powerConsumption;

        public EmulatorViewModel(IOrchestratorService orchestratorService, ISettingsService settingsService)
        {
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));

            _orchestratorService = orchestratorService;

            SetupFramesPerSecondMonitor();

            orchestratorService.ChangesCommitted += ApplyState;

            foreach (var output in settingsService.OutputViewModels)
            {
                output.StateChanged += UpdateOutputState;
                Outputs.Add(output);
            }
        }

        public bool IsEnabled { get; set; } = true;

        public List<OutputViewModel> Outputs { get; } = new List<OutputViewModel>();

        public int FramesPerSecond
        {
            get { return _framesPerSecond; }
            private set { _framesPerSecond = value; OnPropertyChanged(); }
        }

        public int PowerConsumption
        {
            get { return _powerConsumption; }
            private set { _powerConsumption = value; OnPropertyChanged(); }
        }

        ////public EmulatorView View { get; set; }

        private void UpdateOutputState(object sender, EventArgs e)
        {
            var output = (OutputViewModel)sender;
            _orchestratorService.SetOutputState(output.Output.Id, output.IsActive);
            _orchestratorService.CommitChanges();
        }

        private void ApplyState(object sender, ChangesCommittedEventArgs e)
        {
            Interlocked.Increment(ref _frameCount);

            if (!IsEnabled)
            {
                return;
            }

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
                foreach (var output in Outputs)
                {
                    output.IsActive = e.State.GetBit(output.Output.Id);
                }

                ////View.Update();

                PowerConsumption = Outputs.Where(o => o.IsActive).Sum(o => o.Output.Watts);
            }));
        }

        private void SetupFramesPerSecondMonitor()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, __) =>
            {
                FramesPerSecond = Interlocked.Exchange(ref _frameCount, 0);
            };

            timer.Start();
        }
    }
}
