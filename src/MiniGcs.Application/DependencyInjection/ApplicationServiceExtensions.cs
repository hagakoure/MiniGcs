using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MiniGcs.Application.DependencyInjection;

/// <summary>
/// Расширение для регистрации сервисов Application Layer в DI
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Добавить сервисы Application Layer
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Регистрируем MediatR для Commands/Queries/Handlers
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}