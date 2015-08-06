using System.Windows;
using BegawkEditorUtilities;
using GlobalHotKey;
using Microsoft.Practices.Unity;

namespace textsmile.net {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      private static UnityContainer _container;

      public static UnityContainer Container
      {
         get { return _container; }
         private set { _container = value; }
      }

      private void AppStartup(object sender, StartupEventArgs e) {
         //ConsoleManager.Show();
         _container = new UnityContainer();
         _container.RegisterType<HotKeyManager>(new ContainerControlledLifetimeManager());
      }
   }
}
