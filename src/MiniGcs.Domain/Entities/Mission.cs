// src/MiniGcs.Domain/Entities/Mission.cs
namespace MiniGcs.Domain.Entities;

/// <summary>
/// Миссия (маршрут полёта/движения)
/// </summary>
public class Mission
{
    /// <summary>Уникальный идентификатор</summary>
    public Guid Id { get; }
    
    /// <summary>Название миссии</summary>
    public string Name { get; set; }
    
    /// <summary>Описание</summary>
    public string? Description { get; set; }
    
    /// <summary>ID устройства, которому назначена миссия</summary>
    public string? DeviceId { get; set; }
    
    /// <summary>Список waypoints</summary>
    public List<Waypoint> Waypoints { get; }
    
    /// <summary>Статус миссии</summary>
    public MissionStatus Status { get; set; }
    
    /// <summary>Дата создания</summary>
    public DateTime CreatedAt { get; }
    
    /// <summary>Дата последнего изменения</summary>
    public DateTime UpdatedAt { get; set; }
    
    public Mission(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Waypoints = new List<Waypoint>();
        Status = MissionStatus.Planned;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>Добавить waypoint</summary>
    public void AddWaypoint(Waypoint waypoint)
    {
        Waypoints.Add(waypoint);
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>Удалить waypoint</summary>
    public void RemoveWaypoint(Guid waypointId)
    {
        Waypoints.RemoveAll(w => w.Id == waypointId);
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>Начать выполнение миссии</summary>
    public void Start()
    {
        if (Status != MissionStatus.Planned)
            throw new InvalidOperationException("Миссию можно начать только из статуса Planned");
        
        Status = MissionStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>Завершить миссию</summary>
    public void Complete()
    {
        Status = MissionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>Прервать миссию</summary>
    public void Abort()
    {
        Status = MissionStatus.Aborted;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Статус миссии
/// </summary>
public enum MissionStatus
{
    /// <summary>Запланирована</summary>
    Planned = 0,
    
    /// <summary>Выполняется</summary>
    InProgress = 1,
    
    /// <summary>Завершена</summary>
    Completed = 2,
    
    /// <summary>Прервана</summary>
    Aborted = 3,
    
    /// <summary>Приостановлена</summary>
    Paused = 4
}