using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniGcs.App.ViewModels;
using MiniGcs.App.Views;

namespace MiniGcs.App;

public partial class App : Avalonia.Application 
{
    private readonly IHost _host;
    
    // Конструктор для дизайнера (без DI)
    public App()
    {
        _host = null!;
    }
    
    // Конструктор для реального запуска (с DI)
    public App(IHost host)
    {
        _host = host;
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = _host?.Services.GetRequiredService<MainWindowViewModel>()
                                ?? new MainWindowViewModel();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}