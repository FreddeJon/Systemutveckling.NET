using System.Threading.Tasks;
using System.Windows;

// ReSharper disable MemberCanBePrivate.Global

namespace AdministrationApp.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _selectedViewModel;

    public MainViewModel(KitchenViewModel kitchenViewModel, BedroomViewModel bedroomViewModel)
    {
        KitchenViewModel = kitchenViewModel;
        BedroomViewModel = bedroomViewModel;

        SelectedViewModel = KitchenViewModel;

        SelectViewCommand = new RelayCommand<ViewModelBase>(viewModel => { SelectedViewModel = viewModel; });

        CloseApplicationCommand = new RelayCommand(CloseApplication);
    }

    public RelayCommand<ViewModelBase> SelectViewCommand { get; set; }


    public KitchenViewModel KitchenViewModel { get; set; }
    public BedroomViewModel BedroomViewModel { get; }

    public RelayCommand CloseApplicationCommand { get; }


    public ViewModelBase? SelectedViewModel
    {
        get => _selectedViewModel;
        private set
        {
            if (SetProperty(ref _selectedViewModel, value))
                SelectedViewModel?.LoadAsync();
        }
    }

    public override async Task LoadAsync()
    {
        if (SelectedViewModel is not null)
            await SelectedViewModel.LoadAsync();
    }


    private static void CloseApplication()
    {
        Application.Current.Shutdown();
    }
}