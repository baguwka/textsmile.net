using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using textsmile.net.Model;

namespace textsmile.net.VM {
   public class SmileWrapper : INotifyPropertyChanged {
      private readonly Action<SmileWrapper> _raiseRemove;
      private readonly Action<SmileWrapper> _raiseClick;

      public SmileWrapper() {
         RemoveCommand = new ActionCommand(() => {
            _raiseRemove?.Invoke(this);
         });

         ClickCommand = new ActionCommand(() => {
            _raiseClick?.Invoke(this);
         });
      }

      public SmileWrapper(string content) : this() {
         Content = content;
      }

      public SmileWrapper(string content, Action<SmileWrapper> raiseRemove, Action<SmileWrapper> raiseClick)
         : this(content) {
         _raiseRemove = raiseRemove;
         _raiseClick = raiseClick;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public string Content { get; set; }
      public ICommand RemoveCommand { get; set; }
      public ICommand RemoveImmediateCommand { get; set; }
      public ICommand ClickCommand { get; set; }

      [NotifyPropertyChangedInvocator]
      protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
