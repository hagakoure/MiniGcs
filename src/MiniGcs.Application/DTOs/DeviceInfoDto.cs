using MiniGcs.Domain.Enums;

namespace MiniGcs.Application.DTOs;

/// <summary>
/// DTO информации об устройстве
/// </summary>
public record DeviceInfoDto
{
    /// <summary>ID устройства</summary>
    public required string Id { get; init; }
    
    /// <summary>Название</summary>
    public required string Name { get; init; }
    
    /// <summary>Тип устройства</summary>
    public required string DeviceType { get; init; }
    
    /// <summary>Состояние</summary>
    public required DeviceState State { get; init; }
    
    /// <summary>Подключено</summary>
    public required bool IsConnected { get; init; }
    
    /// <summary>Уровень батареи</summary>
    public required double BatteryLevel { get; init; }
    
    /// <summary>Время последнего обновления</summary>
    public required DateTime? LastUpdate { get; init; }
}