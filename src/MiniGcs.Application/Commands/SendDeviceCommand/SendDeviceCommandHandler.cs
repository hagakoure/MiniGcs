using MediatR;
using MiniGcs.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MiniGcs.Application.Commands.SendDeviceCommand;

/// <summary>
/// Обработчик команды отправки команды устройству
/// </summary>
public class SendDeviceCommandHandler(
    IDeviceConnection connection,
    ILogger<SendDeviceCommandHandler> logger)
    : IRequestHandler<SendDeviceCommandCommand, SendDeviceCommandResult>
{
    public async Task<SendDeviceCommandResult> Handle(
        SendDeviceCommandCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!connection.IsConnected)
            {
                return SendDeviceCommandResult.Failure("Устройство не подключено");
            }
            
            logger.LogInformation("Отправка команды {CommandType}", request.CommandType);
            
            await connection.SendCommandAsync(request.CommandType, cancellationToken);
            
            logger.LogInformation("Команда {CommandType} успешно отправлена", request.CommandType);
            
            return SendDeviceCommandResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка отправки команды {CommandType}", request.CommandType);
            return SendDeviceCommandResult.Failure(ex.Message);
        }
    }
}