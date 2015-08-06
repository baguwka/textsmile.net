using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalHotKey;
using textsmile.net.VM;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace textsmile.net.UI {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      private NotifyIcon notify;
      public IVM GetViewModel => (IVM)DataContext;
      public MainWindowVM GetMainWindowVM => (MainWindowVM)DataContext;

      public MainWindow() {
         InitializeComponent();
         //ElementHost.EnableModelessKeyboardInterop(this);
      }

      private void hotkeyManagerOnKeyPressed(object sender, KeyPressedEventArgs e) {
         toggleVisibility();

         Left = System.Windows.Forms.Cursor.Position.X + 20;
         Top = (System.Windows.Forms.Cursor.Position.Y - Height) + 20;
      }

      /// <summary>
      /// Manufactures the notification icon that lives for a very long time.
      /// </summary>
      private void CreateNotifyIcon() {
         notify = new NotifyIcon {
            Text = @"Textsmile.net",
            Icon = Properties.Resources.icon16x16,
            ContextMenu = new WindowContextMenu((sender, args) => {
               Close();
            })
         };

         notify.DoubleClick += OnNotifyDoubleClick;
         notify.Visible = true;
      }

      private void OnNotifyDoubleClick(object sender, EventArgs e) {
         toggleVisibility();
      }

      private void toggleVisibility() {
         Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
         if (Visibility == Visibility.Visible) {
            Activate();
            Focus();
         }
      }

      private void onLoaded(object sender, RoutedEventArgs e) {
         Debug.WriteLine("MainWindow.cs loaded");
         GetViewModel.OnLoad();
         CreateNotifyIcon();
         GetMainWindowVM.HotkeyPressed += hotkeyManagerOnKeyPressed;
         Visibility = Visibility.Hidden;
         HotkeyTextBox.Text = constructHotkeyText(GetMainWindowVM.GetHotkey().Key, GetMainWindowVM.GetHotkey().Modifiers);
      }

      protected override void OnClosing(CancelEventArgs e) {
         Debug.WriteLine("Mainwindow.cs closing");
         GetViewModel.OnClosing();
         GetMainWindowVM.HotkeyPressed -= hotkeyManagerOnKeyPressed;
         base.OnClosing(e);
      }

      protected override void OnClosed(EventArgs e) {
         GetViewModel.OnClosed();
         base.OnClosed(e);
         notify.Visible = false;
         notify = null;
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

         HotkeyTextBox.Text = constructHotkeyText(key, mods);
         GetMainWindowVM.SetHotkey(key, mods);
      }

      private string constructHotkeyText(Key key, ModifierKeys mods) {
         var sb = new StringBuilder(32);
         if ((mods & ModifierKeys.Control) != 0) {
            sb.Append("Ctrl+");
         }
         if ((mods & ModifierKeys.Shift) != 0) {
            sb.Append("Shift+");
         }
         if ((mods & ModifierKeys.Alt) != 0) {
            sb.Append("Alt+");
         }
         if ((mods & ModifierKeys.Windows) != 0) {
            sb.Append("Win+");
         }

         sb.Append(key.ToString());

         return sb.ToString();
      }

      private void OnWindowDeactivated(object sender, EventArgs e) {
         Hide();
      }
   }
}
