using System.Threading.Tasks;
using System.Windows;
using AdministrationApp.MVVM.Core;

namespace AdministrationApp.MVVM.ViewModels;

public class MainViewModel : ObservableObject
{
    public KitchenViewModel KitchenViewModel { get; }

    private ObservableObject? _selectedViewModel;
    public MainViewModel(KitchenViewModel kitchenViewModel)
    {
        KitchenViewModel = kitchenViewModel;

        SelectedViewModel = KitchenViewModel;


        SelectViewModelCommand = new DelegateCommand(SelectViewModel);
        CloseApplicationCommand = new DelegateCommand(CloseApplication);
    }

    public DelegateCommand CloseApplicationCommand { get; set; }

    public DelegateCommand SelectViewModelCommand { get; set; }

    public ObservableObject? SelectedViewModel
    {
        get => _selectedViewModel;
        set
        {
            if (Equals(_selectedViewModel, value)) return;
            _selectedViewModel = value;
            OnPropertyChanged();
        }
    }

    public override async Task LoadAsync()
    {
        if (SelectedViewModel is not null)
        {
            await SelectedViewModel.LoadAsync();
        }
    }

    private async void SelectViewModel(object? parameter)
    {
        SelectedViewModel = parameter as ObservableObject;
        await LoadAsync();
    }

    private void CloseApplication(object? parameter)
    {
        Application.Current.Shutdown();
    }
}