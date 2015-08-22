using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model;
using textsmile.net.Model.Smile;
using textsmile.net.UI;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace textsmile.net {
   [UsedImplicitly]
   public class BackgroundService {
      private readonly INotifyIconController _tray;
      private readonly HotKeyManager _hotKeyManager;
      private readonly SmileCollection _smiles;
      private readonly ConfigurationView _configView;

      public BackgroundService(INotifyIconController tray, HotKeyManager hotKeyManager, SmileCollection smiles) {
         _tray = tray;
         _hotKeyManager = hotKeyManager;
         _smiles = smiles;
         _configView = new ConfigurationView();
      }

      public void Run(bool startMinimized = false) {
         load();

         _tray.Create(@"Textsmile.net", new TrayContextMenu(onTrayExitClicked));
         _tray.Tray.Click += onTrayClick;
         _tray.Tray.DoubleClick += onTrayDoubleClick;

         _configView.Show();
         if (startMinimized) {
            _configView.WindowState = WindowState.Minimized;
         }
         else {
            _configView.WindowState = WindowState.Normal;
            _configView.Activate();
         }

         _hotKeyManager.KeyPressed += onHotKeyManagerKeyPressed;
         _smiles.SmileClicked += onSmileClickRaised;
         _smiles.SmileRemoved += onSmileRemoveRaised;
      }

      private void onTrayClick(object sender, EventArgs eventArgs) {
         _configView.Show();
         _configView.WindowState = WindowState.Normal;
         _configView.Focus();
         _configView.Activate();
      }

      public void Close() {
         unload();
         _tray.Close();

         _hotKeyManager.KeyPressed -= onHotKeyManagerKeyPressed;
         _smiles.SmileClicked -= onSmileClickRaised;
         _smiles.SmileRemoved -= onSmileRemoveRaised;
      }

      private void load() {
         AppSaveData data;

         if (MainAppSaveLoader.TryLoad(out data)) {
            if (data.Smiles != null) {
               LoadSmiles(data.Smiles.Select(_smiles.InstantiateSmile));
            }
            setHotkey(data.Key, data.ModsKeys);
         }
      }

      private void unload() {
         var hotKey = _hotKeyManager.GetHotkey("toggle");

         var data = new AppSaveData(_smiles.Items) {
            Key = hotKey.Return(i => i.Key, Key.N),
            ModsKeys = hotKey.Return(h => h.Modifiers, ModifierKeys.Windows)
         };

         MainAppSaveLoader.Save(data);

         _hotKeyManager.Unregister("toggle");
      }

      private void setHotkey(Key key, ModifierKeys mods) {
         _hotKeyManager.Unregister("toggle");
         var hotKey = new HotKey(key, mods);
         _hotKeyManager.Register("toggle", hotKey);
      }

      private void onSmileClickRaised(object sender, SmileItem smileItem) {
         if (!string.IsNullOrEmpty(smileItem.Content)) {
            Clipboard.SetText(smileItem.Content);
         }
      }

      private void onSmileRemoveRaised(object sender, SmileItem smileItem) {
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

      private void onHotKeyManagerKeyPressed(object sender, HotkeyEventArgs e) {
         var context = (ContextMenu)Application.Current.MainWindow.FindResource("SmilesContextMenu");
         context.IsOpen = false;
         context.PlacementTarget = Application.Current.MainWindow;
         context.IsOpen = true;
         context.Placement = PlacementMode.MousePoint;
      }

      private void onTrayDoubleClick(object sender, EventArgs eventArgs) {
         _configView.Hide();
      }

      private void onTrayExitClicked(object sender, EventArgs eventArgs) {
         Application.Current.MainWindow.Close();
      }
   }
}