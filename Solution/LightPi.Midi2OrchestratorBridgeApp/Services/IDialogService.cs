namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface IDialogService
    {
        DialogResult ShowDialog(string title, IDialogViewModel viewModel);
    }
}