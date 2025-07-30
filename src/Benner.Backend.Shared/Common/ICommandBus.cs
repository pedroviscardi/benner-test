using System.Threading;
using System.Threading.Tasks;
using Benner.Backend.Shared.Commands;

namespace Benner.Backend.Shared.Common
{
    public interface ICommandBus
    {
        Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
        Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>;
    }
}