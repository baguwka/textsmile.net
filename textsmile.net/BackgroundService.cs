using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Monads;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model;
using textsmile.net.Model.Smile;
using textsmile.net.UI;

namespace textsmile.net {
   [UsedImplicitly]
   public class BackgroundService {
      private readonly INotifyIconController _tray;
      private readonly HotKeyManager _hotkeyManager;
      private readonly SmileCollection _smiles;
      private readonly ConfigurationView _configView;

      public BackgroundService(INotifyIconController tray, HotKeyManager hotkeyManager, SmileCollection smiles) {
         _tray = tray;
         _hotkeyManager = hotkeyManager;
         _smiles = smiles;
         _configView = new ConfigurationView();
         _hotkeyManager.KeyPressed += onHotkeyManagerKeyPressed;
         _hotkeyManager.KeyRegistered += onHotkeyRegistered;
         _hotkeyManager.KeyUnregistered += onHotkeyUnregistered;
      }

      public void Run() {
         load();

         _tray.Create(@"Textsmile.net", new TrayContextMenu(onTrayExitClicked));
         _tray.Tray.Click += onTrayClick;
         _tray.Tray.DoubleClick += onTrayDoubleClick;

         _configView.Show();
         //_configView.WindowState = WindowState.Minimized;
      }

      private void onTrayClick(object sender, EventArgs eventArgs) {
         _configView.WindowState = WindowState.Normal;
         _configView.Focus();
      }

      public void Close() {
         unload();
         _tray.Close();
         _hotkeyManager.KeyPressed -= onHotkeyManagerKeyPressed;
         _hotkeyManager.KeyRegistered -= onHotkeyRegistered;
         _hotkeyManager.KeyUnregistered -= onHotkeyUnregistered;
      }

      private void load() {
         AppSaveData data;
         var sl = App.Container.Resolve<SaveLoadController>();
         if (sl.TryLoad("textsmile.net main", out data,
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
            })) {
            if (data.Smiles != null) {
               LoadSmiles(data.Smiles.Select(InstantiateSmile));
            }
            Debug.WriteLine("load" + DateTime.Now);
            SetHotkey(data.Key, data.ModsKeys);
         }
      }

      private void unload() {
         var hotKey = _hotkeyManager.GetHotkey("toggle");

         var sl = App.Container.Resolve<SaveLoadController>();
         sl.Save("textsmile.net main", new AppSaveData(_smiles.Items) {
            Key = hotKey.Return(i => i.Key, Key.N),
            ModsKeys = hotKey.Return(h => h.Modifiers, ModifierKeys.Windows)
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

         _hotkeyManager.Unregister("toggle");
      }

      public void SetHotkey(Key key, ModifierKeys mods) {
         _hotkeyManager.Unregister("toggle");
         var hotKey = new HotKey(key, mods);
         _hotkeyManager.Register("toggle", hotKey);
      }

      public SmileItem InstantiateSmile() {
         return InstantiateSmile(string.Empty);
      }

      private SmileItem InstantiateSmile(string content) {
         return new SmileItem(content) {
            RemoveHandler = onSmileRemoveRaised,
            ClickHandler = onSmileClickRaised
         };
      }

      private void onSmileClickRaised(SmileItem smileItem) {
         if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) { }
         else {
            if (!string.IsNullOrEmpty(smileItem.Content)) {
               if (!string.IsNullOrEmpty(smileItem.Content)) {
                  Clipboard.SetText(smileItem.Content);
               }
            }
         }
      }

      private void onSmileRemoveRaised(SmileItem smileItem) {
         if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) {
            _smiles.Items.Remove(smileItem);
         }
         else {
            var result = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo,
               MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) {
               _smiles.Items.Remove(smileItem);
            }
         }
      }

      private void LoadSmiles(IEnumerable<SmileItem> smileItems) {
         _smiles.Load(smileItems);
      }

      private void onHotkeyManagerKeyPressed(object sender, HotkeyEventArgs e) {
         _configView.ToggleWindowState();
         _configView.Left = System.Windows.Forms.Cursor.Position.X + 20;
         _configView.Top = (System.Windows.Forms.Cursor.Position.Y - _configView.Height) + 20;
      }

      private void onHotkeyRegistered(object sender, HotkeyEventArgs e) {
      }

      private void onHotkeyUnregistered(object sender, HotkeyEventArgs e) {
      }

      private void onTrayDoubleClick(object sender, EventArgs eventArgs) {
         _configView.WindowState = WindowState.Minimized;
      }

      private void onTrayExitClicked(object sender, EventArgs eventArgs) {
         Application.Current.MainWindow.Close();
      }
   }
}