namespace BaseTemplate.Application.Common.Interfaces;

/// <summary>Marker for a write use case that returns <typeparamref name="TResponse"/> (commonly the new entity's id).</summary>
public interface ICommand<out TResponse>
{
}

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
