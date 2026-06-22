using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Domain.Events;

namespace MiniGcs.Infrastructure.Network;

/// <summary>
/// Реализация потока телеметрии
/// Поддерживает pub/sub: несколько producers (от устройств) и несколько subscribers (UI, логгеры)
/// </summary>
public class TelemetryStreamService : ITelemetryStream, IDisposable
{
    private readonly ILogger<TelemetryStreamService> _logger;
    
    // Основной канал для broadcast всем подписчикам
    private readonly Channel<TelemetryReceivedEvent> _channel;
    
    // Последние телеметрии по устройствам (для быстрого доступа)
    private readonly ConcurrentDictionary<string, TelemetryReceivedEvent> _latestTelemetry = new();
    
    public TelemetryStreamService(ILogger<TelemetryStreamService> logger)
    {
        _logger = logger;
        
        // Bounded channel с DropOldest - если UI не успевает, дропаем старые данные
        // Это нормально для телеметрии - новое значение всегда актуальнее
        _channel = Channel.CreateBounded<TelemetryReceivedEvent>(
            new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,   // Несколько producers (разные устройства)
                SingleReader = false,   // Несколько subscribers (UI, логгер, БД)
                AllowSynchronousContinuations = false
            });
        
        _logger.LogInformation("TelemetryStreamService инициализирован (capacity: 1000)");
    }
    
    /// <summary>
    /// Опубликовать телеметрию (вызывается из UdpDeviceConnection)
    /// </summary>
    public async ValueTask PublishAsync(TelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default)
    {
        // Сохраняем как последнюю для устройства
        _latestTelemetry[telemetry.DeviceId] = telemetry;
        
        // Отправляем в канал для всех подписчиков
        await _channel.Writer.WriteAsync(telemetry, cancellationToken);
    }
    
    /// <summary>
    /// Подписаться на поток телеметрии (вызывается из UI или других сервисов)
    /// </summary>
    public async IAsyncEnumerable<TelemetryReceivedEvent> SubscribeAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Новый подписчик на поток телеметрии");
        
        await foreach (var telemetry in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return telemetry;
        }
        
        _logger.LogInformation("Подписчик отключился от потока телеметрии");
    }
    
    /// <summary>
    /// Получить последнюю телеметрию для устройства (быстрый доступ без Channel)
    /// </summary>
    public TelemetryReceivedEvent? GetLatest(string deviceId)
    {
        _latestTelemetry.TryGetValue(deviceId, out var telemetry);
        return telemetry;
    }
    
    /// <summary>
    /// Получить последние телеметрии для всех устройств
    /// </summary>
    public IReadOnlyDictionary<string, TelemetryReceivedEvent> GetAllLatest()
    {
        return _latestTelemetry;
    }
    
    /// <summary>
    /// Освобождение ресурсов
    /// </summary>
    public void Dispose()
    {
        _channel.Writer.TryComplete();
        _latestTelemetry.Clear();
    }
}