using MediatR;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Application.DTOs;

namespace MiniGcs.Application.Queries.GetLatestTelemetry;

/// <summary>
/// Обработчик запроса получения последней телеметрии
/// </summary>
public class GetLatestTelemetryHandler(ITelemetryStream telemetryStream)
    : IRequestHandler<GetLatestTelemetryQuery, TelemetryDto?>
{
    public Task<TelemetryDto?> Handle(
        GetLatestTelemetryQuery request,
        CancellationToken cancellationToken)
    {
        // Если указан DeviceId - берём для него
        if (!string.IsNullOrEmpty(request.DeviceId))
        {
            var telemetry = telemetryStream.GetLatest(request.DeviceId);
            return Task.FromResult(telemetry?.ToDto());
        }
        
        // Иначе - берём первую из всех
        var allLatest = telemetryStream.GetAllLatest();
        var first = allLatest.Values.FirstOrDefault();
        
        return Task.FromResult(first?.ToDto());
    }
}

/// <summary>
/// Extension-методы для маппинга
/// </summary>
internal static class TelemetryMappingExtensions
{
    public static TelemetryDto ToDto(this Domain.Events.TelemetryReceivedEvent telemetry)
    {
        return new TelemetryDto
        {
            DeviceId = telemetry.DeviceId,
            Latitude = telemetry.Position.Latitude,
            Longitude = telemetry.Position.Longitude,
            Altitude = telemetry.Altitude.Meters,
            Speed = telemetry.Speed.MetersPerSecond,
            Heading = telemetry.Heading,
            BatteryLevel = telemetry.BatteryLevel,
            State = telemetry.State,
            Timestamp = telemetry.Timestamp
        };
    }
}