using MiniGcs.Domain.Enums;
using MiniGcs.Domain.ValueObjects;

namespace MiniGcs.Domain.Events;

/// <summary>
/// Событие получения телеметрии от устройства
/// </summary>
public record TelemetryReceivedEvent
{
    /// <summary>ID устройства</summary>
    public string DeviceId { get; }

    /// <summary>Позиция</summary>
    public GeoPosition Position { get; }

    /// <summary>Высота</summary>
    public Altitude Altitude { get; }

    /// <summary>Скорость</summary>
    public Speed Speed { get; }

    /// <summary>Курс (градусы)</summary>
    public double Heading { get; }

    /// <summary>Заряд батареи (%)</summary>
    public double BatteryLevel { get; }

    /// <summary>Состояние устройства</summary>
    public DeviceState State { get; }

    /// <summary>Время получения</summary>
    public DateTime Timestamp { get; }

    public TelemetryReceivedEvent(
        string deviceId,
        GeoPosition position,
        Altitude altitude,
        Speed speed,
        double heading,
        double batteryLevel,
        DeviceState state)
    {
        DeviceId = deviceId;
        Position = position;
        Altitude = altitude;
        Speed = speed;
        Heading = heading;
        BatteryLevel = batteryLevel;
        State = state;
        Timestamp = DateTime.UtcNow;
    }
}