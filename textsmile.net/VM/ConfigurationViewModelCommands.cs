using System;
using System.Monads;
using System.Text;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model.Shortcut;
using textsmile.net.Model.Smile;
using textsmile.net.UI;

namespace textsmile.net.VM {
   [UsedImplicitly]
   public class ConfigurationViewModelCommands {
      private readonly HotKeyManager _hotKeyManager;
      private readonly SmileCollection _smileCollection;
      private readonly IShortcutCreator _shortcutCreator;

      public ConfigurationViewModelCommands(HotKeyManager hotKeyManager, SmileCollection smileCollection, IShortcutCreator shortcutCreator) {
         _hotKeyManager = hotKeyManager;
         _smileCollection = smileCollection;
         _shortcutCreator = shortcutCreator;

         SaveCommand = new DelegateCommand(saveCommandExecute);
         ExportCommand = new DelegateCommand(exportCommandExecute);
         ImportCommand = new DelegateCommand(importCommandExecute);
         QuitCommand = new DelegateCommand(quitCommandExecute);

         RunOnStartupCommand = new DelegateCommand<bool?>(StartupCommandExecute);
         HelpCommand = new DelegateCommand(showHelpCommandExecute);
         CheckLastReleasesCommand = new DelegateCommand(checkLastReleasesExecute);
         AboutCommand = new DelegateCommand(aboutCommandExecute);

         AddCommand = new DelegateCommand(addCommandExecute);
      }

      public ICommand SaveCommand { get; set; }
      public ICommand ExportCommand { get; set; }
      public ICommand ImportCommand { get; set; }
      public ICommand QuitCommand { get; set; }

      public ICommand RunOnStartupCommand { get; set; }
      public ICommand HelpCommand { get; set; }
      public ICommand CheckLastReleasesCommand { get; set; }
      public ICommand AboutCommand { get; set; }

      public ICommand AddCommand { get; set; }

      private void saveCommandExecute() {
         var hotKey = _hotKeyManager.GetHotkey("toggle");

         var data = new AppSaveData(_smileCollection.Items) {
            Key = hotKey.Return(i => i.Key, Key.N),
            ModsKeys = hotKey.Return(h => h.Modifiers, ModifierKeys.Windows)
         };

         MainAppSaveLoader.Save(data);
      }

      private void exportCommandExecute() {
         throw new NotImplementedException();
      }

      private void importCommandExecute() {
         throw new NotImplementedException();
      }

      private void quitCommandExecute() {
         Application.Current.MainWindow.Close();
      }


      private void StartupCommandExecute(bool? b) {
         _shortcutCreator.SetShortcut(b ?? false, Environment.SpecialFolder.Startup, "-minimized");
      }

      private void showHelpCommandExecute() {
         var hotKey = _hotKeyManager.GetHotkey("toggle");

         var sb = new StringBuilder();
         sb.AppendLine($"To toggle visibility of context menu with smiles press hotkey combination ({HotKey.ConstructHotkeyText(hotKey)})")
            .AppendLine()
            .AppendLine("To remove item immediatly hold Shift modifier and click red button next to item");

         MessageBox.Show(sb.ToString(), "Help", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      private void checkLastReleasesExecute() {
         UrlHelper.OpenUrl(UrlHelper.RELEASES);
      }

      private void aboutCommandExecute() {
         var aboutView = new AboutView();
         aboutView.ShowDialog();
      }


      private void addCommandExecute() {
         _smileCollection.AddSmile();
      }
   }
}