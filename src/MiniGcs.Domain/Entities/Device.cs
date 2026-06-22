using MiniGcs.Domain.Enums;
using MiniGcs.Domain.ValueObjects;

namespace MiniGcs.Domain.Entities;

/// <summary>
/// Устройство (дрон/робот)
/// </summary>
public class Device
{
    /// <summary>Уникальный идентификатор устройства</summary>
    public string Id { get; }

    /// <summary>Название устройства</summary>
    public string Name { get; set; }

    /// <summary>Тип устройства</summary>
    public string DeviceType { get; set; }

    /// <summary>Текущее состояние</summary>
    public DeviceState State { get; set; }

    /// <summary>Текущая позиция</summary>
    public GeoPosition Position { get; set; }

    /// <summary>Текущая высота</summary>
    public Altitude Altitude { get; set; }

    /// <summary>Текущая скорость</summary>
    public Speed Speed { get; set; }

    /// <summary>Курс ( Heading) в градусах (0-360)</summary>
    public double Heading { get; set; }

    /// <summary>Уровень заряда батареи (0-100%)</summary>
    public double BatteryLevel { get; set; }

    /// <summary>Время последнего обновления телеметрии</summary>
    public DateTime LastTelemetryUpdate { get; set; }

    /// <summary>Подключено ли устройство</summary>
    public bool IsConnected { get; set; }

    public Device(string id, string name, string deviceType)
    {
        Id = id;
        Name = name;
        DeviceType = deviceType;
        State = DeviceState.Disconnected;
        Position = GeoPosition.Zero;
        Altitude = Altitude.Zero;
        Speed = Speed.Zero;
        Heading = 0;
        BatteryLevel = 0;
        IsConnected = false;
    }

    /// <summary>Обновить телеметрию</summary>
    public void UpdateTelemetry(
        GeoPosition position,
        Altitude altitude,
        Speed speed,
        double heading,
        double batteryLevel,
        DeviceState state)
    {
        Position = position;
        Altitude = altitude;
        Speed = speed;
        Heading = heading;
        BatteryLevel = batteryLevel;
        State = state;
        LastTelemetryUpdate = DateTime.UtcNow;
    }

    /// <summary>Установить подключение</summary>
    public void SetConnected(bool connected)
    {
        IsConnected = connected;
        State = connected ? DeviceState.Idle : DeviceState.Disconnected;
    }
}