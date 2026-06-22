using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MiniGcs.Application.Commands.SendDeviceCommand;
using MiniGcs.Domain.Enums;

namespace MiniGcs.App.ViewModels;

public partial class ControlPanelViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    
    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _statusMessage = "Ожидание подключения";
    
    public ControlPanelViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // Конструктор для дизайнера
    public ControlPanelViewModel() : this(null!)
    {
        IsConnected = true;
    }
    
    /// <summary>
    /// Установить состояние подключения и уведомить команды
    /// </summary>
    public void SetConnected(bool connected)
    {
        IsConnected = connected;
        
        // Уведомляем все команды об изменении CanExecute
        ArmCommand.NotifyCanExecuteChanged();
        DisarmCommand.NotifyCanExecuteChanged();
        TakeoffCommand.NotifyCanExecuteChanged();
        LandCommand.NotifyCanExecuteChanged();
        ReturnToHomeCommand.NotifyCanExecuteChanged();
        HoldPositionCommand.NotifyCanExecuteChanged();
        EmergencyStopCommand.NotifyCanExecuteChanged();
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task ArmAsync()
    {
        await SendCommandAsync(CommandType.Arm, "Взведение");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task DisarmAsync()
    {
        await SendCommandAsync(CommandType.Disarm, "Снятие с взвода");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task TakeoffAsync()
    {
        await SendCommandAsync(CommandType.Takeoff, "Взлёт");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task LandAsync()
    {
        await SendCommandAsync(CommandType.Land, "Посадка");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task ReturnToHomeAsync()
    {
        await SendCommandAsync(CommandType.ReturnToHome, "Возврат домой");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task HoldPositionAsync()
    {
        await SendCommandAsync(CommandType.HoldPosition, "Зависание");
    }
    
    [RelayCommand(CanExecute = nameof(IsConnected))]
    private async Task EmergencyStopAsync()
    {
        await SendCommandAsync(CommandType.EmergencyStop, "АВАРИЙНАЯ ОСТАНОВКА");
    }
    
    private async Task SendCommandAsync(CommandType commandType, string commandName)
    {
        if (_mediator == null) return;
        
        if (!IsConnected)
        {
            StatusMessage = "Устройство не подключено";
            return;
        }
        
        try
        {
            StatusMessage = $"Отправка: {commandName}...";
            
            var result = await _mediator.Send(
                new SendDeviceCommandCommand(commandType));
            
            StatusMessage = result.IsSuccess 
                ? $"Отправлено: {commandName}" 
                : $"Ошибка: {result.ErrorMessage}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Исключение: {ex.Message}";
        }
    }
}