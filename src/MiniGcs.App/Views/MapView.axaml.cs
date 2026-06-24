using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MiniGcs.App.ViewModels;

namespace MiniGcs.App.Views;

public partial class MapView : UserControl
{
    public MapView()
    {
        InitializeComponent();
        
        // Подписываемся на загрузку, чтобы привязать ViewModel
        Loaded += OnLoaded;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MapViewModel vm)
        {
            var canvas = this.FindControl<MapCanvas>("TheMapCanvas");
            canvas?.BindViewModel(vm);
        }
    }
    
    private void ZoomIn_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MapViewModel vm) vm.ZoomIn();
    }
    
    private void ZoomOut_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MapViewModel vm) vm.ZoomOut();
    }
    
    private void CenterOnDrone_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MapViewModel vm) vm.CenterOnDrone();
    }
    
    private void ToggleAutoCenter_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MapViewModel vm) vm.ToggleAutoCenter();
    }
}