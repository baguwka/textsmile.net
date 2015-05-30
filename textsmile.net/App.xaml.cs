using System.Windows;
using BegawkEditorUtilities;

namespace textsmile.net {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      private void AppStartup(object sender, StartupEventArgs e) {
         ConsoleManager.Show();
      }
   }
}
