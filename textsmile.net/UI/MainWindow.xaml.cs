using System;
using System.ComponentModel;
using System.Windows;
using textsmile.net.VM;

namespace textsmile.net.UI {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public IVM GetViewModel {
         get { return (IVM)DataContext; }
      }

      public MainWindow() {
         InitializeComponent();
         Loaded += onLoaded;
      }

      private void onLoaded(object sender, RoutedEventArgs routedEventArgs) {
         GetViewModel.OnLoad();
      }

      protected override void OnClosing(CancelEventArgs e) {
         GetViewModel.OnClosing();
         base.OnClosing(e);
      }

      protected override void OnClosed(EventArgs e) {
         GetViewModel.OnClosed();
         base.OnClosed(e);
      }

      private void onExitClick(object sender, RoutedEventArgs e) {
         var result = MessageBox.Show("Are you sure you want to quit?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question,
            MessageBoxResult.No);

         if (result == MessageBoxResult.Yes) {
            Close();
         }
      }
   }
}
