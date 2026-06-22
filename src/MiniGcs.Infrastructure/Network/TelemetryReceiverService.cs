using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Domain.Interfaces;

namespace MiniGcs.Infrastructure.Network;

/// <summary>
/// Background Service, который связывает IDeviceConnection и ITelemetryStream
/// Получает телеметрию из сети и публикует в поток
/// </summary>
public class TelemetryReceiverService(
    IDeviceConnection connection,
    ITelemetryStream telemetryStream,
    ITelemetryRepository telemetryRepository,
    ILogger<TelemetryReceiverService> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TelemetryReceiverService запущен");
        
        // Подписываемся на телеметрию из сети
        connection.TelemetryReceived += async telemetry =>
        {
            try
            {
                // Публикуем в поток (для UI и других подписчиков)
                await telemetryStream.PublishAsync(telemetry, stoppingToken);
                
                // Сохраняем в историю
                await telemetryRepository.SaveAsync(telemetry, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка обработки телеметрии");
            }
        };
        
        // Логируем изменения подключения
        connection.ConnectionStateChanged += connected =>
        {
            logger.LogInformation(
                connected ? "Устройство подключено" : "Устройство отключено");
        };
        
        // Возвращаем Task, который завершится при остановке
        return Task.CompletedTask;
    }
}