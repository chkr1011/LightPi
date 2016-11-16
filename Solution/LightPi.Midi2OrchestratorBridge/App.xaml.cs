using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridge.Services;
using LightPi.Midi2OrchestratorBridge.UI.Views;
using LightPi.Midi2OrchestratorBridge.ViewModels;
using SimpleInjector;

namespace LightPi.Midi2OrchestratorBridge
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                var container = new Container();
                container.RegisterSingleton<ISettingsService, SettingsService>();
                container.RegisterInitializer<SettingsService>(s => s.Load());

                container.RegisterSingleton<IOrchestratorService, OrchestratorService>();
                container.RegisterSingleton<ILogService, LogService>();
                container.RegisterSingleton<IMidiService, MidiService>();
                container.RegisterSingleton<IDialogService, DialogService>();
                container.RegisterSingleton<IFactoryService, FactoryService>();
                container.Verify();
                
                var mainWindowViewModel = container.GetInstance<MainWindowViewModel>();
                var mainWindow = new MainWindow { DataContext = mainWindowViewModel };
                mainWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }
    }
}
