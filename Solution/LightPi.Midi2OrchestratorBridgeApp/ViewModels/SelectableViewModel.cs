namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class SelectableViewModel<TModel> : BaseViewModel
    {
        private bool _isSelected;

        public SelectableViewModel(TModel model)
        {
            Model = model;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }

        public TModel Model { get; }
    }
}
