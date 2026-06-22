using MediatR;
using MiniGcs.Application.DTOs;

namespace MiniGcs.Application.Queries.GetDeviceState;

/// <summary>
/// Запрос получения состояния устройства
/// </summary>
public record GetDeviceStateQuery : IRequest<DeviceInfoDto?>;