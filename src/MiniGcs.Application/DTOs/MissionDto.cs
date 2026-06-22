using MiniGcs.Domain.Entities;

namespace MiniGcs.Application.DTOs;

/// <summary>
/// DTO миссии
/// </summary>
public record MissionDto
{
    /// <summary>ID миссии</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Название</summary>
    public required string Name { get; init; }
    
    /// <summary>Описание</summary>
    public string? Description { get; init; }
    
    /// <summary>ID устройства</summary>
    public string? DeviceId { get; init; }
    
    /// <summary>Статус миссии</summary>
    public required MissionStatus Status { get; init; }
    
    /// <summary>Список waypoints</summary>
    public required IReadOnlyList<WaypointDto> Waypoints { get; init; }
    
    /// <summary>Дата создания</summary>
    public required DateTime CreatedAt { get; init; }
    
    /// <summary>Дата обновления</summary>
    public required DateTime UpdatedAt { get; init; }
}