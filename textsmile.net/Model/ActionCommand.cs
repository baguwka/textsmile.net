using System;
using System.Windows.Input;

namespace textsmile.net.Model {
   public class ActionCommand : ICommand {

      private readonly Action _codeToExecute;

      public event EventHandler CanExecuteChanged;

      public bool CanExecute(object parameter) {
         return true;
      }

      public ActionCommand(Action codeToExecude) {
         _codeToExecute = codeToExecude;
      }

      public void Execute(object parameter) {
         _codeToExecute();
      }
   }
}
