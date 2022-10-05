using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly KitchenViewModel _kitchenViewModel;

        public MainViewModel()
        {
            _kitchenViewModel = new KitchenViewModel();

            CurrentView = _kitchenViewModel;
        }

        public ObservableObject CurrentView { get; private set; }
    }
}
