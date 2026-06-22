using MiniGcs.Domain.Enums;

namespace MiniGcs.Domain.Interfaces;

/// <summary>
/// Интерфейс для подключения к устройству
/// </summary>
public interface IDeviceConnection
{
    /// <summary>Подключено ли устройство</summary>
    bool IsConnected { get; }

    /// <summary>Подключиться к устройству</summary>
    Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default);

    /// <summary>Отключиться от устройства</summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>Отправить команду устройству</summary>
    Task SendCommandAsync(CommandType command, CancellationToken cancellationToken = default);

    /// <summary>Событие получения телеметрии</summary>
    event Action<Events.TelemetryReceivedEvent>? TelemetryReceived;

    /// <summary>Событие изменения состояния подключения</summary>
    event Action<bool>? ConnectionStateChanged;
}