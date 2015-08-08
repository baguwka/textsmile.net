using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalHotKey;
using textsmile.net.Model;
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

         var context = new MainContextMenu(GetMainWindowVM.Items, (o, args) => {
            WindowState = WindowState.Normal;
         });

         context.Visibility = Visibility.Visible;
         //context.Show(control, 
         //   new System.Drawing.Point(
         //      System.Windows.Forms.Cursor.Position.X + 20,
         //      (int)((System.Windows.Forms.Cursor.Position.Y - Height)) + 20));

         toggleWindowState();

         //Left = System.Windows.Forms.Cursor.Position.X + 20;
         //Top = (System.Windows.Forms.Cursor.Position.Y - Height) + 20;
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
         toggleWindowState();
      }

      private void toggleWindowState() {
         WindowState = WindowState == WindowState.Normal ? WindowState.Minimized : WindowState.Normal;
         if (WindowState == WindowState.Normal) {
            Activate();
            Focus();
         }
      }

      private void onLoaded(object sender, RoutedEventArgs e) {
         this.HideMinimizeAndMaximizeButtons();

         Debug.WriteLine("MainWindow.cs loaded");
         GetViewModel.OnLoad();
         CreateNotifyIcon();
         GetMainWindowVM.HotkeyPressed += hotkeyManagerOnKeyPressed;

         HotkeyTextBox.Text = GetMainWindowVM.ConstructHotkeyText(GetMainWindowVM.GetHotkey().Key, GetMainWindowVM.GetHotkey().Modifiers);
      }

      protected override void OnClosing(CancelEventArgs e) {
         Debug.WriteLine("Mainwindow.cs closing");
         GetViewModel.OnClosing();
         GetMainWindowVM.HotkeyPressed -= hotkeyManagerOnKeyPressed;
         base.OnClosing(e);
      }

      protected override void OnClosed(EventArgs e) {
         GetViewModel.OnClosed();
         notify.With(n => n.Visible = false);
         notify = null;
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

         HotkeyTextBox.Text = GetMainWindowVM.ConstructHotkeyText(key, mods);
         GetMainWindowVM.SetHotkey(key, mods);
      }
   }
}
