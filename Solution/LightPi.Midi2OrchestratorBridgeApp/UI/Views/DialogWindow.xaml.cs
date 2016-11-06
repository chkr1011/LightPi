using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Views
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
            Result = Services.DialogResult.OK;
            Close();
        }

        private void CloseWithCancel(object sender, RoutedEventArgs e)
        {
            Result = Services.DialogResult.Cancel;
            Close();
        }
    }
}
