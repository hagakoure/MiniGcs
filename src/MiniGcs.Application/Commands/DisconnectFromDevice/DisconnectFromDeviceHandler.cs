using MediatR;
using MiniGcs.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MiniGcs.Application.Commands.DisconnectFromDevice;

/// <summary>
/// Обработчик команды отключения
/// </summary>
public class DisconnectFromDeviceHandler(
    IDeviceConnection connection,
    ILogger<DisconnectFromDeviceHandler> logger)
    : IRequestHandler<DisconnectFromDeviceCommand>
{
    public async Task Handle(
        DisconnectFromDeviceCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Отключение от устройства");

        await connection.DisconnectAsync(cancellationToken);

        logger.LogInformation("Успешно отключено от устройства");
    }
}