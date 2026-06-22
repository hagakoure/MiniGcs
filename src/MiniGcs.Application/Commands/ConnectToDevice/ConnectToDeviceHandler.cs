using MediatR;
using MiniGcs.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MiniGcs.Application.Commands.ConnectToDevice;

/// <summary>
/// Обработчик команды подключения к устройству
/// </summary>
public class ConnectToDeviceHandler(
    IDeviceConnection connection,
    ILogger<ConnectToDeviceHandler> logger)
    : IRequestHandler<ConnectToDeviceCommand, ConnectToDeviceResult>
{
    public async Task<ConnectToDeviceResult> Handle(
        ConnectToDeviceCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Подключение к устройству {Host}:{Port}",
                request.Host, request.Port);
            
            await connection.ConnectAsync(request.Host, request.Port, cancellationToken);
            
            logger.LogInformation("Успешно подключено к устройству");
            
            return ConnectToDeviceResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка подключения к устройству");
            return ConnectToDeviceResult.Failure(ex.Message);
        }
    }
}