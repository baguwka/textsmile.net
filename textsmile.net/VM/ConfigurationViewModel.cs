using System;
using System.Diagnostics;
using System.Monads;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model.Smile;

namespace textsmile.net.VM {
   public sealed class ConfigurationViewModel : BindableBase, IVM {
      private readonly HotKeyManager _hotkeyManager;
      private readonly SmileCollection _smileCollection;
      private string _hotkeyText;

      public string HotkeyText {
         get { return _hotkeyText; }
         set { SetProperty(ref _hotkeyText, value); }
      }

      public ConfigurationViewModel() {
         AddCommand = new DelegateCommand(addCommandExecute);
         HelpCommand = new DelegateCommand(showHelpCommandExecute);

         _hotkeyManager = App.Container.Resolve<HotKeyManager>();

         _smileCollection = App.Container.Resolve<SmileCollection>();
      }

      private void addCommandExecute() {
         _smileCollection.AddSmile();
      }

      private void showHelpCommandExecute() {
         var hotKey = _hotkeyManager.GetHotkey("toggle");

         var sb = new StringBuilder();
         sb.Append(
            $"To toggle visibility of window press hotkey combination ({HotKey.ConstructHotkeyText(hotKey.With(h => h.Key), hotKey.With(h => h.Modifiers))})")
            .AppendLine()
            .AppendLine()
            .Append("To edit item hold Ctrl modifier on your keyboard and click to that item")
            .AppendLine()
            .AppendLine()
            .Append("To remove item immediatly hold Shift modifier and click red button next to item")
            .AppendLine();

         MessageBox.Show(sb.ToString(), "Help", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      public ICommand AddCommand { get; set; }
      public ICommand HelpCommand { get; set; }

      public SmileCollection SmileCollection {
         get { return _smileCollection; }
      }

      public void OnLoad() {
         _hotkeyManager.KeyRegistered += onHotkeyRegistered;
         _hotkeyManager.KeyUnregistered += onHotkeyUnregistered;

         Debug.WriteLine("show" + DateTime.Now);
         updateHotkeyText();
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
