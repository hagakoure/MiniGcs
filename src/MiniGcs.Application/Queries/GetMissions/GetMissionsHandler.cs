using MediatR;
using MiniGcs.Application.DTOs;
using MiniGcs.Domain.Interfaces;

namespace MiniGcs.Application.Queries.GetMissions;

/// <summary>
/// Обработчик запроса списка миссий
/// </summary>
public class GetMissionsHandler(IMissionRepository missionRepository)
    : IRequestHandler<GetMissionsQuery, IReadOnlyList<MissionDto>>
{
    public async Task<IReadOnlyList<MissionDto>> Handle(
        GetMissionsQuery request,
        CancellationToken cancellationToken)
    {
        var missions = await missionRepository.GetAllAsync(cancellationToken);
        
        return missions.Select(m => new MissionDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            DeviceId = m.DeviceId,
            Status = m.Status,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            Waypoints = m.Waypoints
                .OrderBy(w => w.Order)
                .Select(w => new WaypointDto
                {
                    Id = w.Id,
                    Order = w.Order,
                    Latitude = w.Position.Latitude,
                    Longitude = w.Position.Longitude,
                    Altitude = w.Altitude.Meters,
                    Speed = w.Speed.MetersPerSecond,
                    Action = w.Action,
                    Description = w.Description
                })
                .ToList()
                .AsReadOnly()
        }).ToList();
    }
}