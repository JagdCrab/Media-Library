using System;
using System.Windows.Input;

namespace Media_Library.Components
{
    public class Command : ICommand
    {
        private Action<object> action;
        private Predicate<object> predicate;


        public Command(Action<object> _action)
        {
            action = _action;
        }

        public Command(Action _action)
        {
            action = new Action<object>(o => _action());
        }

        public Command(Action _action, Predicate<object> _predicate)
        {
            action = new Action<object>(o => _action());
            predicate = _predicate;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (predicate == null)
                return true;
            else
                return predicate(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                action(parameter);
            }
            else
            {
                action("Hello World");
            }
        }

        #endregion
    }
}
