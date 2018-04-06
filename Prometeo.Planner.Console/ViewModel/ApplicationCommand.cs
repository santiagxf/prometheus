using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prometeo.Planner.Console.ViewModel
{
    public class ApplicationCommand : ICommand
    {

        Action<object> execute;

        Predicate<object> canExecute;
        private ApplicationCommand prevElement_Execute;
        private object CmdStockManager_Execute;

        event EventHandler CanExecuteChangedInternal;

        public ApplicationCommand(Action<object> execute)
           : this(execute, DefaultCanExecute)
        {
        }

        public ApplicationCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public ApplicationCommand(ApplicationCommand prevElement_Execute)
        {
            this.prevElement_Execute = prevElement_Execute;
        }

        public ApplicationCommand(object CmdStockManager_Execute)
        {
            this.CmdStockManager_Execute = CmdStockManager_Execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                //CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                //CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()
        {
            this.canExecute = _ => false;
            this.execute = _ => { return; };
        }

        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}
