using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model;
using textsmile.net.VM;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using WindowState = System.Windows.WindowState;

namespace textsmile.net.UI {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class ConfigurationView : Window {
      public IVM GetViewModel => (IVM)DataContext;
      public ConfigurationViewModel GetConfigurationViewModel => (ConfigurationViewModel)DataContext;

      public ConfigurationView() {
         InitializeComponent();
      }

      //private void hotkeyManagerOnKeyPressed(object sender, KeyPressedEventArgs e) {

      //   var context = new MainContextMenu(GetConfigurationViewModel.Items, (o, args) => {
      //      WindowState = WindowState.Normal;
      //   });

      //   context.Visibility = Visibility.Visible;
      //   //context.Show(control, 
      //   //   new System.Drawing.Point(
      //   //      System.Windows.Forms.Cursor.Position.X + 20,
      //   //      (int)((System.Windows.Forms.Cursor.Position.Y - Height)) + 20));

      //   ToggleWindowState();

      //   //Left = System.Windows.Forms.Cursor.Position.X + 20;
      //   //Top = (System.Windows.Forms.Cursor.Position.Y - Height) + 20;
      //}

      private void OnNotifyDoubleClick(object sender, EventArgs e) {
         ToggleWindowState();
      }

      public void ToggleWindowState() {
         WindowState = WindowState == WindowState.Normal ? WindowState.Minimized : WindowState.Normal;
         if (WindowState == WindowState.Normal) {
            Activate();
            Focus();
         }
      }


      private void onLoaded(object sender, RoutedEventArgs e) {
         this.HideMaximizeButton();

         GetViewModel.OnLoad();
         //CreateNotifyIcon();
      }

      protected override void OnClosing(CancelEventArgs e) {
         var result = MessageBox.Show("Are you sure you want to quit the app?", "Closing", MessageBoxButton.YesNo,
            MessageBoxImage.Question, MessageBoxResult.No);

         if (result == MessageBoxResult.Yes) {
            GetViewModel.OnClosing();
            base.OnClosing(e);
         }
         else {
            e.Cancel = true;
         }
      }

      protected override void OnClosed(EventArgs e) {
         GetViewModel.OnClosed();
         base.OnClosed(e);
      }

      private void Hotkey_PreviewKeydDown(object sender, KeyEventArgs e) {
         // The text box grabs all input.
         e.Handled = true;

         // Fetch the actual shortcut key.
         var key = (e.Key == Key.System ? e.SystemKey : e.Key);

         // Ignore modifier keys.
         if (key == Key.LeftShift || key == Key.RightShift
             || key == Key.LeftCtrl || key == Key.RightCtrl
             || key == Key.LeftAlt || key == Key.RightAlt
             || key == Key.LWin || key == Key.RWin) {
            return;
         }

         var mods = ModifierKeys.None;

         if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
            mods = mods | ModifierKeys.Control;
         }
         if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) {
            mods = mods | ModifierKeys.Shift;
         }
         if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) {
            mods = mods | ModifierKeys.Alt;
         }
         if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) {
            mods = mods | ModifierKeys.Windows;
         }

         GetConfigurationViewModel.SetHotkey(key, mods);
      }
   }
}
