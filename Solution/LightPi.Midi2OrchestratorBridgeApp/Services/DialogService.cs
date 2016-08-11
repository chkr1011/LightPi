using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.UI.Views;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class DialogService : IDialogService
    {
        public DialogResult ShowDialog(string title, IDialogViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var window = new DialogWindow
            {
                Title = title,
                Owner = Application.Current.MainWindow,
                DataContext = viewModel
            };

            window.ShowDialog();
            viewModel.Close(window.Result);

            return window.Result;
        }
    }
}
