using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BegawkEditorUtilities;
using ItemDesigner.Commands;
using textsmile.net.Annotations;
using textsmile.net.Model;

namespace textsmile.net.VM {
   public sealed class MainWindowVM : IVM, INotifyPropertyChanged {
      private SmartCollection<SmileWrapper> _items;
      private int _selectedItemIndex;
      private SmileWrapper _selectedItem;
      private IOManager _ioManager;

      public MainWindowVM() {
         Items = new SmartCollection<SmileWrapper>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged("Items");

         _ioManager = new IOManager("textsmile.net", "Text smile list (*.txtsml, *.json)|*.txtsml;*.json|All files (*.*)|*.*", ".txtsml");

         AddCommand = new ActionCommand(() => {
            Items.Add(InstantiateWrapper());
         });

         SaveCommand = new ActionCommand(() => {
            _ioManager.SaveAs("smiles", new SaveData(Items));
         });

         LoadCommand = new ActionCommand(loadCommandHandler);
      }

      public SmileWrapper InstantiateWrapper() {
         return InstantiateWrapper(string.Empty);
      }

      public SmileWrapper InstantiateWrapper(string text) {
         return new SmileWrapper(text, wrapper => {
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
      public ICommand SaveCommand { get; set; }
      public ICommand LoadCommand { get; set; }

      public IOManager IOManager {
         get { return _ioManager; }
         set { _ioManager = value; }
      }

      public void OnLoad() {
         SaveData data;
         if (!string.IsNullOrEmpty(IOManager.LastOpenedFile)) {

            if (IOManager.TryOpen(IOManager.LastOpenedFile, out data)) {
               LoadSmiles(data.smiles.Select(InstantiateWrapper));
            }
            return;
         }

         MainVMSaveData mainVMData;
         if (SaveLoadManager.TryLoadBin("textsmile.net main", out mainVMData)) {
            if (IOManager.TryOpen(mainVMData.LastOpenedFile, out data)) {
               LoadSmiles(data.smiles.Select(InstantiateWrapper));
            }
         }
      }

      public void OnClosing() {
         SaveLoadManager.SaveBin("textsmile.net main", new MainVMSaveData {
            LastOpenedFile = IOManager.LastOpenedFile,
         });
      }

      public void OnClosed() {
      }

      private void loadCommandHandler() {
         SaveData data;
         if (_ioManager.TryOpen(out data)) {
            LoadSmiles(data.smiles.Select(InstantiateWrapper));
         }
         else {
            MessageBox.Show(@"no data ¯\_(ツ)_/¯");
         }
      }

      public void LoadSmiles(IEnumerable<SmileWrapper> wrappers) {
         Items.Clear();
         Items.AddRange(wrappers);
      }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }

   [Serializable]
   public class MainVMSaveData : CommonSaveData {
      
   }
}