using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MiniGcs.Domain.Entities;
using MiniGcs.Domain.Interfaces;

namespace MiniGcs.Infrastructure.Persistence;

/// <summary>
/// In-memory хранилище миссий
/// todo можно заменить на SQLite/PostgreSQL
/// </summary>
public class InMemoryMissionRepository(ILogger<InMemoryMissionRepository> logger) : IMissionRepository
{
    private readonly ConcurrentDictionary<Guid, Mission> _missions = new();

    public Task SaveAsync(Mission mission, CancellationToken cancellationToken = default)
    {
        _missions[mission.Id] = mission;
        logger.LogInformation("Миссия {MissionId} сохранена", mission.Id);
        return Task.CompletedTask;
    }
    
    public Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _missions.TryGetValue(id, out var mission);
        return Task.FromResult(mission);
    }
    
    public Task<IReadOnlyList<Mission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Mission> result = _missions.Values.ToList();
        return Task.FromResult(result);
    }
    
    public Task<IReadOnlyList<Mission>> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Mission> result = _missions.Values
            .Where(m => m.DeviceId == deviceId)
            .ToList();
        return Task.FromResult(result);
    }
    
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _missions.TryRemove(id, out _);
        logger.LogInformation("Миссия {MissionId} удалена", id);
        return Task.CompletedTask;
    }
}