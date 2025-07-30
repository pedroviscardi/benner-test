namespace Benner.Backend.Shared.Commands
{
    public interface ICommand
    {
    }

    public interface ICommand<out TResult> : ICommand
    {
    }
}