using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MiniGcs.Domain.Events;
using MiniGcs.Domain.Interfaces;

namespace MiniGcs.Infrastructure.Persistence;

/// <summary>
/// In-memory хранилище истории телеметрии
/// Хранит последние N записей для каждого устройства
/// </summary>
public class InMemoryTelemetryRepository(ILogger<InMemoryTelemetryRepository> logger) : ITelemetryRepository
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<TelemetryReceivedEvent>> _history = new();
    private readonly ILogger<InMemoryTelemetryRepository> _logger = logger;
    
    private const int MaxHistoryPerDevice = 10000;

    public Task SaveAsync(TelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default)
    {
        var queue = _history.GetOrAdd(telemetry.DeviceId, _ => new ConcurrentQueue<TelemetryReceivedEvent>());
        
        queue.Enqueue(telemetry);
        
        // Ограничиваем размер очереди
        while (queue.Count > MaxHistoryPerDevice)
        {
            queue.TryDequeue(out _);
        }
        
        return Task.CompletedTask;
    }
    
    public Task<TelemetryReceivedEvent?> GetLatestAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        if (_history.TryGetValue(deviceId, out var queue) && queue.TryPeek(out var latest))
        {
            // Последняя запись - в конце очереди
            return Task.FromResult<TelemetryReceivedEvent?>(queue.ToArray().LastOrDefault());
        }
        
        return Task.FromResult<TelemetryReceivedEvent?>(null);
    }
    
    public Task<IReadOnlyList<TelemetryReceivedEvent>> GetHistoryAsync(
        string deviceId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (!_history.TryGetValue(deviceId, out var queue))
        {
            return Task.FromResult<IReadOnlyList<TelemetryReceivedEvent>>([]);
        }
        
        IReadOnlyList<TelemetryReceivedEvent> result = queue
            .Where(t => t.Timestamp >= from && t.Timestamp <= to)
            .OrderBy(t => t.Timestamp)
            .ToList();
        
        return Task.FromResult(result);
    }
}