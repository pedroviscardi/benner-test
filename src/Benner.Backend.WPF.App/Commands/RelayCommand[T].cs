using System;
using System.Windows.Input;

namespace Benner.Backend.WPF.App.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
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
            if (parameter is T typedParameter)
            {
                return _canExecute?.Invoke(typedParameter) ?? true;
            }

            if (parameter == null && !typeof(T).IsValueType)
            {
                return _canExecute?.Invoke(default) ?? true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
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
                return;
            }

            _execute(typedParameter);
        }
    }
}