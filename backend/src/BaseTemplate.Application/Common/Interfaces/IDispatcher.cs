namespace BaseTemplate.Application.Common.Interfaces;

/// <summary>
/// Routes commands and queries to their handler. This is a deliberately small,
/// dependency-free stand-in for a full mediator library — swap in MediatR or
/// another package if the project grows pipeline behaviors beyond validation.
/// </summary>
public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
