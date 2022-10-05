using System.Threading.Tasks;
using System.Windows;

namespace AdministrationApp.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _selectedViewModel;

    public MainViewModel(KitchenViewModel kitchenViewModel, BedroomViewModel bedroomViewModel)
    {
        KitchenViewModel = kitchenViewModel;
        BedroomViewModel = bedroomViewModel;

        SelectedViewModel = KitchenViewModel;


        SelectViewModelCommand = new RelayCommand<ViewModelBase>(SelectViewModel);
        CloseApplicationCommand = new RelayCommand(CloseApplication);


    }

    public KitchenViewModel KitchenViewModel { get; }
    public BedroomViewModel BedroomViewModel { get; }

    public RelayCommand CloseApplicationCommand { get; set; }

    public RelayCommand<ViewModelBase> SelectViewModelCommand { get; set; }

    public ViewModelBase? SelectedViewModel
    {
        get => _selectedViewModel;
        set => SetProperty(ref _selectedViewModel, value);
    }

    public override async Task LoadAsync()
    {
        if (SelectedViewModel is not null) await SelectedViewModel.LoadAsync();
    }

    private async void SelectViewModel(ViewModelBase? viewModel)
    {
        SelectedViewModel = viewModel;
        await LoadAsync();
    }

    private static void CloseApplication()
    {
        Application.Current.Shutdown();
    }
}