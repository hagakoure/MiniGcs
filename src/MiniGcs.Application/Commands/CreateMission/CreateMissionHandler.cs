using MediatR;
using MiniGcs.Domain.Entities;
using MiniGcs.Domain.Interfaces;
using MiniGcs.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MiniGcs.Application.Commands.CreateMission;

/// <summary>
/// Обработчик команды создания миссии
/// </summary>
public class CreateMissionHandler(
    IMissionRepository missionRepository,
    ILogger<CreateMissionHandler> logger)
    : IRequestHandler<CreateMissionCommand, CreateMissionResult>
{
    public async Task<CreateMissionResult> Handle(
        CreateMissionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Создание миссии {Name}", request.Name);
            
            var mission = new Mission(request.Name, request.Description)
            {
                DeviceId = request.DeviceId
            };
            
            foreach (var wpDto in request.Waypoints.OrderBy(w => w.Order))
            {
                var position = new GeoPosition(wpDto.Latitude, wpDto.Longitude);
                var altitude = new Altitude(wpDto.Altitude);
                var speed = new Speed(wpDto.Speed);
                
                var waypoint = new Waypoint(wpDto.Order, position, altitude, speed);
                mission.AddWaypoint(waypoint);
            }
            
            await missionRepository.SaveAsync(mission, cancellationToken);
            
            logger.LogInformation("Миссия {MissionId} успешно создана", mission.Id);
            
            return CreateMissionResult.Success(mission.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка создания миссии");
            return CreateMissionResult.Failure(ex.Message);
        }
    }
}