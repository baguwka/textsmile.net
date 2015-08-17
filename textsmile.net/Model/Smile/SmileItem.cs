using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace textsmile.net.Model.Smile {
   public class SmileItem : BindableBase {
      private string _content;

      public SmileItem() {
         RemoveCommand = new DelegateCommand(removeExecute);
         ClickCommand = new DelegateCommand(clickExecute);
      }

      public SmileItem(string content) : this() {
         _content = content;
      }

      private void removeExecute() {
         RemoveHandler?.Invoke(this);
      }

      private void clickExecute() {
         ClickHandler?.Invoke(this);
      }

      public string Content {
         get { return _content; }
         set { SetProperty(ref _content, value); }
      }

      public ICommand RemoveCommand { get; set; }
      public ICommand ClickCommand { get; set; }

      public Action<SmileItem> RemoveHandler { get; set; }
      public Action<SmileItem> ClickHandler { get; set; }
   }
}