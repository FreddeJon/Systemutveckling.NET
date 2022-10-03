using WpfApp1.MVVM.Core;

namespace WpfApp1.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableObject _currentView;

        public MainViewModel()
        {
            KitchenViewModel = new KitchenViewModel();
            BedroomViewModel = new BedroomViewModel();

            CurrentView = KitchenViewModel;

            BedroomViewCommand = new DelegateCommand(x => { CurrentView = BedroomViewModel; });
            KitchenViewCommand = new DelegateCommand(x => { CurrentView = KitchenViewModel; });
        }

        public DelegateCommand KitchenViewCommand { get; set; }

        public DelegateCommand BedroomViewCommand { get; set; }

        public BedroomViewModel BedroomViewModel { get; set; }

        public ObservableObject CurrentView
        {
            get => _currentView;
            set
            {
                if (Equals(value, _currentView)) return;
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public KitchenViewModel KitchenViewModel { get; set; }
    }
}
