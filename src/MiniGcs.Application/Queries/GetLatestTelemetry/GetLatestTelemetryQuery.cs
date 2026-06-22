using MediatR;
using MiniGcs.Application.DTOs;

namespace MiniGcs.Application.Queries.GetLatestTelemetry;

/// <summary>
/// Запрос получения последней телеметрии
/// </summary>
public record GetLatestTelemetryQuery(
    string? DeviceId = null
) : IRequest<TelemetryDto?>;