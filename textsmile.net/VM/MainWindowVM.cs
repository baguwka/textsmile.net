using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BegawkEditorUtilities;
using GlobalHotKey;
using ItemDesigner.Commands;
using Microsoft.Practices.Unity;
using textsmile.net.Annotations;
using textsmile.net.Model;
using MessageBox = System.Windows.MessageBox;

namespace textsmile.net.VM {
   public sealed class MainWindowVM : IVM, INotifyPropertyChanged {
      private SmartCollection<SmileWrapper> _items;
      private int _selectedItemIndex;
      private SmileWrapper _selectedItem;

      private HotKey hotkey;
      private HotKeyManager hotkeyManager;
      private Visibility _visibility;

      public event EventHandler<KeyPressedEventArgs> HotkeyPressed;

      public Visibility Visibility
      {
         get { return _visibility; }
         set
         {
            if (value == _visibility) {
               return;
            }
            _visibility = value;
            OnPropertyChanged();
         }
      }

      public MainWindowVM() {
         Items = new SmartCollection<SmileWrapper>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));

         AddCommand = new ActionCommand(() => {
            Items.Add(InstantiateWrapper());
         });

         hotkeyManager = App.Container.Resolve<HotKeyManager>();
         hotkeyManager.KeyPressed += hotkeyManagerOnKeyPressed;
      }

      private void hotkeyManagerOnKeyPressed(object sender, KeyPressedEventArgs e) {
         if (!e.HotKey.Equals(hotkey)) return;
         OnHotkeyPressed(e);
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
               Visibility = Visibility.Hidden;
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

      public int SelectedItemIndex {
         get { return _selectedItemIndex; }
         set {
            if (value == _selectedItemIndex) {
               return;
            }
            _selectedItemIndex = value;
            OnPropertyChanged();
         }
      }

      public SmileWrapper SelectedItem {
         get { return _selectedItem; }
         set {
            if (Equals(value, _selectedItem)) {
               return;
            }
            _selectedItem = value;
            OnPropertyChanged();
         }
      }

      public ICommand AddCommand { get; set; }

      public void OnLoad() {
         Debug.WriteLine("--->> OnLoad fired ");

         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

         MainVMSaveData mainVMData;
         if (SaveLoadManager.TryLoadBin("textsmile.net main", out mainVMData)) {
            if (mainVMData.smiles != null) {
               LoadSmiles(mainVMData.smiles.Select(InstantiateWrapper));
            }
         }

         SetHotkey(Key.N, ModifierKeys.Windows);
      }

      public void OnClosing() {
         if (hotkey != null) {
            hotkeyManager.Unregister(hotkey);
         }

         SaveLoadManager.SaveBin("textsmile.net main", new MainVMSaveData(Items));
      }

      public void OnClosed() {

      }

      public void LoadSmiles(IEnumerable<SmileWrapper> wrappers) {
         Items.Clear();
         Items.AddRange(wrappers);
      }

      public void SetHotkey(Key key, ModifierKeys mods) {
         Debug.WriteLine("--->> SetHotkey " + key + " and mods " + mods);
         if (hotkey != null) {
            hotkeyManager.Unregister(hotkey);
         }

         hotkey = new HotKey(key, mods);
         hotkeyManager.Register(hotkey);
      }

      public HotKey GetHotkey() {
         return hotkey ?? new HotKey(Key.N, ModifierKeys.Windows);
      }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      private void OnHotkeyPressed(KeyPressedEventArgs e) {
         HotkeyPressed?.Invoke(this, e);
      }
   }

   [Serializable]
   public class MainVMSaveData : CommonSaveData {
      public List<string> smiles { get; set; }

      public MainVMSaveData(IEnumerable<SmileWrapper> wrappers) {
         if (wrappers != null) {
            smiles = wrappers.Select(w => w.Content).ToList();
         }
      }
   }
}