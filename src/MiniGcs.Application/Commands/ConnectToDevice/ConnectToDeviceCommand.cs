using MediatR;

namespace MiniGcs.Application.Commands.ConnectToDevice;

/// <summary>
/// Команда подключения к устройству
/// </summary>
public record ConnectToDeviceCommand(
    string Host,
    int Port
) : IRequest<ConnectToDeviceResult>;

/// <summary>
/// Результат подключения
/// </summary>
public record ConnectToDeviceResult(
    bool IsSuccess,
    string? ErrorMessage = null
)
{
    public static ConnectToDeviceResult Success() => new(true);
    public static ConnectToDeviceResult Failure(string error) => new(false, error);
}