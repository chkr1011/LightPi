using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using NAudio.Midi;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingsListViewModel : BaseViewModel
    {
        private readonly IFactoryService _factoryService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;
        private readonly IOrchestratorService _orchestratorService;
        private ProfileViewModel _selectedProfile;

        public MappingsListViewModel(
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

        public ObservableCollection<ProfileViewModel> Profiles { get; } = new ObservableCollection<ProfileViewModel>();

        public ProfileViewModel SelectedProfile
        {
            get { return _selectedProfile; }
            set { _selectedProfile = value; OnPropertyChanged(); }
        }

        private bool NoteIsActive(NoteEventReceivedEventArgs e)
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

            return isActive;
        }

        private void MapMidiEvent(object sender, NoteEventReceivedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (SelectedProfile == null)
                {
                    return;
                }

                var isActive = NoteIsActive(e);
                var hasChangedMappings = false;
                foreach (var mapping in SelectedProfile.Mappings)
                {
                    var isMatch = mapping.Channel == e.Channel && mapping.Octave == e.Octave && mapping.Note == e.Note;
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

                    foreach (var output in mapping.Outputs)
                    {
                        _orchestratorService.SetOutputState(output.Output.Id, mapping.IsActive);
                    }
                }

                if (hasChangedMappings)
                {
                    _orchestratorService.CommitChanges();
                }
            }, DispatcherPriority.Render);
        }

        private void LoadMappings()
        {
            Profiles.Clear();
            foreach (var profileViewModel in _settingsService.ProfileViewModels)
            {
                Profiles.Add(profileViewModel);
            }

            SelectedProfile = Profiles.FirstOrDefault();
        }

        private void AddMapping()
        {
            if (SelectedProfile == null)
            {
                return;
            }

            var editor = _factoryService.GetInstance<MappingEditorViewModel>();

            var result = _dialogService.ShowDialog("Add new mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            var mapping = new MappingViewModel();
            editor.Update(mapping);
            SelectedProfile.Mappings.Add(mapping);

            _settingsService.SaveProfile(SelectedProfile);
            _settingsService.SaveSettings();
        }

        private void EditMapping()
        {
            if (SelectedProfile?.SelectedMapping == null)
            {
                return;
            }

            var editor = _factoryService.GetInstance<MappingEditorViewModel>();
            editor.Load(SelectedProfile?.SelectedMapping);

            var result = _dialogService.ShowDialog("Edit existing mapping", editor);
            if (result != DialogResult.OK)
            {
                return;
            }

            editor.Update(SelectedProfile?.SelectedMapping);
            _settingsService.SaveSettings();
            LoadMappings();
        }

        private void DeleteMapping()
        {
            if (SelectedProfile?.SelectedMapping == null)
            {
                return;
            }

            SelectedProfile.Mappings.Remove(SelectedProfile?.SelectedMapping);

            _settingsService.SaveProfile(SelectedProfile);
            _settingsService.SaveSettings();
        }

        private void ResetStates()
        {
            if (SelectedProfile == null)
            {
                return;
            }

            foreach (var mapping in SelectedProfile.Mappings)
            {
                mapping.IsActive = false;
            }

            _orchestratorService.Reset();
        }

        private void MoveMapping(int direction)
        {
            if (SelectedProfile?.SelectedMapping == null)
            {
                return;
            }

            var currentIndex = SelectedProfile.Mappings.IndexOf(SelectedProfile?.SelectedMapping);
            var newIndex = currentIndex + direction;

            if (newIndex < 0 || newIndex >= SelectedProfile.Mappings.Count)
            {
                return;
            }

            SelectedProfile.Mappings.Move(currentIndex, newIndex);

            _settingsService.SaveProfile(SelectedProfile);
            _settingsService.SaveSettings();
        }
    }
}
