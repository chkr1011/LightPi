using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.UI.Views;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class DialogService
    {
        private readonly Window _mainWindow;

        public DialogService(Window mainWindow)
        {
            if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

            _mainWindow = mainWindow;
        }

        public DialogResult ShowDialog(string title, IDialogViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var window = new DialogWindow
            {
                Title = title,
                Owner = _mainWindow,
                DataContext = viewModel
            };

            window.ShowDialog();
            viewModel.Close();

            return window.Result;
        }
    }
}
