using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.UI.Views;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels;
using SimpleInjector;

namespace LightPi.Midi2OrchestratorBridgeApp
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var settingsService = new SettingsService();
                settingsService.Load();
                
                var container = new Container();
                container.Register<ISettingsService>(() => settingsService, Lifestyle.Singleton);
                container.Register<IOrchestratorService, OrchestratorService>(Lifestyle.Singleton);
                container.Register<ILogService, LogService>(Lifestyle.Singleton);
                container.Register<IMidiService, MidiService>(Lifestyle.Singleton);
                container.Register<IDialogService, DialogService>(Lifestyle.Singleton);
                container.Verify();
                
                var mainWindowViewModel = container.GetInstance<MainWindowViewModel>();
                var mainWindow = new MainWindow { DataContext = mainWindowViewModel };
                mainWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
