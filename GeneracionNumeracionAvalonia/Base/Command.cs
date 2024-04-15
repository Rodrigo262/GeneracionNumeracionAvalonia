using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeneracionNumeracionAvalonia.Base
{
    public class Command : ICommand
    {

        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _executeAction;
        private bool _canExecuteCache;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> executeAction) : this(executeAction, null)
        {
        }

        public Command(Action<object> executeAction, Func<object, bool> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            var temp = _canExecute(parameter);

            if (_canExecuteCache != temp)
            {
                _canExecuteCache = temp;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            return _canExecuteCache;
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }


    }
    public class Command<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Constructor not using canExecute.
        /// </summary>
        /// <param name="execute"></param>
        public Command(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// Constructor using both execute and canExecute.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public Command(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// This method is called from XAML to evaluate if the command can be executed.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);

            return true;
        }

        /// <summary>
        /// This method is called from XAML to execute the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        /// <summary>
        /// This method allow us to force the execution of CanExecute method to reevaluate execution.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var tmpHandle = CanExecuteChanged;
            if (tmpHandle != null)
                tmpHandle(this, new EventArgs());
        }

        /// <summary>
        /// This event notify XAML controls using the command to reevaluate the CanExecute of it.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    public class AsyncCommand : IAsyncCommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Func<object, Task> _executeAction;
        private bool _canExecuteCache;

        public AsyncCommand(Func<object, Task> executeAction) : this(executeAction, null)
        {
        }

        public AsyncCommand(Func<object, Task> executeAction, Func<object, bool> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            var temp = _canExecute(parameter);

            if (_canExecuteCache != temp)
            {
                _canExecuteCache = temp;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            return _canExecuteCache;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            await _executeAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

    }
}
