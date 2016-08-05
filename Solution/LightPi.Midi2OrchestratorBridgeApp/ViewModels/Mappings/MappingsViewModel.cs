using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings
{
    public class MappingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IMidiService _midiService;
        private readonly IDialogService _dialogService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogService _logService;

        public MappingsViewModel(ISettingsService settingsService, IMidiService midiService, IOrchestratorService orchestratorService, IDialogService dialogService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (dialogService == null) throw new ArgumentNullException(nameof(dialogService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _midiService = midiService;
            _dialogService = dialogService;
            _orchestratorService = orchestratorService;
            _logService = logService;

            LoadMappings();

            RouteCommand(MappingsCommand.Add, AddMapping);
            RouteCommand(MappingsCommand.Edit, EditMapping);
            RouteCommand(MappingsCommand.Delete, DeleteMapping);

            _midiService.MidiMessageReceived += MapMidiEvent;
        }

        private void MapMidiEvent(object sender, MidiMessageReceivedEventArgs e)
        {
            bool state = e.Note.CommandCode == MidiCommandCode.NoteOn;
            MidiChannel channel = (MidiChannel)e.Note.Channel - 1;

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
                foreach (var mapping in Mappings)
                {
                    if (mapping.Mapping.Channel == channel && mapping.Mapping.Note == e.Note.NoteName)
                    {
                        mapping.State = state;
                        _orchestratorService.SetOutputState(mapping.Mapping.Output, state);
                    }
                }

                _orchestratorService.CommitChanges();
            }));
        }

        private void LoadMappings()
        {
            Mappings.Clear();
            foreach (var mapping in _settingsService.Settings.Mappings.OrderBy(m => m.Output))
            {
                var mappingViewModel = new MappingViewModel(mapping);
                Mappings.Add(mappingViewModel);
            }
        }

        public ObservableCollection<MappingViewModel> Mappings { get; } = new ObservableCollection<MappingViewModel>();

        public MappingViewModel SelectedMapping { get; set; }

        private void AddMapping()
        {
            var editor = new MappingEditorViewModel(_midiService, _logService);
            var result = _dialogService.ShowDialog("Add new mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            var mapping = new Mapping
            {
                Channel = editor.Channel,
                Note = editor.CompleteNote,
                Output = editor.Output,
                Comment = editor.Comment
            };

            _settingsService.Settings.Mappings.Add(mapping);
            _settingsService.Save();

            _logService.Information("Successfully added new mapping");
            LoadMappings();
        }

        private void EditMapping()
        {
            var mapping = SelectedMapping.Mapping;

            var editor = new MappingEditorViewModel(_midiService, _logService);
            editor.Load(mapping);

            var result = _dialogService.ShowDialog("Edit existing mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            mapping.Channel = editor.Channel;
            mapping.Note = editor.CompleteNote;
            mapping.Output = editor.Output;
            mapping.Comment = editor.Comment;
            
            _settingsService.Save();
            _logService.Information("Successfully updated existing mapping");
            LoadMappings();
        }

        private void DeleteMapping()
        {
            _settingsService.Settings.Mappings.Remove(SelectedMapping.Mapping);

            _settingsService.Save();
            _logService.Information("Successfully deleted existing mapping");
            LoadMappings();
        }
    }
}
