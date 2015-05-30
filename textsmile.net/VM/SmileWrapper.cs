using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ItemDesigner.Commands;
using textsmile.net.Annotations;

namespace textsmile.net.VM {
   public class SmileWrapper : INotifyPropertyChanged {
      private readonly Action<SmileWrapper> _raiseRemove;

      public SmileWrapper() {
         RemoveCommand = new ActionCommand(() => {
            if (_raiseRemove != null) {
               _raiseRemove(this);
            }
         });
      }

      public SmileWrapper(string content) : this() {
         Content = content;
      }

      public SmileWrapper(string content, Action<SmileWrapper> raiseRemove)
         : this(content) {
         _raiseRemove = raiseRemove;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public string Content { get; set; }
      public ICommand RemoveCommand { get; set; }
      public ICommand RemoveImmediateCommand { get; set; }

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}