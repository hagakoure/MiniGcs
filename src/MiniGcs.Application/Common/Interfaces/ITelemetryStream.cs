using MiniGcs.Domain.Events;

namespace MiniGcs.Application.Common.Interfaces;

/// <summary>
/// Интерфейс для работы с потоком телеметрии (pub/sub)
/// </summary>
public interface ITelemetryStream
{
    /// <summary>
    /// Опубликовать телеметрию в поток (вызывается Infrastructure)
    /// </summary>
    ValueTask PublishAsync(TelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Подписаться на поток телеметрии (вызывается Application/Presentation)
    /// </summary>
    IAsyncEnumerable<TelemetryReceivedEvent> SubscribeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить последнюю телеметрию для устройства (быстрый доступ)
    /// </summary>
    TelemetryReceivedEvent? GetLatest(string deviceId);
    
    /// <summary>
    /// Получить последние телеметрии для всех устройств
    /// </summary>
    IReadOnlyDictionary<string, TelemetryReceivedEvent> GetAllLatest();
}