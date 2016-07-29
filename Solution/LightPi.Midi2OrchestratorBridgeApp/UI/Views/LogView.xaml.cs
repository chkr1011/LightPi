using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Views
{
    public partial class LogView
    {
        private LogViewModel _dataContext;

        public LogView()
        {
            DataContextChanged += OnDataContextChanged;

            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dataContext != null)
            {
                _dataContext.Logged -= AppendLogMessage;
            }

            if (DataContext == null)
            {
                _dataContext = null;
                return;
            }

            var logViewModel = DataContext as LogViewModel;
            if (logViewModel == null)
            {
                return;
            }

            _dataContext = logViewModel;
            _dataContext.Logged += AppendLogMessage;
        }

        private void AppendLogMessage(object sender, LoggedEventArgs e)
        {
            string message = $"{e.Entry.Timestamp.ToString("HH:mm:ss.fff")}: {e.Entry.Message}\r";

            RichTextBox.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                var textRange = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd)
                {
                    Text = message,
                };

                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, GetBrushForSeverity(e.Entry.Severity));

                RichTextBox.ScrollToEnd();
            }));
        }

        private Brush GetBrushForSeverity(LogEntrySeverity severity)
        {
            switch (severity)
            {
                case LogEntrySeverity.Verbose:
                    return Brushes.Gray;

                case LogEntrySeverity.Information:
                    return Brushes.LightGreen;

                case LogEntrySeverity.Warning:
                    return Brushes.Yellow;

                case LogEntrySeverity.Error:
                    return Brushes.Red;

                default: throw new NotSupportedException();
            }
        }
    }
}
