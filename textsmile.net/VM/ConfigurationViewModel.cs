using System;
using System.Monads;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model.Shortcut;
using textsmile.net.Model.Smile;

namespace textsmile.net.VM {
   public sealed class ConfigurationViewModel : BindableBase, IVM {
      private readonly HotKeyManager _hotkeyManager;
      private string _hotkeyText;
      private bool _isRunOnStartup;
      private IShortcutCreator _shortcutCreator;

      public string HotkeyText {
         get { return _hotkeyText; }
         set { SetProperty(ref _hotkeyText, value); }
      }

      public ConfigurationViewModel() {
         AddCommand = new DelegateCommand(addCommandExecute);
         HelpCommand = new DelegateCommand(showHelpCommandExecute);
         QuitCommand = new DelegateCommand(quitCommandExecute);
         RunOnStartupCommand = new DelegateCommand<bool?>(StartupCommandExecute);

         //todo: constructor injection?
         _hotkeyManager = App.Container.Resolve<HotKeyManager>();
         SmileCollection = App.Container.Resolve<SmileCollection>();
         _shortcutCreator = App.Container.Resolve<IShortcutCreator>();
      }

      private void addCommandExecute() {
         SmileCollection.AddSmile();
      }

      private void showHelpCommandExecute() {
         var hotKey = _hotkeyManager.GetHotkey("toggle");

         var sb = new StringBuilder();
         sb.AppendLine($"To toggle visibility of context menu with smiles press hotkey combination ({HotKey.ConstructHotkeyText(hotKey)})")
            .AppendLine()
            .AppendLine("To remove item immediatly hold Shift modifier and click red button next to item");

         MessageBox.Show(sb.ToString(), "Help", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      private void quitCommandExecute() {
         Application.Current.MainWindow.Close();
      }

      private void StartupCommandExecute(bool? b) {
         _shortcutCreator.SetShortcut(b ?? false, Environment.SpecialFolder.Startup, "-minimized");
      }

      public ICommand AddCommand { get; set; }
      public ICommand HelpCommand { get; set; }
      public ICommand QuitCommand { get; set; }
      public ICommand RunOnStartupCommand { get; set; }

      public bool IsRunOnStartup {
         get { return _isRunOnStartup; }
         private set { SetProperty(ref _isRunOnStartup, value); }
      }

      public SmileCollection SmileCollection { get; }

      public void OnLoad() {
         _hotkeyManager.KeyRegistered += onHotkeyRegistered;
         _hotkeyManager.KeyUnregistered += onHotkeyUnregistered;
         
         updateHotkeyText();
         IsRunOnStartup = _shortcutCreator.CheckShortcut(Environment.SpecialFolder.Startup);
      }

      public void OnClosing() {
      }

      public void OnClosed() {
         _hotkeyManager.KeyRegistered -= onHotkeyRegistered;
         _hotkeyManager.KeyUnregistered -= onHotkeyUnregistered;
      }

      private void onHotkeyRegistered(object sender, HotkeyEventArgs e) {
         if (e.Label == "toggle") {
            updateHotkeyText();
         }
      }

      private void onHotkeyUnregistered(object sender, HotkeyEventArgs e) {
         if (e.Label == "toggle") {
            updateHotkeyText();
         }
      }

      private void updateHotkeyText() {
         var hotKey = _hotkeyManager.GetHotkey("toggle");
         HotkeyText = hotKey.Return(h => h.ToString(), "No");
      }

      public void SetHotkey(Key key, ModifierKeys mods) {
         _hotkeyManager.Unregister("toggle");
         var hotKey = new HotKey(key, mods);
         _hotkeyManager.Register("toggle", hotKey);
         updateHotkeyText();
      }
   }
}
