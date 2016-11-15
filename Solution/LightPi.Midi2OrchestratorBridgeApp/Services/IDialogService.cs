using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface IDialogService
    {
        DialogResult ShowDialog(string title, IDialogViewModel viewModel);
    }
}