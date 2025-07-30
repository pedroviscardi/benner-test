using System;
using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.Common
{
    public class CommandBus : ICommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"Nenhum handler encontrado para o command {typeof(TCommand).Name}");

            await ((ICommandHandler<TCommand>) handler).HandleAsync(command, cancellationToken);
        }

        public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"Nenhum handler encontrado para o command {typeof(TCommand).Name}");

            return await ((ICommandHandler<TCommand, TResult>) handler).HandleAsync(command, cancellationToken);
        }
    }
}