using System;
using System.Windows.Input;

namespace FolderClean.Wpf.Handlers
{
    /// <summary>
    /// Basic Command handlers which is use to Handler events
    /// </summary>
    /// <typeparam name="T">Action Type</typeparam>
    public class CommandHandler<T> : ICommand
    {
        private Action<T> _action;
        private bool _canExecute;

        public CommandHandler(Action<T> action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is bool b)
            {
                _canExecute = b;
            }
            return _canExecute;
        }


        public void Execute(object parameter)
        {
            _action.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}