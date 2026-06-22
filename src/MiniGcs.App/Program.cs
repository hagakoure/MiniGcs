using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using MiniGcs.Application.DependencyInjection;
using MiniGcs.Infrastructure.DependencyInjection;
using MiniGcs.App.ViewModels;
using System.Threading.Tasks;

namespace MiniGcs.App;

class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/minigcs-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Запуск MiniGcs Application");

            // Создаём Host с DI контейнером
            var host = CreateHostBuilder(args).Build();
            
            // Запускаем фоновые сервисы (TelemetryReceiverService)
            await host.StartAsync();

            // Запускаем Avalonia приложение
            BuildAvaloniaApp(host)
                .StartWithClassicDesktopLifetime(args);

            await host.StopAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с ошибкой");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(IHost host)
        => AppBuilder.Configure(() => new App(host))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Регистрация сервисов из всех слоёв
                services.AddApplicationServices();
                services.AddInfrastructureServices();

                // ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<TelemetryViewModel>();
                services.AddTransient<ControlPanelViewModel>();
                services.AddTransient<MapViewModel>();
                services.AddTransient<LogViewModel>();
            });
}