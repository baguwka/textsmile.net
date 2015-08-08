using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GlobalHotKey;
using Microsoft.Practices.Unity;
using textsmile.net.Annotations;
using textsmile.net.Model;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace textsmile.net.VM {
   public sealed class MainWindowVM : IVM, INotifyPropertyChanged {
      private SmartCollection<SmileWrapper> _items;

      private HotKey _hotkey;
      private readonly HotKeyManager _hotkeyManager;
      private WindowState _windowState;
      private SaveLoadController _saveLoadController;

      public event EventHandler<KeyPressedEventArgs> HotkeyPressed;

      public WindowState WindowState
      {
         get { return _windowState; }
         set
         {
            if (value == _windowState) {
               return;
            }
            _windowState = value;
            OnPropertyChanged();
         }
      }

      public MainWindowVM() {
         _saveLoadController = App.Container.Resolve<SaveLoadController>();

         Items = new SmartCollection<SmileWrapper>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));

         AddCommand = new ActionCommand(() => {
            Items.Add(InstantiateWrapper());
         });

         HelpCommand = new ActionCommand(showHelp);

         _hotkeyManager = App.Container.Resolve<HotKeyManager>();
         _hotkeyManager.KeyPressed += hotkeyManagerOnKeyPressed;
         _hotkey = new HotKey();
      }

      private void showHelp() {
         var sb = new StringBuilder();
         sb.Append(
            $"To toggle visibility of window press hotkey combination ({ConstructHotkeyText(_hotkey.With(h => h.Key), _hotkey.With(h => h.Modifiers))})")
            .AppendLine()
            .AppendLine()
            .Append("To edit item hold Ctrl modifier on your keyboard and click to that item")
            .AppendLine()
            .AppendLine()
            .Append("To remove item immediatly hold Shift modifier and click red button next to item")
            .AppendLine();

         MessageBox.Show(sb.ToString(), "Help", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      public string ConstructHotkeyText(Key key, ModifierKeys mods) {
         var sb = new StringBuilder(32);
         if ((mods & ModifierKeys.Control) != 0) {
            sb.Append("Ctrl+");
         }
         if ((mods & ModifierKeys.Shift) != 0) {
            sb.Append("Shift+");
         }
         if ((mods & ModifierKeys.Alt) != 0) {
            sb.Append("Alt+");
         }
         if ((mods & ModifierKeys.Windows) != 0) {
            sb.Append("Win+");
         }

         sb.Append(key.ToString());

         return sb.ToString();
      }

      private void hotkeyManagerOnKeyPressed(object sender, KeyPressedEventArgs e) {
         if (!e.HotKey.Equals(_hotkey)) return;
         onHotkeyPressed(e);
      }

      public SmileWrapper InstantiateWrapper() {
         return InstantiateWrapper(string.Empty);
      }

      public SmileWrapper InstantiateWrapper(string text) {
         return new SmileWrapper(text,
            wrapper => {
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) {
                  Items.Remove(wrapper);
               }
               else {
                  var result = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo,
                     MessageBoxImage.Warning);
                  if (result == MessageBoxResult.Yes) {
                     Items.Remove(wrapper);
                  }
               }
            },

            wrapper => {
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
               }
               else {
                  if (!string.IsNullOrEmpty(wrapper.Content)) {
                     WindowState = WindowState.Minimized;
                     //Visibility = Visibility.Hidden;
                     if (!string.IsNullOrEmpty(wrapper.Content)) {
                        Clipboard.SetText(wrapper.Content);
                        //SendKeys.SendWait(wrapper.Content);
                     }
                  }
               }
            });
      }

      public SmartCollection<SmileWrapper> Items {
         get { return _items; }
         set {
            if (Equals(value, _items)) {
               return;
            }
            _items = value;
            OnPropertyChanged();
         }
      }

      public ICommand AddCommand { get; set; }
      public ICommand HelpCommand { get; set; }

      public void OnLoad() {
         SetHotkey(Key.N, ModifierKeys.Windows);
         MainVMSaveData data;

         if (_saveLoadController.TryLoad("textsmile.net main", out data,
            exception => {
               var result = MessageBox.Show("SaveData corrupted and cannot be loaded. " +
                                            "\nWipe all save data to prevent this error next time? " +
                                            "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                            $"\n\n\n Details:\n{exception.Message}" +
                                            $"\n\n StackTrace:\n{exception.StackTrace}",
                  "Error", MessageBoxButton.YesNo,
                  MessageBoxImage.Error, MessageBoxResult.Yes);

               switch (result) {
                  case MessageBoxResult.Yes:
                     return true;
                  case MessageBoxResult.No:
                     return false;
                  default:
                     return true;
               }
            }
            
            )) {
            if (data.smiles != null) {
               LoadSmiles(data.smiles.Select(InstantiateWrapper));
            }
            SetHotkey(data.Key, data.ModsKeys);
         }
      }

      public void OnClosing() {
         if (_hotkey != null) {
            _hotkeyManager.Unregister(_hotkey);
         }

         _saveLoadController.Save("textsmile.net main", new MainVMSaveData(Items) {
            Key = _hotkey.With(i => i.Key, Key.N),
            ModsKeys = _hotkey.With(h => h.Modifiers, ModifierKeys.Windows)
         }, exception => {
            var result = MessageBox.Show("SaveData corrupted and cannot be saved. " +
                                         "\nBlock writing attempt to not to corrupt the save file?? " +
                                         "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                         $"\n\n\n Details:\n{exception.Message}" +
                                         $"\n\n StackTrace:\n{exception.StackTrace}",
               "Error", MessageBoxButton.YesNo,
               MessageBoxImage.Error, MessageBoxResult.Yes);

            switch (result) {
               case MessageBoxResult.Yes:
                  return true;
               case MessageBoxResult.No:
                  return false;
               default:
                  return true;
            }
         });
      }

      public void OnClosed() {

      }

      public void LoadSmiles(IEnumerable<SmileWrapper> wrappers) {
         Items.Clear();
         Items.AddRange(wrappers);
      }

      public void SetHotkey(Key key, ModifierKeys mods) {
         Debug.WriteLine("--->> SetHotkey " + key + " and mods " + mods);
         if (_hotkey != null) {
            _hotkeyManager.Unregister(_hotkey);
         }

         _hotkey = new HotKey(key, mods);
         _hotkeyManager.Register(_hotkey);
      }

      public HotKey GetHotkey() {
         return _hotkey ?? new HotKey(Key.N, ModifierKeys.Windows);
      }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      private void onHotkeyPressed(KeyPressedEventArgs e) {
         HotkeyPressed?.Invoke(this, e);
      }
   }

   [Serializable]
   public class MainVMSaveData : CommonSaveData {
      public List<string> smiles { get; set; }
      public Key Key { get; set; }
      public ModifierKeys ModsKeys { get; set; }

      public MainVMSaveData(IEnumerable<SmileWrapper> wrappers) {
         if (wrappers != null) {
            smiles = wrappers.Select(w => w.Content).ToList();
         }
      }
   }
}
