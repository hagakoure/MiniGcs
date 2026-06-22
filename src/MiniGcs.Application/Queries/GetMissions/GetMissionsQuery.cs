using MediatR;
using MiniGcs.Application.DTOs;

namespace MiniGcs.Application.Queries.GetMissions;

/// <summary>
/// Запрос получения списка миссий
/// </summary>
public record GetMissionsQuery : IRequest<IReadOnlyList<MissionDto>>;