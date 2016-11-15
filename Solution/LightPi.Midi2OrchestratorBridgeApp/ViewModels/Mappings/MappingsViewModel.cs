using System;
using System.Collections.ObjectModel;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
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
            RouteCommand(MappingsCommand.MoveUp, MoveMappingUp);
            RouteCommand(MappingsCommand.MoveDown, MoveMappingDown);

            midiService.NoteEventReceived += MapMidiEvent;
        }

        public ObservableCollection<MappingViewModel> Mappings { get; } = new ObservableCollection<MappingViewModel>();

        public MappingViewModel SelectedMapping { get; set; }

        private void MapMidiEvent(object sender, NoteEventReceivedEventArgs e)
        {
            var isActive = e.Command == MidiCommandCode.NoteOn && e.Velocity > 0;

            if (e.Command == MidiCommandCode.NoteOn && e.Velocity == 0)
            {
                isActive = false;
            }

            if (e.Command == MidiCommandCode.NoteOff)
            {
                isActive = false;
            }

            foreach (var mapping in Mappings)
            {
                if (mapping.Mapping.Channel != e.Channel || mapping.Mapping.Octave != e.Octave || mapping.Mapping.Note != e.Note)
                {
                    continue;
                }

                if (mapping.IsActive == isActive)
                {
                    continue;
                }

                foreach (var output in mapping.Mapping.Outputs)
                {
                    _orchestratorService.SetOutputState(output, isActive);
                }

                mapping.IsActive = isActive;
            }

            _orchestratorService.CommitChanges();

            ////Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
            ////{
            ////    foreach (var mapping in Mappings)
            ////    {
            ////        if (mapping.Mapping.Channel != e.Channel || mapping.Mapping.Octave != e.Octave || mapping.Mapping.Note != e.Note)
            ////        {
            ////            continue;
            ////        }

            ////        if (mapping.State == isActive)
            ////        {
            ////            continue;
            ////        }

            ////        mapping.State = isActive;
            ////        foreach (var output in mapping.Mapping.Outputs)
            ////        {
            ////            _orchestratorService.SetOutputState(output, isActive);
            ////        }
            ////    }

            ////    _orchestratorService.CommitChanges();
            ////}));
        }

        private void LoadMappings()
        {
            Mappings.Clear();
            foreach (var mapping in _settingsService.Settings.Mappings)
            {
                var mappingViewModel = new MappingViewModel(mapping);
                Mappings.Add(mappingViewModel);
            }
        }

        private void AddMapping()
        {
            _mappingEditorViewModel.Reset();

            var result = _dialogService.ShowDialog("Add new mapping", _mappingEditorViewModel);
            if (result != DialogResult.OK)
            {
                return;
            }

            var mapping = new Mapping();
            _mappingEditorViewModel.Update(mapping);

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

            _mappingEditorViewModel.Update(mapping);

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

        private void MoveMappingUp()
        {
            if (SelectedMapping == null)
            {
                return;
            }

            _settingsService.Save();
        }

        private void MoveMappingDown()
        {
            if (SelectedMapping == null)
            {
                return;
            }

            _settingsService.Save();
        }
    }
}
