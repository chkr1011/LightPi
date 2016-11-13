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
        private readonly MappingEditorViewModel _mappingEditorViewModel;

        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogService _logService;

        public MappingsViewModel(
            MappingEditorViewModel mappingEditorViewModel,
            ISettingsService settingsService,
            IMidiService midiService, 
            IOrchestratorService orchestratorService,
            IDialogService dialogService,
            ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (dialogService == null) throw new ArgumentNullException(nameof(dialogService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _mappingEditorViewModel = mappingEditorViewModel;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _orchestratorService = orchestratorService;
            _logService = logService;

            LoadMappings();

            RouteCommand(MappingsCommand.Add, AddMapping);
            RouteCommand(MappingsCommand.Edit, EditMapping);
            RouteCommand(MappingsCommand.Delete, DeleteMapping);

            midiService.MidiMessageReceived += MapMidiEvent;
        }

        private void MapMidiEvent(object sender, MidiMessageReceivedEventArgs e)
        {
            var isActive = e.Note.CommandCode == MidiCommandCode.NoteOn && e.Note.Velocity > 0;

            if (e.Note.CommandCode == MidiCommandCode.NoteOn && e.Note.Velocity == 0)
            {
                isActive = false;
            }

            if (e.Note.CommandCode == MidiCommandCode.NoteOff)
            {
                isActive = false;
            }
            
            var channel = e.Note.Channel;

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
                foreach (var mapping in Mappings)
                {
                    if (mapping.Mapping.Channel == channel && mapping.Mapping.Note == e.Note.NoteName)
                    {
                        if (mapping.State != isActive)
                        {
                            mapping.State = isActive;
                            _orchestratorService.SetOutputState(mapping.Mapping.Output, isActive);
                        }
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
            _mappingEditorViewModel.Reset();

            var result = _dialogService.ShowDialog("Add new mapping", _mappingEditorViewModel);
            if (result != DialogResult.OK)
            {
                return;
            }

            var mapping = new Mapping
            {
                Channel = _mappingEditorViewModel.Channel,
                Note = _mappingEditorViewModel.CompleteNote,
                Output = _mappingEditorViewModel.Output,
                Comment = _mappingEditorViewModel.Comment
            };

            _settingsService.Settings.Mappings.Add(mapping);
            _settingsService.Save();

            _logService.Information("Successfully added new mapping");
            LoadMappings();
        }

        private void EditMapping()
        {
            if (SelectedMapping == null)
            {
                return;
            }

            var mapping = SelectedMapping.Mapping;

            _mappingEditorViewModel.Load(mapping);

            var result = _dialogService.ShowDialog("Edit existing mapping", _mappingEditorViewModel);
            if (result != DialogResult.OK)
            {
                return;
            }

            mapping.Channel = _mappingEditorViewModel.Channel;
            mapping.Note = _mappingEditorViewModel.CompleteNote;
            mapping.Output = _mappingEditorViewModel.Output;
            mapping.Comment = _mappingEditorViewModel.Comment;
            
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
