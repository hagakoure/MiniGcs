using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniGcs.App.ViewModels;
using MiniGcs.App.Views;

namespace MiniGcs.App;

public partial class App(IHost host) : Avalonia.Application
{
    public App() : this(null!)
    {
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = host?.Services.GetRequiredService<MainWindowViewModel>()
                                ?? new MainWindowViewModel();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}