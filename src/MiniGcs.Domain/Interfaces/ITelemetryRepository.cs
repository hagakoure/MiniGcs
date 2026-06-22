// src/MiniGcs.Domain/Interfaces/ITelemetryRepository.cs
using MiniGcs.Domain.Events;

namespace MiniGcs.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с телеметрией
/// </summary>
public interface ITelemetryRepository
{
    /// <summary>Сохранить телеметрию</summary>
    Task SaveAsync(TelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default);
    
    /// <summary>Получить последнюю телеметрию для устройства</summary>
    Task<TelemetryReceivedEvent?> GetLatestAsync(string deviceId, CancellationToken cancellationToken = default);
    
    /// <summary>Получить историю телеметрии</summary>
    Task<IReadOnlyList<TelemetryReceivedEvent>> GetHistoryAsync(
        string deviceId, 
        DateTime from, 
        DateTime to, 
        CancellationToken cancellationToken = default);
}