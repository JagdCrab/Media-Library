using System;
using System.Windows.Input;

namespace Media_Library.Components
{
    public class Command : ICommand
    {
        private Action<object> _action;

        public Command(Action<object> action)
        {
            _action = action;
        }

        public Command(Action action)
        {
            _action = new Action<object>(o => action());
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                _action("Hello World");
            }
        }

        #endregion
    }
}
