﻿using System.Windows;
using System.Windows.Controls;
using AdministrationApp.MVVM.Core;

namespace AdministrationApp.Components;

/// <summary>
///     Interaction logic for TileComponent.xaml
/// </summary>
public partial class TileComponent : UserControl
{
    public static readonly DependencyProperty DeviceNameProperty =
        DependencyProperty.Register(nameof(DeviceName), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty DeviceTypeProperty =
        DependencyProperty.Register(nameof(DeviceType), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(TileComponent),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public static readonly DependencyProperty IconActiveProperty =
        DependencyProperty.Register(nameof(IconActive), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty IconInActiveProperty =
        DependencyProperty.Register(nameof(IconInActive), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty StateActiveProperty =
        DependencyProperty.Register(nameof(StateActive), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty StateInActiveProperty =
        DependencyProperty.Register(nameof(StateInActive), typeof(string), typeof(TileComponent));

    public static readonly DependencyProperty ToggleActionStateCommandProperty = DependencyProperty.Register(nameof(ToggleActionStateCommand), typeof(DelegateCommand), typeof(TileComponent), new PropertyMetadata(default(DelegateCommand)));

    public TileComponent()
    {
        InitializeComponent();
    }

    public string DeviceName
    {
        get => (string) GetValue(DeviceNameProperty);
        set => SetValue(DeviceNameProperty, value);
    }

    public string DeviceType
    {
        get => (string) GetValue(DeviceTypeProperty);
        set => SetValue(DeviceTypeProperty, value);
    }

    public bool? IsChecked
    {
        get => (bool) GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string IconActive
    {
        get => (string) GetValue(IconActiveProperty);
        set => SetValue(IconActiveProperty, value);
    }

    public string IconInActive
    {
        get => (string) GetValue(IconInActiveProperty);
        set => SetValue(IconInActiveProperty, value);
    }

    public string StateActive
    {
        get => (string) GetValue(StateActiveProperty);
        set => SetValue(StateActiveProperty, value);
    }

    public string StateInActive
    {
        get => (string) GetValue(StateInActiveProperty);
        set => SetValue(StateInActiveProperty, value);
    }

    public DelegateCommand ToggleActionStateCommand
    {
        get => (DelegateCommand) GetValue(ToggleActionStateCommandProperty);
        set => SetValue(ToggleActionStateCommandProperty, value);
    }

    private void TileToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        IsChecked = TileToggleButton.IsChecked;
    }

}