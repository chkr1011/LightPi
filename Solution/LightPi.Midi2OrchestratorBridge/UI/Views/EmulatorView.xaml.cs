using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridge.ViewModels;

namespace LightPi.Midi2OrchestratorBridge.UI.Views
{
    public partial class EmulatorView
    {
        private EmulatorViewModel _dataContext;

        public EmulatorView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dataContext != null)
            {
                throw new InvalidOperationException();
            }

            _dataContext = e.NewValue as EmulatorViewModel;
            if (_dataContext == null)
            {
                return;
            }

            _dataContext.View = this;

            foreach (var output in _dataContext.Outputs)
            {
                Surface.AddOutput(output);
            }

            Surface.Update();
        }

        public void Update()
        {
            Surface.Update();
        }
    }
}
