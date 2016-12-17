using System;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface IDialogService
    {
        event EventHandler DialogShown;

        event EventHandler DialogClosed;

        DialogResult ShowDialog(string title, IDialogViewModel viewModel);
    }
}