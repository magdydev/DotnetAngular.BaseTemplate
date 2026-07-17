using System.Reflection;
using BaseTemplate.Application.Common;
using BaseTemplate.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application;

public static class DependencyInjection
{
    private static readonly Assembly ApplicationAssembly = typeof(DependencyInjection).Assembly;

    private static readonly Type[] OpenGenericHandlerContracts =
    [
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>),
        typeof(IDomainEventHandler<>),
    ];

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(ApplicationAssembly);
        services.AddValidatorsFromAssembly(ApplicationAssembly);

        services.AddScoped<IDispatcher, Dispatcher>();

        RegisterHandlers(services);

        return services;
    }

    /// <summary>
    /// Scans this assembly for concrete classes implementing any of the handler
    /// contracts above and registers each against every contract it implements.
    /// New commands/queries/event handlers only need to be added to a feature
    /// folder — no manual DI wiring required.
    /// </summary>
    private static void RegisterHandlers(IServiceCollection services)
    {
        var candidateTypes = ApplicationAssembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var type in candidateTypes)
        {
            var matchingInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && OpenGenericHandlerContracts.Contains(i.GetGenericTypeDefinition()));

            foreach (var handlerInterface in matchingInterfaces)
            {
                services.AddTransient(handlerInterface, type);
            }
        }
    }
}
