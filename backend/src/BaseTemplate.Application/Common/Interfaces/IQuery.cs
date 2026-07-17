namespace BaseTemplate.Application.Common.Interfaces;

/// <summary>Marker for a read use case that returns <typeparamref name="TResponse"/>.</summary>
public interface IQuery<out TResponse>
{
}

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
