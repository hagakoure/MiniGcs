using MiniGcs.Domain.Enums;

namespace MiniGcs.Application.DTOs;

/// <summary>
/// DTO телеметрии устройства
/// </summary>
public record TelemetryDto
{
    /// <summary>ID устройства</summary>
    public required string DeviceId { get; init; }
    
    /// <summary>Широта</summary>
    public required double Latitude { get; init; }
    
    /// <summary>Долгота</summary>
    public required double Longitude { get; init; }
    
    /// <summary>Высота (метры)</summary>
    public required double Altitude { get; init; }
    
    /// <summary>Скорость (м/с)</summary>
    public required double Speed { get; init; }
    
    /// <summary>Скорость (км/ч)</summary>
    public double SpeedKmh => Speed * 3.6;
    
    /// <summary>Курс (градусы)</summary>
    public required double Heading { get; init; }
    
    /// <summary>Заряд батареи (%)</summary>
    public required double BatteryLevel { get; init; }
    
    /// <summary>Состояние устройства</summary>
    public required DeviceState State { get; init; }
    
    /// <summary>Время получения</summary>
    public required DateTime Timestamp { get; init; }
}