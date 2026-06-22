using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniGcs.Application.Common.Interfaces;
using MiniGcs.Domain.Interfaces;
using MiniGcs.Infrastructure.Network;
using MiniGcs.Infrastructure.Persistence;

namespace MiniGcs.Infrastructure.DependencyInjection;

/// <summary>
/// Расширение для регистрации сервисов Infrastructure Layer в DI
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Добавить сервисы Infrastructure Layer
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Сетевые сервисы
        services.AddSingleton<IDeviceConnection, UdpDeviceConnection>();
        services.AddSingleton<ITelemetryStream, TelemetryStreamService>();
        
        // Background сервисы
        services.AddHostedService<TelemetryReceiverService>();
        
        // Репозитории (in-memory)
        services.AddSingleton<IMissionRepository, InMemoryMissionRepository>();
        services.AddSingleton<ITelemetryRepository, InMemoryTelemetryRepository>();
        
        return services;
    }
}