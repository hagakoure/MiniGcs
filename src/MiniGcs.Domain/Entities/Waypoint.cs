using MiniGcs.Domain.Enums;
using MiniGcs.Domain.ValueObjects;

namespace MiniGcs.Domain.Entities;

/// <summary>
/// Точка маршрута (waypoint)
/// </summary>
public class Waypoint
{
    /// <summary>Уникальный идентификатор</summary>
    public Guid Id { get; }
    
    /// <summary>Порядковый номер в миссии</summary>
    public int Order { get; set; }
    
    /// <summary>Географическая позиция</summary>
    public GeoPosition Position { get; set; }
    
    /// <summary>Целевая высота</summary>
    public Altitude Altitude { get; set; }
    
    /// <summary>Целевая скорость</summary>
    public Speed Speed { get; set; }
    
    /// <summary>Действие в этой точке</summary>
    public WaypointAction Action { get; set; }
    
    /// <summary>Время задержки в секундах (для Hover)</summary>
    public int HoverTimeSeconds { get; set; }
    
    /// <summary>Описание (опционально)</summary>
    public string? Description { get; set; }
    
    public Waypoint(int order, GeoPosition position, Altitude altitude, Speed speed)
    {
        Id = Guid.NewGuid();
        Order = order;
        Position = position;
        Altitude = altitude;
        Speed = speed;
        Action = WaypointAction.None;
        HoverTimeSeconds = 0;
    }
}