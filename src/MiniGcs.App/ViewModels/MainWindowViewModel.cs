using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MiniGcs.Application.Commands.ConnectToDevice;
using MiniGcs.Application.Commands.DisconnectFromDevice;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Application.DTOs;

namespace MiniGcs.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly IMediator _mediator;
    private readonly ITelemetryStream _telemetryStream;
    private readonly CancellationTokenSource _cts = new();
    private Task? _telemetrySubscriptionTask;
    
    // Под-ViewModel для разных панелей
    public TelemetryViewModel Telemetry { get; }
    public ControlPanelViewModel ControlPanel { get; }
    public MapViewModel Map { get; }
    public LogViewModel Log { get; }
    
    // Параметры подключения
    [ObservableProperty] private string _host = "127.0.0.1";
    [ObservableProperty] private int _port = 9001;
    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _connectionStatus = "Не подключено";
    
    [ActivatorUtilitiesConstructor]
    public MainWindowViewModel(IMediator mediator, ITelemetryStream telemetryStream)
    {
        _mediator = mediator;
        _telemetryStream = telemetryStream;
        
        Telemetry = new TelemetryViewModel();
        ControlPanel = new ControlPanelViewModel(mediator);
        Map = new MapViewModel();
        Log = new LogViewModel();
        
        AddLogMessage("Приложение запущено");
        AddLogMessage($"Целевой адрес: {Host}:{Port}");
        
        // Запускаем подписку на телеметрию в фоне
        StartTelemetrySubscription();
    }
    
    // Конструктор для дизайнера
    public MainWindowViewModel() : this(null!, null!)
    {
        Telemetry.Latitude = 55.7558;
        Telemetry.Longitude = 37.6173;
        Telemetry.Altitude = 25.3;
        Telemetry.Speed = 5.2;
        Telemetry.Heading = 45;
        Telemetry.BatteryLevel = 87;
        Telemetry.StateText = "Flying";
        IsConnected = true;
    }
    
    private void StartTelemetrySubscription()
    {
        _telemetrySubscriptionTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var telemetry in _telemetryStream.SubscribeAsync(_cts.Token))
                {
                    // Маппим Domain Event в DTO
                    var dto = new TelemetryDto
                    {
                        DeviceId = telemetry.DeviceId,
                        Latitude = telemetry.Position.Latitude,
                        Longitude = telemetry.Position.Longitude,
                        Altitude = telemetry.Altitude.Meters,
                        Speed = telemetry.Speed.MetersPerSecond,
                        Heading = telemetry.Heading,
                        BatteryLevel = telemetry.BatteryLevel,
                        State = telemetry.State,
                        Timestamp = telemetry.Timestamp
                    };
                    
                    // Обновляем UI в UI потоке
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateTelemetry(dto);
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальное завершение при закрытии приложения
            }
            catch (Exception ex)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    AddLogMessage($"Ошибка в потоке телеметрии: {ex.Message}");
                });
            }
        });
    }
    
    [RelayCommand]
    private async Task ConnectAsync()
    {
        try
        {
            ConnectionStatus = "Подключение...";
            AddLogMessage($"Подключение к {Host}:{Port}...");
            
            var result = await _mediator.Send(
                new ConnectToDeviceCommand(Host, Port), 
                _cts.Token);
            
            if (result.IsSuccess)
            {
                IsConnected = true;
                ConnectionStatus = "Подключено";
                ControlPanel.SetConnected(true);
                AddLogMessage("Успешно подключено к устройству");
            }
            else
            {
                ConnectionStatus = $"Ошибка: {result.ErrorMessage}";
                AddLogMessage($"Ошибка подключения: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Ошибка: {ex.Message}";
            AddLogMessage($"Исключение: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task DisconnectAsync()
    {
        try
        {
            await _mediator.Send(new DisconnectFromDeviceCommand(), _cts.Token);
            IsConnected = false;
            ControlPanel.SetConnected(false);
            ConnectionStatus = "Отключено";
            AddLogMessage("Отключено от устройства");
        }
        catch (Exception ex)
        {
            AddLogMessage($"Ошибка отключения: {ex.Message}");
        }
    }
    
    public void UpdateTelemetry(TelemetryDto telemetry)
    {
        Telemetry.UpdateFromDto(telemetry);
        Map.UpdateDronePosition(telemetry.Latitude, telemetry.Longitude, telemetry.Heading);
    }
    
    public void AddLogMessage(string message)
    {
        Log.AddMessage(message);
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}