using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingsViewModel : BaseViewModel
    {
        private readonly IFactoryService _factoryService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;
        private readonly IOrchestratorService _orchestratorService;

        public MappingsViewModel(
            IFactoryService factoryService,
            ISettingsService settingsService,
            IMidiService midiService,
            IOrchestratorService orchestratorService,
            IDialogService dialogService)
        {
            if (factoryService == null) throw new ArgumentNullException(nameof(factoryService));
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (dialogService == null) throw new ArgumentNullException(nameof(dialogService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));

            _factoryService = factoryService;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _orchestratorService = orchestratorService;

            RouteCommand(MappingsCommand.Add, AddMapping);
            RouteCommand(MappingsCommand.Edit, EditMapping);
            RouteCommand(MappingsCommand.Delete, DeleteMapping);
            RouteCommand(MappingsCommand.MoveUp, () => MoveMapping(-1));
            RouteCommand(MappingsCommand.MoveDown, () => MoveMapping(1));
            RouteCommand(ToolBarCommand.Reset, ResetStates);

            LoadMappings();
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

            var hasChangedMappings = false;
            foreach (var mapping in Mappings)
            {
                var isMatch = mapping.Mapping.Channel == e.Channel
                    && mapping.Mapping.Octave == e.Octave
                    && mapping.Mapping.Note == e.Note;

                if (!isMatch)
                {
                    continue;
                }

                var hasChanged = mapping.IsActive != isActive;
                if (!hasChanged)
                {
                    continue;
                }

                mapping.IsActive = isActive;
                hasChangedMappings = true;

                foreach (var output in mapping.Mapping.Outputs)
                {
                    _orchestratorService.SetOutputState(output, mapping.IsActive);
                }
            }
            
            if (hasChangedMappings)
            {
                _orchestratorService.CommitChanges();
            }
        }

        private void LoadMappings()
        {
            Mappings.Clear();
            foreach (var mapping in _settingsService.MappingViewModels)
            {
                Mappings.Add(mapping);
            }
        }

        private void AddMapping()
        {
            var editor = _factoryService.GetInstance<MappingEditorViewModel>();

            var result = _dialogService.ShowDialog("Add new mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            var mapping = new Mapping();
            editor.Update(mapping);
            Mappings.Add(new MappingViewModel(mapping));

            _settingsService.ImportMappingViewModels(Mappings);
            _settingsService.Save();
        }

        private void EditMapping()
        {
            if (SelectedMapping == null)
            {
                return;
            }

            var mapping = SelectedMapping.Mapping;

            var editor = _factoryService.GetInstance<MappingEditorViewModel>();
            editor.Load(mapping);

            var result = _dialogService.ShowDialog("Edit existing mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            editor.Update(mapping);
            _settingsService.Save();
            LoadMappings();
        }

        private void DeleteMapping()
        {
            if (SelectedMapping == null)
            {
                return;
            }

            Mappings.Remove(SelectedMapping);

            _settingsService.ImportMappingViewModels(Mappings);
            _settingsService.Save();
        }

        private void ResetStates()
        {
            foreach (var mapping in Mappings)
            {
                mapping.IsActive = false;
            }

            _orchestratorService.Reset();
        }

        private void MoveMapping(int direction)
        {
            if (SelectedMapping == null)
            {
                return;
            }

            var currentIndex = Mappings.IndexOf(SelectedMapping);
            var newIndex = currentIndex + direction;

            if (newIndex < 0 || newIndex >= Mappings.Count)
            {
                return;
            }

            Mappings.Move(currentIndex, newIndex);

            _settingsService.ImportMappingViewModels(Mappings);
            _settingsService.Save();
        }
    }
}
