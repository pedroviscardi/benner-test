using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Shared.Common;

public interface IQueryBus
{
    Task<TResult> ExecuteAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>;
}