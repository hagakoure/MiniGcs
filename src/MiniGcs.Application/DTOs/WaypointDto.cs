using MiniGcs.Domain.Enums;

namespace MiniGcs.Application.DTOs;

/// <summary>
/// DTO waypoint
/// </summary>
public record WaypointDto
{
    /// <summary>ID waypoint</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Порядковый номер</summary>
    public required int Order { get; init; }
    
    /// <summary>Широта</summary>
    public required double Latitude { get; init; }
    
    /// <summary>Долгота</summary>
    public required double Longitude { get; init; }
    
    /// <summary>Высота (метры)</summary>
    public required double Altitude { get; init; }
    
    /// <summary>Скорость (м/с)</summary>
    public required double Speed { get; init; }
    
    /// <summary>Действие в точке</summary>
    public required WaypointAction Action { get; init; }
    
    /// <summary>Описание</summary>
    public string? Description { get; init; }
}