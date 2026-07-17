using System.Collections.Concurrent;
using BaseTemplate.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application.Common;

/// <summary>
/// Default <see cref="IDispatcher"/> implementation. Resolves the matching
/// <c>ICommandHandler{,}</c> / <c>IQueryHandler{,}</c> from DI via reflection and,
/// for commands, runs any registered FluentValidation validators first.
/// </summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(command, cancellationToken);

        var handlerType = HandlerTypeCache.GetOrAdd(
            command.GetType(),
            commandType => typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse)));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = HandlerTypeCache.GetOrAdd(
            query.GetType(),
            queryType => typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse)));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, cancellationToken);
    }

    private async Task ValidateAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validators = serviceProvider.GetServices(validatorType).Cast<IValidator>().ToList();

        if (validators.Count == 0)
        {
            return;
        }

        var context = new ValidationContext<object>(command);
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors.Where(f => f is not null));
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }
}
