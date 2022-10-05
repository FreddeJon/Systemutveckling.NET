﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AdministrationApp.MVVM.Models;

namespace AdministrationApp.Components;

/// <summary>
/// Interaction logic for DeviceListView.xaml
/// </summary>
public partial class DeviceListViewComponent : UserControl
{

    public DeviceListViewComponent()
    {
        InitializeComponent();
    }

    public List<DeviceItem> Devices
    {
        get => (List<DeviceItem>)GetValue(DevicesProperty);
        set => SetValue(DevicesProperty, value);
    }

    public static readonly DependencyProperty DevicesProperty =
        DependencyProperty.Register(nameof(Devices), typeof(List<DeviceItem>),
            typeof(DeviceListViewComponent), new PropertyMetadata(new List<DeviceItem>()));



    public RelayCommand ToggleActionStateCommand
    {
        get => (RelayCommand)GetValue(ToggleActionStateCommandProperty);
        set => SetValue(ToggleActionStateCommandProperty, value);
    }
    public static readonly DependencyProperty ToggleActionStateCommandProperty =
        DependencyProperty.Register(nameof(ToggleActionStateCommand), typeof(RelayCommand), typeof(DeviceListViewComponent));

}