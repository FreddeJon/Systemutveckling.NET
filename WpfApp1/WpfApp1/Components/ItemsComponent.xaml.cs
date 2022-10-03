using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.MVVM.Core;
using WpfApp1.MVVM.ViewModels;

namespace WpfApp1.Components
{
    /// <summary>
    /// Interaction logic for ItemsComponent.xaml
    /// </summary>
    public partial class ItemsComponent : UserControl
    {
        public DelegateCommand ChangeSelected
        {
            get => (DelegateCommand)GetValue(ChangeSelectedProperty);
            set => SetValue(ChangeSelectedProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeSelectedProperty =
            DependencyProperty.Register(nameof(ChangeSelected), typeof(DelegateCommand), typeof(ItemsComponent));


        public List<Items> Items
        {
            get => (List<Items>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(List<Items>), typeof(ItemsComponent),
                new PropertyMetadata(new List<Items>()));


        public ItemsComponent()
        {
            InitializeComponent();
        }
    }
}