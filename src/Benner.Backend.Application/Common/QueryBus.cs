using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;

namespace Benner.Backend.Application.Common
{
    public class QueryBus : IQueryBus
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResult> ExecuteAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"Nenhum handler encontrado para a query {typeof(TQuery).Name}");

            return await ((IQueryHandler<TQuery, TResult>) handler).HandleAsync(query, cancellationToken);
        }
    }
}