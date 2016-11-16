using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.ViewModels;

namespace LightPi.Midi2OrchestratorBridge.UI.Views
{
    public partial class LogView
    {
        private readonly int _maxLogLength = Properties.Settings.Default.MaxLogLength;
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
            RichTextBox.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                var message = $"{e.Entry.Timestamp:HH:mm:ss.fff}: {e.Entry.Message}\r";

                var existingTextRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
                if (existingTextRange.Text.Length > _maxLogLength)
                {
                    existingTextRange.Text = string.Empty;
                }

                var textRange = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd)
                {
                    Text = message
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
