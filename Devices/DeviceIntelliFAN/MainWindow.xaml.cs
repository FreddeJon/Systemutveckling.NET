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
using Shared.Services.DeviceService.Interfaces;

namespace DeviceIntelliFAN;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDeviceService _deviceService;

    public MainWindow(IDeviceService deviceService)
    {
        _deviceService = deviceService;
        InitializeComponent();


        Loaded += MainWindow_Loaded;
        _deviceService.ConnectionStateChangedEvent += ConnectionStateChangedEvent;
        Closed += MainWindow_Closed;
    }

    private void ConnectionStateChangedEvent(object sender, Shared.Services.DeviceService.Events.ConnectionStateArgs e)
    {
        throw new NotImplementedException();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        Loaded -= MainWindow_Loaded;


        Closed -= MainWindow_Closed;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      await _deviceService.ConnectDeviceAsync();
     await Task.Delay(10000);
    }
}
