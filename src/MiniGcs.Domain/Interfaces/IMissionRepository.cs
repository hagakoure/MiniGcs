using MiniGcs.Domain.Entities;

namespace MiniGcs.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с миссиями
/// </summary>
public interface IMissionRepository
{
    /// <summary>Сохранить миссию</summary>
    Task SaveAsync(Mission mission, CancellationToken cancellationToken = default);

    /// <summary>Получить миссию по ID</summary>
    Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Получить все миссии</summary>
    Task<IReadOnlyList<Mission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Получить миссии для устройства</summary>
    Task<IReadOnlyList<Mission>> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>Удалить миссию</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}