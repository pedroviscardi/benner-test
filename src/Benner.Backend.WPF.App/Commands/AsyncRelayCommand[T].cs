using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Benner.Backend.WPF.App.Commands
{
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Func<T, Task> _execute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (_isExecuting) return false;

            switch (parameter)
            {
                case T typedParameter:
                    return _canExecute?.Invoke(typedParameter) ?? true;
                case null when !typeof(T).IsValueType:
                    return _canExecute?.Invoke(default) ?? true;
                default:
                    return false;
            }
        }

        public async void Execute(object parameter)
        {
            if (_isExecuting) return;

            var typedParameter = default(T);

            if (parameter is T param)
            {
                typedParameter = param;
            }
            else if (parameter == null && !typeof(T).IsValueType)
            {
                typedParameter = default;
            }
            else
            {
                return; // Invalid parameter type
            }

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute(typedParameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AsyncRelayCommand<{typeof(T).Name}> error: {ex.Message}");
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}