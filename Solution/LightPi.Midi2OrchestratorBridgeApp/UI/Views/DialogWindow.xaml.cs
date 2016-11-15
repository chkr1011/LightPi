using System.Windows;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;

namespace LightPi.Midi2OrchestratorBridge.UI.Views
{
    public partial class DialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        public DialogResult Result { get; private set; }

        private void CloseWithOK(object sender, RoutedEventArgs e)
        {
            Result = Models.DialogResult.OK;
            Close();
        }

        private void CloseWithCancel(object sender, RoutedEventArgs e)
        {
            Result = Models.DialogResult.Cancel;
            Close();
        }
    }
}
