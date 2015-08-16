using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace textsmile.net.Model.Smile {
   public class ConfigurableSmile : BindableBase {
      private readonly Action<ConfigurableSmile> _removeHandler;
      private readonly Action<ConfigurableSmile> _clickHandler;
      private readonly SmileItem _smile;

      public string Content {
         get { return _smile.Content; }
         set { _smile.Content = value; }
      }

      public ConfigurableSmile() {
         _smile = new SmileItem();

         RemoveCommand = new DelegateCommand(() => {
            _removeHandler?.Invoke(this);
         });

         ClickCommand = new DelegateCommand(() => {
            _clickHandler?.Invoke(this);
         });
      }

      public ConfigurableSmile(string content) : this() {
         Content = content;
      }

      public ConfigurableSmile(string content, Action<ConfigurableSmile> removeHandler, Action<ConfigurableSmile> clickHandler)
         : this(content) {
         _removeHandler = removeHandler;
         _clickHandler = clickHandler;
      }

      public ICommand RemoveCommand { get; set; }
      public ICommand ClickCommand { get; set; }
   }
}
