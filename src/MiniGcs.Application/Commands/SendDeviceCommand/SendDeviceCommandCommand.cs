using MediatR;
using MiniGcs.Domain.Enums;

namespace MiniGcs.Application.Commands.SendDeviceCommand;

/// <summary>
/// Команда отправки команды устройству
/// </summary>
public record SendDeviceCommandCommand(
    CommandType CommandType
) : IRequest<SendDeviceCommandResult>;

/// <summary>
/// Результат отправки команды
/// </summary>
public record SendDeviceCommandResult(
    bool IsSuccess,
    string? ErrorMessage = null
)
{
    public static SendDeviceCommandResult Success() => new(true);
    public static SendDeviceCommandResult Failure(string error) => new(false, error);
}