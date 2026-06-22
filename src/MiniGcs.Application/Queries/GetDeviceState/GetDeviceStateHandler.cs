using MediatR;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Application.DTOs;
using MiniGcs.Domain.Interfaces;

namespace MiniGcs.Application.Queries.GetDeviceState;

/// <summary>
/// Обработчик запроса состояния устройства
/// </summary>
public class GetDeviceStateHandler(
    IDeviceConnection connection,
    ITelemetryStream telemetryStream)
    : IRequestHandler<GetDeviceStateQuery, DeviceInfoDto?>
{
    public Task<DeviceInfoDto?> Handle(
        GetDeviceStateQuery request,
        CancellationToken cancellationToken)
    {
        var latest = telemetryStream.GetAllLatest().Values.FirstOrDefault();
        
        var dto = new DeviceInfoDto
        {
            Id = latest?.DeviceId ?? "unknown",
            Name = "Drone-001",  // Можно вынести в конфигурацию
            DeviceType = "ASV",
            State = latest?.State ?? (connection.IsConnected 
                ? Domain.Enums.DeviceState.Idle 
                : Domain.Enums.DeviceState.Disconnected),
            IsConnected = connection.IsConnected,
            BatteryLevel = latest?.BatteryLevel ?? 0,
            LastUpdate = latest?.Timestamp
        };
        
        return Task.FromResult<DeviceInfoDto?>(dto);
    }
}