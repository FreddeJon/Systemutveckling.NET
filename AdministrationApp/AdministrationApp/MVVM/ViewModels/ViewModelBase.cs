using System.Threading.Tasks;

namespace AdministrationApp.MVVM.ViewModels;

public class ViewModelBase : ObservableObject
{
    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}