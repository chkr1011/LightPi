using System.Collections.ObjectModel;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class ProfileViewModel : BaseViewModel
    {
        private MappingViewModel _selectedMapping;

        public string Name { get; set; }

        public ObservableCollection<MappingViewModel> Mappings { get; private set; } = new ObservableCollection<MappingViewModel>();

        public MappingViewModel SelectedMapping
        {
            get { return _selectedMapping; }
            set { _selectedMapping = value; OnPropertyChanged(); }
        }
    }
}
