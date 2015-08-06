using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ItemDesigner.Commands;
using textsmile.net.Annotations;

namespace textsmile.net.VM {
   public class SmileWrapper : INotifyPropertyChanged {
      private readonly Action<SmileWrapper> _raiseRemove;
      private readonly Action<SmileWrapper> _raiseClick;

      public SmileWrapper() {
         RemoveCommand = new ActionCommand(() => {
            _raiseRemove?.Invoke(this);
         });

         ClickCommand = new ActionCommand(() => {
            if (!string.IsNullOrEmpty(Content)) {
               Clipboard.SetText(Content);
            }
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
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}