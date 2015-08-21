using System;
using System.Monads;
using System.Windows.Input;
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
      private readonly IShortcutCreator _shortcutCreator;
      private readonly ConfigurationViewModelCommands _commands;

      public string HotkeyText {
         get { return _hotkeyText; }
         set { SetProperty(ref _hotkeyText, value); }
      }

      public ConfigurationViewModel() {
         //todo: constructor injection?
         _hotkeyManager = App.Container.Resolve<HotKeyManager>();
         SmileCollection = App.Container.Resolve<SmileCollection>();
         _shortcutCreator = App.Container.Resolve<IShortcutCreator>();
         _commands = App.Container.Resolve<ConfigurationViewModelCommands>();
      }

      public bool IsRunOnStartup {
         get { return _isRunOnStartup; }
         private set { SetProperty(ref _isRunOnStartup, value); }
      }

      public SmileCollection SmileCollection { get; }

      public ConfigurationViewModelCommands Commands => _commands;

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
         if (key == Key.Escape && mods == ModifierKeys.None) {
            return;
         }

         _hotkeyManager.Unregister("toggle");
         var hotKey = new HotKey(key, mods);
         _hotkeyManager.Register("toggle", hotKey);
         updateHotkeyText();
      }
   }
}
