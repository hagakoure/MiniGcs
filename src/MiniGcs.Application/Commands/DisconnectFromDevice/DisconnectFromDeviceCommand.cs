using MediatR;

namespace MiniGcs.Application.Commands.DisconnectFromDevice;

/// <summary>
/// Команда отключения от устройства
/// </summary>
public record DisconnectFromDeviceCommand : IRequest;