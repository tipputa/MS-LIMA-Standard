using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metabolomics.Core
{
    /// <summary>
    /// This code should be required to use 'command' code in XAML code.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Action<object> executeAction;
        private Func<object, bool> canExecuteAction { get; set; }
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        public DelegateCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = x => true;
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            return canExecuteAction(parameter);
        }

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
