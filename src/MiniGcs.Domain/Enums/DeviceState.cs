namespace MiniGcs.Domain.Enums;

/// <summary>
/// Состояние устройства (дрона/робота)
/// </summary>
public enum DeviceState
{
    /// <summary>Нет соединения с устройством</summary>
    Disconnected = 0,

    /// <summary>Устройство подключено, но не активно</summary>
    Idle = 1,

    /// <summary>Устройство взведено (готово к полёту/движению)</summary>
    Armed = 2,

    /// <summary>Устройство в полёте/движении</summary>
    Flying = 3,

    /// <summary>Устройство выполняет посадку</summary>
    Landing = 4,

    /// <summary>Аварийная ситуация</summary>
    Emergency = 5,

    /// <summary>Устройство возвращается домой</summary>
    ReturningHome = 6,

    /// <summary>Устройство зависло в точке</summary>
    HoldingPosition = 7
}