using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Documents;
using WpfApp1.MVVM.Core;

namespace WpfApp1.MVVM.ViewModels
{
    public class KitchenViewModel : ObservableObject
    {
        private List<Items> _items;
        private Items? _selectedItem = null;

        public KitchenViewModel()
        {
            items = new List<Items>()
            {
                new Items("1", "fan"),
                new Items("2", "light"),
                new Items("3", "light")
            };


            ChangeSelected = new DelegateCommand(Change);
        }

        public DelegateCommand ChangeSelected { get; }


        public Items? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }



        public List<Items> items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        private void Change(object? obj)
        {

        }

    }



    public class Items
    {
        public Items(string itemsName, string itemsType)
        {
            ItemsName = itemsName;
            ItemsType = itemsType;
        }

        public string ItemsName { get; set; }
        public string ItemsType { get; set; }


    }
}
