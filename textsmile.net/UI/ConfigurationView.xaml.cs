using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using textsmile.net.Model;
using textsmile.net.VM;

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

      public void ToggleVisibility() {
         Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
         if (Visibility == Visibility.Visible) {
            WindowState = WindowState.Normal;
            Activate();
            Focus();
         }
      }

      protected override void OnSourceInitialized(EventArgs e) {
         base.OnSourceInitialized(e);
         var source = (HwndSource)PresentationSource.FromVisual(this);
         source?.AddHook(WndProc);
      }

      private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
         if (msg == SingleInstance.WM_SHOWFIRSTINSTANCE) {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            Focus();
         }

         return IntPtr.Zero;
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

      private void onWindowStateChanged(object sender, EventArgs e) {
         if (WindowState == WindowState.Minimized) {
            Visibility = Visibility.Hidden;
         }
         if (WindowState == WindowState.Normal) {
            Visibility = Visibility.Visible;
         }
      }
   }
}
