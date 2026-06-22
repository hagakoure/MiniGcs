using MediatR;

namespace MiniGcs.Application.Commands.CreateMission;

/// <summary>
/// Команда создания миссии
/// </summary>
public record CreateMissionCommand(
    string Name,
    string? Description,
    string? DeviceId,
    List<CreateWaypointDto> Waypoints
) : IRequest<CreateMissionResult>;

/// <summary>
/// DTO для waypoint в команде создания миссии
/// </summary>
public abstract record CreateWaypointDto(
    double Latitude,
    double Longitude,
    double Altitude,
    double Speed,
    int Order
);

/// <summary>
/// Результат создания миссии
/// </summary>
public record CreateMissionResult(
    bool IsSuccess,
    Guid? MissionId = null,
    string? ErrorMessage = null
)
{
    public static CreateMissionResult Success(Guid missionId) => new(true, missionId);
    public static CreateMissionResult Failure(string error) => new(false, null, error);
}